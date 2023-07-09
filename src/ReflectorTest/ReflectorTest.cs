using NUnit.Framework;
using System;
using ReflectorTest.Entities;
using System.Reflection;
using Reflector;
using System.Linq;

namespace ReflectorTest
{

    public class ReflectorTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase(typeof(MyEmptyEnum), true)]
        [TestCase(typeof(MyDefaultEnum), true)]
        [TestCase(typeof(MyEnum), true)]
        [TestCase(typeof(MyByteEnum), true)]
        [TestCase(typeof(Enum), false)]
        [TestCase(typeof(Guid), false)]
        [TestCase(typeof(Int32), false)]
        public void Verify_Type_Is_Enum(Type type, bool expected)
        {

            var result = Reflector.ReflectionExtensions.IsEnum(type);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetAllConstructors_ReturnsNull_WhenTypeIsNull()
        {
            Type? type = null;
            var constructors = Reflector.ReflectionExtensions.GetAllConstructors(type);
            Assert.IsNull(constructors);
        }


        [Test]
        public void GetAllConstructors_ReturnsPublicConstructors()
        {
            var type = typeof(PersonAll);
            var constructors = Reflector.ReflectionExtensions.GetConstructors(type);
            Assert.That(constructors!.Length == 2);
        }

        [Test]
        public void GetAllConstructors_ReturnsAllConstructors_WhenTypeHasStaticConstructors()
        {
            var type = typeof(PersonAll);
            var constructors = Reflector.ReflectionExtensions.GetAllConstructors(type);
            Assert.That(constructors!.Length == 5);
            Assert.That(constructors.Count(x => x.IsStatic) == 1);
        }



        [Test]
        public void GetAllConstructors_ReturnsAllConstructors()
        {
            var type = typeof(PersonAll);
            var constructors = Reflector.ReflectionExtensions.GetAllConstructors(type);
            Assert.That(constructors!.Length == 5);
        }

        [Test]
        public void GetAllConstructors_ReturnsConstructorsOfCorrectType()
        {
            var type = typeof(Person);
            var constructors = Reflector.ReflectionExtensions.GetAllConstructors(type);
            Assert.That(constructors![0] is ConstructorInfo);
        }

        [Test]
        public void GetConstructor_AllParameters()
        {
            var type = typeof(PersonAllParm);
            var constructors = Reflector.ReflectionExtensions.GetConstructors(type);
            Assert.That(constructors!.Length == 1);
            Assert.That(constructors![0] is ConstructorInfo);
            var parameters = Reflector.ReflectionExtensions.GetAllParameters(constructors[0]);
            Assert.That(parameters!.Length == 4);

        }

        [Test]
        public void GetConstructor_RequiredParameters()
        {
            var type = typeof(PersonAllParm);
            var constructors = Reflector.ReflectionExtensions.GetConstructors(type);
            Assert.That(constructors!.Length == 1);
            Assert.That(constructors![0] is ConstructorInfo);
            var parameters = Reflector.ReflectionExtensions.GetRequiredParameters(constructors[0]);
            Assert.That(parameters!.Length == 2);

        }

        [Test]
        public void GetConstructor_ParmsParameters()
        {
            var type = typeof(PersonParm);
            var constructors = Reflector.ReflectionExtensions.GetConstructors(type);
            Assert.That(constructors!.Length == 1);
            Assert.That(constructors![0] is ConstructorInfo);
            var parameters = Reflector.ReflectionExtensions.GetRequiredParameters(constructors[0]);
            Assert.That(parameters!.Length == 1);

        }

        [Test]
        public void GetExtendedType_ReturnsAllConstructors()
        {
            var type = typeof(PersonEntened);
            var constructors = Reflector.ReflectionExtensions.GetAllConstructors(type);
            Assert.That(constructors!.Length == 1);
        }

        [Test]
        [TestCase(MyByteEnum.Second, typeof(MyByteEnum))]
        [TestCase(42, typeof(Int32))]
        [TestCase("hello", typeof(string))]
        [TestCase(null, null)]
        public void GetType_ReturnsObjectType(object type, Type expected)
        {
            Type result = Reflector.ReflectionExtensions.GetType(type);
            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase(null, true)]
        [TestCase(typeof(MyEmptyEnum?), true)]
        [TestCase(typeof(MyDefaultEnum), false)]
        [TestCase(typeof(string), true)]
        [TestCase(typeof(object), true)]
        [TestCase(typeof(Enum), true)]
        [TestCase(typeof(DateTimeOffset), false)]
        [TestCase(typeof(DateTimeOffset?), true)]
        [TestCase(typeof(Guid), false)]
        [TestCase(typeof(Guid?), true)]
        [TestCase(typeof(Int32?), true)]
        [TestCase(typeof(Int32), false)]
        [TestCase(typeof(PersonParm), true)]
        [TestCase(typeof(FirstRecord), true)]
        [TestCase(typeof(MyPoint), false)]
        [TestCase(typeof(MyPoint?), true)]
        public void Verify_Type_Is_Nullable(Type type, bool expected)
        {

            var result = Reflector.ReflectionExtensions.IsNullable(type);
            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase(typeof(MyEmptyEnum), false)]
        [TestCase(typeof(MyDefaultEnum), true)]
        [TestCase(typeof(MyEnum), true)]
        [TestCase(typeof(MyByteEnum), true)]
        [TestCase(typeof(MyEmptyEnum?), false)]
        [TestCase(typeof(MyDefaultEnum?), true)]
        [TestCase(typeof(MyEnum?), true)]
        [TestCase(typeof(MyByteEnum?), true)]
        public void Verify_GetNamesOfEnum(Type type, bool expected)
        {
            var result = Reflector.ReflectionExtensions.GetNamesOfEnum(type);
            Assert.That(result.Any() == expected);
        }

        [TestCase(typeof(int), ExpectedResult = true)]
        [TestCase(typeof(string), ExpectedResult = true)]
        [TestCase(typeof(decimal), ExpectedResult = true)]
        [TestCase(typeof(DateTime), ExpectedResult = true)]
        [TestCase(typeof(TimeSpan), ExpectedResult = true)]
        [TestCase(typeof(char), ExpectedResult = true)]
        [TestCase(typeof(bool), ExpectedResult = true)]
        [TestCase(typeof(double), ExpectedResult = true)]
        [TestCase(typeof(System.Reflection.TypeExtensions), ExpectedResult = false)]
        [TestCase(typeof(Person), ExpectedResult = false)]
        [TestCase(typeof(object), ExpectedResult = false)]
        public bool IsPrimitive_ReturnsExpectedResult(Type type)
        {
            return type.IsPrimitive();
        }


        [TestCase(typeof(Person), ExpectedResult = false)]
        [TestCase(typeof(object), ExpectedResult = true)]
        [TestCase(null, ExpectedResult = false)]
        [TestCase(typeof(ImmutableUser), ExpectedResult = false)]
        [TestCase(typeof(D), ExpectedResult = true)]
        [TestCase(typeof(A), ExpectedResult = true)]
        [TestCase(typeof(MutableUser), ExpectedResult = true)]
        [TestCase(typeof(ABCD), ExpectedResult = false)]
        [TestCase(typeof(XYZ), ExpectedResult = true)]
        [TestCase(typeof(IXYZ), ExpectedResult = true)]
        public bool IsDeepMutable_ReturnsExpectedResult(Type type)
        {
            return type.IsDeepMutable();
        }


        [TestCase(typeof(Person), ExpectedResult = false)]
        [TestCase(typeof(object), ExpectedResult = true)]
        [TestCase(typeof(ImmutableUser), ExpectedResult = false)]
        [TestCase(typeof(D), ExpectedResult = false)]
        [TestCase(typeof(A), ExpectedResult = true)]
        [TestCase(typeof(MutableUser), ExpectedResult = true)]
        [TestCase(typeof(ABCD), ExpectedResult = false)]
        [TestCase(typeof(XYZ), ExpectedResult = true)]
        [TestCase(typeof(IXYZ), ExpectedResult = true)]
        public bool IsMutable_ReturnsExpectedResult(Type type)
        {
            return type.GetTypeInfo().IsMutable();
        }
    }
}