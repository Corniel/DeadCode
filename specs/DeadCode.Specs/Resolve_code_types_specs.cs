namespace Resolve_code_types_specs;

public class Resolves
{
    [Test]
    public void classes()
        => Setup.Collector().AddSnippet("public class MyClass { }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyClass"] = Symbol.Array()
        });

    [Test]
    public void enums()
      => Setup.Collector().AddSnippet("public enum MyEnum { }")
      .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
      {
          ["MyEnum"] = Symbol.Array()
      });

    [Test]
    public void interfaces()
      => Setup.Collector().AddSnippet("public interface IMyInterface { }")
      .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
      {
          ["IMyInterface"] = Symbol.Array()
      });


    [Test]
    public void records()
        => Setup.Collector().AddSnippet("public record MyRecord { }")
        .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
        {
            ["MyRecord"] = Symbol.Array()
        });

    [Test]
    public void structs()
       => Setup.Collector().AddSnippet("public struct MyStruct { }")
       .CodeBase().Should().HaveUsedBys(new Dictionary<Symbol, Symbol[]>()
       {
           ["MyStruct"] = Symbol.Array()
       });
}
