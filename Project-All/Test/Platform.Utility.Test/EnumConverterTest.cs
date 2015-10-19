using System;
using NUnit.Framework;
using Platform.Utility;
using System.Collections.Generic;

namespace Platform.Utility.Test
{
    [TestFixture]
    public class EnumConverterTest
    {
        [Test]
        public void TestEnumFromDescription()
        {
            //Wrong
            Assert.IsNull(EnumConverter.EnumFromDescription<Genders>("MALEE"));
            Assert.IsNull(EnumConverter.EnumFromDescription<Genders>("females"));
            Assert.IsNull(EnumConverter.EnumFromDescription<Genders>("others"));

            //Correct
            Assert.AreEqual(Genders.Male, EnumConverter.EnumFromDescription<Genders>("MALE").Value);
            Assert.AreEqual(Genders.Male, EnumConverter.EnumFromDescription<Genders>("male").Value);
            Assert.AreEqual(Genders.Female, EnumConverter.EnumFromDescription<Genders>("female").Value);
            Assert.AreEqual(Genders.Others, EnumConverter.EnumFromDescription<Genders>("other").Value);
        }
    }


    /// <summary>
    /// Used for test enum
    /// </summary>
    public enum Genders
    {
        [System.ComponentModel.Description("male")]
        Male,

        [System.ComponentModel.Description("female")]
        Female,

        [System.ComponentModel.Description("other")]
        Others
    }
}
