using System.Reflection;

namespace Wst.Trios.Core.BrokerSide.Persistence;

public record Anchor;
// ReSharper disable once UnusedType.Global
public static class AssemblyReference
{
    public static Assembly Assembly => typeof(Anchor).Assembly;
}

