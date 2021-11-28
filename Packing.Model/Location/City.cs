using Packing.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Packing.Model.Location
{
    public class City
    {
        public string Name { get; }

        public Country Country { get; }

        City(string name, Country country)
        {
            Name = name;
            Country = country;
        }

        public static Result<City, MessageError> Create(string cityName, Country country)
            => string.IsNullOrEmpty(cityName) ?
            new MessageError("City name cannot be empty") :
            new Result<City, MessageError>(new City(cityName, country));
    }
}
