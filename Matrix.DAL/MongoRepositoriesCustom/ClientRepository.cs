using Matrix.Core.FrameworkCore;
using Matrix.Core.MongoCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using Matrix.Entities.MongoEntities;
using MongoDB.Driver.Builders;
using MongoDB.Driver;
using Matrix.Core.QueueCore;
using Matrix.DAL.MongoRepositoriesBase;

namespace Matrix.DAL.MongoRepositoriesCustom
{
    public class ClientRepository : MXBusinessMongoRepository
    {
        IMXRabbitClient _queueClient;

        public ClientRepository(IMXRabbitClient queueClient)
        {
            _queueClient = queueClient;
        }

        //Storing client information is absolutely critical to me. Hence queuing it to RabbitMQ
        public override string Insert<T>(T entity)
        {
            SetDocumentDefaults(entity);

            _queueClient.Bus.Publish<IMXEntity>(entity);

            return "queued";
        }

        public override bool Update<T>(T entity)
        {
            var collection = DbContext.GetCollection<Client>(typeof(Client).Name);

            var input = entity as Client;

            var query = Query<Client>.EQ(e => e.Id, entity.Id);

            var doc = collection.FindOne(query);

            WriteConcernResult result = null;

            if (entity.Version == doc.Version)
            {
                //APPROACH - 1; looks clumsy though
                //var update = MongoDB.Driver.Builders.Update<Client>                    
                //    .Set(c => c.Name, input.Name)
                //    .Set(c => c.Address, input.Address)
                //    .Set(c => c.ClientType, input.ClientType)
                //    .Set(c => c.Code, input.Code)
                //    .Set(c => c.PhoneNumber, input.PhoneNumber)
                //    .Set(c => c.Website, input.Website)
                //    //set defaults explicitly as I'm not using the framework methods.                    
                //    .Inc(c => c.Version, 1)
                //    .Set(c => c.CreatedBy, CurrentUser)
                //    .Set(c => c.CreatedDate, CurrentDate);

                //result = collection.Update(query, update, WriteConcern.Acknowledged); //this updates the DB
                                
                //input = collection.FindOne(query);

                //Task.Run(() =>
                //    InsertOneIntoHistory<Client>(input)
                //);

                //APPROACH - 2; better one.
                doc.Name = input.Name;
                doc.Address = input.Address;
                doc.ClientType = input.ClientType;
                doc.Code = input.Code;
                doc.PhoneNumber = input.PhoneNumber;
                doc.Website = input.Website;
                //set defaults explicitly as I'm not using the framework methods.                    
                SetDocumentDefaults(doc);

                result = collection.Save(doc, WriteConcern.Acknowledged);

                //insert into History now
                Task.Run(() =>
                    InsertOneIntoHistory<Client>(doc)
                );
            }

            return result.Ok;
        }

    }//End of DAO
}
