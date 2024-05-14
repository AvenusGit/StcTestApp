using StcTestRouter.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StcTestRouter.Models
{
    /// <summary>
    /// Динамический сегмент маршрута - параметр маршрута
    /// </summary>
    public class DynamicSegment
    {
        public DynamicSegment() { }
        public DynamicSegment(string name, Type type){
            Name = name;
            Type= type;
        }
        /// <summary>
        /// Имя сегмента из шаблона
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Тип параметра
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Проверяет, что шаблон сегмента подходит динамическому сегменту
        /// </summary>
        /// <param name="segmentTemplate">Шаблон сегмента</param>
        /// <returns>Boolean значение соответствия шаблону</returns>
        public static bool IsDynamicSegmentTemplate(string segmentTemplate)
        {
            Regex regex = new Regex("^\\{[\\S]+\\:(int|string|float|dateTime|bool)\\}$");
            return regex.IsMatch(segmentTemplate);
        }
        /// <summary>
        /// Получить тип по его текстовому описанию из шаблона
        /// </summary>
        /// <param name="typeName">Текстовое описание</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Type GetTypeByString(string typeName)
        {
            switch (typeName)
            {
                case "int": return typeof(int);
                case "float": return typeof(float);
                case "string": return typeof(string);
                case "bool": return typeof(bool);
                case "dateTime": return typeof(DateTime);
                default: throw new Exception("Не удалось определить тип параметра, вероятно он не поддерживается.");
            }
        }
        /// <summary>
        /// Попытка парсинга типа маршрута из строки
        /// </summary>
        /// <param name="dynamicSegmentTemplate">Строка - описание маршрута</param>
        /// <param name="dynamicSegment">out параметр, куда положить результат</param>
        /// <returns>True - если парсинг описания параметра маршрута удался, иначе - false</returns>
        public static bool TryParse(string dynamicSegmentTemplate, out DynamicSegment? dynamicSegment)
        {
            if (DynamicSegment.IsDynamicSegmentTemplate(dynamicSegmentTemplate))
            {
                dynamicSegmentTemplate = dynamicSegmentTemplate.Replace("{", "");
                dynamicSegmentTemplate = dynamicSegmentTemplate.Replace("}", "");
                string[] dynamicSegmentsParts = dynamicSegmentTemplate.Split(':');
                dynamicSegment = new DynamicSegment(dynamicSegmentsParts[0], GetTypeByString(dynamicSegmentsParts[1]));
                return true;
            }
            else
            {
                dynamicSegment = null;
                return false;
            }
        }
        /// <summary>
        /// Метод, используя рефлексию, пытается проверить наличие у типа Т метода на парсинг объекта из строки и применить его к подаваемой на вход строке
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="stringValue">Строковое значение объекта</param>
        /// <param name="convertedValue">Результат парсинга</param>
        /// <returns>True - если парсинг удался, иначе false</returns>
        public static bool TryConvertValue<T>(string stringValue, out T convertedValue)
        {
            var targetType = typeof(T);
            if (targetType == typeof(string))
            {
                convertedValue = (T)Convert.ChangeType(stringValue, typeof(T));
                return true;
            }
            var nullableType = targetType.IsGenericType &&
                           targetType.GetGenericTypeDefinition() == typeof(Nullable<>);
            if (nullableType)
            {
                if (string.IsNullOrEmpty(stringValue))
                {
                    convertedValue = default(T);
                    return true;
                }
                targetType = new NullableConverter(targetType).UnderlyingType;
            }

            Type[] argTypes = { typeof(string), targetType.MakeByRefType() };
            var tryParseMethodInfo = targetType.GetMethod("TryParse", argTypes);
            if (tryParseMethodInfo == null)
            {
                convertedValue = default(T);
                return false;
            }

            object[] args = { stringValue, null };
            var successfulParse = (bool)tryParseMethodInfo.Invoke(null, args);
            if (!successfulParse)
            {
                convertedValue = default(T);
                return false;
            }

            convertedValue = (T)args[1];
            return true;
        }

        public override bool Equals(object? obj)
        {
            if(obj is DynamicSegment)
            {
                if ((obj as DynamicSegment)!.Name.Equals(Name) && (obj as DynamicSegment)!.Type == Type)
                    return true;
            }
            return false;
        }
    }
}
