using ChineseAbs.ABSManagement.Foundation;
using Nancy;
using Newtonsoft.Json;

namespace ChineseAbs.ABSManagement.ReminderHost
{
    public static class NancyUtils
    {
        public static Response Success(object obj)
        {
            var result = new NancyResult
            {
                Status = NancyResponseStatus.Success,
                Message = "Success",
                Content = JsonConvert.SerializeObject(obj),
            };

            var response = (Response)(JsonConvert.SerializeObject(result));
            response.ContentType = "application/json";
            return response;
        }

        public static Response Error(string errorMsg, string url, string stackTrace)
        {
            var result = new NancyResult
            {
                Status = NancyResponseStatus.Failed,
                Message = errorMsg,
                Content = string.Empty,
                Url = url,
                StackTrace = stackTrace,
            };

            var response = (Response)(JsonConvert.SerializeObject(result));
            response.ContentType = "application/json; charset=utf-8";
            return response;
        }
    }
}
