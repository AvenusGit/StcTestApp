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
            Assert.IsTrue(second?.Key == "b" && second.Value == 87);
        }

        [TestMethod]
        public void TryAddTest()
        {
        }

        [TestMethod]
        public void GetValueTest()
        {
        }

        [TestMethod]
        public void RemoveTest()
        {
        }

        [TestMethod]
        public void NodeWithValueExistTest()
        {
        }
    }
}