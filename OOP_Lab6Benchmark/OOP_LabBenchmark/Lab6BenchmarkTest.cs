using BenchmarkDotNet.Attributes;
using System.Text;

namespace OOP_Lab6Benchmark {
    [MemoryDiagnoser]
    [RankColumn]
    [MarkdownExporter, AsciiDocExporter, HtmlExporter]
    public class Lab6BenchmarkTest {
        [Params(1, 10, 100, 1000, 10000, 100000)]
        public int N;


        [Benchmark]
        public string TestWithConcat() {
            string result = "";
            for (int i = 0; i < N; i++) {
                result += 'e';
            }
            return result;
        }

        [Benchmark]
        public string TestWithStringBuilder() {
            StringBuilder sb = new();
            for (int i = 0; i < N; i++) {
                sb.Append('e');
            }
            return sb.ToString();
        }
    }
}
