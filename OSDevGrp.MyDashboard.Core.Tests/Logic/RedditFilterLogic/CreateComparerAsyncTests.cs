using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;

namespace OSDevGrp.MyDashboard.Core.Tests.Logic.RedditFilterLogic
{
    [TestClass]
    public class CreateComparerAsyncTests
    {
        [TestMethod]
        public void CreateComparerAsync_WhenCalledWithRedditThing_ReturnsRedditThingComparer()
        {
            IRedditFilterLogic sut = CreateSut();

            Task<IRedditThingComparer<IRedditThing>> createComparerTask = sut.CreateComparerAsync<IRedditThing>();
            createComparerTask.Wait();

            IRedditThingComparer<IRedditThing> result = createComparerTask.Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OSDevGrp.MyDashboard.Core.Logic.RedditThingComparer<IRedditThing>));
        }

        [TestMethod]
        public void CreateComparerAsync_WhenCalledWithRedditLink_ReturnsRedditThingComparer()
        {
            IRedditFilterLogic sut = CreateSut();

            Task<IRedditThingComparer<IRedditLink>> createComparerTask = sut.CreateComparerAsync<IRedditLink>();
            createComparerTask.Wait();

            IRedditThingComparer<IRedditLink> result = createComparerTask.Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OSDevGrp.MyDashboard.Core.Logic.RedditThingComparer<IRedditLink>));
        }

        private IRedditFilterLogic CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Logic.RedditFilterLogic();
        }
    }
}