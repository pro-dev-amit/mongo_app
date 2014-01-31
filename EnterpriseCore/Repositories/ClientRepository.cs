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
        public bool Update(Client entity)
        {
            var collection = db.GetCollection<Client>("Client");

            var query = Query<Client>.EQ(e => e.Id, entity.Id);

            var update = MongoDB.Driver.Builders.Update<Client>
                .Set(c => c.Name, entity.Name)
                .Set(c => c.Address, entity.Address)
                .Set(c => c.ClientType, entity.ClientType)
                .Set(c => c.Code, entity.Code)
                .Set(c => c.PhoneNumber, entity.PhoneNumber)
                .Set(c => c.Website, entity.Website);
                
            var result = collection.Update(query, update, WriteConcern.Acknowledged);

            return result.Ok;
        }


        public IList<DenormalizedReference> GetClientTypesForAutoComplete(string term)
        {
            var sTerm = term.ToLower();

            var collection = db.GetCollection<ClientType>("ClientType");

            return collection.AsQueryable()
                .Where(c => c.IsActive == true && c.Name.ToLower().Contains(sTerm))
                .Select(c => new DenormalizedReference
                {
                    DenormalizedId = c.Id,
                    DenormalizedName = c.Name
                }).OrderBy(c => c.DenormalizedName).Take(10).ToList();
        }

    }//End of DAO
}
