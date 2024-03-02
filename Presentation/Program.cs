using BusinessLogic;
using BusinessLogic.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();

// Register your service
builder.Services.AddScoped<IRandomUserService, RandomUserService>();

builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Use detailed error pages in development.
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Use a custom error handling page in production.
    app.UseHsts(); // Enforce HTTPS in production.
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
