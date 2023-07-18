using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectorTest.Entities;

internal class MyClasses
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

public record struct MyEmptyRecordStruct
{

}
public record struct MyRecordStruct();

public record MyRecord2(string Name);

public static class MyStaticClass
{

}
