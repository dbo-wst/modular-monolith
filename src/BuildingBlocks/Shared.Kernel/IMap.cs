using System.Diagnostics.Contracts;

namespace Template.Core.Shared.Kernel;

public interface IMap<TLeft, TRight>
{
    [Pure]
    TLeft Map(TRight right);

    [Pure]
    TRight Map(TLeft left);

}

