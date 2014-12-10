using Matrix.Core.MongoCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Matrix.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //map the special custom type DenormalizedRefrence; code based mapping so that our class could act as a real domain object
            //DenormalizedRefrenceMap.RegisterMappings();
            
            //intializing the IoC container
            if (ConfigurationManager.AppSettings["bUseAutofacIoc"].ToString().ToLower() == "true")            
                AutofacBootstrapper.Initialise();            
            else            
                UnityBootstrapper.Initialise();
        }
        
    }//End of class
}