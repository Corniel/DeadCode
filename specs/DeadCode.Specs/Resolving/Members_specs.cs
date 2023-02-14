namespace Resolving.Members_specs;

public class Construcors_are
{
    [Test]
    public void used_by_containing_and_parameter_types()
        => Setup.Collector().AddSnippet(@"
            
    public class MyClass
    {
        public MyClass(int arg) { }
        public MyClass(int arg1, double arg2) { }
    }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Array("MyClass.MyClass(int)", "MyClass.MyClass(int, double)"),
            ["MyClass.MyClass(int)"] = Symbol.Array(),
            ["MyClass.MyClass(int, double)"] = Symbol.Array(),
            ["int"] = Symbol.Array("MyClass.MyClass(int)", "MyClass.MyClass(int, double)"),
            ["double"] = Symbol.Array("MyClass.MyClass(int, double)"),
        });

    [Test]
    public void using_type_when_default_ctor()
        => Setup.Collector().AddSnippet(@"
            
    public class MyClass
    {
        public MyClass() { }
        public MyClass(int arg) { }
    }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Array("MyClass.MyClass()", "MyClass.MyClass(int)"),
            ["MyClass.MyClass()"] = Symbol.Array("MyClass"),
            ["MyClass.MyClass(int)"] = Symbol.Array(),
            ["int"] = Symbol.Array("MyClass.MyClass(int)"),
        });

    [Test]
    public void using_type_when_single_ctor()
        => Setup.Collector().AddSnippet(@"
            
    public class MyClass
    {
        public MyClass(int arg) { }
    }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Array("MyClass.MyClass(int)"),
            ["MyClass.MyClass(int)"] = Symbol.Array("MyClass"),
            ["int"] = Symbol.Array("MyClass.MyClass(int)"),
        });

    [Test]
    public void using_base_ctor()
       => Setup.Collector().AddSnippet(@"
            
    public class MyBase
    {
        public MyBase() { }    
    }
    public class MyClass : MyBase
    {
        public MyClass() : base() { }
    }")
       .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
       {
           ["MyBase"] = Symbol.Array("MyBase.MyBase()"),
           ["MyBase.MyBase()"] = Symbol.Array("MyBase", "MyClass.MyClass()"),
           ["MyClass"] = Symbol.Array("MyClass.MyClass()"),
           ["MyClass.MyClass()"] = Symbol.Array("MyClass"),
       });

    [Test]
    public void used_by_referenced_ctor()
        => Setup.Collector().AddSnippet(@"
            
    public class MyClass
    {
        public MyClass() : this(42) { }
        public MyClass(int arg) { }
    }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Array("MyClass.MyClass()", "MyClass.MyClass(int)"),
            ["MyClass.MyClass()"] = Symbol.Array("MyClass"),
            ["MyClass.MyClass(int)"] = Symbol.Array("MyClass.MyClass()"),
            ["int"] = Symbol.Array("MyClass.MyClass(int)"),
        });
}

public class Enum_members_are
{
    [Test]
    public void used_by_containing_type()
        => Setup.Collector().AddSnippet(@"
            
    public enum MyEnum
    {
        None
    }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyEnum"] = Symbol.Array("MyEnum.None"),
            ["MyEnum.None"] = Symbol.Array()
        });
}

public class Fields_are
{
    [Test]
    public void used_by_containing_and_return_type()
        => Setup.Collector().AddSnippet(@"
            
    public class MyClass
    {
        public int MyField;
    }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Array("MyClass.MyField"),
            ["MyClass.MyField"] = Symbol.Array(),
            ["int"] = Symbol.Array("MyClass.MyField"),
        });
}

public class Methods_are
{
    [Test]
    public void used_by_containing_return_and_parameter_types()
        => Setup.Collector().AddSnippet(@"
            
    public class MyClass
    {
        public int MyMethod(double f, bool s) => 42;
    }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Array("MyClass.MyMethod(double, bool)"),
            ["MyClass.MyMethod(double, bool)"] = Symbol.Array(),
            ["int"] = Symbol.Array("MyClass.MyMethod(double, bool)"),
            ["double"] = Symbol.Array("MyClass.MyMethod(double, bool)"),
            ["bool"] = Symbol.Array("MyClass.MyMethod(double, bool)"),
        });
}

public class Properties_are
{
    [Test]
    public void used_by_containing_and_return_type()
        => Setup.Collector().AddSnippet(@"
            
    public class MyClass
    {
        public int MyProperty { get; set; }
    }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Array("MyClass.MyProperty"),
            ["MyClass.MyProperty"] = Symbol.Array(),
            ["int"] = Symbol.Array("MyClass.MyProperty"),
        });
}

