using System;

namespace Nancy.RapidCache.Tests.Mocks
{
    public class MockModule : NancyModule
    {
        public MockModule()
        {
            Get("/CachedResponse", _ =>
            {
                return Response
                    .AsText($"this is a cached response random number: {Guid.NewGuid()}");
            });
        }
    }
}
