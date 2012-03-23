---
layout: post
title: "Multi-threading and Console.WriteLine"
---

I have seen many of the following pattern in multi-threading programs

{% highlight csharp %}
Console.WriteLine("Doing Something!");
// Do things in parallel..
{% endhighlight %}

Many people use <code class="inline">Console.WriteLine</code> as logging in multi-threading programs. But actually, it will make things **slow**. [Console][] I/O Streams are _synchronized_, i.e. it is **blocking I/O operation**. Whenever multiple threads use <code class="inline">Console.WriteLine</code>, only one thread can do I/O operation and others need to wait.

Lets do some performance benchmarking test with this code

{% highlight csharp %}
class Program {
    static void Main(string[] args) {
        var stopwatch = new Stopwatch();

        Console.WriteLine("");
        stopwatch.Start();

        Parallel.ForEach(Enumerable.Range(1, 10000), num => {
            // simulate some CPU operations
            var total = 0;
            for (int i = 0; i < 1000000; i++) {
                total += i;
            }

            // comment this for without Console.WriteLine operation
            Console.Write("....");
        });

        stopwatch.Stop();
        Console.WriteLine();
        Console.WriteLine("With Console.WriteLine: {0:c}", stopwatch.Elapsed);
        // Console.WriteLine("Without Console.WriteLine: {0:c}", stopwatch.Elapsed);
    }
}
{% endhighlight %}

Here is the result I have got in **My Machine**.

![with Console.WriteLine](http://i.imgur.com/m8aM1.png)
![without Console.WriteLine](http://i.imgur.com/edYA4.png)

Even for this small test, there is 500ms different.

IMO, I prefer to use some Asynchronous logging rather than <code class="inline">Console.WriteLine</code>.

_Note:_ I'm _not saying_ that you should never use Console.WriteLine in multi-threading executing. I'm trying to say that you should **aware that it is I/O blocking operations.**

[Console]:http://msdn.microsoft.com/en-us/library/system.console.aspx
