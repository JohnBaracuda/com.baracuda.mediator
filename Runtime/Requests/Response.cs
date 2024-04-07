namespace Baracuda.Mediator.Requests
{
    public readonly ref struct Response
    {
        public readonly ResponseType Type;

        public Response(ResponseType type)
        {
            Type = type;
        }

        public static implicit operator bool(Response response)
        {
            return response.Type == ResponseType.Answered;
        }
    }

    public readonly ref struct Response<T>
    {
        public readonly ResponseType Type;
        public readonly T Result;

        public Response(ResponseType type, T result)
        {
            Type = type;
            Result = result;
        }

        public static implicit operator bool(Response<T> response)
        {
            return response.Type == ResponseType.Answered;
        }
    }

    public readonly ref struct Response<T1, T2>
    {
        public readonly ResponseType Type;
        public readonly (T1, T2) Result;

        public Response(ResponseType type, T1 result1, T2 result2)
        {
            Type = type;
            Result = (result1, result2);
        }

        public static implicit operator bool(Response<T1, T2> response)
        {
            return response.Type == ResponseType.Answered;
        }
    }

    public readonly ref struct Response<T1, T2, T3>
    {
        public readonly ResponseType Type;
        public readonly (T1, T2, T3) Result;

        public Response(ResponseType type, T1 result1, T2 result2, T3 result3)
        {
            Type = type;
            Result = (result1, result2, result3);
        }

        public static implicit operator bool(Response<T1, T2, T3> response)
        {
            return response.Type == ResponseType.Answered;
        }
    }

    public readonly ref struct Response<T1, T2, T3, T4>
    {
        public readonly ResponseType Type;
        public readonly (T1, T2, T3, T4) Result;

        public Response(ResponseType type, T1 result1, T2 result2, T3 result3, T4 result4)
        {
            Type = type;
            Result = (result1, result2, result3, result4);
        }

        public static implicit operator bool(Response<T1, T2, T3, T4> response)
        {
            return response.Type == ResponseType.Answered;
        }
    }
}