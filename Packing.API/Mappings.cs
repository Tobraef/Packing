using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Packing.API
{
    public static class MappingsRegistration
    {
        public static MapperConfiguration CreateConfiguration()
        {
            var config = new MapperConfiguration(cfg =>
            {
                Services.Mapper.ServicesMappings.Add(cfg);
            });
            config.AssertConfigurationIsValid();
            return config;
        }
    }
}
