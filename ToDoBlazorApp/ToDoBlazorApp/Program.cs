using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// регистрируем Razor Pages и Blazor Server
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// если тебе нужен HttpClient внутри компонентов:
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["AppBaseAddress"]
                          ?? "https://localhost")
});

var app = builder.Build();

// стандартный пайплайн для Blazor Server
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
