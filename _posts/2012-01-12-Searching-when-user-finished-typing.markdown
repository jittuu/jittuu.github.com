---
layout: post
title: "Searching when user finished typing (AJAX)"
---

![searching](//i.imgur.com/27Sq4.png)

My application has a search field.

I want to search with criteria just after user **stop** typing for 200ms like above image. And this is how I did it.

{% highlight javascript %}
var delay = (function() {
var timeoutId = 0;
return function(callback, ms) {
clearTimeout(timeoutId);
timeoutId = setTimeout(callback, ms);
}
})();

\$("input#search").keyup(function() {
delay(function() {
// invoke ajax for searching..
// in here I'll use alert as DEMO
alert("Seaching.. now!");
}, 200);
});
{% endhighlight %}

In the nutshell, I start a timer with some delay and then invoke the callback function. Of course, I clear the existing timer first before I start new timer with _[setTimeout][]_ method.

The interesting part is the _delay_ variable. Lets see what was happened in details.

{% highlight javascript %}
var delay = (function() {
// ....
})();
{% endhighlight %}

It is called _self-executed_ function. I declared a anonymous function and immediately execute it. I used it for _scoping_.

{% highlight javascript %}
var delay = (function() {
var timeoutId = 0;
// ....
})();
{% endhighlight %}

Then I used _timeoutId_ variable which is used later to clear time out. Because of _self-executed_ function, _timeoutId_ is only available within that function _(did I mention scoping? :) )_.

{% highlight javascript %}
var delay = (function() {
var timeoutId = 0;
return function(callback, ms) {
clearTimeout(timeoutId);
timeoutId = setTimeout(callback, ms);
}
})();
{% endhighlight %}

Then I return _function_ which is _[closure function][closure]_. _Closure function_ has access to its local variables. That means _timeoutId_ is still **alive** within the _closure function_ which is stored as _delay_ variable.

{% highlight javascript %}
delay(function() {
// invoke ajax for searching..
// in here I'll use alert as DEMO
alert("Seaching.. now!");
}, 200);
{% endhighlight %}

When user type, I invoke _delay_ (closure function) with the callback function and delay (200 ms).

#Example

Lets say; user type "Awesome", the keyup event will be fired for A-Aw-Awe-Awes-Aweso-Awesom-Awesome. _delay_ is invoked for 7 times.

1. A - (**Before**) invoke _timeoutId_ is 0. clearTimeout will do nothing. (**After**) invoke _timeoutId_ will be the return number from _setTimeout_ method. Lets assume that as 1.
2. Aw - (**Before**) invoke _timeoutId_ is 1. clearTimeout will clear the timer for "A". (**After**) invoke _timeoutId_ will be new number again, e.g. 2.
3. Awe - (**Before** invoke _timeoutId_ is 2. clearTimeout will clear the timer for "Aw". (**After**) invoke _timeoutId_ will be new number again, e.g. 3.
4. Awe... so on
5. Awesome - (**Before**) invoke it clears previous timer and set new one.
6. after 200ms - invoke the callback which is showing alert in my example.

Hope this one saves a few hours for you. :).

[settimeout]: https://developer.mozilla.org/en/DOM/window.setTimeout
[closure]: //stackoverflow.com/a/111200/102940
