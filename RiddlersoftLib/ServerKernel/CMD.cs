using System;
using System.Collections.Generic;
using System.Text;


namespace ServerKernel
{
    public static class CMD
    {
        public static bool IsInitalized { get; private set; } = false;

        public static List<string> Lines { get; private set; }

        public static Action<object> CallBack;

        public static bool IsReady { get; private set; } = true;
        public static void CommandCompleate(object returnValue)
        {
            if (CallBack != null)
                CallBack(returnValue);

            IsReady = true;
        }

        public static void Run(CMDQueueItem item)
        {
            Run(item.CommandLine, item.CommandAction);
        }

        public static void Run(string command)
        {
            Run(command, null);
        }
        /// <summary>
        /// a queue that holds all commands that need to be run
        /// </summary>
        private static Queue<CMDQueueItem> _queuedCommand = new Queue<CMDQueueItem>();

        public static void Run(string command, Action<object> callback)
        {
            if (!IsInitalized)
                throw new Exception("CMD must be initalized first");

            if (!IsReady)
            {
                ConsoleWriteLine("CMD Busy. Queueing up command... '" +  command + "'");
                _queuedCommand.Enqueue(new CMDQueueItem()
                {
                    CommandLine = command,
                    CommandAction = callback,
                });
                return;
            }

            IsReady = false;
            CallBack = callback; //set the callback
            ConsoleWriteLine(command); //output to screen
            IsReady = DecodeCommand(command); //decode and run the command
        }

        private static Dictionary<string, Action<string>> _commandLines = new Dictionary<string, Action<string>>();

        /// <summary>
        /// allows the adding of custom comands buy the using program
        /// </summary>
        /// <param name="commandline"></param>
        public static void AddCustomCommand(string commandline)
        {
            _commandLines.Add(commandline, (string command) => { CommandCompleate(command); IsReady = true; });
        }

        private static bool _lineChanged;

        public static bool LineChanged
        {
            get
            {
                bool tmp = _lineChanged;
                _lineChanged = false;
                return tmp;
            }
        }
        public static void ConsoleWrite(object data)
        {
           
            string line = data as string;
            List<string> lines = new List<string>();
            int index = line.IndexOf("\n");
            string firstLine = null;
            while (index > 0)
            {
                if (firstLine ==null)
                {
                    firstLine = line.Substring(0, index);
                    lines.Add(firstLine);
                }
                else
                    lines.Add(line.Substring(0, index));

                if (index + 2 <= line.Length)
                    line = line.Substring(index + 2);
                else
                    break;

                index = line.IndexOf("\n");
            }

            if (Lines.Count == 0)
                Lines.Add("");
            Lines[Lines.Count - 1] += line;

            if (lines.Count > 1)
                Lines.AddRange(lines);

            _lineChanged = true;
        }

        public static void ConsoleWriteLine(object data)
        {
            ConsoleWrite(data);
            Lines.Add("");
        }

        private static string ConsoleReadLine(object data)
        {
            return "";
        }

        private static void ConsoleClear(object data)
        {
            Lines.Clear();
        }

        /// <summary>
        /// this will find the correct action for the command
        /// </summary>
        /// <param name="command">the command to use</param>
        private static bool DecodeCommand(string command)
        {
           // command = command.ToLower(); //lower case the command
            //basic commands

            if (_commandLines.ContainsKey(command))
            {
                _commandLines[command].Invoke(command);
                return true;
            }


            if (command.Contains(".")) //if we have a space we know there could be more complex commands
            {
                int divider = command.IndexOf('.') + 1;
                string com = command.Substring(0, divider);
                string par = command.Substring(divider);

                if (_commandLines.ContainsKey(com))
                {
                    _commandLines[com].Invoke(par);
                    return true;
                }
            }

            ConsoleWriteLine("Invalid Command");
            return true;
            //more complex commands
        }

        private static void BuildCommandLine()
        {
            //attach server kernal
            Kernal.WriteLine = ConsoleWriteLine;
            Kernal.Write = ConsoleWrite;
            Kernal.Clear = ConsoleClear;
            Kernal.ReadLine = ConsoleReadLine;
            
            _commandLines.Add("clrscr", (string command) => { ConsoleClear(""); CommandCompleate(null); });
            _commandLines.Add("server.", (string command) => { Kernal.Execute(command); });
            _commandLines.Add("genpass", (string command) =>
            {
                CommandCompleate(null);
            });
            _commandLines.Add("exit", (string command) => { CommandCompleate(null); });
        }

        public static void Initalize()
        {
            if (IsInitalized)
                return;

            Lines = new List<string>();
            Lines.Add("");
            BuildCommandLine();
            ConsoleWriteLine("Riddlersoft Testing Console Version 1.0");
            ConsoleWriteLine("");
            Kernal.SetCallback(CommandCompleate); //set the callback
            IsInitalized = true;
            Kernal.OnError = OnError;
        }

        static void Main()
        {
            while (true)
            {
                ConsoleWrite("  >> ");
                Run(ConsoleReadLine(""));
            }

        }

        private const float RunCommandIntervel = .1f;
        private static float _runCommandTimer = 0f;
        public static void Update(float dt)
        {
            _runCommandTimer += dt;
            if (_runCommandTimer >= RunCommandIntervel)
            {
                _runCommandTimer = 0f; //reset timer

                if (_queuedCommand.Count <= 0)
                    return;

                //check if CMD is avalible
                if (IsReady)
                {
                    CMDQueueItem queueItem = _queuedCommand.Dequeue(); //get the command to run
                    ConsoleWrite("Running Queue Cmd: ");
                    Run(queueItem); //run the command
                  
                }

            }
        }

        private static void OnError(object data, string text)
        {
            IsReady = true;
        }
    }
}
