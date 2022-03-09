using Duende.IdentityServer;
using Identity.Data;
using Identity.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Identity;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices( this WebApplicationBuilder builder )
    {
        builder.Services.AddRazorPages();

        builder.Services.AddDbContext<ApplicationDbContext>( options =>
                                                                 options.UseSqlServer( builder.Configuration.GetConnectionString( "DefaultConnection" ) ) );

        builder.Services.AddIdentity<ApplicationUser , IdentityRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();

        builder.Services.AddCors( opt  => opt.AddPolicy( "Default"
                                                       , builder =>
                                                             builder.AllowAnyHeader()
                                                                    .AllowAnyMethod()
                                                                    .AllowAnyOrigin() ) );

        builder.Services
               .AddIdentityServer( options =>
                                   {
                                       options.Events.RaiseErrorEvents       = true;
                                       options.Events.RaiseInformationEvents = true;
                                       options.Events.RaiseFailureEvents     = true;
                                       options.Events.RaiseSuccessEvents     = true;

                                       // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                                       options.EmitStaticAudienceClaim = true;
                                   } )
               .AddInMemoryIdentityResources( Config.IdentityResources )
               .AddInMemoryApiScopes( Config.ApiScopes )
               .AddInMemoryClients( Config.Clients )
               .AddAspNetIdentity<ApplicationUser>();

        builder.Services
               .AddAuthentication( opt =>
                                   {
                                       opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                       opt.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                                   } )
               .AddJwtBearer()
               .AddOpenIdConnect( "oidc"
                                , "Prometheus"
                                , opt =>
                                  {
                                      opt.SignInScheme  = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                                      opt.SignOutScheme = IdentityServerConstants.SignoutScheme;
                                      opt.SaveTokens    = true;
                                      opt.Authority     = "https://prometheus.demo";
                                      opt.ResponseType  = "code";
                                      opt.ClientId      = "prometheus_client";
                                      opt.UsePkce       = true;

                                      opt.TokenValidationParameters = new TokenValidationParameters
                                                                      {
                                                                          NameClaimType = "name"
                                                                        , RoleClaimType = "role"
                                                                      };
                                  } );

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline( this WebApplication app )
    {
        app.UseSerilogRequestLogging();

        if ( app.Environment.IsDevelopment() )
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors( "Default" );
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages()
           .RequireAuthorization();

        return app;
    }
}