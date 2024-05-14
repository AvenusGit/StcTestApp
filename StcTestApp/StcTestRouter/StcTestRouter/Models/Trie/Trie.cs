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
        /// <summary>
        /// Корневые ноды
        /// </summary>
        public List<TrieNode<T>> RootNodes { get; private set; } = new List<TrieNode<T>>();

        /// <summary>
        /// Добавление новой ноды
        /// </summary>
        /// <param name="keys">Путь к создаваемой ноде</param>
        /// <param name="newValue">Значение ноды</param>
        /// <exception cref="RouterException">Общая ошибка</exception>
        /// <exception cref="RouteExistException">Ошибка, что нода уже существует</exception>
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
                    RootNodes.Add(currentNode);
                    continue;
                }
                if (currentNode.Key == currentKey) continue;
                if (!currentNode.HasChildrenByKey(currentKey))
                    currentNode.AddChildren(currentKey);
                currentNode = currentNode.GetChildrenByKey(currentKey)!;
            }
            if (currentNode!.HasValue)
                throw new RouteExistException();
            else currentNode!.Value = newValue;
        }

        /// <summary>
        /// Метод попытки добавления новой ноды
        /// </summary>
        /// <param name="keys">Путь к создаваемой ноде</param>
        /// <param name="newValue">Значение ноды</param>
        /// <returns>True - если нода успешно добавлена, иначе false</returns>
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
                    RootNodes.Add(currentNode);
                    continue;
                }
                if (currentNode.Key == currentKey) continue;
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

        /// <summary>
        /// Петод получения ноды по ключам
        /// </summary>
        /// <param name="keys">Список ключей</param>
        /// <returns>Нода или null</returns>
        public TrieNode<T>? GetNode(string[] keys)
        {
            if (keys is null || keys?.Length == 0)
                return null;
            TrieNode<T>? currentNode = RootNodes.Find(node => node.Key == keys![0]);
            for (int i = 0; i < keys!.Length; i++)
            {
                string currentKey = keys[i];
                if (currentNode is null) return null;
                if (currentNode.Key == currentKey) continue;
                if (!currentNode.HasChildrenByKey(currentKey)) return null;
                currentNode = currentNode.GetChildrenByKey(currentKey)!;
            }
            return currentNode;
        }

        /// <summary>
        /// Метод получения значения ноды по ключам
        /// </summary>
        /// <param name="keys">Список ключей</param>
        /// <returns>Значение ноды или null</returns>
        public T? GetValue (string[] keys)
        {
            if (keys is null || keys?.Length == 0)
                return default(T);
            TrieNode<T>? currentNode = RootNodes.Find(node => node.Key == keys![0]);
            for (int i = 0; i < keys.Length; i++)
            {
                string currentKey = keys[i];                
                if (currentNode is null) return default(T);
                if (currentNode.Key == currentKey) continue;
                if(!currentNode.HasChildrenByKey(currentKey)) return default(T);
                currentNode = currentNode.GetChildrenByKey(currentKey)!;
            }
            return currentNode!.Value;
        }

        /// <summary>
        /// Метод проверки существования ноды
        /// </summary>
        /// <param name="keys">Набор ключей</param>
        /// <returns>True - если существует, иначе false</returns>
        public bool NodeExist(string[] keys)
        {
            if (keys is null || keys?.Length == 0) return false;
            TrieNode<T>? currentNode = RootNodes.Find(node => node.Key == keys![0]);
            for (int i = 0; i < keys!.Length; i++)
            {
                string currentKey = keys[i];
                if (currentNode is null) return false;
                if (currentNode.Key == currentKey) continue;
                if (currentNode.HasChildrenByKey(currentKey))
                    currentNode = currentNode.GetChildrenByKey(currentKey)!;
                else return false;
            }
            return currentNode is not null;
        }

        /// <summary>
        /// Метод проверки существования заполненной ноды (со значением)
        /// </summary>
        /// <param name="keys">Набор ключей</param>
        /// <returns>True - если существует, иначе false</returns>
        public bool NodeWithValueExist (string[] keys)
        {
            if (keys is null || keys?.Length == 0) return false;
            TrieNode<T>? currentNode = RootNodes.Find(node => node.Key == keys![0]);
            for (int i = 0; i < keys.Length; i++)
            {
                string currentKey = keys[i];
                if (currentNode is null) return false;
                if (currentNode.Key == currentKey) continue;
                if (currentNode.HasChildrenByKey(currentKey))
                    currentNode = currentNode.GetChildrenByKey(currentKey)!;
                else return false;
            }
            return currentNode is not null ? currentNode.HasValue : false;
        }
    }
}
