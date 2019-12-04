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

namespace MyApp.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public new void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore(options => options.EnableEndpointRouting = false)
                .AddAuthorization()
                .AddNewtonsoftJson();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options => {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;

                    options.Audience = "api1";
                });
        }

        public new void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();

            app.UseServiceStack(new AppHost {
                AppSettings = new NetCoreAppSettings(Configuration)
            });
            
            app.UseMvc();
        }
    }

    public class AppHost : AppHostBase
    {
        public AppHost()
            : base(nameof(Api), typeof(AppHost).Assembly) { }

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
}