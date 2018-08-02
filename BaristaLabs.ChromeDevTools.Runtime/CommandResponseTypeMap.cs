namespace BaristaLabs.ChromeDevTools.Runtime
{
    using System;
    using System.Collections.Generic;

    public static class CommandResponseTypeMap
    {
        private readonly static IDictionary<Type, Type> s_commandResponseTypeDictionary;

        static CommandResponseTypeMap()
        {
            s_commandResponseTypeDictionary = new Dictionary<Type, Type>()
            {
                { typeof(Runtime.AwaitPromiseCommand), typeof(Runtime.AwaitPromiseCommandResponse) },
                { typeof(Runtime.CallFunctionOnCommand), typeof(Runtime.CallFunctionOnCommandResponse) },
                { typeof(Runtime.CompileScriptCommand), typeof(Runtime.CompileScriptCommandResponse) },
                { typeof(Runtime.DisableCommand), typeof(Runtime.DisableCommandResponse) },
                { typeof(Runtime.DiscardConsoleEntriesCommand), typeof(Runtime.DiscardConsoleEntriesCommandResponse) },
                { typeof(Runtime.EnableCommand), typeof(Runtime.EnableCommandResponse) },
                { typeof(Runtime.EvaluateCommand), typeof(Runtime.EvaluateCommandResponse) },
                { typeof(Runtime.GetIsolateIdCommand), typeof(Runtime.GetIsolateIdCommandResponse) },
                { typeof(Runtime.GetHeapUsageCommand), typeof(Runtime.GetHeapUsageCommandResponse) },
                { typeof(Runtime.GetPropertiesCommand), typeof(Runtime.GetPropertiesCommandResponse) },
                { typeof(Runtime.GlobalLexicalScopeNamesCommand), typeof(Runtime.GlobalLexicalScopeNamesCommandResponse) },
                { typeof(Runtime.QueryObjectsCommand), typeof(Runtime.QueryObjectsCommandResponse) },
                { typeof(Runtime.ReleaseObjectCommand), typeof(Runtime.ReleaseObjectCommandResponse) },
                { typeof(Runtime.ReleaseObjectGroupCommand), typeof(Runtime.ReleaseObjectGroupCommandResponse) },
                { typeof(Runtime.RunIfWaitingForDebuggerCommand), typeof(Runtime.RunIfWaitingForDebuggerCommandResponse) },
                { typeof(Runtime.RunScriptCommand), typeof(Runtime.RunScriptCommandResponse) },
                { typeof(Runtime.SetCustomObjectFormatterEnabledCommand), typeof(Runtime.SetCustomObjectFormatterEnabledCommandResponse) },
                { typeof(Runtime.TerminateExecutionCommand), typeof(Runtime.TerminateExecutionCommandResponse) },
            };
        }

        /// <summary>
        /// Gets the command response type corresponding to the specified command type
        /// </summary>
        public static bool TryGetCommandResponseType<T>(out Type commandResponseType)
            where T : ICommand
        {
            return s_commandResponseTypeDictionary.TryGetValue(typeof(T), out commandResponseType);
        }
    }
}