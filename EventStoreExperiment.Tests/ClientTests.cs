using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStoreExperiment.Tests
{
    public class ClientTests
    {
        private const string _stream = "test_stream";

        [SetUp]
        [TearDown]
        public async Task SetUpTearDown()
        {
            var client = EventStoreClient.EventStoreClient.Instance;
            await client.Delete(_stream, false);
        }

        [Test]
        public async Task TestWriteAndReadEnumerable()
        {
            var client = EventStoreClient.EventStoreClient.Instance;

            var data = Encoding.UTF8.GetBytes("{\"Foo\":\"Bar\"}");

            await client.Append(_stream, "test_event", data);
            await client.Append(_stream, "test_event", data);
            await client.Append(_stream, "test_event", data);
            var events = await client.ReadEnumerable(_stream).ToListAsync();

            events.Count.Should().Be(3);
        }
    }
}
