using StcTestRouter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StcTestRouter.Models.Routes
{
    /// <summary>
    /// Маршрут без параметров
    /// </summary>
    public class Route : RouteBase
    {
        public Route() { }
        /// <summary>
        /// Конструктор маршрута без параметров
        /// </summary>
        /// <param name="staticSegments">Список статических сегментов маршрута</param>
        /// <param name="action">Действие, которое необходимо вызвать при вызове маршрута</param>
        public Route(List<string> staticSegments, Action action)
        {
            StaticSegments = staticSegments;
            DynamicSegments = new List<DynamicSegment?>();
            Action = action;
        }

        /// <summary>
        /// Действие, которое необходимо применить при вызове маршрута
        /// </summary>
        public Action Action { get; private set; }

        /// <summary>
        /// Метод парсинга строки шаблона в маршрут
        /// </summary>
        /// <param name="template">Строка шаблона</param>
        /// <param name="action">Действие, которое необходимо вызвать при вызове маршрута</param>
        /// <param name="route">Выходная переменная, куда будет сохранен результат парсинга при успехе</param>
        /// <returns>True - если парсинг успешен, иначе false</returns>
        public static bool TryParse(string template, Action action, out Route? route)
        {
            if (IsRoutePatternMath(template))
            {
                // Проверка на наличие параметров в шаблоне
                string[] segments = template.Split('/', StringSplitOptions.RemoveEmptyEntries);
                List<string> staticSegments = new List<string>();
                foreach (string segment in segments)
                {
                    if (DynamicSegment.IsDynamicSegmentTemplate(segment))
                    {
                        route = null;
                        return false;
                    }
                    else
                        staticSegments.Add(segment);
                }
                route = new Route(staticSegments, action);
                return true;
            }
            else
            {
                route = null;
                return false;
            }
        }


        public override object[]? GetActionParameters(string[] args)
        {
            if(args.Length != 0)
                return null;
            return new object[0];
        }

        public override void CallAction(object[] parameters)
        {
            Action.Invoke();
        }

        public override async Task CallActionAsync(CancellationToken cancellationToken, object[] parameters)
        {
            await Task.Run(Action,cancellationToken);
        }

        public override bool CanCallAction(object[] parameters)
        {
            if(parameters.Length != 0)
                return false;
            return true;
        }

        public override Type[] GetDynamicSegmentsTypes()
        {
            return new Type[0];
        }
    }
}
