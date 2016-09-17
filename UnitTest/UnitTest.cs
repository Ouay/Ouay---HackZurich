using System;
using Ouay_HackZurich;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Ouay_HackZurich.BlueMix;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
			BlueMixCom.SendEntrance(DateTime.Now); 
        }
    }
}
