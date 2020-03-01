using Nancy.Routing;

namespace Nancy.RapidCache.Tests.Fakes
{
    public class FakeRouteResolver : IRouteResolver
    {
        public string Path { get; }

        public string ModulePath { get; }

        ResolveResult IRouteResolver.Resolve(NancyContext context)
        {
            return new ResolveResult
            {
                Route = new FakeRoute(),
                Parameters = new DynamicDictionary(),
                Before = null,
                After = null,
                OnError = null
            };
        }
    }
}
