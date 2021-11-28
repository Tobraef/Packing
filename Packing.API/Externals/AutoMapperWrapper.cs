using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Packing.API.Externals
{
    public class AutoMapperWrapper : Services.Mapper.IMapper
    {
        readonly Mapper _mapper;

        public AutoMapperWrapper(Mapper mapper)
        {
            _mapper = mapper;
        }

        public TResult Map<TResult, TFrom>(TFrom from) => _mapper.Map<TFrom, TResult>(from);
    }
}
