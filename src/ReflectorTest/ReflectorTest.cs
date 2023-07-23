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

    [TestCase("MyProperty", ExpectedResult = "public")]
    [TestCase("MyPropertyGetOnly", ExpectedResult = "public")]
    [TestCase("MyPropertySet", ExpectedResult = "public")]
    [TestCase("MyPropertyNoBacking", ExpectedResult = "public")]
    [TestCase("MyPropertyInitOnly", ExpectedResult = "public")]
    [TestCase("MyPropertyLambda", ExpectedResult = "public")]
    [TestCase("MyPropertyPrivate", ExpectedResult = "private")]
    [TestCase("MyPropertyProtected", ExpectedResult = "protected")]
    [TestCase("MyPropertyInternal", ExpectedResult = "internal")]
    [TestCase("MyPropertyProtectedInternal", ExpectedResult = "protected internal")]
    [TestCase("MyPropertyStatic", ExpectedResult = "public")]
    [TestCase("MyPropertyAbstract", ExpectedResult = "public")]
    [TestCase("MyPropertyVirtual", ExpectedResult = "public")]
    [TestCase("MyPropertySealed", ExpectedResult = "public")]
    public string Test_GetPropertyAccessModifier(string name)
    {
        var type = typeof(PropertyTest);
        var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        return property.GetPropertyAccessModifier();

    }

    [TestCase("MyProperty", ExpectedResult = "")]
    [TestCase("MyPropertyGetOnly", ExpectedResult = "")]
    [TestCase("MyPropertySet", ExpectedResult = "")]
    [TestCase("MyPropertyNoBacking", ExpectedResult = "")]
    [TestCase("MyPropertyInitOnly", ExpectedResult = "")]
    [TestCase("MyPropertyLambda", ExpectedResult = "")]
    [TestCase("MyPropertyPrivate", ExpectedResult = "")]
    [TestCase("MyPropertyProtected", ExpectedResult = "")]
    [TestCase("MyPropertyInternal", ExpectedResult = "")]
    [TestCase("MyPropertyProtectedInternal", ExpectedResult = "")]
    [TestCase("MyPropertyStatic", ExpectedResult = "static")]
    [TestCase("MyPropertyAbstract", ExpectedResult = "abstract")]
    [TestCase("MyPropertyVirtual", ExpectedResult = "virtual")]
    [TestCase("MyPropertySealed", ExpectedResult = "sealed override")]
    public string Test_GetPropertyModifiers(string name)
    {
        var type = typeof(PropertyTest);
        var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        return property.GetPropertyModifiers();

    }


    [TestCase("state", ExpectedResult = "private")]
    [TestCase("field", ExpectedResult = "public")]
    [TestCase("fieldConst", ExpectedResult = "protected")]
    [TestCase("fieldReadonly", ExpectedResult = "internal")]
    [TestCase("fieldPro", ExpectedResult = "protected internal")]
    public string Test_GetFieldAccessModifier(string name)
    {
        var type = typeof(PropertyTest);
        var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        return field.GetFieldAccessModifier();

    }


    [TestCase("field", ExpectedResult = "static")]
    [TestCase("fieldConst", ExpectedResult = "const")]
    [TestCase("fieldReadonly", ExpectedResult = "readonly")]
    [TestCase("fieldPro", ExpectedResult = "static readonly")]
    public string Test_GetFieldModifiers(string name)
    {
        var type = typeof(PropertyTest);
        var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        return field.GetFieldModifiers();

    }


    [TestCase("MyStaticMethod", ExpectedResult = "static")]
    [TestCase("MyAbstract", ExpectedResult = "abstract")]
    [TestCase("MyVirtual", ExpectedResult = "virtual")]
    [TestCase("MySealed", ExpectedResult = "sealed override")]
    [TestCase("MyMethod", ExpectedResult = "")]
    public string Test_GetMethodModifiers(string name)
    {
        var type = typeof(PropertyTest);
        var method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        return method.GetMethodModifiers();

    }


    [TestCase("MyEvent1", ExpectedResult = "public")]
    [TestCase("MyEvent4", ExpectedResult = "private")]
    [TestCase("MyEvent5", ExpectedResult = "protected")]
    [TestCase("MyEvent6", ExpectedResult = "protected internal")]
    [TestCase("MyEvent7", ExpectedResult = "internal")]
    public string Test_GetEventAccessModifier(string name)
    {
        var type = typeof(MyDerivedClass);
        var evnt = type.GetEvent(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        return evnt.GetEventAccessModifier();

    }

    [TestCase("MyEvent4", ExpectedResult = "")]
    [TestCase("MyEvent3", ExpectedResult = "static")]
    [TestCase("MyEvent2", ExpectedResult = "sealed override")]
    [TestCase("MyEvent1", ExpectedResult = "virtual")]
    public string Test_GetEventModifiers(string name)
    {
        var type = typeof(MyDerivedClass);
        var evnt = type.GetEvent(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        return evnt.GetEventModifiers();

    }

    [TestCase(true, ExpectedResult = "public")]
    [TestCase(false, ExpectedResult = "private")]
    public string Test_Get_ConstructorAccessModifier(bool isPublic)
    {
        var type = typeof(MyConstructorClass);
        var ctor = type.GetAllConstructors().First(x => x.IsPublic == isPublic && x.IsPrivate == !isPublic);
        return ctor.GetConstructorAccessModifier();

    }

    [Test]
    public void Test_Get_Protected_ConstructorAccessModifier()
    {
        var type = typeof(MyConstructorClass);
        var ctor = type.GetAllConstructors().First(x => x.IsFamily);
        var result= ctor.GetConstructorAccessModifier();
        Assert.AreEqual("protected", result);
    }

    [Test]
    public void Test_Static_GetConstructorModifiers()
    {
        var type = typeof(MyConstructorClass);
        var ctor = type.GetAllConstructors().First(c => c.IsStatic);
        var result = ctor.GetConstructorModifiers();

        Assert.AreEqual("static", result);
    }


    [TestCase(typeof(string[]), ExpectedResult = "String[]:Array,IList<String>,ICollection<String>,IEnumerable<String>,IReadOnlyList<String>,IReadOnlyCollection<String>")]
    [TestCase(typeof(List<string>), ExpectedResult = "public class List<String>:IList<String>,ICollection<String>,IEnumerable<String>,IEnumerable,IList,ICollection,IReadOnlyList<String>,IReadOnlyCollection<String>")]
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
    [TestCase(typeof(SampleRecord), ExpectedResult = "public readonly record struct SampleRecord:IEquatable<SampleRecord>")]
    [TestCase(typeof(Sample), ExpectedResult = "public readonly struct Sample")]
    [TestCase(typeof(MyEmptyRecordStruct), ExpectedResult = "public record struct MyEmptyRecordStruct:IEquatable<MyEmptyRecordStruct>")]
    [TestCase(typeof(MyStaticClass), ExpectedResult = "public static class MyStaticClass")]
    [TestCase(typeof(MyRecordAndClass), ExpectedResult = "public record MyRecordAndClass:IEquatable<MyRecordAndClass>")]
    [TestCase(typeof(MyRecord), ExpectedResult = "public record MyRecord:IEquatable<MyRecord>")]
    [TestCase(typeof(MyRecordAb), ExpectedResult = "public abstract record MyRecordAb:IEquatable<MyRecordAb>")]
    [TestCase(typeof(MyRecordSealed), ExpectedResult = "public sealed record MyRecordSealed:IEquatable<MyRecordSealed>")]
    [TestCase(typeof(MyClasses), ExpectedResult = "internal class MyClasses")]
    [TestCase(typeof(MyClassesAb), ExpectedResult = "internal abstract class MyClassesAb")]
    [TestCase(typeof(MyClassesSealed), ExpectedResult = "internal sealed class MyClassesSealed")]

    public string Test_Type_AsString(Type type)
    {
       return type.AsString().Trim();
    }

    [TestCase("MyProperty", ExpectedResult = "public  Int32 MyProperty{get;set;}")]
    [TestCase("MyPropertyGetOnly", ExpectedResult = "public  Int32 MyPropertyGetOnly{get;}")]
    [TestCase("MyPropertySet", ExpectedResult = "public  Int32 MyPropertySet{set;}")]
    [TestCase("MyPropertyNoBacking", ExpectedResult = "public  Int32 MyPropertyNoBacking{get;set;}")]
    [TestCase("MyPropertyInitOnly", ExpectedResult = "public  Int32 MyPropertyInitOnly{get;init;}")]
    [TestCase("MyPropertyLambda", ExpectedResult = "public  Int32 MyPropertyLambda{get;}")]
    [TestCase("MyPropertyPrivate", ExpectedResult = "private  Int32 MyPropertyPrivate{get;}")]
    [TestCase("MyPropertyProtected", ExpectedResult = "protected  Int32 MyPropertyProtected{get;}")]
    [TestCase("MyPropertyInternal", ExpectedResult = "internal  Int32 MyPropertyInternal{get;}")]
    [TestCase("MyPropertyProtectedInternal", ExpectedResult = "protected internal  Int32 MyPropertyProtectedInternal{get;}")]
    [TestCase("MyPropertyStatic", ExpectedResult = "public static Int32 MyPropertyStatic{get;set;}")]
    [TestCase("MyPropertyAbstract", ExpectedResult = "public abstract Int32 MyPropertyAbstract{get;set;}")]
    [TestCase("MyPropertyVirtual", ExpectedResult = "public virtual Int32 MyPropertyVirtual{get;set;}")]
    [TestCase("MyPropertySealed", ExpectedResult = "public sealed override Int32 MyPropertySealed{get;set;}")]
    public string Test_Property_AsString(string name)
    {
        var type = typeof(PropertyTest);
        var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        return property.AsString();

    }


    [TestCase("state", ExpectedResult = "private  Int32 state")]
    [TestCase("field", ExpectedResult = "public static Int32 field")]
    [TestCase("fieldConst", ExpectedResult = "protected const Int32 fieldConst")]
    [TestCase("fieldReadonly", ExpectedResult = "internal readonly Int32 fieldReadonly")]
    [TestCase("fieldPro", ExpectedResult = "protected internal static readonly Int32 fieldPro")]
    public string Test_Field_AsString(string name)
    {
        var type = typeof(PropertyTest);
        var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        return field.AsString();

    }

    [TestCase("MyStaticMethod", ExpectedResult = "public static Void MyStaticMethod()")]
    [TestCase("MyAbstract", ExpectedResult = "protected abstract Void MyAbstract()")]
    [TestCase("MyVirtual", ExpectedResult = "public virtual Void MyVirtual()")]
    [TestCase("MySealed", ExpectedResult = "public sealed override Void MySealed()")]
    [TestCase("MyMethod", ExpectedResult = "public  Void MyMethod()")]
    public string Test_Method_AsString(string name)
    {
        var type = typeof(PropertyTest);
        var method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        return method.AsString();

    }

    [TestCase("MyEvent4", ExpectedResult = "private  EventHandler MyEvent4")]
    [TestCase("MyEvent3", ExpectedResult = "public static EventHandler MyEvent3")]
    [TestCase("MyEvent2", ExpectedResult = "public sealed override EventHandler MyEvent2")]
    [TestCase("MyEvent1", ExpectedResult = "public virtual EventHandler MyEvent1")]
    public string Test_Event_AsString(string name)
    {
        var type = typeof(MyDerivedClass);
        var evnt = type.GetEvent(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        return evnt.AsString();

    }

    [TestCase(true, ExpectedResult = "public  MyConstructorClass()")]
    [TestCase(false, ExpectedResult = "private  MyConstructorClass(Int32 age)")]
    public string Test_Constructor_AsString(bool isPublic)
    {
        var type = typeof(MyConstructorClass);
        var ctor = type.GetAllConstructors().First(x => x.IsPublic == isPublic && x.IsPrivate == !isPublic);
        return ctor.AsString();

    }

    [Test]
    public void Test_Protected_Constructor_AsString()
    {
        var type = typeof(MyConstructorClass);
        var ctor = type.GetAllConstructors().First(x => x.IsFamily);
        var result = ctor.AsString();
        Assert.AreEqual("protected  MyConstructorClass(String name)", result);
    }

    [Test]
    public void Test_Static_GetConstructor_AsString()
    {
        var type = typeof(MyConstructorClass);
        var ctor = type.GetAllConstructors().First(c => c.IsStatic);
        var result = ctor.AsString();

        Assert.AreEqual("private static MyConstructorClass()", result);
    }

    [TestCase("Find", ExpectedResult = "public  T Find(Predicate<T> match)")]
    [TestCase("Exists", ExpectedResult = "public  Boolean Exists(Predicate<T> match)")]
    [TestCase("AddRange", ExpectedResult = "public  Void AddRange(IEnumerable<T> collection)")]
    [TestCase("Add", ExpectedResult = "public sealed override Void Add(T item)")]
    [TestCase("ToString", ExpectedResult = "public virtual String ToString()")]
    public string Test_Generic_Method_AsString(string name)
    {
        var type = typeof(List<>);
        var method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        return method.AsString();

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