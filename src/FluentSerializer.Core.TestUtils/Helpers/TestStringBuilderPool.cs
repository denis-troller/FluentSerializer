using Microsoft.Extensions.ObjectPool;
using FluentSerializer.Core.Text;
using FluentSerializer.Core.Text.Extensions;

namespace FluentSerializer.Core.TestUtils.Helpers;

public readonly struct TestStringBuilderPool
{
	private static readonly ObjectPoolProvider ObjectPoolProvider = new DefaultObjectPoolProvider();

	public static readonly ObjectPool<ITextWriter> Default =
		ObjectPoolProvider.CreateStringBuilderPool(TestStringBuilderConfiguration.Default);
}