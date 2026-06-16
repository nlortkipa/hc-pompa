using Dal.Interfaces;
using Dal.Repositories;
using Data;using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
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
                options.UseSqlServer(builder.Configuration.GetConnectionString("PompaDBConnection"));
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

                        // 🔥 Tell ASP.NET which claim to use for roles
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

            // Add services to the container
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ISmsCodeService, SmsCodeService>();
            builder.Services.AddScoped<ICompanyService, CompanyService>();
            builder.Services.AddScoped<IPumpOperationService, PumpOperationService>();

            builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpClient();

            builder.Services.AddControllers();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ProjectContext>();
                    await context.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while creating or migrating the database.");
                }
            }
            // -----------------------------------------

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
            app.MapControllers();

            await app.RunAsync();
        }
    }
}