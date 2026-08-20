using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TnbIcoms.Application.Account;
using TnbIcoms.Application.Auth;
using TnbIcoms.Application.AuthorisationPersonnel;
using TnbIcoms.Application.ChangeRequests;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.ConflictingLines;
using TnbIcoms.Application.DropdownValues;
using TnbIcoms.Application.Email;
using TnbIcoms.Application.Equipment;
using TnbIcoms.Application.EquipmentTypes;
using TnbIcoms.Application.Files;
using TnbIcoms.Application.LinkingLines;
using TnbIcoms.Application.Mnemonics;
using TnbIcoms.Application.Lookups;
using TnbIcoms.Application.Organisations;
using TnbIcoms.Application.Outages;
using TnbIcoms.Application.Gnc;
using TnbIcoms.Application.Handover;
using TnbIcoms.Application.CommissioningMemos;
using TnbIcoms.Application.EmailLogs;
using TnbIcoms.Application.EmailTemplates;
using TnbIcoms.Application.ErrorLogs;
using TnbIcoms.Application.SingleLineDiagrams;
using TnbIcoms.Application.Reports;
using TnbIcoms.Application.Statistics;
using TnbIcoms.Application.OutageScheduleWindows;
using TnbIcoms.Application.OutageTypeRules;
using TnbIcoms.Application.Projects;
using TnbIcoms.Application.Roles;
using TnbIcoms.Application.RoleTransferRequests;
using TnbIcoms.Application.Stations;
using TnbIcoms.Application.SystemSettings;
using TnbIcoms.Application.TransmissionLines;
using TnbIcoms.Application.Users;
using TnbIcoms.Application.VoltageLevels;
using TnbIcoms.Infrastructure.Identity;
using TnbIcoms.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services
    .AddAuthentication(options =>
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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://tnb-icoms-development.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddMemoryCache();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleAdminService, RoleAdminService>();
builder.Services.AddScoped<IZoneService, ZoneService>();
builder.Services.AddScoped<IOrganisationService, OrganisationService>();
builder.Services.AddScoped<IStationService, StationService>();
builder.Services.AddScoped<IVoltageLevelService, VoltageLevelService>();
builder.Services.AddScoped<IEquipmentTypeService, EquipmentTypeService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IDropdownValueService, DropdownValueService>();
builder.Services.AddScoped<IDropdownValueAdminService, DropdownValueAdminService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IOutageTypeRuleService, OutageTypeRuleService>();
builder.Services.AddScoped<IOutageScheduleWindowService, OutageScheduleWindowService>();
builder.Services.AddScoped<IAuthorisationPersonnelService, AuthorisationPersonnelService>();
builder.Services.AddScoped<ISystemSettingService, SystemSettingService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IRoleTransferRequestService, RoleTransferRequestService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IMnemonicService, MnemonicService>();
builder.Services.AddScoped<ITransmissionLineService, TransmissionLineService>();
builder.Services.AddScoped<IConflictingLineService, ConflictingLineService>();
builder.Services.AddScoped<ILinkingLineService, LinkingLineService>();
builder.Services.AddScoped<IOutageService, OutageService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IGncService, GncService>();
builder.Services.AddScoped<IHandoverService, HandoverService>();
builder.Services.AddScoped<ISldService, SldService>();
builder.Services.AddScoped<ICommissioningMemoService, CommissioningMemoService>();
builder.Services.AddScoped<IErrorLogService, ErrorLogService>();
builder.Services.AddScoped<IEmailLogService, EmailLogService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
builder.Services.AddScoped<IChangeRequestService, ChangeRequestService>();

var adEnabled = builder.Configuration.GetValue<bool>("Ad:Enabled");
if (adEnabled)
{
    builder.Services.AddScoped<IAdAuthProvider, LdapAdAuthProvider>();
}
else
{
    builder.Services.AddScoped<IAdAuthProvider, StubAdAuthProvider>();
}

builder.Services.AddValidatorsFromAssembly(typeof(IAuthService).Assembly);

// Every email is logged to EmailLogs regardless of provider — LoggingEmailSender wraps the
// real transport (Stub or Resend) rather than each call site logging for itself.
var emailProvider = builder.Configuration["Email:Provider"];
if (string.Equals(emailProvider, "Resend", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddHttpClient<ResendEmailSender>();
}
else
{
    builder.Services.AddScoped<StubEmailSender>();
}
builder.Services.AddScoped<IEmailSender>(sp =>
{
    IEmailSender inner = string.Equals(emailProvider, "Resend", StringComparison.OrdinalIgnoreCase)
        ? sp.GetRequiredService<ResendEmailSender>()
        : sp.GetRequiredService<StubEmailSender>();
    return new LoggingEmailSender(inner, sp.GetRequiredService<TnbIcoms.Infrastructure.Persistence.AppDbContext>());
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Angular form controls emit numeric ids as strings (e.g. select values);
        // accept "3" for an int property instead of requiring a JSON number.
        options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "TNB ICOMS 2.0 API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter a valid JWT bearer token.",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, Array.Empty<string>() } });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await TnbIcoms.Infrastructure.Persistence.Seed.DbSeeder.SeedAsync(scope.ServiceProvider);
}

app.UseMiddleware<TnbIcoms.Api.Middleware.ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AngularDev");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
