using ChineseAbs.ABSManagement.Foundation;
using Nancy;

namespace ChineseAbs.ABSManagement.ReminderHost
{
    public class ReminderHostModule : NancyModule
    {
        public ReminderHostModule()
            : base("Reminder")
        {
            Post["/SendEmail"] = _ =>
            {
                var emailTo = this.Request.Query["emailTo"];
                var emailContent = this.Request.Query["emailContent"];
                var title = this.Request.Query["title"];

                if (string.IsNullOrWhiteSpace(emailTo)
                    || emailContent == null
                    || title == null)
                {
                    var errorMsg = "SendEmail failed:emailTo=" + emailTo
                        + ";emailContent=" + emailContent + ";title=" + title;
                    return NancyUtils.Error(errorMsg, this.Request.Url, "");
                }

                MailSender.SendAsync(emailTo, emailContent, title);
                return NancyUtils.Success("OK");
            };
        }
    }
}
