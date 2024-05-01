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
    }
}
