using System;
using System.Threading;
using System.Threading.Tasks;
using Nancy.Routing;

namespace Nancy.RapidCache.Tests.Fakes
{
    public class FakeRoute : Route<object>
    {
        public bool ActionWasInvoked;
        public DynamicDictionary ParametersUsedToInvokeAction;

        public FakeRoute()
            : this(new Response())
        {
        }

        public FakeRoute(object response)
#pragma warning disable IDE1006 // Naming Styles
            : base("GET", "/", null, (_, __) => Task.FromResult<object>(null))
#pragma warning restore IDE1006 // Naming Styles
        {
            Action = (parameters, _) =>
            {
                ActionWasInvoked = true;
                ParametersUsedToInvokeAction = (DynamicDictionary) parameters;
                return Task.FromResult(response);
            };
        }

        public FakeRoute(Func<object, CancellationToken, Task<object>> action)
            : base("GET", "/", null, (_, __) => Task.FromResult<object>(null))
        {
            Action = (parameters, token) =>
            {
                ActionWasInvoked = true;
                ParametersUsedToInvokeAction = (DynamicDictionary) parameters;
                return action.Invoke(parameters, token);
            };
        }
    }
}
