using System;
using System.Collections;
using System.Collections.Generic;
namespace OOP_Main.Entities {
    public class ProductCatalogCollection : IEnumerable<Product> {
        private Product[] _items;
        private int _count = 0;

        public ProductCatalogCollection(int initialCapacity = 10) {
            _items = new Product[initialCapacity];
        }

        public void Add(Product product) {
            if (_count == _items.Length) {
                Array.Resize(ref _items, _items.Length * 2);
            }
            _items[_count++] = product;
        }

        public int Count => _count;

        public IEnumerator<Product> GetEnumerator() => new ProductEnumerator(_items, _count);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class ProductEnumerator : IEnumerator<Product> {
            private readonly Product[] _array;
            private readonly int _size;
            private int _currentIndex = -1;

            public ProductEnumerator(Product[] array, int size) {
                _array = array;
                _size = size;
            }

            public Product Current {
                get {
                    if (_currentIndex < 0 || _currentIndex >= _size)
                        throw new InvalidOperationException("Iterator is out of bounds.");
                    return _array[_currentIndex];
                }
            }

            object IEnumerator.Current => Current;

            public bool MoveNext() {
                _currentIndex++;
                return _currentIndex < _size;
            }

            public void Reset() {
                _currentIndex = -1;
            }

            public void Dispose() { }
        }
    }
}
