using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using TraceIpWebApi.Repositories;
using TraceIpWebApi.Service;

namespace TraceIpWebApi
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

            //services.AddDistributedRedisCache((options) => {
            //    IConfigurationSection section = Configuration.GetSection("RedisDB");
            //    ConfigurationOptions config = new ConfigurationOptions
            //    {
            //        EndPoints =
            //        {
            //            { section["Location"], Convert.ToInt32(section["Port"]) },
            //        },
            //        AbortOnConnectFail = false
            //    };
            //    options.ConfigurationOptions = config;
            //});

            IConfigurationSection section = Configuration.GetSection("RedisDB");
            string connectionString = section["Location"] + ":" + section["Port"];
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionString));
            services.AddSingleton<ITraceIpCache, TraceIpCache>();
            services.AddSingleton<IStatsCache, StatsCache>();
            services.AddScoped<ITraceIpService, TraceIpService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
