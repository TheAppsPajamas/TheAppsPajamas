using System;
using System.Collections.Generic;

namespace Build.Client.Ioc
{
    //http://adventuresdotnet.blogspot.co.uk/2009/09/creating-your-own-ioc-container-part-2.html


    /// <summary>
    /// Resolves abstract types or interfaces to concrete implementations. Use <see cref="ServiceLocator" /> for static/shared access.
    /// </summary>
    public class ServiceResolver : IServiceResolver
    {
        private Dictionary<Type, object> _store;
        private Dictionary<Type, Type> _bindings;

        /// <summary>
        /// Default constructor; instantiates a new ServiceResolver object.
        /// </summary>
        public ServiceResolver()
        {
            this.DependencyInjector = new DependencyInjector(this);
            _store = new Dictionary<Type, object>();
            _bindings = new Dictionary<Type, Type>();
        }

        /// <summary>
        /// Creates a new ServiceResolver with a given IDependencyInjector.
        /// </summary>
        /// <param name="injector">The IDependencyInjector to use for dependency injection.</param>
        public ServiceResolver(IDependencyInjector injector)
        {
            this.DependencyInjector = injector;
            _store = new Dictionary<Type, object>();
            _bindings = new Dictionary<Type, Type>();
        }

        /// <summary>
        /// Gets an implementation object of a registered abstract type or interface.
        /// </summary>
        /// <typeparam name="T">The registered abstract type or interface to look up.</typeparam>
        /// <returns>Returns an object of the given type.</returns>
        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        /// <summary>
        /// Gets an implementation object of a registered abstract type or interface.
        /// </summary>
        /// <param name="fromType">The registered abstract type or interface to look up.</param>
        /// <returns>Returns an object of the given type.</returns>
        public object Resolve(Type fromType)
        {
            // check for registration
            if (!_bindings.ContainsKey(fromType))
                return DependencyInjector.GetInjectedInstance(fromType);

            // get destination type
            Type dest = _bindings[fromType];

            // check for already requested object
            if (_store.ContainsKey(dest))
                return _store[dest];

            // create a new instance of this type
            object obj = DependencyInjector.GetInjectedInstance(dest);

            // add to store for future use
            _store.Add(dest, obj);

            return obj;
        }

        /// <summary>
        /// Registers a type with its corresponding implementation type.
        /// </summary>
        /// <typeparam name="TFrom">The abstract type or interface to use as a key.</typeparam>
        /// <typeparam name="TTo">The implementation type to use as a value.</typeparam>
        public void Register<TFrom, TTo>()
        {
            _bindings.Add(typeof(TFrom), typeof(TTo));
        }

        /// <summary>
        /// Gets or sets a dependency injector to use for types that need injection.
        /// </summary>
        public IDependencyInjector DependencyInjector { get; set; }

    }
}
