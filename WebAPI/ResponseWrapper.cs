using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessGame.WebAPI
{
    [Serializable]
    public class ResponseWrapper<T>
    {
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
        public T? Data { get; set; }
    }
}
