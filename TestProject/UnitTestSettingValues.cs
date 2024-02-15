using Pixelscoding.ObjectByPath;
using Pixelscoding.ObjectByPath.Extensions;
using TestProject.Classes;
using Xunit;
using Xunit.Abstractions;

namespace TestProject;

public class UnitTestSettingValues
{
	[Fact]
	public void TestEnumSet()
	{
		var testClass = new TestingClass();
		PathReflector.Set(testClass, "EnumValue", "Maybe");
		
		Assert.True(testClass.EnumValue is MaybeEnum.Maybe);
	}

	[Fact]
	public void TestBoolSet()
	{
		var testClass = new TestingClass();
		PathReflector.Set(testClass, "IsTest", 1);

		Assert.True(testClass.IsTest);
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
			{ "subpolis[0].mr[0].bestuurder[1].test", 2 },
			{ "subpolis[1].mr[1].bestuurder[3].test", 16 },
			{ "subpolis[0].mr[1].bestuurder[3].test", 8 },
			{ "subpolis[0].mr[1].bestuurder[0].test", 5 },
			{ "subpolis[1].mr[0].bestuurder[3].test", 12 },
			{ "subpolis[0].mr[0].bestuurder[2].test", 3 },
			{ "subpolis[1].mr[1].bestuurder[0].test", 13 },
			{ "subpolis[0].mr[1].bestuurder[1].test", 6 },
			{ "subpolis[1].mr[0].bestuurder[0].test", 9 },
			{ "subpolis[1].mr[1].bestuurder[2].test", 15 },
			{ "subpolis[0].mr[0].bestuurder[0].test", 1 },
			{ "subpolis[0].mr[0].bestuurder[3].test", 4 },
			{ "subpolis[1].mr[1].bestuurder[1].test", 14 },
			{ "subpolis[1].mr[0].bestuurder[1].test", 10 },
			{ "subpolis[0].mr[1].bestuurder[2].test", 7 },
			{ "subpolis[1].mr[0].bestuurder[2].test", 11 }
		};

		var dict = dictionary.SortDictionaryDescendingKeynames();
		var total = dict.Count;

		var counter = total;
		foreach (var kvp in dict)
		{
			Assert.True(kvp.Value == counter);
			counter--;
		}
	}
}