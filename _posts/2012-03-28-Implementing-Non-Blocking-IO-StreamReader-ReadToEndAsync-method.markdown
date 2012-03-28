---
layout: post
title: "Implementing non-blocking-IO method ReadToEndAsync for StreamReader"
---

I have posted about [implementation of non-blocking IO method, Stream.ReadAsync, as extension method][read_async]. But even with that method, the consumer still need to do a lot of things such as creating buffer, writing to buffer and reading from buffer, etc.

When we need to read the File, we mostly use [StreamReader.ReadToEnd][read_to_end] method. It will be great if we implement the non-blocking IO version of _ReadToEnd_ method as Task-based **ReadToEndAsync** method.

First, lets see the usage of _ReadToEndAsync_ method.
{% highlight csharp %}
using (var fs = File.Open(fileName, FileMode.Open))
using (var reader = new StreamReader(fs)) {
    var task = reader
                .ReadToEndAsync()
                .ContinueWith(t => {
                  var content = t.Result;
                  // do your work with file content
                });

    //
    // do other things while reading the file (non-blocking IO)
    //

    // wait until the file reading is complete
    task.Wait();
}
{% endhighlight %}

OK. It is pretty easy to use, right? :)

But the implementation is not that straight forward. Lets see the implementation
{% highlight csharp %}
public static class StreamReaderTaskParallelism {
  public static Task<string> ReadToEndAsync(this StreamReader reader) {
      var stream = reader.BaseStream;

      if (stream is MemoryStream) {
          return Task.Factory.StartNew<string>(() => reader.ReadToEnd());
      }

      var ts = new TaskCompletionSource<string>();

      var writer = new MemoryStream();
      var enumerator = EnumerateReadAsync(stream, writer).GetEnumerator();
      Action action = null;
      action = delegate {
          try {
              if (enumerator.MoveNext()) {
                  enumerator.Current.ContinueWith(delegate { action(); });
              }
              else {
                  using (var r = new StreamReader(writer)) {
                      writer.Position = 0;
                      var result = r.ReadToEnd();
                      ts.TrySetResult(result);
                  }
                  enumerator.Dispose();
              }
          }
          catch (Exception ex) {
              ts.TrySetException(ex);
          }
      };
      action();

      return ts.Task;
  }

  private static IEnumerable<Task<int>> EnumerateReadAsync(Stream stream, MemoryStream writer) {
      var buffer = new byte[4 * 1024];

      while (true) {
          var pendingRead = stream.ReadAsync(buffer, 0, buffer.Length);
          yield return pendingRead;

          int bytesRead = pendingRead.Result;
          if (bytesRead <= 0) break;

          writer.Write(buffer, 0, bytesRead);
      }
  }
}
{% endhighlight %}

First, we check whether it is [MemoryStream][]. If it is, we just use _ReadToEnd_ method since it is **not** IO-bound operation.

If it is _IO-bound_ operation, we should utilise non-blocking IO method, _ReadAsync_. The overview operation is

1. create a [TaskCompletionSource][] which will control the Task completion.
2. _scheduled_ to recursively read using non-blocking IO method, ReadAsync, by chaining with ContinueWith method until the end of stream.
3. return Task from TaskCompletionSource.
4. when reading IO completes, TaskCompletionSource provides the result for _returned Task_ and signal the Task is completed. Of course, if there is error, it will set Exception in the task.

{% highlight csharp %}
var ts = new TaskCompletionSource<string>();

var writer = new MemoryStream();
var enumerator = EnumerateReadAsync(stream, writer).GetEnumerator();
Action action = null;
action = delegate {
    try {
        if (enumerator.MoveNext()) {
            enumerator.Current.ContinueWith(delegate { action(); });
        }
        else {
            using (var r = new StreamReader(writer)) {
                writer.Position = 0;
                var result = r.ReadToEnd();
                ts.TrySetResult(result);
            }
            enumerator.Dispose();
        }
    }
    catch (Exception ex) {
        ts.TrySetException(ex);
    }
};
action();
{% endhighlight %}

The issue is we need to chain [ContinueWith][] method **recursively** until the end of file.

To achieve that, we need to retrieve an enumerator from the enumerable and uses that enumerator in a delegate.  The delegate moves the enumerator to its next element, and if there is a next element, retrieves it and uses [ContinueWith][] to schedule the delegate (recursively, in a sense) for execution when that current Task completes.  When the enumerator reaches the end, TaskCompletionSource will set result and enumerator is disposed. If there is any [Exception][] throws, it will set it in the Task. With that delegate created, it simply executes the delegate to get the execution started.

Now, we need to create EnumerateReadAsync method which is state machine ([in-dept article][state_machine] from [jon skeet][]) by using [yield][].

{% highlight csharp %}
private static IEnumerable<Task<int>> EnumerateReadAsync(Stream stream, MemoryStream writer) {
  var buffer = new byte[4 * 1024];

  while (true) {
      var pendingRead = stream.ReadAsync(buffer, 0, buffer.Length);
      yield return pendingRead;

      int bytesRead = pendingRead.Result;
      if (bytesRead <= 0) break;

      writer.Write(buffer, 0, bytesRead);
  }
}
{% endhighlight %}

Task&lt;int&gt; is **yield** from the EnumerateReadAsync method. When Task&lt;int&gt; completes, its continuation will be executed, which will cause the enumerator to MoveNext, thus ending up back in the EnumerateReadAsync method _just after the yield location_. When there is nothing to read from the file, MoveNext will return false, which causes the Task completion.

[read_async]:http://jittuu.com/2012/3/26/TPL-with-Extension-for-Tranditional-Net-Async-Programming/
[read_to_end]:http://msdn.microsoft.com/en-us/library/system.io.streamreader.readtoend.aspx
[MemoryStream]:http://msdn.microsoft.com/en-us/library/system.io.memorystream.aspx
[TaskCompletionSource]:http://msdn.microsoft.com/en-us/library/dd449174.aspx
[ContinueWith]:http://msdn.microsoft.com/en-us/library/system.threading.tasks.task.continuewith.aspx
[Exception]:http://msdn.microsoft.com/en-us/library/system.exception.aspx
[state_machine]:http://csharpindepth.com/articles/chapter6/iteratorblockimplementation.aspx
[jon skeet]:http://stackoverflow.com/users/22656/jon-skeet
[yield]:http://msdn.microsoft.com/en-us/library/9k7k7cf0.aspx
