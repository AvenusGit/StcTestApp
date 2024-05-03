using StcTestRouter.Exceptions;
using StcTestRouter.Interfaces;
using StcTestRouter.Models.Routes;

namespace StcTestRouter.Models
{
    /// <summary>
    /// Реализация маршрутизатора
    /// </summary>
    public class Router : IRouter
    {
        /// <summary>
        /// Конструктор маршрутизатора с пустым списком маршрутов
        /// </summary>
        public Router() 
        {
            Routes = new Dictionary<string, List<RouteBase>>();
        }
        /// <summary>
        /// Конструктор маршрутизатора с готовым списком маршрутов
        /// </summary>
        /// <param name="routes">Словарь маршрутов</param>
        public Router(Dictionary<string, List<RouteBase>> routes)
        {
            Routes = routes;
        }
        /// <summary>
        /// Словарь маршрутов. Ключ - полный набор статических сегментов с разделителями. Значение - список маршрутов, с одинаковым и соответствующим ключу 
        /// набором статических сегментов, но разными наборами сегментов динамических
        /// </summary>
        public Dictionary<string, List<RouteBase>> Routes { get; set; }

        public void RegisterRoute(string template, Action action)
        {
            Route? route;
            if (Models.Routes.Route.TryParse(template, action, out route))
            {
                if (route!.DynamicSegments?.Count != 0)
                    throw new RouterWrongParamsCountExceptions();

                List<RouteBase>? routes;
                if (Routes.TryGetValue(route!.GetFullStaticSegments(), out routes))
                {
                    if (routes.Where(
                        existRoute => route.Equals(existRoute))
                        .Count() > 0)
                        throw new RouteExistException();
                    routes.Add(route);
                }
                else
                {
                    Routes.Add(route.GetFullStaticSegments(), new List<RouteBase>() { route });
                }
            }
            else
                throw new RouterParseException();
        }

        public void RegisterRoute<T>(string template, Action<T> action)
        {
            Route<T> newRoute;
            if (Route<T>.TryParse(template, action, out newRoute))
            {
                List<RouteBase>? routes;
                if (Routes.TryGetValue(newRoute!.GetFullStaticSegments(), out routes))
                {
                    if (routes.Where(existRoute => existRoute.Equals(newRoute))
                        .Count() > 0)
                        throw new RouteExistException();
                    routes.Add(newRoute);
                }
                else
                {
                    Routes.Add(newRoute.GetFullStaticSegments(), new List<RouteBase>() { newRoute });
                }
            }
            else
                throw new RouterParseException();
        }

        public void RegisterRoute<T1, T2>(string template, Action<T1, T2> action)
        {
            Route<T1,T2> newRoute;
            if (Route<T1,T2>.TryParse(template, action, out newRoute))
            {
                List<RouteBase>? routes;
                if (Routes.TryGetValue(newRoute!.GetFullStaticSegments(), out routes))
                {
                    if (routes.Where(existRoute => existRoute.Equals(newRoute))
                        .Count() > 0)
                        throw new RouteExistException();
                    routes.Add(newRoute);
                }
                else
                {
                    Routes.Add(newRoute.GetFullStaticSegments(), new List<RouteBase>() { newRoute });
                }
            }
            else
                throw new RouterParseException();
        }

        public void Route(string route)
        {            
            string[] segments = route.Split('/', StringSplitOptions.RemoveEmptyEntries);
            (RouteBase, object[])? searchResult = FindRoute(segments);
            if (searchResult.HasValue)
                searchResult.Value.Item1.CallAction(searchResult.Value.Item2);           
            else
                throw new RouterNotFoundExceptions();

        }
        /// <summary>
        /// Асинхронный вызов метода, соответствующего маршруту
        /// </summary>
        /// <param name="route">Строка - вызываемый маршрут</param>
        /// <param name="cancellationToken">Токен отмены асинхронной операции</param>
        /// <returns></returns>
        /// <exception cref="RouterNotFoundExceptions"></exception>
        public async Task RouteAsync(string route, CancellationToken cancellationToken)
        {
            string[] segments = route.Split('/', StringSplitOptions.RemoveEmptyEntries);
            (RouteBase, object[])? searchResult = FindRoute(segments);
            if (searchResult.HasValue)
                await searchResult.Value.Item1.CallActionAsync(cancellationToken, searchResult.Value.Item2);
            else
                throw new RouterNotFoundExceptions();
        }
        /// <summary>
        /// Поиск подходящего маршрута, рекурсивный метод
        /// </summary>
        /// <param name="segments">Массив сегментов маршрута</param>
        /// <param name="lastStaticSegmentLenght">Сколько сегментов считать статическими</param>
        /// <returns>Кортеж из найденного маршрута и готового набора параметров для вызова метода</returns>
        private (RouteBase, object[])? FindRoute(string[] segments, int lastStaticSegmentLenght = -1)
        {
            if (lastStaticSegmentLenght == 0) return null;
            if(lastStaticSegmentLenght == -1) lastStaticSegmentLenght = segments.Length;
            Span<string> staticSegments = segments.AsSpan<string>(0, lastStaticSegmentLenght);
            Span<string> dynamicSegments = segments.AsSpan<string>(lastStaticSegmentLenght, segments.Length - lastStaticSegmentLenght);

            List<RouteBase> targetRoutes = new List<RouteBase>();
            if(Routes.TryGetValue(RouteBase.GetFullSegmentString(staticSegments.ToArray()),out targetRoutes))
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
    }
}
