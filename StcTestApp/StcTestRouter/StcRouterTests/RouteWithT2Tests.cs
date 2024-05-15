using StcTestRouter.Interfaces;
using StcTestRouter.Exceptions;
using StcTestRouter.Models;
using StcTestRouter.Models.Routes;
using StcTestRouter.Models.Trie;

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
                Console.WriteLine("������ ������� �� ����� ���������� �������� c ����� �����������");
            };

            router.RegisterRoute<int, float>("/a/b/{a:int}/{b:float}/", action);
            Assert.IsTrue(router.RouterTree.RootNodes.Count == 1);

            TrieNode<Dictionary<RouteTypeParams, RouteBase >>? parentNode = router.RouterTree.RootNodes.Find(node => node.Key == "a" && node.HasValue == false);
            Assert.IsNotNull(parentNode);

            TrieNode<Dictionary<RouteTypeParams, RouteBase>>? childNode = parentNode.Childrens.Find(node => node.Key == "b" && node.HasValue == true);
            Assert.IsNotNull(childNode);
            Assert.IsTrue(childNode.HasValue == true && childNode.Value is not null);


            RouteBase? route = childNode.Value.GetValueOrDefault(new RouteTypeParams(["{a:int}", "{b:float}"], true));
            Assert.IsNotNull(route);
            Assert.IsTrue(route.GetFullStaticSegments() == "/a/b/");
        }

        [TestMethod]
        public void AddRouteWithWrongParamsTypeTest()
        {
            Router router = new Router();
            var action = (bool a, bool b) => {
                Console.WriteLine("������ ������� �� ����� ���������� �������� � ����� �������� ���������� � �������");
            };

            Assert.ThrowsException<RouterParseException>(() => router.RegisterRoute<bool, bool>("/a/b/c/{a:bool}/{b:int}/", action));            ;
        }

        [TestMethod]
        public void AddRouteWithWrongParamsCountTest()
        {
            Router router = new Router();
            var action = (int a, int b) => {
                Console.WriteLine("������ ������� �� ����� ���������� �������� � ����� ������ ���������� � �������");
            };

            Assert.ThrowsException<RouterParseException>(() => router.RegisterRoute<int,int>("/a/b/c/{a:int}/{b:int}/{c:int}/", action)); ;
        }

        [TestMethod]
        public void AddSameRoutesTest()
        {
            Router router = new Router();
            var action = (int a, bool b) =>
            {
                Console.WriteLine("������ ������� �� ����� ���������� ���������� ��������� � ����� ����������");
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
                Console.WriteLine("������ ������� �� ����� ������ �������� � ����� �����������");
                firstParam = a;
                secondParam = b;
            };
            router.RegisterRoute<int, bool>("/a/b/c/{a:int}/{b:bool}/", action);

            router.Route("/a/b/c/4/true/");

            Assert.IsTrue(firstParam == 4);
            Assert.IsTrue(secondParam == true);

        }

        // ����� �� ������� ���������� ����� ������ � RouteWithT1Tests


        //[TestMethod]
        //public void CallRouteAsyncTest()
        //{
        //    Router router = new Router();
        //    int startThreadId = Thread.CurrentThread.ManagedThreadId;
        //    int actionThreadId = Thread.CurrentThread.ManagedThreadId;
        //    var action = (int a, int b) => {
        //        Console.WriteLine("������ ������� �� ����� ���������� ������������� ������� ��� �������� ��� ���������");
        //        actionThreadId = Thread.CurrentThread.ManagedThreadId;
        //    };

        //    router.RegisterRoute<int, int>("/a/b/c/{a:int}/{b:int}/", action);
        //    CancellationTokenSource tokenSourse = new CancellationTokenSource();
        //    CancellationToken token = tokenSourse.Token;

        //    router.RouteAsync("/a/b/c/2/4", token);

        //    Assert.IsTrue(startThreadId != actionThreadId); // ���� ������ ������������� �.�. �� ������ �������� ��������� ������ �����

        //}

        [TestMethod]
        public void ChangeOrderParamsTest()
        {
            Router router = new Router();
            int firstParam = 0;
            int secondParam = 0;
            var action = (int a, int b) => {
                firstParam = a;
                secondParam = b;
                Console.WriteLine("������ ������� �� ����� �������� ����� ������� ����������");
            };
            router.RegisterRoute<int, int>("/a/b/c/{b:int}/{a:int}/", action);

            router.Route("/a/b/c/1/2/");

            Assert.IsTrue(firstParam == 2);
            Assert.IsTrue(secondParam == 1);

        }
    }
}