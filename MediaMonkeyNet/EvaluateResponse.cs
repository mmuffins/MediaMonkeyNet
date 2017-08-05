using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Runtime;


namespace MediaMonkeyNet
{
    public class EvaluateResponse
    {
        public object Value { get; set; }
        public string Exception { get; set; }


        public EvaluateResponse(object value, string exception)
        {
            this.Value = value;
            this.Exception = exception;
        }

        public EvaluateResponse(EvaluateCommandResponse response)
        {
            this.Value = response.Result.Value;
            if (response.ExceptionDetails != null)
            {
                this.Exception = response.ExceptionDetails.Text;
            }
        }
    }
}
