using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Build.Client.Ioc
{
    public interface IDependencyInjector
    {
        T GetInjectedInstance<T>() where T : class;
        object GetInjectedInstance(Type fromType);
        IServiceResolver ServiceResolver { get; set; }
    }

    /// <summary>
    /// A class for getting instances of objects that need dependencies injected to function.
    /// </summary>
    public class DependencyInjector : IDependencyInjector
    {
        /// <summary>
        /// Creates a new DependencyInjector with a default IServiceResolver for resolving types.
        /// </summary>
        public DependencyInjector() : this(new ServiceResolver())
        { }

        /// <summary>
        /// Creates a new DependencyInjector with a given IServiceResolver for resolving types.
        /// </summary>
        /// <param name="resolver">The IServiceResolver to use for resolving types to implementations.</param>
        public DependencyInjector(IServiceResolver resolver)
        {
            this.ServiceResolver = resolver;
        }

        /// <summary>
        /// Gets or sets an IServiceResolver to use for resolving types to implementations.
        /// </summary>
        public IServiceResolver ServiceResolver { get; set; }

        /// <summary>
        /// Gets an injected instance of a given type.
        /// </summary>
        /// <typeparam name="T">The type to instantiate and inject.</typeparam>
        /// <returns>Returns a new instance of the given type.</returns>
        public virtual T GetInjectedInstance<T>() where T : class
        {
            return (T)GetInjectedInstance(typeof(T));
        }

        /// <summary>
        /// Gets an injected instance of a given type.
        /// </summary>
        /// <param name="fromType">The type to instantiate and inject.</param>
        /// <returns>Returns a new instance of the given type.</returns>
        public virtual object GetInjectedInstance(Type fromType)
        {
            object obj = null;

            foreach (var constructor in fromType.GetConstructors())
            {
                // look for inject attribute
                var attr = GetConstructorInjectAttribute(constructor);

                if (attr != null)
                {
                    // get parameters to inject
                    var parmValues = GetResolvedParameterValues(constructor);

                    if (parmValues.Count > 0)
                        obj = Activator.CreateInstance(fromType, parmValues.ToArray());

                    break;
                }
            }

            // handle case where no constructor injections
            if (obj == null)
                obj = Activator.CreateInstance(fromType);

            foreach (var prop in fromType.GetProperties())
            {
                // look for inject attribute
                var attr = GetPropertyInjectAttribute(prop);

                if (attr != null)
                    prop.SetValue(obj, ServiceResolver.Resolve(prop.PropertyType), new object[] { });
            }

            return obj;
        }

        /// <summary>
        /// Gets an InjectAttribute (if any) for the given constructor.
        /// </summary>
        /// <param name="constructor">The constructor to inspect for an InjectAttribute.</param>
        /// <returns>Returns an InjectAttribute if exists, or null if not.</returns>
        protected virtual InjectAttribute GetConstructorInjectAttribute(ConstructorInfo constructor)
        {
            var attrs = constructor.GetCustomAttributes(typeof(InjectAttribute), true);
            return attrs.OfType<InjectAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// Gets an InjectAttribute (if any) for the given property.
        /// </summary>
        /// <param name="prop">The property to inspect for an InjectAttribute.</param>
        /// <returns>Returns an InjectAttribute if exists, or null if not.</returns>
        protected virtual InjectAttribute GetPropertyInjectAttribute(PropertyInfo prop)
        {
            var attrs = prop.GetCustomAttributes(typeof(InjectAttribute), true);
            return attrs.OfType<InjectAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// Gets a list of parameter values for the given constructor, resolved by the ServiceResolver.
        /// </summary>
        /// <param name="constructor">The constructor to resolve parameters for.</param>
        /// <returns>Returns a new list of parameter values.</returns>
        protected virtual List<object> GetResolvedParameterValues(ConstructorInfo constructor)
        {
            var parms = constructor.GetParameters();
            List<object> parmValues = new List<object>();

            foreach (var parm in parms)
            {
                if (parm.ParameterType.IsInterface || parm.ParameterType.IsAbstract)
                {
                    Type parmType = parm.ParameterType;
                    object parmValue = ServiceResolver.Resolve(parmType);
                    parmValues.Add(parmValue);
                }
                else
                    throw new InvalidOperationException("Unable to inject a parameter that is not an interface or abstract type.");
            }

            return parmValues;
        }

    }
}

