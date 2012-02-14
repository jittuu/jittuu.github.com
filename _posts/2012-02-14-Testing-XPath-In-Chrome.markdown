---
layout: post
title: "Testing XPath in Google Chrome browser"
---

Recently, I'm doing [web scraping][scraping] using the _awesome_ library [HtmlAgilityPack][]. With _HtmlAgilityPack_, I use [XPath][] to match the required object/tag in the [DOM][]. While working with [XPath][], I'm wondering is there anyway to test _XPath_ in [Google Chrome][chrome] browser with **highlighting** the _matched_ tag in browser. The answer is **YES**. We can test _XPath_ in _Google Chrome_.

Here is the steps to do so:

1. Open [Developer Tools][devTools]
2. Select [Console][console] tab.
3. Use <code class="inline">$x</code> token. For example, <code class="inline">$x("/html/body")</code> will select the <code class="inline">body</code> tag.

A picture worth a thousand words. :)

![xpath in chrome](http://i.imgur.com/dSrrv.png)

[scraping]:http://en.wikipedia.org/wiki/Web_scraping
[HtmlAgilityPack]:http://htmlagilitypack.codeplex.com
[XPath]:http://en.wikipedia.org/wiki/XPath
[DOM]:http://en.wikipedia.org/wiki/Document_Object_Model
[chrome]:http://www.google.com.sg/chrome/
[devTools]:http://code.google.com/chrome/devtools/docs/overview.html
[console]:http://code.google.com/chrome/devtools/docs/console.html
