﻿using System.Reflection;
using System.Text;
using Account.Application.Endpoints.AccountEndpoints.Authorize;
using Account.Application.Endpoints.AccountEndpoints.ChangePassword;
using Account.Application.Endpoints.AccountEndpoints.ForgetAccount.ResetPassword;
using Account.Application.Endpoints.AccountEndpoints.ForgetAccount.SendForgetCode;
using Account.Application.Endpoints.AccountEndpoints.ForgetAccount.VerifyForgetCode;
using Account.Application.Endpoints.AccountEndpoints.Login;
using Account.Application.Endpoints.AccountEndpoints.Register.CreateAccount;
using Account.Application.Endpoints.AccountEndpoints.Register.SendRegistrationCode;
using Account.Application.Endpoints.AccountEndpoints.Register.VerifyRegistrationCode;
using Account.Application.Endpoints.UserEndpoints.ChangeProfile;
using Account.Application.SeedWorks;
using Account.Application.Services;
using Account.Entity.RoleEntity.Repositories;
using Account.Entity.UserEntity.Repositories;
using Account.Infrastructure.Persistence;
using Account.Infrastructure.Services;
using Auth.Authorization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;

namespace Account.Infrastructure.Configurations
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection ConfigureServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services
                .ConfigureAuthentication(configuration)
                .ConfigureAuthorization(configuration)
                .ConfigureEndpoints()
                .AddPersistence(
                    configuration.GetConnectionString("AccountDb")
                        ?? throw new NullReferenceException(
                            "Couldn't find the database connection string."
                        )
                )
                .AddDistributedCache(
                    configuration.GetConnectionString("DistributedCache")
                        ?? throw new NullReferenceException(
                            "Couldn't find the cache connection string."
                        )
                );

            services.AddScoped<ITokenSender, EmailTokenSender>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtProvider, JwtProvider>();

            return services;
        }

        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                var scheme = new OpenApiSecurityScheme
                {
                    Description = "Authorization Header \r\nExample:'Bearer 123456789'",
                    Reference = new() { Type = ReferenceType.SecurityScheme, Id = "Authorization" },
                    Scheme = "oauth2",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                };
                options.AddSecurityDefinition("Authorization", scheme);
                var requirement = new OpenApiSecurityRequirement { [scheme] = new List<string>() };
                options.AddSecurityRequirement(requirement);
                var xmlFilename = $"{Assembly.GetEntryAssembly()!.GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
            return services;
        }

        public static IServiceCollection ConfigureEndpoints(this IServiceCollection services)
        {
            services
                .RegisterEndpointResolver<
                    LoginRequest,
                    LoginEndpointHandler,
                    LoginRequestValidator
                >()
                .RegisterEndpointResolver<
                    SendRegistrationCodeRequest,
                    SendRegistrationCodeEndpointHandler,
                    SendRegistrationCodeRequestValidator
                >()
                .RegisterEndpointResolver<
                    VerifyRegistrationCodeRequest,
                    VerifyRegistrationCodeEndpointHandler,
                    VerifyRegistrationCodeRequestValidator
                >()
                .RegisterEndpointResolver<
                    SendForgetCodeRequest,
                    SendForgetCodeEndpointHandler,
                    SendForgetCodeRequestValidator
                >()
                .RegisterEndpointResolver<
                    VerifyForgetCodeRequest,
                    VerifyForgetCodeEndpointHandler,
                    VerifyForgetCodeRequestValidator
                >()
                .RegisterEndpointResolver<
                    ResetPasswordRequest,
                    ResetPasswordEndpointHandler,
                    ResetPasswordValidator
                >()
                .RegisterAuthEndpointResolver<
                    AuthorizeRequest,
                    AuthorizeEndpointHandler,
                    AuthorizeRequestValidator
                >()
                .RegisterAuthEndpointResolver<
                    CreateAccountRequest,
                    CreateAccountEndpointHandler,
                    CreateAccountValidator
                >()
                .RegisterAuthEndpointResolver<
                    ChangeProfileRequest,
                    ChangeProfileEndpointHandler,
                    ChangeProfileRequestValidator
                >()
                .RegisterAuthEndpointResolver<
                    ChangePasswordRequest,
                    ChangePasswordEndpointHandler,
                    ChangePasswordRequestValidator
                >();
            return services;
        }

        private static IServiceCollection ConfigureAuthentication(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    var secKey =
                        configuration["Authentication:SecKey"]
                        ?? throw new NullReferenceException("Couldn't find 'SecKey'.");
                    options.TokenValidationParameters = new()
                    {
                        NameClaimType = "Username",
                        RoleClaimType = "Roles",
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.Default.GetBytes(secKey)
                        ),
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        LifetimeValidator = (notbefore, expires, _, _) =>
                        {
                            return DateTime.UtcNow > (notbefore ?? DateTime.MinValue)
                                && DateTime.UtcNow < (expires ?? DateTime.MaxValue);
                        },
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidAlgorithms = [configuration["Authentication:Algorithm"]]
                    };
                });

            return services;
        }

        private static IServiceCollection ConfigureAuthorization(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services
                .AddAuthorizationBuilder()
                .AddPolicy(
                    AuthorizationRoles.User,
                    policy =>
                        policy
                            .RequireAuthenticatedUser()
                            .RequireClaim("Username")
                            .RequireClaim("Roles", AuthorizationRoles.User)
                            .RequireClaim("Id")
                )
                .AddPolicy(
                    AuthorizationRoles.Admin,
                    policy =>
                        policy
                            .RequireAuthenticatedUser()
                            .RequireClaim("Username")
                            .RequireClaim("Roles", AuthorizationRoles.Admin)
                            .RequireClaim("Id")
                )
                .AddPolicy(
                    AuthorizationRoles.Registrant,
                    policy =>
                        policy
                            .RequireAuthenticatedUser()
                            .RequireClaim("Email")
                            .RequireClaim("Roles", AuthorizationRoles.Registrant)
                );
            return services;
        }

        private static IServiceCollection AddPersistence(
            this IServiceCollection services,
            string connectionString
        )
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddDbContext<AccountDbContext>(options =>
            {
                options.UseLoggerFactory(
                    new LoggerFactory().AddSerilog(
                        new LoggerConfiguration()
                            .Enrich
                            .FromLogContext()
                            .WriteTo
                            .Console()
                            .MinimumLevel
                            .Warning()
                            .CreateLogger()
                    )
                );
                options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
            });

            services.AddScoped<IUserQueryRepository, UserRepository>();
            services.AddScoped<IUserCommandRepository, UserRepository>();
            services.AddScoped<IUserCheckRepository, UserRepository>();
            services.AddScoped<IRoleRespository, RoleRepository>();
            return services;
        }

        private static IServiceCollection AddDistributedCache(
            this IServiceCollection services,
            string connectionString
        )
        {
            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(connectionString)
            );
            services.AddScoped<IAuthCache, RedisAuthCache>();
            return services;
        }

        private static IServiceCollection RegisterEndpointResolver<TRequest, THandler, TValidator>(
            this IServiceCollection services
        )
            where TRequest : IRequest
            where THandler : class, IEndpointHandler<TRequest>
            where TValidator : class, IValidator<TRequest>
        {
            services.AddScoped<IEndpointHandler<TRequest>, THandler>();
            services.AddScoped<IValidator<TRequest>, TValidator>();
            return services;
        }

        private static IServiceCollection RegisterAuthEndpointResolver<
            TRequest,
            THandler,
            TValidator
        >(this IServiceCollection services)
            where TRequest : IRequest
            where THandler : class, IAuthEndpointHandler<TRequest>
            where TValidator : class, IValidator<TRequest>
        {
            services.AddScoped<IAuthEndpointHandler<TRequest>, THandler>();
            services.AddScoped<IValidator<TRequest>, TValidator>();
            return services;
        }
    }
}
