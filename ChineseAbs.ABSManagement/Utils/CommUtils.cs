using ChineseAbs.ABSManagement.Models;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;


namespace ChineseAbs.ABSManagement.Utils
{
    public static class CommUtils
    {
        #region Split/Join

        public static readonly string Spliter = ",";

        public static readonly string[] SpliterArray = new[] { Spliter, "|" };

        public static string Join(string[] values)
        {
            return string.Join(Spliter, values);
        }

        public static string Join(List<string> values)
        {
            return string.Join(Spliter, values);
        }

        public static string Join(string firstSpliter, string secondSpliter, List<string> values)
        {
            if (values.Count <= 2)
            {
                return string.Join(secondSpliter, values);
            }
            else
            {
                var lastValue = values[values.Count-1];
                values.Remove(lastValue);
                return string.Join(firstSpliter, values) + secondSpliter + lastValue;
            }
        }

        public static string[] Split(string value)
        {
            return value.Split(SpliterArray, StringSplitOptions.RemoveEmptyEntries);
        }
        public static string[] Split(string value, string[] spliterArray)
        {
            return value.Split(spliterArray, StringSplitOptions.RemoveEmptyEntries);
        }
        #endregion

        #region PercentHelper

        public static string Percent(double value, int reserveDigit = 2)
        {
            return (value * 100.0).ToString("n" + reserveDigit) + "%";
        }

        public static string Percent(int numerator, int denominator, int reserveDigit = 2)
        {
            return Percent((double)numerator, (double)denominator, reserveDigit);
        }

        public static string Percent(double numerator, double denominator, int reserveDigit = 2)
        {
            if (denominator == 0)
            {
                return denominator.ToString("n" + reserveDigit) + "%";
            }

            return (numerator * 100.0 / denominator).ToString("n" + reserveDigit) + "%";
        }

        #endregion

        #region Value parser
        public static T ParseEnum<T>(object value) where T : struct
        {
            if (value == null)
            {
                Assert(false, "无法转换[null]为[{0}]", typeof(T).ToString());
            }

            T result;
            if (!Enum.TryParse<T>(value.ToString(), out result))
            {
                Assert(false, "无法转换[{0}]为[{1}]", value.ToString(), typeof(T).ToString());
            }

            return result;
        }

        public static List<T> ParseEnumList<T>(string value, bool removeDuplicate = false) where T : struct
        {
            var array = Split(value);
            var list = removeDuplicate ? array.Distinct().ToList() : array.ToList();
            return list.ConvertAll(x => ParseEnum<T>(x));
        }

        public static IEnumerable<Enum> GetEnumFlags(Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
            {
                if (input.HasFlag(value))
                {
                    yield return value;
                }
            }
        }

        public static bool ParseBool(string value)
        {
            var result = false;
            Assert(bool.TryParse(value, out result), "无法转换[{0}]为true/false", value);
            return result;
        }

        #endregion

        #region Reverse

        public static IEnumerable<T> FastReverse<T>(this IList<T> items)
        {
            for (int i = items.Count - 1; i >= 0; --i)
            {
                yield return items[i];
            }
        }

        #endregion

        #region Assert
        public static string FormatString(string errorMsg, params object[] args)
        {
            return args == null ? errorMsg : string.Format(errorMsg, args);
        }

        public static void Assert(bool value, string errorMsg, params object[] args)
        {
            if (!value)
            {
                throw new ApplicationException(FormatString(errorMsg, args));
            }
        }

        public static void AssertNot(bool value, string errorMsg, params object[] args)
        {
            Assert(!value, errorMsg, args);
        }

        public static void AssertEquals(object left, object right, string errorMsg, params object[] args)
        {
            bool isEqual = left == null ? right == null : left.Equals(right);
            Assert(isEqual, errorMsg, args);
        }

        public static void AssertNotNull(object value, string errorMsg, params object[] args)
        {
            Assert(value != null, errorMsg, args);
        }

        public static void AssertHasContent(IEnumerable<string> values, string errorMsg, params object[] args)
        {
            Assert(values.All(x => !string.IsNullOrWhiteSpace(x)), errorMsg, args);
        }

        public static void AssertHasContent(string value, string errorMsg, params object[] args)
        {
            Assert(!string.IsNullOrWhiteSpace(value), errorMsg, args);
        }

        public static void AssertExistFile(string filePath)
        {
            Assert(File.Exists(filePath), "找不到文件[{0}]", filePath);
        }

        public static void AssertExistFolder(string folderPath)
        {
            Assert(Directory.Exists(folderPath), "找不到文件夹[{0}]", folderPath);
        }

        public static void AssertHasPermission(string loginUserName, string checkUserName,
            PermissionObjectType objectType, string objectUid, PermissionType permissionType)
        {
            var userInfo = new UserInfo(loginUserName);
            var permissionManager = new DBAdapter().Permission;
            var hasPermission = permissionManager.HasPermission(checkUserName,
                objectUid, permissionType);
            if (!hasPermission)
            {
                var objectName = string.Empty;
                var objectTypeName = string.Empty;
                switch (objectType)
                {
                    case PermissionObjectType.ProjectSeries:
                        objectTypeName = "产品";
                        objectName = new DBAdapter().ProjectSeries.GetByGuid(objectUid).Name;
                        break;
                    case PermissionObjectType.Project:
                        objectTypeName = "产品";
                        objectName = new DBAdapter().Project.GetProjectByGuid(objectUid).Name;
                        break;
                    case PermissionObjectType.TaskGroup:
                        objectTypeName = "工作组";
                        objectName = new DBAdapter().TaskGroup.GetByGuid(objectUid).Name;
                        break;
                    case PermissionObjectType.Task:
                        objectTypeName = "工作";
                        objectName = new DBAdapter().Task.GetTask(objectUid).Description;
                        break;
                    default:
                        objectTypeName = "未知";
                        objectName = "未知";
                        break;
                }

                var action = string.Empty;
                switch (permissionType)
                {
                    case PermissionType.Read:
                        action = "读取";
                        break;
                    case PermissionType.Write:
                        action = "修改";
                        break;
                    case PermissionType.Execute:
                        action = "操作";
                        break;
                    default:
                        action = "未知";
                        break;
                }

                //var loader = new UserProfileLoader(loginUserName);
                //var checkUserDisplayName = loader.GetDisplayRealNameAndUserName(checkUserName);

                Assert(hasPermission, "用户{0}没有{1}[{2}]的[{3}]权限",
                    checkUserName, objectTypeName, objectName, action);
            }
        }


        #endregion

        #region Money helper

        /// <summary>
        /// 将金额转换为人民币大写
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static String ToCnString(this Decimal number)
        {
            var s = number.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            var d = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
            var r = Regex.Replace(d, ".", m => "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟万亿兆京垓秭穰"[m.Value[0] - '-'].ToString());
            if (r.EndsWith("元"))
                r += "整";
            return r;
        }

        public static String ToCnString(this int sequence)
        {
            decimal number = sequence;
            var s = number.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            var d = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
            var r = Regex.Replace(d, ".", m => "负元空零一二三四五六七八九空空空空空空空分角十百千万亿兆京垓秭穰"[m.Value[0] - '-'].ToString());
            if (r.EndsWith("元"))
                r = r.Substring(0, r.Length - 1);

            if (r.StartsWith("一十"))
                r = r.Substring(1);
            return r;
        }

        #endregion

        #region Json helper

        public static string ToJson(object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, obj);
            byte[] dataBytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
            var json = Encoding.UTF8.GetString(dataBytes);

            return json;
        }

        public static T FromJson<T>(string json)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return (T)serializer.ReadObject(stream);
        }

        #endregion

        #region Document helper

        //比较两个word.docx文档是否相等
        //由于word生成后的二进制文件不同，但解压后的元数据相同
        //所以此处需要先解压，再比较
        public static bool DocxEquals(Stream leftStream, Stream rightStream)
        {
            Stream left = new MemoryStream();
            leftStream.CopyTo(left);
            left.Seek(0, SeekOrigin.Begin);
            var leftZis = new ZipInputStream(left);
            ZipEntry leftEntry;

            Stream right = new MemoryStream();
            rightStream.CopyTo(right);
            right.Seek(0, SeekOrigin.Begin);
            var rightZis = new ZipInputStream(right);
            ZipEntry rightEntry;

            while (true)
            {
                leftEntry = leftZis.GetNextEntry();
                rightEntry = rightZis.GetNextEntry();
                if ((leftEntry == null) ^ (rightEntry == null))
                {
                    //压缩包中Entry数不相等
                    return false;
                }

                if (leftEntry == null)
                {
                    break;
                }

                if (leftEntry.Name != rightEntry.Name
                    || leftEntry.Size != rightEntry.Size)
                {
                    return false;
                }

                byte[] leftBytes = new byte[leftEntry.Size];
                leftZis.Read(leftBytes, 0, leftBytes.Length);

                byte[] rightBytes = new byte[rightEntry.Size];
                rightZis.Read(rightBytes, 0, rightBytes.Length);

                if (!leftBytes.SequenceEqual(rightBytes))
                {
                    return false;
                }
            }

            return true;
        }
        
        #endregion

        public static bool IsWPS(Stream zipStream)
        {
            var key = "www.wps.cn";
            List<string> searchedFiles = new List<string> { "customXml/item1.xml", "customXml/itemProps1.xml" };

            Stream stream = new MemoryStream();
            zipStream.CopyTo(stream);
            stream.Seek(0, SeekOrigin.Begin);

            using (var zipInputStream = new ZipInputStream(stream))
            {
                ZipEntry entry = null;
                while ((entry = zipInputStream.GetNextEntry()) != null)
                {
                    if (entry.IsFile && searchedFiles.Contains(entry.Name))
                    {
                        var bs = new Byte[zipInputStream.Length];
                        zipInputStream.Read(bs, 0, bs.Length);
                        var data = Encoding.Default.GetString(bs);
                        if (data.Contains(key))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        #region Container helper

        // 获取集合中元素序号（1~N）
        public static int GetSequence<T>(T[] items, T item)
        {
            return GetSequence(items.ToList(), item);
        }

        // 获取集合中元素序号（1~N）
        public static int GetSequence<T>(List<T> items, T item)
        {
            var index = items.ToList().IndexOf(item);
            return index < 0 ? index : index + 1;
        }

        // 比较两个不含重复元素的Array（无序）是否相同
        public static bool IsEqual<T>(T[] left, T[] right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            if (left.Count() != right.Count())
            {
                return false;
            }
          
            var uniqueLeft = left.Distinct();
            var uniqueRight = right.Distinct();

            return left.Count() == uniqueLeft.Count()
                && uniqueLeft.Count() == uniqueRight.Count()
                && left.Except(right).Count() == 0
                && right.Except(left).Count() == 0;
        }

        #endregion

        #region Configuration

        public static bool IsLocalDeployed()
        {
            return WebConfigUtils.LocalDeployed;
        }

        public static string GetWatermarkTitle()
        {
            return WebConfigUtils.WatermarkTitle;
        }

        public static string GetEnterpriseName()
        {
            var enterpriseName = WebConfigUtils.EnterpriseName;
            if (enterpriseName != null)
            {
                return enterpriseName.ToString();
            }

            return string.Empty;
        }

        #endregion

        #region Configuration

        public static string DefaultAvatarPath = "/Images/avatar/headerDefault.jpg";

        #endregion

        static public string CreateFolder(params string[] paths)
        {
            var folder = Path.Combine(paths);
            return CreateFolder(folder);
        }

        static public string CreateFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                try
                {
                    Directory.CreateDirectory(folder);
                }
                catch (Exception e)
                {
                    Assert(false, "Create temporary folder failed! Exception: " + e.Message);
                }
            }

            return folder;
        }

        static public string GetTemporaryRootFolder()
        {
            return Path.Combine(WebConfigUtils.RootFolder, "Temporay");
        }

        static public string CreateTemporaryFolder(string userName)
        {
            var temporaryFolder = Path.Combine(GetTemporaryRootFolder(), userName);
            CreateFolder(temporaryFolder);
            var folder = Path.Combine(temporaryFolder, DateTime.Now.ToString("[yyyy-MM-dd HH.mm.ss fff]") + "-[" + Guid.NewGuid().ToString() + "]");
            CreateFolder(folder);
            return folder;
        }

        static public void DeleteFolderAync(string folder)
        {
            CommUtils.Assert(folder.StartsWith(GetTemporaryRootFolder(), StringComparison.CurrentCultureIgnoreCase),
                "文件夹[{0}]删除失败，只能删除临时文件夹下内容", folder);

            ParallelUtils.Start(() => Directory.Delete(folder, true));
        }

        //格式化 评级字符串
        static public string FormatChineseRating(SFL.Enumerations.ZEnums.EChineseRating chineseRating)
        {
            var rating = chineseRating.ToString();
            if (rating.EndsWith("_MINUS"))
            {
                return rating.Replace("_MINUS", "-");
            }
            else if (rating.EndsWith("_PLUS"))
            {
                return rating.Replace("_PLUS", "+");
            }

            return rating;
        }

        public static string ToCnString(PrimeInterestRate interestRateType)
        {
            return m_dictPrimeInterestRateName.ContainsKey(interestRateType) ?
                m_dictPrimeInterestRateName[interestRateType] : "-";
        }

        private static readonly Dictionary<PrimeInterestRate, string> m_dictPrimeInterestRateName = new Dictionary<PrimeInterestRate, string> {
            { PrimeInterestRate.CNS003M, "三个月存款利率" },
            { PrimeInterestRate.CNS006M, "六个月存款利率" },
            { PrimeInterestRate.CNS012M, "一年期存款利率" },

            { PrimeInterestRate.CNL006M, "六个月内贷款利率" },
            { PrimeInterestRate.CNL012M, "六个月至一年贷款利率" },
            { PrimeInterestRate.CNL003Y, "一年至三年贷款利率" },
            { PrimeInterestRate.CNL005Y, "三年至五年贷款利率" },
            { PrimeInterestRate.CNL010Y, "五年以上贷款利率" },

            { PrimeInterestRate.CNHL005Y, "五年期以下（含五年）个人住房公积金贷款利率" },
            { PrimeInterestRate.CNHL010Y, "五年期以上个人住房公积金贷款利率" },

            { PrimeInterestRate.SHIBOR1M, "一个月上海银行间同业拆放利率" },
            { PrimeInterestRate.SHIBOR3M, "三个月上海银行间同业拆放利率" },
            { PrimeInterestRate.SHIBOR6M, "六个月上海银行间同业拆放利率" },
            { PrimeInterestRate.SHIBOR9M, "九个月上海银行间同业拆放利率" },
            { PrimeInterestRate.SHIBOR1Y, "一年上海银行间同业拆放利率" },
        };

        static public StringComparer StringComparerCN = StringComparer.Create(new System.Globalization.CultureInfo("zh-CN"), false);
    }
}
