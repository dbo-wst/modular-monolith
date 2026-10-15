namespace Acme.Core.BuildingBlocks.Domain.Functional;

public static class Advanced
{
    public static
        Func<R> Applying<T1, R>
        (this Func<T1, R> that, T1 t1)
        => () => that(t1);

    public static
        Func<T2, R> Applying<T1, T2, R>
        (this Func<T1, T2, R> that, T1 t1)
        => t2 => that(t1, t2);


    public static Func<T2, T3, R> Applying<T1, T2, T3, R>
        (this Func<T1, T2, T3, R> func, T1 t1)
        => (t2, t3) => func(t1, t2, t3);

    public static Func<T2, T3, T4, R> Applying<T1, T2, T3, T4, R>
        (this Func<T1, T2, T3, T4, R> func, T1 t1)
        => (t2, t3, t4) => func(t1, t2, t3, t4);

    public static Func<T2, T3, T4, T5, R> Applying<T1, T2, T3, T4, T5, R>
        (this Func<T1, T2, T3, T4, T5, R> func, T1 t1)
        => (t2, t3, t4, t5) => func(t1, t2, t3, t4, t5);

    public static Func<T1, Func<T2, R>> Currying<T1, T2, R>(this Func<T1, T2, R> func) =>
        t1 => t2 => func(t1, t2);

    public static Func<T1, Func<T2, Func<T3, R>>> Currying<T1, T2, T3, R>(this Func<T1, T2, T3, R> func) =>
        t1 => t2 => t3 => func(t1, t2, t3);
}

