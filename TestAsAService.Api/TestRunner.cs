using System.Reflection;
using Xunit.SimpleRunner;

namespace TestAsAService.Api;

public class TestRunner
{
    private readonly string _testAssembly;
    private ExecutionCompleteInfo? executionCompleteInfo=null;

    public TestRunner()
    {
        _testAssembly = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location) + "\\TestAsAService.Tests.dll";

    }
    public async Task<ExecutionCompleteInfo?> Run(string className, string methodName)
    {


        // The options class allows you to not only influence how the test execution happens,
        // but also to subscribe to notifications of events during discovery and execution.
        var options = new AssemblyRunnerOptions(assemblyFileName: _testAssembly)
        {
            OnDiagnosticMessage = msg => Console.WriteLine(">> Diagnostic message: {0} <<", msg.Message),
            OnDiscoveryStarting = OnDiscoveryStarting,
            OnDiscoveryComplete = OnDiscoveryComplete,
            OnErrorMessage = OnErrorMessage,
            OnExecutionStarting = OnExecutionStarting,
            OnExecutionComplete = OnExecutionComplete,
            OnInternalDiagnosticMessage = msg => Console.WriteLine(">> Internal diagnostic message: {0} <<", msg.Message),
            OnTestFailed = OnTestFailed,
            OnTestNotRun = OnTestNotRun,
            OnTestPassed = OnTestPassed,
            OnTestSkipped = OnTestSkipped,
        };


        options.Filters.AddIncludedClassFilter(className);
        options.Filters.AddIncludedMethodFilter(methodName);

        await using (var runner = new AssemblyRunner(options))
            await runner.Run();

        return executionCompleteInfo;
    }
    void OnDiscoveryComplete(DiscoveryCompleteInfo info)
    {
        Console.WriteLine($">> Discovery complete ({info.TestCasesToRun} test cases to run) <<");
    }

    void OnDiscoveryStarting()
    {
        Console.WriteLine(">> Discovery starting <<");
    }

    void OnErrorMessage(ErrorMessageInfo info)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;

        Console.WriteLine("[ERROR] {0}", info.ErrorMessageType);
        WriteException(info.Exception);

        Console.ResetColor();
    }

    void OnExecutionComplete(ExecutionCompleteInfo info)
    {
        executionCompleteInfo = info;
       
    }

    void OnExecutionStarting(ExecutionStartingInfo info)
    {
        Console.WriteLine(">> Execution starting <<");
    }

    void OnTestFailed(TestFailedInfo info)
    {
        Console.ForegroundColor = ConsoleColor.Red;

        Console.WriteLine("[FAIL] {0}", info.TestDisplayName);
        WriteException(info.Exception);

        Console.ResetColor();

    }

    void OnTestNotRun(TestNotRunInfo info)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("[NOT RUN] {0}", info.TestDisplayName);
        Console.ResetColor();
    }

    void OnTestPassed(TestPassedInfo info)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("[PASS] {0}", info.TestDisplayName);
        Console.ResetColor();
    }

    static void OnTestSkipped(TestSkippedInfo info)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[SKIP] {0}: {1}", info.TestDisplayName, info.SkipReason);
        Console.ResetColor();
    }

    void WriteException(ExceptionInfo exception, string indent = "   ")
    {
        var indentedNewLine = Environment.NewLine + indent;

        Console.WriteLine("{0}{1}: {2}", indent, exception.FullType, exception.Message.Replace(Environment.NewLine, indentedNewLine));
        if (exception.StackTrace is not null)
            Console.WriteLine("{0}{1}", indent, exception.StackTrace.Replace(Environment.NewLine, indentedNewLine));

        var newIndent = indent + "   ";
        for (var idx = 0; idx < exception.InnerExceptions.Count; ++idx)
        {
            Console.WriteLine("{0}----- Inner exception #{1} -----", newIndent, idx + 1);
            WriteException(exception.InnerExceptions[idx], newIndent);
        }
    }
}
