
namespace ReflectorTest.Entities;

public class Person
{
}

public class PersonAll
{

    public PersonAll() : this(new PersonAll())
    {

    }
    public PersonAll(string message)
    {

    }
    static PersonAll()
    {

    }
    private PersonAll(PersonAll? person)
    {
    }

    protected PersonAll(int count)
    {
    }
}

public class PersonAllParm
{
    public PersonAllParm(int first, string second, int defaultValue = 10, object state = null)
    {

    }
}

public class PersonParm
{
    public PersonParm(params string[] second)
    {

    }
}

public class PersonEntened : PersonAll
{
    public PersonEntened(params string[] second)
    {

    }
}

public record FirstRecord(string name);

public record struct MyPoint(int X, int Y);



public interface IA
{

}
public interface IB : IA
{

}
public interface IC
{

}
public interface ID
{

}
public interface IE
{

}
public abstract class A : IB
{
    public int Id { get; private set; }
    public string? Name { get; init; }
}

public class B : A, IC
{

}
public class C : B
{
    public A? MyProperty { get; }
}
public class D : C, IE, ID
{
    public B? MyProperty2 { get; }
    public IA? MyProperty3 { get; init; }
}