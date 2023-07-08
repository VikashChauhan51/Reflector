using NUnit.Framework;
using System;
using ReflectorTest.Entities;
using static Reflector.Reflector;

namespace ReflectorTest
{

    public class ReflectorTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase(typeof(MyEmptyEnum))]
        [TestCase(typeof(MyDefaultEnum))]
        [TestCase(typeof(MyEnum))]
        [TestCase(typeof(MyByteEnum))]
        public void Verify_Type_Is_Enum(Type type)
        {
            
            var result= IsEnum(type);
            Assert.IsTrue(result,$"{type.Name} is not Enum type");  
        }
    }
}