using LanguageExt.Common;

namespace Wst.Trios.Core.BrokerSide.Persistence;

public static class PersistenceErrors
{
    public static Error AlreadyExists<T>(T entity) => Error.New($"Entity {entity} already exists");
    public static Error NotFound<T>(T entity) => Error.New($"Entity {entity} was not found.");

}

