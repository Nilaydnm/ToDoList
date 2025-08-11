using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Results
{
    public class OperationResult
    {
        public bool Succeeded { get; }
        public IReadOnlyList<string> Errors { get; }

        protected OperationResult(bool succeeded, IEnumerable<string>? errors = null)
        {
            Succeeded = succeeded;
            Errors = (errors ?? Array.Empty<string>()).ToArray();
        }

        public static OperationResult Ok() => new(true);
        public static OperationResult Fail(params string[] errors) => new(false, errors);
    }

    public class OperationResult<T> : OperationResult
    {
        public T? Data { get; }

        private OperationResult(bool succeeded, T? data, IEnumerable<string>? errors = null)
            : base(succeeded, errors)
        {
            Data = data;
        }

        public static OperationResult<T> Ok(T data) => new(true, data);
        public static new OperationResult<T> Fail(params string[] errors) => new(false, default, errors);
    }
}
