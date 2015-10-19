namespace GoEat.Models
{

    public class Result<T> where T : class, new()
    {
        public Result(T data) : this(data, null)
        {
        }

        public Result(ErrorCodes? error)
            : this(null, error)
        {
        }

        public Result(T data, ErrorCodes? error)
        {
            Data = data;

            if (data == null)
                Error = error;
        }



        public T Data { get; private set; }
        public ErrorCodes? Error { get; private set; }


        public bool Succeeded
        {
            get { return !Error.HasValue; }
        }

        public bool HasData
        {
            get { return Data != null; }
        }

        public Result<T> Nullify(ErrorCodes? err = null)
        {
            Data = null;
            Error = err;
            return this;
        }


        #region STATIC 

        public static Result<T> Make(T data, ErrorCodes? errorIfNull = null)
        {
            return new Result<T>(data, errorIfNull);
        }

        public static Result<T> Null(ErrorCodes? err = null)
        {
            return new Result<T>(null, err);
        }

        #endregion
    }
}