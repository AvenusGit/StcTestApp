using StcTestRouter.Exceptions;
using StcTestRouter.Interfaces;
using StcTestRouter.Models.Routes;
using StcTestRouter.Models.Trie;

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
            RouterTree = new Trie<Dictionary<RouteTypeParams,RouteBase>>();
        }
        /// <summary>
        /// Префиксное дерево маршрутов. Каждый узел имеет ключ в виде строки (не char!), значение подразумевает словарь маршрутов, где ключ - структура типов параметров маршрутов,
        /// значение - сам маршрут. Несколько маршрутов с одним и тем же набором статических сегментов и набором параметров (в том числе и по последовательности) быть не должно
        /// </summary>
        public Trie<Dictionary<RouteTypeParams, RouteBase>> RouterTree { get; set; }

        public void RegisterRoute(string template, Action action)
        {
            Route? route;
            if (Models.Routes.Route.TryParse(template, action, out route))
            {
                if (route!.DynamicSegments?.Count != 0)
                    throw new RouterWrongParamsCountExceptions();
                TrieNode<Dictionary<RouteTypeParams, RouteBase>>? node = RouterTree.GetNode(RouteBase.GetSplittedStaticSegments(route.GetFullStaticSegments()));
                if (node is null)
                {
                    if (!RouterTree.TryAdd(RouteBase.GetSplittedStaticSegments(route.GetFullStaticSegments()), new Dictionary<RouteTypeParams, RouteBase>() 
                        { { new RouteTypeParams(new Type[0]), route } }))
                        throw new RouterException("Не удалось добавить ноду, неизвестная ошибка");
                }
                else
                {
                    if (node.HasValue)
                    {
                        if (!node.Value!.TryAdd(route.GetRouteTypeParams(), route))
                            throw new RouteExistException();
                    }
                    else
                        node.Value = new Dictionary<RouteTypeParams, RouteBase>()
                        { { new RouteTypeParams(), route } };
                } 
            }
            else
                throw new RouterParseException();
        }

        public void RegisterRoute<T>(string template, Action<T> action)
        {
            Route<T>? route;
            if (Models.Routes.Route<T>.TryParse(template, action, out route))
            {
                if (route!.DynamicSegments?.Count != 1)
                    throw new RouterWrongParamsCountExceptions();
                TrieNode<Dictionary<RouteTypeParams, RouteBase>>? node = RouterTree.GetNode(RouteBase.GetSplittedStaticSegments(route.GetFullStaticSegments()));
                if (node is null)
                {
                    if (!RouterTree.TryAdd(RouteBase.GetSplittedStaticSegments(route.GetFullStaticSegments()), new Dictionary<RouteTypeParams, RouteBase>()
                        { { new RouteTypeParams(new Type[1] {typeof(T)}), route } }))
                        throw new RouterException("Не удалось добавить ноду, неизвестная ошибка");
                }
                else
                {
                    if (node.HasValue)
                    {
                        if (!node.Value!.TryAdd(route.GetRouteTypeParams(), route))
                            throw new RouteExistException();
                    }
                    else
                        node.Value = new Dictionary<RouteTypeParams, RouteBase>()
                        { { new RouteTypeParams(), route } };
                }
            }
            else
                throw new RouterParseException();
        }

        public void RegisterRoute<T1, T2>(string template, Action<T1, T2> action)
        {
            Route<T1, T2>? route;
            if (Models.Routes.Route<T1, T2>.TryParse(template, action, out route))
            {
                if (route!.DynamicSegments?.Count != 2)
                    throw new RouterWrongParamsCountExceptions();
                TrieNode <Dictionary<RouteTypeParams, RouteBase>>? node = RouterTree.GetNode(RouteBase.GetSplittedStaticSegments(route.GetFullStaticSegments()));
                if (node is null)
                {
                    if (!RouterTree.TryAdd(RouteBase.GetSplittedStaticSegments(route.GetFullStaticSegments()), new Dictionary<RouteTypeParams, RouteBase>()
                        { { new RouteTypeParams(new Type[2] {typeof(T1), typeof(T2)}), route } }))
                        throw new RouterException("Не удалось добавить ноду, неизвестная ошибка");
                }
                else
                {
                    if (node.HasValue)
                    {
                        if (!node.Value!.TryAdd(route.GetRouteTypeParams(), route))
                            throw new RouteExistException();
                    }
                    else
                        node.Value = new Dictionary<RouteTypeParams, RouteBase>()
                        { { new RouteTypeParams(), route } };
                }
            }
            else
                throw new RouterParseException();
        }
        /// <summary>
        /// Вызов маршрута
        /// </summary>
        /// <param name="route">Строка маршрута</param>
        /// <exception cref="RouterNotFoundExceptions">Исключение в случае отсутствия подходящего маршрута</exception>
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
        /// Поиск подходящего маршрута. Метод последовательно ищет подходящий маршрут, с каждой итерацией меняя статус последнего статического сегмента на динамический,
        /// т.к. в наборе сегментов нет возможности сразу понять какой сегмент статический (строка), а какой динамический (например типа string).
        /// 
        /// </summary>
        /// <param name="segments">Массив сегментов маршрута</param>
        /// <returns>Кортеж из найденного маршрута и готового набора параметров для вызова метода</returns>
        private (RouteBase, object[])? FindRoute(string[] segments)
        {
            for (int i = segments.Length; i > 0; i--)
            {
                Span<string> staticSegments = segments.AsSpan<string>(0, i);
                Span<string> dynamicSegments = segments.AsSpan<string>(i, segments.Length - i);

                Dictionary<RouteTypeParams, RouteBase>? routeList = RouterTree.GetValue(staticSegments.ToArray());
                if (routeList is not null)
                {
                    RouteBase? findedRoute;
                    if (routeList.TryGetValue(new RouteTypeParams(dynamicSegments.ToArray(), false), out findedRoute))
                    {
                        object[]? parameters = findedRoute.GetActionParameters(dynamicSegments.ToArray());
                        if (parameters is not null)
                            return (findedRoute, parameters);
                        else throw new RouterException("Ошибка маршрутизатора: маршрут был однозначно определен, но не удалось применить параметры");
                    }
                }               
            }
            return null;
        }
    }
}
