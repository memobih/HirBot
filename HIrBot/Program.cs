using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using HirBot.Comman.Idenitity;
using HirBot.Redies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using HirBot.Repository;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using User.Services;
using HirBot.EntityFramework.DataBaseContext;
using Mailing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Text.Json;

// Set absolute paths for Linux compatibility
var isWindows = System.Runtime.InteropServices.RuntimeInformation
    .IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);

var contentRoot = isWindows ? Directory.GetCurrentDirectory() : "/mnt/mahmoud/gr/HirBot/HIrBot";
var webRoot = Path.Combine(contentRoot, "wwwroot");

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = contentRoot,
    WebRootPath = webRoot
});
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Define the Bearer authentication scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and the JWT token."
    });

    // Add a global security requirement
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
            new string[] {}
        }
    });
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("cs"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("cs")),
    options => options.MigrationsAssembly("Project.EntityFramework"));
});

builder.Services.AddSingleton<RedisService>(sp =>
{
    var config = builder.Configuration;
    var redisHost = config["Redis:Host"];
    var redisPassword = config["Redis:Password"];
    int redisPort = 14070;
    var redisUsername = config["Redis:Username"];
    return new RedisService(redisHost, redisPassword, redisPort, redisUsername);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Dependency Injection
builder.Services.AddInfrastructureServices().AddUsersServices();
#endregion
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequiredLength = 5;
}).AddDefaultTokenProviders()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = configuration["JWT:ValidIssuer"],
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
}).AddOAuth("github", options =>
{
    options.ClientId = "Ov23liRDEoQn7AlxrRew";
    options.ClientSecret = "da71b0cc44bc557fb594ac4235770e396b517d8b";
    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
    options.CallbackPath = "/api/ExternalAuth/github-callback";
    options.UserInformationEndpoint = "https://api.github.com/user";
    options.SaveTokens = true;
    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

    options.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
            response.EnsureSuccessStatusCode();

            var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            context.RunClaimActions(user.RootElement);
        }
    };

}).AddOAuth("google", options =>
{
    options.ClientId = "1025210167148-c2slbofk5jg4pk7626qkqh8avi768r1r.apps.googleusercontent.com";
    options.ClientSecret = "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"; 
    options.AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/auth";
    options.TokenEndpoint = "https://accounts.google.com/o/oauth2/token";
    options.CallbackPath = "/api/ExternalAuth/google-callback";
    options.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v1/userinfo";
    options.SaveTokens = true;
    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
    options.Scope.Add("email");
    options.SaveTokens = true;
    options.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
            response.EnsureSuccessStatusCode();

            var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            context.RunClaimActions(user.RootElement);
        }
    };
});

builder.Services.AddCors(corsOptions =>
{
    corsOptions.AddPolicy("MyPolicy",
        corsPolicyBuilder => corsPolicyBuilder.SetIsOriginAllowed(origin => true)
                                              .AllowAnyHeader()
                                              .AllowAnyMethod()
                                              .AllowCredentials() 
                                              );
});

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("Mailing"));
var app = builder.Build();
app.UseAuthentication(); // Check JWT token

app.UseCors("MyPolicy");
// Configure the HTTP request pipeline.
app.MapGet("/", (
    HttpContext ctx) =>
{
    return ctx.User.Claims.Select(x => new { x.Type, x.Value }).ToList();
});
app.MapGet("/login", (
    HttpContext ctx) =>
{
    return Results.Challenge(authenticationSchemes: new List<string>() { "github" });
}
    );
app.MapGet("/login/google", (
    HttpContext ctx) =>
{
    return Results.Challenge(authenticationSchemes: new List<string>() { "google" });
}
    );

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c=>c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();