using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using SupabaseApiCall.Infrastructure;
using SupabaseApiCall.Repositories;
using SupabaseApiCall.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
// Tramite questa funzione vado a configurare il client Supabase che sarà un Singleton nell'app
// Di volta in volta questo viene iniettato nei repository/servizi che ne hanno bisogno
builder.Services.AddSingleton(supabaseService =>
{
    var url = builder.Configuration["Supabase:Url"] ?? throw new InvalidOperationException("Url errato o assente");
    var key = builder.Configuration["Supabase:Key"] ?? throw new InvalidOperationException("Key errata o assente");
    var options = new Supabase.SupabaseOptions { AutoConnectRealtime = true };
    return new Supabase.Client(url, key, options);
});
//JWT Validator
builder.Services.AddJwtValidator(builder.Configuration);
// Init async all'avvio 
builder.Services.AddHostedService<SupabaseInitializer>();

// App layers
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IArticoloRepository, ArticoloRepository>();
builder.Services.AddScoped<IFatturaClienteRepository, FatturaClienteRepository>();
builder.Services.AddScoped<IFatturaFornitoreRepository, FatturaFornitoreRepository>();

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IArticoloService, ArticoloService >();
builder.Services.AddScoped<IFattureService, FattureService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
    app.MapOpenApi();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();
