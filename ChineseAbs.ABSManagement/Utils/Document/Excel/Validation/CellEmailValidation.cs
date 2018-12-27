using System.Text.RegularExpressions;

namespace ChineseAbs.ABSManagement.Utils.Excel
{
    public class CellEmailValidation : ICellValidation
    {
        public CellEmailValidation(bool isNullOrNot = true)
        {
            ErrorMsg = string.Empty;
            IsNullOrNot = isNullOrNot;
        }

        public bool IsValid(string cellText)
        {
            if ((!IsNullOrNot) && (string.IsNullOrWhiteSpace(cellText) || cellText == "-"))
            {
                ErrorMsg = "不能为空";
                return false;
            }

            //名称@域名
            //只允许英文字母、数字、下划线、英文句号、以及中划线组成,例如 bingxin.jiang@sail-fs.com, 1234@qq.com, sdrtgb@mp.weixin.qq.com
            //'@'前至少两个上述字符（除英文句号以外）
            Regex r = new Regex(@"^[a-zA-Z0-9_-]+[\.]?[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$");
            if (!string.IsNullOrWhiteSpace(cellText) && cellText != "-" && (!r.IsMatch(cellText))) 
            {
                ErrorMsg = "无法将[" + cellText + "]解析为电子邮箱(只允许英文字母、数字、下划线、英文句号、以及中划线组成)";
                return false;
            }

            return true;
        }

        public string ErrorMsg { get; set; }

        public bool IsNullOrNot { get; set; }
    }
}
