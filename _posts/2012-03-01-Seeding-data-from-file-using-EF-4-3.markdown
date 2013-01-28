---
layout: post
title: "Seeding data from file using EF 4.3 Migration"
---

[Entity Framework 4.3 Migration][EF43] supports seeding data by overriding the Seed method of [DbMigrationsConfiguration][DbMigrationsConfiguration] class.

For my project, I need to execute some sql files to insert initial data. (I'm migrating from old system and new system have different table structure)

First, I try with this code

{% highlight csharp %}
internal sealed class Configuration : DbMigrationsConfiguration<DbContext>
{
  public Configuration()
  {
    AutomaticMigrationsEnabled = true;
  }

  protected override void Seed(DbContext context)
  {
    //  This method will be called after migrating to the latest version.

    var dirDatabaseScripts = Path.Combine(Directory.GetCurrentDirectory(), "DatabaseScripts");
    foreach (var filePath in Directory.EnumerateFiles(dirDatabaseScripts, "*.sql")) {
      context.Database.ExecuteSqlCommand(File.ReadAllText(filePath));
    }
  }
}
{% endhighlight %}

Boom! When I run it, I got the following errors.

<p class="removed">
Could not find a part of the path 'c:\Program Files\Microsoft Visual Studio 10.0\Common7\IDE\DatabaseScripts\'
</p>

The **Solution** is to use <code class="inline">AppDomain.CurrentDomain.BaseDirectory</code> instead of <code class="inline">Directory.GetCurrentDirectory()</code>.

Here is the working code

{% highlight csharp %}
var dirDatabaseScripts = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatabaseScripts");
foreach (var filePath in Directory.EnumerateFiles(dirDatabaseScripts, "*.sql")) {
  context.Database.ExecuteSqlCommand(File.ReadAllText(filePath));
}
{% endhighlight %}

I hope it can save you a few hours. :)

**update**: The issue was solved by [Michael Sync][sync]. Thanks Mike.

[EF43]:http://nuget.org/packages/EntityFramework/4.3.1
[DbMigrationsConfiguration]:http://msdn.microsoft.com/en-us/library/hh829093(v=vs.103).aspx
[sync]:http://michaelsync.net
