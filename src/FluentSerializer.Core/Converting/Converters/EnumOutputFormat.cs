﻿using System;
using System.ComponentModel;

namespace FluentSerializer.Core.Converting.Converters;

/// <summary>
/// Formatting configuration for <see cref="EnumConverterBase"/>
/// </summary>
[Flags]
public enum EnumOutputFormat
{
	/// <summary>
	/// Use the <see cref="DescriptionAttribute" /> to write out and fallback to the name if not present
	/// </summary>
	Default = UseDescription | UseName,

	/// <summary>
	/// Use the <see cref="DescriptionAttribute" /> to write out
	/// </summary>
	[Description("Use the System.ComponentModel.DescriptionAttribute to write out")]
	UseDescription = 0x1,
	/// <summary>
	/// Use the name to write out
	/// </summary>
	[Description("Use the name to write out")]
	UseName = 0x2,
	/// <summary>
	/// Use the name to write out
	/// </summary>
	[Description("Use the number value to write out")]
	UseNumberValue = 0x4
}