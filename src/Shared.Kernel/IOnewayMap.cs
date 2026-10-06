using System.Diagnostics.Contracts;

namespace Wst.Trios.Core.Shared.Kernel;

public interface IOnewayMap<TLeft, TRight>
{
    [Pure]
    TRight Map(TLeft left);
}

