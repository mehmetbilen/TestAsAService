namespace TestAsAService.Tests;

public class UnitTestClass
{
    [Fact]
    public void Test1()
    {
        System.Threading.Thread.Sleep(1000);
        Assert.True(true);

    }

    [Fact]
    public void Test2()
    {
        System.Threading.Thread.Sleep(1000);
        Assert.True(true);
    }
}