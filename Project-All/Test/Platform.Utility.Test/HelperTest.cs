using System;
using NUnit.Framework;
using Platform.Utility;
using System.Collections.Generic;
using System.IO;

namespace Platform.Utility.Test
{
    [TestFixture]
    public class HelperTest
    {
        [Test]
        public void TestIsEmailValid()
        {
            string email = "tester1@gmail.com";
            Assert.AreEqual(true, Helper.IsEmailValid(email));

            Assert.AreEqual(true, Helper.IsEmailValid("tester@gmail.com.vn"));
        }

        [Test]

        public void TestdisplayDecimal()
        {
            Assert.AreEqual("10.1001", Helper.displayDecimal(10.1001m));
            Assert.AreEqual("10.0001", Helper.displayDecimal(10.0001m));
            Assert.AreEqual("10", Helper.displayDecimal(10.000m));
            Assert.AreEqual("0.01", Helper.displayDecimal(0.01m));
            Assert.AreEqual("0", Helper.displayDecimal(0.000m));
        }

        [Test]
        public void TestUppercaseFirst()
        {
            Assert.AreEqual("Test", Helper.UppercaseFirst("test"));
            Assert.AreEqual("TT", Helper.UppercaseFirst("TT"));
            Assert.AreEqual("Tt this", Helper.UppercaseFirst("  Tt this  "));
        }

        [Test]
        public void TestOnewayHash()
        {
            Assert.AreEqual(true, !String.IsNullOrEmpty(Helper.OnewayHash("this hashed code")));
            Assert.AreEqual(true, String.IsNullOrEmpty(Helper.OnewayHash(null)));
            Assert.AreEqual(true, String.IsNullOrEmpty(Helper.OnewayHash(String.Empty)));
        }

        [Test]
        public void TestSample()
        {

            var test = new List<string>() { "One", "Two", "Three", "four", "test" };
            List<string> list = (List<string>)test.Sample(3);
            Assert.AreEqual(3, list.Count);

            Assert.AreEqual(true, test.Contains(list[0]));
            Assert.AreEqual(true, test.Contains(list[1]));
            Assert.AreEqual(true, test.Contains(list[2]));
        }

        [Test]
        public void TestFindCountryName()
        {
            var filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/Data/country/countries-20140629.csv";
            Assert.AreEqual("Singapore", Helper.FindCountryName("SG", filePath));
            Assert.AreEqual("Viet Nam", Helper.FindCountryName("VN", filePath));
        }

    }
}
