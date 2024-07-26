using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessGame.WebAPI
{
    [Serializable]
    public class Payload<T>
    {
        public T? Data { get; set; }
    }
}
