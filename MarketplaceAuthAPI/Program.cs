using System.Text;
using DAL.Context;
using DAL.Repository;
using DAL.Repository.Interface;
using Domain.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

        builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        ConfigureServices(builder);

        var app = builder.Build();

// Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        //JWT and Identity Configuration
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }
        ).AddJwtBearer(options =>
        {
            //Only for development
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
                ValidAudience = builder.Configuration["JwtConfig:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"])),
                
            };
        });
        builder.Services.AddAuthorization();
        
        //add swagger configuration for JWT
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "MarketplaceAuthAPI", Version = "V1" });
            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "ENTER 'Bearer':"
            });

            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id ="Bearer"
                        }
                    },
                    new string[]{ }
                }
            });

        });
        
        //add DbContext
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MarketplaceAuthDb")));
        
        //add identity
        builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
        
        //register services
        //repository
        builder.Services.AddScoped<IGenericRepository<MarketplaceUser>, MarketplaceUserRepository>();
        builder.Services.AddScoped<IGenericRepository<MarketplaceShop>, MarketplaceShopRepository>();
        builder.Services.AddScoped<IGenericRepository<MarketplaceAdmin>, MarketplaceAdminRepository>();
    }
}
