using System;
using System.Linq;

namespace TinyWebStack.Extensions
{
    internal static class TypeExtensions
    {
        public static bool ImplementsInterface(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Any(t => (interfaceType.IsGenericType && t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType) || (!interfaceType.IsGenericType && t == interfaceType));
        }

        public static bool TryGetGenericInterfaceImplementedType(this Type type, Type generic, out Type implemented)
        {
            implemented = type.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == generic).Select(t => t.GetGenericArguments()[0]).FirstOrDefault();
            return (implemented != null);
        }
    }
}
