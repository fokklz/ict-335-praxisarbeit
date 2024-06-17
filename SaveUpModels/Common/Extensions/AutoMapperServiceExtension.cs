using Microsoft.Extensions.DependencyInjection;

namespace SaveUpModels.Common.Extensions
{
    public static class AutoMapperServiceExtension
    {
        public static IServiceCollection AddAutoMapperProfile(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            return services;
        }
    }
}
