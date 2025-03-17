using eLearnApps.Business;
using eLearnApps.Business.Interface;
using eLearnApps.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace eLearnApps
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IAppSettingService, AppSettingService>();
            builder.Services.AddKeyedScoped(typeof(IRepository<>), "default", typeof(Repository<>));
            builder.Services.AddKeyedScoped(typeof(IRepository<>), "lms", typeof(LmsRepository<>));
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DataContext")));
            builder.Services.AddScoped<IDbContext, DataContext>();
            builder.ConfigureAuth();


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

            app.UseAuthorization();

            app.MapControllerRoute(
           name: "default",
           pattern: "{controller}/{action}/{id?}",
           defaults: new { controller = "Account", action = "LTIVIEW" });

            app.Run();
        }
    }
}
