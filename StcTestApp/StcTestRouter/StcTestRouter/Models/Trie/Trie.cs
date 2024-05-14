using StcTestRouter.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StcTestRouter.Models.Trie
{
    /// <summary>
    /// Префиксное дерево маршрутов
    /// </summary>
    /// <typeparam name="T">Тип хранимого значения</typeparam>
    public class Trie<T>
    {
        public List<TrieNode<T>> RootNodes { get; private set; } = new List<TrieNode<T>>();

        public void Add(string[] keys, T newValue)
        {
            if (keys is null || keys?.Length == 0)
                throw new RouterException("Попытка зарегистрировать маршрут с пустым путем");
            TrieNode<T>? currentNode = RootNodes.Find(node => node.Key == keys![0]);
            for (int i = 0; i < keys!.Length; i++)
            {
                string currentKey = keys[i];
                if (currentNode is null)
                {
                    currentNode = new TrieNode<T>(currentKey);
                    continue;
                }
                if (!currentNode.HasChildrenByKey(currentKey))
                    currentNode.AddChildren(currentKey);
                currentNode = currentNode.GetChildrenByKey(currentKey)!;
            }
            if (currentNode!.HasValue)
                throw new RouteExistException();
            else currentNode!.Value = newValue;
        }

        public bool TryAdd(string[] keys, T newValue)
        {
            if (keys is null || keys?.Length == 0)
                return false;
            TrieNode<T>? currentNode = RootNodes.Find(node => node.Key == keys![0]);
            for (int i = 0; i < keys!.Length; i++)
            {
                string currentKey = keys[i];
                if (currentNode is null)
                {
                    currentNode = new TrieNode<T>(currentKey);
                    continue;
                }
                if (!currentNode.HasChildrenByKey(currentKey))
                    currentNode.AddChildren(currentKey);
                currentNode = currentNode.GetChildrenByKey(currentKey)!;
            }
            if (currentNode!.HasValue)
                return false;
            else 
            { 
                currentNode!.Value = newValue; 
                return true; 
            }
        }

        public T? GetValue (string[] keys)
        {
            if (keys is null || keys?.Length == 0)
                return default(T);
            TrieNode<T>? currentNode = RootNodes.Find(node => node.Key == keys![0]);
            for (int i = 0; i < keys.Length; i++)
            {
                string currentKey = keys[i];
                if (currentNode is null || !currentNode.HasChildrenByKey(currentKey))
                    return default(T);
                currentNode = currentNode.GetChildrenByKey(currentKey)!;
            }
            return currentNode.Value;
        }

        public void Remove(string[] keys)
        {
            if (keys is null || keys?.Length == 0) return;
            TrieNode<T>? currentNode = RootNodes.Find(node => node.Key == keys![0]);
            for (int i = 0; i < keys.Length; i++)
            {
                string currentKey = keys[i];
                if (currentNode is null || !currentNode.HasChildrenByKey(currentKey))
                    return;
                currentNode = currentNode.GetChildrenByKey(currentKey);
            }
            if (currentNode is null) return;

            if (currentNode.HasChildrens)
                currentNode.Value = default(T);
            else
                currentNode = null;
        }

        public bool NodeWithValueExist (string[] keys)
        {
            if (keys is null || keys?.Length == 0) return false;

            TrieNode<T>? currentNode = RootNodes.Find(node => node.Key == keys![0]);
            for (int i = 0; i < keys.Length; i++)
            {
                string currentKey = keys[i];
                if (currentNode is null || !currentNode.HasChildrenByKey(currentKey))
                    return false;
                currentNode = currentNode.GetChildrenByKey(currentKey)!;
            }
            return currentNode is null ? false : currentNode.HasValue;
        }
    }
}
