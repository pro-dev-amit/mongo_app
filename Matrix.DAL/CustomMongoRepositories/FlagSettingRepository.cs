using Matrix.Core.ConfigurationsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.DAL.CustomMongoRepositories
{
    public class FlagSettingRepository : IFlagSettingRepository
    {
        IMXConfigurationMongoRepository _repository;

        CacheItemPolicy _cachePolicy;

        public FlagSettingRepository(IMXConfigurationMongoRepository repository)
        {
            _repository = repository;

            _cachePolicy = new CacheItemPolicy{ Priority = CacheItemPriority.NotRemovable };
        }

        public IList<FlagSetting> Get(int skip = 0, int take = -1)
        {
            return _repository.GetMany<FlagSetting>().ToList();
        }

        public FlagSetting GetOne(string id)
        {            
            return _repository.GetOne<FlagSetting>(id);
        }

        public bool Save(FlagSetting entity)
        {
            bool isSuccess = false;

            if (string.IsNullOrEmpty(entity.Id))
            {
                _repository.Insert<FlagSetting>(entity);

                isSuccess = !string.IsNullOrEmpty(entity.Id) ? true : false;

                if (isSuccess) MemoryCache.Default.Set(entity.Name, entity.FlagValue, _cachePolicy); 

                return isSuccess;
            }
            else
            {
                isSuccess = _repository.Update<FlagSetting>(entity);

                if (isSuccess) MemoryCache.Default.Set(entity.Name, entity.FlagValue, _cachePolicy);
            }

            return isSuccess;
        }

        public bool Delete(string id)
        {
            var flagDoc = GetOne(id);
                        
            var isSuccess = _repository.Delete<FlagSetting>(id);

            if (isSuccess) MemoryCache.Default.Remove(flagDoc.Name);

            return isSuccess;
        }
    }
}
