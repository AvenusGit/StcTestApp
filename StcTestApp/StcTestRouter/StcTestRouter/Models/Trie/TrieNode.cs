using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StcTestRouter.Models.Trie
{
    /// <summary>
    /// Нода префиксного дерева
    /// </summary>
    public class TrieNode<T>
    {
        /// <summary>
        /// Нода префиксного дерева
        /// </summary>
        /// <param name="key">Ключ ноды</param>
        public TrieNode(string key) 
        {
            Key = key;
            HasValue = false;
        }
        /// <summary>
        /// Нода префиксного дерева
        /// </summary>
        /// <param name="key">Ключ ноды</param>
        /// <param name="value">Значение ноды</param>
        public TrieNode(string key, T value)
        {
            Key = key;
            Value = value;
            Childrens = Childrens;
            HasValue = true;
        }
        /// <summary>
        /// Поле хранения значения ноды
        /// </summary>
        private T? _value;
        /// <summary>
        /// Ключ ноды
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// Свойство значение ноды
        /// </summary>
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
        /// <summary>
        /// Список дочек ноды
        /// </summary>
        public List<TrieNode<T>> Childrens { get; set; } = new List<TrieNode<T>>();
        /// <summary>
        /// Метод добавления дочки ноды по ключу. Создает пустую ноду.
        /// </summary>
        /// <param name="key">Ключ дочки</param>
        public void AddChildren (string key)
        {
            Childrens.Add(new TrieNode<T>(key));
        }
        /// <summary>
        /// Метод добавления дочки ноды по ключу и значению.
        /// </summary>
        /// <param name="key">Ключ дочки</param>
        /// <param name="value">Значение дочки</param>
        public void AddChildren(string key, T value)
        {
            Childrens.Add(new TrieNode<T>(key, value));
        }
        /// <summary>
        ///  Метод добавления дочки ноды по экземпляру.
        /// </summary>
        /// <param name="node">Экземпляр дочки</param>
        public void AddChildren(TrieNode<T> node)
        {
            Childrens.Add(node);
        }
        /// <summary>
        /// Проверка существования дочки по ключу
        /// </summary>
        /// <param name="key">Ключ дочки</param>
        /// <returns>True - если дочка найдена, иначе false</returns>
        public bool HasChildrenByKey (string key)
        {
            return Childrens.Exists(children => children.Key == key);
        }
        /// <summary>
        /// Метод получения дочки по ключу
        /// </summary>
        /// <param name="key">Ключ дочки</param>
        /// <returns>Экземпляр дочки, если существует, иначе false</returns>
        public TrieNode<T>? GetChildrenByKey (string key)
        {
            return Childrens.Find(children => children.Key == key);
        }
        /// <summary>
        /// Свойство проверки существования дочек ноды
        /// </summary>
        public bool HasChildrens
        {
            get
            {
                if(Childrens is null || Childrens.Count == 0)
                    return false;
                return true;
            }
        }
        /// <summary>
        /// Свойство наличия значения ноды
        /// </summary>
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
            return $"Key:{Key},Childrens:{Childrens.Count}, HasValue:{HasValue}";
        }
    }
}
