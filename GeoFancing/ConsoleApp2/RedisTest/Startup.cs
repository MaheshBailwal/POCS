using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RedisTest
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
            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDistributedRedisCache(options =>
            {
                // These settings are to connect with Azure VM
                options.Configuration = "my-redis-cache.redis.cache.windows.net:6380,password=GC+dMrF3dWQ+FSgN9Wptie2XLOpjc+Fej3GJgezMIQg=,ssl=True,abortConnect=False";
                options.InstanceName = "my-redis-cache.redis.cache.windows.net";

                // These settings are to connect with Redis installed on VM
                //options.Configuration = appSettings.RedisCacheConfig;
                //options.InstanceName = "OnPrimRedis";
            });

            services.AddSingleton<RedisCache, RedisCache>();
            services.AddSingleton<FileSystemCache, FileSystemCache>();
            services.AddSingleton<InMemoryCache, InMemoryCache>();
            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
