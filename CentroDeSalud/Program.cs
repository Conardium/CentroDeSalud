using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CentroDeSalud.Data;
using CentroDeSalud.Repositories;
using CentroDeSalud.Models;
using CentroDeSalud.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using CentroDeSalud.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Para poder acceder al HttpContext (necesario para las Coockies)
builder.Services.AddHttpContextAccessor();

//============================ REPOSITORIOS ===============================
builder.Services.AddTransient<IRepositorioUsuarios, RepositorioUsuarios>();
builder.Services.AddTransient<IRepositorioPacientes, RepositorioPacientes>();
builder.Services.AddTransient<IRepositorioRoles, RepositorioRoles>();
builder.Services.AddTransient<IRepositorioUsuariosLoginExterno, RepositorioUsuariosLoginExterno>();
builder.Services.AddTransient<IRepositorioMedicos, RepositorioMedicos>();
builder.Services.AddTransient<IRepositorioCitas, RepositorioCitas>();
builder.Services.AddTransient<IRepositorioDisponibilidadesMedicos, RepositorioDisponibilidadesMedicos>();
builder.Services.AddTransient<IRepositorioChats, RepositorioChats>();
builder.Services.AddTransient<IRepositorioMensajes, RepositorioMensajes>();

//============================ SERVICIOS ===============================
builder.Services.AddTransient<IServicioPacientes, ServicioPacientes>();
builder.Services.AddTransient<IServicioEmail, ServicioEmail>();
builder.Services.AddTransient<IServicioMedicos, ServicioMedicos>();
builder.Services.AddTransient<IServicioUsuariosLoginsExternos, ServicioUsuariosLoginsExternos>();
builder.Services.AddTransient<IServicioCitas, ServicioCitas>();
builder.Services.AddTransient<IServicioDisponibilidadesMedicos, ServicioDisponibilidadesMedicos>();
builder.Services.AddTransient<IServicioChats, ServicioChats>();
builder.Services.AddTransient<IServicioMensajes, ServicioMensajes>();
builder.Services.AddSignalR(); //Servicio del chat en linea

//============================ IDENTITY ===============================
builder.Services.AddTransient<IUserClaimsPrincipalFactory<Usuario>, CustomClaimsPrincipalFactory>();//Claims Personalizado
builder.Services.AddTransient<SignInManager<Usuario>>(); //Coockies
builder.Services.AddTransient<IUserStore<Usuario>, UsuarioStore>(); //UserManager
//Habilitar Identity
builder.Services.AddIdentityCore<Usuario>(opciones =>
{
    //Personalizacion y modificacion de las reglas que trae por defecto Identity
    opciones.SignIn.RequireConfirmedAccount = false; //No requiere confirmacion de cuenta
    opciones.Password.RequiredLength = 6; //Numero minimo de caracteres es de 6
    opciones.Password.RequireDigit = false; //NO Requiere Numeros
    opciones.Password.RequireLowercase = false; //NO Requiere minusculas
    opciones.Password.RequireUppercase = false; //NO Requiere mayusculas
    opciones.Password.RequireNonAlphanumeric = false; //NO Requerir alfanuméricos
})
    .AddErrorDescriber<MensajesDeErrorIdentity>() //Establecer el descriptor de errores
    .AddDefaultTokenProviders(); //Tokens para recuperar/modificar contraseña, cambiar email, etc.

builder.Services.AddAuthorization(); //Permitimos la comprobacion de autorizaciones (Para los roles)

//============================ COOCKIE DE SESION ===============================
//Configuracion del servicio de autentificacion
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme; //Necesario para el login externo
})
    .AddCookie(IdentityConstants.ApplicationScheme) //Login normal
    .AddCookie(IdentityConstants.ExternalScheme)    //Login externo
    .AddMicrosoftAccount(options => //Microsoft
    {
        options.ClientId = builder.Configuration["MicrosoftClientId"];
        options.ClientSecret = builder.Configuration["MicrosoftSecretId"];
    })
    .AddGoogle(options => //Google
    {
        options.ClientId = builder.Configuration["GoogleClientId"];
        options.ClientSecret = builder.Configuration["GoogleSecretId"];
        
        options.Scope.Add("https://www.googleapis.com/auth/calendar.events"); //Acceso al scope de Google Calendar

        //Solicitar refresh token
        options.AccessType = "offline";
        options.SaveTokens = true; //Permite guardar los tokens en la coockie
    });

//============================ PERSONALIZACIÓN DE RUTAS DE LAS COOCKIES ===============================
builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
    opciones =>
    {
        opciones.LoginPath = "/usuarios/login";
        opciones.AccessDeniedPath = "/usuarios/login";
    });

//============================ CONEXIÓN PARA LAS MIGRACIONES DE EF CORE =============================
//Asignacion de la Cadena de Conexión
builder.Services.AddDbContext<CentroSaludContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevelopmentConnection") ?? throw new InvalidOperationException("Cadena de Conexión 'DevelopmentConnection' no encontrada")));


var app = builder.Build();

//Insercion automatica de los Roles
using (var scope = app.Services.CreateScope())
{
    var rolRepo = scope.ServiceProvider.GetRequiredService<IRepositorioRoles>();
    var rolesPredeterminados = new List<Rol>
    {
        new Rol { Id = 1, Nombre = "Admin", NombreNormalizado = "ADMIN" },
        new Rol { Id = 2, Nombre = "Medico", NombreNormalizado = "MEDICO" },
        new Rol { Id = 3, Nombre = "Paciente", NombreNormalizado = "PACIENTE" }
    };

    foreach (var rol in rolesPredeterminados)
    {
        var existe = await rolRepo.Existe(rol.NombreNormalizado);
        if (!existe)
        {
            await rolRepo.InsertarRol(rol);
        }
    }
    
    //Seed para generar medicos POR AHORA
    
    //var services = scope.ServiceProvider;
    //await SeedMedicos.CrearMedicosAsync(services);
    
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); //Permite obtener los datos del usuario autenticado
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<ChatHub>("/chathub"); // Ruta de SignalR para el cliente JS

app.Run();
