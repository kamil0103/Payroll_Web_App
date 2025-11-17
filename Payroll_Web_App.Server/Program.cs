using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Payroll_Web_App.Server.Data;
using Payroll_Web_App.Server.Models;
using Payroll_Web_App.Server.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<PayrollCalculationService>();

var jwt = builder.Configuration.GetSection("Jwt");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IPayrollCalculationService, PayrollCalculationService>();
builder.Services.AddScoped<PayrollCalculationService>(); 


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>

{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Payroll Web API", Version = "v1" });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT}",
         Reference = new OpenApiReference
         {
             Type = ReferenceType.SecurityScheme,
             Id = "Bearer"
         }
    };
    c.AddSecurityDefinition("Bearer", jwtScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});

// Add Database Connection 
// Reads "DefaultConnection" from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;   // set true behind HTTPS in prod
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

var app = builder.Build();

// Seed Default Users
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Only create if table is empty
        if (!db.Users.Any())
        {
            db.Users.AddRange(
                new AppUser
                {
                    UserName = "admin",
                    Email = "admin@local",
                    Role = "Admin",
                    IsActive = true,
                    PasswordHash = "Admin123!", // replace with hashed password
                    CreatedAt = DateTime.UtcNow
                },
                new AppUser
                {
                    UserName = "hruser",
                    Email = "hr@local",
                    Role = "HR",
                    IsActive = true,
                    PasswordHash = "Hr123!", // replace with hashed password
                    CreatedAt = DateTime.UtcNow
                },
                new AppUser
                {
                    UserName = "finuser",
                    Email = "finance@local",
                    Role = "Finance",
                    IsActive = true,
                    PasswordHash = "Finance123!", // replace with hashed password
                    CreatedAt = DateTime.UtcNow
                }
            );

            db.SaveChanges();
            Console.WriteLine(" Dev users seeded (Admin, HR, Finance).");
        }
        else
        {
            Console.WriteLine(" Users already exist — skipping seeding.");
        }
    }
}

// Static file settings 
app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payroll Web API v1");
    });
}


app.UseHttpsRedirection();

app.UseAuthentication(); 

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();




