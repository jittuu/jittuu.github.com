---
layout: post
title: "Monitoring Windows services"
---

We usually use windows services to run the back-end jobs for the business needs. But what if the services fail which can be happened because of any unforeseen reason?

Of course, we would like to be **notified** and/or try to restart the service if necessary.

Writing another service as monitoring service? ( _I know you are thinking it now_ :P ) I don't think it is very good idea. If we choose this route, we need to make sure the monitoring service is **always** running. ( _inception_?)

If you want _just right_ version for monitoring, you can use **built-in** feature from Windows. Look into the **Recovery** tab of the service _properties_, as available via **services.msc**.

![service properties](http://i.imgur.com/JhfGG.png)

You can choose one of the followings when service fails:

1. Restart the Service
2. Run a program
3. Restart the Computer

_Run a program_ could be a small script just for sending email and restart the service or a console application with complex logic.

If you want a _bigger_ solution with an overview dashboard and all, you should ask your boss to buy third party commercial system monitoring solution. :)
