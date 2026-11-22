using Microsoft.Extensions.Configuration;
using Service.DTOs;
using Service.Interfaces;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly bool _sendEmail;

        public EmailService(IConfiguration configuration)
        {
            _sendEmail = !bool.TryParse(configuration["Email:SendEmail"], out var sendEmail) || sendEmail;
        }

        public async Task SendPumpOperationReviewEmailAsync(ReviewEmailKind kind, PumpOperationDto operation, string checkerEmail, string reviewUrl)
        {
            var subject = BuildReviewSubject(kind, operation);
            var body = BuildReviewBody(kind, operation, reviewUrl);

            if (!_sendEmail)
            {
                Console.WriteLine($"[EMAIL DEV] {checkerEmail} -> {subject}");
                Console.WriteLine($"[EMAIL DEV] Review URL: {reviewUrl}");
                return;
            }

            await SendEmailAsync(checkerEmail, subject, body);
        }

        private static async Task SendEmailAsync(string to, string subject, string body)
        {
            var host = GetRequiredEnv("SMTP_HOST");
            var port = GetRequiredIntEnv("SMTP_PORT");
            var fromEmail = GetRequiredEnv("SMTP_FROM_EMAIL");
            var fromName = Environment.GetEnvironmentVariable("SMTP_FROM_NAME");
            var username = Environment.GetEnvironmentVariable("SMTP_USERNAME");
            var password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
            var enableSsl = GetOptionalBoolEnv("SMTP_ENABLE_SSL", true);

            using var message = new MailMessage
            {
                From = string.IsNullOrWhiteSpace(fromName)
                    ? new MailAddress(fromEmail)
                    : new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
                SubjectEncoding = System.Text.Encoding.UTF8,
                BodyEncoding = System.Text.Encoding.UTF8
            };
            message.To.Add(to);

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl
            };

            if (!string.IsNullOrWhiteSpace(username))
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    throw new Exception("SMTP_PASSWORD Not Set.");
                }

                client.Credentials = new NetworkCredential(username, password);
            }

            try
            {
                await client.SendMailAsync(message);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }

        private static string BuildReviewSubject(ReviewEmailKind kind, PumpOperationDto operation) => kind switch
        {
            ReviewEmailKind.Updated => $"ჩანაწერი #{operation.Id} შეიცვალა",
            ReviewEmailKind.Deleted => $"ჩანაწერი #{operation.Id} წაიშალა",
            _ => $"ჩანაწერი #{operation.Id} შეიქმნა"
        };

        private static string BuildReviewBody(ReviewEmailKind kind, PumpOperationDto operation, string reviewUrl)
        {
            var message = kind switch
            {
                ReviewEmailKind.Updated => $"ჩანაწერი #{operation.Id} შეიცვალა. გთხოვთ, შეამოწმოთ.",
                ReviewEmailKind.Deleted => $"ჩანაწერი #{operation.Id} წაიშალა.",
                _ => $"ჩანაწერი #{operation.Id} შეიქმნა. გთხოვთ, შეამოწმოთ."
            };

            return string.Join(Environment.NewLine, message, "", reviewUrl);
        }

        private static string GetRequiredEnv(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new Exception($"{name} Not Set.");
            }

            return value;
        }

        private static int GetRequiredIntEnv(string name)
        {
            var value = GetRequiredEnv(name);
            if (!int.TryParse(value, out var number))
            {
                throw new Exception($"{name} is not valid.");
            }

            return number;
        }

        private static bool GetOptionalBoolEnv(string name, bool defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(name);
            return bool.TryParse(value, out var result) ? result : defaultValue;
        }
    }
}
