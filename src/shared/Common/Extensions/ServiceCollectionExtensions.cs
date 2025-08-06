// FILE: src/shared/Common/Extensions/ServiceCollectionExtensions.cs
using System.Text;
using Common.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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

        // AutoMapper - will be added in Phase 4 when we create mapping profiles
        // services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);

        // FluentValidation - will be added in Phase 4 when we create validators
        // services.AddValidatorsFromAssemblyContaining<Common.Entities.BaseEntity>(ServiceLifetime.Transient);

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

    // Extension method to add AutoMapper when we need it (Phase 4)
    public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
    {
        // Will be implemented in Phase 4 when we create actual mapping profiles
        // services.AddAutoMapper(typeof(UserProfile), typeof(TenantProfile));
        return services;
    }

    // Extension method to add FluentValidation when we need it (Phase 4)
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        // Will be implemented in Phase 4 when we create actual validators
        // services.AddValidatorsFromAssemblyContaining<LoginRequestDtoValidator>(ServiceLifetime.Transient);
        return services;
    }
}
