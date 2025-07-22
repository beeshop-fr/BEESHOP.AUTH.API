using BEESHOP.AUTH.API.Config;
using BEESHOP.AUTH.PERSISTENCE;
using BEESHOP.AUTH.PERSISTENCE.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddPersistenceServices();
builder.Services.AddKeycloakAuthentication();
builder.Services.Configure<KeycloakConfig>(builder.Configuration.GetSection("Keycloak"));


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Swagger uniquement en dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Active les Controllers
app.MapControllers();

app.Run();
