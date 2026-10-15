using System.Reflection;

namespace Template.Core.Ordering.Persistence;

public record Anchor;
// ReSharper disable once UnusedType.Global
public static class AssemblyReference
{
    public static Assembly Assembly => typeof(Anchor).Assembly;
}

