using FinalBanco.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

//generamos las opciones del dbContext
builder.Services.AddDbContext<MyContext>(options =>
{   //le decimos que use sqlserver y que el builder use la conexcion context
    options.UseSqlServer(builder.Configuration.GetConnectionString("context"));

});

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDistributedMemoryCache(); // Usamos el proveedor de cach� de memoria

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(3); // Tiempo de expiraci�n de la sesi�n
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
/*Este c�digo configura el servicio de sesi�n 
* para usar el proveedor de cach� de memoria,
* establece un tiempo de expiraci�n de sesi�n de 3 minutos 
* y establece algunas opciones de cookie.*/






var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


//agregado
app.UseSession();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
