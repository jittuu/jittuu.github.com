---
layout: post
title: "How to set up squid as forward proxy in Azure"
---

I was playing with [squid][] to set up as [forward proxy][]. If you are not sure about proxy, there a [great answer][proxy-answer] at stackoverflow. In this post, I will use Azure as Cloud platform, but it should also work on Amazon as well.

# Set up a Linux VM

We will first create a linux VM using Azure [portal][] and later we use _PuTTy_ to access.

From azure portal, I'll go `NEW | COMPUTE | VIRTUAL MACHINE | FROM GALLERY` and choose Ubuntu.

![gallery](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/squid-1.png)

I will just choose user name with password. If you prefer SSH key, you can also use that too.

![vm configuration](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/squid-2.png)

I'll create new `CLOUD SERVICE` as well - if you are wondering what cloud service is, it is just the **container** of _one or more_ virtual machines. Please note that there is `CLOUD SERVICE DNS NAME` - we will use that name to connect the vm.

![cloud service](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/squid-3.png)

Now, I'll use [PuTTy][] to connect the vm (We just need _Putty.exe_). The host name will be _cloud-service-name_.cloudapp.net - in my case, the host name is `squidpxy.cloudapp.net`. 

![putty](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/squid-4.png)

After connect, you should be able to log in with user name and password.

# Install and configure squid

Before install anything, we will update the system itself first.

```
$ sudo apt-get update -y
$ sudo apt-get upgrade -y
```

We will install Squid and some utilities that we need later.

```
$ sudo apt-get install squid apache2-utils
```

We are going to use HTTP Digest authentication to authenticate users using a local password file. Let's create the password file.

```
$ cd /etc/squid3/
$ sudo touch passwd
$ sudo chown proxy:proxy passwd
$ sudo chmod 640 passwd
```

We don't need to change the owner and permission to make squid works, but it’s good security practice. If you check the file,

```
$ ls -l passwd
```

you should see this - of course, the date and time will be different.

```
-rw-r----- 1 proxy proxy 0 May 29 09:02 passwd
```

Now, we are going to add user to password file by using [htdigest][] from `apache2-utils`.

```
$ sudo htdigest /etc/squid3/passwd krt jittuu
Adding user jittuu in realm krt
New password:
Re-type new password:
```

We can test the new user with squid digest auth as below. (of course, md5 hash will be different. If the same, we are using the same password. :smiling_imp:)

```
$ sudo /usr/lib/squid3/digest_file_auth -c /etc/squid3/passwd
"jittuu":"krt"
01b21a5c47050b4e56d6c1c5540acd8f
```

It is the time to configure squid. The default configuration file is in `/etc/squid3/squid.conf` with thousands of line - because it is heavily documented configuration file. I think it is better to create new file than editing the default config. 

```
$ sudo mv /etc/squid3/squid.conf /etc/squid3/squid.conf.origin
$ sudo touch /etc/squid3/squid.conf
```

Luckily, we are just setting forward proxy without any caching. For now, these are all we need:

```
auth_param digest program /usr/lib/squid3/digest_file_auth -c /etc/squid3/passwd
auth_param digest realm krt
auth_param digest children 5

acl auth_users proxy_auth REQUIRED

http_access allow auth_users
http_access deny all

http_port 3128
```

I will just use squid default port: `3128`, but I strongly recommend to **change** other random port. After we configure, we need restart the squid with this command.

```
$ sudo service squid3 restart
```

At azure, we still need to open the endpoint of the vm. Go to `VIRTUAL MACHINES | <vm> | ENDPOINTS`

![endpoint](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/squid-5.png)

For now, we will just use standalone endpoint and specify the endpoint details with the port squid use.

![standalone](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/squid-6.png)

![open port](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/squid-7.png)

OK. That's all to install and configure squid.

# Accessing via squid

I'll use Firefox browser to use with proxy. Go to `Options | Advanced | Network | Settings`

![firefox network setting](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/squid-8.png)

I'll use Manual proxy configuration

![firefox manual proxy](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/squid-9.png)

When the browser prompt the dialog box, enter user name and password. You should be able to browse via proxy now. You could test your IP at <http://whatismyipaddress.com/>. It should be different if you test your IP with different browser.

But if you visit <http://www.whatismyip.com/>, you will see like: 

![no privacy](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/squid-10.png)

It is because the server still can detect that you are browser via proxy.

# Privacy

To protect the privacy, we can strip proxy header by adding the following settings to `squid.conf`.

```
forwarded_for delete
via off
```

We restart squid to reload the config.

```
$ sudo service squid3 restart
```

Now, if you visit <http://www.whatismyip.com/>, the server should not able to detect you are behind the proxy.

![privacy](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/squid-11.png)

I hope this post will help someone who want to set up squid as forward proxy in azure.

[squid]:http://www.squid-cache.org/
[forward proxy]:http://en.wikipedia.org/wiki/Proxy_server#Forward_proxies
[proxy-answer]:http://stackoverflow.com/a/366212
[portal]:http://azure.microsoft.com/en-us/
[putty]:http://www.chiark.greenend.org.uk/~sgtatham/putty/download.html
[htdigest]:http://httpd.apache.org/docs/2.2/programs/htdigest.html
