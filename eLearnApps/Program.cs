using DocumentFormat.OpenXml.InkML;
using eLearnApps.Business;
using eLearnApps.Business.Interface;
using eLearnApps.Core.Caching;
using eLearnApps.Data;
using eLearnApps.Data.Interface;
using eLearnApps.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Net;

namespace eLearnApps
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20); // Set session timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add services to the container.

            builder.Services.AddMemoryCache();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddScoped<ICacheManager, MemoryCacheManager>();

            builder.Services.AddScoped<IDaoFactory, DaoFactory>();

            builder.Services.AddScoped<IAppSettingService, AppSettingService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ILoggingService, LoggingService>();
            builder.Services.AddScoped<IAuditLogService, AuditLogService>();
            builder.Services.AddScoped<IGPTAuditLogService, GPTAuditLogService>();
            builder.Services.AddScoped<IGPTDebugLogService, GPTDebugLogService>();
            builder.Services.AddScoped<IBatchJobLogService, BatchJobLogService>();
            builder.Services.AddScoped<IBatchJobLogDetailService, BatchJobLogDetailService>();
            builder.Services.AddScoped<ICourseCategoryService, CourseCategoryService>();
            builder.Services.AddScoped<ICourseTemplateService, CourseTemplateService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<IErrorLogService, ErrorLogService>();
            builder.Services.AddScoped<IUserEnrollmentService, UserEnrollmentService>();
            builder.Services.AddScoped<ISemesterService, SemesterService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IUserGroupService, UserGroupService>();
            builder.Services.AddScoped<IPermissionRoleService, PermissionRoleService>();
            builder.Services.AddScoped<IToolAccessLogService, ToolAccessLogService>();
            builder.Services.AddScoped<ICategoryGroupService, CategoryGroupService>();
            builder.Services.AddScoped<IAuditService, AuditService>();

            builder.Services.AddScoped<IValenceService, ValenceService>();



            builder.Services.AddKeyedScoped(typeof(IRepository<>), "default", typeof(Repository<>));
            builder.Services.AddKeyedScoped(typeof(IRepository<>), "lms", typeof(LmsRepository<>));
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DataContext")));
            builder.Services.AddDbContext<LMSIsisContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("LMSIsisContext")));
            builder.Services.AddScoped<IDbContext, DataContext>();
            builder.ConfigureAuth();
            builder.Services.AddControllers();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuth();
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=LTIVIEW}/{id?}");


            app.MapControllers(); // For API routes
            using (var scope = app.Services.CreateScope())
            {
                var permissionRoleService = scope.ServiceProvider.GetRequiredService<IPermissionRoleService>();
                permissionRoleService.ResetPermissionRole();
                var endpointDataSource = scope.ServiceProvider.GetRequiredService<EndpointDataSource>();
                foreach (var endpoint in endpointDataSource.Endpoints)
                {
                    Console.WriteLine($"Route: {endpoint.DisplayName}");
                }

            }

            app.Run();
        }
    }
}
