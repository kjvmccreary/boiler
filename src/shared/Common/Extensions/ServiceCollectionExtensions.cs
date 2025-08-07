// FILE: src shared/Common/Extensions/ServiceCollectionExtensions.cs
using System.Text;
using Common.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using DTOs.Validators;

namespace Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration settings
        services.AddConfigurationSettings<JwtSettings>(configuration, JwtSettings.SectionName);
        services.AddConfigurationSettings<TenantSettings>(configuration, TenantSettings.SectionName);

        // HTTP Context Accessor
        services.AddHttpContextAccessor();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetRequiredSection<JwtSettings>(JwtSettings.SectionName);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = jwtSettings.ValidateIssuer,
                ValidateAudience = jwtSettings.ValidateAudience,
                ValidateLifetime = jwtSettings.ValidateLifetime,
                ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();
        return services;
    }

    public static T GetRequiredSection<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var section = configuration.GetSection(sectionName);
        if (!section.Exists())
        {
            throw new InvalidOperationException($"Configuration section '{sectionName}' is missing.");
        }

        var settings = new T();
        section.Bind(settings);
        return settings;
    }

    public static IServiceCollection AddConfigurationSettings<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
        where T : class, new()
    {
        var settings = configuration.GetRequiredSection<T>(sectionName);
        services.AddSingleton(settings);
        return services;
    }

    // IMPROVED: Proper AutoMapper extension method
    public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services, params Type[] profileTypes)
    {
        services.AddAutoMapper(profileTypes);
        return services;
    }

    // Extension method to add FluentValidation - IMPLEMENTED
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<LoginRequestDtoValidator>(ServiceLifetime.Transient);
        services.AddScoped<IValidator<DTOs.Auth.LoginRequestDto>, LoginRequestDtoValidator>();
        services.AddScoped<IValidator<DTOs.Auth.RegisterRequestDto>, RegisterRequestDtoValidator>();
        services.AddScoped<IValidator<DTOs.Auth.RefreshTokenRequestDto>, RefreshTokenRequestDtoValidator>();
        services.AddScoped<IValidator<DTOs.Auth.ChangePasswordDto>, ChangePasswordDtoValidator>();
        services.AddScoped<IValidator<DTOs.Auth.LogoutRequestDto>, LogoutRequestDtoValidator>();
        services.AddScoped<IValidator<DTOs.Auth.ResetPasswordRequestDto>, ResetPasswordRequestDtoValidator>();
        services.AddScoped<IValidator<DTOs.Auth.ConfirmEmailRequestDto>, ConfirmEmailRequestDtoValidator>();
        
        return services;
    }
}
