namespace GraphQL.Conventions
{
    public abstract class Union
    {
        public object Instance { get; set; }
    }

    public abstract class Union<T1> : Union { }

    public abstract class Union<T1, T2> : Union { }

    public abstract class Union<T1, T2, T3> : Union { }

    public abstract class Union<T1, T2, T3, T4> : Union { }

    public abstract class Union<T1, T2, T3, T4, T5> : Union { }

    public abstract class Union<T1, T2, T3, T4, T5, T6> : Union { }

    public abstract class Union<T1, T2, T3, T4, T5, T6, T7> : Union { }

    public abstract class Union<T1, T2, T3, T4, T5, T6, T7, T8> : Union { }

    public abstract class Union<T1, T2, T3, T4, T5, T6, T7, T8, T9> : Union { }

    public abstract class Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : Union { }
}
