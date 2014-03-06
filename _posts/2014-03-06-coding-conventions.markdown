---
layout: post
title: "Coding conventions"
---

Recently, I'm working on creating [coding conventions][] for my company. That is always difficult to create guideline because there is no "right" answer. Everyone has their own preferred style. No matter what coding guidelines we choose, we're not going to make everyone happy. But I believe that one standard is **always better** than two standards.

Most of the time, we solve it by creating coding guideline _document_. The issue with this approach is **no one read it**. I have worked in a few companies, either big or small, they usually have one coding guideline document _(mostly outdated)_ - but developer don't read it. They just **scan it**. Developer are not good at reading documents. Even ~~they~~we do read it, but it is not impossible to remember all the guidelines while we are coding. Eventually, the guideline document become **dead** document.

I personally believed in automation. If we can't automate checking the code against the guideline, I prefer to drop that guideline. Fortunately, there are some tools created by smart people to address the issue. I chose [StyleCop][] and [FxCop][] to address the coding guideline issue for our .NET development. Of course, we don't bring every rules which are defined in those tools. We picked most of them that is good enough for us. That rules must be **living rules** which means the rules need to be updated from time to time. [Retrospective ][] meeting might be the good place to talk about it.

The workflow will be:

1. Create team/company-wise setting/rulesets for StyleCop and FxCop
2. Integrate with the solution and development environment
3. Integrate with [CI][] _(Well, this is one of the good reason to have CI)_
4. Update the setting/ruleset (if required)
5. Repeat 2-4

I have created coding guideline at [HERE](https://gist.github.com/jittuu/9360990). Of course, I would be happy if you can take a look and put some comment on it. :)


[coding conventions]: http://en.wikipedia.org/wiki/Coding_conventions
[StyleCop]: http://stylecop.codeplex.com/
[FxCop]: http://en.wikipedia.org/wiki/FxCop
[Retrospective]: http://en.wikipedia.org/wiki/Retrospective#Software_development
[CI]: http://en.wikipedia.org/wiki/Continuous_integration
