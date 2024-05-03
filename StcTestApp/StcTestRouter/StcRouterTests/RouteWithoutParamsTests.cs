using StcTestRouter.Interfaces;
using StcTestRouter.Exceptions;
using StcTestRouter.Models;
using StcTestRouter.Models.Routes;

namespace StcRouterTests
{
    [TestClass]
    public class RouterWithoutParamsTests
    {
        [TestMethod]
        public void AddRouteTest()
        {
            Router router = new Router();
           
            var action = () => { 
                Console.WriteLine("Вызван делегат из теста добавления маршрута без параметра"); 
            };
            
            router.RegisterRoute("/a/b/c/", action);

            List<RouteBase> list = new List<RouteBase>();
            Assert.IsTrue(router.Routes.TryGetValue("/a/b/c/", out list));
            Assert.IsTrue(list.Count == 1);
        }

        [TestMethod]
        public void AddSameRoutesTest()
        {
            Router router = new Router();
            var action = () => {
                Console.WriteLine("Вызван делегат из теста добавления одинаковых маршрутов без параметра");
            };

            router.RegisterRoute("/a/b/c/", action);

            Assert.ThrowsException<RouteExistException>(() => router.RegisterRoute("/a/b/c/", action));
        }

        [TestMethod]
        public void CallRouteTest()
        {
            Router router = new Router();
            bool status = false;
            var action = () => {
                status = true;
                Console.WriteLine("Вызван делегат из теста вызова маршрута без параметра");
            };

            router.RegisterRoute("/a/b/c/", action);
            router.Route("/a/b/c/");

            Assert.IsTrue(status);
        }

        [TestMethod]
        public void CallWrongRouteTest()
        {
            Router router = new Router();
            var action = () => {
                Console.WriteLine("Вызван делегат из теста вызова отсутствующего маршрута без параметра");
            };
            router.RegisterRoute("/a/b/c/", action);

            Assert.ThrowsException<RouterNotFoundExceptions>(() => router.Route("/A/B/C/"));           
        }

        [TestMethod]
        public void AddWrongTemplateTest()
        {
            Router router = new Router();
            var action = () => {
                Console.WriteLine("Вызван делегат из теста добавления некорректного шаблона для маршрута без параметра");
            };

            Assert.ThrowsException<RouterParseException>(() => router.RegisterRoute("/a/b/c/{d:int}/", action));
            
        }
    }
}