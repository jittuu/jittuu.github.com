---
layout: default
title: Custom 404 page in ASP.NET MVC 3 and IIS 7.0
---

## TL;DR
Add the following config under **system.webServer** in web.config. It will execute **NotFound** action of **ErrorsController** class when 404 error is raised. It is just executing URL instead of **redirecting**. _Only works in IIS 7+_

{% highlight xml %}
<httpErrors errorMode="Custom" existingResponse="Replace">
  <remove statusCode="404" />
  <error statusCode="404" responseMode="ExecuteURL" path="/Errors/NotFound" />
</httpErrors>
{% endhighlight %}

## More

For every live web application, we want to show custom friendly 404 Not Found error whenever use type invalid URLs or cannot find specific resource with provided data _(e.g. cannot find product with id 1, when user type products/1 in the address bar)_.

What can happen if user type invalid URLs in ASP.NET MVC 3? It may be: 

- matched routes with bad actions
- matched routes with bad controllers
- un-matched routes
- not-found resource (*we can just throws HttpException with 404 status code when we can't find*)

For all cases, ASP.Net MVC 3 already did a good job to inform IIS by throwing HttpException with 404 status code. There are two ways to tell IIS to use web application specific 404 Error page.

One is using **customErrors** tag in web.config.

{% highlight xml %}
<customErrors mode="RemoteOnly" defaultRedirect="/Errors/Trouble">
  <error statusCode="404" redirect="/Errors/NotFound" />
</customErrors>
{% endhighlight %}

In above configuration, the 404 error will **redirect** to _Errors/NotFound_ url while all other errors will **redirect** to _Errors/Trouble_ url. Please note that this is **redirect**. For me, it is not optimum solution. I would rather like to show custom 404 page **without losing** the URL.

If we are using IIS 7 or IIS Express, we can achieved by using **httpErrors** tag inside of **system.webServer** in web.config.

{% highlight xml %}
<httpErrors errorMode="Custom" existingResponse="Replace">
  <remove statusCode="404" />
  <error statusCode="404" responseMode="ExecuteURL" path="/Errors/NotFound" />
</httpErrors>
{% endhighlight %}

Now, IIS will show our custom 404 error page without **losing** URL.
