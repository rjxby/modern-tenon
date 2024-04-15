using ModernTenon.Api.Host;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRepositoryLayer(builder.Configuration);

builder.Services.AddServiceLayer();

builder.Services.AddPresentationLayer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerSetup();
}
else
{
    // issue require us to have dummy delegate for middleware - https://github.com/dotnet/aspnetcore/issues/51888
    // monitor the issue why exception handler does not triggers on development environment - https://github.com/dotnet/aspnetcore/issues/52622
    app.UseExceptionHandler(o => { });
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapProductsEndpoints();

await app.RunAsync();

public partial class Program { }