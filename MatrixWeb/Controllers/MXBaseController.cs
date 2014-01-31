using MatrixCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MatrixWeb.Controllers
{
    public class MXBaseController : Controller
    {
        /// <summary>
        /// DbContext for mongoDB
        /// </summary>        
        private Lazy<IRepository> repository = new Lazy<IRepository>
            (
                () => new MXMongoRepository()
            );
        
        protected IRepository _mongoRepository
        {
            get
            {
                return repository.Value;
            }
        }

        public MXBaseController()
        {
            
        }

    }
}
