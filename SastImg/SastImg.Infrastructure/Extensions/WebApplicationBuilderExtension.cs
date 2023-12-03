﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Primitives.Common.Policies;

namespace SastImg.Infrastructure.Extensions
{
    public static class WebApplicationBuilderExtension
    {
        public static void ConfigureServices(
            this WebApplicationBuilder builder,
            IConfigurationRoot configuration
        )
        {
            builder.Services.TryAddSingleton(configuration);
            builder
                .Services
                .ConfigureOptions(builder.Configuration)
                .AddLogging()
                .ConfigureDatabase(
                    configuration.GetConnectionString("SastimgDb")
                        ?? throw new Exception("The connection string \"SastimgDb\" is null.")
                )
                .ConfigureCache(
                    configuration.GetConnectionString("DistributedCache")
                        ?? throw new Exception(
                            "The connection string \"DistributedCache\" is null."
                        )
                )
                .ConfigureMediator()
                .ConfigureRateLimiter(builder.Configuration);

            builder.Services.ConfigureSwagger();
            builder
                .Services
                .AddControllers(options =>
                {
                    options
                        .CacheProfiles
                        .Add(
                            ResponseCachePolicyNames.Default,
                            new()
                            {
                                Duration = configuration
                                    .GetSection("Cache")
                                    .GetValue<int>("ResponseCacheDuration"),
                                Location = ResponseCacheLocation.Any
                            }
                        );
                });
        }
    }
}
