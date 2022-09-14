using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NonFactors.Mvc.Grid;
using Plataforma.Data;
using Plataforma.Helpers;
using Plataforma.Helpers.Identity;
using Plataforma.Infrastructure;
using Plataforma.Models.Identity;
using Plataforma.Services.Components;
using Plataforma.Services.Components.FlashMessage;
using Plataforma.Services.Components.Razor;
using Plataforma.Services.Components.Repositories;
using Plataforma.Services.Contracts.FlashMessage;
using Plataforma.Services.Contracts.Razor;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

var builder = WebApplication.CreateBuilder(args);

var isProduction = builder.Environment.IsProduction();

var contentRootPath = builder.Environment.ContentRootPath;
var wwwRootPath = Path.Combine(contentRootPath, "wwwroot\\");

// Load configurations file settings
var cookieDomain = builder.Configuration.GetSection("Configurations").GetValue<string>("Domain");
cookieDomain = (cookieDomain.StartsWith("localhost") || cookieDomain.StartsWith("192")) ? "" : string.Concat(".", cookieDomain);
var cookieTime = TimeSpan.FromMinutes(builder.Configuration.GetSection("Configurations").GetValue<int>("LoginTime"));
var cookieSecure = builder.Configuration.GetSection("Configurations").GetValue<bool>("RedirectHttps") ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
var maxFileSizeBytes = builder.Configuration.GetSection("Configurations").GetValue<int>("MaxFileSizeMb") * 1024 * 1024;
var azureAd = builder.Configuration.GetSection("Configurations").GetSection("AzureAd");
var azureClientId = azureAd.GetValue<string>("ClientId");
var azureClientSecret = azureAd.GetValue<string>("ClientSecret");
var azureTenantId = azureAd.GetValue<string>("TenantId");

var services = builder.Services;


// Register the EF application database context using proxies for lazy loading
services.AddDbContext<ApplicationDbContext>(options => {
    if (!isProduction)
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
        //options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
    }
    //options.ConfigureWarnings(warnings => warnings.Ignore(20606, 20704, 10400));
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), o => {
        o.CommandTimeout(3000);
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    })
    .UseLazyLoadingProxies();
});

// Rewrite urls as lowercase
services.AddRouting(options => options.LowercaseUrls = true);

services.AddHttpClient();
services.AddHttpContextAccessor();

services.AddSingleton(_ => new ConfigurationsService(builder.Configuration, contentRootPath, wwwRootPath));

services.AddTransient<HttpContextAccessor>();
services.AddTransient<EmailService>();
services.AddTransient<RedirectService>();
services.AddTransient<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
services.AddTransient<IFlashMessage, FlashMessage>();
services.AddTransient<IFlashMessageSerializer, FlashMessageSerializer>();


services.AddScoped<SettingsRepository>();
services.AddScoped<UsersRepository>();

services.AddAutoMapper(_ => { }, typeof(Program));



// Adds Identity
services
    .AddIdentity<User, IdentityRole<int>>(options => {
        options.SignIn.RequireConfirmedAccount = true;
        options.User.RequireUniqueEmail = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>() // Tells Identity to fetch the users from this database
    .AddClaimsPrincipalFactory<ClaimsPrincipalFactory>()
    .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider)
    .AddErrorDescriber<TranslationsHelper.CustomIdentityErrorDescriber>();

// Adds session for storage
services.AddDistributedMemoryCache();
services.AddSession(options => {
    options.Cookie.Name = "SessionCookie";
    options.Cookie.Domain = cookieDomain;
    options.Cookie.SecurePolicy = cookieSecure;
    options.IdleTimeout = cookieTime;
    options.Cookie.IsEssential = true;
});

// Adds controllers
if (isProduction)
    services
        .AddControllersWithViews()
        .AddNewtonsoftJson(options => { options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; });
else
    services
        .AddControllersWithViews()
        .AddNewtonsoftJson(options => { options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; })
        .AddRazorRuntimeCompilation();


// Anti forgery settings
services.AddAntiforgery(options => {
    options.FormFieldName = "AntiForgery";
    options.HeaderName = "X-CSRF-TOKEN";
    options.SuppressXFrameOptionsHeader = false;
    options.Cookie.Name = "AntiForgeryCookie";
    options.Cookie.Expiration = cookieTime;
    options.Cookie.Domain = cookieDomain;
    options.Cookie.SecurePolicy = cookieSecure;
});

// Cookie for temp data
services.Configure<CookieTempDataProviderOptions>(options => {
    options.Cookie.Name = "TempDataCookie";
    options.Cookie.Expiration = cookieTime;
    options.Cookie.Domain = cookieDomain;
    options.Cookie.SecurePolicy = cookieSecure;
});

// Configures the Identity by cookie
services.ConfigureApplicationCookie(options => {
    options.ExpireTimeSpan = cookieTime;
    options.Cookie.Name = "AuthCookie";
    options.Cookie.Domain = cookieDomain;
    options.Cookie.SecurePolicy = cookieSecure;
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = ctx => {
            var back = ctx.Request.GetEncodedPathAndQuery();
            if (back == "/")
                back = "";
            back = $"?redirect={HttpUtility.UrlEncode(back)}";
            ctx.Response.Redirect($"/auth/login{back}");
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = ctx => {
            ctx.Response.Redirect("/auth/accessdenied");
            return Task.CompletedTask;
        }
    };
});


// Password security
services.Configure<IdentityOptions>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 2;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});

// Create an access policy for each claim
services.AddAuthorization(options => {
    // Creates one policy for each claim
    foreach (var claim in ClaimStore.ClaimGroups(false).SelectMany(claimGroup => claimGroup.Claims).Where(c => !string.IsNullOrEmpty(c.Type)))
        options.AddPolicy(claim.Type, policy => {
            policy.RequireClaim(claim.Type); //Enforce the policy
        });
});

services.AddMvcGrid(filters => {
    filters.BooleanFalseOptionText = () => "Não";
    filters.BooleanTrueOptionText = () => "Sim";
    filters.BooleanEmptyOptionText = () => "";
});

services.AddMvc(options => {
    TranslationsHelper.TranslateValidations(ref options);
}).AddDataAnnotationsLocalization().AddViewLocalization();

// IIS & forms
services.Configure<IISServerOptions>(options => {
    options.MaxRequestBodySize = maxFileSizeBytes;
});
services.Configure<FormOptions>(options => {
    options.ValueLengthLimit = maxFileSizeBytes;
    options.MultipartBodyLengthLimit = maxFileSizeBytes;
});

services.AddControllersWithViews();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var configurationsService = scope.ServiceProvider.GetRequiredService<ConfigurationsService>();

var provider = new FileExtensionContentTypeProvider
{
    Mappings = {
        [".webmanifest"] = "application/manifest+json"
    }
};

// Use static files
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider,
    FileProvider = new PhysicalFileProvider(wwwRootPath),
    OnPrepareResponse = ctx => {
        ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=604800");
    }
});

// Uses routes
app.UseRouting();

// Uses authentication
app.UseAuthentication();

// Use authorization
app.UseAuthorization();

// Uses session
app.UseSession();

// Session handler
app.UseMiddleware<SessionHandler>();


if (isProduction)
{
    app.UseHsts();

    // Error Handling
    app.UseStatusCodePages();
    app.UseMiddleware<ErrorHandler>();

    // Redirect options (www and https)
    var rewriteOptions = new RewriteOptions();
    if (configurationsService.RedirectWww)
        rewriteOptions.AddRedirectToWwwPermanent();
    else
        rewriteOptions.AddRedirectToNonWwwPermanent();

    if (configurationsService.RedirectHttps)
    {
        app.UseHttpsRedirection();
        rewriteOptions.AddRedirectToHttpsPermanent();
    }

    app.UseRewriter(rewriteOptions);
}
else
{
    app.UseDeveloperExceptionPage();
}

// Anti forgery validator
app.UseMiddleware<AntiForgeryHandler>();

app.UseMiddleware<RedirectHandler>();

// Map routes
app.UseEndpoints(endpoints => {
    endpoints.MapDefaultControllerRoute();
    endpoints.MapControllers();
});

app.Run();