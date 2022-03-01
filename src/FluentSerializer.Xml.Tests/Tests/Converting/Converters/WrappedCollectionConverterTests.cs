using FluentAssertions;
using FluentSerializer.Core.Context;
using FluentSerializer.Core.Naming;
using FluentSerializer.Core.Tests.ObjectMother;
using FluentSerializer.Xml.Converting.Converters;
using FluentSerializer.Xml.DataNodes;
using FluentSerializer.Xml.Services;
using FluentSerializer.Xml.Tests.ObjectMother;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

using static FluentSerializer.Xml.XmlBuilder;

namespace FluentSerializer.Xml.Tests.Tests.Converting.Converters;

/// <summary>
/// Test the converting between <see cref="List{T}"/> and a collection of XML elements.
/// In general most XML formats wrap collections in a named element looking like this:
/// <code>
/// <![CDATA[
/// <ParentElement>
///		<list>
///			<ListItem />
///			<ListItem />
///			<ListItem />
///		</list>
/// </ParentElement>
/// ]]>
/// </code>
/// These tests compare a named element with children to a list to validate the converter is doing it's job.
/// </summary>
public sealed class WrappedCollectionConverterTests
{
	private const string ListName = "list";
	private const string ListItemName = "ListItem";

	private readonly WrappedCollectionConverter _sut;
	private readonly Mock<ISerializerContext<IXmlNode>> _contextMock;
	private readonly Mock<IAdvancedXmlSerializer> _serializerMock;

	public WrappedCollectionConverterTests()
	{
		_sut = new WrappedCollectionConverter();
		_serializerMock = new Mock<IAdvancedXmlSerializer>();
		_contextMock = new Mock<ISerializerContext<IXmlNode>>()
			.SetupDefault(_serializerMock)
			.WithNamingStrategy(Names.Equal(ListName));
	}

	#region Failing checks
	[Fact,
		Trait("Category", "UnitTest"),	Trait("DataFormat", "XML")]
	public void Serialize_NotEnumerable_Throws()
	{
		// Arrange
		// Give it any arbitrary object that doesn't implement IEnumerable
		var input = false;

		// Act
		var canConvert = _sut.CanConvert(input.GetType());
		var result = () => _sut.Serialize(input, _contextMock.Object);

		// Assert
		canConvert.Should().BeFalse();
		result.Should()
			.ThrowExactly<NotSupportedException>()
			.WithMessage("Type 'System.Boolean' does not implement IEnumerable");
	}

	[Fact,
		Trait("Category", "UnitTest"),	Trait("DataFormat", "XML")]
	public void Deserialize_NotEnumerable_Throws()
	{
		// Arrange
		var input = Element(ListName);

		_contextMock
			// Give it any arbitrary type that doesn't implement IEnumerable
			.WithPropertyType<bool>();

		// Act
		var canConvert = _sut.CanConvert(input.GetType());
		var result = () => _sut.Deserialize(input, _contextMock.Object);

		// Assert
		canConvert.Should().BeFalse();
		result.Should()
			.ThrowExactly<NotSupportedException>()
			.WithMessage("Unable to create an enumerable collection of 'System.Boolean'");
	}
	#endregion

	#region Serialize
	[Fact,
		Trait("Category", "UnitTest"),	Trait("DataFormat", "XML")]
	public void Serialize_EmptyListOfIXmlElement()
	{
		// Arrange
		var expected = Element(ListName);
		var input = new List<IXmlElement>(0);

		// Act
		var canConvert = _sut.CanConvert(input.GetType());
		var result = _sut.Serialize(input, _contextMock.Object);

		// Assert
		canConvert.Should().BeTrue();
		result.Should().BeEquivalentTo(expected);
	}

	[Fact,
		Trait("Category", "UnitTest"),	Trait("DataFormat", "XML")]
	public void Serialize_ListOfIXmlElement()
	{
		// Arrange
		var expected = Element(ListName,
			Element(ListItemName, Text("1")),
			Element(ListItemName, Text("2"))
		);
		var input = new List<IXmlElement> {
			Element(ListItemName, Text("1")),
			Element(ListItemName, Text("2"))
		};

		_serializerMock
			.WithSerializeToElement();

		// Act
		var canConvert = _sut.CanConvert(input.GetType());
		var result = _sut.Serialize(input, _contextMock.Object);

		// Assert
		canConvert.Should().BeTrue();
		result.Should().BeEquivalentTo(expected);
	}
	#endregion

	#region Deserialize
	[Fact,
		Trait("Category", "UnitTest"),	Trait("DataFormat", "XML")]
	public void Deserialize_EmptyListOfIXmlElement()
	{
		// Arrange
		var expected = new List<IXmlElement>(0);
		var input = Element(ListName);

		_contextMock
			.WithFindNamingStrategy(Names.Equal(ListItemName))
			.WithPropertyType(expected.GetType());

		// Act
		var canConvert = _sut.CanConvert(expected.GetType());
		var result = _sut.Deserialize(input, _contextMock.Object);

		// Assert
		canConvert.Should().BeTrue();
		result.Should().BeEquivalentTo(expected);
	}

	[Fact,
		Trait("Category", "UnitTest"),	Trait("DataFormat", "XML")]
	public void Deserialize_ListOfIXmlElement()
	{
		// Arrange
		var expected = new List<IXmlElement> {
			Element(ListItemName, Text("1")),
			Element(ListItemName, Text("2"))
		};
		var input = Element(ListName,
			Element(ListItemName, Text("1")),
			Element(ListItemName, Text("2"))
		);

		_serializerMock
			.WithDeserialize();
		_contextMock
			.WithFindNamingStrategy(Names.Equal(ListItemName))
			.WithPropertyType(expected.GetType());

		// Act
		var canConvert = _sut.CanConvert(expected.GetType());
		var result = _sut.Deserialize(input, _contextMock.Object);

		// Assert
		canConvert.Should().BeTrue();
		result.Should().BeEquivalentTo(expected);
	}
	#endregion
}

