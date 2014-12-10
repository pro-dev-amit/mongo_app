using Matrix.Core.ConfigurationsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.DAL.CustomMongoRepositories
{
    public interface IFlagSettingRepository
    {   
        IList<FlagSetting> Get(int skip = 0, int take = -1);

        FlagSetting GetOne(string id);

        bool Save(FlagSetting entity);

        bool Delete(string id);
    }
}
