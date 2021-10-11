﻿using Ardalis.GuardClauses;
using FluentSerializer.Core.Mapping;
using FluentSerializer.Core.NamingStrategies;
using System;
using System.Reflection;

namespace FluentSerializer.Core.Context
{
    /// <inheritdoc cref="INamingContext"/>
    public class NamingContext : INamingContext
    {
        private readonly IScanList<Type, IClassMap> _classMappings;

        public NamingContext(IScanList<Type, IClassMap> classMappings)
        {
            _classMappings = classMappings;
        }

        public INamingStrategy? FindNamingStrategy(Type type, PropertyInfo property)
        {
            Guard.Against.Null(type, nameof(type));
            Guard.Against.Null(property, nameof(property));

            var classMap = _classMappings.Scan(type);
            if (classMap is null) return null;

            return FindNamingStrategy(classMap.PropertyMaps, property);
        }

        protected INamingStrategy? FindNamingStrategy(IScanList<PropertyInfo, IPropertyMap> propertyMapping, PropertyInfo property)
        {
            Guard.Against.Null(propertyMapping, nameof(propertyMapping));
            Guard.Against.Null(property, nameof(property));

            return propertyMapping.Scan(property)?.NamingStrategy;
        }

        public INamingStrategy? FindNamingStrategy(Type type)
        {
            Guard.Against.Null(type, nameof(type));

            return _classMappings.Scan(type)?.NamingStrategy;
        }
    }
}
