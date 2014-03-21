using System;
using System.Collections.Generic;
using Munq;
using Munq.LifetimeManagers;

namespace TinyWebStack
{
    public class Container : IContainer
    {
        private ILifetimeManager RequestLifetime = new RequestLifetime();

        private ILifetimeManager ContainerLifetime = new ContainerLifetime();

        static Container()
        {
            Container.Current = new Container();
        }

        private Container()
        {
            this.IocContainer = new IocContainer();

            this.IocContainer.Register<IApplicationState>(_ => new Implementation.ApplicationState())
                .WithLifetimeManager(RequestLifetime);

            this.IocContainer.Register<IRequest>(_ => new Implementation.Request())
                .WithLifetimeManager(RequestLifetime);

            this.IocContainer.Register<IResponse>(_ => new Implementation.Response())
                .WithLifetimeManager(RequestLifetime);

            this.IocContainer.Register<IServerUtility>(_ => new Implementation.ServerUtility())
                .WithLifetimeManager(ContainerLifetime);
        }

        private IocContainer IocContainer { get; set; }

        /// <summary>
        /// Gets the current container.
        /// </summary>
        /// <remarks>The setter is intended for use by tests to override the default creation.</remarks>
        public static IContainer Current { get; set; }

        public void Register<T>(Func<IContainerDependencyResolver, T> register) where T : class
        {
            // Add a little hop in the register to use our container to do the registration
            // so we don't expose the internal IocContainer from Munq implementation details.
            //
            this.IocContainer.Register<T>(c => register(this));
        }

        //public bool CanResolve(string name, Type type)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool CanResolve(Type type)
        //{
        //    return this.IocContainer.CanResolve(type);
        //}

        public bool CanResolve<T>() where T : class
        {
            return this.IocContainer.CanResolve<T>();
        }

        //public bool CanResolve<T>(string name) where T : class
        //{
        //    throw new NotImplementedException();
        //}

        //public Func<object> LazyResolve(string name, Type type)
        //{
        //    throw new NotImplementedException();
        //}

        //public Func<object> LazyResolve(Type type)
        //{
        //    return this.IocContainer.LazyResolve(type);
        //}

        public Func<T> LazyResolve<T>() where T : class
        {
            return this.IocContainer.LazyResolve<T>();
        }

        //public Func<T> LazyResolve<T>(string name) where T : class
        //{
        //    throw new NotImplementedException();
        //}

        //public object Resolve(string name, Type type)
        //{
        //    throw new NotImplementedException();
        //}

        //public object Resolve(Type type)
        //{
        //    return this.IocContainer.Resolve(type);
        //}

        public T Resolve<T>() where T : class
        {
            return this.IocContainer.Resolve<T>();
        }

        //public T Resolve<T>(string name) where T : class
        //{
        //    throw new NotImplementedException();
        //}

        //public IEnumerable<object> ResolveAll(Type type)
        //{
        //    return this.IocContainer.ResolveAll(type);
        //}

        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            return this.IocContainer.ResolveAll<T>();
        }
    }
}
