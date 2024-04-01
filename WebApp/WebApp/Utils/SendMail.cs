using System.Net.Mail;
using System.Net.Mime;
using System.Net;

namespace WebApp.Utils
{
    public class SendMail
    {
        static IConfiguration conf = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());

        public static void Send(EmailDTO emailDTO)
        {
            String[] emailTo = emailDTO.To.Split(';');

            emailTo = emailTo.Where(val => !String.IsNullOrEmpty(val)).ToArray();

            String[] emailCc = null;
            if (!String.IsNullOrEmpty(emailDTO.CC))
            {
                emailCc = emailDTO.CC.Split(';');
            }

            using (SmtpClient client = new SmtpClient(conf["MailSettings:SMTP_Server"].ToString()))
            {
                client.Port = Int32.Parse(conf["MailSettings:SMTP_Port"].ToString());

                if (conf["MailSettings:SMTP_Server"].ToString().IndexOf("gmail") > 0)
                {
                    client.Credentials = new NetworkCredential(conf["MailSettings:User_Gmail"].ToString(), conf["MailSettings:Pass_Gmail"].ToString());
                    client.EnableSsl = true;
                }
                else
                {
                    ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                    client.EnableSsl = false;
                }

                MailMessage message = new MailMessage();
                message.IsBodyHtml = true;

                if (string.IsNullOrEmpty(emailDTO.From))
                    message.From = new MailAddress(conf["MailSettings:SMTP_Mail_From"].ToString());
                else
                    message.From = new MailAddress(emailDTO.From);

                foreach (string to in emailTo)
                {
                    message.To.Add(new MailAddress(to));
                }

                if (emailCc != null)
                {
                    foreach (string cc in emailCc)
                    {
                        message.CC.Add(new MailAddress(cc));
                    }
                }

                message.Subject = emailDTO.Subject;
                message.Body = emailDTO.Content;

                if (emailDTO.AttachmentFile != null)
                {
                    foreach (string filePath in emailDTO.AttachmentFile)
                    {
                        Attachment data = new Attachment(filePath, MediaTypeNames.Application.Pdf);
                        message.Attachments.Add(data);
                    }
                }

                client.Send(message);
                message.Dispose();
            }
        }
    }

    public class EmailDTO
    {
        public String Subject { get; set; }
        public String From { get; set; }
        public String To { get; set; }
        public String CC { get; set; }
        public String Content { get; set; }
        public IList<String> AttachmentFile;
    }
}
