using Packing.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Packing.Model.Location
{
    public class Country
    {
        public string Name { get; }

        Country(string name)
        {
            Name = name;
        }

        public static Result<Country, MessageError> Create(string countryName) =>
            string.IsNullOrEmpty(countryName) ?
            new Result<Country, MessageError>(new MessageError("Country name cannot be empty")) :
            new Country(countryName);
    }
}
