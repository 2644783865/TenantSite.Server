using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlonsoAdmin.HttpApi.Init.Middleware;
using AlonsoAdmin.HttpApi.Init.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AlonsoAdmin.HttpApi.Init
{
    public class Startup
    {
        readonly string _allowSpecificOrigins = "_allowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(_allowSpecificOrigins, builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                //.WithMethods("GET", "POST", "HEAD", "PUT", "DELETE", "OPTIONS")
                );
                //.AllowAnyMethod()
                //.AllowAnyHeader()
                //.AllowCredentials());
            });

            services.AddControllers()  
             // �趨json���л�����
            .AddNewtonsoftJson(options =>
            {
                // ����ѭ������
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                // ʹ��С�շ�
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); // new DefaultContractResolver();
                // ����ʱ���ʽ
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";

            });

            services.AddSingleton<ISettingService, SettingService>();
            services.AddSwaggerGen(c =>
            {
                // ����ĵ���Ϣ
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tenant Site Developer Tools Apis", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tenant Site Developer Tools Apis");
                //���ݰ汾���Ƶ��� ����չʾ

                //c.RoutePrefix = "";//ֱ�Ӹ�Ŀ¼����
                //c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);//�۵�Api
            });

            // ȫ���쳣����
            app.UseErrorHandlingMiddleware();

            // ������
            app.UseCors(_allowSpecificOrigins);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
