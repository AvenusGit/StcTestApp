using StcTestRouter.Models;
using StcTestRouter.Models.Routes;

namespace StcRouterTests
{
    [TestClass]
    public class OtherTests
    {
        [TestMethod]
        public void DynamicSegmentRegexTest()
        {
            Assert.IsTrue(DynamicSegment.IsDynamicSegmentTemplate("{a:int}"));
            Assert.IsTrue(DynamicSegment.IsDynamicSegmentTemplate("{b:string}"));
            Assert.IsTrue(DynamicSegment.IsDynamicSegmentTemplate("{c:float}"));
            Assert.IsTrue(DynamicSegment.IsDynamicSegmentTemplate("{d:dateTime}"));
            Assert.IsTrue(DynamicSegment.IsDynamicSegmentTemplate("{e:bool}"));

            Assert.IsFalse(DynamicSegment.IsDynamicSegmentTemplate("anyString"));
            Assert.IsFalse(DynamicSegment.IsDynamicSegmentTemplate("a:bool"));
            Assert.IsFalse(DynamicSegment.IsDynamicSegmentTemplate("{a:bool"));
            Assert.IsFalse(DynamicSegment.IsDynamicSegmentTemplate("a:bool}"));
            Assert.IsFalse(DynamicSegment.IsDynamicSegmentTemplate("{:bool}"));
            Assert.IsFalse(DynamicSegment.IsDynamicSegmentTemplate("{a bool}"));
            Assert.IsFalse(DynamicSegment.IsDynamicSegmentTemplate("{}"));
            Assert.IsFalse(DynamicSegment.IsDynamicSegmentTemplate("bool"));
            Assert.IsFalse(DynamicSegment.IsDynamicSegmentTemplate("{{a:bool}}"));
            Assert.IsFalse(DynamicSegment.IsDynamicSegmentTemplate("{a:boolean}"));
        }

        [TestMethod]
        public void RouteRegexTest()
        {
            Assert.IsTrue(RouteBase.IsRoutePatternMath("/a/b/c/"));
            Assert.IsTrue(RouteBase.IsRoutePatternMath("/a/"));
            Assert.IsTrue(RouteBase.IsRoutePatternMath("/veryLooooooooooooooooooooooooooooongString/"));
            Assert.IsTrue(RouteBase.IsRoutePatternMath("/abc/acb/bca/bac/"));

            Assert.IsFalse(RouteBase.IsRoutePatternMath("anyString"));
            Assert.IsFalse(RouteBase.IsRoutePatternMath("//"));
            Assert.IsFalse(RouteBase.IsRoutePatternMath("/"));
            Assert.IsFalse(RouteBase.IsRoutePatternMath("/ac"));
            Assert.IsFalse(RouteBase.IsRoutePatternMath("ac/"));
            Assert.IsFalse(RouteBase.IsRoutePatternMath("{a:boolean}"));
        }
    }
}