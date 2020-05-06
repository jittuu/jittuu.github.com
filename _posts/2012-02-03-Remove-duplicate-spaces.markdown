---
layout: post
title: "Remove duplicate whitespace"
---

We often need to replace duplicate <s>space</s>[whitespace][] with _single_ space. For example, I want to remove duplicate spaces in this string, <code class="inline">The world is awesome</code> as <code class="inline">The world is awesome</code>.

{% highlight csharp %}
public static class StringExt {
private static string RemoveDuplicateWhitespace(this string value) {
return Regex.Replace(value, @"\s+", " ");
}
}
{% endhighlight %}

The above method used [regex][] to find _one or more_ [whitespace][] and replace with _single_ space. You can use that method as:

{% highlight csharp %}
"The world is awesome.".RemoveDuplicateWhitespace(); // => "The world is awesome."
{% endhighlight %}

[regex]: //en.wikipedia.org/wiki/Regular_expression
[whitespace]: //en.wikipedia.org/wiki/Whitespace_(computers)
