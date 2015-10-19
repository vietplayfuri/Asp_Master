using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Platform.Utility;
using System.Net;

namespace Platform.Utility.Test
{
    [TestFixture]
    class IPAddressExtensionTest
    {
        [Test]
        public void TestGetDefaultCountryCode()
        {
            IPAddress ip;
            IPAddress.TryParse("127.0.0.1",out ip);
            Assert.AreEqual("SG", ip.GetDefaultCountryCode());

            IPAddress.TryParse(" ", out ip);
            Assert.AreEqual("SG", ip.GetDefaultCountryCode());

            IPAddress.TryParse("192.168.1.1", out ip);
            Assert.AreEqual("SG", ip.GetDefaultCountryCode());

            IPAddress.TryParse("ipadress", out ip);
            Assert.AreEqual("SG", ip.GetDefaultCountryCode());

            IPAddress.TryParse("1.1.1.1", out ip);
            Assert.AreEqual("SG", ip.GetDefaultCountryCode());

            IPAddress.TryParse("192.168.0", out ip);
            Assert.AreEqual("SG", ip.GetDefaultCountryCode());

        }

        [Test]
        public void TestGetDefaultCountryName()
        {
            IPAddress ip;
            IPAddress.TryParse("127.0.0.1", out ip);
            Assert.AreEqual("Singapore", ip.GetDefaultCountryName());

            IPAddress.TryParse(" ", out ip);
            Assert.AreEqual("Singapore", ip.GetDefaultCountryName());

            IPAddress.TryParse("192.168.1.1", out ip);
            Assert.AreEqual("Singapore", ip.GetDefaultCountryName());

            IPAddress.TryParse("ipadress", out ip);
            Assert.AreEqual("Singapore", ip.GetDefaultCountryName());

            IPAddress.TryParse("1.1.1.1", out ip);
            Assert.AreEqual("Singapore", ip.GetDefaultCountryName());

            IPAddress.TryParse("192.168.0", out ip);
            Assert.AreEqual("Singapore", ip.GetDefaultCountryName());

        }

        [Test]
        public void TestGetDefaultIp()
        {
            Assert.AreEqual("118.189.35.242", IPAdressExtension.GetDefaultIp());
        }

        //[Test]
        //public void TestGetCountryCode()
        //{
        //    IPAddress ip;
        //    string countryCode = "";
        //    string countryName = "";
        //    IPAddress.TryParse("127.0.0.1", out ip);
        //    ip.GetCountryCode(c => countryCode = c, n => countryName = n);
        //    Assert.AreEqual("SG", countryCode);
        //    Assert.AreEqual("Singapore", countryName);
        //}
        //
    }
}
