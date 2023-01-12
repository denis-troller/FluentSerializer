using Ardalis.GuardClauses;

using FluentSerializer.Core.DataNodes;
using FluentSerializer.Core.Extensions;
using FluentSerializer.Core.Text;
using FluentSerializer.Json.Configuration;

using Microsoft.Extensions.ObjectPool;

namespace FluentSerializer.Json.DataNodes.Nodes;

public readonly partial struct JsonProperty
{
	/// <inheritdoc />
	public override string ToString() => this.ToString(JsonSerializerConfiguration.Default);

	/// <inheritdoc />
	public string WriteTo(in ObjectPool<ITextWriter> stringBuilders, in bool format = true, in bool writeNull = true, in int indent = 0) =>
		DataNodeExtensions.WriteTo(this, in stringBuilders, in format, in writeNull, in indent);

	/// <inheritdoc />
	public ITextWriter AppendTo(ref ITextWriter stringBuilder, in bool format = true, in int indent = 0, in bool writeNull = true)
	{
		Guard.Against.NullOrWhiteSpace(Name, message: "The property was is an illegal state, it contains no Name"
#if NETSTANDARD2_1
			, parameterName: nameof(Name)
#endif
		);

		const char spacer = ' ';

		if (!writeNull && !HasValue) return stringBuilder;

		stringBuilder
			.Append(JsonCharacterConstants.PropertyWrapCharacter)
			.Append(Name)
			.Append(JsonCharacterConstants.PropertyWrapCharacter);

		stringBuilder.Append(JsonCharacterConstants.PropertyAssignmentCharacter);
		if (format) stringBuilder.Append(spacer);

		if (!HasValue) stringBuilder.Append(JsonCharacterConstants.NullValue);
		else stringBuilder.AppendNode(Value!, in format, in indent, in writeNull);

		return stringBuilder;
	}
}