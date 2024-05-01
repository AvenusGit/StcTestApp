using StcTestRouter.Exceptions;
using StcTestRouter.Interfaces;
using StcTestRouter.Models.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StcTestRouter.Models
{
    public class Router : IRouter
    {
        public Router() 
        {
            Routes = new Dictionary<string, List<RouteBase>>();
        }

        public Dictionary<string, List<RouteBase>> Routes { get; set; }

        public void RegisterRoute(string template, Action action)
        {
            Route? route;
            if (Models.Routes.Route.TryParse(template, action, out route))
            {
                List<RouteBase>? routes;
                if (Routes.TryGetValue(route!.GetFullStaticSegments(), out routes))
                {
                    if (routes.Where(route =>
                    route.DynamicSegments is null
                    || route.DynamicSegments?.Count() == 0)
                        .Count() > 0)
                        throw new RouterException("Ошибка регистрации маршрута: маршрут без параметров с таким набором статических сегментов уже существует");
                    routes.Add(route);
                }
                else
                {
                    Routes.Add(route.GetFullStaticSegments(), new List<RouteBase>() { route });
                }
            }
            else
                throw new RouterException("Ошибка регистрации маршрута: не удалось распознать строку маршрута или количество параметров не совпадает с действием");
        }

        public void RegisterRoute<T>(string template, Action<T> action)
        {
            Route<T> newRoute;
            if (Models.Routes.Route<T>.TryParse(template, action, out newRoute))
            {
                List<RouteBase>? routes;
                if (Routes.TryGetValue(newRoute!.GetFullStaticSegments(), out routes))
                {
                    if (routes.Where(route =>
                    route.DynamicSegments is not null
                    && route.DynamicSegments?.Count() == 1
                    && route.DynamicSegments?.FirstOrDefault()?.Type == newRoute.DynamicSegments?.FirstOrDefault()?.Type)
                        .Count() > 0)
                        throw new RouterException("Ошибка регистрации маршрута: маршрут без параметров с таким набором статических сегментов уже существует");
                    routes.Add(newRoute);
                }
                else
                {
                    Routes.Add(newRoute.GetFullStaticSegments(), new List<RouteBase>() { newRoute });
                }
            }
            else
                throw new RouterException("Ошибка регистрации маршрута: не удалось распознать строку маршрута или количество параметров или их тип не совпадает с действием");
        }

        public void RegisterRoute<T1, T2>(string template, Action<T1, T2> action)
        {
            throw new NotImplementedException();
        }

        public void Route(string route)
        {            
            string[] segments = route.Split('/', StringSplitOptions.RemoveEmptyEntries);
            (RouteBase, object[])? searchResult = FindRoute(segments);
            if (searchResult.HasValue)
                searchResult.Value.Item1.CallAction(searchResult.Value.Item2);           
            else
                throw new RouterException("Ошибка маршрутизатора: не удалось сопоставить маршрут");

        }

        private (RouteBase, object[])? FindRoute(string[] segments, int lastStaticSegmentLenght = -1)
        {
            if (lastStaticSegmentLenght == 0) return null;
            if(lastStaticSegmentLenght == -1) lastStaticSegmentLenght = segments.Length;
            Span<string> staticSegments = segments.AsSpan<string>(0, lastStaticSegmentLenght);
            Span<string> dynamicSegments = segments.AsSpan<string>(lastStaticSegmentLenght, segments.Length - lastStaticSegmentLenght);

            List<RouteBase> targetRoutes = new List<RouteBase>();
            if(Routes.TryGetValue(Router.GetFullSegmentString(staticSegments.ToArray()),out targetRoutes))
            {
                foreach (RouteBase route in targetRoutes)
                {
                    object[]? parameters = route.GetActionParameters(dynamicSegments.ToArray());
                    if (parameters is not null)
                        return (route, parameters);
                }                    
            }
            return FindRoute(segments, lastStaticSegmentLenght - 1);
        }

        /// <summary>
        /// Возвращает полную строку с разделителями статических сегментов
        /// </summary>
        /// <param name="segments">Статичествие сегменты</param>
        /// <returns></returns>
        private static string GetFullSegmentString(IEnumerable<string> segments)
        {
            StringBuilder stringBuilder = new StringBuilder();
            return "/" + stringBuilder.AppendJoin('/', segments) + "/";
        }
    }
}
