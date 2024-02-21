using BenchmarkDotNet.Running;

namespace Stellar3.Benchmarks;

public static class Program
{
    /// <summary>
    /// The main entry point into the benchmarking application.
    /// </summary>
    /// <param name="args">Arguments from the command line.</param>
    public static void Main(string[] args)
    {

        var obj = new MyVM();
        _ = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }

    public class MyVM : RxObject
    {
        
    }
}