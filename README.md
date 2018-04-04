Nancy.RapidCache
====================

Cache content asynchronously inside NancyFx. Allows you to set a timespan of lifecycle for your cache. Also allows to redefine the desired Cache backend and desires key request. By default it provides a default memory cache and a default uri key.

## Builds

| Appveyor  |
| :---:     |
| [![Build status](https://ci.appveyor.com/api/projects/status/3cfeq9e3lh4edbcg?svg=true)](https://ci.appveyor.com/project/Jaxelr/nancy-rapidcache) |

## Packages

Package | NuGet (Stable) | MyGet (Prerelease)
| :--- | :---: | :---: |
| Nancy.RapidCache | [![NuGet](https://img.shields.io/nuget/v/Nancy.RapidCache.svg)](https://www.nuget.org/packages/Nancy.RapidCache) | [![MyGet](https://img.shields.io/myget/nancy-rapid-cache/v/Nancy.RapidCache.svg)](https://www.myget.org/feed/nancy-rapid-cache/package/nuget/Nancy.RapidCache) |
| Nancy.RapidCache.Redis | [![NuGet](https://img.shields.io/nuget/v/Nancy.RapidCache.Redis.svg)](https://www.nuget.org/packages/Nancy.RapidCache.Redis) | [![MyGet](https://img.shields.io/myget/nancy-rapid-cache/v/Nancy.RapidCache.Redis.svg)](https://www.myget.org/feed/nancy-rapid-cache/package/nuget/Nancy.RapidCache.Redis) |

## Installation

Install via nuget https://nuget.org/packages/Nancy.RapidCache

```
PM> Install-Package Nancy.RapidCache
```

## Example usage 

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

2. Enable caching by adding the "AsCacheable" extension method to any of your routes

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

 *Keep in mind that for *Post* methods, the requests the body is __NOT__ keyed in this scenario. 
 In context, you can filter by the values of: 
 
 * Query
 * Form values
 * Accept Headers

 along with the url that identifies your resource (this is using the DefaultKeyGenerator). 

## Using other Cache Stores

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

```
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

## Definining your own cache key generation using ICacheKeyGenerator

Define your own key per request that will help you cache to the level of granulatity as needed.

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
               using(var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
               {
                   var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(request.Url.ToString()));
                   return Convert.ToBase64String(hash);
               }
            }
        }
    }
}
```
