using System.IO.Compression;
using Hydro.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Plunger.Data;
using Plunger.Middlewares;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is(
        builder.Environment.IsDevelopment() ? LogEventLevel.Debug : LogEventLevel.Information
    )
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog(dispose: true);
builder.Services.AddWebOptimizer(pipeline =>
{
    pipeline.AddCssBundle("~/bundles/css/styles", "/lib/**/*.css");
    pipeline.AddJavaScriptBundle(
        "~/bundles/js/scripts/preload",
        "/lib/jquery/jquery.js",
        "/js/preload/**/*.js"
    );

    var excludeFilters = new List<Func<string, bool>>
    {
        file => Path.GetFileName(file).StartsWith("jquery", StringComparison.OrdinalIgnoreCase),
        // file => Path.GetFileName(file).EndsWith(".min.js", StringComparison.OrdinalIgnoreCase)
    };

    var jsFiles = Directory
        .GetFiles("wwwroot/lib", "*.js", SearchOption.AllDirectories)
        .Where(file => !excludeFilters.Any(filter => filter(file)))
        .Select(file => file.Replace("wwwroot", "").Replace("\\", "/"));

    pipeline.AddJavaScriptBundle("~/bundles/js/scripts", jsFiles.ToArray());
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
builder.Services.AddAuthorization();
builder.Services.AddIdentity<PlungerUser, PlungerRole>().AddEntityFrameworkStores<AuthDbContext>();
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
    // Configure the HTTP request pipeline.
    app.UseExceptionHandler("/Error");
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
