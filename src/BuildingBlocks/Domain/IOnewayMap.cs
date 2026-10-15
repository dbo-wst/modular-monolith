using System.Diagnostics.Contracts;

namespace Template.Core.BuildingBlocks.Domain;

public interface IOnewayMap<TLeft, TRight>
{
    [Pure]
    TRight Map(TLeft left);
}

