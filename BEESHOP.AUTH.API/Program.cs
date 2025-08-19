using BEESHOP.AUTH.API.Config;
using BEESHOP.AUTH.PERSISTENCE;
using BEESHOP.AUTH.PERSISTENCE.Config;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez votre token JWT ci-dessous (ex: Bearer eyJ...)",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddPersistenceServices();
builder.Services.AddKeycloakAuthentication();
builder.Services.AddScoped<IClaimsTransformation, KeycloakRolesClaimsTransformer>();
builder.Services.AddAuthorization();
builder.Services.Configure<KeycloakConfig>(builder.Configuration.GetSection("Keycloak"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("using BEESHOP.AUTH.API.Config;
using BEESHOP.AUTH.PERSISTENCE;
using BEESHOP.AUTH.PERSISTENCE.Config;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez votre token JWT ci-dessous (ex: Bearer eyJ...)",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddPersistenceServices();
builder.Services.AddKeycloakAuthentication();
builder.Services.AddScoped<IClaimsTransformation, KeycloakRolesClaimsTransformer>();
builder.Services.AddAuthorization();
builder.Services.Configure<KeycloakConfig>(builder.Configuration.GetSection("Keycloak"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://51.75.140.195:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


var app = builder.Build();

app.UseCors("AllowLocalhost3000");

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
")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


var app = builder.Build();

app.UseCors("AllowLocalhost3000");

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
