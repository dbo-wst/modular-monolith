using System.Diagnostics.Contracts;

namespace Acme.Core.BuildingBlocks.Domain;

public interface IOnewayMap<TLeft, TRight>
{
    [Pure]
    TRight Map(TLeft left);
}

