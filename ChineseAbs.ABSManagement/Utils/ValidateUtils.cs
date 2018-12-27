using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChineseAbs.ABSManagement.Utils
{
    public class ValidateUtils
    {
        public static Validation Create(IEnumerable<string> values, string fieldName)
        {
            return new Validation(values, fieldName);
        }

        public static Validation Create(string value, string fieldName)
        {
            return new Validation(value, fieldName);
        }

        public static Validation Name(IEnumerable<string> values, string fieldName, int maxLength = 30)
        {
            return Length(values, fieldName, maxLength);
        }

        public static Validation Description(IEnumerable<string> values, string fieldName, int maxLength = 500)
        {
            return Length(values, fieldName, maxLength);
        }

        public static Validation Name(string value, string fieldName, int maxLength = 30)
        {
            return Length(value, fieldName, maxLength);
        }

        public static Validation Description(string value, string fieldName, int maxLength = 500)
        {
            return Length(value, fieldName, maxLength);
        }

        public static Validation FileName(IEnumerable<string> values, string fieldName)
        {
            return new Validation(values, fieldName).FileName();
        }

        public static Validation FileName(string value, string fieldName)
        {
            return new Validation(value, fieldName).FileName();
        }

        public static Validation Length(IEnumerable<string> values, string fieldName, int maxLength)
        {
            return new Validation(values, fieldName).Length(maxLength);
        }

        public static Validation Length(string value, string fieldName, int maxLength)
        {
            return new Validation(value, fieldName).Length(maxLength);
        }

        public static Validation Date(string value, string fieldName)
        {
            return new Validation(value, fieldName).Date();
        }
    }

    public class Validation
    {
        public Validation(IEnumerable<string> values, string fieldName)
        {
            m_values = values.ToList();
            m_fieldName = fieldName;
        }

        public Validation(string value, string fieldName)
        {
            m_values = new List<string> { value };
            m_fieldName = fieldName;
        }

        public Validation Name()
        {
            return Length(30);
        }

        public Validation Description()
        {
            return Length(500);
        }

        public Validation Length(int maxLength)
        {
            m_values.ForEach(value => {
                CommUtils.AssertHasContent(value, "[{0}]不能为空", m_fieldName);
                CommUtils.Assert(value.Length <= maxLength, "[{0}]不能超过{1}个字符数", m_fieldName, maxLength);
            });
            return this;
        }

        public Validation FileName()
        {
            m_values.ForEach(value =>
                CommUtils.Assert(value.IndexOfAny(Path.GetInvalidFileNameChars()) < 0,
                "{0}[{1}]不能包含特殊字符/\\:*?\"<>|", m_fieldName, value));
            return this;
        }

        public Validation Date()
        {
            m_values.ForEach(value =>
                CommUtils.Assert(DateUtils.IsDate(value),
                "无法解析[{0}={1}]为日期", m_fieldName, value));
            return this;
        }

        private List<string> m_values;
        private string m_fieldName;
    }
}
