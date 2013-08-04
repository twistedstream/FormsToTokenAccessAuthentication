# FormsToTokenAccessAuthentication #

Provides a simple implementation of the [HTTP Token Access Authentication scheme](http://tools.ietf.org/html/draft-hammer-http-token-auth-01) for use with ASP.NET Web API by leveraging the [ASP.NET Forms Authentication](ASP.NET Forms Authentication) engine.

FormsToTokenAccessAuthentication provides the following features:
* A means to authenticate API requests with a token that adheres to the Token Access Authentication spec
* Provides proper error responses when requests don't provide a token or a token has expired
* Interacts with ASP.NET Forms Authentication to generate tokens and use them to authenticate a request

## Getting Started ##

Typically, you're using FormsToTokenAccessAuthentication in a Web API project in Visual Studio.  Here's how to get started:

Run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console) to install the [FormsToTokenAccessAuthentication](http://nuget.org/packages/TS.FormsToTokenAccessAuthentication) NuGet package and configure your web project to use it: 

```
PM> Install-Package TS.FormsToTokenAccessAuthentication
```

The NuGet package install performs the following tasks:
* Adds the necessary FormsToTokenAccessAuthentication assembly references to your project
* Adds a new Web API controller class called **AuthenticationController** to your project that can be used by clients to authenticate against your API and obtain a token
* Configures the web.config file to use the HTTP module required by FormsToTokenAccessAuthentication

Once installed via NuGet, this [readme](/src/web-package/readme.txt) file will be displayed, describing the final steps required to get FormsToTokenAccessAuthentication up and running.

## To Build ##

```
git submodule update --init
.\build.cmd
```

**NOTE:** You should perform an initial build before compiling for the first time in Visual Studio as some non-source-controled code files need to be generated.

## Questions?
[@twistedstream](http://twitter.com/twistedstream)
