using System.Web.Optimization;

namespace ChineseAbs.ABSManagementSite
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/modeling").Include(
            "~/Scripts/pagejs/dealModeling.js"));

            bundles.Add(new ScriptBundle("~/bundles/alertify").Include(
                        "~/Scripts/alertify.js"));

            bundles.Add(new ScriptBundle("~/bundles/pagewalkthrough").Include(
                        "~/Scripts/jquery.pagewalkthrough.js"));
            bundles.Add(new ScriptBundle("~/bundles/report").Include(
                        "~/Scripts/alertify.js",
                        "~/Scripts/qrcode.js"));

            bundles.Add(new StyleBundle("~/Content/cnabsUtils/jqueryfont").Include(
 "~/Content/cnabsUtils/jquery-ui.icon-font.css"));

            /* StyleBundles */
            bundles.Add(new StyleBundle("~/Content/layout").Include(
                        "~/Content/site.css",
                        "~/Content/alertify/alertify.css",
                        "~/Content/alertify/themes/semantic.css",
                       // "~/Content/cnabsMenu/index.css",
                        "~/Content/cnabsUtils/cnabsUtils.css",
                        "~/Content/cnabsUtils/cnabsUtils.file.css",
                        "~/Content/cnabsUtils/layout.css",
                        "~/Content/cnabsUtils/jquery-ui.css",
                        "~/Content/icon.css"));




            bundles.Add(new StyleBundle("~/Content/alertifystyle").Include(
                        "~/Content/alertify/alertify.css",
                        "~/Content/alertify/themes/semantic.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            //bundles.Add(new ScriptBundle("~/bundles/cnabsMenu").Include(
            //    "~/Scripts/cnabsMenu/megamenu.js"));

            bundles.Add(new ScriptBundle("~/bundles/cnabsUtils").Include(
                "~/Scripts/cnabsUtils/cnabsUtils.js",
                "~/Scripts/cnabsUtils/cnabsUtils.core.js",
                "~/Scripts/cnabsUtils/cnabsUtils.file.js"));

            bundles.Add(new ScriptBundle("~/bundles/highchart-dark").Include(
                "~/Scripts/highchart-dark/highcharts.js",
                "~/Scripts/highchart-dark/lib.chart.js",
                "~/Scripts/highchart-dark/theme.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include("~/Scripts/angular.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryForm").Include("~/Scripts/jquery.form.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/fileDownload").Include("~/Scripts/jquery.fileDownload.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryTimeAgo").Include(
                "~/Scripts/jquery.timeago.js",
                "~/Scripts/jquery.timeago.zh-CN.js"
            ));

            bundles.Add(new StyleBundle("~/Content/cnabsDatepick").Include("~/Content/cnabsUtils/cnabs-jquery-ui-datepick.css", "~/Scripts/jquery-ui-timepicker-addon.min.css"));
            bundles.Add(new ScriptBundle("~/bundles/cnabsDatepick").Include("~/Scripts/cnabsUtils/cnabs-jquery-ui-datepick.js", "~/Scripts/jquery-ui-timepicker-addon.min.js", "~/Scripts/jquery-ui-timepicker-zh-CN.js"));

            bundles.Add(new StyleBundle("~/Content/calendar").Include("~/Scripts/fullcalendar/fullcalendar.css"));
            bundles.Add(new ScriptBundle("~/bundles/calendar").Include(
               "~/Scripts/fullcalendar/lib/moment.min.js",
               "~/Scripts/fullcalendar/fullcalendar.min.js",
               "~/Scripts/fullcalendar/locale/zh-cn.js"));

            bundles.Add(new ScriptBundle("~/bundles/lodash").Include(
               "~/Scripts/lodash/lodash.js"));

            bundles.Add(new ScriptBundle("~/bundles/handsontable").Include(
              "~/Scripts/handsontable/handsontable.full.js"));
            bundles.Add(new ScriptBundle("~/bundles/handsontableCustom").Include(
            "~/Scripts/handsontable/handson-custom.js"));
            bundles.Add(new StyleBundle("~/Content/handsontable").Include(
              "~/Scripts/handsontable/handsontable.full.css"));
            bundles.Add(new StyleBundle("~/Content/handsontableCustom").Include(
              "~/Scripts/handsontable/handson-custom.css"));
            

        }
    }
}