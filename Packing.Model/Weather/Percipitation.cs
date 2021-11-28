using Packing.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Packing.Model.Weather
{
    public class Precipitation
    {
        public enum PrecipationType
        { 
            None,
            Snow,
            Rain,
        }

        public enum PrecipationAmount
        {
            None,
            Light,
            Medium,
            Strong,
        }

        public PrecipationType Type { get; }

        public PrecipationAmount Amount { get; }

        Precipitation(PrecipationAmount amount, PrecipationType type)
        {
            Amount = amount;
            Type = type;
        }

        public static Result<Precipitation, MessageError> Create(PrecipationType type, PrecipationAmount amount)
        {
            if (type == PrecipationType.None && amount != PrecipationAmount.None)
                return new MessageError("Received contrary data: none type and non none amount.");
            return new Precipitation(amount, type);
        }
    }
}
