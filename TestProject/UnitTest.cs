using Pixelscoding.ObjectByPath;
using Xunit;
namespace TestProject;

public class UnitTest
{
	[Fact]
	public void TestEnumSet()
	{
		var testClass = new TestClass();
		PathReflector.Set(testClass, "EnumValue", "Maybe");
		
		Assert.True(testClass.EnumValue == MaybeEnum.Maybe);
	}

	[Fact]
	public void TestBoolSet()
	{
		var testClass = new TestClass();
		PathReflector.Set(testClass, "IsTest", "1");

		Assert.True(testClass.IsTest);
	}
}

public class TestClass
{
	public MaybeEnum EnumValue { get; set; }
	public bool IsTest { get; set; }
}

public enum MaybeEnum
{
	Yes,
	No,
	Maybe
}