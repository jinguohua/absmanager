using ChineseAbs.FrameworkLogic;
using ChineseAbs.FrameworkLogic.Message.Email;
using System;
using System.Collections.Concurrent;

namespace ChineseAbs.ABSManagement.Reminder
{
    public class MailSender
    {
        public MailSender()
        {
            m_queue = new ConcurrentQueue<EmailMessageBody>();
            m_thread = new System.Threading.Thread(Run) { IsBackground = true };
            m_thread.Start();
        }

        public void SendAsync(EmailMessageBody emailMsgBody)
        {
            m_queue.Enqueue(emailMsgBody);
        }

        private string GetExceptionMsg(string actionName, Exception e)
        {
            var errMsg = "[" + actionName + "] " + System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff")
                + System.Environment.NewLine
                + "[Exception-Message]"
                + e.Message
                + System.Environment.NewLine
                + "[Exception-StackTrace]"
                + e.StackTrace
                + System.Environment.NewLine;
            return errMsg;
        }

        private void Run()
        {
            m_mailService = EmailSenderServiceFactory.CreateLocalEmailSenderService();
            while (true)
            {
                EmailMessageBody emailMsgBody;
                if (m_queue.TryDequeue(out emailMsgBody))
                {
                    try
                    {
                        m_mailService.Send(emailMsgBody);
                    }
                    catch(Exception e)
                    {
                        var errMsg = GetExceptionMsg("发送邮件失败", e);
                        System.Console.WriteLine(errMsg);

                        try
                        {
                            TxtLogHelper.WriteLog(errMsg);
                        }
                        catch (Exception logException)
                        {
                            var logErrMsg = GetExceptionMsg("写Log文件失败", logException);
                            System.Console.WriteLine(logErrMsg);
                        }
                    }
                }
                else
                {
                    System.Threading.Thread.Sleep(500);
                }
            }
        }

        private System.Threading.Thread m_thread;
        private IEmailSenderService m_mailService;
        private ConcurrentQueue<EmailMessageBody> m_queue;
    }
}
