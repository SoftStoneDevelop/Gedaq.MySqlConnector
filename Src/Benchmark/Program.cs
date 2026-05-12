using Benchmark.Benchmarks;
using BenchmarkDotNet.Running;
using System.Threading.Tasks;

namespace Benchmark
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            BenchmarkRunner.Run<CompareDapper>();
        }
    }
}