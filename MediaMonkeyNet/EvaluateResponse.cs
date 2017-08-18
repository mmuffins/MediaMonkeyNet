using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Runtime;


namespace MediaMonkeyNet
{
    public class EvaluateResponse <T>
    {
        public T Value { get; private set; }
        public string Exception { get; private set; }
        public string Description { get; private set; }
        public string ObjectId { get; private set; }
        public string Type { get; private set; }

        public EvaluateResponse(T value, string exception, string description, string objectId, string type)
        {
            this.Value = (T)value;
            this.Exception = exception;
            this.Description = description;
            this.ObjectId = objectId;
            this.Type = type;
        }

        public EvaluateResponse(EvaluateCommandResponse response)
        {
            if (response.ExceptionDetails != null)
            {
                this.Exception = response.ExceptionDetails.Text;
            }

            this.Value = (T)response.Result.Value;
            this.Type = response.Result.Type;
            this.Description = response.Result.Description;
            this.ObjectId = response.Result.ObjectId;
        }

        public EvaluateResponse(GetPropertiesCommandResponse response)
        {
            if (response.ExceptionDetails != null)
            {
                this.Exception = response.ExceptionDetails.Text;
            }

            this.Value = (T)response.Result.Select(x => new EvaluateObjectProperty<object>(x));
        }
    }

    public class EvaluateObjectProperty <T>
    {
        public T Value { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string ObjectId { get; private set; }
        public string Type { get; private set; }

        public EvaluateObjectProperty(PropertyDescriptor obj)
        {
            this.Name = obj.Name;
            this.Value = (T)obj.Value.Value;
            this.Type = obj.Value.Type;
            this.ObjectId = obj.Value.ObjectId;
            this.Description = obj.Value.Description;
        }
    }
}
