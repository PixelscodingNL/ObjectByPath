using Pixelscoding.ObjectByPath;
using Pixelscoding.ObjectByPath.Extensions;
using TestProject.Classes;
using Xunit;
using Xunit.Abstractions;

namespace TestProject;

public class UnitTestSettingValues
{
	private readonly ITestOutputHelper _testOutputHelper;

	public UnitTestSettingValues(ITestOutputHelper testOutputHelper)
	{
		_testOutputHelper = testOutputHelper;
	}

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
		Dictionary<string, object> dictionary = new Dictionary<string, object>
		{
			{ "subpolis[0].mr[0].bestuurder[1].geboortedatum", 2 },
			{ "subpolis[1].mr[1].bestuurder[3].geboortedatum", 16 },
			{ "subpolis[0].mr[1].bestuurder[3].geboortedatum", 8 },
			{ "subpolis[0].mr[1].bestuurder[0].geboortedatum", 5 },
			{ "subpolis[1].mr[0].bestuurder[3].geboortedatum", 12 },
			{ "subpolis[0].mr[0].bestuurder[2].geboortedatum", 3 },
			{ "subpolis[1].mr[1].bestuurder[0].geboortedatum", 13 },
			{ "subpolis[0].mr[1].bestuurder[1].geboortedatum", 6 },
			{ "subpolis[1].mr[0].bestuurder[0].geboortedatum", 9 },
			{ "subpolis[1].mr[1].bestuurder[2].geboortedatum", 15 },
			{ "subpolis[0].mr[0].bestuurder[0].geboortedatum", 1 },
			{ "subpolis[0].mr[0].bestuurder[3].geboortedatum", 4 },
			{ "subpolis[1].mr[1].bestuurder[1].geboortedatum", 14 },
			{ "subpolis[1].mr[0].bestuurder[1].geboortedatum", 10 },
			{ "subpolis[0].mr[1].bestuurder[2].geboortedatum", 7 },
			{ "subpolis[1].mr[0].bestuurder[2].geboortedatum", 11 }
		};

		var dict = dictionary.SortDictionaryDescendingKeynames();
		var total = dict.Count;
		
		_testOutputHelper.WriteLine($"Total in collection: {total}");

		var counter = total;
		foreach (var kvp in dict)
		{
			_testOutputHelper.WriteLine($"{kvp.Key}: {kvp.Value}, checking to be value {counter}.");
			Assert.True((int)kvp.Value == counter);
			counter--;
		}
	}
}