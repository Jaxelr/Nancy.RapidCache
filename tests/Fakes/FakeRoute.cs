using Nancy.Routing;
using System;
using System.Threading;
using System.Threading.Tasks;

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
            : base("GET", "/", null, (args, ct) => null)
        {
            Action = (parameters, token) =>
            {
                ActionWasInvoked = true;
                ParametersUsedToInvokeAction = (DynamicDictionary) parameters;
                return Task.FromResult(response);
            };
        }

        public FakeRoute(Func<object, CancellationToken, Task<object>> action)
            : base("GET", "/", null, (args, ct) => null)
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
