using NUnit.Framework;
using System;
using ReflectorTest.Entities;
using System.Reflection;
using System.Linq;
using VReflector;

namespace ReflectorTest;


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

        var result = IsType.IsEnum(type);
        Assert.AreEqual(expected, result);
    }

    [Test]
    public void GetAllConstructors_ReturnsNull_WhenTypeIsNull()
    {
        Type? type = null;
        var constructors = type.GetAllConstructors();
        Assert.IsNull(constructors);
    }


    [Test]
    public void GetAllConstructors_ReturnsPublicConstructors()
    {
        var type = typeof(PersonAll);
        var constructors = type.GetConstructors();
        Assert.That(constructors!.Length == 2);
    }

    [Test]
    public void GetAllConstructors_ReturnsAllConstructors_WhenTypeHasStaticConstructors()
    {
        var type = typeof(PersonAll);
        var constructors = type.GetAllConstructors();
        Assert.That(constructors!.Length == 5);
        Assert.That(constructors.Count(x => x.IsStatic) == 1);
    }



    [Test]
    public void GetAllConstructors_ReturnsAllConstructors()
    {
        var type = typeof(PersonAll);
        var constructors = type.GetAllConstructors();
        Assert.That(constructors!.Length == 5);
    }

    [Test]
    public void GetAllConstructors_ReturnsConstructorsOfCorrectType()
    {
        var type = typeof(Person);
        var constructors = type.GetAllConstructors();
        Assert.That(constructors![0] is ConstructorInfo);
    }

    [Test]
    public void GetConstructor_AllParameters()
    {
        var type = typeof(PersonAllParm);
        var constructors = type.GetConstructors();
        Assert.That(constructors!.Length == 1);
        Assert.That(constructors![0] is ConstructorInfo);
        var parameters = constructors[0].GetAllParameters();
        Assert.That(parameters!.Length == 4);

    }

    [Test]
    public void GetConstructor_RequiredParameters()
    {
        var type = typeof(PersonAllParm);
        var constructors = type.GetConstructors();
        Assert.That(constructors!.Length == 1);
        Assert.That(constructors![0] is ConstructorInfo);
        var parameters = constructors[0].GetRequiredParameters();
        Assert.That(parameters!.Length == 2);

    }

    [Test]
    public void GetConstructor_ParmsParameters()
    {
        var type = typeof(PersonParm);
        var constructors = type.GetConstructors();
        Assert.That(constructors!.Length == 1);
        Assert.That(constructors![0] is ConstructorInfo);
        var parameters = constructors[0].GetRequiredParameters();
        Assert.That(parameters!.Length == 1);

    }

    [Test]
    public void GetExtendedType_ReturnsAllConstructors()
    {
        var type = typeof(PersonEntened);
        var constructors = type.GetAllConstructors();
        Assert.That(constructors!.Length == 1);
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

        var result = IsType.IsNullable(type);
        Assert.AreEqual(expected, result);
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
        return IsType.Primitive(type);
    }


    [TestCase(typeof(Person), ExpectedResult = false)]
    [TestCase(typeof(object), ExpectedResult = true)]
    [TestCase(null, ExpectedResult = false)]
    [TestCase(typeof(ImmutableUser), ExpectedResult = false)]
    [TestCase(typeof(D), ExpectedResult = false)]
    [TestCase(typeof(A), ExpectedResult = false)]
    [TestCase(typeof(MutableUser), ExpectedResult = false)]
    [TestCase(typeof(ABCD), ExpectedResult = false)]
    [TestCase(typeof(XYZ), ExpectedResult = false)]
    [TestCase(typeof(IXYZ), ExpectedResult = true)]
    public bool IsDeepMutable_ReturnsExpectedResult(Type type)
    {
        return IsType.IsDeepMutable(type);
    }


    [TestCase(typeof(Person), ExpectedResult = false)]
    [TestCase(typeof(object), ExpectedResult = true)]
    [TestCase(typeof(ImmutableUser), ExpectedResult = false)]
    [TestCase(typeof(D), ExpectedResult = false)]
    [TestCase(typeof(A), ExpectedResult = false)]
    [TestCase(typeof(MutableUser), ExpectedResult = false)]
    [TestCase(typeof(ABCD), ExpectedResult = false)]
    [TestCase(typeof(XYZ), ExpectedResult = false)]
    [TestCase(typeof(IXYZ), ExpectedResult = true)]
    public bool IsMutable_ReturnsExpectedResult(Type type)
    {
        return IsType.IsMutable(type.GetTypeInfo());
    }

    [TestCase(typeof(Person), ExpectedResult = false)]
    [TestCase(typeof(object), ExpectedResult = false)]
    [TestCase(typeof(ImmutableUser), ExpectedResult = false)]
    [TestCase(typeof(IXYZ), ExpectedResult = false)]
    [TestCase(typeof(MyEmptyRecord), ExpectedResult = true)]
    [TestCase(typeof(MyRecord), ExpectedResult = true)]
    [TestCase(typeof(MyEmptyRecordStruct), ExpectedResult = false)]
    [TestCase(typeof(MyRecordStruct), ExpectedResult = false)]
    public bool IsRecordClass_ReturnsExpectedResult(Type type)
    {
        return IsType.RecordClass(type);
    }

    [TestCase(typeof(Person), ExpectedResult = false)]
    [TestCase(typeof(object), ExpectedResult = false)]
    [TestCase(typeof(ImmutableUser), ExpectedResult = false)]
    [TestCase(typeof(IXYZ), ExpectedResult = false)]
    [TestCase(typeof(MyEmptyRecord), ExpectedResult = false)]
    [TestCase(typeof(MyRecord), ExpectedResult = false)]
    [TestCase(typeof(MyEmptyRecordStruct), ExpectedResult = true)]
    [TestCase(typeof(MyRecordStruct), ExpectedResult = true)]
    public bool IsRecordStruct_ReturnsExpectedResult(Type type)
    {
        return IsType.RecordStruct(type);
    }

    [TestCase(typeof(Person), ExpectedResult = false)]
    [TestCase(typeof(object), ExpectedResult = false)]
    [TestCase(typeof(ImmutableUser), ExpectedResult = false)]
    [TestCase(typeof(IXYZ), ExpectedResult = false)]
    [TestCase(typeof(MyEmptyRecord), ExpectedResult = true)]
    [TestCase(typeof(MyRecord), ExpectedResult = true)]
    [TestCase(typeof(MyEmptyRecordStruct), ExpectedResult = true)]
    [TestCase(typeof(MyRecordStruct), ExpectedResult = true)]
    public bool IsRecord_ReturnsExpectedResult(Type type)
    {
        return IsType.Record(type);
    }

    [Test]
    public void Test_IsAnonymous_Type()
    {
        // Create an anonymous type.
        var anonType = new
        {
            Name = "Bill",
            Age = 30
        };
        var type = anonType.GetType();
        var result = IsType.IsAnonymous(type);

        Assert.That(result);
    }

    [TestCase(typeof(Person), ExpectedResult = false)]
    [TestCase(typeof(object), ExpectedResult = false)]
    [TestCase(typeof(ImmutableUser), ExpectedResult = false)]
    [TestCase(typeof(IXYZ), ExpectedResult = false)]
    [TestCase(typeof(MyEmptyRecord), ExpectedResult = false)]
    [TestCase(typeof(MyRecord), ExpectedResult = false)]
    [TestCase(typeof(MyEmptyRecordStruct), ExpectedResult = false)]
    [TestCase(typeof(MyRecordStruct), ExpectedResult = false)]
    [TestCase(typeof(MyRecord2), ExpectedResult = false)]
    public bool Test_Is_Not_Anonymous_Type(Type type)
    {

        return IsType.IsAnonymous(type);
    }

    [TestCase(typeof(Person), ExpectedResult = false)]
    [TestCase(typeof(object), ExpectedResult = false)]
    [TestCase(typeof(ImmutableUser), ExpectedResult = false)]
    [TestCase(typeof(IXYZ), ExpectedResult = false)]
    [TestCase(typeof(MyEmptyRecord), ExpectedResult = false)]
    [TestCase(typeof(MyRecord), ExpectedResult = false)]
    [TestCase(typeof(MyEmptyRecordStruct), ExpectedResult = false)]
    [TestCase(typeof(MyRecordStruct), ExpectedResult = false)]
    [TestCase(typeof(MyRecord2), ExpectedResult = false)]
    [TestCase(typeof(MyStaticClass), ExpectedResult = false)]
    [TestCase(typeof(Sample), ExpectedResult = true)]
    [TestCase(typeof(SampleRecord), ExpectedResult = true)]
    public bool Test_Is_Readonly_Struct(Type type)
    {
        return IsType.IsReadonlyStruct(type);
    }

    public class PublicClass
    {
    }

    private class PrivateClass
    {
    }

    internal class InternalClass
    {
    }

    protected class ProtectedClass
    {
    }

    protected internal class ProtectedInternalClass
    {
    }

    private protected class PrivateProtectedClass
    {
    }

    class ABA
    {

    }
}

class XYZ
{

}