using ApiRestAlchemy.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.Text;
using ApiRestAlchemy.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


var configuration = builder.Services.BuildServiceProvider()
                                    .GetRequiredService<IConfiguration>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DatabaseContext>
    (options => { options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseContext")); });

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                  .AddEntityFrameworkStores<DatabaseContext>()
                 .AddDefaultTokenProviders();

builder.Services.AddTransient<IMailService, SendGridMailService>();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; x.DefaultChallengeScheme =
                                            JwtBearerDefaults.AuthenticationScheme;
}).AddCookie(x => { x.Cookie.Name = "token"; }).AddJwtBearer(x => {
    x.RequireHttpsMetadata = false;
    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context => { context.Token = context.Request.Cookies["token"]; return Task.CompletedTask; }
    };
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "localhost",
        ValidAudience = "localhost",
        IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Llave_super_secreta"])),
        ClockSkew = TimeSpan.Zero
    };
    

    
});


builder.Services.AddCors(options => 
                        { var frontEndUrl = configuration.GetValue<string>("frontend_url");
                            options.AddDefaultPolicy(builder => { builder
                                .WithOrigins( frontEndUrl )
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                            });
                        });

var app = builder.Build();


using (var scope =app.Services.CreateScope())
{
    var context=scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();


app.Run();
