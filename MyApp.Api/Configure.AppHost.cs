// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Funq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using ServiceStack.Auth;

[assembly: HostingStartup(typeof(MyApp.AppHost))]

namespace MyApp;

/// <summary>
/// To create Identity SQL Server database, change "ConnectionStrings" in appsettings.json
///   $ dotnet ef migrations add CreateMyAppIdentitySchema
///   $ dotnet ef database update
/// </summary>
public class AppHost : AppHostBase, IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) => {
            var config = context.Configuration;

            services.AddMvcCore(options => options.EnableEndpointRouting = false)
                .AddAuthorization()
                .AddNewtonsoftJson();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options => {
                    options.Authority = "https://localhost:5000";
                    options.RequireHttpsMetadata = false;

                    options.Audience = "api1";
                });
        })
        .Configure(app => {
            app.UseAuthentication();

            app.UseServiceStack(new AppHost());
            
            app.UseMvc();
        });

    public AppHost() : base(nameof(Api), typeof(AppHost).Assembly) { }

    public override void Configure(Container container)
    {
        SetConfig(new HostConfig {
            DefaultRedirectPath = "/metadata"
        });
            
        Plugins.Add(new AuthFeature(() => new AuthUserSession(), 
            new IAuthProvider[] {
                new NetCoreIdentityAuthProvider(AppSettings), 
            }));
    }
}

[Route("/servicestack-identity")]
public class GetIdentity : IReturn<GetIdentityResponse> { }

public class GetIdentityResponse
{
    public List<Property> Claims { get; set; }
    public AuthUserSession Session { get; set; }
}

[Authenticate]
public class IdentityService : Service
{
    public object Any(GetIdentity request)
    {
        return new GetIdentityResponse {
            Claims = Request.GetClaims().Map(x => new Property { Name = x.Type, Value = x.Value }),
            Session = SessionAs<AuthUserSession>(),
        };
    }
}