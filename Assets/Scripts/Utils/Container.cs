using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace MiniBricks.Utils {
    public static class Container {
        private static readonly Dictionary<Type, object> services;

        static Container() {
            services = new Dictionary<Type, object>();
        }

        public static void Register<TInterface, TImplementation>(TImplementation service)  where TImplementation : TInterface {
            var type = typeof(TInterface);
            services.Add(type, service);
        }

        [NotNull]
        public static TInterface Get<TInterface>() {
            var type = typeof(TInterface);
            return (TInterface)Get(type);
        }

        [NotNull]
        public static object Get(Type type) {
            var result = services.Get(type);
            if (result == null) {
                throw new Exception($"Cant find instance of type {type}");
            }

            return result;
        }
    }
}