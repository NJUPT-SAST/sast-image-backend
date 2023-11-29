﻿using System.Text.Json;
using System.Text.Json.Serialization;
using Account.Application.Users.Login;
using Account.WebAPI.Endpoints.Login;
using Microsoft.AspNetCore.Mvc;

namespace Account.WebAPI.Configurations
{
    public static class HttpJsonSerializerExtension
    {
        public static IServiceCollection ConfigureJsonSerializer(this IServiceCollection services)
        {
            services.ConfigureHttpJsonOptions(options =>
            {
                options
                    .SerializerOptions
                    .TypeInfoResolverChain
                    .Insert(0, AppJsonSerializerContext.Default);
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
            return services;
        }
    }

    [JsonSerializable(typeof(LoginRequest))]
    [JsonSerializable(typeof(ObjectResult))]
    [JsonSerializable(typeof(IActionResult))]
    [JsonSerializable(typeof(LoginDto))]
    public partial class AppJsonSerializerContext : JsonSerializerContext { }
}
