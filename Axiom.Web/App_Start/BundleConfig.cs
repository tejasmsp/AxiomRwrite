using System.Web;
using System.Web.Optimization;

namespace Axiom.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                      "~/Scripts/jquery-{version}.js"
                      , "~/Scripts/jquery-ui.min.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));


            //--Global stylesheets-- 
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/googlefonts.css",
                      "~/Content/icons/icomoon/styles.css",
                      "~/Content/iconmoon.css",
                      "~/Content/bootstrap.css",
                      "~/Content/toastr.css",
                      "~/Content/jquery.dataTables.min.css",
                      "~/Content/core.css",
                      "~/Content/components.css",
                      "~/Content/colors.css",
                      "~/Content/Notification.css",
                      "~/Scripts/switch/angular-ui-switch.css",
                      "~/Content/Site.css", "~/Content/Axiom.css"));

            bundles.Add(new StyleBundle("~/Content/cssLeagalEagle").Include(
                      "~/Content/googlefonts.css",
                      "~/Content/icons/icomoon/styles.css",
                      "~/Content/iconmoon.css",
                      "~/Content/bootstrap.css",
                      "~/Content/toastr.css",
                      "~/Content/jquery.dataTables.min.css",
                      "~/Content/core.css",
                      "~/Content/components.css",
                      "~/Content/colors.css",
                      "~/Content/Notification.css",
                      "~/Scripts/switch/angular-ui-switch.css",
                      "~/Content/Site.css",
                      "~/Content/Axiom.css"));

            bundles.Add(new StyleBundle("~/Content/1").Include(
                      "~/Content/1/style.css"));

            bundles.Add(new StyleBundle("~/Content/4").Include(
                      "~/Content/4/style.css"));
            // --End global stylesheets

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/switch").Include(
                        "~/Scripts/switch/angular-ui-switch.js"
                        // "~/Scripts/switch/app.js"
                        ));

            //<!-- Core JS files -->
            bundles.Add(new ScriptBundle("~/bundles/Common").Include(
                      //"~/Scripts/loaders/pace.min.js",
                      "~/Scripts/jquery.min.js",
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/loaders/blockui.min.js",
                      "~/Scripts/ui/nicescroll.min.js",
                      "~/Scripts/ui/drilldown.js",
                       "~/Scripts/ui/nicescroll.min.js",
                      "~/Scripts/respond.js",
                       "~/Scripts/visualization/d3/d3.min.js",
                       "~/Scripts/visualization/d3/d3_tooltip.js",
                       "~/Scripts/forms/styling/uniform.min.js",
                       "~/Scripts/forms/selects/bootstrap_multiselect.js",
                        "~/Scripts/ui/moment/moment.min.js",
                        //"~/Scripts/forms/Wizard/stepy.min.js",
                        //"~/Scripts/forms/Wizard/select2.min.js",
                        //  "~/Scripts/forms/Wizard/wizard_stepy.js",
                        //"~/Scripts/forms/Wizard/jasny_bootstrap.min.js",
                        "~/Scripts/bootbox.min.js",
                       //"~/Scripts/forms/Wizard/validate.min.js",
                       //  "~/Scripts/pickers/daterangepicker.js",
                       "~/Scripts/bootstrap-datepicker.js",
                       "~/Scripts/app.js"


                      // "~/Scripts/pages/dashboard.js"
                      ));
            //< !-- End core JS files -->


            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                   "~/Scripts/angular.js",
                   "~/Scripts/angular-filter.min.js",
                   "~/Scripts/angular-animate.min.js",
                   "~/Scripts/angular-ui-router.min.js",
                   "~/Scripts/angular-local-storage.js",
                   "~/Scripts/angular-sanitize.js",
                   "~/Scripts/angucomplete-alt.min.js",
                   "~/Scripts/loadash.min.js",
                   "~/Scripts/switch/angular-ui-switch.js"
                   )
                   );


            var appConfigBundle = new Bundle("~/bundles/appConfig");
            appConfigBundle.Include("~/Scripts/jquery.dataTables.min.js");
            appConfigBundle.Include("~/Scripts/dataTables.tableTools.js");
            appConfigBundle.Include("~/Scripts/toastr.js");
            appConfigBundle.Include("~/Scripts/pages/form_multiselect.js");
            appConfigBundle.Include("~/JS/appConfiguration.js");
            appConfigBundle.Include("~/JS/Common.js");
            appConfigBundle.Include("~/JS/Enums.js");
            appConfigBundle.IncludeDirectory("~/JS/Directives", "*.js", false);
            appConfigBundle.IncludeDirectory("~/JS/Factory", "*.js", false);
            appConfigBundle.IncludeDirectory("~/JS/Filters", "*.js", false);
            bundles.Add(appConfigBundle);

            var controllerBundle = new Bundle("~/bundles/ControllerAndServices");
            controllerBundle.IncludeDirectory("~/JS/Master", "*.js", true);
            controllerBundle.IncludeDirectory("~/JS/UserProfile", "*.js", true);
            controllerBundle.IncludeDirectory("~/JS/ChangePassword", "*.js", true);
            controllerBundle.IncludeDirectory("~/JS/OrderList", "*.js", true);
            controllerBundle.IncludeDirectory("~/JS/OrderWizard", "*.js", true);
            controllerBundle.IncludeDirectory("~/JS/OrderDetail", "*.js", true);
            controllerBundle.IncludeDirectory("~/JS/PartDetail", "*.js", true);
            controllerBundle.IncludeDirectory("~/JS/Billing", "*.js", true);
            controllerBundle.IncludeDirectory("~/JS/PrintInvoice", "*.js", true);
            controllerBundle.IncludeDirectory("~/JS/SearchOrderList", "*.js", true);
            controllerBundle.IncludeDirectory("~/JS/Client", "*.js", true);
            controllerBundle.IncludeDirectory("~/JS/InvoiceBatch", "*.js", true);
            controllerBundle.IncludeDirectory("~/JS/AccessReports", "*.js", true);
            bundles.Add(controllerBundle);


        }
    }
}
