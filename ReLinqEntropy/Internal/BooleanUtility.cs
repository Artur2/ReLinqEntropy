using System;
using System.Reflection;

namespace ReLinqEntropy.Internal
{
    public static class BooleanUtility
    {
        public static bool IsBooleanType(Type type) => type == typeof(bool) || type == typeof(bool?);

        public static Type GetMatchingIntType(Type type)
        {
            if (type == typeof(bool))
            {
                return typeof(int);
            }
            else if (type == typeof(bool?))
            {
                return typeof(int?);
            }

            throw new ArgumentException("Type must be Boolean or Nullable<Boolean>.", "type");
        }

        public static Type GetMatchingBoolType(Type type)
        {
            if (type == typeof(int))
            {
                return typeof(bool);
            }
            if (type == typeof(int?))
            {
                return typeof(bool?);
            }

            throw new ArgumentException("Type must be Int32 or Nullable<Int32>.", "type");
        }

        public static bool? ConvertNullableIntToNullableBool(int? nullableValue) => nullableValue.HasValue ? (bool?)Convert.ToBoolean(nullableValue.Value) : null;

        public static MethodInfo GetIntToBoolConversionMethod(Type intType)
        {
            if (intType == typeof(int))
            {
                return typeof(Convert).GetMethod("ToBoolean", new[] { typeof(int) });
            }
            if (intType == typeof(int?))
            {
                return typeof(BooleanUtility).GetMethod("ConvertNullableIntToNullableBool", new[] { typeof(int?) });
            }

            throw new ArgumentException("Type must be Int32 or Nullable<Int32>.", "intType");
        }
    }
}
