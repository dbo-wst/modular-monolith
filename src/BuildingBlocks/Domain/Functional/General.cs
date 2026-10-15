using LanguageExt;
using LanguageExt.Common;
using Acme.Core.BuildingBlocks.Domain.Errors;

namespace Acme.Core.BuildingBlocks.Domain.Functional;

public static class General
{
    extension(Exception source)
    {
        public Error ToError() => Error.New("Error occured.", source);
    }

    public static TResult Then<T, TResult>(this T value, Func<T, TResult> f) => f(value);

    extension(Unit that)
    {
        public static Fin<Unit> Success() => Fin<Unit>.Succ(Unit.Default);
        public static Fin<Unit> Failure(Error error) => Fin<Unit>.Fail(error);
        public static Task<Fin<Unit>> SuccessAsync() => Fin<Unit>.Succ(Unit.Default).AsTask();
        public static Task<Fin<Unit>> FailureAsync(Error error) => Fin<Unit>.Fail(error).AsTask();
    }

    extension<A>(A that)
    {
        public Fin<A> ToFin()
        {
            return that == null ?
                Fin<A>.Fail(ValidationError.New($"A value of type {typeof(A).Name} is required.")) : Fin<A>.Succ(that);
        }

        public Task<Fin<A>> ToFinAsync()
        {
            return that.ToFin().AsTask();
        }
        public Task<Fin<A>> ToFinAsync(Error error)
        {
            return that.ToFin(error).AsTask();
        }

        public Fin<A> ToFin(Error error)
        {
            return that == null ? Fin<A>.Fail(error) : Fin<A>.Succ(that);
        }
    }

    extension<A>(Option<A> that)
    {
        public Fin<A> ToFin()
        {
            return that.Match(Fin<A>.Succ, () => Fin<A>.Fail(Error.New($"is none.{nameof(A)}")));
        }

        public Task<Fin<A>> ToFinAsync()
        {
            return that.ToFin().AsTask();
        }
        public Task<Fin<A>> ToFinAsync(Error error)
        {
            return that.ToFin(error).AsTask();
        }

        public Fin<A> ToFin(Error error)
        {
            return that.Match(Fin<A>.Succ, () => Fin<A>.Fail(error));
        }
    }
    
    extension<A>(Task<Option<A>> that)
    {
        public Task<Fin<A>> ToFinAsync() => that.Bind(i => i.ToFin().AsTask());

        public Task<Fin<A>> ToFinAsync(Error error) => that.Bind(i => i.ToFin(error).AsTask());
    }
}

