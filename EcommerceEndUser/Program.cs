using Ecommerce;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_myAllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("https://localhost:44327");
        });
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddControllersWithViews();
builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue;
});
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});
builder.Services.AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
string connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<EcommerceDbContext>(options => options
    .UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseCors("_myAllowSpecificOrigins");

app.UseSession();

app.UseCookiePolicy();

app.UseHttpsRedirection();
app.UseStaticFiles();

var provider = new FileExtensionContentTypeProvider();
app.UseRouting();

app.UseAuthorization();
app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=HomeEndUser}/{action=Index}/{id?}");
app.Run();
