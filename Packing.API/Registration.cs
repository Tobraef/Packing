using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Packing.API.Externals;
using Packing.Services.Location;
using Packing.Services.Mapper;
using Packing.Services.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Packing.API
{
    public static class Registration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<HttpClient>();
            services.AddScoped<ILatLonService, TeleportLatLonService>();
            services.AddScoped<IWeatherService, Timer7WeatherService>();
            services.AddScoped<Services.Mapper.IMapper, AutoMapperWrapper>(_ => new AutoMapperWrapper(new Mapper(MappingsRegistration.CreateConfiguration())));
        }
    }
}
