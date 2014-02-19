---
layout: post
title: "Multitenancy"
---

[Multitenancy][] is defined in wiki as:

> Multitenancy refers to a principle in software architecture where a single instance of the software runs on a server, serving multiple client-organizations (tenants). Multitenancy contrasts with multi-instance architectures where separate software instances (or hardware systems) operate on behalf of different client organizations. With a multitenant architecture, a software application is designed to virtually partition its data and configuration, and each client organization works with a customized virtual application.

In the cloud computing, it is SaaS (Software as a Service) with some customizations. In this kind of application, the complexity is customization. If it only allows to customize skinning and allow to add some additional fields, it mostly works like normal single-tenant application. But if it allows to customize behavior of the applications, it is different story.

In this blog post, multi-tenant application means it allows tenant to change not only skinning but also _some behavior_ of the application.

Lets say, we are building multi-tenant E-commerce web-base application. At very high level, we have a few tenants with some customization to our core web E-commerce application and one tenants management application in our hosting.

Lets see what are the requirements for our multi-tenant application.

 - **Isolation** - each tenant should isolated from each other, including data and system uptime.
 - **Sacability** - our hosting should allow to scale _horizontally_ because we can't just host all the tenants in one server.
 - **Extensibility** - each tenant should allow to customize _some_ behavior of the core system.
 - **Release Management** - we should able to release updates/hot-fixes to each tenant without affecting other tenants. But if we do updates/hot-fixes to the **Core** system, we should be able to release all the tenants as well.
 - **Single source code** - there should be only one source code for the **Core** system. It requires to mention specifically because I want to highlight that we are not building _copy-paste-customize_ system. (sound familiar?)
 - **Simplicity** - Actually, it applies to all the systems. When building customizable system, the simplicity is much more important because people tend to build the system which allows to customize _dynamically_. Dynamic will lead to a lot of complexity.

 In the next few posts, I'll talk about how are we going to build multi-tenant application.

 [Multitenancy]: http://en.wikipedia.org/wiki/Multitenancy
