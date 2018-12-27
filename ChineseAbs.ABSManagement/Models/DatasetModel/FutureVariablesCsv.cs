using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Models.DatasetModel
{
    //public class FutureVariablesCsv : VariablesCsv
    //{
    //    public FutureVariablesCsv() : base()
    //    {
    //    }

    //    public List<InterestRateReset> GetInterestRateResets()
    //    {
    //        var varCsv = this;
    //        var keywordPostfix = ".reset";
    //        PrimeInterestRate rateType;
    //        var interestRateRecords = varCsv.Where(x =>
    //            x.Name.EndsWith(keywordPostfix, StringComparison.CurrentCultureIgnoreCase) &&
    //            Enum.TryParse<PrimeInterestRate>(x.Name.Substring(0, x.Name.Length - keywordPostfix.Length), true, out rateType)).ToList();

    //        var nowDate = DateTime.Today;
    //        var result = new List<InterestRateReset>();
    //        foreach (var record in interestRateRecords)
    //        {
    //            Enum.TryParse<PrimeInterestRate>(record.Name.Substring(0, record.Name.Length - keywordPostfix.Length), true, out rateType);

    //            var rateReset = new InterestRateReset();
    //            rateReset.Code = rateType;

    //            foreach (var paymentDate in varCsv.DateColumns)
    //            {
    //                var rateValue = MathUtils.ParseDouble(record.Items[paymentDate]);
    //                if (rateReset.ResetRecords.Count == 0 || rateReset.ResetRecords.Last().InterestRate != rateValue)
    //                {
    //                    var resetRecord = new InterestRateResetRecord();
    //                    resetRecord.Date = paymentDate;
    //                    resetRecord.InterestRate = rateValue;
    //                    rateReset.ResetRecords.Add(resetRecord);
    //                }
    //            }

    //            //查找第一个不大于今天的的利率作为今天利率
    //            rateReset.CurrentInterestRate = rateReset.ResetRecords.First().InterestRate;
    //            foreach (var resetRecord in rateReset.ResetRecords)
    //            {
    //                if (resetRecord.Date < nowDate)
    //                {
    //                    rateReset.CurrentInterestRate = resetRecord.InterestRate;
    //                }
    //            }

    //            result.Add(rateReset);
    //        }

    //        return result;
    //    }
    //}
}
