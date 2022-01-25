using Ardalis.GuardClauses;
using FluentSerializer.Core.DataNodes;
using FluentSerializer.Json.Configuration;
using System.Diagnostics;
using FluentSerializer.Core.Text;
using FluentSerializer.Core.Text.Extensions;

namespace FluentSerializer.Json.DataNodes.Nodes;

/// <inheritdoc cref="IJsonComment"/>
[DebuggerDisplay("// {Value,nq}")]
public readonly struct JsonCommentSingleLine : IJsonComment
{
	private static readonly int TypeHashCode = typeof(JsonCommentSingleLine).GetHashCode();

	public string Name => JsonCharacterConstants.SingleLineCommentMarker;
	public string? Value { get; }

	/// <inheritdoc cref="JsonBuilder.Comment(string)"/>
	/// <remarks>
	/// <b>Please use <see cref="JsonBuilder.Comment"/> method instead of this constructor</b>
	/// </remarks>
	public JsonCommentSingleLine(string value)
	{
		Guard.Against.NullOrEmpty(value, nameof(value));

		Value = value;
	}

	/// <inheritdoc cref="IJsonComment"/>
	/// <remarks>
	/// <b>Please use <see cref="JsonParser.Parse"/> method instead of this constructor</b>
	/// </remarks>
	public JsonCommentSingleLine(in ITokenReader reader)
	{
		reader.Advance(JsonCharacterConstants.SingleLineCommentMarker.Length);

		var valueStartOffset = reader.Offset;
		var valueEndOffset = reader.Offset;

		while (reader.CanAdvance())
		{
			reader.Advance();
			valueEndOffset = reader.Offset;

			if (reader.HasCharacterAtOffset(JsonCharacterConstants.LineReturnCharacter)) break;
			if (reader.HasCharacterAtOffset(JsonCharacterConstants.NewLineCharacter)) break;
		}

		Value = reader.ReadAbsolute(valueStartOffset..valueEndOffset).ToString().Trim();
	}

	public override string ToString() => this.ToString(JsonSerializerConfiguration.Default);

	public ITextWriter AppendTo(ref ITextWriter stringBuilder, in bool format = true, in int indent = 0, in bool writeNull = true)
	{
		// JSON does not support empty property assignment or array members
		if (!writeNull && string.IsNullOrEmpty(Value)) return stringBuilder;

		const char spacer = ' ';

		// Fallback because otherwise JSON wouldn't be readable
		if (!format)
			return stringBuilder
				.Append(JsonCharacterConstants.MultiLineCommentStart)
				.Append(spacer)
				.Append(Value)
				.Append(spacer)
				.Append(JsonCharacterConstants.MultiLineCommentEnd);

		return stringBuilder
			.Append(JsonCharacterConstants.SingleLineCommentMarker)
			.Append(spacer)
			.Append(Value);
	}

	#region IEquatable

	public override bool Equals(object? obj) => obj is IDataNode node && Equals(node);

	public bool Equals(IDataNode? other) => other is IJsonNode node && Equals(node);

	public bool Equals(IJsonNode? other) => DataNodeComparer.Default.Equals(this, other);

	public override int GetHashCode() => DataNodeComparer.Default.GetHashCodeForAll(TypeHashCode, Value);

	#endregion
}