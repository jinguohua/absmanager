using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace ChineseAbs.ABSManagement.Foundation
{
    public class SmsMessage
    {
        public SmsMessage()
        {
            ToList = new List<string>();
            Datas = new List<string>();
        }

        public string To
        {
            get
            {
                return String.Join(",", ToList.ToArray());
            }
            set
            {
                ToList = new List<string>(value.Split(','));
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public List<string> ToList { get; private set; }

        public string AppId { get { return SmsConfigOption.Current.AppID; } }

        public string TemplateId { get; set; }

        public List<string> Datas { get; set; }
    }

    public class SMSResponse
    {
        public string StatusCode { get; set; }

        public string StatusMsg { get; set; }
    }

    public class Sender
    {
        SmsMessage sendData;

        public Sender()
        {

        }

        public void Send(string to, string templateID, params string[] messages)
        {
            sendData = new SmsMessage();
            sendData.To = to;
            sendData.TemplateId = templateID;
            sendData.Datas.AddRange(messages);

            DateTime time = DateTime.Now;
            string url = SmsConfigOption.Current.GetServerURL(time);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Accept = "application/json";
            request.ContentType = "application/json;charset=utf-8";
            request.Headers.Add(HttpRequestHeader.Authorization, SmsConfigOption.Current.GetAuthorization(time));
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            string dataContent = JsonConvert.SerializeObject(sendData, Formatting.None, setting);
            request.ContentLength = UTF8Encoding.UTF8.GetByteCount(dataContent);
            request.Method = "POST";
            using (StreamWriter stream = new StreamWriter(request.GetRequestStream()))
            {
                stream.Write(dataContent);
            }
            WebResponse response = request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string content = reader.ReadToEnd();
                SMSResponse responseData = JsonConvert.DeserializeObject<SMSResponse>(content, setting);

                if (!String.IsNullOrEmpty(responseData.StatusMsg))
                    throw new SMSException("发送短信出现错误：" + responseData.StatusMsg);
            }

        }

        public void Send(SmsMessage sms)
        {
            sendData = sms;

            DateTime time = DateTime.Now;
            string url = SmsConfigOption.Current.GetServerURL(time);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Accept = "application/json";
            request.ContentType = "application/json;charset=utf-8";
            request.Headers.Add(HttpRequestHeader.Authorization, SmsConfigOption.Current.GetAuthorization(time));
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            string dataContent = JsonConvert.SerializeObject(sendData, Formatting.None, setting);
            request.ContentLength = UTF8Encoding.UTF8.GetByteCount(dataContent);
            request.Method = "POST";
            using (StreamWriter stream = new StreamWriter(request.GetRequestStream()))
            {
                stream.Write(dataContent);
            }
            WebResponse response = request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string content = reader.ReadToEnd();
                SMSResponse responseData = JsonConvert.DeserializeObject<SMSResponse>(content, setting);

                if (!String.IsNullOrEmpty(responseData.StatusMsg))
                    throw new SMSException("发送短信出现错误：" + responseData.StatusMsg);
            }

        }

    }
}
