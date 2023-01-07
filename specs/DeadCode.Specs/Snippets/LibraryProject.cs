namespace LibraryProject;

public class SomeClass
{
    public SomeClass(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public bool BegToDiffer(OtherClass other) => Equals(this, other);

    public void WithContent()
    {
        System.Console.WriteLine(new SomeStruct());
    }
}

public class OtherClass
{

}

public struct SomeStruct
{
    public readonly int Value;
}