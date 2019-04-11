using AVS.CoreLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AVS.CoreLib.Tests
{
    [TestClass]
    public class ArgsParserTests
    {
        [TestMethod]
        public void UseBufferTest()
        {
            //arrange
            var args = "-p value1 -x val/complex2";

            //act
            var dict = ArgsParser.Parse(args);
            
            //assert
            Assert.AreEqual(dict.Count, 2);
            Assert.IsTrue(dict.ContainsKey("p"),"Expected the dictionary contains -p parameter");
            Assert.IsTrue(dict.ContainsValue("value1"), "Expected the dictionary contains 'value1' of the -p parameter");

            Assert.IsTrue(dict.ContainsKey("x"), "Expected the dictionary contains -x parameter");
            Assert.IsTrue(dict.ContainsValue("val/complex2"), "Expected the dictionary contains 'val/complex2' of the -x parameter");
        }
    }
}