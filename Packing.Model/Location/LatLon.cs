using Packing.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Packing.Model.Location
{
    /// <summary>
    /// N and E are positive values.
    /// </summary>
    public class LatLon
    {
        public decimal Latitude { get; }

        public decimal Longtitude { get; }

        public LatLon(decimal latitude, decimal longtitude)
        {
            Latitude = latitude;
            Longtitude = longtitude;
        }

        public static Result<LatLon, MessageError> Create(decimal latitude, decimal longtitude)
        {
            if (latitude < -90 || latitude > 90)
                return new MessageError("Latitude cannot be lower than -90 or exceed 90. Received: " + latitude);
            if (longtitude < -180 || longtitude > 180)
                return new MessageError("Longitude cannot be lower than -180 or exceed 180. Received: " + longtitude);
            return new LatLon(latitude, longtitude);
        }
    }
}
