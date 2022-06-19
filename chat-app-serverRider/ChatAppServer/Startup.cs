using AutoMapper;
using ChatAppServer.Auth;
using ChatAppServer.ClientConnections;
using ChatAppServer.ClientHubs;
using ChatAppServer.DataAccess;
using ChatAppServer.DataAccess.Entities;
using ChatAppServer.Dto;
using ChatAppServer.Models;
using ChatAppServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Npgsql;

namespace ChatAppServer
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
			services.AddDbContext<ChatDbContext>(options =>
			{
				var databaseConnectionSettings = Configuration.GetSection("DatabaseConnection");

				var connectionStringBuilder = new NpgsqlConnectionStringBuilder
				{
					Host = databaseConnectionSettings["Host"],
					Database = databaseConnectionSettings["Database"],
					Username = databaseConnectionSettings["Username"],
					Password = databaseConnectionSettings["Password"],
					SslMode = SslMode.Disable,
					TrustServerCertificate = true,
					Pooling = true,
				};

				options.UseNpgsql(connectionStringBuilder.ToString());
			});

			services.AddRouting(options => options.LowercaseUrls = true);
			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChatAppServer", Version = "v1" });
			});

			services.AddCors(options =>
			{
				options.AddPolicy("AllowAllHeaders",
					builder =>
					{
						builder.AllowAnyOrigin()
							.AllowAnyHeader()
							.AllowAnyMethod()
							.WithOrigins("http://localhost:4200");
					});
			});

			services.AddSignalR();

			RegisterServices(services);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});

			if (env.IsDevelopment())
			{
				app.UseHttpsRedirection();
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatAppServer v1"));
			}

			app.UseRouting();

			app.UseCors(options =>
			{
				options.AllowAnyHeader();
				options.AllowAnyMethod();
				options.WithOrigins("http://localhost:4200");
				options.AllowCredentials();
			});

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapHub<ClientHub>("/chatshub");
			});
		}

		private void RegisterServices(IServiceCollection services)
		{
			var autoMapper = ConfigureAutoMapper();
			services.AddSingleton(autoMapper);

			services.AddSingleton<IClientConnectionsCache, ClientConnectionsCache>();
			// services.AddSingleton<IGoogleClientSecretsProvider>(new GoogleClientSecretsProvider(Configuration));
			services.AddScoped<IUsersService, UsersService>();
			services.AddScoped<IChatsService, ChatsService>();
			services.AddScoped<IMessagesService, MessagesService>();
		}

		private static IMapper ConfigureAutoMapper()
		{
			var config = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<User, UserEntity>();
				cfg.CreateMap<UserEntity, User>();
				cfg.CreateMap<GoogleUserInfo, User>();
				cfg.CreateMap<UserEntity, UserDto>();
				cfg.CreateMap<UserDto, UserEntity>();
			});

			var mapper = config.CreateMapper();
			return mapper;
		}
	}
}
