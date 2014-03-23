using System;

namespace TinyWebStack
{
    /// <summary>
    /// Interface to the IoC container.
    /// </summary>
    public interface IContainer : IContainerDependencyResolver
    {
        void Register<T>(Func<IContainerDependencyResolver, T> register, Lifetime lifetime = Lifetime.None) where T : class;

        void Register<T, I>(Lifetime lifetime = Lifetime.None)
            where T : class
            where I : class, T;
    }
}
