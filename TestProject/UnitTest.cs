using Pixelscoding.ObjectByPath;
using Xunit;
namespace TestProject;

public class UnitTest
{
	[Fact]
	public void Test()
	{
		var testClass = new TestClass();
		PathReflector.Set(testClass, "EnumValue", "Maybe");
		
		Assert.True(testClass.EnumValue == TestEnum.Maybe);
	}
}

public class TestClass
{
	public TestEnum EnumValue { get; set; }
}

public enum TestEnum
{
	Yes,
	No,
	Maybe
}