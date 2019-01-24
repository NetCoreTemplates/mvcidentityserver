# mvcidentityserver

.NET Core 2.2 MVC Website integrated with ServiceStack using IdentityServer4 Auth

[![](https://raw.githubusercontent.com/ServiceStack/Assets/master/csharp-templates/mvcidentityserver.png)](http://mvcidentityserver.web-templates.io/)

> IdentityServer4 Login

[![](https://raw.githubusercontent.com/ServiceStack/Assets/master/csharp-templates/mvcidentityserver-is4.png)](http://mvcidentityserver.web-templates.io/)

> Browse [source code](https://github.com/NetCoreTemplates/mvcidentityserver), view live demo [mvcidentityserver.web-templates.io](http://mvcidentityserver.web-templates.io) and install with the `web` dotnet tool:

    $ dotnet tool install -g web

    $ web new mvcidentityserver ProjectName

### OAuth Setup

Replace the `oauth.*` App settings with your own in `appsettings.Development.json` for local development and `appsettings.json` for production deployments.

 - Twitter - [Create Twitter App](https://dev.twitter.com/apps) with `{BaseUrl}/signin-twitter` referrer and follow [Twitter walk through](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/twitter-logins?view=aspnetcore-2.2)
 - Facebook - [Create Facebook App](https://developers.facebook.com/apps) with `{BaseUrl}/signin-facebook` referrer and follow [Facebook walk through](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/facebook-logins?view=aspnetcore-2.2)
 - Google - [Create Google App](https://console.developers.google.com/apis/credentials) with `{BaseUrl}/signin-google` referrer and follow [Google walk through](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-2.2)
 - Microsoft - [Create Microsoft App](https://apps.dev.microsoft.com) with `{BaseUrl}/signin-microsoft` referrer and follow [Microsoft walk through](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/microsoft-logins?view=aspnetcore-2.2)
