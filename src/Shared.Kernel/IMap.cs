using System.Diagnostics.Contracts;

namespace Wst.Trios.Core.Shared.Kernel;

public interface IMap<TLeft, TRight>
{
    [Pure]
    TLeft Map(TRight right);

    [Pure]
    TRight Map(TLeft left);

}

