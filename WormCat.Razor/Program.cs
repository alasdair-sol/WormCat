using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Polly;
using WormCat.Data.Data;
using WormCat.Data.DataAccess;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Services;
using WormCat.Razor.Areas.Identity.Data;
using WormCat.Razor.Data;
using WormCat.Razor.Models;

var builder = CreateBuilder(args);

ConfigureServices(builder.Configuration, builder.Services);
ConfigureIdentity(builder.Configuration, builder.Services);

var app = BuildApplication(builder);

app.Run();


// ------------------------------------------------------------------------------


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
    //var defaultConnectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    var wormCatConnectionString = config.GetConnectionString("WormCatRazorContext") ?? throw new InvalidOperationException("Connection string 'WormCatRazorContext' not found.");

    var timeoutPolicyDefault = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));
    var timeoutPolicyLong = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30));

    services.AddHttpClient<HttpServiceDefault>()
        .AddTransientHttpErrorPolicy(policyBuilder =>
        {
            // Wait and Retry 3 times, with incrementing wait periods
            return policyBuilder.WaitAndRetryAsync(3, iteration => TimeSpan.FromMilliseconds(600 * iteration));
        }).AddPolicyHandler(httpRequestMessage =>
        {
            // Set default timeout for GET requests, and longer for other requests
            if (httpRequestMessage.Method == HttpMethod.Get)
                return timeoutPolicyDefault;
            else return timeoutPolicyLong;
        });

    services.AddHttpClient<HttpServiceGoogleBooks>()
        .AddTransientHttpErrorPolicy(policyBuilder =>
        {
            // Wait and Retry 3 times, with incrementing wait periods
            return policyBuilder.WaitAndRetryAsync(3, iteration => TimeSpan.FromMilliseconds(600 * iteration));
        }).AddPolicyHandler(httpRequestMessage =>
        {
            // Set default timeout for GET requests, and longer for other requests
            if (httpRequestMessage.Method == HttpMethod.Get)
                return timeoutPolicyDefault;
            else return timeoutPolicyLong;
        });

    services.AddHttpLogging((opt) => { });

    //services.AddDbContext<ApplicationDbContext>(options =>
    //    options.UseSqlServer(defaultConnectionString));
    services.AddDatabaseDeveloperPageExceptionFilter();

    services.AddDbContext<WormCatRazorContext>(options =>
         options
         .UseLazyLoadingProxies()
         .UseSqlServer(wormCatConnectionString, m => m.MigrationsAssembly("WormCat.Data")));

    services.AddSingleton<IAuthDisplayService, AuthDisplayServiceDev>();
    services.AddSingleton<IGenericService, GenericService>();
    services.AddSingleton<IRecordUtility, RecordUtility>();

    services.AddSingleton<IEnrichedContentService, EnrichedContentServiceGoogle>();

    services.AddScoped<IUserAccess, UserAccess>();
    services.AddScoped<IRecordAccess, RecordAccess>();
    services.AddScoped<IContainerAccess, ContainerAccess>();
    services.AddScoped<IBookAccess, BookAccess>();
    services.AddScoped<ILocationAccess, LocationAccess>();

    services.AddScoped<SeedDatabase>();

    services.AddRazorPages();
}

static void ConfigureIdentity(IConfigurationManager config, IServiceCollection services)
{
    var connectionString = config.GetConnectionString("WormCatAuthContextConnection") ?? throw new InvalidOperationException("Connection string 'WormCatAuthContexstConnection' not found.");

    services.AddDbContext<WormCatAuthContext>(options => options.UseSqlServer(connectionString));

    services.AddDefaultIdentity<WormCatUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.User.RequireUniqueEmail = true;
    }).AddEntityFrameworkStores<WormCatAuthContext>();

    services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequiredLength = 4;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
    });

    services.AddAuthentication().AddGoogle(opt =>
    {
        opt.ClientId = config["Authentication:Google:ClientId"] ?? string.Empty;
        opt.ClientSecret = config["Authentication:Google:ClientSecret"] ?? string.Empty;
    });

    services.AddAuthorization(options =>
    {
        /*options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();*/
    });

    services.AddHttpContextAccessor();

    //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    //    .AddEntityFrameworkStores<ApplicationDbContext>();
}

static WebApplication BuildApplication(WebApplicationBuilder builder)
{
    var app = builder.Build();

    app.Logger.LogInformation($"App is running in [{app.Environment.EnvironmentName}] mode");

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

    app.UseHttpLogging();

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorPages();

    return app;
}