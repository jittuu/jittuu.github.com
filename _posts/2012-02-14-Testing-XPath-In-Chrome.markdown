---
layout: post
title: "Testing XPath in Google Chrome browser"
---

Recently, I'm doing [web scraping][scraping] using the _awesome_ library [HtmlAgilityPack][]. With _HtmlAgilityPack_, I use [XPath][] to match the required object/tag in the [DOM][]. While working with [XPath][], I'm wondering is there anyway to test _XPath_ in [Google Chrome][chrome] browser with **highlighting** the _matched_ tag in browser. The answer is **YES**. We can test _XPath_ in _Google Chrome_.

Here is the steps to do so:

1. Open [Developer Tools][devtools]
2. Select [Console][console] tab.
3. Use <code class="inline">$x</code> token. For example, <code class="inline">$x("/html/body")</code> will select the <code class="inline">body</code> tag.

A picture worth a thousand words. :)

![xpath in chrome](//i.imgur.com/dSrrv.png)

[scraping]: //en.wikipedia.org/wiki/Web_scraping
[htmlagilitypack]: //htmlagilitypack.codeplex.com
[xpath]: //en.wikipedia.org/wiki/XPath
[dom]: //en.wikipedia.org/wiki/Document_Object_Model
[chrome]: //www.google.com.sg/chrome/
[devtools]: //code.google.com/chrome/devtools/docs/overview.html
[console]: //code.google.com/chrome/devtools/docs/console.html
