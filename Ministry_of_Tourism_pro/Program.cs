using Microsoft.AspNetCore.Authentication.Cookies;
using Ministry_of_Tourism_pro.Application.Interfaces;
using Ministry_of_Tourism_pro.Application.Services;
using Ministry_of_Tourism_pro.Common;
using Ministry_of_Tourism_pro.WebConstants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddHttpContextAccessor();

// Register HttpClient factory for API communication
builder.Services.AddHttpClient("mainclient", client =>
{
    var baseUrl = builder.Configuration["CnetApiBaseUrl"];
    client.BaseAddress = new Uri(baseUrl ?? "http://196.191.244.147:4520/api/");
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

// Register application infrastructure services
builder.Services.AddScoped<SharedHelpers>();
builder.Services.AddScoped<AuthenticationManager>();

//// Register mock authentication
builder.Services.AddAuthentication(CNET_WebConstantes.CookieScheme)
    .AddCookie(CNET_WebConstantes.CookieScheme, options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = CNET_WebConstantes.CookieScheme;
    });

// Register Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register application services
builder.Services.AddScoped<IHotelService, HotelService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}");

app.Run();
