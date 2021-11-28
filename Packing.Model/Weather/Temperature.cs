using Packing.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Packing.Model.Weather
{
    public class Temperature
    {
        public int Value { get; }

        Temperature(int value)
        {
            Value = value;
        }

        public static Result<Temperature, MessageError> Create(int value)
        {
            if (value < -273)
                return new MessageError("Temperature cannot go below absolute zero.");
            if (value > 50)
                return new MessageError<int>(value, "Temperature cannot exceed 50 degrees");
            return new Temperature(value);
        }
    }
}
