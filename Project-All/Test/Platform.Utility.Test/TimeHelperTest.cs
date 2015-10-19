using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Platform.Utility;


namespace Platform.Utility.Test
{
    [TestFixture]
    class TimeHelperTest
    {
        [Test]
        public void TestTimeHelper()
        {
            DateTime dt = DateTime.UtcNow;

            var result = TimeHelper.convertDatetimeToCountryTimezone(dt, "SG");

            Assert.AreEqual(DateTime.Now.Hour + 1, result.Hour);

            //invalid timezone
            result = TimeHelper.convertDatetimeToCountryTimezone(dt, "SGh");
            Assert.AreEqual(new DateTime(), result);
        }

        [Test]
        public void TestLocalizeDatetime() { 
               

        }
    }
}
