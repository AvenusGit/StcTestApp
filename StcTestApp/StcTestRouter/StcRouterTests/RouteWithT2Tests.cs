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
                Console.WriteLine("������ ������� �� ����� ���������� �������� � ����� �����������"); 
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
        // ����� �� ������� ���������� ����� ������ � RouteWithT1Tests
    }
}