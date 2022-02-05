using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Ardalis.GuardClauses;
using FluentSerializer.Core.Configuration;
using FluentSerializer.Core.Mapping;

namespace FluentSerializer.Core.Profiles;

/// <summary>
/// Helper class to find <see cref="ISerializerProfile"/>s
/// </summary>
public static class ProfileScanner
{
	private static IEnumerable<ISerializerProfile> ScanAssembly<TSerializerProfile>(Assembly assembly) where TSerializerProfile : ISerializerProfile
	{
		foreach (var type in assembly.GetTypes())
		{
			if (type.IsAbstract) continue;
			if (!typeof(TSerializerProfile).IsAssignableFrom(type)) continue;

			yield return (ISerializerProfile)Activator.CreateInstance(type)!;
		}
	}

	/// <summary>
	/// Find all profiles of type <typeparamref name="TSerializerProfile"/> in the given <paramref name="assembly"/>,
	/// generate the profile definitions and push into an <see cref="IClassMapScanList{TSerializer}"/>
	/// </summary>
	public static IClassMapScanList<TSerializerProfile> FindClassMapsInAssembly<TSerializerProfile>(
		in Assembly assembly, SerializerConfiguration configuration)
		where TSerializerProfile : ISerializerProfile
	{
		Guard.Against.Null(assembly, nameof(assembly));
		Guard.Against.Null(configuration, nameof(configuration));

		var profiles = ScanAssembly<TSerializerProfile>(assembly);
		var classMaps = FindClassMapsInProfiles(profiles, configuration).ToList();

		return new ClassMapScanList<TSerializerProfile>(classMaps);
	}

#if NET6_OR_GREATER
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	private static IEnumerable<IClassMap> FindClassMapsInProfiles(
		IEnumerable<ISerializerProfile> profiles, SerializerConfiguration configuration)
	{
		foreach (var profile in profiles)
		{
			var classMaps = FindClassMapsInProfile(in profile, in configuration);
			foreach (var classMap in classMaps) yield return classMap;
		}
	}

#if NET6_OR_GREATER
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	private static IEnumerable<IClassMap> FindClassMapsInProfile(
		in ISerializerProfile profile, in SerializerConfiguration configuration)
	{
		return profile.Configure(in configuration);
	}
}