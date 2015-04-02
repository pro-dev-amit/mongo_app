ASP.Net MVC app using MongoDB, ElasticSearch, Redis and RabbitMQ
===========================

**NoSql** is absolutely awesome. The thing that I like the most, specially about document databases is they fit much closer to the object oriented design. Just store your domain structure flat into the database no matter how complex the hierarchy is; this also suits me personally as I'm a great fan of storing the data flat.

Do take a look at my video series for this application on youtube **[HERE](https://www.youtube.com/playlist?list=PLj5fYy-TIvPfJIVFdHCTiqb4g2IKnl9KD)**.
This will help you in gaining concepts for NoSQL as a whole, setting up the dev environment and running this application.

Some highlights of this app are
-------------------------------

- A generic repository pattern with **MongoDB**; having automatic support for parallel execution with the same context object.
- Supporting the new features of MongoDB 2.6 through the latest 3.0 series in the framework such as **free text search**, bulk insert, bulk update and bulk delete.
- Full fledged dynamic predicate building, paging and complete auditing support.
- **Ioc containers** have been used for dependency injection. There is flexibility to switch between **Autofac** and **Microsoft Unity** though.
- **ElasticSearch** engine has been used for search scenarios, such as implmenting search on product catalog.
- Using **RabbitMQ** for queuing(polymorphic **pub/sub**, **RPC request/response** async patterns) certain operations such as saving critical data into MongoDB and pumping searchDocs into the Search Engine. The client used is **EasyNetQ** which runs on top of RabbitMQ .Net driver. A sample app about the basic usage of RabbitMQ with .Net can be found **[here](https://github.com/amitstefen/RabbitMQSample)**
- Using **Redis** for distributed caching. All operations are defined in generic manner.
- **Featured flag settings** to roll out new functionalities and conducting experiments in production with minimum risks.
- **Automatic failover** support for MongoDB and ElasticSearch is built into the core framework.
- Completely ajax based.

Architecture and usage
----------------------

- For setting up MongoDB, ElasticSearch and RabbitMQ; please go through the instructions in "Setup.txt" file at the root folder.
- The heart of the project lies in MatrixCore, this is where all the abstractions/generalisations are done. Focus has been on "Code to abstractions" principle throughout.
- You can switch the Ioc containers between Autofac and Unity by flipping the flag "bUseAutofacIoc" in web.config. Take a look at Bootstrapper classes for defining dependencies beforehand.
- It's a must create a new dataAccess base repository class(one per database) by just inheriting it from **"MXMongoRepository"** abstract class so that you get access to MongoContext object and other low level methods from mongoDbCSharp driver. Further, custom repositories can also be created by sub-classing the then base repository.
- All your mongo entities must inherit from **"MXMongoEntity"**. 
- For processing messages queued to RabbitMQ; **run the subscriber application** at **"..\Matrix.Processor\bin\Debug\Matrix.Processor.exe"**. You can run multiple instances of the subscriber as well.
- For creating SearchDocuments, inherit from **"MXSearchDocument"** class. And for Creating SearchRepositories, just inherit from **"MXSearchRepository"** class. I've enforced the idea of creating a separate repository for a new index.
- Various settings such as database connection string, RabbitMQ port, ElasticSearch port etc. are defined in web.config file, please check out the appsettings section there.


**Note:** I'll be adding more to this project as I move forward. Aim is to keep design paradigms such as extensibility and flexibility as top priorities.
- If you have any questions, feel free to contact me at amit.nosql@gmail.com. If you need a new feature in the core framework, please create a new issue in the "Issues" section.
- Feel free to share this application and video series to other developers.
