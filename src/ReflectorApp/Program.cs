

// See https://aka.ms/new-console-template for more information

Console.WriteLine("");






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


