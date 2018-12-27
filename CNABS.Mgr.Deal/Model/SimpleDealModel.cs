using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNABS.Mgr.Deal.Model
{
    public class SimpleDealModel
    {
        public SimpleDealModel()
        {
            this.BasicInformation = new BasicInformation();
            this.CollateralRule = new CollateralRule();
            this.Fees = new FeeRule();
            this.Notes = new NoteRule();

            this.Schedule = new ScheduleRule();
            this.CreditEnhancement = new CreditEnhancementRule();

            this.SplitInterestAndPrincipalProceeds = true;

        }

       // public bool UseCustomCashflow { get; set; }

        public BasicInformation BasicInformation { get; set; }

        public CollateralRule CollateralRule { get; set; }

        public FeeRule Fees { get; set; }

        public NoteRule Notes { get; set; }

        public ScheduleRule Schedule { get; set; }

        public CreditEnhancementRule CreditEnhancement { get; set; }

        public ScenarioRule ScenarioRule { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static SimpleDealModel Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<SimpleDealModel>(json);
        }

        public bool SplitInterestAndPrincipalProceeds { get; set; }
    }

    [PetaPoco.TableName("lab.Model")]
    [PetaPoco.PrimaryKey("model_id")]
    public class ModelJson
    {
        public int model_id { get; set; }

        [PetaPoco.Column("project_id")]
        public int ProjectId { get; set; }

        [PetaPoco.Column("model_json")]
        public string Json { get; set; }

        [PetaPoco.Column("update_time")]
        public DateTime UpdateTime { get; set; }
    }
}
