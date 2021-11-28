using System;
using System.Collections.Generic;
using System.Text;

namespace Packing.Services.Date
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime GetTodayDate() => DateTime.Now;
    }
}
