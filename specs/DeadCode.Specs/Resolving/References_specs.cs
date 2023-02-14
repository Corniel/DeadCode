using System.Security.Permissions;

namespace Resolving.References_specs;

public class Ignores
{
    [Test]
    public void Void()
        => Setup.Collector().AddSnippet(@"
            
    public class MyClass
    {
        public void MyMethod() { };
    }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Array("MyClass.MyMethod()"),
            ["MyClass.MyMethod()"] = Symbol.Array(),
        });
}

public class Links
{
    [Test]
    public void new_instances()
     => Setup.Collector().AddSnippet(@"
            
    public class Other { }
    public class MyClass
    {
        public void MyMethod() 
        {
            _ = new Other();
        };
    }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["Other"] = Symbol.Array("Other.Other()"),
            ["Other.Other()"] = Symbol.Array("MyClass.MyMethod()"),
            ["MyClass"] = Symbol.Array("MyClass.MyMethod()"),
            ["MyClass.MyMethod()"] = Symbol.Array(),
        });

    [Test]
    public void properties()
        => Setup.Collector().AddSnippet(@"
            
    public class MyClass
    {
        public int MyMethod() => MyProperty;
        public int MyProperty => 42;
    }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Array("MyClass.MyMethod()", "MyClass.MyProperty"),
            ["MyClass.MyMethod()"] = Symbol.Array(),
            ["MyClass.MyProperty"] = Symbol.Array("MyClass.MyMethod()"),
            ["int"] = Symbol.Array("MyClass.MyMethod()", "MyClass.MyProperty"),
        });
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

        public int Do() => 42;
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