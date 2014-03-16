using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;
using Matrix.Core.FrameworkCore;
using Matrix.Web.Controllers;
using Matrix.Web.Areas.Sales.Controllers;
using Matrix.Core.QueueCore;
using Matrix.DAL.Repositories;
using Matrix.Core.SearchCore;
using Matrix.DAL.SearchRepositories;

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

        //register rabbitMQ client as a singleton
        container.RegisterType<IQueueClient, MXRabbitClient>(new ContainerControlledLifetimeManager());

        container.RegisterType<IBookRepository, BookRepository>();

        //searh repos
        container.RegisterType<ISearchRepository, MXSearchRepository>();
        container.RegisterType<IBookSearchRepository, BookSearchRepository>();
    }
  }
}