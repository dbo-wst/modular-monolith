using LanguageExt;
using LanguageExt.Common;

namespace Acme.Core.BuildingBlocks.Domain.Functional;

public static class FuncFinAsyncExtensions
{
    extension<A>(Func<Task<Fin<A>>> that)
    {
        public Task<Fin<Unit>> ToUnitAsync()
        {
            return that().Map(i => i.ToUnit());
        }

        public Task<Fin<TOut>> TryAsync<TOut>(Func<A, TOut> func)
        {
            return that().Bind(fin => fin.Try(func).AsTask());
        }


        public Task<Fin<Unit>> TryAsync(Action<A> action)
        {
            return that().Bind(fin => fin.Try(action).AsTask());
        }

        public Task<Fin<A>> MapErrorAsync(Func<Error> mappedError)
        {
            return that().Bind(fin =>
            {
                var result = fin.MapError(mappedError);
                return result.AsTask();
            });
        }

        // public Task<Fin<B>> BindAsync<B>(Func<A, Fin<B>> func) => that().Bind(finA => finA.Bind(func).AsTask());

        public Task<Fin<B>> MapAsync<B>(Func<A, B> func)
        {
            return that().Bind(finA => finA.Map(func).AsTask());
        }

        public Task<Fin<B>> BindAsync<B>(Func<A, Task<Fin<B>>> func)
        {
            return that().Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.MatchAsync(func, Fin<B>.Fail);
            });
        }

        public Task<Fin<B>> BindAsync<B>(Func<A, Fin<B>> func)
        {
            return that().Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.MatchAsync(func, fail => Fin<B>.Fail(fail).AsTask());
            });
        }

        public Task<Fin<B>> BindAsync<B>(Func<Task<Fin<B>>> func)
        {
            return that().Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.MatchAsync(_ => func(), Fin<B>.Fail);
            });
        }

        public Task<Fin<B>> BindForceAsync<B>(Func<Task<Fin<B>>> func)
        {
            return that().Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.MatchAsync(_ => func(), async fail =>
                {
                    await func();
                    return Fin<B>.Fail(fail);
                });
            });
        }

        public Task<Fin<B>> MatchAsync<B>(Func<A, Task<Fin<B>>> successAsync, Func<Error, Task<Fin<B>>> failureAsync)
        {
            return that().Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.MatchAsync(successAsync, failureAsync);
            });
        }

        public Task<B> MatchAsync<B>(Func<A, Task<B>> successAsync, Func<Error, Task<B>> failureAsync)
        {
            return that().Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.MatchAsync(successAsync, failureAsync);
            });
        }

        public Task<B> MatchAsync<B>(Func<A, B> successAsync, Func<Error, B> failureAsync)
        {
            return that().Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.Match(successAsync, failureAsync).AsTask();
            });
        }

        public Task<Fin<B>> MatchAsync<B>(Func<Task<Fin<B>>> successAsync, Func<Task<Fin<B>>> failureAsync)
        {
            return that().Bind(finA =>
            {
                Either<Error, A> either = finA.ToEither();
                return either.MatchAsync(_ => successAsync(), _ => failureAsync());
            });
        }
    }
}

