---
layout: post
title: Desktop Application deployment with auto update (Shimmer/Squirrel)
---

Recently, I'm working on a desktop application. Deployment is always a challenge for the desktop application. There is [ClickOnce][] for .Net application, but it doesn't work perfectly. You can find more issues that are [faced by GitHub for WIndows Team][GHFWIssues]. I trust the Github guys and want to give it a try [Squirrel][] for ClickOnce replacement.

## The Application

I create a WPF application. To be more real-world-like application, I integrate with [awesomium][] in the application. After I installed [Aweominum SDK][aweSDK] and add it to my application. I set _Copy Local_ to true to deploy all awesomium binaries to the bin folder. The MainWindow.xaml file will looks like:

{% highlight xml %}
<Window
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:awe="http://schemas.awesomium.com/winfx" x:Class="Banshee.MainWindow"
        Title="MainWindow" Height="800" Width="1024">
    <Grid>

    <awe:WebControl 
        HorizontalAlignment="Stretch" 
        VerticalAlignment="Stretch"
        Source="http://jittuu.com"
        />

    </Grid>
</Window>
{% endhighlight %}

and, I update `AssemblyInfo.cs` with

{% highlight csharp %}
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
{% endhighlight %}

That's it! This is the only changes I made in `File -> New` WPF application (.Net Framework 4).

## ~Squirrel~ Shimmer

I follow [QuickStart][] from the wiki of Squirrel. But when I reach to [Publish Release][] state, the `New-Release` cmdlet is not found. After spending a few hours, I notice that **Squirrel package is not ready** yet (as of 25 April 2014) and there is already [pull request][PR211] about it. So, I just use old version [Shimmer][] by `Install-Package Shimmer`. It will download all the dependencies and open `nuspec` file for you.

Now, it is time to publish a release. I invoke `New-Release` and it ask me to build first. So, I build it and run `New-Release` again. Still got error! After researching a bit, I realize that it need to `Enable Nuget Package Restore`. I enable package restore, rebuild and run `New-Release` again. Now, it works!

It creates `Releases` folder under solution folder and put all the [releases files][]. I'm lazy person, so I just
 double click the `Setup.exe` and install it. It works perfectly. Cool! But, when I check all the deployed binarys in the local appdata, it doesn't include the awesomium binaries. I uninstall it from Control Panel and check `AppData\Local` for remaining files. All are cleanly uninstall.
 
To add all the awesomium binaries, I modify nuspec by adding files:

{% highlight xml %}
  <files>
    <file src="bin\$configuration$\Awesomium.*.dll" target="lib\net40\" />
    <file src="bin\$configuration$\avcodec-53.dll" target="lib\net40\" />
    <file src="bin\$configuration$\avformat-53.dll" target="lib\net40\" />
    <file src="bin\$configuration$\avutil-51.dll" target="lib\net40\" />
    <file src="bin\$configuration$\awesomium.dll" target="lib\net40\" />
    <file src="bin\$configuration$\icudt.dll" target="lib\net40\" />
    <file src="bin\$configuration$\libEGL.dll" target="lib\net40\" />
    <file src="bin\$configuration$\libGLESv2.dll" target="lib\net40\" />
    <file src="bin\$configuration$\xinput9_1_0.dll" target="lib\net40\" />
  </files>
{% endhighlight %}

I rebuild the solution, publish release and install again. When I check the deployed binaries, I can see all the awesomium binaries as well. So far, so good.

## Setup IIS website

Now, it is time to host at IIS. I did:
 1. Create a website with port 8080 and point to a folder called RTW.
 2. Copy the files from Releases folder (NOT Release under bin) to RTW folder.
 3. Download the setup file from something like: `http://yourserver:8080/setup.exe` and install it.
 
It is working too! Next, we need to test for update.

## Updating itself

Now it is time to add update feature in our application. I hook `Loaded` event and add the following code to update itself:

{% highlight csharp %}
private async void Window_Loaded(object sender, RoutedEventArgs e)
{
    var updateManager = new UpdateManager(@"http://yourserver:8080/", "Banshee", FrameworkVersion.Net40);
    using (updateManager)
    {
        var updateInfo = await updateManager.CheckForUpdate().ToTask();
        if (updateInfo == null || !updateInfo.ReleasesToApply.Any())
        {
            return;
        }
        else
        {
            var releases = updateInfo.ReleasesToApply;
            await updateManager.DownloadReleasesAsync(releases, _ => { });
            await updateManager.ApplyReleasesAsync(updateInfo, _ => { });
        }
    }
}
{% endhighlight %}

 1. Uninstall the application
 2. Clear the Releases folder
 3. Clear RTW folder at server
 4. Rebuild the solution
 5. Publish new release with `New-Release`
 6. Copy all the release files to RTW folder
 7. Download and run setup.exe from the server.
 8. **Boom!** It Crashs!
 
After struggling a bit, I found that we need to give web access to the releases file in the web server. I add this web config to the web server and then it works.

{% highlight xml %}
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
    <system.webServer>
        <staticContent>
            <mimeMap fileExtension="." mimeType="text/plain" />
            <mimeMap fileExtension=".nupkg" mimeType="application/zip" />
        </staticContent>
    </system.webServer>
</configuration>
{% endhighlight %}

## Releasing new version

For new version, I did:

 1. Update the `Title` just to make sure it is updated to new version
 2. Increase version to `1.0.1` in AssemblyInfo.cs
 3. Clean the solution
 4. Rebuild the solution
 5. Publish new release with `New-Release`
 6. Then I got this error.
    ```
    Method invocation failed because [System.Object[]] doesn't contain a method named 'Split'.
At C:\Users\soe.moe\documents\visual studio 2013\Projects\Banshee\packages\Shimmer.0.7.4\tools\commands.psm1:77 char:41
+         $packages = $releaseOutput.Split <<<< (";")
    + CategoryInfo          : InvalidOperation: (Split:String) [], RuntimeException
    + FullyQualifiedErrorId : MethodNotFound
    ```
  7. I tried to clear `Releases` folder and `*.nupkg` in the output folder.
  8. It works after I clear, but I didn't generate _delta_ package.
  9. If I don't clear `Releases` folder, the error is back. :sweat:
  
Hmm... OK. I'll break down into a few problems; 
  1. If it can generate delta package, it **cannot** generate setup.exe.
  2. If it can generate setup.exe and full package, it **cannot** generate delta package.
  3. How about `RELEASE` file? Above two generate two different `RELEASE` file.
  
Let's check delta package first.

 1. I leave `1.0.0-beta-full.nupkg` package and RELEASE file in the Releases folder.
 2. Clean the solution.
 3. Rebuild the solution
 4. Publish new release with `New-Release`
 5. I _Ignore_ error for now.
 6. Copy three files, `RELEASE` and two full and delta packages of new version, to the server
 7. Run the _installed_ application.
 8. Woof! It was updated to new version.

[ClickOnce]: http://msdn.microsoft.com/clickonce
[GHFWIssues]: https://github.com/Squirrel/Squirrel.Windows/issues/82
[Squirrel]: https://github.com/Squirrel/Squirrel.Windows
[awesomium]: http://www.awesomium.com/
[aweSDK]: http://www.awesomium.com/download/
[QuickStart]: https://github.com/squirrel/Squirrel.Windows/wiki/QuickStart
[Publish Release]: https://github.com/squirrel/Squirrel.Windows/wiki/QuickStart#publish-a-release
[PR211]: https://github.com/Squirrel/Squirrel.Windows/pull/211
[Shimmer]: http://www.nuget.org/packages/Shimmer
[releases files]: https://github.com/squirrel/Squirrel.Windows/wiki/QuickStart#whats-in-a-release
