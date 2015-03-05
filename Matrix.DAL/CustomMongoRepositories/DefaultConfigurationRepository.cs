using Matrix.Core.ConfigurationsCore;
using Matrix.DAL.BaseMongoRepositories;
using Matrix.Entities.MongoEntities;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Matrix.Core.MongoCore;
using Matrix.Core.CacheCore;
using System.Configuration;
using Matrix.DAL.SearchBaseRepositories;

namespace Matrix.DAL.CustomMongoRepositories
{
    public class DefaultConfigurationRepository : IDefaultConfigurationRepository
    {
        #region "Initializatin and Attributes"

        IMXBusinessMongoRepository _bRepository;
        IMXProductCatalogMongoRepository _pcRepository;
        IMXConfigurationMongoRepository _cRepository;
        IMXCacheRepository _redisCache;
        IBookSearchRepository _bookSearchRepository;

        public DefaultConfigurationRepository(IMXBusinessMongoRepository bRepository, 
            IMXProductCatalogMongoRepository pcRepository, 
            IMXConfigurationMongoRepository cRepository,
            IBookSearchRepository bookSearchRepository)
        {
            this._bRepository = bRepository;
            this._pcRepository = pcRepository;
            this._cRepository = cRepository;
            _redisCache = new MXRedisCacheRepository(ConfigurationManager.AppSettings["redisConnectionString"].ToString(),
                                                    MXRedisDatabaseName.FlagSettings);
            _bookSearchRepository = bookSearchRepository;
        }

        #endregion

        public bool IsMasterDataSet
        {
            get { return isMasterDataSet(); }
        }

        public void SetMasterData()
        {
            //insert these flags before hand for now.
            List<FlagSetting> lstFlagSetting = new List<FlagSetting>() 
            {
                new FlagSetting { Name = "bUseElasticSearchEngine", Description = "enable product search on ElasticSearch engine", FlagValue = "true", IsPermanent = false },
                new FlagSetting { Name = "bShowLoadTime", Description = "Show load timings. This is used for identifying the bottle necks", FlagValue = "true", IsPermanent = true },
            };
            _cRepository.Insert<FlagSetting>(lstFlagSetting);

            List<ProgrammingRating> lstProgrammingRating = new List<ProgrammingRating>() 
            {
                new ProgrammingRating { Name = "Expert", Code = "EX" },
                new ProgrammingRating { Name = "Good",  Code = "GD" },
                new ProgrammingRating { Name = "Average", Code = "AVG" },
                new ProgrammingRating { Name = "Beginner", Code = "B" },
                new ProgrammingRating { Name = "Hopeless", Code = "HL" },

            };
            _bRepository.Insert<ProgrammingRating>(lstProgrammingRating);

            List<Gender> lstGender = new List<Gender>() 
            {
                new Gender { Name = "Male", Code = "M" },
                new Gender { Name = "Female",  Code = "F" },
            };
            _bRepository.Insert<Gender>(lstGender);

            List<Skill> lstSkill = new List<Skill>() 
            {
                new Skill { Name = "Ruby On Rails", Code = "RR" },
                new Skill { Name = "Core Java",  Code = "J" },
                new Skill { Name = "Java EE",  Code = "J" },
                new Skill { Name = "Scala" },
                new Skill { Name = "Play Framework" },
                new Skill { Name = "Python" },
                new Skill { Name = "ASP.Net MVC" },
            };
            _bRepository.Insert<Skill>(lstSkill);

            List<ClientType> lstClientType = new List<ClientType>() 
            {
                new ClientType { Name = "Information Technology-1" },
                new ClientType { Name = "Information Technology-2" },
                new ClientType { Name = "Information Technology-3" },
                new ClientType { Name = "Information Technology-4" },
                new ClientType { Name = "Energy Sector-1" },
                new ClientType { Name = "Energy Sector-2" },
                new ClientType { Name = "Energy Sector-3" },
                
            };
            _bRepository.Insert<ClientType>(lstClientType);

            List<Author> lstAuthor = new List<Author>() 
            {
                new Author { Name = "Paulo Coelho" },
                new Author { Name = "Amit Kumar" },
                new Author { Name = "David Schwartz" },
                new Author { Name = "Max Payne" },
                new Author { Name = "Michael Hartl" },
            };
            _pcRepository.Insert<Author>(lstAuthor);

            List<BookCategory> lstBookCategory = new List<BookCategory>() 
            {
                new BookCategory { Name = "Python" },
                new BookCategory { Name = "Java" },
                new BookCategory { Name = "Inspiration, Motivation" },
                new BookCategory { Name = "Fiction" },
                new BookCategory { Name = "Ruby On Rails" },
            };
            _pcRepository.Insert<BookCategory>(lstBookCategory);
            
            //Create a text index on Book collection, this required for free text searching actually.
            var collection = _pcRepository.DbContext.GetCollection<Book>("Book");

            if (!collection.IndexExistsByName("book_text"))
            {
                collection.CreateIndex(IndexKeys.Text("nm", "au.nm", "ct.nm"), IndexOptions.SetName("book_text").SetWeight("nm", 4).SetWeight("au.nm", 3));
            }
        }

        public void ClearEverything()
        {
            _bRepository.DropDatabase();
            _cRepository.DropDatabase();
            _pcRepository.DropDatabase();

            _redisCache.Clear(MXRedisDatabaseName.FlagSettings);

            _bookSearchRepository.DeleteIndex(ConfigurationManager.AppSettings["bookIndex"].ToString());
        }

        bool isMasterDataSet()
        {
            var results = _bRepository.GetCount<Gender>(); //just checking one of the entity here.

            return results > 0 ? true : false;
        }
    }
}
