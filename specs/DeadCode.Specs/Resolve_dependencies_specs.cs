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
            ["MyClass"] = Symbol.Array("MyClass.MyClass()", "MyClass.MyClass(MyClass)"),
            ["MyClass.MyClass()"] = Symbol.Array("MyClass", "MyClass.MyClass(MyClass)"),
            ["MyClass.MyClass(MyClass)"] = Symbol.Array(),
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
            ["MyClass"] = Symbol.Array("MyClass.ToString()"),
            ["MyClass.ToString()"] = Symbol.Array(),
            ["string"] = Symbol.Array("MyClass.ToString()"),
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
            ["MyClass"] = Symbol.Array("MyClass.Do()"),
            ["MyClass.Do()"] = Symbol.Array(),
            ["MyInterface"] = Symbol.Array(
                "MyInterface.Do()", 
                "MyInterface.Parent",
                "MyClass"),
            ["MyInterface.Do()"] = Symbol.Array(),
            ["MyInterface.Parent"] = Symbol.Array(),
        })
        .And.HaveIsActive(
            "MyClass.Do()",
            "MyClass.Parent");
    }
}

public class References_regonized
{
    [Test]
    public void method()
    {
        Setup.Collector().AddSnippet(@"
            
    public class MyClass
    {
        protected void Other() { }

        public void Do() => Other();
    }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Array("MyClass.Other()", "MyClass.Do()"),
            ["MyClass.Other()"] = Symbol.Array("MyClass.Do()"),
            ["MyClass.Do()"] = Symbol.Array(),
        });
    }

    [Test]
    public void property()
    {
        Setup.Collector().AddSnippet(@"
            
    public class MyClass
    {
        protected int Other { get; }

        public int Do() => Other;
    }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Array("MyClass.Other", "MyClass.Do()"),
            ["MyClass.Other"] = Symbol.Array("MyClass.Do()"),
            ["MyClass.Do()"] = Symbol.Array(),
            ["int"] = Symbol.Array("MyClass.Other", "MyClass.Do()"),
        });
    }

    [Test]
    public void new_ctor()
    {
        Setup.Collector().AddSnippet(@"
            
    public class MyClass
    {
        void Do()
        {
            _ = new Other();
        }
    }
    class Other { }
")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Array( "MyClass.Do()"),
            ["MyClass.Do()"] = Symbol.Array(),
            ["Other"] = Symbol.Array(),
            ["Other.Other()"] = Symbol.Array("MyClass.Do()"),
        });
    }

    [Test]
    public void in_method_calls()
    {
        Setup.Collector().AddSnippet(@"
            
    public class MyClass
    {
        void Do()
        {
            string.Format(""{0}"", new Other());
        }
    }
    class Other { }
")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Array("MyClass.Do()"),
            ["MyClass.Do()"] = Symbol.Array(),
            ["Other"] = Symbol.Array(),
            ["Other.Other()"] = Symbol.Array("MyClass.Do()"),
            ["string.Format(string, object?)"] = Symbol.Array("MyClass.Do()"),
        });
    }
}