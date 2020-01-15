namespace AVS.CoreLib.ClientApi
{
    public struct ApiCommand
    {
        public string Command;
        public string Verb;
        public string Version;

        public ApiCommand(string command, string verb)
        {
            Command = command;
            Verb = verb;
            Version = null;
        }

        public ApiCommand(string command, string verb, string version)
        {
            Command = command;
            Verb = verb;
            Version = version;
        }
    }
}