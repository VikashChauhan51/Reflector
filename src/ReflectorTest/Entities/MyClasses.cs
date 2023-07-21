using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectorTest.Entities;

internal class MyClasses
{
}
internal abstract class MyClassesAb
{
}
internal sealed class MyClassesSealed
{
}
internal class PropertiesAndFieldsClass
{
    public int MyIntProperty { get; set; }
    public string MyStringProperty { get; set; } = string.Empty;
    public int? MyNulableIntProperty { get; set; }
    public string? MyNulableStringProperty { get; set; } = null;
    public int MyPrivateSetIntProperty { get; private set; }
    public string MyPrivateSetStringProperty { get; private set; } = string.Empty;
    public int MyPrivateInitSetIntProperty { get; private init; }
    public string MyPrivateInitSetStringProperty { get; private init; } = string.Empty;
    public int MyProtectedInitSetIntProperty { get; protected init; }
    public string MyProtectedInitSetStringProperty { get; protected init; } = string.Empty;
    public int MyInternalInitSetIntProperty { get; internal init; }
    public string MyInternalInitSetStringProperty { get; internal init; } = string.Empty;
    public int MyInitSetIntProperty { get; init; }
    public string MyInitSetStringProperty { get; init; } = string.Empty;
    public int MyPrivateGetIntProperty { private get; init; }
    public string MyPrivateGetStringProperty { private get; init; } = string.Empty;
    public int MyProtectedGetIntProperty { protected get; init; }
    public string MyProtectedGetStringProperty { protected get; init; } = string.Empty;
    private int MyPrivateIntProperty { get; set; }
    private string MyPrivateStringProperty { get; set; } = string.Empty;
    protected int MyProtectedIntProperty { get; set; }
    protected string MyProtectedStringProperty { get; set; } = string.Empty;
    internal int MyInternalIntProperty { get; set; }
    internal string MyInternalStringProperty { get; set; } = string.Empty;

    public int MyInternalGetIntProperty { internal get; init; }
    public string MyInternalGetStringProperty { internal get; init; } = string.Empty;

    public int MyIntField;
    public string MyStringField = string.Empty;
    public int? MyNullableIntField;
    public string? MyNullableStringField = null;

    private int MyPrivateIntField;
    private string MyPrivateStringField = string.Empty;

    protected int MyProtectedIntField;
    protected string MyProtectedStringField = string.Empty;
    internal int MyInternalIntField;
    internal string MyInternalStringField = string.Empty;

    public const int MyConstIntField = 10;
    public const string MyConstStringField = "Empty";

    public readonly int MyReadonlyIntField;
    public readonly string MyReadonlyStringField = string.Empty;

    public static int MyStaticIntField;
    public static string MyStaticStringField = string.Empty;

    public static readonly int MyStaticReadonlyIntField;
    public static readonly string MyStaticReadonlyStringField = string.Empty;
}

public record MyEmptyRecord
{

}

public record MyRecord();
public record class MyRecordAndClass();
public abstract record MyRecordAb();
public sealed record MyRecordSealed();
public record struct MyEmptyRecordStruct
{

}
public record struct MyRecordStruct();

public record MyRecord2(string Name);

public static class MyStaticClass
{

}


public readonly struct Sample
{

}

public readonly record struct SampleRecord
{

}



public abstract class PropertyTest : MyBase
{
    private int state;
    public static int field;
    protected const int fieldConst = 10;
    internal readonly int fieldReadonly;
    protected internal static readonly int fieldPro = 20;

    public int MyProperty { get; set; }

    public int MyPropertyGetOnly { get; }
    public int MyPropertySet { set { state = value; } }

    public int MyPropertyNoBacking { get { return state; } set { state = value; } }

    public int MyPropertyInitOnly { get; init; }
    public int MyPropertyLambda => state;

    private int MyPropertyPrivate { get; set; }
    protected int MyPropertyProtected { get; set; }
    internal int MyPropertyInternal { get; set; }
    protected internal int MyPropertyProtectedInternal { get; set; }

    public static int MyPropertyStatic { get; set; }
    public abstract int MyPropertyAbstract { get; set; }
    public virtual int MyPropertyVirtual { get; set; }

    public sealed override int MyPropertySealed { get; set; }

    public sealed override void MySealed()
    {

    }

    public virtual void MyVirtual()
    {

    }

    public void MyMethod()
    {

    }
    public static void MyStaticMethod()
    {

    }

    protected abstract void MyAbstract();
}

public class MyBase
{
    public virtual int MyPropertySealed { get; set; }

    public virtual void MySealed()
    {

    }
}


public abstract class MyBaseClass
{
    // Abstract event
    public abstract event EventHandler MyEvent1;

    // Virtual event
    public virtual event EventHandler MyEvent2;
}

public class MyDerivedClass : MyBaseClass
{
    // Override abstract event
    public override event EventHandler MyEvent1;

    public static event EventHandler MyEvent3;
    // Override and seal virtual event
    public sealed override event EventHandler MyEvent2;

    private event EventHandler MyEvent4;
    protected event EventHandler MyEvent5;
    protected internal event EventHandler MyEvent6;
    internal event EventHandler MyEvent7;
}


public class MyConstructorClass
{
    public MyConstructorClass()
    {

    }

    private MyConstructorClass(int age)
    {

    }
    static MyConstructorClass()
    {

    }

    protected MyConstructorClass(string name)
    {

    }

    protected internal MyConstructorClass(string name, int age)
    {

    }
    internal MyConstructorClass(string name, int age, int data)
    {

    }
}