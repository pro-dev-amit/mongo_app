using Matrix.Core.MongoCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.DAL.MongoBaseRepositories
{
    /// <summary>
    /// Contract for "MXBusiness" dataset; can have behavior specific to this dataset only. Right now, I do not have anything specific on this dataset; but a real 
    /// business would certainly need such extensibility points.
    /// Create a similar contract for a new database.
    /// </summary>
    public interface IMXBusinessMongoRepository : IMXMongoRepository
    {

    }
}
