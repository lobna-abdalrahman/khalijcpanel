using System.Web;
using System.Web.Optimization;

namespace Cpanel
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                       "~/Scripts/jquery.unobtrusive-ajax.js",
                  "~/Scripts/jquery.unobtrusive-ajax.min.js",
                  "~/Scripts/jquery.validate*",
                  "~/Scripts/jquery.validate.unobtrusive.js",
                  "~/Scripts/jquery.validate.unobtrusive.min.js"));  
            

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/datetime").Include(

            "~/Scripts/moment*",

            "~/Scripts/bootstrap-datetimepicker*"));

            bundles.Add(new ScriptBundle("~/Js").Include(
                //JQuery 3

                     "~/Content/bower_components/jquery-ui/jquery-ui.min.js",
                //JQuery UI 1.11.4
                     "~/Content/bower_components/jquery-ui/jquery-ui.min.js",
                //Bootstrap 3.3.7
                     "~/Content/bower_components/bootstrap/dist/js/bootstrap.min.js",
                //Sparkline
                     "~/Content/bower_components/jquery-sparkline/dist/jquery.sparkline.min.js",
                //jvectormap 
                     "~/Content/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js",
                     "~/Content/plugins/jvectormap/jquery-jvectormap-world-mill-en.js",
                //jQuery Knob Chart
                     "~/Content/bower_components/jquery-knob/dist/jquery.knob.min.js",
                //daterangepicker
                     "~/Content/bower_components/moment/min/moment.min.js",
                     "~/Content/bower_components/bootstrap-daterangepicker/daterangepicker.js",
                //DatePicker
                     "~/Content/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js",
                //Bootstrap WYSIHTML5
                     "~/Content/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js",
                //slimScroll
                     "~/Content/bower_components/jquery-slimscroll/jquery.slimscroll.min.js",
                //AdminLTE app
                     "~/Content/dist/js/adminlte.min.js",
                //Dashboard Demo
                     "~/Content/dist/js/pages/dashboard.js",
                // Demo
                     "~/Content/dist/js/demo.js"
                //        "~/Scripts/respond.js"
                     ));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                           "~/Content/bower_components/bootstrap/dist/css/bootstrap.min.css",
                      "~/Content/bower_components/font-awesome/css/font-awesome.min.css",
                      "~/Content/bower_components/Ionicons/css/ionicons.min.css",
                      "~/Content/dist/css/AdminLTE.min.css",
                      "~/Content/dist/css/skins/_all-skins.min.css",
                      //jvectorMap
                      "~/Content/bower_components/jvectormap/jquery-jvectormap.css",
                      //DatePicker
                      "~/Content/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css",
                      //DateRangePicker
                      "~/Content/bower_components/bootstrap-daterangepicker/daterangepicker.css",
                      //bootstrap-wysihtml
                      "~/Content/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.min.css",
                      //"~/Content/site.css"
                      "~/Content/site.css"));
        }
    }
}
