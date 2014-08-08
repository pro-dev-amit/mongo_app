using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;
using Matrix.Core.FrameworkCore;
using Matrix.Web.Controllers;
using Matrix.Web.Areas.Sales.Controllers;
using Matrix.Core.QueueCore;
using Matrix.DAL.CustomRepositories;
using Matrix.Core.SearchCore;
using Matrix.DAL.SearchRepositories;
using Matrix.Core.MongoDbBaseRepositories;

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
        container.RegisterType<IMXBusinessMongoRepository, MXBusinessMongoRepository>();

        container.RegisterType<IMXBusinessMongoRepository, ClientRepository>("ClientRepository");

        //inject specific implementation of IRepository Interface. A better approach though is to create a separate interface as it's done with Books and then inject.
        //I'll keep this for reference purpose though.
        container.RegisterType<ClientController>(
            new InjectionConstructor(new ResolvedParameter<IMXBusinessMongoRepository>("ClientRepository")
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