using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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


        public abstract object[]? GetActionParameters(string[] args); 

        public abstract void CallAction(params object[] parameters);

        public abstract bool CanCallAction(params object[] parameters);

        public static bool CheckDynamicSegmentsCount(DynamicSegment[] dynamicSegments, int requiredSegmentsCount)
        {
            return (dynamicSegments is null ? 0 : dynamicSegments.Length) == requiredSegmentsCount;
        }

        public static bool CheckDynamicSegmentsTypes(DynamicSegment[] dynamicSegments, Type[] types)
        {
            if(CheckDynamicSegmentsCount(dynamicSegments,(types is null ? 0 : types.Length)))
            {
                for(int i = 0; i < dynamicSegments.Length; i++)
                {
                    if (dynamicSegments[i].Type != types[i])
                        return false;
                }
                return true;
            }
            return false;
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
