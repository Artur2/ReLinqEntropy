using Remotion.Linq.Utilities;
using System;
using System.Reflection;

namespace ReLinqEntropy.Internal
{
    internal static class ReflectionUtility
    {
        public static Type GetMemberReturnType(MemberInfo member)
        {
            var propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                return propertyInfo.PropertyType;
            }

            var fieldInfo = member as FieldInfo;
            if (fieldInfo != null)
            {
                return fieldInfo.FieldType;
            }

            var methodInfo = member as MethodInfo;
            if (methodInfo != null)
            {
                return methodInfo.ReturnType;
            }

            throw new ArgumentException("Argument must be FieldInfo, PropertyInfo, or MethodInfo.", "member");
        }

        public static Type GetItemTypeOfClosedGenericIEnumerable(Type enumerableType, string argumentName)
        {
            if (!ItemTypeReflectionUtility.TryGetItemTypeOfClosedGenericIEnumerable(enumerableType, out Type itemType))
            {
                var message = string.Format("Expected a closed generic type implementing IEnumerable<T>, but found '{0}'.", enumerableType);
                throw new ArgumentException(message, argumentName);
            }

            return itemType;
        }
    }
}
