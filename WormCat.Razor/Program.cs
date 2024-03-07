using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WormCat.Razor.Data;
using WormCat.Data.Data;
using WormCat.Razor.Models;
using WormCat.Library.Utility;

var builder = CreateBuilder(args);

ConfigureServices(builder.Configuration, builder.Services);
ConfigureIdentity(builder.Configuration, builder.Services);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<SeedDatabase>().TrySeed();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

static WebApplicationBuilder CreateBuilder(string[] args)
{
    var builder = WebApplication.CreateBuilder();

    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();

    return builder;
}

static void ConfigureServices(IConfigurationManager config, IServiceCollection services)
{
    // Add services to the container.
    var defaultConnectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    var wormCatConnectionString = config.GetConnectionString("WormCatRazorContext") ?? throw new InvalidOperationException("Connection string 'WormCatRazorContext' not found.");

    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(defaultConnectionString));
    services.AddDatabaseDeveloperPageExceptionFilter();

    services.AddDbContext<WormCatRazorContext>(options =>
         options
         .UseLazyLoadingProxies()
         .UseSqlServer(wormCatConnectionString, m => m.MigrationsAssembly("WormCat.Data")));

    services.AddSingleton<IAuthDisplayUtility, AuthDisplayUtilityDev>();
    services.AddSingleton<IGenericUtility, GenericUtility>();

    services.AddScoped<SeedDatabase>();
    services.AddRazorPages();
}

static void ConfigureIdentity(ConfigurationManager configuration, IServiceCollection services)
{
    services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequiredLength = 4;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
    });

    services.AddAuthentication();
    services.AddAuthorization(options =>
    {
        //options.FallbackPolicy = new AuthorizationPolicyBuilder()
        //    .RequireAuthenticatedUser()
        //    .Build();
    });

    services.AddHttpContextAccessor();

    services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>();
}