namespace GeoApp.Application.Common.Models
{
    public class Result
    {
        internal Result(bool succeeded, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Errors = errors.ToArray();
        }

        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }

        public static Result Success()
        {
            return new Result(true, Array.Empty<string>());
        }

        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(false, errors);
        }
    }

    public class Result<T> : Result
    {
        internal Result(T data, bool succeeded, IEnumerable<string> errors)
            : base(succeeded, errors)
        {
            Data = data;
        }

        public T Data { get; set; }

        public static Result<T> Success(T data)
        {
            return new Result<T>(data, true, Array.Empty<string>());
        }

        public new static Result<T> Failure(IEnumerable<string> errors)
        {
            return new Result<T>(default!, false, errors);
        }
    }
}