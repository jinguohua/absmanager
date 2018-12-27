using ABS.Infrastructure;
using SAFS.Core.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum enj)
        {
            return GetDescription(enj.GetType(), enj.ToString());
        }

        public static string GetDescription(Type t, string name)
        {
            var field = t.GetField(name);
            if (field == null)
                return "";
            var des = field.GetCustomAttribute<DescriptionAttribute>();
            string disName = "";
            if (des != null)
            {
                disName = des.Description;
            }
            if (!String.IsNullOrEmpty(disName))
                return disName;
            else
                return name;
        }

        public static Dictionary<string, string> GetItems(Type enumType)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            var names = Enum.GetNames(enumType);
            foreach (var name in names)
            {
                values.Add(((int)Enum.Parse(enumType, name)).ToString(), GetDescription(enumType, name));
            }
            return values;
        }
    }

    public class EnumItem
    {
        public int Value { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        //public Type EnumType { get; set; }

        public override string ToString()
        {
            return $"Value: {Value}, Name: {Name}, Description: {Description}";
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }

    public class EnumHelper
    {
        public static IAssemblyFinder AssemberFinder { get; set; }

        static Dictionary<Type, List<EnumItem>> Init()
        {
            Dictionary<Type, List<EnumItem>> enums = new Dictionary<Type, List<EnumItem>>();
            var assemblies = AssemberFinder.FindAll();
            assemblies.ToList().ForEach(ass => {
                ass.GetTypes().Where(o => o.IsEnum).ToList().ForEach(
                        item => 
                            enums.Add(item,  ConvertEnumToItem(item)));
            });
            return enums;
        }

        public static Dictionary<Type, List<EnumItem>> Enums
        {
            get
            {
                return CacheHelper.Get<Dictionary<Type, List<EnumItem>>>("EnumCaches", Init);
            }
        }

        static List<EnumItem> ConvertEnumToItem(Type enumType)
        {
            List<EnumItem> items = new List<EnumItem>();
            var names = enumType.GetEnumNames();
            foreach(var name in names)
            {
                int value = (int)Enum.Parse(enumType, name, true);
                var des = EnumExtension.GetDescription(enumType, name);
                items.Add(new EnumItem() { Value = value, Name = name, Description = des  });
            }
            return items;
        }
    }
}
