---
layout: post
title: Using Azure Explorer on Windows Server 2012
---

I use [Azure Explorer][] from cerebrata to work with [azure storage][storage]. Today, I need to use it at my azure VM with Windows Server 2012. 

I opened IE and go to [download][] Azure Explorer - then I got this error.

![error 1](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/azure-explorer-1.png)


As error suggest, I added http://installers.cerebrata.com into the Trusted sites. And I tried to download again. This time I got this error;

![error 2](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/azure-explorer-2.png)


I check Details.. and found these two interesting log:

```
Application url			: https://cerebratainstallers.blob.core.windows.net/installers/Azure%20Explorer/production/1.0.0.529/Cerebrata.AzureExplorer.UI.exe.manifest
Server		: Windows-Azure-Blob/1.0 Microsoft-HTTPAPI/2.0

Deployment and application do not have matching security zones.
```

This time I add https://cerebratainstallers.blob.core.windows.net into the Trusted sites and download _again_. Now it shows me Security Warning and I just click install since I know what I'm installing. :)

![security warning](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/azure-explorer-3.png)


When the installation finish, .... it works!

![working](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/azure-explorer-4.png)


[Azure Explorer]: http://www.cerebrata.com/products/azure-explorer/introduction
[storage]: http://azure.microsoft.com/en-us/services/storage/
[download]: http://www.cerebrata.com/download/azure-explorer
