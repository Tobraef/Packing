using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Packing.Shared
{
    public class Result<TResult, TError>
    {
        readonly TResult _result;
        readonly TError _error;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public Result(TResult result)
        {
            _result = result;
        }

        public Result(TError error)
        {
            IsErr = true;
            _error = error;
        }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        public bool IsErr { get; }

        public bool IsOk => !IsErr;

        public TResult Get
        { 
            get
            {
                if (IsErr) 
                    throw new InvalidOperationException($"Tried to get value out of empty result of {typeof(TResult)} and {typeof(TError)}");
                return _result;
            }
        }

        public TError Err
        {
            get
            {
                if (!IsErr) 
                    throw new InvalidOperationException($"Tried to get error out of OK result of {typeof(TResult)} and {typeof(TError)}");
                return _error;
            }
        }

        public static implicit operator bool(Result<TResult, TError> result) => !result.IsErr;

        public static implicit operator Result<TResult, TError>(TResult okValue) => new Result<TResult, TError>(okValue);

        public static implicit operator Result<TResult, TError>(TError error) => new Result<TResult, TError>(error);

        public Result<TResult, TOtherError> Map<TOtherError>(Func<TError, TOtherError> mapping)
        {
            if (!IsErr) return new Result<TResult, TOtherError>(_result);
            return new Result<TResult, TOtherError>(mapping(_error));
        }

        public Result<TOtherResult, TError> Then<TOtherResult>(Func<TResult, Result<TOtherResult, TError>> then)
        {
            if (IsErr) return new Result<TOtherResult, TError>(_error);
            return then(_result);
        }

        public async Task<Result<TOtherResult, TError>> Then<TOtherResult>(Func<TResult, Task<Result<TOtherResult, TError>>> then)
        {
            if (IsErr) return new Result<TOtherResult, TError>(_error);
            return await then(_result);
        }

        public Result<TOther, TError> MapErr<TOther>()
        {
            if (IsOk) throw new InvalidOperationException("Cannot map OK value into other OK.");
            return new Result<TOther, TError>(_error);
        }
    }
}
