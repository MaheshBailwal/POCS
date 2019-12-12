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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDistributedRedisCache(options =>
            {
                //options.Configuration = "svm-rdscache-qa-us.redis.cache.windows.net:6380,password=oPcROQMGT7fngwuq2E4UFQIB4TDdMq5ikafehYosXwM=,ssl=True,abortConnect=False";
                //options.InstanceName = "svm-rdscache-qa-us.redis.cache.windows.net";

                options.Configuration = "my-redis-cache.redis.cache.windows.net:6380,password=GC+dMrF3dWQ+FSgN9Wptie2XLOpjc+Fej3GJgezMIQg=,ssl=True,abortConnect=False";
                options.InstanceName = "my-redis-cache.redis.cache.windows.net";
            });

            services.AddSingleton<RedisCache, RedisCache>();

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
