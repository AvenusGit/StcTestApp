using StcTestRouter.Interfaces;
using StcTestRouter.Exceptions;
using StcTestRouter.Models;
using StcTestRouter.Models.Routes;
using StcTestRouter.Models.Trie;
using Newtonsoft.Json.Linq;

namespace StcRouterTests
{
    [TestClass]
    public class TrieNodeTests
    {
        [TestMethod]
        public void ConstructorTest()
        {
            string key = "anyKey";
            bool value = true;

            TrieNode<bool> node = new TrieNode<bool>(key, value);

            Assert.AreEqual(key, node.Key);
            Assert.AreEqual(value, node.Value);
        }

        [TestMethod]
        public void AddChildrenTest()
        {
            TrieNode<int> node = new TrieNode<int>("anyKey", 9);

            node.AddChildren("newNode");

            Assert.IsTrue(node.Childrens.Where(node => node.Key == "newNode").Count() > 0);
            Assert.IsFalse(node.Childrens.Where(node => node.Key == "notExistNode").Count() > 0);
        }

        [TestMethod]
        public void AddChildrenWithValueTest()
        {
            TrieNode<int> node = new TrieNode<int>("anyKey", 9);

            node.AddChildren("newNode", 26);

            Assert.IsTrue(node.Childrens.Where(node => node.Key == "newNode" && node.Value == 26).Count() > 0);
            Assert.IsFalse(node.Childrens.Where(node => node.Key == "notExistNode" && node.Value == 96).Count() > 0);
        }

        [TestMethod]
        public void HasChildrenByKeyTest()
        {
            TrieNode<int> node = new TrieNode<int>("anyKey", 9);
            node.AddChildren("ok", 67);

            Assert.IsTrue(node.HasChildrenByKey("ok"));
            Assert.IsFalse(node.HasChildrenByKey("notOk"));
        }

        [TestMethod]
        public void GetChildrenByKeyTest()
        {
            TrieNode<int> children = new TrieNode<int>("children", 19);
            TrieNode<int> parent = new TrieNode<int>("parent", 47);
            parent.AddChildren(children);

            Assert.IsTrue(parent.HasChildrenByKey("children").Equals(children));
            Assert.IsFalse(parent.HasChildrenByKey("children").Equals(parent));
        }

        [TestMethod]
        public void HasChildrensTest()
        {
            TrieNode<int> children = new TrieNode<int>("children", 19);
            TrieNode<int> parent1 = new TrieNode<int>("parent1", 47);
            TrieNode<int> parent2 = new TrieNode<int>("parent2", 47);
            parent1.AddChildren(children);

            Assert.IsTrue(parent1.HasChildrens);
            Assert.IsFalse(parent2.HasChildrens);
        }


        [TestMethod]
        public void HasValueTest()
        {
            TrieNode<int?> node1 = new TrieNode<int?>("node1", 47);
            TrieNode<int?> node2 = new TrieNode<int?>("node2");

            Assert.IsTrue(node1.HasValue);
            Assert.IsFalse(node2.HasValue);
        }

        [TestMethod]
        public void EqualsTest()
        {
            TrieNode<int> node1 = new TrieNode<int>("node1", 47);
            TrieNode<int> node2 = new TrieNode<int>("node2");
            TrieNode<int> node3 = new TrieNode<int>("node1", 47);

            Assert.IsTrue(node1.Equals(node3));
            Assert.IsFalse(node1.Equals(node2));
        }
    }
}