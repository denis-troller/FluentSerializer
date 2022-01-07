using FluentSerializer.Core.Configuration;
using FluentSerializer.Core.Text.Writers;
using Microsoft.Extensions.ObjectPool;

namespace FluentSerializer.Core.Text.Extensions;

public static class ObjectPoolExtensions
{
	public static ObjectPool<ITextWriter> CreateLowAllocationStringBuilderPool(this ObjectPoolProvider provider, in ITextConfiguration textConfiguration) => 
		provider.Create(new LowAllocationStringBuilderPolicy(textConfiguration));
}