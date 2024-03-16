using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WormCat.Library.Models
{
    public class TaskResponse<T>
    {
        public TaskResponse(T? result)
        {
            Result = result;
        }

        public TaskResponse(T? result, string? output)
        {
            Result = result;
            Output = output;
        }

        public T? Result { get; set; }
        public string? Output { get; set; }
    }

    public class TaskResponseErrorCode<T>
    {
        public TaskResponseErrorCode(T? result)
        {
            Result = result;
        }

        public TaskResponseErrorCode(T? result, int? output, params string[] parameters)
        {
            Result = result;
            ErrorCode = output;
            Params = parameters;
        }

        public T? Result { get; set; }
        public int? ErrorCode { get; set; }
        public string[] Params { get; set; } = new string[0];
    }
}
