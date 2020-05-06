---
layout: post
title: "Formatting Number with Section Separator"
---

![formatting](//i.imgur.com/E13ud.png)

While I'm working with number for reporting, I need to format number with different formatting depending on whether its value is positive or negative.

If its value is positive number, I want to show with two decimal point, e.g <code class="inline">1234.00</code>. If its value is negative number, I want to show number in bracket, e.g. <code class="inline">(1234.00)</code>.

Surprisingly, at least for me, .Net Numeric Formatting already support the above case with [Section Separator][section_separator].

Here is the code:

{% highlight csharp %}
var value = 1234;
var negValue = -1234;

var format = "The result is: {0:##.##;(##.##)}";

Console.WriteLine(format, value); // => The result is: 1234.00
Console.WriteLine(format, negValue); // => The result is: (1234.00)
{% endhighlight %}

If you have to handle for _zero_ as well, you can do as

{% highlight csharp %}
var format = "The result is: {0:##.##;(##.##);zero}";
Console.WriteLine(format, 0); // => The result is: zero
{% endhighlight %}

.Net Numeric Formatting is pretty powerful, you can read more detail in [here][n_format_1] and [here][n_format_2].

[section_separator]: //msdn.microsoft.com/en-us/library/0c899ak8.aspx#SectionSeparator
[n_format_1]: //msdn.microsoft.com/en-us/library/dwhawy9k.aspx
[n_format_2]: //msdn.microsoft.com/en-us/library/0c899ak8.aspx
