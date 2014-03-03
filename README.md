ASP.Net MVC app with MongoDB
===========================

NoSql is absolutely awesome. The thing that I like the most, specially about document databases is it fits much closer to the object oriented design. Just store your domain structure flat into the database no matter how complex the hierarchy is; this also suits me personally as I'm a great fan of storing the data flat.

This sample ASP.Net MVC 4(.Net 4.5, Visual Studio 2012) app is using MongoDB repository pattern(built from scratch); a library that you can hook directly into you project. You can go through the code, it's concise and fairly simple to understand.

Some highlights of this app are
-------------------------------

- Completely ajax based.
- A generic repository pattern with MongoDB; having support for parallel execution with the same context object.
- Full fledged dynamic predicate building, paging and a basic auditing support.
- Ioc container have been used for dependency injection. There is flexibility to switch between Autofac and Microsoft Unity though.
- Using RabbitMQ for queuing(pub/sub pattern) certain operations such as saving critical data. The client used is EasyNetQ which runs on top of RabbitMQ .Net driver.
- On front end, I've also added certain jQuery stuff; load data on scroll as it's done in facebook, some autocompletion, jQuery tabs layout loading on demand etc.

Architecture and usage
----------------------

- For setting up MongoDB and RabbitMQ; please go through the instructions in "Setup.txt" file at the root folder.
- The heart of the project lies in MatrixCore, this is where all the abstractions/generalisations are done. Focus has been on "Code to abstractions" principle throughout.
- you can switch the Ioc containers between Autofac and Unity by flipping the flag "bUseAutofacIoc" in web.config. Take a look at Bootstrapper classes for defining dependencies beforehand.
- This generic repository would suffice most of the operations, but we also know that a generalized solution cannot cater to all the finer details of a system; so we need extensibility, go ahead and crate a new dataAccess repository class by just inheriting it from MXMongoRepository so that you get access to MongoContext object and other low level methods from mongoDbCSharp driver. One such class is already there in this sample code.
- All your mongo entities must inherit from "MXMongoEntity". 
- For processing messages queued to RabbitMQ using pub/sub pattern; run the subscriber application at "..\Matrix.Processor\bin\Debug\Matrix.Processor.exe". You can run multiple instances of the subscriber as well.
- Various settings such as database connection string, RabbitMQ port etc. are defined in web.config file, please check out the appsettings section there.


Note: I'll be adding more to this project as I move forward. Aim is to experiment with MongoDB, keeping design paradigms such as extensibility and flexibility as top priorities.
