XPlatformCloudKit
=================

A Hands-on lab for creating your very own multi-platform app with Azure Mobile Services as a backend.


Prerequisites for all projects
------------------------------

* Visual Studio 2012 Premium or above 
* Windows 8 Pro 
* (Does not currently build all project in Visual Studio 2013 RC)

Notes on getting Windows Phone 8 project to run:
-----------------------------------------------

* Requires Windows 8 Pro - 64 bit
* Requires installation of [Windows Phone 8 SDK](http://aka.ms/phonesdk-cr)

Notes on getting Android Project to run
---------------------------------------

- To build in Visual Studio will require a [Xamarin Business License](https://store.xamarin.com/).
- You will need to copy the folder and all contents of `XPlatformCloudKit/SupportedFrameworks` to:
  
```
C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETPortable\v4.5\Profile\Profile78\SupportedFrameworks
```
This allows PCLs which target `Profile78` to be consumed by Xamarin IOS/Android projects.
- You must apply the [Xamarin hotfix](http://forums.xamarin.com/discussion/5507/using-system-linq-expressions-in-a-pcl-method-causes-typeloadexpression-mono-android-4-7-10024). This fixes an issue in the `System.Linq.Expressions.dll` facade used by Xamarin
