using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fiver.Security.AuthServer;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SVM.Enterprise.Users.Infrastructure.IdentityServer;

namespace IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
          //  services.AddIdentity<ApplicationUser, IdentityRole>();
            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.Caching.ClientStoreExpiration = TimeSpan.FromHours(1);
                options.Caching.ResourceStoreExpiration = TimeSpan.FromHours(1);
            })
           .AddDeveloperSigningCredential()
           .AddCorsPolicyService<IdentityCorsPolicyService>()

                       //.AddInMemoryApiResources(Config.GetApiResources())
                       //.AddInMemoryIdentityResources(Config.GetIdentityResources())
                       //.AddInMemoryClients(Config.GetClients())
                       //.AddTestUsers(Config.GetUsers());


          //  builder.Services.AddTransient<IUserClaimsPrincipalFactory, UserClaimsPrincipalFactory>();
            .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
            .AddProfileService<ProfileService>()
            .AddResourceStore<DbResourceStore>()
            .AddClientStore<ClientStore>();
        }

        //public static IIdentityServerBuilder AddUserStore(this IIdentityServerBuilder builder)
        //{
        //    builder.Services.AddTransient<IUserClaimsPrincipalFactory, UserClaimsPrincipalFactory>();
        //    builder.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();
        //    builder.AddProfileService<ProfileService>();
        //    builder.AddResourceStore<DbResourceStore>();
        //    builder.AddClientStore<ClientStore>();

        //    return builder;
        //}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseIdentityServer();
            app.UseMvc();
        }
    }

    public class IdentityCorsPolicyService : ICorsPolicyService
    {
        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            return Task.FromResult(true);
        }
    }
}
