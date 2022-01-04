using System;
using System.Reflection;
using FluentSerializer.Core.Context;
using FluentSerializer.Core.Naming.NamingStrategies;

namespace FluentSerializer.Xml.Profiles;

/// <summary>
/// The XML #text node does not require a naming strategy.
/// However making a naming strategy nullable doesn't seem to be worth the added complexity.
/// </summary>
internal sealed class TextNamingStrategy : INamingStrategy
{
	internal static readonly INamingStrategy Instance = new TextNamingStrategy();
	private static readonly NotSupportedException UsingException = new(
		"This INamingStrategy is not supposed to be used outside of elements that don't require a name!"
	);

	private TextNamingStrategy() { }
	public string GetName(PropertyInfo property, INamingContext namingContext) => throw UsingException;
	public string GetName(Type classType, INamingContext namingContext) => throw UsingException;
}