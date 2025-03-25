using CellPhoneB_Store.Models;
using CellPhoneB_Store.Respository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CellPhoneB_Store
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var builderRazor = builder.Services.AddRazorPages();

            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
            });

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			builder.Services.AddDistributedMemoryCache();

			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30);
				options.Cookie.IsEssential = true;
			});

			builder.Services.AddIdentity<AppUserModel,IdentityRole>().AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

			builder.Services.Configure<IdentityOptions>(options =>
			{
				// Password settings.
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 4;

				options.User.RequireUniqueEmail = true;
			});

			var app = builder.Build();

            app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");
            app.UseSession();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                builderRazor.AddRazorRuntimeCompilation();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllerRoute(
	name: "category",
	pattern: "/category/{Slug?}",
	defaults: new { controller = "Category", action = "Index" });

			app.MapControllerRoute(
				name: "brand",
				pattern: "/brand/{Slug?}",
				defaults: new { controller = "Brand", action = "Index" });

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.MapControllerRoute(
				name: "Areas",
				pattern: "{areas:exists}/{controller=Product}/{action=Index}/{id?}");


			//Seeding data
			var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
            SeedData.SeedingData(context);

            app.Run();
        }
    }
}
