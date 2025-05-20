using System.ComponentModel;
using System.Reflection;

namespace Neuro_SDK_Csharp.Utilities;
    
internal static class ReflectionHelpers
    {
        public static IEnumerable<T> GetAllInDomain<T>()
        {
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes())
                .Where(type => !type.IsAbstract)
                .Where(type => typeof(T).IsAssignableFrom(type));

            foreach (Type type in types)
            {
                if (type.GetMethod("CreateInstance", BindingFlags.Static | BindingFlags.Public) is { } method)
                {
                    yield return (T) method.Invoke(null, null);
                }
                else
                {
                    yield return (T) Activator.CreateInstance(type);
                }
            }
        }
    }
