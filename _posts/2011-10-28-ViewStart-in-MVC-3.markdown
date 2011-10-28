---
layout: post
title: Using ViewStart in ASP.NET MVC 3
---

When you create ASP.NET MVC 3 project, you may notice that there is one file named \_ViewStart.cshtml in the **Views** folder. 

## What does it do?

First and foremost, it is the **start** place of **executing pipelines** for views by [RazorViewEngine][] as its name suggested. The \_ViewStart.cshtml applied to all views in _current_ folder and _all subfolders_. That means, whenever **RazorViewEngine** renders the view ( _inside of Views folder_ ), it execute \_ViewStart.cshtml first and then renders the view.

Lets take a look at this example.

![ViewStart](http://i.imgur.com/QNtct.png)

There are two \_ViewStart.cshtml files, one is under _Views_ folder and another is under _Post_ folder.

When **RazorViewEngine** renders _Index_ view under _Home_ folder,
- the \_ViewStart.cshtml under _Views_ folder will be executed 
- Index.cshtml under _Home_ folder will be rendered.

When **RazorViewEngine** renders _Index_ view under _Post_ folder,
- the \_ViewStart.cshtml under _Views_ folder will be executed.
- the \_ViewStart.cshtml under _Post_ folder will be executed.
- Index.cshtml under _Post_ folder will be rendered.

*note: you can put breakpoint inside \_ViewStart.cshtml to check the order of execution*

## Why it exists?

\_ViewStart.cshtml is there to make easy (and [DRY][]) to apply the same logic to all the views under its folder and subfolders such as setting the Layout.

From previous example, the following code inside \_ViewStart.cshtml under _Views_ folder will set the "\_Layout.cshtml" as default layout to all the views.

{% highlight csharp %}
@{
    Layout = "~/Views/Shared/_Layout.cshtml";
}
{% endhighlight %}

And this code inside \_ViewStart.cshtml under _Post_ folder will **override** the layout as "\_PostLayout.cshtml" to all the views under _Post_ folder.

{% highlight csharp %}
@{
    Layout = "~/Views/Shared/_PostLayout.cshtml";
}
{% endhighlight %}

Of course, it is not limited to setting the layout, you can also apply other complex logic in the \_ViewStart.cshtml.

[RazorViewEngine]:http://msdn.microsoft.com/en-us/library/system.web.mvc.razorviewengine(v=vs.98).aspx
[DRY]:http://en.wikipedia.org/wiki/Don't_repeat_yourself
