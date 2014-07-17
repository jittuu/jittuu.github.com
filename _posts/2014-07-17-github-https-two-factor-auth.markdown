---
layout: post
title: "Github - two-factor authentication and cloning with https"
---

If you are on windows, [cloning with https](https://help.github.com/articles/which-remote-url-should-i-use#cloning-with-https) may be a better option for you. You don't have to set up ssh agent (which sounds alien for most windows user). You also don't have to enter password every time you use, you can cache your password by following github's [article](https://help.github.com/articles/caching-your-github-password-in-git). In short, make sure you have **msysgit 1.8.1 and above** and set credential helper as follows:

```
git config --global credential.helper wincred
```

# Two factors authentication

But after you enable [2FA](https://github.com/blog/1614-two-factor-authentication), the above method doesn't work anymore. We need a few more steps to make https url work with 2FA.

1. Go to Github [application settings](https://github.com/settings/applications)
  ![github application settings](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/github2FA-https-1.png)
2. Click `Generate new token`
3. Note down your token.
4. Go to Windows Credentials Manager
  ![Windows credentials manager](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/github2FA-https-2.png)
5. Click `Add a generic credential`
6. Enter your credential with the _generated token_. Note that address format is `git:https://<username>@github.com`
  ![Add credential](https://raw.githubusercontent.com/jittuu/jittuu.github.com/master/images/github2FA-https-3.png)

That's all, you should now be able to push without entering any username and password.
