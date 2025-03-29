using System.Collections;
using System.Globalization;
using Pixelscoding.ObjectByPath;
using Pixelscoding.ObjectByPath.Extensions;
using TestProject.Classes;
using Xunit;

namespace TestProject;

public class UnitTestSettingValues
{
	[Fact]
	public void TestEnumSet()
	{
		var testClass = new TestingClass();
		PathReflector.Set(testClass, "EnumValue", "maybe");
		
		Assert.True(testClass.EnumValue is MaybeEnum.Maybe);
	}

	[Fact]
	public void TestEnumGet()
	{
		var testClass = new TestingClass
		{
			EnumValue = MaybeEnum.Maybe
		};
		var enumValue = PathReflector.Get<MaybeEnum>(testClass, "EnumValue");

		Assert.True(enumValue is MaybeEnum.Maybe);
	}

	[Fact]
	public void TestBoolSet()
	{
		var testClass = new TestingClass();
		PathReflector.Set(testClass, "IsTest", 1);

		Assert.True(testClass.IsTest);
	}
	
	[Fact]
	public void TestArraySet()
	{
		var testClass = new Test();
		PathReflector.Set(testClass, "items[1].name", "Test");
		PathReflector.Set(testClass, "items[0].name", "Test2");

		Assert.True(testClass.Items.Length == 2);
		Assert.True(testClass.Items[1].Name == "Test");
		Assert.True(testClass.Items[0].Name == "Test2");
	}
	
	[Fact]
	public void TestNullableBoolSet()
	{
		var testClass = new TestingClass();
		PathReflector.Set(testClass, "IsNullableTest", null);

		Assert.True(testClass.IsNullableTest is null);
	}

	[Fact]
	public void TestSortingKeynames()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>
		{
			{ "object[0].test[0].collection[1].property", 2 },
			{ "object[1].test[1].collection[3].property", 16 },
			{ "object[0].test[1].collection[3].property", 8 },
			{ "object[0].test[1].collection[0].property", 5 },
			{ "object[1].test[0].collection[3].property", 12 },
			{ "object[0].test[0].collection[2].property", 3 },
			{ "object[1].test[1].collection[0].property", 13 },
			{ "object[0].test[1].collection[1].property", 6 },
			{ "object[1].test[0].collection[0].property", 9 },
			{ "object[1].test[1].collection[2].property", 15 },
			{ "object[0].test[0].collection[0].property", 1 },
			{ "object[0].test[0].collection[3].property", 4 },
			{ "object[1].test[1].collection[1].property", 14 },
			{ "object[1].test[0].collection[1].property", 10 },
			{ "object[0].test[1].collection[2].property", 7 },
			{ "object[1].test[0].collection[2].property", 11 }
		};

		var dict = dictionary.SortDictionaryDescendingKeynames();
		var counter = dict.Count;
		foreach (var kvp in dict)
		{
			Assert.True(kvp.Value == counter);
			counter--;
		}
	}
}