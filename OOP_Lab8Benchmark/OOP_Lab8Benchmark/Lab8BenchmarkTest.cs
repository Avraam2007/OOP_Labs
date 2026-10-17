using BenchmarkDotNet.Attributes;

namespace OOP_Lab8Benchmark {
    [MemoryDiagnoser]
    [RankColumn]
    [MarkdownExporter, AsciiDocExporter, HtmlExporter]
    public class Lab8BenchmarkTest {
        [Params(1, 10, 100, 1000, 10000)]
        public int N;

        private List<int> _arrayList = [];
        private LinkedList<int> _linkedList = new();
        private LinkedListNode<int> _middleNode = null!;
        private int _middleIndex;
        private readonly int testValue = 999;

        [IterationSetup]
        public void Setup() {
            _arrayList = new List<int>(N);
            _linkedList = new();

            for (int i = 0; i < N; i++) {
                _arrayList.Add(i);
                _linkedList.AddLast(i);
            }

            _middleIndex = N / 2;

            var currentNode = _linkedList.First;
            for (int i = 0; i < _middleIndex; i++) {
                currentNode = currentNode?.Next;
            }

            _middleNode = currentNode ?? new LinkedListNode<int>(0);
        }

        [Benchmark]
        public void TestRemoveListCenter() {
            _arrayList.RemoveAt(_middleIndex);
        }

        [Benchmark]
        public void TestInsertListCenter() {
            _arrayList.Insert(_middleIndex, testValue);
        }

        [Benchmark]
        public void TestInsertLinkedListCenter() {
            _linkedList.AddAfter(_middleNode, testValue);
        }

        [Benchmark]
        public void TestRemoveLinkedListCenter() {
            _linkedList.Remove(_middleNode);
        }
    }
}
