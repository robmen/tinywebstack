using System;
using System.Collections.Generic;
using Munq;
using Munq.LifetimeManagers;
using TinyWebStack.Implementation;

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

            this.IocContainer.Register<IApplicationState, ApplicationState>()
                .WithLifetimeManager(RequestLifetime);

            this.IocContainer.Register<IRequest, Request>()
                .WithLifetimeManager(RequestLifetime);

            this.IocContainer.Register<IResponse, Response>()
                .WithLifetimeManager(RequestLifetime);

            this.IocContainer.Register<IRoutes, Routes>()
                .WithLifetimeManager(RequestLifetime);

            this.IocContainer.Register<IServerUtility, ServerUtility>()
                .WithLifetimeManager(ContainerLifetime);
        }

        private IocContainer IocContainer { get; set; }

        /// <summary>
        /// Gets the current container.
        /// </summary>
        /// <remarks>The setter is intended for use by tests to override the default creation.</remarks>
        public static IContainer Current { get; set; }

        public void Register<T>(Func<IContainerDependencyResolver, T> register, Lifetime lifetime = Lifetime.None) where T : class
        {
            // Add a little hop in the register to use our container to do the registration
            // so we don't expose the internal IocContainer from Munq implementation details.
            //
            this.RegistrationLifetime(this.IocContainer.Register<T>(c => register(this)), lifetime);
        }

        public void Register<T, I>(Lifetime lifetime = Lifetime.None)
            where T : class
            where I : class, T
        {
            this.RegistrationLifetime(this.IocContainer.Register<T, I>(), lifetime);
        }

        public bool CanResolve<T>() where T : class
        {
            return this.IocContainer.CanResolve<T>();
        }

        public Func<T> LazyResolve<T>() where T : class
        {
            return this.IocContainer.LazyResolve<T>();
        }

        public object Resolve(Type type)
        {
            return this.IocContainer.Resolve(type);
        }

        public T Resolve<T>() where T : class
        {
            return this.IocContainer.Resolve<T>();
        }

        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            return this.IocContainer.ResolveAll<T>();
        }

        private void RegistrationLifetime(IRegistration registration, Lifetime lifetime)
        {
            switch (lifetime)
            {
                case Lifetime.Application:
                    registration.WithLifetimeManager(this.ContainerLifetime);
                    break;

                case Lifetime.Request:
                    registration.WithLifetimeManager(this.RequestLifetime);
                    break;
            }
        }
    }
}
