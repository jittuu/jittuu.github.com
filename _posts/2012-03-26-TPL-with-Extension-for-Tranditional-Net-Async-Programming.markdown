---
layout: post
title: "TPL with Extension method and Traditional .Net Asynchronous Programming"
---

[TPL][] provides [a way to work][tpl_amp] with Tasks and Traditional .Net Asynchronous Programming Model (APM). I don't want to explain how to wrap APM operation in this post, but you can read [MSDN article][tpl_amp] about wrapping APM operations in a Task in details.

In this post, I want to show how we can provide additional Task-based API method in existing .Net library by using [extension methods][ext_method] and TPL.

Lets use [Stream.BeginRead][s_begin_read] and [Stream.EndRead][s_end_read] as example for wrapping APM operation.


{% highlight csharp %}
public static class StreamTaskParallelism {
  public static Task<int> ReadAsync(this Stream stream, byte[] buffer, int offset, int count, object state) {
    return Task.Factory.FromAsync<byte[], int, int, int>(stream.BeginRead, stream.EndRead, buffer, offset, count, state);
  }
}
{% endhighlight %}

By using that extension method, you can write Task-based code like
{% highlight csharp %}
const int MAX_FILE_SIZE = 14000000;
public Task<string> GetFileStringAsync(string path) {
  FileInfo fi = new FileInfo(path);
  byte[] data = null;
  data = new byte[fi.Length];

  FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, data.Length, true);

  var task = fs.ReadAsync(data, 0, data.Length, null);

  // It is possible to do other work here while waiting
  // for the antecedent task to complete.
  // ...

  // Add the continuation, which returns a Task<string>. 
  return task.ContinueWith((antecedent) => {
      fs.Close();

      // Result = "number of bytes read" (if we need it.)
      if (antecedent.Result < 100) {
          return "Data is too small to bother with.";
      }
      else {
          // If we did not receive the entire file, the end of the
          // data buffer will contain garbage.
          if (antecedent.Result < data.Length)
              Array.Resize(ref data, antecedent.Result);

          // Will be returned in the Result property of the Task<string>
          // at some future point after the asynchronous file I/O operation completes.
          return new UTF8Encoding().GetString(data);
      }
  });
}
{% endhighlight %}

IMO, this is much more readable code than using APM operation. Of course by using Task-based API, you can get [the goodness of TPL functionalities][TPL].

[TPL]:http://msdn.microsoft.com/en-us/library/dd537609.aspx
[tpl_amp]:http://msdn.microsoft.com/en-us/library/dd997423.aspx
[ext_method]:http://msdn.microsoft.com/en-us/library/bb383977.aspx
[s_begin_read]:http://msdn.microsoft.com/en-us/library/system.io.stream.beginread.aspx
[s_end_read]:http://msdn.microsoft.com/en-us/library/system.io.stream.endread.aspx
