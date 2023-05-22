using Sustainability.Deloitte.com.Helpers;
using Sustainability.Deloitte.com.Services;

var builder = WebApplication.CreateBuilder(args);
var services = new ServiceCollection();
// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IAuthentication, Authentication>();
builder.Services.AddSingleton<IResourceGroupService, ResourceGroupService>();
var app = builder.Build();

//services
//    .RegisterType<Authentication>()
//    .As<IAuthentication>()
//    .SingleInstance();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
