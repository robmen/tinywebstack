using System;
using System.Collections.Generic;

namespace TinyWebStack
{
    /// <summary>
    /// Interface to the IoC container resolver methods.
    /// </summary>
    public interface IContainerDependencyResolver
    {
        bool CanResolve<T>() where T : class;

        Func<T> LazyResolve<T>() where T : class;

        object Resolve(Type type);

        T Resolve<T>() where T : class;

        IEnumerable<T> ResolveAll<T>() where T : class;
    }
}
