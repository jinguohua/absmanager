namespace ChineseAbs.ABSManagement.AssetEvent
{
    /// <summary>
    /// 提前偿付本金分配类型
    /// </summary>
    public enum PrepayDistrubutionType
    {
        /// <summary>
        /// 未定义
        /// </summary>
        Undefined,
        /// <summary>
        /// 从本期开始，偿还本金逐渐减少，直到最后一期
        /// </summary>
        CurrentToLast,
        /// <summary>
        /// 从最后一期开始，偿还本金逐渐减少，直到本期
        /// </summary>
        LastToCurrent,
        /// <summary>
        /// 每期减少本金 = 提前偿付本金 / 本期到最后一期期数
        /// </summary>
        Average,
        /// <summary>
        /// 每期减少本金 = 提前偿付本金 * 每期偿付本金 / 所有期未偿付本金和
        /// </summary>
        EqualRatio,
        /// <summary>
        /// 自定义每期减少本金数
        /// </summary>
        Custom
    }

    public class AssetEventPrepayment : AssetEventBase
    {
        public AssetEventPrepayment(string userName):
            base(userName)
        {
        }

        public override bool PrevExecute()
        {
            return true;
        }

        public override bool PrevRevert()
        {
            return true;
        }

        public override bool Execute()
        {
            return true;
        }

        public override bool Revert()
        {
            return true;
        }
    }
}
