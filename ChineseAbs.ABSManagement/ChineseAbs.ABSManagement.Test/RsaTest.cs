using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChineseAbs.ABSManagement.Utils;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class RsaTest
    {
        [Serializable]
        class RsaTestObj
        {
            public int MyProperty { get; set; }

            public List<string> MyPropertyList { get; set; }
        }

        [TestMethod]
        public void RsaTestFunction()
        {
            RsaKeys keys = RsaUtils.GenerateRsaKeys();

            RsaTestObj obj = new RsaTestObj()
            {
                MyProperty = 9527,
                MyPropertyList = new List<string>{"9527","HelloWorld"}
            };

            var text = RsaUtils.Encrypt(keys.PublicKey, obj);
            var newObj = RsaUtils.Decrypt<RsaTestObj>(keys.PrivateKey, text);

            bool result = newObj != null
                && newObj.MyProperty == obj.MyProperty
                && newObj.MyPropertyList != null
                && newObj.MyPropertyList.Count == obj.MyPropertyList.Count;
            if (result)
            {
                for (int i = 0; i < obj.MyPropertyList.Count; ++i)
                {
                    var left = obj.MyPropertyList[i];
                    var right = newObj.MyPropertyList[i];
                    if (left != right)
                    {
                        result = false;
                    }
                }
            }

            Assert.IsTrue(result);
        }
    }
}
