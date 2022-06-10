using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MTBrokerAPI;
using MTBrokerAPI.ModelViewModels.UserMngt;
using MTBrokerAPI.Services.Dossier;
using MTBrokerAPI.Services.MessageMngt;
using MTBrokerAPI.Services.UserMngt;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region Primaries

builder.Services.AddCors();

#region Kestrel





//REMOVE WHEN FILES WILL BE SAVED OUTSIDE THE APPLICATION
//builder.WebHost.UseWebRoot("wwwroot");

#endregion

//declare connection string
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



//configure user
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>().AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = StaticVariables.JwtAuthenticationType;
})

#region JWT Bearer Option
                .AddJwtBearer(options =>
                {
                    var key = System.Text.Encoding.UTF8.GetBytes(builder.Configuration["UserSettings:Secret"]);
                    var signingKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key);

                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        LifetimeValidator = (before, expires, token, param) => expires > DateTime.Now,
                        RequireExpirationTime = false,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateActor = false,
                        ValidateLifetime = true,
                        //ClockSkew = TimeSpan.FromSeconds(2D),
                        IssuerSigningKey = signingKey,

                        //ValidateIssuerSigningKey = true,
                    };
                    //options.Authority = StaticVariables.AppApiBaseUri;
                    //options.Audience = "api1";
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = true;

                    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            context.HttpContext.User = context.Principal;
                            return Task.CompletedTask;
                        }
                    };
                })
#endregion
                ;
#endregion


#region Consumer Services

builder.Services.AddTransient<IDossier, DossierService>();

builder.Services.AddTransient<IMessageManager, MessageManagerService>();

builder.Services.AddTransient<IUserAccount, UserAccountService>();

#endregion


#region Configure

var app = builder.Build();

await Task.Run(async () =>
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var serviceProvider = services.GetRequiredService<IServiceProvider>();
            var configuration = services.GetRequiredService<IConfiguration>();


            await CreateRoles(serviceProvider);

        }
        catch (Exception exception)
        {
            services.GetRequiredService<ILogger<Program>>().LogError(exception, StaticVariables.CreateRolesErrorMessage);
        }
    }

    #region Create Roles
    async Task CreateRoles(IServiceProvider serviceProvider)
    {
        var appDbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();


        #region Roles

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        IdentityResult roleResult;
        for (int roleIndex = 0; roleIndex < StaticVariables.RoleNames.Length; roleIndex++)
        {

            var roleExists = await roleManager.RoleExistsAsync(StaticVariables.RoleNames[roleIndex]);
            if (!roleExists)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole<int>(StaticVariables.RoleNames[roleIndex]));
            }

        }

        await appDbContext.SaveChangesAsync();

        #endregion


        #region Launch Users

        var iConfiguration = serviceProvider.GetRequiredService<IConfiguration>();

        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var admins = await userManager.GetUsersInRoleAsync(StaticVariables.AdminRole);
        if (admins.Count < 1)
        {
            var userService = serviceProvider.GetRequiredService<IUserAccount>();

            await userService.RegisterAdminAsync(appDbContext, iConfiguration, roleManager, userManager, new RegisterAppUserMVM
            {
                EmailAddress = "user@gmail.com",
                Name = "User",
                Password = "User@123",
                UserName = "user"
            });
        }

        #endregion


    }

    #endregion
});

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
/*
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
*/

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

// Shows UseCors with CorsPolicyBuilder.
app.UseCors(builder =>
{
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
});


app.Run();

#endregion
