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
        //No need to get the repository from here anymore; this is being injected by unity now. 
        //Keeping this class for extensibility purpose.

        /// <summary>
        /// DbContext for mongoDB
        /// </summary>        
        //private Lazy<IRepository> repository = new Lazy<IRepository>
        //    (
        //        () => new MXMongoRepository()
        //    );
        
        //protected IRepository _mongoRepository
        //{
        //    get
        //    {
        //        return repository.Value;
        //    }
        //}

        //public MXBaseController()
        //{
            
        //}

    }
}
