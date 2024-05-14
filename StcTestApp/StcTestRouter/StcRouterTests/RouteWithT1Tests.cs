using StcTestRouter.Interfaces;
using StcTestRouter.Exceptions;
using StcTestRouter.Models;
using StcTestRouter.Models.Routes;

namespace StcRouterTests
{
    [TestClass]
    public class RouterWithT1Tests
    {
        [TestMethod]
        public void AddRouteTest()
        {
            Router router = new Router();
           
            var action = (int a) => { 
                Console.WriteLine("Вызван делегат из теста добавления маршрута с одним параметром"); 
            };
            
            router.RegisterRoute<int>("/a/b/c/{a:int}/", action);

            List<RouteBase> list = new List<RouteBase>();
            Assert.IsTrue(router.Routes.TryGetValue("/a/b/c/", out list));
            Assert.IsTrue(list.Count == 1);
            Assert.IsTrue(list?.FirstOrDefault()?.DynamicSegments?.Count() == 1);
            Assert.IsTrue(list[0].DynamicSegments[0]?.Type == typeof(int));
        }

        [TestMethod]
        public void AddRouteWithWrongParamsTypeTest()
        {
            Router router = new Router();
            var action = (int a) => {
                Console.WriteLine("Вызван делегат из теста добавления маршрута с одним параметром при неверном типе параметра");
            };

            Assert.ThrowsException<RouterParseException>(() => router.RegisterRoute<int>("/a/b/c/{a:bool}/", action));            ;
        }

        [TestMethod]
        public void AddRouteWithWrongParamsCountTest()
        {
            Router router = new Router();
            var action = (int a) => {
                Console.WriteLine("Вызван делегат из теста добавления маршрута с одним параметром при неверном количестве параметров");
            };

            Assert.ThrowsException<RouterParseException>(() => router.RegisterRoute<int>("/a/b/c/{a:int}/{b:int}/", action)); ;
        }

        [TestMethod]
        public void AddSameRoutesTest()
        {
            Router router = new Router();
            var action = (int a) =>
            {
                Console.WriteLine("Вызван делегат из теста добавления одинаковых маршрутов с одним параметром");
            };

            router.RegisterRoute<int>("/a/b/c/{a:int}/", action);
            Assert.ThrowsException<RouteExistException>(() => router.RegisterRoute("/a/b/c/{a:int}/", action));
        }

        [TestMethod]
        public void CallRouteIntTest()
        {
            Router router = new Router();
            int status = 0;
            var action = (int newStatus) =>
            {
                status = newStatus;
                Console.WriteLine("Вызван делегат из теста вызова маршрута с одним параметром");
            };

            router.RegisterRoute<int>("/a/b/c/{newStatus:int}/", action);
            router.Route("/a/b/c/50/");

            Assert.IsTrue(status == 50);
        }

        [TestMethod]
        public void CallRouteFloatTest()
        {
            Router router = new Router();
            float status = 0f;
            var action = (float newStatus) =>
            {
                status = newStatus;
                Console.WriteLine("Вызван делегат из теста вызова маршрута с одним параметром типа float");
            };

            router.RegisterRoute<float>("/a/b/c/{newStatus:float}/", action);
            router.Route("/a/b/c/1,1/"); // тут важна запятая, т.к культура и все такое, культуру не обрабатывал

            Assert.IsTrue(status == 1.1f);
        }

        [TestMethod]
        public void CallRouteStringTest()
        {
            Router router = new Router();
            string status = "old";
            var action = (string newStatus) =>
            {
                status = newStatus;
                Console.WriteLine("Вызван делегат из теста вызова маршрута с одним параметром типа string");
            };

            router.RegisterRoute<string>("/a/b/c/{status:string}/", action);
            router.Route("/a/b/c/new/");

            Assert.IsTrue(status == "new");
        }

        [TestMethod]
        public void CallRouteDateTimeTest()
        {
            Router router = new Router();
            DateTime date = DateTime.Now;
            var action = (DateTime newDate) =>
            {
                date = newDate;
                Console.WriteLine("Вызван делегат из теста вызова маршрута с одним параметром типа DateTime");
            };

            router.RegisterRoute<DateTime>("/a/b/c/{date:dateTime}/", action);
            router.Route("/a/b/c/2009-05-01 14:57:32.8/");

            Assert.IsTrue(date.Year == 2009);
        }

        //[TestMethod]
        //public void CallRouteAsyncTest()
        //{
        //    Router router = new Router();
        //    int startThreadId = Thread.CurrentThread.ManagedThreadId;
        //    int actionThreadId = Thread.CurrentThread.ManagedThreadId;
        //    var action = (int a) => {
        //        Console.WriteLine("Вызван делегат из теста добавления некорректного шаблона для маршрута без параметра");
        //        actionThreadId = Thread.CurrentThread.ManagedThreadId;
        //    };

        //    router.RegisterRoute<int>("/a/b/c/{a:int}/", action);
        //    CancellationTokenSource tokenSourse = new CancellationTokenSource();
        //    CancellationToken token = tokenSourse.Token;

        //    router.RouteAsync("/a/b/c/2", token);

        //    Assert.IsTrue(startThreadId != actionThreadId); // тест иногда проваливается т.к. не всегда действие выполняет другой поток

        //}
    }
}