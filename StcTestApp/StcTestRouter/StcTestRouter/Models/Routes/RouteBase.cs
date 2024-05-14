using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StcTestRouter.Models.Routes
{
    public abstract class RouteBase
    {
        /// <summary>
        /// Список статических сегментов маршрута
        /// </summary>
        protected IEnumerable<string> StaticSegments { get; set; }
        /// <summary>
        /// Список динамических сегментов
        /// </summary>
        public IList<DynamicSegment?> DynamicSegments { get; set; }

        /// <summary>
        /// Простейшая проверка соответствует ли строка минимальному шаблону маршрута
        /// </summary>
        /// <param name="template">Строка шаблона</param>
        /// <returns></returns>
        protected static bool IsRoutePatternMath(string template)
        {
            Regex regex = new Regex("^\\/[\\s\\S]*\\/$");
            return regex.IsMatch(template);
        }

        /// <summary>
        /// Метод получения всех статических сегментов в виде одной строки с разделителем
        /// </summary>
        public string GetFullStaticSegments()
        {
            StringBuilder stringBuilder = new StringBuilder();
            return "/" + stringBuilder.AppendJoin('/', StaticSegments) + "/";
        }

        /// <summary>
        /// Получить все параметры метода из аргументов маршрута
        /// </summary>
        /// <param name="args">Аргументы маршрута</param>
        /// <returns></returns>
        public abstract object[]? GetActionParameters(string[] args);
        /// <summary>
        /// Вызвать действие синхронно
        /// </summary>
        /// <param name="parameters">Параметры действия</param>
        public abstract void CallAction(params object[] parameters);
        /// <summary>
        /// Вызвать действие асинхронно
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <param name="parameters">Параметры действия</param>
        /// <returns></returns>
        public abstract Task CallActionAsync(CancellationToken cancellationToken, params object[] parameters);
        /// <summary>
        /// Проверка на возможность выполнения действия с указанным набором параметров
        /// </summary>
        /// <param name="parameters">Набор параметров</param>
        /// <returns></returns>
        public abstract bool CanCallAction(params object[] parameters);
        /// <summary>
        /// Получить массив типов параметров маршрута
        /// </summary>
        public abstract Type[] GetDynamicSegmentsTypes();
        /// <summary>
        /// Проверка соответствия количества динамических сегментов требуемому
        /// </summary>
        /// <param name="dynamicSegments">Массив динамических сегментов</param>
        /// <param name="requiredSegmentsCount">Требуемое количество</param>
        /// <returns></returns>
        public static bool CheckDynamicSegmentsCount(DynamicSegment[] dynamicSegments, int requiredSegmentsCount)
        {
            return (dynamicSegments is null ? 0 : dynamicSegments.Length) == requiredSegmentsCount;
        }
        /// <summary>
        /// Проверка соответствия типов динамических сегментов требуемым
        /// </summary>
        /// <param name="dynamicSegments">Массив динамических сегментов</param>
        /// <param name="types">Массив требуемых типов в правильном порядке</param>
        /// <returns></returns>
        public static bool CheckDynamicSegmentsTypes(DynamicSegment[] dynamicSegments, Type[] types)
        {
            if (CheckDynamicSegmentsCount(dynamicSegments, (types is null ? 0 : types.Length)))
            {
                for (int i = 0; i < dynamicSegments.Length; i++)
                {
                    if (dynamicSegments[i].Type != types[i])
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Возвращает полную строку с разделителями статических сегментов
        /// </summary>
        /// <param name="segments">Статические сегменты</param>
        /// <returns></returns>
        public static string GetFullSegmentString(IEnumerable<string> segments)
        {
            StringBuilder stringBuilder = new StringBuilder();
            return "/" + stringBuilder.AppendJoin('/', segments) + "/";
        }
        /// <summary>
        /// Возрвщает набор статических сегментов без разделителей в виде массива
        /// </summary>
        /// <param name="staticSegments">Строка - набор статических сегментов с разделителем / </param>
        /// <returns>Массив строк</returns>
        public static string[] GetSplittedStaticSegments(string staticSegments)
        {
            return staticSegments.Split('/', StringSplitOptions.RemoveEmptyEntries);
        }

        public override bool Equals(object? obj)
        {
            if (obj is RouteBase)
            {
                if ((obj as RouteBase)!.StaticSegments.SequenceEqual(StaticSegments) &&
                    (obj as RouteBase)!.DynamicSegments.SequenceEqual(DynamicSegments))
                    // Action в сравнении не участвует, сравнение только по статическим и динамическим сегментам
                    return true;
            }
            return false;
        }
    }
}
