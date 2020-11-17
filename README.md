# mvcidentityserver

.NET 5.0 MVC Website integrated with IdentityServer4 Auth and ServiceStack

![](https://raw.githubusercontent.com/ServiceStack/Assets/master/csharp-templates/mvcidentityserver.png)

> IdentityServer4 Login

![](https://raw.githubusercontent.com/ServiceStack/Assets/master/csharp-templates/mvcidentityserver-is4.png)

> Browse [source code](https://github.com/NetCoreTemplates/mvcidentityserver) and install with the `web` dotnet tool:

    $ dotnet tool install -g x

    $ x new mvcidentityserver ProjectName

### Docs

See [Using IdentityServer4 Auth docs](https://docs.servicestack.net/authentication-identityserver) for more info about this template.

### OAuth Setup

Replace the `oauth.*` App settings with your own in `appsettings.Development.json` for local development and `appsettings.json` for production deployments.

 - Twitter - [Create Twitter App](https://dev.twitter.com/apps) with `{BaseUrl}/signin-twitter` referrer and follow [Twitter walk through](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/twitter-logins?view=aspnetcore-2.2)
 - Facebook - [Create Facebook App](https://developers.facebook.com/apps) with `{BaseUrl}/signin-facebook` referrer and follow [Facebook walk through](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/facebook-logins?view=aspnetcore-2.2)
 - Google - [Create Google App](https://console.developers.google.com/apis/credentials) with `{BaseUrl}/signin-google` referrer and follow [Google walk through](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-2.2)
 - Microsoft - [Create Microsoft App](https://apps.dev.microsoft.com) with `{BaseUrl}/signin-microsoft` referrer and follow [Microsoft walk through](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/microsoft-logins?view=aspnetcore-2.2)
