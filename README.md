ASP.Net MVC app using MongoDB
===========================

NoSql is absolutely awesome, I love it. This sample ASP.Net MVC 4(.Net 4.5) app is using MongoDB repository pattern(built from scratch); a library that you can hook directly into you project. You can go through the code, it's concise and fairly simple to understand. I dedicate this work to CareerBuilder.com; the guys that taught me the art of programming years back. 

Some highlights of this app are
-------------------------------

- Completely ajax based.
- A generic repository pattern with MongoDB; can be hooked directly into your application.
- Full fledged dynamic predicate building, paging and a basic auditing support.
- On front end, I've also added certain jQuery stuff; load data on scroll as it's done in facebook, some autocompletion, jQuery tabs layout etc.

Project architecture and usage
-------------------------------

- The solution has three projects. The core of the project lies in MatrixCore, this is where all the heavylifting is done. The EnterpriseCore contains DBEntities, custom repositories and ViewModels. And the front end being asp.net MVC project; MatrixWeb.
- This generic repository would suffice most of the operations, but we also know that a generalized solution cannot cater to the finer details of a system; so we need extensibility, go ahead and crate a new dataAccess repository class by just inheriting it from MXMongoRepository so that you get access to MongoContext object and other low level methods from mongoDbCSharp driver. One such class is already there in this sample code.
- Also, all your mongo entities must inherit from MXEntity. 

