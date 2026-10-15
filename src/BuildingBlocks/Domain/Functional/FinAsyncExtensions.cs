using LanguageExt;
using LanguageExt.Common;

namespace Template.Core.BuildingBlocks.Domain.Functional;

public static class FinAsyncExtensions
{
    extension<A>(Task<Fin<A>> that)
    {
        public Task<Fin<Unit>> ToUnitAsync()
        {
            return that.Map(i => i.ToUnit());
        }

        public Task<Fin<TOut>> TryAsync<TOut>(Func<A, TOut> func)
        {
            return that.Bind(fin => fin.Try(func).AsTask());
        }

        public Task<Fin<Unit>> TryAsync(Action<A> action)
        {
            return that.Bind(fin => fin.Try(action).AsTask());
        }

        public Task<Fin<A>> MapErrorAsync(Func<Error> mappedError)
        {
            return that.Bind(fin =>
            {
                var result = fin.MapError(mappedError);
                return result.AsTask();
            });
        }

        public Task<Fin<B>> MapAsync<B>(Func<A, B> func)
        {
            return that.Bind(finA => finA.Map(func).AsTask());
        }

        public Task<Fin<B>> BindAsync<B>(Func<A, Task<Fin<B>>> func)
        {
            return that.Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.MatchAsync(func, Fin<B>.Fail);
            });
        }

        public Task<Fin<B>> BindAsync<B>(Func<A, B> func)
        {
            return that.Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.MatchAsync(a => func(a).ToFin(), error => Fin<B>.Fail(error).AsTask());
            });
        }

        public Task<Fin<B>> BindAsync<B>(Func<A, Task<B>> func)
        {
            return that.Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.MatchAsync(a => func(a).Map(t => t.ToFin()), error => Fin<B>.Fail(error).AsTask());
            });
        }
        public Task<Fin<B>> BindAsync<B>(Func<A, Fin<B>> func)
        {
            return that.Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.MatchAsync(func, error => Fin<B>.Fail(error).AsTask());
            });
        }

        public Task<Fin<B>> BindAsync<B>(Func<Task<Fin<B>>> func)
        {
            return that.Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.MatchAsync(_ => func(), Fin<B>.Fail);
            });
        }

        public Task<Fin<B>> BindForceAsync<B>(Func<Task<Fin<B>>> func)
        {
            return that.Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.MatchAsync(_ => func(), async error =>
                {
                    await func();
                    return Fin<B>.Fail(error);
                });
            });
        }

        public Task<Fin<B>> MatchAsync<B>(Func<A, Task<Fin<B>>> successAsync, Func<Error, Task<Fin<B>>> failureAsync)
        {
            return that.Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.MatchAsync(successAsync, failureAsync);
            });
        }

        public Task<B> MatchAsync<B>(Func<A, Task<B>> successAsync, Func<Error, Task<B>> failureAsync)
        {
            return that.Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.MatchAsync(successAsync, failureAsync);
            });
        }

        public Task<B> MatchAsync<B>(Func<A, B> successAsync, Func<Error, B> failureAsync)
        {
            return that.Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.Match(successAsync, failureAsync).AsTask();
            });
        }

        public Task<Fin<B>> MatchAsync<B>(Func<Task<Fin<B>>> successAsync, Func<Task<Fin<B>>> failureAsync)
        {
            return that.Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.MatchAsync(_ => successAsync(), _ => failureAsync());
            });
        }
    }
}

