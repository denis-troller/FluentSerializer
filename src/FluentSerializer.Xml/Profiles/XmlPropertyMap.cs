﻿using FluentSerializer.Core.Configuration;
using FluentSerializer.Core.NamingStrategies;
using FluentSerializer.Core.Services;
using System.Reflection;
using System.Xml.Linq;

namespace FluentSerializer.Xml.Profiles
{
    public sealed class XmlPropertyMap : PropertyMap<XObject>
    {
        public XmlPropertyMap(
            SerializerDirection direction, PropertyInfo property, INamingStrategy namingStrategy, IConverter? customConverter) : 
            base (direction, property, namingStrategy, customConverter)
        { }
    }
}
