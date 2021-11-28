using System;
using System.Collections.Generic;
using System.Text;

namespace Packing.Shared
{
    public class MessageError
    {
        public string Message { get; private set; }

        public MessageError(string message)
        {
            Message = message;
        }

        public MessageError Chain(MessageError other)
        {
            Message += '\n' + other.Message;
            return this;
        }
    }

    public class MessageError<T> : MessageError
    {
        public T Value { get; }

        public MessageError(T value, string message) : base(message)
        {
            Value = value;
        }
    }

    public class MessageError<T1, T2> : MessageError
    {
        public T1 FirstValue { get; }

        public T2 SecondValue { get; }

        public MessageError(T1 firstValue, T2 secondValue, string message) : base(message)
        {
            FirstValue = firstValue;
            SecondValue = secondValue;
        }
    }
}
