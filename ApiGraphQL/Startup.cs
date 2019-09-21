using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGraphQL.GraphQL;
using ApiGraphQL.GraphQL.Schemas;
using ApiGraphQL.Repository;
using ApiGraphQL.Repository.Interfaces;
using DataAccess.Models;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Server.Ui.Voyager;
using GraphQL.Validation.Complexity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApiGraphQL
{
	public class Startup
	{
		public Startup(IConfiguration configuration, IHostingEnvironment environment)
		{
			this.Configuration = configuration;
			this.Environment = environment;
		}

		public IConfiguration Configuration { get; }
		public IHostingEnvironment Environment { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			var conString = this.Configuration.GetConnectionString("ZolikEntities");
			services.AddScoped(x => new ZoliksEntities(conString))
					.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService))
					.SetProjectRepositories()
					.SetSchemas()
					.AddGraphQL(o => {
						o.ExposeExceptions = this.Environment.IsDevelopment();
						o.ComplexityConfiguration = new ComplexityConfiguration() {
							MaxDepth = 15
						};
					})
					.AddGraphTypes(ServiceLifetime.Scoped)
					.AddDataLoader();

			services.AddAuthentication(x => {
				x.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				x.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			} else {
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection()
			   .UseAuthentication()
			   .UseGraphQL<AppSchema>()
			   .UseGraphQLPlayground(new GraphQLPlaygroundOptions {Path = "/"})
			   .UseGraphQLVoyager(new GraphQLVoyagerOptions {Path = "/voyager"});

			app.UseMvc();
		}
	}
}