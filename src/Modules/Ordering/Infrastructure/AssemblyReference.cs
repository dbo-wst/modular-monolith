using System.Reflection;

namespace Template.Core.Ordering.Infrastructure;

public record Anchor;
// ReSharper disable once UnusedType.Global
public static class AssemblyReference
{
    public static Assembly Assembly => typeof(Anchor).Assembly;
}

