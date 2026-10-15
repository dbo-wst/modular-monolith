using System.Diagnostics.Contracts;

namespace Template.Core.Shared.Kernel;

public interface IOnewayMap<TLeft, TRight>
{
    [Pure]
    TRight Map(TLeft left);
}

