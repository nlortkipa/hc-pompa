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

        public async Task SendPumpOperationReviewRequestAsync(PumpOperationDto operation, string checkerEmail, string reviewUrl)
        {
            var subject = $"Pump operation #{operation.Id} review request";
            var body = BuildReviewBody(operation, reviewUrl);

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
                IsBodyHtml = false
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

        private static string BuildReviewBody(PumpOperationDto operation, string reviewUrl)
        {
            var operatorName = $"{operation.OperatorFirstName} {operation.OperatorLastName}".Trim();

            return string.Join(Environment.NewLine, new[]
            {
                "A pump operation is ready for checker review.",
                "",
                $"Operation ID: {operation.Id}",
                $"Date: {operation.Date:yyyy-MM-dd HH:mm}",
                $"Operator: {operatorName}",
                $"Company: {operation.CompanyName}",
                $"Address: {operation.AddressName}",
                $"Plate: {operation.PlateNumber}",
                $"Pump type: {operation.PumpType}",
                $"Distance range: {GetDistanceRangeLabel(operation)}",
                $"Concrete volume: {operation.ConcreteVolume}",
                $"Note: {(string.IsNullOrWhiteSpace(operation.Note) ? "—" : operation.Note)}",
                "",
                $"Review link: {reviewUrl}"
            });
        }

        private static string GetDistanceRangeLabel(PumpOperationDto operation)
        {
            if (!string.Equals(operation.PumpType, "<60მ", StringComparison.Ordinal))
            {
                return "—";
            }

            return operation.DistanceRange ?? "60მ-მდე";
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
