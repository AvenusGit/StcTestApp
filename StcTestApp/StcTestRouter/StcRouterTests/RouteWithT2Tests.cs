using StcTestRouter.Interfaces;
using StcTestRouter.Exceptions;
using StcTestRouter.Models;
using StcTestRouter.Models.Routes;

namespace StcRouterTests
{
    [TestClass]
    public class RouterWithT2Tests
    {
        [TestMethod]
        public void AddRouteTest()
        {
            Router router = new Router();
           
            var action = (int a, float b) => { 
                Console.WriteLine("Вызван делегат из теста добавления маршрута с двумя параметрами"); 
            };
            
            router.RegisterRoute<int,float>("/a/b/c/{a:int}/{b:float}/", action);

            List<RouteBase> list = new List<RouteBase>();
            Assert.IsTrue(router.Routes.TryGetValue("/a/b/c/", out list));
            Assert.IsTrue(list.Count == 1);
            Assert.IsTrue(list?.FirstOrDefault()?.DynamicSegments?.Count() == 2);
            Assert.IsTrue(list[0].DynamicSegments[0]?.Type == typeof(int));
            Assert.IsTrue(list[0].DynamicSegments[1]?.Type == typeof(float));
        }

        [TestMethod]
        public void AddRouteWithWrongParamsTypeTest()
        {
            Router router = new Router();
            var action = (bool a, bool b) => {
                Console.WriteLine("Вызван делегат из теста добавления маршрута с одним неверным параметром в шаблоне");
            };

            Assert.ThrowsException<RouterParseException>(() => router.RegisterRoute<bool, bool>("/a/b/c/{a:bool}/{b:int}/", action));            ;
        }

        [TestMethod]
        public void AddRouteWithWrongParamsCountTest()
        {
            Router router = new Router();
            var action = (int a, int b) => {
                Console.WriteLine("Вызван делегат из теста добавления маршрута с одним лишним параметром в шаблоне");
            };

            Assert.ThrowsException<RouterParseException>(() => router.RegisterRoute<int,int>("/a/b/c/{a:int}/{b:int}/{c:int}/", action)); ;
        }

        [TestMethod]
        public void AddSameRoutesTest()
        {
            Router router = new Router();
            var action = (int a, bool b) =>
            {
                Console.WriteLine("Вызван делегат из теста добавления одинаковых маршрутов с одним параметром");
            };

            router.RegisterRoute<int,bool>("/a/b/c/{a:int}/{b:bool}/", action);
            Assert.ThrowsException<RouteExistException>(() => router.RegisterRoute("/a/b/c/{a:int}/{b:bool}/", action));
        }

        [TestMethod]
        public void CallRouteTest()
        {
            Router router = new Router();
            int firstParam = 0;
            bool secondParam = false;
            var action = (int a, bool b) => {
                Console.WriteLine("Вызван делегат из теста вызова маршрута с двумя параметрами");
                firstParam = a;
                secondParam = b;
            };
            router.RegisterRoute<int, bool>("/a/b/c/{a:int}/{b:bool}/", action);

            router.Route("/a/b/c/4/true/");

            Assert.IsTrue(firstParam == 4);
            Assert.IsTrue(secondParam == true);

        }

        // Тесты на парсинг конкретных типов смотри в RouteWithT1Tests


        [TestMethod]
        public void CallRouteAsyncTest()
        {
            Router router = new Router();
            int startThreadId = Thread.CurrentThread.ManagedThreadId;
            int actionThreadId = Thread.CurrentThread.ManagedThreadId;
            var action = (int a, int b) => {
                Console.WriteLine("Вызван делегат из теста добавления некорректного шаблона для маршрута без параметра");
                actionThreadId = Thread.CurrentThread.ManagedThreadId;
            };

            router.RegisterRoute<int, int>("/a/b/c/{a:int}/{b:int}/", action);
            CancellationTokenSource tokenSourse = new CancellationTokenSource();
            CancellationToken token = tokenSourse.Token;

            router.RouteAsync("/a/b/c/2/4", token);

            Assert.IsTrue(startThreadId != actionThreadId); // тест иногда проваливается т.к. не всегда действие выполняет другой поток

        }

        [TestMethod]
        public void ChangeOrderParamsTest()
        {
            Router router = new Router();
            int firstParam = 0;
            int secondParam = 0;
            var action = (int a, int b) => {
                firstParam = a;
                secondParam = b;
                Console.WriteLine("Вызван делегат из теста проверки смена порядка параметров");
            };
            router.RegisterRoute<int, int>("/a/b/c/{b:int}/{a:int}/", action);

            router.Route("/a/b/c/1/2/");

            Assert.IsTrue(firstParam == 2);
            Assert.IsTrue(secondParam == 1);

        }
    }
}