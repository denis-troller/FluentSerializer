using System;
using System.Collections.Generic;
using System.Reflection;
using FluentSerializer.Core.Configuration;
using FluentSerializer.Core.Naming.NamingStrategies;

namespace FluentSerializer.Core.Mapping;

/// <inheritdoc />
public sealed class ClassMap : IClassMap
{
	private readonly Func<INamingStrategy> _namingStrategy;
	private readonly IReadOnlyList<IPropertyMap> _propertyMap;

	/// <inheritdoc />
	public INamingStrategy NamingStrategy => _namingStrategy();
	/// <inheritdoc />
	public IScanList<PropertyInfo, IPropertyMap> PropertyMaps => new PropertyMapScanList(_propertyMap);

	/// <inheritdoc />
	public Type ClassType { get; }
	/// <inheritdoc />
	public SerializerDirection Direction { get; }

	/// <inheritdoc />
	public ClassMap(
		Type classType,
		SerializerDirection direction,
		Func<INamingStrategy> namingStrategy,
		IReadOnlyList<IPropertyMap> propertyMap)
	{
		_namingStrategy = namingStrategy;
		_propertyMap = propertyMap;

		Direction = direction;
		ClassType = Nullable.GetUnderlyingType(classType) ?? classType;
	}

}