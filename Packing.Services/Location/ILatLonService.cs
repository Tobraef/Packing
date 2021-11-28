using Packing.Model.Location;
using Packing.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Packing.Services.Location
{
    public interface ILatLonService
    {
        Task<Result<LatLon, MessageError>> GetGeoLocationFor(City city);
    }
}
