// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ServiceStack;

namespace IdentityServer
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(Config.GetUsers());

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }

            services.AddAuthentication()
                .AddTwitter(options => { /* Create Twitter App at: https://dev.twitter.com/apps */
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    
                    options.ConsumerKey = Configuration["oauth.twitter.ConsumerKey"];
                    options.ConsumerSecret = Configuration["oauth.twitter.ConsumerSecret"];
                    options.SaveTokens = true;
                    options.RetrieveUserDetails = true;
                })
                .AddFacebook(options => { /* Create App https://developers.facebook.com/apps */
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.AppId = Configuration["oauth.facebook.AppId"];
                    options.AppSecret = Configuration["oauth.facebook.AppSecret"];
                    options.SaveTokens = true;
                    options.Scope.Clear();
                    Configuration.GetSection("oauth.facebook.Permissions").GetChildren()
                        .Each(x => options.Scope.Add(x.Value));
                })
                .AddGoogle(options => { /* Create App https://console.developers.google.com/apis/credentials */
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = Configuration["oauth.google.ConsumerKey"];
                    options.ClientSecret = Configuration["oauth.google.ConsumerSecret"];
                    options.SaveTokens = true;
                })
                .AddMicrosoftAccount(options => { /* Create App https://apps.dev.microsoft.com */
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = Configuration["oauth.microsoftgraph.AppId"];
                    options.ClientSecret = Configuration["oauth.microsoftgraph.AppSecret"];
                    options.SaveTokens = true;
                })
                .AddOpenIdConnect("oidc", "OpenID Connect", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                    options.SaveTokens = true;

                    options.Authority = "https://demo.identityserver.io/";
                    options.ClientId = "implicit";
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                    
                    options.Events = new OpenIdConnectEvents {
                        OnRemoteFailure = context => { // Handle cancel callback
                            context.Response.Redirect("/");
                            context.HandleResponse();

                            return Task.FromResult(0);
                        }
                    };
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseIdentityServer();

            app.UseMvcWithDefaultRoute();
        }
    }
    
}