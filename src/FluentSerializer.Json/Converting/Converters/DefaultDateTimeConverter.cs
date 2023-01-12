#if NETSTANDARD2_0
using FluentSerializer.Core.Dirty.BackwardsCompatibility.NetFramework;
#endif
using FluentSerializer.Core.Constants;
using FluentSerializer.Json.Converting.Converters.Base;
using FluentSerializer.Json.DataNodes;

using System;
using System.Globalization;

namespace FluentSerializer.Json.Converting.Converters;

/// <summary>
/// Converts dates <br/>
/// Using <see cref="DateTime.Parse(string, IFormatProvider?)"/> 
/// with <see cref="CultureInfo.CurrentCulture"/>
/// and <see cref="DateTimeStyles.AdjustToUniversal"/> for deserializing <br/>
/// Using <c>DateTime.ToUniversalTime().ToString(IsoDateTimeFormat, CultureInfo.CurrentCulture)</c> for serializing
/// with a format like yyyy-MM-ddTHH:mm:ssK
/// </summary>
public sealed class DefaultDateTimeConverter : SimpleTypeConverter<DateTime>
{
	/// <inheritdoc />
	protected override DateTime ConvertToDataType(in string currentValue)
	{
		var dateValue = currentValue.Length >= 2 && currentValue.StartsWith(JsonCharacterConstants.PropertyWrapCharacter)
			? currentValue[1..^1]
			: currentValue;
		return DateTime.Parse(dateValue, CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal);
	}

	/// <inheritdoc />
	protected override string ConvertToString(in DateTime value) =>
		JsonCharacterConstants.PropertyWrapCharacter +
		value.ToUniversalTime().ToString(DateTimeConstants.IsoDateTimeFormat, CultureInfo.CurrentCulture) +
		JsonCharacterConstants.PropertyWrapCharacter;
}