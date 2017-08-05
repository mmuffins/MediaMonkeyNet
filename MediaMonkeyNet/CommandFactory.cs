using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Runtime;

namespace MediaMonkeyNet
{
    public static class CommandFactory
    {
        public static EvaluateCommand Create(string Expression = "", bool AwaitPromise = false)
        {
            return new EvaluateCommand
            {
                ObjectGroup = "console",
                IncludeCommandLineAPI = true,
                ReturnByValue = true,
                AwaitPromise = AwaitPromise,
                Expression = Expression,
            };
        }
    }
}