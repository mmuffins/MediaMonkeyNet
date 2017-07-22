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
        public bool WasThrown { get; set; }


        public EvaluateResponse(object value, bool wasThrown, string exception)
        {
            this.Value = value;
            this.WasThrown = wasThrown;
            this.Exception = exception;
        }

        public EvaluateResponse(EvaluateCommandResponse response)
        {
            var xx = response.Result;
            this.Value = response.Result.Value;

            this.WasThrown = (bool)response.WasThrown;

            if (this.WasThrown)
            {
                this.Exception = response.ExceptionDetails.Text;
            }
        }
    }
}
