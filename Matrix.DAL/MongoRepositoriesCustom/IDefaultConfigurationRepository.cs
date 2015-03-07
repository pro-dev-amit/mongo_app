using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.DAL.MongoRepositoriesCustom
{
    public interface IDefaultConfigurationRepository
    {
        bool IsMasterDataSet { get; }

        void SetMasterData();

        void ClearEverything();
    }
}
