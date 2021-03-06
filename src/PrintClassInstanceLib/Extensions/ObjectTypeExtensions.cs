﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PrintClassInstanceLib.Model;

namespace PrintClassInstanceLib.Extensions
{
    internal static class ObjectTypeExtensions
    {
        public static ObjType GetObjTypeCategory(this object obj)
        {
            if (obj == null)
            {
                return ObjType.BaseType;
            }
            var type = obj.GetType();

            var isValueType = ModifiedIsValueType(type);
            var isGenericType = ModifiedIsGenericType(type);
            var isAssignableFrom = typeof(IEnumerable).IsAssignableFrom(type);
            
            if (isGenericType)
            {
                if (isAssignableFrom &&
                    !type.FullName.Equals("System.String", StringComparison.OrdinalIgnoreCase))
                    return ObjType.IsEnumerable;

                var baseType = type.GetGenericTypeDefinition();
                if ((baseType == typeof(KeyValuePair<,>)) &&
                    !type.FullName.Equals("System.String", StringComparison.OrdinalIgnoreCase))
                    return ObjType.IsKeyValPair;
            }

            if (isValueType && type == typeof(DateTime))
            {
                return ObjType.DateTime;
            }

            return ModifiedIsArrayType(type) ? ObjType.IsArray : ObjType.BaseType;
        }

        public static bool IsEnumerable(this object obj)
        {
            var objType = obj.GetObjTypeCategory();
            return objType == ObjType.IsEnumerable;
        }

        public static bool IsArray(this object obj)
        {
            var objType = obj.GetObjTypeCategory();
            return objType == ObjType.IsArray;
        }

        public static Tuple<bool, object> HasMembersOrGetValue(this object data)
        {
            var type = data?.GetType();
            if (type == null)
                return Tuple.Create<bool, object>(true, null);

            var isValueType = ModifiedIsValueType(type);
            var isPrimitive = ModifiedIsPrimitiveType(type);
            var isGeneric = ModifiedIsGenericType(type);
            var isEnum = ModifiedIsEnum(type);
            var containsPropsOrFeilds = ContainsPropsOrFeilds(type);

            if (type.ToString() == "System.String" || isPrimitive)
            {
                return Tuple.Create(true, data);
            }

            if (isValueType && type == typeof(Guid))
            {
                return Tuple.Create<bool, object>(true, $"Guid.Parse(\"{data}\")");
            }

            if (isValueType && isGeneric)
            {
                return Tuple.Create<bool, object>(false, null);
            }

            if (isValueType && isEnum)
            {
                return Tuple.Create<bool, object>(true, $"{type}.{data}");
            }

            if (isValueType && type == typeof(DateTime))
            {
                return Tuple.Create<bool, object>(true, $"DateTime.Parse(\"{data}\")");
            }

            if (isValueType && containsPropsOrFeilds)
            {
                return Tuple.Create<bool, object>(false, null);
            }

            if (isValueType)
            {
                return Tuple.Create(true, data);
            }

            return Tuple.Create<bool, object>(false, null);
        }

        private static bool ContainsPropsOrFeilds(this Type type)
        {
            return type
                .GetMembers()
                .Where(s => s.MemberType == MemberTypes.Property || s.MemberType == MemberTypes.Field)
                .ToList()
                .Any();
        }

        public static bool ModifiedIsArrayType(this Type type)
        {
            return type.GetTypeInfo().IsArray;
        }

        public static bool ModifiedIsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }

        public static bool ModifiedIsPrimitiveType(this Type type)
        {
            return type.GetTypeInfo().IsPrimitive;
        }

        public static bool ModifiedIsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        public static bool ModifiedIsEnum(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }
    }
}
