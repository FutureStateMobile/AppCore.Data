using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FutureState.AppCore.Data.Attributes;

namespace FutureState.AppCore.Data.Extensions
{
    public static class TypeExtensions
    {
        // todo: convert this to a PrimaryKeyFactory
        internal static readonly Dictionary<Type, string> PrimaryKeys = new Dictionary<Type, string>();
        private const string _defaultPrimaryKeyName = "Id";

        public static string GetPrimaryKeyName(this Type type)
        {
                string identifierName;
                if (PrimaryKeys.ContainsKey(type)) identifierName = PrimaryKeys[type];
                else
                {
                    identifierName = type.GetRuntimeProperties()
                                         .FirstOrDefault(property => property.GetCustomAttributes(true).Any(a => a.GetType().Name == nameof(PrimaryKeyAttribute)))
                                         ?.Name ?? _defaultPrimaryKeyName;
                    PrimaryKeys[type] = identifierName;
                }

                return identifierName;
        }

        public static void AddOrUpdatePrimaryKey(this Type type, string key)
        {
            if (PrimaryKeys.ContainsKey(type))
                PrimaryKeys[type] = key;
            else
                PrimaryKeys.Add(type, key);
        }
    }
}