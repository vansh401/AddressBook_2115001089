using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendResetEmail(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogError("Recipient email cannot be empty.");
                throw new ArgumentException("Recipient email cannot be empty", nameof(email));
            }

            try
            {
                // Read SMTP configuration
                var smtpHost = _configuration["SMTP:Host"];
                var smtpPort = int.Parse(_configuration["SMTP:Port"]);
                var smtpUser = _configuration["SMTP:Username"];
                var smtpPass = _configuration["SMTP:Password"];
                var enableSSL = bool.Parse(_configuration["SMTP:EnableSSL"]);

                if (string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrWhiteSpace(smtpPass))
                {
                    _logger.LogError("SMTP configuration is missing or incomplete.");
                    throw new InvalidOperationException("SMTP configuration is missing.");
                }

                // Construct reset link with localhost for testing
                string resetLink = $"https://localhost:7108/api/auth/resetpassword?token={token}";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser),
                    Subject = "Password Reset Link",
                    Body = $@"
        <p>Click the link to reset your password: 
        <a href='{resetLink}'>Reset Password</a></p>
        <p>If the above link doesn't work, use this token directly in the API:</p>
        <p><strong>{token}</strong></p>",
                    IsBodyHtml = true,
                };



                mailMessage.To.Add(email);

                // Configure and send email
                using var smtpClient = new SmtpClient(smtpHost)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = enableSSL,
                };

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Password reset email sent to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send password reset email to {email}");
                throw;
            }
        }
    }
}
