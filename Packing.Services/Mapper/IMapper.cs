using System;
using System.Collections.Generic;
using System.Text;

namespace Packing.Services.Mapper
{
    public interface IMapper
    {
        TResult Map<TResult, TFrom>(TFrom from);
    }
}
