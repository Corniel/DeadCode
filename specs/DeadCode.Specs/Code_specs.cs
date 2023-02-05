using DeadCode;
using FluentAssertions;
using Specs.Tooling;

namespace Code_specs;

public class Is_dead
{
    [Test]
    public void does_no_crash_on_circularity()
    {
        var clas = new Code(Moqcs.ISymbol("MyClass").Object);
        var ctor = new Code(Moqcs.ISymbol("MyClass.MyClass()").Object);

        clas.UsedBy.Add(ctor);
        ctor.UsedBy.Add(clas);

        ctor.IsDead.Should().BeTrue();
    }
}
