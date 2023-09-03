using Integration.EndToEnd.Fixtures;
using Xunit;

namespace Integration.EndToEnd
{
    public class End2End : IClassFixture<WebApplicationFactoryFixture>
    {
        private readonly WebApplicationFactoryFixture _factory;

        public End2End(WebApplicationFactoryFixture factory)
        {
            _factory = factory;
        }

        [Fact]
        public void End2EndTest()
        {

        }
    }
}