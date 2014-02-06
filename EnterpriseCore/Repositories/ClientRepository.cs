using MatrixCore.DataAccess;
using MatrixCore.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using EnterpriseCore.Entities;
using MongoDB.Driver.Builders;
using MongoDB.Driver;

namespace EnterpriseCore.DataAccessObjects
{
    public class ClientRepository : MXMongoRepository
    {
        public override bool Update<T>(T entity, bool bMaintainHistory = false)
        {
            var collection = db.GetCollection<Client>("Client");

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

            if (bMaintainHistory) base.InsertDocumentIntoHistory<Client>(entity.Id);

            return result.Ok;
        }

    }//End of DAO
}
