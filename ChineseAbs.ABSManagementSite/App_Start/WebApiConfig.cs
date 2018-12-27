using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using SAFS.Utility.Web;
using SAFS.Utility.Extensions;
using ABS.ABSManagementSite;

namespace ChineseAbs.ABSManagementSite
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            //Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            SwaggerConfig.Register();

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            var jsonFormat = config.Formatters.JsonFormatter;
            var setting = jsonFormat.SerializerSettings;
            setting.Formatting = Newtonsoft.Json.Formatting.Indented;
            setting.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            setting.Converters.Add(new BoolConverter());
            setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";

            config.MessageHandlers.Add(new ApiHttpMessageHandler((request)=> {

                return request.GetRouteData().Route.Handler == null;
            }));
           // config.EnsureInitialized();
        }
    }

    public class BoolConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(int))
            {
                int v = Convert.ToInt32(existingValue);
                return v > 0;
            }
            else
                return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var flag = Convert.ToBoolean(value);
            writer.WriteValue(flag ? 1 : 0);
        }
    }

    public class ApiHttpMessageHandler : System.Net.Http.DelegatingHandler
    {
        Func<HttpRequestMessage, bool> m_conditions = null;

        public ApiHttpMessageHandler(Func<HttpRequestMessage, bool> conditions  = null)
        {
            m_conditions = conditions;
        }

        protected override async Task<HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if(m_conditions == null || m_conditions(request))
            {
                return BuildApiReponse(request, response);
            }
            else
            {
                return response;
            }
        }

        private static HttpResponseMessage BuildApiReponse(HttpRequestMessage request, HttpResponseMessage response)
        {
            object content;
            string errorMessage = null;
            if (response.TryGetContentValue(out content) && !response.IsSuccessStatusCode)
            {
                HttpError error = content as HttpError;
                if (error != null)
                {
                    content = null;
                    var log = log4net.LogManager.GetLogger("API");
                    foreach (var key in error)
                    {
                        log.Error(key.Key + ":" + key.Value.ToStringSafe());
                    }
                    errorMessage = error.ExceptionMessage;

                    if (String.IsNullOrEmpty(errorMessage))
                        errorMessage = error.Message;

                    errorMessage += GetInnerErrorMsg(error);
                    log.Error(errorMessage);
                }
            }

            if (response.Content is StreamContent)
            {
                return response;
            }
            Type contentType = content != null ? content.GetType() : null;
            if (contentType != null && contentType.IsGenericType && contentType.GetGenericTypeDefinition() == typeof(JsonResultDataEntity<>))
                return response;
            var wrapperData = new JsonResultDataEntity<Object>();
            wrapperData.IsAuthorized = response.StatusCode != System.Net.HttpStatusCode.Unauthorized;
            wrapperData.Code = ((int)response.StatusCode >= 200 && (int)response.StatusCode <= 300) ? 0 : 1;
            wrapperData.Message = errorMessage;
            wrapperData.Data = content;
            var newResponse = request.CreateResponse(System.Net.HttpStatusCode.OK, wrapperData);
            foreach (var header in response.Headers)
            {
                newResponse.Headers.Add(header.Key, header.Value);
            }
            return newResponse;
        }

        private static string GetInnerErrorMsg(HttpError error)
        {
            string errorMsgs = string.Empty;
            while (error.InnerException != null)
            {
                if (error.InnerException.ExceptionMessage != null)
                    errorMsgs += "," + error.InnerException.ExceptionMessage;

                error = error.InnerException;
            }
            return errorMsgs;
        }
    }
}