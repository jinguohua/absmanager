using ABS.AssetManagement.Objects;
using Newtonsoft.Json;
using SAFS.Core;
using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.AssetManagement.Services
{
    public class AssetConfigService : ServiceBase
    {
        const string AssetConfigCacheKey = "AssetConfigs.CacheKey";

        public IRepository<Models.AssetRawDataConfig, int> DbConfiges { get; set; }

        List<ColumnsConfig> Configes
        {
            get
            {
                return Infrastructure.CacheHelper.Get(AssetConfigCacheKey, LoadConfiges);

            }
        }

        public Dictionary<int, string> ConfigNames
        {
            get
            {
                return Configes.Select(o => new { o.Id, o.Name }).ToDictionary(o => o.Id, o => o.Name);
            }
        }

        public List<ColumnConfigItem> GetColumnItems(int id)
        {
            var item = Configes.FirstOrDefault(o => o.Id == id);
            if (item != null)
                return item.Items ?? new List<ColumnConfigItem>();
            else
                return new List<ColumnConfigItem>();
        }

        private List<ColumnsConfig> LoadConfiges()
        {
            var configes = LoadFromDb();
            var fileConfiges = LoadConfigFiles();
            var existCodes = configes.Select(o => o.Code).ToList();
            var newConfiges = fileConfiges.Where(o => !existCodes.Contains(o.Code));

            var dbconfiges = newConfiges.Select(o => new Models.AssetRawDataConfig() { Code = o.Code, Name = o.Name, Creator = "import" }).ToList();
            DbConfiges.Insert(dbconfiges);
            configes = LoadFromDb();

            foreach(var conf in configes)
            {
                var item = fileConfiges.FirstOrDefault(o => o.Code == conf.Code);
                if (item != null)
                    conf.Items = item.Items;
            }
            return configes;
        }

        private List<ColumnsConfig> LoadFromDb()
        {
            return DbConfiges.NoTrackingEntities.Select(o => new ColumnsConfig { Id = o.Id, Code = o.Code, Name = o.Name }).ToList();
        }

        private List<ColumnsConfig> LoadConfigFiles()
        {
            string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "assets");
            var files = Directory.GetFiles(filepath, "*.json");
            List<ColumnsConfig> values = new List<ColumnsConfig>();
            files.ToList().ForEach(o =>
            {
                var code = Path.GetFileNameWithoutExtension(o);
                try
                {
                    var items = JsonConvert.DeserializeObject<List<ColumnConfigItem>>(File.ReadAllText(o));

                    values.Add(new ColumnsConfig() { Code = code, Items = items });
                }
                catch
                {
                    
                }
            });
            return values;
        }


        public AssetConfigService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
