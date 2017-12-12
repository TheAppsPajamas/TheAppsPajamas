using System;
namespace Build.Client.Ioc
{
    public interface IServiceResolver
    {
        void Register<TFrom, TTo>();
        T Resolve<T>();
        object Resolve(Type fromType);
    }
}
