﻿using FluentSerializer.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FluentSerializer.Core.Mapping
{
    public sealed class PropertyMapDictionary : TypeDictionary<PropertyInfo, IPropertyMap>
    {
        public PropertyMapDictionary(IEnumerable<IPropertyMap> dataTypes) : base(dataTypes) { }

        /// <remarks>
        /// Because <see cref="PropertyInfo"/> isn't comparible just check important properties.
        /// </remarks>
        protected override bool Compare(PropertyInfo type, IPropertyMap dataType)
        {
            if (!type.Name.Equals(dataType.Property.Name, StringComparison.Ordinal)) return false;
            if (!type.PropertyType.EqualsTopLevel(dataType.Property.PropertyType)) return false;

            return true;
        }
    }
}
