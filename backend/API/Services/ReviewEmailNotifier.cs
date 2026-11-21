using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service.DTOs;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    /// <summary>Mails review links to checkers. A failed email never fails the request that saved the data.</summary>
    public class ReviewEmailNotifier(
        IEmailService emailService,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ReviewEmailNotifier> logger)
    {
        public async Task SendAsync(
            PumpOperationDto operation,
            IEnumerable<PumpOperationReviewRequestLinkDto> reviewRequests,
            bool warnIfNoChecker = true)
        {
            var reviewBaseUrl = GetReviewBaseUrl();
            var hasCheckerRequest = false;

            foreach (var reviewRequest in reviewRequests)
            {
                hasCheckerRequest = true;
                if (string.IsNullOrWhiteSpace(reviewRequest.CheckerGmail))
                {
                    // Checker has no email: they will see the request in the app after signing in.
                    continue;
                }

                var reviewUrl = $"{reviewBaseUrl}/{reviewRequest.Token}";
                try
                {
                    await emailService.SendPumpOperationReviewRequestAsync(operation, reviewRequest.CheckerGmail, reviewUrl);

                    logger.LogInformation(
                        "Pump operation {PumpOperationId} review email processed for checker {CheckerGmail}: {ReviewUrl}",
                        operation.Id,
                        reviewRequest.CheckerGmail,
                        reviewUrl);
                }
                catch (Exception ex)
                {
                    // The operation and review request are already persisted. An email-provider
                    // outage must not report the successful save as a failed operation.
                    logger.LogError(
                        ex,
                        "Pump operation {PumpOperationId} was saved, but the review email failed for checker {CheckerGmail}.",
                        operation.Id,
                        reviewRequest.CheckerGmail);
                }
            }

            if (!hasCheckerRequest && warnIfNoChecker)
            {
                logger.LogWarning("Pump operation {PumpOperationId} was submitted, but no checker is configured.", operation.Id);
            }
        }

        private string GetReviewBaseUrl()
        {
            var configuredUrl = configuration["ReviewBaseUrl"];
            if (!string.IsNullOrWhiteSpace(configuredUrl))
            {
                return configuredUrl.TrimEnd('/');
            }

            var productionHost = Environment.GetEnvironmentVariable("VERCEL_PROJECT_PRODUCTION_URL");
            if (!string.IsNullOrWhiteSpace(productionHost))
            {
                return $"https://{productionHost.TrimEnd('/')}/review";
            }

            var origin = httpContextAccessor.HttpContext?.Request.Headers.Origin.ToString();
            if (!string.IsNullOrWhiteSpace(origin))
            {
                return $"{origin.TrimEnd('/')}/review";
            }

            return "http://localhost:4200/review";
        }
    }
}
