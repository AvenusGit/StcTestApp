using StcTestRouter.Exceptions;
using StcTestRouter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StcTestRouter.Models.Routes
{
    /// <summary>
    /// Маршрут с одним параметром
    /// </summary>
    /// <typeparam name="T">Тип параметра маршрута</typeparam>
    public class Route<T> : RouteBase
    {
        public Route(List<string> staticSegments, DynamicSegment? dynamicSegment, Action<T> action)
        {
            if (dynamicSegment?.Type !=  typeof(T))
                throw new RouterException("Попытка создать маршрут с параметром не соответствующего типа");

            StaticSegments = staticSegments;            
            DynamicSegments = new List<DynamicSegment?>() { dynamicSegment };
            Action = action;
        }

        /// <summary>
        /// Действие, которое необходимо применить при вызове маршрута
        /// </summary>
        public Action<T> Action { get; private set; }

        /// <summary>
        /// Метод парсинга строки шаблона в маршрут
        /// </summary>
        /// <param name="template">Строка шаблона</param>
        /// <param name="action">Действие, которое необходимо вызвать при вызове маршрута</param>
        /// <param name="route">Выходная переменная, куда будет сохранен результат парсинга при успехе</param>
        /// <returns>True - если парсинг успешен, иначе false</returns>
        public static bool TryParse(string template, Action<T> action, out Route<T>? route)
        {
            if (IsRoutePatternMath(template))
            {
                // Проверка на наличие параметров в шаблоне
                string[] segments = template.Split('/', StringSplitOptions.RemoveEmptyEntries);
                List<string> staticSegments = new List<string>();
                List<DynamicSegment?> dynamicSegments = new List<DynamicSegment?>();

                foreach (string segment in segments)
                {
                    if (DynamicSegment.IsDynamicSegmentTemplate(segment))
                    {
                        DynamicSegment? newDynamicSegment;
                        if(DynamicSegment.TryParse(segment,out newDynamicSegment))
                            dynamicSegments.Add(newDynamicSegment);                        
                    }
                    else
                        staticSegments.Add(segment);
                }
                if(dynamicSegments.Count != 1 || dynamicSegments.FirstOrDefault()?.Type != typeof(T))
                {
                    route = null;
                    return false;
                }
                route = new Route<T>(staticSegments, dynamicSegments.FirstOrDefault(), action);
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
           if(args.Length != DynamicSegments.Count())
                return null;
           List<object> parameters = new List<object>();
           for (int i = 0; i < DynamicSegments.Count(); i++)
            {
                T newParam;
                if (DynamicSegment.TryConvertValue<T>(args[i], out newParam))
                {
                    parameters.Add(newParam);
                }
                else return null;
            }
           return parameters.ToArray();
        }

        public override bool CanCallAction(params object[] parameters)
        {
            if (parameters.Length == 1 && parameters[0].GetType() == DynamicSegments.FirstOrDefault()?.Type)
                return true;
            return false;
        }

        public override void CallAction(params object[] parameters)
        {
            Action.Invoke((T)parameters[0]);
        }
    }
}
