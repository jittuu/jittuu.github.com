---
layout: post
title: "Unit of work pattern and ASP.NET MVC"
---

First of all, we need to understand what is [Unit of work][uow] pattern. Here is definition from [EAA catalog][uow]:

_Maintains a list of objects affected by a business transaction and coordinates the writing out of changes and the resolution of concurrency problems._

Basically unit of work is transaction management. Within one transaction, we may pull data from database, make some changes, add new record, etc.

So, what is unit of work in typical _Web_ application?

Web application is all about Request and Response. Clients, _e.g. browsers_, make request and then application do some business transaction at server and response the result. For most of the cases, the life time of one request is the life time of one transaction. **When we receive the request, we start unit of work. Just before we sent the response, we commit or rollback the unit of work.**

There are some important notes about unit of work and web application:

1. Unit of work should be **unique** for each request, i.e. unit of work cannot share between requests.
2. Unit of work should be share within **one** request, i.e. if you use unit of work from your controller and other places, that unit of work object should be the same if it is within one request.

[Dependency Injection][di] comes to rescue to make sure the above requirements are met. [Dependency Injection][di] provides _life time management_ and _resolving the dependencies_. For our case, unit of work object's **life time** must be **per request**.

_Explaining about dependency injection is out of scope of this <s>article</s>post. Of course, you can always download the attached [sample project][zip] to check how dependency injection works. ;)_

If we put together with Asp.net MVC, we get the following request and response cycle.

![png](//i.imgur.com/GkLJM.png)

Armed with this knowledge, lets start some coding.

First, unit of work which should able to commit and rollback.

{% highlight csharp %}
public interface IUnitOfWork : IDisposable {
int Commit();
}
{% endhighlight %}

Now, <code class="inline">IUnitOfWork</code> has ability to commit and rollback (dispose). In this post, I'll use EF code first as our [ORM][]. To implement [unit of work][uow] with EF, we need our <code class="inline">DbContext</code> implement <code class="inline">IUnitOfWork</code>.

{% highlight csharp %}
public class MyDbContext : DbContext, IUnitOfWork {
public DbSet<Project> Projects { get; set; }

public int Commit() {
return this.SaveChanges();
}
}
{% endhighlight %}

To integrate with ASP.net MVC executing pipeline, we should use [ActionFilterAttribute][actionfilter] which allow us to hook **before** and **after** action method executes.

{% highlight csharp %}
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class UnitOfWorkAttribute : ActionFilterAttribute {
// this property is injected by DI (before action executes)
// this post won't cover how this DI inject work.
// for more info, please download the attached sample project..
[Inject]
public IUnitOfWork UnitOfWork { get; set; }

// after action executed
public override void OnActionExecuted(ActionExecutedContext filterContext) {
base.OnActionExecuted(filterContext);

      if (filterContext.Exception == null) {
          this.UnitOfWork.Commit();
      }

}
}
{% endhighlight %}

After we created <code class="inline">UnitOfWorkAttribute</code>, we just need to apply it to <code class="inline">Controller</code> or <code class="inline">Action</code> where we want unit of work.

{% highlight csharp %}
[UnitOfWork]
public class ProjectController : Controller {
private MyDbContext db;

public ProjectController(IUnitOfWork db) {
this.db = (MyDbContext)db;
}

//...
//... other actions...

[HttpPost]
public ActionResult Create(Project project) {
if (ModelState.IsValid) {
db.Projects.Add(project);
return RedirectToAction("Index");
}

    return View(project);

}
}
{% endhighlight %}

Of course, you can also add **and** update the model, project. Those actions will be within one unit of work, i.e. transaction.

Finally, here is the [sample project][zip] for those who want to see the whole code-base.

[di]: //martinfowler.com/articles/injection.html
[uow]: //martinfowler.com/eaaCatalog/unitOfWork.html
[orm]: //en.wikipedia.org/wiki/Object-relational_mapping
[zip]: https://github.com/downloads/jittuu/jittuu.github.com/UnitOfWorkPattern.zip
[actionfilter]: //msdn.microsoft.com/en-us/library/system.web.mvc.actionfilterattribute.aspx
