using Microsoft.Extensions.DependencyInjection;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Text;


namespace Services
{
    
        public static class IServiceCollectionExtention
        {
            public static IServiceCollection AddService(this IServiceCollection services)
            {
            
                services.AddScoped<IAlgoritemServices,AlgoritemServices >();
      
                return services;
            }
        }
    
}
