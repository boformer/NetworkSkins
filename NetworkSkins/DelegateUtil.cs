namespace NetworkSkins {
    using System;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    internal static class DelegateUtil {
        /// <typeparam name="TDelegate">delegate type</typeparam>
        /// <returns>Type[] representing arguments of the delegate.</returns>
        internal static Type[] GetParameterTypes<TDelegate>()
            where TDelegate : Delegate {
            var parameters = typeof(TDelegate)
                .GetMethod("Invoke")
                .GetParameters()
                .Select(p => p.ParameterType);
            return parameters.ToArray();
        }


        /// <summary>
        /// Gets directly declared method based on a delegate that has
        /// the same name as the target method
        /// </summary>
        /// <param name="type">the class/type where the method is delcared</param>
        /// <param name="name">the name of the method</param>
        internal static MethodBase GetMethod<TDelegate>(this Type type, string name = null) where TDelegate : Delegate {
            name ??= typeof(TDelegate).Name;
            var ret = type.GetMethod(
                name,
                HarmonyLib.AccessTools.all,
                null,
                GetParameterTypes<TDelegate>(),
                null);
            if(ret == null)
                Debug.LogWarning($"could not find method {type.Name}.{name}");
            return ret;
        }

        /// <summary>
        /// creates a closed instance delegate.
        /// </summary>
        /// <typeparam name="TDelegate">a delegate with all the parameters and the return type but without the instance argument</typeparam>
        /// <param name="instance">target instace for the delegate to close on</param>
        /// <param name="name">method name or null to use delegate type as name</param>
        /// <returns>a delegate that can be called without the need to provide instance argument.</returns>
        internal static TDelegate CreateClosedDelegate<TDelegate>(object instance, string name = null) where TDelegate : Delegate {
            try {
                var type = instance.GetType();
                var method = type.GetMethod<TDelegate>(name);
                if(method == null) return null;
                return (TDelegate)Delegate.CreateDelegate(type: typeof(TDelegate), firstArgument: instance, method: method);
            } catch(Exception ex) {
                throw new Exception($"CreateClosedDelegate<{typeof(TDelegate).Name}>({instance.GetType().Name},{name}) failed!", ex);
            }
        }
    }
}
