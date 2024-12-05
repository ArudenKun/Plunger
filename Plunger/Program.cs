using System.IO.Compression;
using Hydro.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Plunger.Data;
using Plunger.Middlewares;
using Plunger.Services.Hosted;
using Serilog;
using Serilog.Events;
using WebOptimizer;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is(
        builder.Environment.IsDevelopment() ? LogEventLevel.Debug : LogEventLevel.Information
    )
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {ClassName}] {Message:lj}{NewLine}{Exception}"
    )
    .Enrich.FromLogContext()
    .Enrich.WithClassName()
    .CreateLogger();

builder.Host.UseSerilog(dispose: true);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHostedService<TailwindService>();
}

builder.Services.AddWebOptimizer(pipeline =>
{
    pipeline
        .AddCssBundle("~/bundles/css/styles", "/**/*.css")
        .ExcludeFiles("/**/*.min.css", "/**/tailwind.source.css");
    pipeline.AddJavaScriptBundle(
        "~/bundles/js/scripts/preload",
        "/lib/jquery/jquery.js",
        "/js/preload/**/*.js"
    );
    pipeline
        .AddJavaScriptBundle("~/bundles/js/scripts", "/lib/**/*.js")
        .ExcludeFiles("/**/*.min.js", "/**/jquery*.js");
});
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.Duration;
    logging.CombineLogs = true;
});
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
);
builder.Services.AddAntiforgery();
builder.Services.AddRazorPages();
builder.Services.AddHydro(options => options.AntiforgeryTokenEnabled = true);
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo("../"));
builder
    .Services.AddIdentityCore<PlungerUser>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddSignInManager();
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
        options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddBearerToken()
    .AddCookie(
        IdentityConstants.ApplicationScheme,
        options =>
        {
            options.LoginPath = "/account/login";
            options.LogoutPath = "/account/logout";
            options.AccessDeniedPath = "/account/denied";
            options.ReturnUrlParameter = "returnUrl";
            options.Events = new CookieAuthenticationEvents
            {
                OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync,
            };
        }
    )
    .AddExternalCookie();
builder.Services.AddAuthorization();
builder.Services.AddResponseCompression(options => options.EnableForHttps = true);
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    options.Level = CompressionLevel.Optimal
);
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    options.Level = CompressionLevel.Optimal
);
builder.Services.Configure<RouteOptions>(option =>
{
    option.LowercaseUrls = true;
    option.LowercaseQueryStrings = true;
});

var app = builder.Build();
app.UseWebOptimizer();
if (app.Environment.IsProduction())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseResponseCompression();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseExceptionHandler(b => b.UseMiddleware<HydroExceptionMiddleware>());
app.UseStaticFiles();
app.UseHttpLogging();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHydro(builder.Environment);
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.Run();
