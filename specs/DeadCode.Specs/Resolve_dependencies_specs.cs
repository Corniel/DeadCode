using FluentAssertions;
using Specs.Tooling;

namespace Resolve_dependencies_specs;

public class Ctor
{
    [Test]
    public void Depends_on_base()
    {
        Setup.Collector().AddSnippet(@"
            
    public class MyClass
    {
        public MyClass() { }
        public MyClass(MyClass other) : this(){ }
    }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Refs("MyClass.MyClass()", "MyClass.MyClass(MyClass)"),
            ["MyClass.MyClass()"] = Symbol.Refs("MyClass", "MyClass.MyClass(MyClass)"),
            ["MyClass.MyClass(MyClass)"] = Symbol.Refs(),
        });
    }
}

public class Overrides
{
    [Test]
    public void Is_Not_dead()
    {
        Setup.Collector().AddSnippet(@"
            
    public class MyClass
    {
        public override string ToString() => "";
    }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Refs("MyClass.ToString()"),
            ["MyClass.ToString()"] = Symbol.Refs(),
            ["string"] = Symbol.Refs("MyClass.ToString()"),
        })
        .And.HaveIsActive(
            "MyClass.ToString()",
            "string");
    }
}

public class Implements
{
    [Test]
    public void Is_not_dead()
    {
        Setup.Collector().AddSnippet(@"

    public interface MyInterface
    {
        void Do();
        MyInterface Parent { get; }
    }

    public class MyClass : MyInterface
    {
        public void Do() { }
        public MyInterface => this;
    }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Refs("MyClass.Do()"),
            ["MyClass.Do()"] = Symbol.Refs(),
            ["MyInterface"] = Symbol.Refs(
                "MyInterface.Do()", 
                "MyInterface.Parent",
                "MyClass"),
            ["MyInterface.Do()"] = Symbol.Refs(),
            ["MyInterface.Parent"] = Symbol.Refs(),
        })
        .And.HaveIsActive(
            "MyClass.Do()",
            "MyClass.Parent");
    }
}
