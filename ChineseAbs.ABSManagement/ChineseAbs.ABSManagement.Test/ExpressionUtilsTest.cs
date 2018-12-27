using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class ExpressionUtilsTest
    {
        [TestMethod]
        public void FloatingRateTest()
        {
            double d = 2 / 100.0d + 3.3 / 100.0d;
            Console.Write(d);

            var expression = new ExpressionUtils("2% + 3.3%");
            var tObj = expression.Eval<double>();
            Assert.AreEqual(tObj, 0.053);
        }


        [TestMethod]
        public void FloatingRateWithParamTest()
        {
            var expression = new ExpressionUtils("CNL003Y + 3%");
            expression.AddParam("CNL003Y", 0.0475);
            var result = expression.Eval<double>();
            Assert.AreEqual(result, 0.0775);
        }


        [TestMethod]
        public void FloatingRateWithParamTest2()
        {
            var expression = new ExpressionUtils("A + 3% + B - C + d");
            expression.AddParam("A", 0.0475);
            expression.AddParam("B", 0.0475);
            expression.AddParam("C", 0.0475);
            expression.AddParam("d", 0.0475);
            var result = expression.Eval<double>();

            result =  new ExpressionUtils("A + 3% + BBB - CC + 3 + d+BBB-BBB")
                .AddParam("A", 1)
                .AddParam("BBB", 3)
                .AddParam("CC", 5)
                .AddParam("d", 4).Eval<double>();
            Assert.AreEqual(result, 6.03);
        }
    }
}
