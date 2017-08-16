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
        public object Value { get; private set; }
        public string Exception { get; private set; }
        public string Description { get; private set; }
        public string ObjectId { get; private set; }
        public string Type { get; private set; }

        public EvaluateResponse(object value, string exception)
        {
            this.Value = value;
            this.Exception = exception;
        }

        public EvaluateResponse(EvaluateCommandResponse response)
        {
            if (response.ExceptionDetails != null)
            {
                this.Exception = response.ExceptionDetails.Text;
            }

            this.Value = response.Result.Value;
            this.Type = response.Result.Type;
            this.Description = response.Result.Description;
            this.ObjectId = response.Result.ObjectId;

        }
    }
}
