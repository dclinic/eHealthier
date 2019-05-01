using System.Web;
using System.Web.Optimization;

namespace ePRom
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Resources/scripts/jquery-{version}.js",
                "~/Resources/scripts/date.format.js",
                "~/Resources/scripts/jquery.cookie.js",
                "~/Resources/scripts/ClientTimeZone.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Resources/scripts/jquery-ui-{version}.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Resources/scripts/jquery.unobtrusive*",
                "~/Resources/scripts/jquery.validate*"
            ));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Resources/scripts/modernizr-*"
            ));

            bundles.Add(new StyleBundle("~/bundles/provider").Include(
                "~/Resources/styles/provider.css"
            ));

            #region theme

            #region admin

            bundles.Add(new StyleBundle("~/bundles/admin/theme").Include(
                "~/Resources/styles/admin.css",
                "~/Resources/styles/reset.css"
            ));

            bundles.Add(new StyleBundle("~/bundles/admin/bootstrap").Include(
                "~/Resources/styles/bootstrap.css",
                "~/Resources/styles/Site.css",
                "~/Resources/styles/font-awesome.css",
                "~/Resources/styles/animate.css",
                "~/Resources/styles/bootstrap-switch.css",
                "~/Resources/styles/toaster.css"
            ));

            #endregion admin

            #region user

            bundles.Add(new StyleBundle("~/bundles/user/bootstrap").Include(
                "~/Resources/styles/bootstrap.css",
                "~/Resources/styles/font-awesome.css",
                "~/Resources/styles/animate.css",
                "~/Resources/styles/bootstrap-switch.css",
                "~/Resources/styles/toaster.css",
                "~/Resources/styles/angular-material.min.css",
                "~/Resources/styles/select/select2-bootstrap.min.css",
                "~/Resources/styles/select/select2.min.css",
                "~/Resources/styles/select/bootstrap-select.min.css",
                "~/Resources/styles/datatables.min.css",
                "~/Resources/styles/datatables.bootstrap.css",
                "~/Resources/styles/bootstrap-datepicker3.min.css",
                "~/Resources/styles/components.min.css",
                "~/Resources/styles/plugins.min.css",
                "~/Resources/styles/autocomplete.css"
            ));

            bundles.Add(new StyleBundle("~/bundles/user/theme").Include(
                "~/Resources/styles/eprom.css",
                "~/Resources/styles/reset.css"
            ));

            bundles.Add(new StyleBundle("~/bundles/user/webresponsive").Include(
                "~/Resources/styles/WebResponsive.css"
            ));

            #endregion user

            #endregion theme

            #region js

            bundles.Add(new ScriptBundle("~/bundles/admin-js").Include(
                "~/Resources/scripts/bootstrap.js",
                "~/Resources/scripts/third-party/bootstrap-switch.min.js",
                "~/Resources/scripts/third-party/bootstrap-switch.js",
                "~/Resources/scripts/third-party/bootbox.js",
                "~/Resources/scripts/custom.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/admin-angular-js").Include(
               "~/Resources/scripts/angular.js",
               "~/Resources/scripts/angular-route.js",
               "~/Resources/scripts/third-party/angular-file-upload.min.js",
               "~/Resources/scripts/third-party/ngModelOptions.js",
               "~/Resources/scripts/third-party/angular-ng-message.js",
               "~/Resources/scripts/third-party/ui-bootstrap-tpls-1.2.2.js",
               "~/Resources/scripts/third-party/ngBootbox.js",
               "~/Resources/scripts/third-party/angular-animate.js",
               "~/Resources/scripts/third-party/toaster.js",
               "~/Resources/scripts/third-party/angular-cookies.js",
               "~/Resources/scripts/admin_app.js"
           ));

            bundles.Add(new ScriptBundle("~/bundles/user-js").Include(
                "~/Resources/NEW_DESIGN/bootstrap3.3.7.min.js",
                "~/Resources/scripts/third-party/highcharts.js",
                "~/Resources/scripts/third-party/highcharts-3d.js",
                "~/Resources/scripts/third-party/exporting.js",
                "~/Resources/scripts/app.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/angular-js").Include(
                "~/Resources/scripts/angular.js",
                "~/Resources/scripts/angular-route.js",
                "~/Resources/scripts/third-party/angular-file-upload.min.js",
                "~/Resources/scripts/third-party/ngModelOptions.js",
                "~/Resources/scripts/third-party/angular-ng-message.js",
                "~/Resources/scripts/third-party/ui-bootstrap-tpls-1.2.2.js",
                "~/Resources/scripts/third-party/ngBootbox.js",
                "~/Resources/scripts/third-party/angular-animate-1.5.5.min.js",
                "~/Resources/scripts/third-party/angular-aria-1.5.5.min.js",
                "~/Resources/scripts/third-party/angular-messages-1.5.5.min.js",
                "~/Resources/scripts/third-party/angular-material-1.1.0.min.js",
                "~/Resources/scripts/third-party/toaster.js",
                "~/Resources/scripts/third-party/angular-bootstrap-select.js",
                "~/Resources/scripts/third-party/angular-cookies.js",
                "~/Resources/scripts/app.js"
          ));

            bundles.Add(new ScriptBundle("~/bundles/select-js").Include(
                "~/Resources/scripts/third-party/select2.full.min.js",
                "~/Resources/scripts/third-party/bootstrap-select.min.js",
                "~/Resources/scripts/third-party/components-bootstrap-select.min.js",
                "~/Resources/scripts/third-party/jquery.slimscroll.min.js",
                "~/Resources/scripts/third-party/bootstrap-tabdrop.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/datepicker-js").Include(
                "~/Resources/scripts/third-party/bootstrap-datepicker.min.js",
                "~/Resources/scripts/third-party/components-date-time-pickers.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/datatable-js").Include(
                "~/Resources/scripts/third-party/jquery.dataTables-1.10.15.min.js",
                "~/Resources/scripts/third-party/dataTables.bootstrap1.10.15.min.js"
            ));


            bundles.Add(new StyleBundle("~/bundles/ProviderOrganization-js").Include(
                "~/Resources/scripts/App/ProviderOrganizationController.js"
            ));

            #region App

            bundles.Add(new StyleBundle("~/bundles/clear-js").Include(
             "~/Resources/scripts/App/ClearController.js"
         ));

            bundles.Add(new StyleBundle("~/bundles/master-js").Include(
              "~/Resources/scripts/App/MasterController.js"
          ));

            bundles.Add(new ScriptBundle("~/bundles/provider-js").Include(
                "~/Resources/scripts/App/providerController.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/patient-js").Include(
               "~/Resources/scripts/App/patientController.js"
           ));

            bundles.Add(new ScriptBundle("~/bundles/practice-js").Include(
              "~/Resources/scripts/App/PracticeController.js"
          ));

            bundles.Add(new ScriptBundle("~/bundles/organization-js").Include(
               "~/Resources/scripts/App/OrganizationController.js"
           ));

            bundles.Add(new ScriptBundle("~/bundles/dashboard-js").Include(
             "~/Resources/scripts/App/dashboardController.js"
         ));

            bundles.Add(new StyleBundle("~/bundles/login-js").Include(
                "~/Resources/scripts/App/LoginController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/customer-js").Include(
                "~/Resources/scripts/App/CustomerController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/add-customer-js").Include(
                "~/Resources/scripts/App/AddCustomerController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/restaurant-js").Include(
                "~/Resources/scripts/App/RestaurantController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/add-restaurant-js").Include(
                "~/Resources/scripts/App/AddRestaurantController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/restaurant-item-js").Include(
                "~/Resources/scripts/App/RestaurantItemController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/add-restaurant-item-js").Include(
                "~/Resources/scripts/App/AddRestaurantItemController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/category-js").Include(
                "~/Resources/scripts/App/CategoryController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/patient-category-js").Include(
               "~/Resources/scripts/App/PatientCategoryController.js"
           ));

            bundles.Add(new StyleBundle("~/bundles/add-patient-category-js").Include(
             "~/Resources/scripts/App/AddPatientCategoryController.js"
         ));

            bundles.Add(new StyleBundle("~/bundles/thirdpartyapp-js").Include(
              "~/Resources/scripts/App/ThirdPartyAppController.js"
          ));

            bundles.Add(new StyleBundle("~/bundles/add-thirdpartyapp-js").Include(
             "~/Resources/scripts/App/AddThirdPartyAppController.js"
         ));


            bundles.Add(new StyleBundle("~/bundles/indicators-js").Include(
               "~/Resources/scripts/App/IndicatorsController.js"
           ));

            bundles.Add(new StyleBundle("~/bundles/add-indicators-js").Include(
             "~/Resources/scripts/App/AddIndicatorsController.js"
         ));

            bundles.Add(new StyleBundle("~/bundles/add-category-js").Include(
                "~/Resources/scripts/App/AddCategoryController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/modal-category-js").Include(
                "~/Resources/scripts/App/ModalCategoryController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/order-js").Include(
                "~/Resources/scripts/App/OrdersController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/order-detail-js").Include(
                "~/Resources/scripts/App/OrderDetailsController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/provider-master-js").Include(
                "~/Resources/scripts/App/ProviderMasterController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/user-master-js").Include(
                "~/Resources/scripts/App/UserMasterController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/system-flag-js").Include(
                "~/Resources/scripts/App/SystemFlagController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/add-system-flag-js").Include(
                "~/Resources/scripts/App/AddSystemFlagController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/flag-group-js").Include(
                "~/Resources/scripts/App/FlagGroupController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/add-flag-group-js").Include(
                "~/Resources/scripts/App/AddFlagGroupController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/eproms-js").Include(
               "~/Resources/scripts/App/EpromsController.js"
           ));

            bundles.Add(new StyleBundle("~/bundles/add-eproms-js").Include(
                "~/Resources/scripts/App/AddEpromsController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/survey-monkey-js").Include(
                "~/Resources/scripts/App/AddFlagGroupController.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/patient-eprom-js").Include(
               "~/Resources/scripts/App/PatientEpromController.js"
           ));

            bundles.Add(new StyleBundle("~/bundles/cint-eprom-js").Include(
               "~/Resources/scripts/App/CINTepromController.js"
           ));

            bundles.Add(new StyleBundle("~/bundles/patient-detail-js").Include(
              "~/Resources/scripts/App/PatientDetailsController.js"
          ));

            bundles.Add(new StyleBundle("~/bundles/pathway-js").Include(
               "~/Resources/scripts/App/PathwayController.js"
           ));

            bundles.Add(new StyleBundle("~/bundles/add-pathway-js").Include(
             "~/Resources/scripts/App/AddPathwayController.js"
         ));

            bundles.Add(new StyleBundle("~/bundles/moment-js").Include(
             "~/Resources/scripts/moment.min.js"
         ));

            #endregion App

            #endregion js

#if !DEBUG
                 BundleTable.EnableOptimizations = true;
#endif
        }
    }
}