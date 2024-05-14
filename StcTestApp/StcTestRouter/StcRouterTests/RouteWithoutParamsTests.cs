using StcTestRouter.Interfaces;
using StcTestRouter.Exceptions;
using StcTestRouter.Models;
using StcTestRouter.Models.Routes;
using StcTestRouter.Models.Trie;

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
            
            router.RegisterRoute("/a/b/", action);            
            Assert.IsTrue(router.RouterTree.RootNodes.Count == 1);

            TrieNode<List<RouteBase>>? parentNode = router.RouterTree.RootNodes.Find(node => node.Key == "a" && node.HasValue == false);
            Assert.IsNotNull(parentNode);

            TrieNode<List<RouteBase>>? childNode = parentNode.Childrens.Find(node => node.Key == "b" && node.HasValue == true);
            Assert.IsNotNull(childNode);
            Assert.IsTrue(childNode.HasValue == true && childNode.Value is not null && childNode.Value.FirstOrDefault()?.GetFullStaticSegments() == "/a/b/");
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

        //[TestMethod]
        //public void CallRouteAsyncTest()
        //{
        //    Router router = new Router();
        //    int startThreadId = Thread.CurrentThread.ManagedThreadId;
        //    int actionThreadId = Thread.CurrentThread.ManagedThreadId;
        //    var action = () => {
        //        Console.WriteLine("Вызван делегат из теста добавления некорректного шаблона для маршрута без параметра");
        //        actionThreadId = Thread.CurrentThread.ManagedThreadId;
        //    };

        //    router.RegisterRoute("/a/b/c/", action);
        //    CancellationTokenSource tokenSourse = new CancellationTokenSource();
        //    CancellationToken token =  tokenSourse.Token;

        //    router.RouteAsync("/a/b/c/", token);

        //    Assert.IsTrue(startThreadId != actionThreadId); // тест иногда проваливается т.к. не всегда действие выполняет другой поток

        //}
    }
}