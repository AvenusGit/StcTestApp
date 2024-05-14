using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StcTestRouter.Models.Trie
{
    /// <summary>
    /// Ветвь префиксного дерева
    /// </summary>
    public class TrieNode<T>
    {
        /// <summary>
        /// Нода префиксного дерева
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public TrieNode(string key) 
        {
            Key = key;
            HasValue = false;
        }

        public TrieNode(string key, T value)
        {
            Key = key;
            Value = value;
            Childrens = Childrens;
            HasValue = true;
        }
        private T? _value;
        public string Key { get; set; }
        public  T? Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                 HasValue = value != null;
            }
        }
        public List<TrieNode<T>> Childrens { get; set; } = new List<TrieNode<T>>();

        public void AddChildren (string key)
        {
            Childrens.Add(new TrieNode<T>(key));
        }

        public void AddChildren(string key, T value)
        {
            Childrens.Add(new TrieNode<T>(key, value));
        }

        public void AddChildren(TrieNode<T> node)
        {
            Childrens.Add(node);
        }

        public bool HasChildrenByKey (string key)
        {
            return Childrens.Exists(children => children.Key == key);
        }

        public TrieNode<T>? GetChildrenByKey (string key)
        {
            return Childrens.Find(children => children.Key == key);
        }

        public bool HasChildrens
        {
            get
            {
                if(Childrens is null || Childrens.Count == 0)
                    return false;
                return true;
            }
        }
        public bool HasValue { get; set; }

        public override bool Equals(object? obj)
        {
            if(obj is TrieNode<T>)
            {
                if(
                    (obj as TrieNode<T>)!.Key?.Equals(Key) == true && 
                    (obj as TrieNode<T>)!.Value?.Equals(Value) == true &&
                     (obj as TrieNode<T>)!.Childrens?.SequenceEqual(Childrens) == true
                    )
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            return $"Key:{Key},Childrens:{Childrens.Count}, Value:{Value?.ToString()}";
        }
    }
}
