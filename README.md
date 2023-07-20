# VReflector
Reflection `Type` helper utility.
  

## Basic usage

```C#

// Check type is mutable or not.
public class ImmutableUser
{

    public readonly int Id;
    public string Name { get; init; }

    public static readonly int status;
    public static string Message { get; }
}

public class MutableUser
{

    public readonly int Id;
    public string Name { get; init; }
    public static string Message { get; private set; }
}

var result1 = typeof(ImmutableUser).GetTypeInfo().IsMutable();

var result2 = typeof(MutableUser).GetTypeInfo().IsMutable();

```

```C#

// Deep check type is mutable or not.
public abstract class A  
{
    public int Id { get; private set; }
    public string? Name { get; init; }
}

public class B : A
{

}
public class C : B
{
    public A? MyProperty { get; }
}
public class D : C
{
    public B? MyProperty2 { get; }
    public IA? MyProperty3 { get; init; }
}

var result1 = typeof(D).GetTypeInfo().IsMutable();

var result2 = typeof(D).IsDeepMutable();

```

```C#

// Get all parents.
public abstract class A  
{
    public int Id { get; private set; }
    public string? Name { get; init; }
}

public class B : A
{

}
public class C : B
{
    public A? MyProperty { get; }
}
public class D : C
{
    public B? MyProperty2 { get; }
    public IA? MyProperty3 { get; init; }
}

var result = typeof(D).GetParentTypes();


```
## Check static class
``` C#
public static class MyStaticClass
{

}
typeof(MyStaticClass).IsStatic()  //true

```

## Check record class
``` C#
public record MyEmptyRecord
{

}
typeof(MyEmptyRecord).IsRecordClass()  //true

```

## Check anonymous class
``` C#
// Create an anonymous type.
var anonType = new
{
    Name = "Bill",
    Age = 30
};
var type = anonType.GetType();
var result = type.IsAnonymous(); //true

```

## Get Class,Enum and Struct Access modifier and modifiers
``` C#
// Create an anonymous type.
protected internal abstract class PublicClass
{
}
var type = typeof(PublicClass);
var result = type.GetAccessModifier(); // protected internal
var info = type.GetTypeModifiers();  // abstract class

```

## Get Method Access modifier and modifiers
``` C#
// Create an anonymous type.
protected internal abstract class PublicClass
{
}
var type = typeof(PublicClass);
var method = type.GetMethod("ToString");
var result = method.GetMethodAccessModifier(); // public
var info = method.GetMethodModifiers();  // virtual

```
Lot more...