﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Funq;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.IdentityModel.Tokens;
using ServiceStack;
using ServiceStack.Auth;
using MyApp.ServiceInterface;
using Newtonsoft.Json.Linq;
using ServiceStack.Configuration;

namespace MyApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration) => Configuration = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
#if DEBUG
            services.AddMvc(options => options.EnableEndpointRouting = false).AddRazorRuntimeCompilation();
#else
            services.AddMvc(options => options.EnableEndpointRouting = false);
#endif

            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.ConfigureNonBreakingSameSiteCookies();
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";

                    options.Authority = "https://localhost:5000";
                    options.RequireHttpsMetadata = false;

                    options.ClientId = "mvc";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code id_token";

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Add("api1");
                    options.Scope.Add("offline_access");
                    
                    options.ClaimActions.MapJsonKey("website", "website");
                    options.ClaimActions.MapJsonKey("role", "role");
                    options.ClaimActions.Add(new AdminRolesClaimAction("Manager", "Employee"));
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };

                    options.Events = new OpenIdConnectEvents {
                        OnRemoteFailure = CustomHandlers.HandleCancelAction,
                        OnTokenResponseReceived = CustomHandlers.CopyAllowedScopesToUserClaims,                        
                    };
                });

            services.AddAuthorization(options => {
                options.AddPolicy("ProfileScope", policy =>
                    policy.RequireClaim("scope", "profile"));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseServiceStack(new AppHost {
                AppSettings = new NetCoreAppSettings(Configuration)
            });
            
            app.UseMvcWithDefaultRoute();
        }
    }

    public class AppHost : AppHostBase
    {
        public AppHost() 
            : base("MyApp", typeof(MyServices).Assembly) { }
        
        public override void Configure(Container container)
        {
            Plugins.Add(new AuthFeature(() => new CustomUserSession(), 
                new IAuthProvider[] {
                    // Adapter to enable ASP.NET Core Identity Auth in ServiceStack
                    new NetCoreIdentityAuthProvider(AppSettings) {
                        RoleClaimType = "role"
                    }, 
                }));
        }
    }

    // Add any additional metadata properties you want to store in the Users Typed Session
    public class CustomUserSession : AuthUserSession
    {
    }

    public static class CustomHandlers
    {
        /// <summary>
        /// Use this handler to copy requested Scopes to User Claims so they can be validated using a Policy  
        /// </summary>
        public static Task CopyAllowedScopesToUserClaims(TokenResponseReceivedContext context)
        {
            var scopes = context.ProtocolMessage.Scope?.Split(' ');
            if (scopes != null && context.Principal.Identity is ClaimsIdentity identity)
            {
                foreach (var scope in scopes)
                {
                    identity.AddClaim(new Claim("scope", scope));
                }
            }
            return Task.CompletedTask;
        }

        public static Task HandleCancelAction(RemoteFailureContext context)
        {
            context.Response.Redirect("/");
            context.HandleResponse();
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Use this class to assign additional roles to Admin Users
    /// </summary>
    public class AdminRolesClaimAction : ClaimAction
    {
        string[] AdminRoles { get; }
        public AdminRolesClaimAction(params string[] adminRoles) : base("role", null) => AdminRoles = adminRoles;

//        public override void Run(JObject userData, ClaimsIdentity identity, string issuer)
        public override void Run(JsonElement userData, ClaimsIdentity identity, string issuer)
        {
            if (!HasAdminRole(JObject.Parse(userData.GetRawText()))) return;
            foreach (var role in AdminRoles)
            {
                identity.AddClaim(new Claim("role", role));
            }
        }

        private bool HasAdminRole(JObject userData)
        {
            var jtoken = userData?[this.ClaimType];
            if (jtoken is JValue)
            {
                if (jtoken?.ToString() == RoleNames.Admin)
                    return true;
            }
            else if (jtoken is JArray)
            {
                foreach (var obj in jtoken)
                    if (obj?.ToString() == RoleNames.Admin)
                        return true;
            }
            return false;
        }
    }
}