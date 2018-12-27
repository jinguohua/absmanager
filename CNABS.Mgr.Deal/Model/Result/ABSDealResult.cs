using System.Data;

namespace CNABS.Mgr.Deal.Model.Result
{
    public class ABSDealResult
    {
        public ABSDealResult()
        {
        }

        /// <summary>
        /// 根据存续期管理规则格式化后的资产端现金流
        /// </summary>
        public DataTable Acf { get; set; }

        /// <summary>
        /// Nancy返回的原始资产端现金流
        /// </summary>
        public DataTable OriginAcf { get; set; }

        /// <summary>
        /// 根据存续期管理规则格式化后的证券端现金流
        /// </summary>
        public DataTable Cf { get; set; }

        /// <summary>
        /// Nancy返回的原始证券端现金流
        /// </summary>
        public DataTable OriginCf { get; set; }

        /// <summary>
        /// 格式化后的资产端数据
        /// </summary>
        public AcfResult AcfResult { get; set; }
    }
}
