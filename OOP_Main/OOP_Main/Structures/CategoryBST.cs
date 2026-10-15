using System;

namespace OOP_Main.Structures {
    public class CategoryNode {
        public string CategoryName { get; set; }
        public CategoryNode Left { get; set; }
        public CategoryNode Right { get; set; }

        public CategoryNode(string name) {
            CategoryName = name;
        }
    }

    public class CategoryBST {
        public CategoryNode Root { get; private set; }

        public void Insert(string categoryName) {
            Root = InsertRecursive(Root, categoryName);
        }

        private CategoryNode InsertRecursive(CategoryNode current, string name) {
            if (current == null) return new CategoryNode(name);

            int compare = string.Compare(name, current.CategoryName, StringComparison.OrdinalIgnoreCase);
            if (compare < 0) {
                current.Left = InsertRecursive(current.Left, name);
            }
            else if (compare > 0) {
                current.Right = InsertRecursive(current.Right, name);
            }
            return current;
        }

        // Обхід у симетричному порядку (In-Order: A-Z)
        public void InOrderTraversal(Action<string> visit) {
            InOrderRecursive(Root, visit);
        }

        private void InOrderRecursive(CategoryNode node, Action<string> visit) {
            if (node == null) return;
            InOrderRecursive(node.Left, visit);
            visit(node.CategoryName);
            InOrderRecursive(node.Right, visit);
        }

        // Прямий обхід (Pre-Order)
        public void PreOrderTraversal(Action<string> visit) {
            PreOrderRecursive(Root, visit);
        }

        private void PreOrderRecursive(CategoryNode node, Action<string> visit) {
            if (node == null) return;
            visit(node.CategoryName);
            PreOrderRecursive(node.Left, visit);
            PreOrderRecursive(node.Right, visit);
        }
    }
}
