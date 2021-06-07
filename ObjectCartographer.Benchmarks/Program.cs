using BenchmarkDotNet.Running;

namespace ObjectCartographer.Benchmarks
{
    internal class Program
    {
        private static void Main(string[] args) => new BenchmarkSwitcher(typeof(Program).Assembly).Run(args);
    }
}