using System;
using Munq;

namespace TinyWebStack
{
    /// <summary>
    /// Interface to the IoC container.
    /// </summary>
    public interface IContainer : IContainerDependencyResolver
    {
        void Register<T>(Func<IContainerDependencyResolver, T> register) where T : class;
    }
}
