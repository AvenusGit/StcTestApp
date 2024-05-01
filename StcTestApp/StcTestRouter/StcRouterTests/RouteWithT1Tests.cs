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
        public void AddRoute()
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
        }

        [TestMethod]
        public void AddRouteWithWrongParamsType()
        {
            Router router = new Router();
            var action = (int a) => {
                Console.WriteLine("Вызван делегат из теста добавления маршрута с одним неверным параметром в шаблоне");
            };

            Assert.ThrowsException<RouterException>(() => router.RegisterRoute<int>("/a/b/c/{a:bool}/", action));            ;
        }

        [TestMethod]
        public void AddRouteWithWrongParamsCount()
        {
            Router router = new Router();
            var action = (int a) => {
                Console.WriteLine("Вызван делегат из теста добавления маршрута с одним неверным параметром в шаблоне");
            };

            Assert.ThrowsException<RouterException>(() => router.RegisterRoute<int>("/a/b/c/{a:int}/{b:int}/", action)); ;
        }

        [TestMethod]
        public void AddSameRoutes()
        {
            Router router = new Router();
            var action = (int a) =>
            {
                Console.WriteLine("Вызван делегат из теста добавления одинаковых маршрутов с одним параметром");
            };

            router.RegisterRoute<int>("/a/b/c/{a:int}/", action);
            Assert.ThrowsException<RouterException>(() => router.RegisterRoute("/a/b/c/{a:int}/", action));
        }

        [TestMethod]
        public void CallRouteInt()
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
        public void CallRouteFloat()
        {
            Router router = new Router();
            float status = 0f;
            var action = (float newStatus) =>
            {
                status = newStatus;
                Console.WriteLine("Вызван делегат из теста вызова маршрута с одним параметром");
            };

            router.RegisterRoute<float>("/a/b/c/{newStatus:float}/", action);
            router.Route("/a/b/c/1,1/"); // тут важна запятая, т.к политика культуры и все такое

            Assert.IsTrue(status == 1.1f);
        }

        [TestMethod]
        public void CallRouteString()
        {
            Router router = new Router();
            string status = "old";
            var action = (string newStatus) =>
            {
                status = newStatus;
                Console.WriteLine("Вызван делегат из теста вызова маршрута с одним параметром");
            };

            router.RegisterRoute<string>("/a/b/c/{status:string}/", action);
            router.Route("/a/b/c/new/");

            Assert.IsTrue(status == "new");
        }

        [TestMethod]
        public void CallRouteDateTime()
        {
            Router router = new Router();
            DateTime date = DateTime.Now;
            var action = (DateTime date) =>
            {
                date = date.Subtract(new TimeSpan(1000,0,0,0,0,0));
                Console.WriteLine("Вызван делегат из теста вызова маршрута с одним параметром");
            };

            router.RegisterRoute<DateTime>("/a/b/c/{date:dateTime}/", action);
            router.Route("/a/b/c/2009-05-01 14:57:32.8/");

            Assert.IsTrue(date.Year == 2007);
        }

        [TestMethod]
        public void CallWrongArgRoute()
        {
            Router router = new Router();
            int status = 0;
            var action = (int newStatus) =>
            {
                status = newStatus;
                Console.WriteLine("Вызван делегат из теста вызова маршрута с одним параметром неверного типа");
            };

            router.RegisterRoute("/a/b/c/{newStatus:int}/", action);            

            Assert.ThrowsException<RouterException>(() => router.Route("/a/b/c/true/"));
        }

        //[TestMethod]
        //public void CallWrongRoute()
        //{
        //    Router router = new Router();
        //    var action = () => {
        //        Console.WriteLine("Вызван делегат из теста вызова отсутствующего маршрута без параметра");
        //    };
        //    router.RegisterRoute("/a/b/c/", action);
        //    Assert.ThrowsException<RouterException>(() => router.Route("/A/B/C/"));           
        //}

        //[TestMethod]
        //public void AddWrongTemplate()
        //{
        //    Router router = new Router();
        //    var action = () => {
        //        Console.WriteLine("Вызван делегат из теста добавления некорректного шаблона для маршрута без параметра");
        //    };

        //    Assert.ThrowsException<RouterException>(() => router.RegisterRoute("/a/b/c/{d:int}/", action));

        //}
    }
}