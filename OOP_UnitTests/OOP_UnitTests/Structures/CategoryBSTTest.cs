using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Structures;
using System.Collections.Generic;

namespace OOP_UnitTests.Structures {
    [TestClass]
    public class CategoryBSTTests {
        private CategoryBST _bst;

        [TestInitialize]
        public void Setup() {
            _bst = new CategoryBST();
        }

        [TestCleanup]
        public void Cleanup() {
            _bst = null;
        }

        [TestMethod]
        public void Insert_SingleElement_SetsRoot() {
            _bst.Insert("Electronics");

            Assert.IsNotNull(_bst.Root);
            Assert.AreEqual("Electronics", _bst.Root.CategoryName);
            Assert.IsNull(_bst.Root.Left);
            Assert.IsNull(_bst.Root.Right);
        }

        [TestMethod]
        public void Insert_MultipleElements_BuildsCorrectTreeStructure() {
            _bst.Insert("Home");
            _bst.Insert("Books");
            _bst.Insert("Toys");

            Assert.AreEqual("Home", _bst.Root.CategoryName);
            Assert.AreEqual("Books", _bst.Root.Left.CategoryName);
            Assert.AreEqual("Toys", _bst.Root.Right.CategoryName);
        }

        [TestMethod]
        public void Insert_DuplicateCaseInsensitive_IgnoresDuplicate() {
            _bst.Insert("Electronics");
            _bst.Insert("electronics");
            _bst.Insert("ELECTRONICS");

            var visited = new List<string>();
            _bst.InOrderTraversal(name => visited.Add(name));

            Assert.AreEqual(1, visited.Count);
            Assert.AreEqual("Electronics", visited[0]);
        }

        [TestMethod]
        public void InOrderTraversal_PopulatedTree_ReturnsAlphabeticalOrder() {
            _bst.Insert("Media");
            _bst.Insert("Auto");
            _bst.Insert("Garden");
            _bst.Insert("Books");

            var result = new List<string>();
            _bst.InOrderTraversal(name => result.Add(name));

            CollectionAssert.AreEqual(new List<string> { "Auto", "Books", "Garden", "Media" }, result);
        }

        [TestMethod]
        public void PreOrderTraversal_PopulatedTree_ReturnsRootFirstOrder() {
            _bst.Insert("Media");
            _bst.Insert("Auto");
            _bst.Insert("Garden");
            _bst.Insert("Books");

            var result = new List<string>();
            _bst.PreOrderTraversal(name => result.Add(name));

            CollectionAssert.AreEqual(new List<string> { "Media", "Auto", "Garden", "Books" }, result);
        }

        [TestMethod]
        public void Traversal_EmptyTree_DoesNotInvokeAction() {
            int callCount = 0;

            _bst.InOrderTraversal(_ => callCount++);
            _bst.PreOrderTraversal(_ => callCount++);

            Assert.AreEqual(0, callCount);
        }
    }
}
