namespace NetworkSkins {
    using System;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    internal static class DelegateUtil {
        /// <typeparam name="TDelegate">delegate type</typeparam>
        /// <returns>Type[] represeting arguments of the delegate.</returns>
        internal static Type[] GetParameterTypes<TDelegate>(bool instance)
            where TDelegate : Delegate {
            var parameters = typeof(TDelegate)
                .GetMethod("Invoke")
                .GetParameters()
                .Select(p => p.ParameterType);
            if(instance)
                parameters = parameters.Skip(1);
            return parameters.ToArray();
        }
            

        /// <summary>
        /// Gets directly declared method based on a delegate that has
        /// the same name as the target method
        /// </summary>
        /// <param name="type">the class/type where the method is delcared</param>
        /// <param name="name">the name of the method</param>
        internal static MethodInfo GetMethod<TDelegate>(this Type type, string name, bool instance) where TDelegate : Delegate {
            var ret = type.GetMethod(
                name,
                types: GetParameterTypes<TDelegate>(instance));
            if(ret == null)
                Debug.LogWarning($"could not find method {type.Name}.{name}");
            return ret;
        }

        internal static TDelegate CreateDelegate<TDelegate>(Type type, bool instance, string name = null) where TDelegate : Delegate {
            var method = type.GetMethod<TDelegate>(name ?? typeof(TDelegate).Name, instance);
            if(method == null) return null;
            return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), method);
        }
    }
}
