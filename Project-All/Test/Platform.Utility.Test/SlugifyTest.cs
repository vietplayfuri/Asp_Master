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
    class SlugifyTest
    {
        [Test]
        public void GenerateSlugTest()
        {
            string test = "the day you went away 123";
            Assert.AreEqual("the-day-you-went-away-123", test.GenerateSlug(100));
            test = String.Empty;
            Assert.AreEqual(string.Empty, test.GenerateSlug());
            test = null;
            Assert.AreEqual(string.Empty, test.GenerateSlug());
        }
    }
}
