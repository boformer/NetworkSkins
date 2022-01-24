namespace NetworkSkins {
    using System;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    internal static class DelegateUtil {
        /// <typeparam name="TDelegate">delegate type</typeparam>
        /// <returns>Type[] represeting arguments of the delegate.</returns>
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
        internal static MethodInfo GetMethod<TDelegate>(this Type type, string name = null) where TDelegate : Delegate {
            if (name == null) name = typeof(TDelegate).Name;
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
    }
}
