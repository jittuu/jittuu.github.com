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





