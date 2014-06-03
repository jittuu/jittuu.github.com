---
layout: post
published: false
title: "How to set up load balanced squid in Azure"
---

In my [previous post][], we have setup squid as forward proxy. In this post, we are going to add loading balancing feature.

# Capture the Linux Virtual Machine with squid

1. Connect to VM and login via SSH client (I use [PuTTy][])
2. We need to capture linux image with squid - to make it easier to add more instance into load balancing cluster.

```
$ sudo waagent -deprovision
WARNING! The waagent service will be stopped.
WARNING! All SSH host key pairs will be deleted.
WARNING! Cached DHCP leases will be deleted.
WARNING! Nameserver configuration in /etc/resolvconf/resolv.conf.d/{tail,originial} will be deleted.
Do you want to proceed (y/n)? y
```

2. Type **Exit** to close the SSH client.
3. In the Management Portal, select the virtual machine, and then click Shut down.
4. When VM is stopped, click **Capture** on the command bar.
5. Provide the image name and **check** `I have run the Windows Azure Linux Agent on the virtual machine`

![capture virtual machine](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/lb-squid-1.png)

The captured Image should be available under `VIRTUAL MACHINES | IMAGES`

![capture virtual machine](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/lb-squid-2.png)


# Create first VM

We are going to create our first VM of load balancing set from the captured Image and configure with load balancing endpoint.

1. In the azure portal, create a virtual machine `NEW | COMPUTE | VIRTUAL MACHINES | FROM GALLERY`.
2. Choose the captured Image from `MY IMAGES`.
	![capture virtual machine](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/lb-squid-3.png)
3. Provide vm name, user name and password.
	![capture virtual machine](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/lb-squid-4.png)
4. Choose the cloud service - in my case, I choose the old one since I just delete vm while capturing as image, but not the cloud service.
	![capture virtual machine](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/lb-squid-5.png)
5. Configure virtual machine. I **uncheck** the `the VM agent that supports extensions is already installed` since we are not using any extensions.
	![capture virtual machine](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/lb-squid-6.png)
6. Go to `VIRTUAL MACHINES | <your instance> | ENDPOINTS` and click **ADD** on the command bar.
7. Configure endpoint as Load-balanced set.
	![capture virtual machine](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/lb-squid-7.png)

Before we add another VM, try to configure proxy in your browser and test it. For instance, this is the setting for firefox.

![capture virtual machine](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/lb-squid-8.png)

# Add one more VM to load-balanced set

Repeat the same steps as creating first VM. _The only important thing is that **make sure to use the same cloud service as first VM**_

We add endpoint for squid as load-balanced set.

![capture virtual machine](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/lb-squid-9.png)


[previous post]:(http://www.jittuu.com/2014/5/29/how-to-setup-squid-as-forward-proxy-in-azure/)
[PuTTy]:http://www.chiark.greenend.org.uk/~sgtatham/putty/download.html
