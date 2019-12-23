using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTestLibrary.Services
{
    public class Email
    {
        public async Task SendEmailAsync(EmailRequest emailRequest)
        {
            var htmlContent = emailRequest.Body;

            var smtpClient = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = "smtp.gmail.com",
                Port = 587,
                // Timeout = _amazonOptions.TimeOut,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential("contionusintegration@gmail.com", "1234test!")
            };

            var message = new MailMessage()
            {
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure,
                Subject = emailRequest.Subject,
                Body = htmlContent,
                From = new MailAddress("contionusintegration@gmail.com")
            };

            foreach (var toAddress in emailRequest.ToEmails)
            {
                message.To.Add(toAddress);
            }

            await smtpClient.SendMailAsync(message);

            message.Dispose();
        }

        public void SendEmailWithMetricsAsync(Dictionary<DataStoreType, Dictionary<MetricsType, double>> response)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<table border=1><tr><b><td>Data Location </td>");

            foreach (string name in Enum.GetNames(typeof(MetricsType)))
            {
                sb.AppendLine($"<td>{name}</td>");
            }
            sb.Append("</b></tr>");
           
            foreach (var dataStoreType in response.Keys)
            {
                var metreics = response[dataStoreType];

                sb.Append($"<tr><td>{dataStoreType.ToString()}</td>");

                Console.WriteLine($"{Environment.NewLine}Metrics for datastor {dataStoreType.ToString()}");

                foreach (var metricsType in metreics.Keys)
                {
                    sb.Append($"<td>{metreics[metricsType]}</td>");
                    Console.WriteLine($" {metricsType.ToString()} : time in ms { metreics[metricsType]} ");
                }
                sb.Append($"</tr>");
            }

            sb.AppendLine("</table>");

            EmailRequest emailRequest = new EmailRequest()
            {
                Body = sb.ToString(),
                ToEmails = new[] { "mahesh.bailwal@rsystems.com" },
                Subject = "gggggg"
            };

            SendEmailAsync(emailRequest).Wait();
        }
    }

    public class EmailRequest
    {
        public IEnumerable<string> ToEmails { get; set; }
        public IEnumerable<string> CcEmails { get; set; }
        public string FromEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

    }
}
