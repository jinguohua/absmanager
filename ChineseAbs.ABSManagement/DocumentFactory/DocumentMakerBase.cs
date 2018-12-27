using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using FilePattern;
using System;
using System.IO;

namespace ChineseAbs.ABSManagement.DocumentFactory
{
    public abstract class DocumentMakerBase
    {
        abstract protected object MakeObjectInstance();
        abstract protected string GetPatternFilePath();

        public object GetObjInstance(params object[] param)
        {
            //解析生成文件需要的参数
            ParseParams(param);

            return MakeObjectInstance();
        }

        public void Generate(MemoryStream ms, params object[] param)
        {
            //制作生成模板文件需要的实例
            var obj = GetObjInstance(param);

            var patternFilePath = GetPatternFilePath();

            //配置文件生成参数
            var setting = new Setting();
            setting.PatternTextStyle.SignWithColor = true;
            setting.PatternTextStyle.ForecolorName = System.Drawing.Color.Red.Name;

            //文件生成
            var wordPattern = new WordPattern(setting);
            if (!wordPattern.Generate(patternFilePath, obj, ms))
            {
                throw new ApplicationException("Generate file failed.");
            }
        }

        public DocumentMakerBase(string userName)
        {
            m_userName = userName;
        }

        protected virtual void ParseParams(params object[] param)
        {
            CommUtils.Assert(param.Length == 4, "Generate document need projectId/paymentDay/timeStamp/documentName.");
            var projectId = (int)(param[0]);
            m_paymentDay = (DateTime)(param[1]);
            m_timeStamp = (DateTime)(param[2]);
            m_docName = (string)(param[3]);
            m_project = m_dbAdapter.Project.GetProjectById(projectId);
        }

        protected string InsertHyphenBeforeNumber(string text)
        {
            for (int i = 1; i < text.Length; ++i)
            {
                if (Char.IsNumber(text[i]))
                {
                    return text.Insert(i, "-");
                }
            }

            return text;
        }

        protected string GenerateNameCNHyphen(Note note)
        {
            if (note.IsEquity)
            {
                return "次";
            }
            else
            {
                return "优先" + InsertHyphenBeforeNumber(note.ShortName);
            }
        }

        protected string m_userName;

        protected DBAdapter m_dbAdapter
        {
            get { return new DBAdapter(); }
        }

        protected Project m_project;
        protected DateTime m_paymentDay;
        protected string m_docName;
        protected DateTime m_timeStamp;
    }
}
