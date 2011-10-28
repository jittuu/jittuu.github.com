---
layout: post
title: Default Conventions in ASP.NET MVC 3
---

- Controller's name end with _Controller_. (e.g Home_Controller_)
- All _public_ methods of Controller are acts as action methods.
- If you return ViewResult without name, it will find the view named as action method.
- Views/Shared folder is used for shared views.
- By Default, ViewEngine find the view as Views/_[controllerName]_/_[actionName].cshtml_.
- Views conventions such as template, EditTemplates
