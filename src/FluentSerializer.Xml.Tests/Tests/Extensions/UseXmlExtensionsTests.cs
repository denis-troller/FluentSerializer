using FluentSerializer.Core.Configuration;
using FluentSerializer.Core.Converting;
using FluentSerializer.Core.Converting.Converters;
using FluentSerializer.Xml.Converting;
using FluentSerializer.Xml.Extensions;

using Moq;

using System;
using System.Globalization;

using Xunit;

namespace FluentSerializer.Xml.Tests.Tests.Extensions;

public sealed class UseXmlExtensionsTests
{
	[Fact,
		Trait("Category", "UnitTest"), Trait("DataFormat", "XML")]
	public void UseEnum_Default_UseCalled()
	{
		// Arrange
		var configurationStackMock = new Mock<IConfigurationStack<IConverter>>(MockBehavior.Loose);

		// Act
		configurationStackMock.Object.UseEnum(EnumFormats.Default);

		// Assert
		configurationStackMock
			.Verify(
				stack => stack.Use(It.IsAny<Func<IXmlConverter>>(), false),
				Times.Once
			);
	}

	[Fact,
		Trait("Category", "UnitTest"), Trait("DataFormat", "XML")]
	public void UseParsable_UseCalled()
	{
		// Arrange
		var configurationStackMock = new Mock<IConfigurationStack<IConverter>>(MockBehavior.Loose);

		// Act
		configurationStackMock.Object.UseParsable();
		configurationStackMock.Object.UseParsable(true);
		configurationStackMock.Object.UseParsable(false);
		configurationStackMock.Object.UseParsable(CultureInfo.InvariantCulture);
		configurationStackMock.Object.UseParsable(CultureInfo.InvariantCulture, true);
		configurationStackMock.Object.UseParsable(CultureInfo.InvariantCulture, false);

		// Assert
		configurationStackMock
			.Verify(
				stack => stack.Use(It.IsAny<Func<IXmlConverter>>(), It.IsAny<bool>()),
				Times.Exactly(5)
			);
	}
}
