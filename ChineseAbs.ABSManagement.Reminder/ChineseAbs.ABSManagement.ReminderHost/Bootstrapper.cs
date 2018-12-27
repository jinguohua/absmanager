using ChineseAbs.ABSManagement.Foundation;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;

namespace ChineseAbs.ABSManagement.ReminderHost
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration { Password = @"Tod@y" }; }
        }

        protected override void RequestStartup(Nancy.TinyIoc.TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            Log.Info("Url: " + context.Request.Url + " User: " + context.Request.UserHostAddress.ToString());

            pipelines.OnError.AddItemToEndOfPipeline((z, a) =>
            {
                Log.Error("Unhandled error on request: " + context.Request.Url + " : " + a.Message, a);
                return NancyUtils.Error(a.Message, context.Request.Url, a.ToString());
            });

            base.RequestStartup(container, pipelines, context);
        }
    }
}
