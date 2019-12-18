using EventStoreClient;
using NUnit.Framework;
using System.Text;
using System.Threading.Tasks;

namespace EventStoreExperiment.Tests
{
    public class ClientTests
    {
        [Test]
        public async Task TestWriteAndRead()
        {
            var client = EventStoreClient.EventStoreClient.Instance;

            var stream = "stream";
            var data = Encoding.UTF8.GetBytes("{\"Foo\":\"Bar\"}");

            await client.Append(stream, "test_event", data);
            var events = await client.List(stream);

            Assert.AreEqual(events[0].Event.Data, data);
        }
    }
}
