using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Matrix.Web.Controllers;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using System.Reflection;
using Matrix.Core.FrameworkCore;
using Matrix.Web.Areas.Sales.Controllers;
using Matrix.Core.MongoCore;
using System.Configuration;
using Matrix.Core.QueueCore;
using Matrix.DAL.Repositories;
using Matrix.DAL.SearchRepositories;
using Matrix.Core.SearchCore;


namespace Matrix.Web
{
    public static class AutofacBootstrapper
    {
        public static void Initialise()
        {
            var builder = new ContainerBuilder();

            BuildContainer(builder);
        }

        static void BuildContainer(ContainerBuilder builder)
        {
            //Register MVC controllers first
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            //then the types
            builder.RegisterType<MXMongoRepository>().As<IRepository>();

            //Named types
            builder.RegisterType<ClientRepository>().Named<IRepository>("ClientRepository");


            //inject specific implementation of IRepository Interface
            builder.Register(c => new ClientController(c.ResolveNamed<IRepository>("ClientRepository")));

            //register rabbitMQ client as a singleton
            builder.RegisterType<MXRabbitClient>().As<IQueueClient>().SingleInstance();

            builder.RegisterType<BookRepository>().As<IBookRepository>();

            //searh repos
            builder.RegisterType<MXSearchRepository>().As<ISearchRepository>();
            builder.RegisterType<BookSearchRepository>().As<IBookSearchRepository>();

            
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
        
    }//End of class
}