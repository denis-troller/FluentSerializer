﻿using Ardalis.GuardClauses;
using FluentSerializer.Core.DataNodes;
using FluentSerializer.Core.Extensions;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace FluentSerializer.Json.DataNodes.Nodes
{
    /// <inheritdoc cref="IJsonProperty"/>
    [DebuggerDisplay("{Name}: {GetDebugValue(), nq},")]
    public readonly struct JsonProperty : IJsonProperty
    {
        [DebuggerHidden, DebuggerNonUserCode, DebuggerStepThrough]
        private string GetDebugValue()
        {
            if (_children.Length == 0) return JsonCharacterConstants.NullValue;
            var value = _children[0];
            if (value is JsonValue jsonValue) return jsonValue.Value ?? JsonCharacterConstants.NullValue;
            return value.Name;
        }

        public string Name { get; }

        private readonly IJsonNode[] _children;
        public IReadOnlyList<IJsonNode> Children => _children;

        public IJsonNode? Value => _children.FirstOrDefault();

        /// <inheritdoc cref="JsonBuilder.Property(string, IJsonPropertyContent)"/>
        /// <remarks>
        /// <b>Please use <see cref="JsonBuilder.Property"/> method instead of this constructor</b>
        /// </remarks>
        public JsonProperty(string name, IJsonPropertyContent? value = null)
        {
            Guard.Against.InvalidName(name, nameof(name));

            Name = name;
            _children = value is null ? Array.Empty<IJsonNode>() : new IJsonNode[] { value }; 
        }

        /// <inheritdoc cref="IJsonObject"/>
        /// <remarks>
        /// <b>Please use <see cref="JsonParser.Parse"/> method instead of this constructor</b>
        /// </remarks>
        public JsonProperty(ReadOnlySpan<char> text, ref int offset)
        {
            var nameStartOffset = offset;
            var nameEndOffset = offset;

            while (offset < text.Length)
            {
                nameEndOffset = offset;

                var character = text[offset];

                if (character == JsonCharacterConstants.ObjectEndCharacter) break;
                if (character == JsonCharacterConstants.ArrayEndCharacter) break;
                offset++;
                if (character == JsonCharacterConstants.DividerCharacter) break;
                if (character == JsonCharacterConstants.PropertyWrapCharacter) break;
            }
            
            Name = text[nameStartOffset..nameEndOffset].ToString().Trim();

            while (offset < text.Length)
            {
                var character = text[offset];

                if (character == JsonCharacterConstants.ObjectEndCharacter) break;
                if (character == JsonCharacterConstants.ArrayEndCharacter) break;
                offset++;
                if (character == JsonCharacterConstants.DividerCharacter) break;
                if (char.IsWhiteSpace(character)) continue;

                if (character == JsonCharacterConstants.PropertyAssignmentCharacter) break;
            }

            _children = new IJsonNode[1];

            while (offset < text.Length)
            {
                var character = text[offset];

                if (character == JsonCharacterConstants.ObjectEndCharacter) return;
                if (character == JsonCharacterConstants.ArrayEndCharacter) return;

                if (character == JsonCharacterConstants.ObjectStartCharacter)
                {
                    _children[0] = new JsonObject(text, ref offset);
                    return;
                }
                if (character == JsonCharacterConstants.ArrayStartCharacter)
                {
                    _children[0] = new JsonArray(text, ref offset);
                    return;
                }

                if (!char.IsWhiteSpace(character)) break;
                offset++;
                if (character == JsonCharacterConstants.DividerCharacter) return;
            }

            _children[0] = new JsonValue(text, ref offset);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder = AppendTo(stringBuilder);
            return stringBuilder.ToString();
        }

        public void WriteTo(ObjectPool<StringBuilder> stringBuilders, TextWriter writer, bool format = true, bool writeNull = true, int indent = 0)
        {
            Guard.Against.NullOrWhiteSpace(Name, nameof(Name), "The property was is an illegal state, it contains no Name");

            var stringBuilder = stringBuilders.Get();

            stringBuilder = AppendTo(stringBuilder, format, indent, writeNull);
            writer.Write(stringBuilder);

            stringBuilder.Clear();
            stringBuilders.Return(stringBuilder);
        }

        public StringBuilder AppendTo(StringBuilder stringBuilder, bool format = true, int indent = 0, bool writeNull = true)
        {
            Guard.Against.NullOrWhiteSpace(Name, nameof(Name), "The property was is an illegal state, it contains no Name");

            const char spacer = ' ';

            var childValue = Children.FirstOrDefault();
            if (!writeNull && childValue is null) return stringBuilder;

            stringBuilder
                .Append(JsonCharacterConstants.PropertyWrapCharacter)
                .Append(Name)
                .Append(JsonCharacterConstants.PropertyWrapCharacter);

            if (format) stringBuilder.Append(spacer);
            stringBuilder.Append(JsonCharacterConstants.PropertyAssignmentCharacter);
            if (format) stringBuilder.Append(spacer);

            if (childValue is null) stringBuilder.Append(JsonCharacterConstants.NullValue);
            else stringBuilder.AppendNode(childValue, format, indent, writeNull);

            return stringBuilder;
        }


        #region IEquatable

        public override bool Equals(object? obj)
        {
            if (obj is not IJsonNode node) return false;
            return Equals(node);
        }

        public bool Equals(IJsonNode? other)
        {
            if (other is not JsonProperty otherProperty) return false;

            if (Name != otherProperty.Name) return false;

            return Children[0].Equals(otherProperty.Children[0]);
        }

        public override int GetHashCode()
        {
            if (_children.Any() != true) return 0;

            var hash = new HashCode();
            hash.Add(Name.GetHashCode());
            foreach (var child in _children)
                hash.Add(child);

            return hash.ToHashCode();
        }

        #endregion
    }
}
