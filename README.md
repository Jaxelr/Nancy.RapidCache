# Nancy.RapidCache [![Mit License][mit-img]][mit]

Cache content asynchronously inside NancyFx. Allows you to set a timespan of lifecycle for your cache. Also allows to redefine the desired Cache backend and desires key request. By default it provides a default memory cache and a default uri key.

## Builds

| Appveyor  |
| :---:     |
| [![Build status][build-img]][build] |

## Packages

Package | NuGet (Stable) | MyGet (Prerelease)
| :--- | :---: | :---: |
| Nancy.RapidCache | [![NuGet][nuget-rapid-img]][nuget-rapid] | [![MyGet][myget-rapid-img]][myget-rapid] |
| Nancy.RapidCache.Redis | [![NuGet][nuget-redis-img]][nuget-redis] | [![MyGet][myget-redis-img]][myget-redis] |
| Nancy.RapidCache.Couchbase | [![NuGet][nuget-couchbase-img]][nuget-couchbase] | [![MyGet][myget-couchbase-img]][myget-couchbase] |

## Installation

Install via nuget https://nuget.org/packages/Nancy.RapidCache

```powershell
PM> Install-Package Nancy.RapidCache
```

## Sample usage

The following example is using the default "In-Memory" CacheStore which is nothing more than a concurrent dictionary.

1. Add to your Nancy bootstrapper

```c#
using Nancy.RapidCache.Extensions;
using Nancy.Routing;

namespace WebApplication
{
    public class ApplicationBootrapper : Nancy.DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            /* Enable rapidcache, vary by url, query, form and accept header */
            this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new[] { "query", "form", "accept" });
        }
    }
}
```

1. Enable caching by adding the "AsCacheable" extension method to any of your routes

```c#
using System;
using Nancy;
using Nancy.RapidCache.Extensions;

namespace WebApplication
{
    public class ExampleModule : NancyModule
    {
        public ExampleModule()
        {
            Get("/", _ =>
            {
                /* cache view for 30 secs */
                return View["hello.html"].AsCacheable(DateTime.Now.AddSeconds(30));
            });

            Get("/CachedResponse", _ =>
            {
                /* cache response for 30 secs */
                return Response.AsText("hello").AsCacheable(DateTime.Now.AddSeconds(30));
            });
        }
    }
}
```

Or alternatively, set the Cache on all requests by using an after request on the start of the pipeline.

```c#
using Nancy.RapidCache.Extensions;
using Nancy.Routing;

namespace WebApplication
{
    public class ApplicationBootrapper : Nancy.DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            /* Enable RapidCache, vary by url params query, form and accept headers */
            this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new[] { "query", "form", "accept" });
            pipelines.AfterRequest.AddItemToStartOfPipeline(ConfigureCache);
        }

        public void ConfigureCache(NancyContext context)
        {
            /* Cache all responses by 30 seconds with status code OK */
            context.Response = context.Response.AsCacheable(DateTime.Now.AddSeconds(30));
        }
    }
}
```

 *Keep in mind that for *Post* methods, the requests body is __NOT__ part of the key in this scenario. In context, you can filter by the values of:

* Query
* Form values
* Accept Headers

 along with the url that identifies your resource (this is using the DefaultKeyGenerator). 

## Using Different Cache Stores

### DiskCacheStore

When using Nancy in self hosting mode, you can use the DiskCacheStore to enable caching throught RapidCache to a tmp file.

```c#
using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Extensions;
using Nancy.Routing;

namespace WebApplication
{
    public class ApplicationBootrapper : Nancy.DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            /* Enable RapidCache using the DiskCacheStore, vary by url params id,query,takem, skip and accept header */
            this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new[] { "query", "form", "accept" }, new DiskCacheStore("c:/tmp/cache"));
        }
    }
}
```

### RedisCacheStore

RapidCache provides a small lib for integration with Redis, given that you provide a valid connection. It is provided as a separate package, so install it first via nuget:

``` powershell
PM> Install-Package Nancy.RapidCache.Redis
```

the usage is similar to the other stores:

```c#
using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Extensions;
using Nancy.Routing;

namespace WebApplication
{
    public class ApplicationBootrapper : Nancy.DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            /* Enable RapidCache using the RedisCacheStore, vary by url params id,query,takem, skip and accept header */
            this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new[] { "query", "form", "accept" }, new RedisCacheStore("localhost"));
        }
    }
}
```

### CouchbaseCacheStore

RapidCache also provides a small lib for a integration with Couchbase, given that you provide a valid Cluster object from the CouchbaseNetClient package and a bucket name that can be located. Install it via nuget:

``` powershell
PM> Install-Package Nancy.RapidCache.Couchbase
```

Once installed, we need to configure the cluster first, and then pass the cluster to the constructor:

```c#
using Nancy.RapidCache.CacheStore;
using Nancy.RapidCache.Extensions;
using Nancy.Routing;

namespace WebApplication
{
    public class ApplicationBootrapper : Nancy.DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri("http://127.0.0.1") }
            });

            var authenticator = new PasswordAuthenticator("user", "password");
            cluster.Authenticate(authenticator);

            /* Enable RapidCache using the CouchbaseCacheStore, vary by url params id,query,takem, skip and accept header */
            /* Provide a bucket on the configuration */
            this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new[] { "query", "form", "accept" }, new CouchbaseCacheStore(cluster, "myBucket")));
        }
    }
}
```

## Definining your own cache key generation using ICacheKeyGenerator

Define your own key per resolver that will help you cache to the level of granulatity you need. 

```c#
using System;
using System.Text;
using Nancy;
using Nancy.RapidCache.Extensions;
using Nancy.Routing;

namespace WebApplication
{
    public class ApplicationBootrapper : Nancy.DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            this.EnableRapidCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new UrlHashKeyGenerator());
        }

        public class UrlHashKeyGenerator : Nancy.RapidCache.CacheKey.ICacheKeyGenerator
        {
            public string Get(Request request)
            {
                using (var md5 = MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(request.Url.ToString()));
                    return Convert.ToBase64String(hash);
                }
            }
        }
    }
}
```

For more details please check the [documentation][wiki].

[mit-img]: http://img.shields.io/badge/License-MIT-blue.svg
[mit]: https://github.com/Jaxelr/Nancy.RapidCache/blob/master/LICENSE
[build-img]: https://ci.appveyor.com/api/projects/status/3cfeq9e3lh4edbcg/branch/master?svg=true
[build]: https://ci.appveyor.com/project/Jaxelr/nancy-rapidcache/branch/master
[nuget-rapid-img]: https://img.shields.io/nuget/v/Nancy.RapidCache.svg
[nuget-rapid]: https://www.nuget.org/packages/Nancy.RapidCache
[myget-rapid-img]: https://img.shields.io/myget/nancy-rapid-cache/v/Nancy.RapidCache.svg
[myget-rapid]: https://www.myget.org/feed/nancy-rapid-cache/package/nuget/Nancy.RapidCache
[nuget-redis-img]: https://img.shields.io/nuget/v/Nancy.RapidCache.Redis.svg
[nuget-redis]: https://www.nuget.org/packages/Nancy.RapidCache.Redis
[myget-redis-img]: https://img.shields.io/myget/nancy-rapid-cache/v/Nancy.RapidCache.Redis.svg
[myget-redis]: https://www.myget.org/feed/nancy-rapid-cache/package/nuget/Nancy.RapidCache.Redis
[nuget-couchbase-img]: https://img.shields.io/nuget/v/Nancy.RapidCache.Couchbase.svg
[nuget-couchbase]: https://www.nuget.org/packages/Nancy.RapidCache.Couchbase
[myget-couchbase-img]: https://img.shields.io/myget/nancy-rapid-cache/v/Nancy.RapidCache.Couchbase.svg
[myget-couchbase]: https://www.myget.org/feed/nancy-rapid-cache/package/nuget/Nancy.RapidCache.Couchbase
[wiki]: https://github.com/Jaxelr/Nancy.RapidCache/wiki
