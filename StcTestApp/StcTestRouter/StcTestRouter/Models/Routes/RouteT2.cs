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
    /// Маршрут с двумя параметрами
    /// </summary>
    /// <typeparam name="T1">Тип параметра маршрута 1</typeparam>
    /// <typeparam name="T2">Тип параметра маршрута 2</typeparam>
    public class Route<T1,T2> : RouteBase
    {
        public Route(List<string> staticSegments, DynamicSegment[] dynamicSegments, Action<T1,T2> action)
        {
            if(!RouteBase.CheckDynamicSegmentsCount(dynamicSegments,2))
                throw new RouterWrongParamsCountExceptions();
            if(!RouteBase.CheckDynamicSegmentsTypes(dynamicSegments, new Type[] {typeof(T1), typeof(T2)}))
                throw new RouterWrongParamsTypesExceptions();

            StaticSegments = staticSegments;            
            DynamicSegments = dynamicSegments.ToList()!;
            Action = action;
        }

        /// <summary>
        /// Действие, которое необходимо применить при вызове маршрута
        /// </summary>
        public Action<T1,T2> Action { get; private set; }

        /// <summary>
        /// Метод парсинга строки шаблона в маршрут
        /// </summary>
        /// <param name="template">Строка шаблона</param>
        /// <param name="action">Действие, которое необходимо вызвать при вызове маршрута</param>
        /// <param name="route">Выходная переменная, куда будет сохранен результат парсинга при успехе</param>
        /// <returns>True - если парсинг успешен, иначе false</returns>
        public static bool TryParse(string template, Action<T1,T2> action, out Route<T1,T2>? route)
        {
            if (IsRoutePatternMath(template))
            {
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
                if(!RouteBase.CheckDynamicSegmentsCount(dynamicSegments.ToArray()!,2) || 
                   !RouteBase.CheckDynamicSegmentsTypes(dynamicSegments.ToArray()!, new Type[] {typeof(T1), typeof(T2)}))
                {
                    route = null;
                    return false;
                }
                route = new Route<T1,T2>(staticSegments, dynamicSegments.ToArray()!, action);
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
            if (args is null || args.Length != DynamicSegments.Count()) return null;

            List<object> parameters = new List<object>();
            T1 firstParam;
            T2 secondParam;
            if (DynamicSegment.TryConvertValue<T1>(args[0], out firstParam))
                parameters.Add(firstParam!);
            else return null;
            if (DynamicSegment.TryConvertValue<T2>(args[1], out secondParam))
                parameters.Add(firstParam!);
            else return null;
            return parameters.ToArray();
        }

        public override bool CanCallAction(params object[] parameters)
        {
            if (parameters.Length == 2 && parameters[0].GetType() == typeof(T1) && parameters[1].GetType() == typeof(T2))
                return true;
            return false;
        }

        public override void CallAction(params object[] parameters)
        {
            Action.Invoke((T1)parameters[0], (T2)parameters[1]);
        }
    }
}
