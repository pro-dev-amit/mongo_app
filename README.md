ASP.Net MVC app with MongoDB
===========================

NoSql is absolutely awesome. The thing that I like the most, specially about document databases is it fits much closer to the object oriented design. Just store your domain structure flat into the database no matter how complex the hierarchy is; this also suits me personally as I'm a great fan of storing the data flat.

This sample ASP.Net MVC 4(.Net 4.5) app is using MongoDB repository pattern(built from scratch); a library that you can hook directly into you project. You can go through the code, it's concise and fairly simple to understand. I dedicate this work to CareerBuilder.com; the guys that taught me the art of programming. 

Some highlights of this app are
-------------------------------

- Completely ajax based.
- A generic repository pattern with MongoDB; having support for parallel execution with the same context object.
- Full fledged dynamic predicate building, paging and a basic auditing support.
- Microsoft Unity DI container has been used for dependency injection of repositories.
- On front end, I've also added certain jQuery stuff; load data on scroll as it's done in facebook, some autocompletion, jQuery tabs layout loading on demand etc.

Architecture and usage
----------------------

- For setting up MongoDB; please go though the text in "Mongo_Setup.txt" file at the root folder.
- The solution has three projects. The core of the project lies in MatrixCore, this is where all the heavylifting is done. The EnterpriseCore contains DBEntities, custom repositories and ViewModels. And the front end being asp.net MVC project; MatrixWeb.
- This generic repository would suffice most of the operations, but we also know that a generalized solution cannot cater to the finer details of a system; so we need extensibility, go ahead and crate a new dataAccess repository class by just inheriting it from MXMongoRepository so that you get access to MongoContext object and other low level methods from mongoDbCSharp driver. One such class is already there in this sample code.
- To induce a custom Repository, just register it with the container. Take a look at the "RegisterTypes()" method in Bootstrapper class.
- Also, all your mongo entities must inherit from "MXEntity". 
- Database connection string is defined in web.config file, check out the appsettings section.

Note: I'll be adding more to this project as I move forward. Aim is to showcase a ASP.Net MVC application using MongoDB. For me, right now the focus is not so much on the UI though.
