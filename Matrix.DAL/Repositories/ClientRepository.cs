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

namespace Matrix.DAL.Repositories
{
    public class ClientRepository : MXMongoRepository
    {
        IQueueClient _queueClient;

        public ClientRepository(IQueueClient queueClient)
        {
            _queueClient = queueClient;
        }

        //Storing client information is absolutely critical to me. Hence queuing it to RabbitMQ
        public override string Insert<T>(T entity, bool isActive = true)
        {
            _queueClient.Bus.Publish<IMXEntity>(entity);

            return "queued";
        }

        public override bool Update<T>(T entity, bool bMaintainHistory = false)
        {
            if (bMaintainHistory) base.InsertDocumentIntoHistory<Client>(entity.Id);

            var collection = dbContext.GetCollection<Client>("Client");

            var input = entity as Client;

            var query = Query<Client>.EQ(e => e.Id, entity.Id);

            var update = MongoDB.Driver.Builders.Update<Client>
                .Set(c => c.Name, input.Name)
                .Set(c => c.Address, input.Address)
                .Set(c => c.ClientType, input.ClientType)
                .Set(c => c.Code, input.Code)
                .Set(c => c.PhoneNumber, input.PhoneNumber)
                .Set(c => c.Website, input.Website);
                
            var result = collection.Update(query, update, WriteConcern.Acknowledged);

            return result.Ok;
        }

    }//End of DAO
}
