using EventStore.ClientAPI;
using EventStore.ClientAPI.Embedded;
using EventStore.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventStoreClient
{
    public sealed class EventStoreClient
    {
        private EventStoreClient() { }
        private static EventStoreClient instance = null;
        private static ClusterVNode _node;
        public static EventStoreClient Instance
        {
            get
            {
                if (instance == null)
                {
                    var nodeBuilder = EmbeddedVNodeBuilder.AsSingleNode()
                                  .OnDefaultEndpoints()
                                  .RunInMemory();

                    _node = nodeBuilder.Build();
                    _node.StartAndWaitUntilReady().Wait();

                    instance = new EventStoreClient();
                }
                return instance;
            }
        }

        public async Task Append(string stream, string type, byte[] events)
        {
            using (var cn = EmbeddedEventStoreConnection.Create(_node))
            {
                cn.ConnectAsync().Wait();
                await cn.AppendToStreamAsync(stream, ExpectedVersion.Any,
                                new EventData(Guid.NewGuid(), type, true,
                                events, null));

            }
        }

        public async Task<List<ResolvedEvent>> List(string stream)
        {
            var streamEvents = new List<ResolvedEvent>();

            StreamEventsSlice currentSlice;
            long nextSliceStart = StreamPosition.Start;
            using (var cn = EmbeddedEventStoreConnection.Create(_node))
            {
                do
                {
                    currentSlice = await cn.ReadStreamEventsForwardAsync(stream, nextSliceStart, 200, false);
                    nextSliceStart = currentSlice.NextEventNumber;
                    streamEvents.AddRange(currentSlice.Events);
                } while (!currentSlice.IsEndOfStream);
            }

            return streamEvents;
        }
    }
}
