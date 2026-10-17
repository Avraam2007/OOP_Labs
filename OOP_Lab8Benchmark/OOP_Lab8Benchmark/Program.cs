using BenchmarkDotNet.Running;

namespace OOP_Lab8Benchmark {
    internal class Program {
        static void Main(string[] args) {
            BenchmarkRunner.Run<Lab8BenchmarkTest>();
        }
    }
}