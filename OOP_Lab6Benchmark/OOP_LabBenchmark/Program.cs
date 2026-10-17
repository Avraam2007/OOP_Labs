using BenchmarkDotNet.Running;

namespace OOP_Lab6Benchmark {
    internal class Program {
        static void Main(string[] args) {
            BenchmarkRunner.Run<Lab6BenchmarkTest>();
        }
    }
}