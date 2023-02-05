using Moq;

namespace Specs.Tooling;

internal static class Moqcs
{
    public static Mock<ISymbol> ISymbol(string name, bool external = false)
    {
        var symbol = new Mock<ISymbol>();
        symbol.Setup(s => s.Name).Returns(name);
        symbol.Setup(s => s.IsExtern).Returns(external);
        symbol.Setup(s => s.ToString()).Returns(name);
        return symbol;
    }
}
