using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;
using Matrix.Core.DataAccess;
using Matrix.DAL.DataAccessObjects;
using Matrix.Web.Controllers;
using Matrix.Web.Areas.Sales.Controllers;

namespace Matrix.Web
{
  public static class UnityBootstrapper
  {
    public static IUnityContainer Initialise()
    {
      var container = BuildUnityContainer();

      DependencyResolver.SetResolver(new UnityDependencyResolver(container));

      return container;
    }

    private static IUnityContainer BuildUnityContainer()
    {
      var container = new UnityContainer();

      // register all your components with the container here
      // it is NOT necessary to register your controllers

      // e.g. container.RegisterType<ITestService, TestService>();    
      RegisterTypes(container);

      return container;
    }

    public static void RegisterTypes(IUnityContainer container)
    {
        container.RegisterType<IRepository, MXMongoRepository>();

        container.RegisterType<IRepository, ClientRepository>("ClientRepository");
        
        container.RegisterType<ClientController>(
            new InjectionConstructor(new ResolvedParameter<IRepository>("ClientRepository")
            ));
    }
  }
}