using StcTestRouter.Interfaces;
using StcTestRouter.Exceptions;
using StcTestRouter.Models;
using StcTestRouter.Models.Routes;
using StcTestRouter.Models.Trie;

namespace StcRouterTests
{
    [TestClass]
    public class TrieTests
    {
        [TestMethod]
        public void AddTest()
        {
            Trie<int> trie = new Trie<int>();

            trie.Add(["a", "b"], 87);

            TrieNode<int>? first = trie.RootNodes.Find(node => node.Key == "a");
            Assert.IsTrue(first?.Key == "a" && !first.HasValue);
            TrieNode<int>? second = first.GetChildrenByKey("b");
            Assert.IsTrue(second?.Key == "b" && second.HasValue && second.Value == 87);
            Assert.ThrowsException<RouteExistException> (() => trie.Add(["a", "b"], 87));
            Assert.ThrowsException<RouterException>(() => trie.Add(null!, 87));
        }

        [TestMethod]
        public void TryAddTest()
        {
            Trie<int> trie = new Trie<int>();

            Assert.IsTrue(trie.TryAdd(["a", "b"], 87));
            Assert.IsFalse(trie.TryAdd(["a", "b"], 87));
            Assert.IsFalse(trie.TryAdd(null!, 87));
        }

        [TestMethod]
        public void GetValueTest()
        {
            Trie<int> trie = new Trie<int>();

            trie.Add(["a", "b"], 102);
            Assert.AreEqual(trie.GetValue(["a", "c"]), default(int));
            Assert.IsTrue(trie.GetValue(["a", "b"]) == 102);

            Trie<string> stringTrie = new Trie<string>();
            stringTrie.Add(["x", "y"], "testValue");
            Assert.IsNull(stringTrie.GetValue(["a", "c"]));
            Assert.IsTrue(stringTrie.GetValue(["x", "y"]) == "testValue");
        }

        [TestMethod]
        public void GetValueSameStaticSegmentTest()
        {
            Trie<int> trie = new Trie<int>();

            trie.Add(["a", "a"], 400);
            Assert.AreEqual(trie.GetValue(["a", "c"]), default(int));
            Assert.IsTrue(trie.GetValue(["a", "a"]) == 400);
        }

        [TestMethod]
        public void GetNodeTest() 
        {
            Trie<int> trie = new Trie<int>();
            trie.Add(["a", "b"], 102);

            TrieNode<int>? node = trie.GetNode(["a", "b"]);

            Assert.IsNotNull(node);
            Assert.IsTrue(node.HasValue);
            Assert.IsTrue(node.Key == "b" && node.Value == 102);
        }

        [TestMethod]
        public void NodeExistTest()
        {
            Trie<int> trie = new Trie<int>();
            trie.Add(["a", "b"], 102);

            Assert.IsTrue(trie.NodeExist(["a", "b"]));
            Assert.IsFalse(trie.NodeExist(["a", "c"]));
        }

        [TestMethod]
        public void NodeWithValueExistTest()
        {
            Trie<int> trie = new Trie<int>();
            trie.Add(["a", "b", "c"], 100);

            Assert.IsTrue(trie.NodeWithValueExist(["a", "b", "c"]));
            Assert.IsFalse(trie.NodeWithValueExist(["a", "b"]));
        }
    }
}