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
        public async Task CreateComparerAsync_WhenCalledWithRedditThing_ReturnsRedditThingComparer()
        {
            IRedditFilterLogic sut = CreateSut();

            IRedditThingComparer<IRedditThing> result = await sut.CreateComparerAsync<IRedditThing>();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OSDevGrp.MyDashboard.Core.Logic.RedditThingComparer<IRedditThing>));
        }

        [TestMethod]
        public async Task CreateComparerAsync_WhenCalledWithRedditLink_ReturnsRedditThingComparer()
        {
            IRedditFilterLogic sut = CreateSut();

            IRedditThingComparer<IRedditLink> result = await sut.CreateComparerAsync<IRedditLink>();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OSDevGrp.MyDashboard.Core.Logic.RedditThingComparer<IRedditLink>));
        }

        private IRedditFilterLogic CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Logic.RedditFilterLogic();
        }
    }
}