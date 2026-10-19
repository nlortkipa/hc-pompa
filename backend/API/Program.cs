using Dal.Interfaces;
using Dal.Repositories;
using Data;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using Service.Helpers;
using Service.Mapping;
using Service.Services;
using System.Security.Claims;
using System.Text;

namespace API 
{
    public class Program
    {
        public static async Task Main(string[] args) // Changed void to async Task for non-blocking migration
        {
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });

            builder.Services.AddDbContext<ProjectContext>(options =>
            {
                var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
                if (!string.IsNullOrWhiteSpace(databaseUrl))
                {
                    options.UseNpgsql(
                        Data.PostgresMigrations.PostgresConnectionString.Normalize(databaseUrl),
                        npgsql => npgsql.MigrationsAssembly("Data.PostgresMigrations"));
                }
                else
                {
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString("PompaDBConnection")
                        ?? throw new InvalidOperationException("PompaDBConnection must be configured."));
                }
            });
            builder.Services.AddScoped<DbContext, ProjectContext>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY") ??
                                throw new Exception("JWT_KEY not set"))),

                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,

                        // Use the same role claim type that TokenHelper emits.
                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.NameIdentifier
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.ContainsKey("Token"))
                            {
                                context.Token = context.Request.Cookies["Token"];
                            }

                            if (string.IsNullOrEmpty(context.Token) &&
                                context.Request.Headers.ContainsKey("Authorization"))
                            {
                                var authHeader = context.Request.Headers["Authorization"].ToString();
                                if (authHeader.StartsWith("Bearer "))
                                {
                                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                                }
                            }

                            return Task.CompletedTask;
                        },
                        OnTokenValidated = async context =>
                        {
                            var userIdClaim = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                            var tokenVersionClaim = context.Principal?.FindFirstValue(TokenHelper.TokenVersionClaim);

                            if (!int.TryParse(userIdClaim, out var userId) ||
                                !int.TryParse(tokenVersionClaim, out var tokenVersion))
                            {
                                context.Fail("Invalid token.");
                                return;
                            }

                            var db = context.HttpContext.RequestServices.GetRequiredService<ProjectContext>();
                            var currentTokenVersion = await db.Users
                                .Where(user => user.Id == userId)
                                .Select(user => (int?)user.TokenVersion)
                                .FirstOrDefaultAsync();

                            if (currentTokenVersion == null || currentTokenVersion.Value != tokenVersion)
                            {
                                context.Fail("Token has been invalidated.");
                            }
                        }
                    };
                });

            // Add repositories to the container
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ISmsCodeRepository, SmsCodeRepository>();
            builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
            builder.Services.AddScoped<IAddressRepository, AddressRepository>();
            builder.Services.AddScoped<ICompanyAddressRepository, CompanyAddressRepository>();
            builder.Services.AddScoped<IPumpOperationRepository, PumpOperationRepository>();
            builder.Services.AddScoped<IPlateRepository, PlateRepository>();
            builder.Services.AddScoped<IPumpOperationReviewRequestRepository, PumpOperationReviewRequestRepository>();

            // Add services to the container
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ISmsCodeService, SmsCodeService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ICompanyService, CompanyService>();
            builder.Services.AddScoped<IPumpOperationService, PumpOperationService>();
            builder.Services.AddScoped<IPlateService, PlateService>();

            builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpClient();

            builder.Services.AddControllers();

            var app = builder.Build();

            // Apply pending migrations at startup. RecreateDatabaseOnStartup remains available
            // for disposable local databases, but production never deletes existing data.
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ProjectContext>();
                    if (builder.Configuration.GetValue<bool>("RecreateDatabaseOnStartup"))
                    {
                        await context.Database.EnsureDeletedAsync();
                    }

                    await context.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while applying database migrations.");
                    throw;
                }
            }

            app.UseCors("AllowAll");

            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async context =>
                {
                    var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
                    if (exception is Service.Exceptions.ConflictException)
                    {
                        context.Response.StatusCode = 409;
                        await context.Response.WriteAsync(exception.Message);
                    }
                    else
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An error occurred.");
                    }
                });
            });

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapGet("/api/health", async (ProjectContext db) =>
            {
                var databaseAvailable = await db.Database.CanConnectAsync();
                return Results.Ok(new
                {
                    status = databaseAvailable ? "ok" : "degraded",
                    database = databaseAvailable ? "connected" : "unavailable"
                });
            });
            app.MapControllers();

            await app.RunAsync();
        }
    }
}
