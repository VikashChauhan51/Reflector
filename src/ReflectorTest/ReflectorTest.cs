using NUnit.Framework;
using System;
using ReflectorTest.Entities;
using System.Reflection;
using Reflector;
using System.Linq;
using System.Collections.Generic;

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
    [TestCase(typeof(D), ExpectedResult = false)]
    [TestCase(typeof(A), ExpectedResult = false)]
    [TestCase(typeof(MutableUser), ExpectedResult = false)]
    [TestCase(typeof(ABCD), ExpectedResult = false)]
    [TestCase(typeof(XYZ), ExpectedResult = false)]
    [TestCase(typeof(IXYZ), ExpectedResult = true)]
    public bool IsDeepMutable_ReturnsExpectedResult(Type type)
    {
        return type.IsDeepMutable();
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
        return type.GetTypeInfo().IsMutable();
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
        return type.IsRecordClass();
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
        return type.IsRecordStruct();
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
        return type.IsRecord();
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
        var result = type.IsAnonymous();

        Assert.That(result, Is.True);
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

        return type.IsAnonymous();
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
    [TestCase(typeof(MyStaticClass), ExpectedResult = true)]
    public bool Test_Is_Static_Type(Type type)
    {
        return type.IsStatic();
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
        return type.IsReadonlyStruct();
    }

    [TestCase(typeof(PublicClass), ExpectedResult = "public")]
    [TestCase(typeof(ABA), ExpectedResult = "private")]
    [TestCase(typeof(PrivateClass), ExpectedResult = "private")]
    [TestCase(typeof(InternalClass), ExpectedResult = "internal")]
    [TestCase(typeof(ProtectedClass), ExpectedResult = "protected")]
    [TestCase(typeof(ProtectedInternalClass), ExpectedResult = "protected internal")]
    [TestCase(typeof(PrivateProtectedClass), ExpectedResult = "private protected")]
    [TestCase(typeof(XYZ), ExpectedResult = "internal")]
    [TestCase(typeof(string[]), ExpectedResult = "")]
    [TestCase(typeof(List<string>), ExpectedResult = "public")]
    [TestCase(typeof(string), ExpectedResult = "")]
    [TestCase(typeof(int), ExpectedResult = "")]
    [TestCase(typeof(object), ExpectedResult = "")]
    [TestCase(typeof(Guid), ExpectedResult = "")]
    [TestCase(typeof(UIntPtr), ExpectedResult = "")]
    [TestCase(typeof(String), ExpectedResult = "")]
    [TestCase(typeof(DateTimeOffset), ExpectedResult = "")]
    [TestCase(typeof(DateTime), ExpectedResult = "")]
    [TestCase(typeof(DateOnly), ExpectedResult = "")]
    [TestCase(typeof(TimeOnly), ExpectedResult = "")]
    [TestCase(typeof(TimeSpan), ExpectedResult = "")]
    [TestCase(typeof(Char), ExpectedResult = "")]
    [TestCase(typeof(char), ExpectedResult = "")]
    [TestCase(typeof(Boolean), ExpectedResult = "")]
    [TestCase(typeof(Double), ExpectedResult = "")]
    [TestCase(typeof(UInt32), ExpectedResult = "")]
    [TestCase(typeof(Single), ExpectedResult = "")]
    [TestCase(typeof(ValueType), ExpectedResult = "")]
    public string Test_GetAccessModifier(Type type)
    {
        return type.GetAccessModifier();
    }

    [TestCase(typeof(string[]), ExpectedResult = "")]
    [TestCase(typeof(List<string>), ExpectedResult = "class ")]
    [TestCase(typeof(string), ExpectedResult = "")]
    [TestCase(typeof(int), ExpectedResult = "")]
    [TestCase(typeof(object), ExpectedResult = "")]
    [TestCase(typeof(Guid), ExpectedResult = "")]
    [TestCase(typeof(UIntPtr), ExpectedResult = "")]
    [TestCase(typeof(String), ExpectedResult = "")]
    [TestCase(typeof(DateTimeOffset), ExpectedResult = "")]
    [TestCase(typeof(DateTime), ExpectedResult = "")]
    [TestCase(typeof(DateOnly), ExpectedResult = "")]
    [TestCase(typeof(TimeOnly), ExpectedResult = "")]
    [TestCase(typeof(TimeSpan), ExpectedResult = "")]
    [TestCase(typeof(Char), ExpectedResult = "")]
    [TestCase(typeof(char), ExpectedResult = "")]
    [TestCase(typeof(Boolean), ExpectedResult = "")]
    [TestCase(typeof(Double), ExpectedResult = "")]
    [TestCase(typeof(UInt32), ExpectedResult = "")]
    [TestCase(typeof(Single), ExpectedResult = "")]
    [TestCase(typeof(ValueType), ExpectedResult = "")]
    [TestCase(typeof(SampleRecord), ExpectedResult = "readonly record struct ")]
    [TestCase(typeof(Sample), ExpectedResult = "readonly struct ")]
    [TestCase(typeof(MyEmptyRecordStruct), ExpectedResult = "record struct ")]
    [TestCase(typeof(MyStaticClass), ExpectedResult = "static class ")]
    [TestCase(typeof(MyRecordAndClass), ExpectedResult = "record ")]
    [TestCase(typeof(MyRecord), ExpectedResult = "record ")]
    [TestCase(typeof(MyRecordAb), ExpectedResult = "abstract record ")]
    [TestCase(typeof(MyRecordSealed), ExpectedResult = "sealed record ")]
    [TestCase(typeof(MyClasses), ExpectedResult = "class ")]
    [TestCase(typeof(MyClassesAb), ExpectedResult = "abstract class ")]
    [TestCase(typeof(MyClassesSealed), ExpectedResult = "sealed class ")]

    public string Test_GetTypeModifiers(Type type)
    {
        return type.GetTypeModifiers();
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