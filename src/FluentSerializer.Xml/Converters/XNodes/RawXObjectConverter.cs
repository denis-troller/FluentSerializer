﻿using FluentSerializer.Core.Configuration;
using FluentSerializer.Core.Context;
using FluentSerializer.Core.Services;
using System;
using System.Xml.Linq;

namespace FluentSerializer.Xml.Converters.XNodes
{
    public sealed class RawXObjectConverter : IConverter<XElement>
    {
        public SerializerDirection Direction => SerializerDirection.Both;

        public bool CanConvert(Type targetType) => typeof(XObject).IsAssignableFrom(targetType);

        public object? Deserialize(XElement objectToDeserialize, ISerializerContext context)
        {
            return objectToDeserialize;
        }

        public XElement? Serialize(object objectToSerialize, ISerializerContext context)
        {
            if (objectToSerialize is null) return null;
            if (objectToSerialize is XObject xObjectToSerialize) return new XFragment(xObjectToSerialize);

            throw new NotSupportedException($"Type of '${objectToSerialize.GetType().FullName}' could not be converted");
        }
    }
}