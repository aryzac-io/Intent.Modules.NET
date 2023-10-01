using System;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.ReflectionHelper", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal static class ReflectionHelper
    {
        public static T CreateNewInstanceOf<T>()
        {
            var constructorInfo = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null)!;
            var instance = (T)constructorInfo.Invoke(Array.Empty<object>());
            return instance;
        }
    }
}