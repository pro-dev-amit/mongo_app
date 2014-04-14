ASP.Net MVC app using MongoDB, ElasticSearch & RabbitMQ
===========================

**NoSql** is absolutely awesome. The thing that I like the most, specially about document databases is they fit much closer to the object oriented design. Just store your domain structure flat into the database no matter how complex the hierarchy is; this also suits me personally as I'm a great fan of storing the data flat.

Some highlights of this app are
-------------------------------

- Completely ajax based.
- A generic repository pattern with **MongoDB**; having automatic support for parallel execution with the same context object.
- Supporting the new features of MongoDB 2.6 in the framework such as **free text search**, bulk insert, bulk update and bulk delete.
- Full fledged dynamic predicate building, paging and a basic auditing support.
- **Ioc containers** have been used for dependency injection. There is flexibility to switch between **Autofac** and **Microsoft Unity** though.
- **ElasticSearch** engine has been used for search scenarios, such as implmenting search on product catalog.
- Using **RabbitMQ** for queuing(polymorphic **pub/sub**, **RPC request/response** async patterns) certain operations such as saving critical data into MongoDB and pumping searchDocs into the Search Engine. The client used is **EasyNetQ** which runs on top of RabbitMQ .Net driver. A sample app about the basic usage of RabbitMQ with .Net can be found **[here](https://github.com/amitstefen/RabbitMQSample)**
- On front end, I've also added certain jQuery stuff; load data on scroll as it's done in facebook, some autocompletion, jQuery tabs layout loading on demand etc.

Architecture and usage
----------------------

- For setting up MongoDB, ElasticSearch and RabbitMQ; please go through the instructions in "Setup.txt" file at the root folder.
- The heart of the project lies in MatrixCore, this is where all the abstractions/generalisations are done. Focus has been on "Code to abstractions" principle throughout.
- You can switch the Ioc containers between Autofac and Unity by flipping the flag "bUseAutofacIoc" in web.config. Take a look at Bootstrapper classes for defining dependencies beforehand.
- This generic repository would suffice most of the needs, but we also know that a generalized solution cannot cater to all the finer details of a system; so we need extensibility, go ahead and create a new dataAccess repository class by just inheriting it from **"MXMongoRepository"** so that you get access to MongoContext object and other low level methods from mongoDbCSharp driver. Couple of such classes are already there in this sample code.
- All your mongo entities must inherit from **"MXMongoEntity"**. 
- For processing messages queued to RabbitMQ; **run the subscriber application** at **"..\Matrix.Processor\bin\Debug\Matrix.Processor.exe"**. You can run multiple instances of the subscriber as well.
- For creating SearchDocuments, inherit from **"MXSearchDocument"** class. And for Creating SearchRepositories, just inherit from **"MXSearchRepository"** class.
- Various settings such as database connection string, RabbitMQ port, ElasticSearch port etc. are defined in web.config file, please check out the appsettings section there.


Note: I'll be adding more to this project as I move forward. Aim is to experiment with MongoDB, ElasticSearch & RabbitMQ keeping design paradigms such as extensibility and flexibility as top priorities.
