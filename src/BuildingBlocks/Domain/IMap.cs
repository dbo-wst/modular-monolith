using System.Diagnostics.Contracts;

namespace Acme.Core.BuildingBlocks.Domain;

public interface IMap<TLeft, TRight>
{
    [Pure]
    TLeft Map(TRight right);

    [Pure]
    TRight Map(TLeft left);

}

