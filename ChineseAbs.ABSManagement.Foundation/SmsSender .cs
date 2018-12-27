using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.Foundation
{
    public class SmsSender
    {
        private SmsSender()
        {
        }

        private static SmsSender GetInstance()
        {
            if (m_sender == null)
            {
                lock (m_lockObj)
                {
                    if (m_sender == null)
                    {
                        m_sender = new SmsSender();
                        m_sender.Initialize();
                    }
                }
            }

            return m_sender;
        }

        private SmsConfigOption ParseSmsSetting()
        {
            var config = System.Configuration.ConfigurationManager.AppSettings["SmsConfig"];
            return Newtonsoft.Json.JsonConvert.DeserializeObject<SmsConfigOption>(config);
        }

        private void Initialize()
        {
            Log.Info("启动短信服务...");
            // m_mailConfig = ParseSmsSetting();

            m_queue = new ConcurrentQueue<SmsMessage>();
            m_thread = new System.Threading.Thread(Run)
            {
                IsBackground = true,
                Name = "ThreadSmsSender"
            };

            m_running = true;
            m_thread.Start();
        }

        public SmsMessage CreateSmsMessage(string smsTo, string codename, params string[] smsContent)
        {
            var sms = new SmsMessage();
            string code = SmsConfigOption.Current.Templates[codename];
            sms.To = smsTo;
            sms.TemplateId = code;
            sms.Datas.AddRange(smsContent);
            return sms;
        }

        public static void SendAsync(string smsTo, string codename, params string[] smsContent)
        {
            GetInstance().EnqueueSms(smsTo, codename, smsContent);
        }

        private void EnqueueSms(string smsTo, string codename, params string[] smsContent)
        {
            m_queue.Enqueue(CreateSmsMessage(smsTo, codename, smsContent));
        }

        private void Run()
        {
            Log.Info("短信服务已启动");
            while (true)
            {
                SmsMessage sms;
                if (m_queue.TryDequeue(out sms))
                {
                    try
                    {
                        Log.Info("正在发送短信到：" + sms.To + "...");
                        new Sender().Send(sms);
                    }
                    catch (Exception e)
                    {
                        Log.Error("发送短信[" + sms.To + "]失败", e);
                    }
                }
                else
                {
                    System.Threading.Thread.Sleep(500);
                }

                if (!m_running && m_queue.Count == 0)
                {
                    break;
                }
            }
        }

        public static void StopService()
        {
            GetInstance().Stop();
        }

        private void Stop()
        {
            Log.Info("停止短信服务...");
            if (!m_running)
            {
                System.Threading.Thread.Sleep(5000);
            }

            m_running = false;
            if (m_thread != null)
            {
                m_thread.Join();
            }
            Log.Info("短信服务已停止");
        }

        #region 变量

        private bool m_running = false;

        private System.Threading.Thread m_thread;
        private ConcurrentQueue<SmsMessage> m_queue;

        private static SmsSender m_sender;
        private static Object m_lockObj = new object();

        private SmsConfigOption m_mailConfig;
        #endregion

        #region 属性
        public static string DisplayAccount { get; set; }
        #endregion
    }

}
