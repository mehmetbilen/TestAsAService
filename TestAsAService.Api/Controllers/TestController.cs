using Microsoft.AspNetCore.Mvc;

namespace TestAsAService.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TestController(TestRunner testRunner) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult> Test1()
    {
        var result = await testRunner.Run("TestAsAService.Tests.UnitTestClass", "TestAsAService.Tests.UnitTestClass.Test1");
        return result?.TestsPassed > 0
            ? Ok("Test1 executed successfully")
            : StatusCode(500, "Test1 execution failed");
    }

    [HttpGet]
    public async Task<ActionResult> Test2()
    {
        var result = await testRunner.Run("TestAsAService.Tests.UnitTestClass", "TestAsAService.Tests.UnitTestClass.Test2");
        return result?.TestsPassed > 0
            ? Ok("Test2 executed successfully")
            : StatusCode(500, "Test2 execution failed");
    }
}
