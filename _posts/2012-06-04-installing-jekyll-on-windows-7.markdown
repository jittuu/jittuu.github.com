---
layout: post
title: "Installing jekyll on Windows 7"
---

I'm [using github pages][pblog] for this blog. Today I'm setting up [jekyll][] on my new laptop which is running on _Windows 7_.

While setting up, I noted down the steps for installing [jekyll][] on Windows. Hopefully it can help someone like me who want to use [jekyll][] as blogging system and also need to use Windows.

Here are the steps;

1. [install Ruby][install_ruby]
2. download [Ruby Dev Kit][ruby_dev_kit] which is required to build native gem and extract it to path such as `C:\RubyDevKit`. And then run the following commands `ruby dk.rb init` and `ruby dk.rb install`.
  ![ruby and dev kit](http://i.imgur.com/sfxWV.png)
3. install jekyll gem, `gem install jekyll`
  ![jekyll gem](http://i.imgur.com/NgSBU.png)
4. install rdiscount gem, `gem install rdiscount`. _(I can't manage to work maruku in my machine. Since rdiscount is faster and working fine on Windows, I just use it.)_
5. [install python][install_python] _(this steps and belows are required for pygments syntax highlighting.)_
6. To install `easy_install` download `distribute_setup.py` from [here][distribute_setup] and run this command in python, `python.exe distribute_setup.py`.
7. install [pygments][] using easy\_install; `easy_install.exe pygments`
  ![install pygments](http://i.imgur.com/qv7gF.png)
8. you might need to add Pygmentize.exe in your PATH. In my case, it is `C:\Python32\Scripts`.
9. if you got error like `Liquid error: Bad file descriptor` when you running Pygmentize, you need to change this file `C:\Ruby192\lib\ruby\gems\1.9.1\gems\albino-1.3.3\lib\albino.rb` as this [gist][].

[pblog]:http://jittuu.com/2011/10/19/this-blog-is-hosted-on-Github/
[jekyll]:https://github.com/mojombo/jekyll
[install_ruby]:http://rubyinstaller.org/downloads/
[ruby_dev_kit]:http://rubyinstaller.org/downloads/
[install_python]:http://www.python.org/getit/
[distribute_setup]:http://pypi.python.org/pypi/distribute#distribute-setup-py
[pygments]:http://pygments.org/
[gist]:https://gist.github.com/1166390
