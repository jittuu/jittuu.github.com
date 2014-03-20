---
layout: post
title: ASP.NET MVC Localization - Routing
---

When we build multi-language web application, we have to think:
  1. where should we store localization token, and
  2. the application flow for first-time users and revisiting-users.

I prefer to store localization token in the URL because it is SEO-friendly and cache-friendly. For examples, `http://www.microsoft.com/en-us/default.aspx`.

For the first time users - users without stored locale cookie:

  - when he/she navigate to home page (or any page without localization token), we need to redirect to default localized url. For example, we need to redirect from `http://www.example.com` to `http://www.example.com/en-us`. or from `http://www.example.com/posts` to `http://www.example.com/en-us/posts`. During the redirection, we also need to set the locale cookie for the next visit.
  - when he/she navigate to page _with_ localization token, we need serve the request with provided locale. In the response, we also need to set the locale cookie.

For revisiting users - users with stored locale cookie;

  - when he/she navigate to page _without_ localization token, we need to redirect to localized url based on the users' cookie.
  - when he/she navigate to page _with_ localization token, we have two scenarios. If the url token and the cookie value are the same, we just need to serve the request. If the url token and the cookie value are different, we take cookie value as higher priority (he/she might click url from somewhere). In that case, we need to redirect to url with localization token - which is from cookie.

# Implementation

> Enough talking, Show me the code!. 

First, I create a simple [HttpHandler][] to handle redirection.

{% highlight csharp %}
using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace MvcLocalization
{    
    class RedirectHandler : IHttpHandler
    {
        private string _newUrl;

        [SuppressMessage(category: "Microsoft.Design", checkId: "CA1054:UriParametersShouldNotBeStrings",
            Justification = "We just use string since HttpResponse.Redirect only accept as string parameter.")]
        public RedirectHandler(string newUrl)
        {
            this._newUrl = newUrl;
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Redirect(this._newUrl);
        }
    }
}
{% endhighlight %}

I create [RouteHandler][],a hook to Asp.Net routing, to handle redirection for _without localization token in url_  and a wrapper handler of [MvcRouteHandler][] to handle other cases. `LocalizedRouteHandler` update current thread's culture information before it delegate to `MvcRouteHandler`. I believe that the comment in the code should explain well itself.

{% highlight csharp %}
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcLocalization
{
    public class LocalizationRedirectRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var routeValues = requestContext.RouteData.Values;

            var cookieLocale = requestContext.HttpContext.Request.Cookies["locale"];
            if (cookieLocale != null)
            {
                    routeValues["culture"] = cookieLocale.Value;
                    return new RedirectHandler(new UrlHelper(requestContext).RouteUrl(routeValues));
            }

            var uiCulture = CultureInfo.CurrentUICulture;
            routeValues["culture"] = uiCulture.Name;
            return new RedirectHandler(new UrlHelper(requestContext).RouteUrl(routeValues));
        }
    }
}
{% endhighlight %}


{% highlight csharp %}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcLocalization
{
    public class LocalizedRouteHandler : MvcRouteHandler
    {
        protected override System.Web.IHttpHandler GetHttpHandler(System.Web.Routing.RequestContext requestContext)
        {
            var urlLocale = requestContext.RouteData.Values["culture"] as string;
            var cultureName = urlLocale ?? "";

            var cookieLocale = requestContext.HttpContext.Request.Cookies["locale"];
            if (cookieLocale != null)
            {
                // if request contains locale cookie, we need to put higher priority than url locale
                // user might click the link from somewhere but he/she already set different locale
                if (!cookieLocale.Value.Equals(urlLocale, StringComparison.OrdinalIgnoreCase))
                {
                    // if cookie locale and url cookie are different,
                    // we should redirect with cookie locale
                    var routeValues = requestContext.RouteData.Values;
                    routeValues["culture"] = cookieLocale.Value;
                    return new RedirectHandler(new UrlHelper(requestContext).RouteUrl(routeValues));
                }
                else
                {
                    cultureName = cookieLocale.Value;
                }
            }

            if (cultureName == "")
            {
                return GetDefaultLocaleRedirectHandler(requestContext);
            }

            try
            {
                var culture = CultureInfo.GetCultureInfo(cultureName);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            catch (CultureNotFoundException)
            {
                // if CultureInfo.GetCultureInfo throws exception
                // we should redirect with default locale
                return GetDefaultLocaleRedirectHandler(requestContext);
            }

            if (cookieLocale == null)
            {
                requestContext.HttpContext.Response.AppendCookie(new HttpCookie("locale", cultureName));
            }
            return base.GetHttpHandler(requestContext);
        }

        private static IHttpHandler GetDefaultLocaleRedirectHandler(RequestContext requestContext)
        {
            var uiCulture = CultureInfo.CurrentUICulture;
            var routeValues = requestContext.RouteData.Values;
            routeValues["culture"] = uiCulture.Name;
            return new RedirectHandler(new UrlHelper(requestContext).RouteUrl(routeValues));
        }
    }
}
{% endhighlight %}

Now, we need to hook that route handler to the routing. Just to be neat, I created extension method to [RouteCollection][] and added to the routes table when application starts.

{% highlight csharp %}
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcLocalization
{
    public static class RouteCollectionExtensions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings",
            Justification = "This is a URL template with special characters, not just a regular valid URL.")]
        public static Route MapRouteToLocalizeRedirect(this RouteCollection routes, string name, string url, object defaults)
        {
            var redirectRoute = new Route(url, new RouteValueDictionary(defaults), new LocalizationRedirectRouteHandler());
            routes.Add(name, redirectRoute);

            return redirectRoute;
        }

        public static Route MapLocalizeRoute(this RouteCollection routes, string name, string url, object defaults)
        {
            return routes.MapLocalizeRoute(name, url, defaults, new { });
        }

        public static Route MapLocalizeRoute(this RouteCollection routes, string name, string url, object defaults, object constraints)
        {            
            var route = new Route(
                url,
                new RouteValueDictionary(defaults),
                new RouteValueDictionary(constraints),
                new LocalizedRouteHandler());

            routes.Add(name, route);

            return route;
        }
    }
}
{% endhighlight %}

{% highlight csharp %}
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcLocalization.Sample
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapLocalizeRoute("Default",
                url: "{culture}/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { culture = "[a-zA-Z]{2}-[a-zA-Z]{2}" });

            routes.MapRouteToLocalizeRedirect("RedirectToLocalize",
                        url: "{controller}/{action}/{id}",
                        defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}
{% endhighlight %}

That's it! Now, you have localization-aware mvc application. Of course, I created [github repo](https://github.com/jittuu/MvcLocalization) for the whole source code and sample web application.

[HttpHandler]: http://msdn.microsoft.com/en-us/library/bb398986(v=vs.100).aspx#Background
[MvcRouteHandler]: http://msdn.microsoft.com/en-us/library/system.web.mvc.mvcroutehandler(v=vs.118).aspx
[RouteHandler]: http://msdn.microsoft.com/en-us/library/cc668201.aspx
[RouteCollection]: http://msdn.microsoft.com/en-us/library/system.web.routing.routecollection(v=vs.110).aspx
