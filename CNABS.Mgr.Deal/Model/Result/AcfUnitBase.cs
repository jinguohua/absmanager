using System.Collections.Generic;
using System.Linq;

namespace CNABS.Mgr.Deal.Model.Result
{
    /// <summary>
    /// 基本的资产相关数据 / 多期多笔资产的合计数据
    /// </summary>
    public class AcfUnitBase
    {
        public AcfUnitBase DeepCopy()
        {
            return new AcfUnitBase
            {
                Principal = Principal,
                Interest = Interest,
                Performing = Performing,
                Loss = Loss,
                Defaulted = Defaulted,
                Fee = Fee,
            };
        }

        public void SetSumOf(IEnumerable<AcfUnitBase> units)
        {
            Principal = units.Sum(x => x.Principal);
            Interest = units.Sum(x => x.Interest);
            Performing = units.Sum(x => x.Performing);
            Loss = units.Sum(x => x.Loss);
            Defaulted = units.Sum(x => x.Defaulted);
            Fee = units.Sum(x => x.Fee);
        }

        public void SetValue(string name, string value)
        {
            double money;
            if (!double.TryParse(value, out money))
            {
                return;
            }

            if (name == "Interest")
            {
                Interest = money;
            }
            else if (name == "Principal")
            {
                Principal = money;
            }
            else if (name == "Performing")
            {
                Performing = money;
            }
            else if (name == "Loss")
            {
                Loss = money;
            }
            else if (name == "Defaulted")
            {
                Defaulted = money;
            }
            else if (name == "Received")
            {
                Received = money;
            }
            else if (name == "Fee")
            {
                Fee = money;
            }
        }

        public double Interest { get; set; }

        public double Principal { get; set; }

        public double Performing { get; set; }

        public double Loss { get; set; }

        public double Defaulted { get; set; }

        public double Received { get; set; }

        public double Fee { get; set; }

        public double Sum { get { return Interest + Principal; } }
    }
}
