using AutoMapper;
using drug_store_api.data;
using drug_store_api.entities.PurchaseOrders;
using drug_store_api.entities.PurchaseRequests;
using drug_store_api.entities.Users;
using drug_store_api.repositories;
using drug_store_api.services;
using drug_store_api.systemcommon.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


builder.Services.AddDbContext<DrugStoreDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
    {
        npgsqlOptions.MapEnum<UserRole>("user_role");
        npgsqlOptions.MapEnum<PurchaseOrderStatusEnum>("purchase_order_status_enum");
        npgsqlOptions.MapEnum<PurchaseRequestStatusEnum>("purchase_request_status_enum");
    }));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DI for Repository and Service
builder.Services.AddRepositories();
builder.Services.AddServices();

builder.Services.AddSingleton(provider =>
{
    var config = new MapperConfiguration(cfg =>
    {
        cfg.AddMaps(typeof(MappingProfile).Assembly);
    });
    return config.CreateMapper();
});

//Config JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");

var DrugStoreAllowSpecificOrigins = "_drugStoreOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: DrugStoreAllowSpecificOrigins,
        policy =>
        {
            var origins = new[]
            {
                "http://localhost:8081",
                "http://localhost:8080"
            };

            policy.WithOrigins(origins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]!)),
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(DrugStoreAllowSpecificOrigins);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
