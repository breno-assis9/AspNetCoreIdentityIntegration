using AspNetCoreIdentityApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adicionando o serviço do Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configuração de JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();

// Criação de roles ao inicializar a aplicação
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    await CreateRoles(roleManager, userManager);
}

app.UseSwagger();
app.UseSwaggerUI();

// Configuração do pipeline HTTP
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task CreateRoles(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
{
    // Criando a role "Admin"
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        var role = new IdentityRole("Admin");
        await roleManager.CreateAsync(role);
    }

    // Criando a role "User"
    if (!await roleManager.RoleExistsAsync("User"))
    {
        var role = new IdentityRole("User");
        await roleManager.CreateAsync(role);
    }

    // Criando um usuário Admin se não existir
    var adminUser = await userManager.FindByEmailAsync("admin@admin.com");
    if (adminUser == null)
    {
        var user = new ApplicationUser
        {
            UserName = "admin@admin.com",
            Email = "admin@admin.com"
        };
        var createUserResult = await userManager.CreateAsync(user, "Password123!");

        if (createUserResult.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}