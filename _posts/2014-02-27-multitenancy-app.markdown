---
layout: post
title: "Multitenancy - App"
---

If all the tenants have the same system behavior, we can just use **webfarm** with one single web application instance. But we allow tenant to customize the _behavior_ of the system. It brings a lot of complexity.

# Physical

In multi-tenancy application, the system components will be looked like this:

![tenants][]

We have different combination of physical components (DLLs) for each tenant. That means we have to host each tenant as each web application in IIS, otherwise we have to load all the customize component (DLL) into the single application domain. One web application per tenant setting also has some trade-offs that we need to consider.

- **Isolation**: Since each tenant is one app instance, they won't effect each other.
- **Debugging**: Each tenant has its own worker process. It is much easier to debug single tenant context.
- **Resources monitoring and management**: Resource usage is separate from each tenant. It is much easier to monitor and manage tenant's resource usage.
- **Deployment**: It is the main drawback of this approach. It is more complex than single instance webfarm when we need to deploy. When we upgrade the **Core** system component, we have to deploy all the web application instances of all tenants.

# Extensibility

Basically, we need to have some extension point in the _core_ system. We can achieve that by using [strategy patterns][] and the likes. The most common mistake is trying to make very flexible system at early stage. My advice is try to **keep it simple** and focus on [Single Responsibility][sr] principle at first and apply _strategy pattern_ when needed. (_I am not saying that strategy pattern is the only way for extensibility. You may also want to use [MEF][]. So, it depends._)

[tenants]: https://raw.github.com/jittuu/jittuu.github.com/master/images/tenants.png
[strategy patterns]: //en.wikipedia.org/wiki/Strategy_pattern
[sr]: //en.wikipedia.org/wiki/Single_responsibility_principle
[mef]: //msdn.microsoft.com/en-us/library/dd460648(v=vs.110).aspx
