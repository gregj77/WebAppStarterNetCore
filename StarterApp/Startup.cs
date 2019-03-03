using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReferenceModule;
using StarterApp.Configuration;
using Utils;
using Validation;
using Module = Autofac.Module;

namespace StarterApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IContainer AppContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddFluentValidation(fv =>
                {
                    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                    fv.ValidatorFactory = AutofacValidatorFactory.Instance;
                })
                .AddMvcOptions(o =>
                {
                    o.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                    o.InputFormatters.Add(new XmlDataContractSerializerInputFormatter(o));
                });

            AutoMapper.Mapper.Initialize(cfg =>
            {
                //cfg.AddProfile();
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var contextBuilder = new DbContextOptionsBuilder();
            if (bool.Parse(Configuration["UseInMemoryDB"]))
            {
                contextBuilder.UseInMemoryDatabase("test");
            }

            contextBuilder.EnableDetailedErrors(true);
            builder.RegisterInstance(contextBuilder.Options);

            builder.RegisterModule(new MainWebModule());
            builder.RegisterModule(new UtilsModule());
            builder.RegisterModule(new ValidationModule());

            foreach (var module in GetApplicationModules())
            {
                builder.RegisterModule(module);
            }

            builder.Register(ctx => AppContainer);

            builder.RegisterBuildCallback(c => AppContainer = c);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            //loggerFactory.AddProvider();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseExceptionHandler();
            }

            app.UseStatusCodePages();
            app.UseHttpsRedirection();
            app.UseMvc();
        }

        protected IEnumerable<Module> GetApplicationModules()
        {
            yield return new ReferenceModuleConfiguration();
        }
    }
}
