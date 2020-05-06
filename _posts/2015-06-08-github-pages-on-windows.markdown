---
layout: post
title: "Set up github pages with jekyll on Windows"
---

I re-setup [jekyll][] on my PC as I didn't upgrade since I [did it 3 years ago][1]. It is much easier to setup jekyll on windows - thanks to `bundler` and `github-pages` gem.

Here is the steps:

1. [Install Ruby and DevKit][2]. I install **ruby 2.0.0** series because there are [some issues][3] in nokogiri gem with ruby 2.2 series
2. Install bundler by running the command `gem install bundler`
3. Create/add a file called `Gemfile` with this content

   ```
   source 'https://rubygems.org'
   gem 'github-pages'
   gem 'wdm', '>= 0.1.0' if Gem.win_platform?
   ```

4. Install all the dependencies by simply running the command `bundle install`

:tada: :tada: :tada: :tada:

## Run the jekyll

Use the command `bundle exec jekyll serve` in the root of repository and the site should be available at `//localhost:4000`.

[1]: //www.jittuu.com/2012/6/4/installing-jekyll-on-windows-7/
[2]: //jekyll-windows.juthilo.com/1-ruby-and-devkit/
[3]: https://www.google.com/search?q=gem%20nokogiri%20ruby%202.2
[jekyll]: https://github.com/mojombo/jekyll
