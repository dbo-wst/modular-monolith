using LanguageExt;
using LanguageExt.Common;

namespace Template.Core.BuildingBlocks.Domain.Functional;

public static class FinExtensions
{
    extension<A>(Fin<A> that)
    {
        public Fin<Unit> ToUnit()
        {
            return that.Bind<Unit>(_ => Unit.Default);
        }

        public Fin<A> MapError(Func<Error> mappedError)
        {
            var newError = mappedError();
            return that.BiBind(
                a => a,
                err => Fin<A>.Fail(Error.New(newError.Message, err)));
        }

        public Fin<TOut> Try<TOut>(Func<A, TOut> func)
        {
            return that.Bind(a =>
            {
                try
                {
                    return func(a) ?? Fin<TOut>.Fail(Error.New($"function execution on failed. that={a}"));
                }
                catch (Exception e)
                {
                    return Error.New($"Error trying to execute a function {nameof(func)},{func.Method.Name}. that={a}",
                        e);
                }
            });
        }

        public Fin<Unit> Try(Action<A> action)
        {
            return that.Bind(a =>
            {
                try
                {
                    action(a);
                    return Fin<Unit>.Succ(Unit.Default);
                }
                catch (Exception e)
                {
                    return Error.New(
                        $"Error trying to execute an action. {nameof(action)},{action.Method.Name}. that={a}",
                        e.Message);
                }
            });
        }

        public Fin<B> Bind<B>(Func<A, Fin<B>> func)
        {
            return that.ToEither().Match(func, Fin<B>.Fail);
        }

        public Fin<B> Bind<B>(Func<A, B> func)
        {
            return that.Match(a => func(a).ToFin(), Fin<B>.Fail);
        }

        public Fin<B> Bind<B>(Func<Fin<B>> func)
        {
            return that.ToEither().Match(_ => func(), Fin<B>.Fail);
        }

    }
}

