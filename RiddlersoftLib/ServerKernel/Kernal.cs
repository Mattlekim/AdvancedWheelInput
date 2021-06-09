using System;
using System.Collections.Generic;
using MySqlDI; //server API
using MySqlDI.Database;

namespace ServerKernel
{
    public static class Kernal
    {
        private static DatabaseManager  _server = null;

        private static ServerStatus _status = ServerStatus.RequireInitalization;
        private static DatabaseRequest _currentRequest = DatabaseRequest.Null;

        public delegate void ConsoleAction(object data);
        public delegate string ConsoleFunction(object data);
        public delegate void Feedback(object data, string text);

        public static Feedback OnError;


        public static ConsoleAction WriteLine;

        public static ConsoleAction Write;
        public static ConsoleFunction ReadLine;
        public static ConsoleAction Clear;

        public static bool DebugMode { get; set; } = false;

        /// <summary>
        /// a dictionary all all commands that can be runn
        /// </summary>
        private static Dictionary<string, Action<string>> _commands = new Dictionary<string, Action<string>>();

        /// contains all server request that we have
        /// </summary>
        private static Dictionary<string, DatabaseRequest> _dbRequest = new Dictionary<string, DatabaseRequest>();

        private static Action<object> _callBack;

        public static void SetCallback(Action<object> callback)
        {
            _callBack = callback;
        }

        /// <summary>
        /// build all the command that we can run
        /// </summary>
        private static void BuildCommandLine()
        {
            WriteLine("Building Server Command Line...");
            _commands.Add("logmein", (string args) =>
            {
                _commands["init"].Invoke(""); _commands["login"].Invoke("user=23 pass=1234"); CustomHttpEncoders.AttackingId = 23;
            });
            _commands.Add("reconnect", (string args) => { _server.Disconnect(); _commands["login"].Invoke("user=23 pass=1234"); CustomHttpEncoders.AttackingId = 23; });
            _commands.Add("init", (string args) => {Connect(args); _server.OnPageError = DatabaseError;} );
            _commands.Add("login", (string args) => { Login(args); });
            _commands.Add("createuser", (string args) => { CreateUser(args); });
            _commands.Add("status", (string args) => 
            {
                if (_status == ServerStatus.RequireInitalization)
                {
                    WriteLine($"Server Status: {_status.ToString()}");
                    return;
                }

                WriteLine($"Server Status: {_status.ToString()}");
                WriteLine($"Server Name: {_server.Name }");
                WriteLine($"Server URL: {_server.ServerAddress }");
                WriteLine($"Data Trasfered: {_server.BandwidthUsed }");
                WriteLine($"Login Details > UserName: {_server.LoginUserName } Password: {_server.LoginPassword}");
                
            });
            _commands.Add("help", (string args) => 
            {
                Write("Commands: ");
                foreach (KeyValuePair<string, Action<string>> kp in _commands)
                    Write($"{kp.Key}   ");
                WriteLine("");
            });
            _commands.Add("send", (string args) => 
            {
                if (_status != ServerStatus.LoggedIn && _status != ServerStatus.Ready)
                {
                    WriteLine("Error Not Connected");
                    CreateError(_server, "Not Connected");
                    
                    if (_callBack != null)
                        _callBack(null);
                    return;
                }

                if (_dbRequest.ContainsKey(args))
                {
                    _currentRequest = _dbRequest[args];
                    _server.SendRequest(_currentRequest);
                    WriteLine("Sending Command...");
                    return;
                }

                //now we need to seperate the command with any args
                //seperate command from args
                int index = args.IndexOf(' ');
                if (index < 0)
                {
                    WriteLine("Command Arguments not valid");
                    if (_callBack != null)
                        _callBack(null);
                    return;
                }

                string com = args.Substring(0, index);
                string subargs = args.Substring(index + 1);

                if (_dbRequest.ContainsKey(com))
                {
                    _currentRequest = _dbRequest[com];
                    _server.SendRequest(_currentRequest, subargs);

                    return;
                }

                WriteLine("Invalid Syntax for command");
                if (_callBack != null)
                    _callBack(null);
                return;
            });

            _commands.Add("showlog", (string args) =>
            {
                if (_server == null)
                {
                    WriteLine("Server not initalized. No log to show.");
                    return;
                }
                WriteLine("");
                WriteLine("=============Log================");
                WriteLine(_server.Log);
                WriteLine("=============End================");
                WriteLine("");
                return;
            });
            WriteLine("Done.");
        }

        /// <summary>
        /// creates all the server request that we call call
        /// </summary>
        private static void BuildSererRequest()
        {
            WriteLine("Building Server Request Actions...");
            _dbRequest.Add("dlbd", new DatabaseRequest(DbAction.DownloadBasicData, "http://riddlersoftgames.co.uk/database/osrh2h/getuserinfo.php", HttpEncoders.None,
                (object o) =>
            {
                Dictionary<string, List<string>> db = o as Dictionary<string, List<string>>; //convert to correct type
                WriteLine("Data Recived:");
                foreach(KeyValuePair<string, List<String>> data in db)
                {
                    Write($"{data.Key}: {data.Value[0]}, ");
                }
                _status = ServerStatus.Ready;
                if (_callBack != null)
                    _callBack(db);
                WriteLine("");
            }));

            _dbRequest.Add("findmatch", new DatabaseRequest(DbAction.SendAction, "http://riddlersoftgames.co.uk/database/osrh2h/match/findMatch.php", CustomHttpEncoders.FindMatch,
               (object o) =>
               {
                   if (_status != ServerStatus.Ready)
                   {
                       WriteLine("You need to download user data first!");
                      
                       return;
                   }
                   
                   Dictionary<string, List<string>> db = MySqlWebServer.DecodeMySqlDatabase(o as string); //convert to readable data

                   if (db != null && db.Count > 0)
                   {
                       CustomHttpEncoders.OpponateId = Convert.ToInt32(db["Id"][0]);
                       CustomHttpEncoders.OpponatesLevel = Convert.ToInt32(db["Level"][0]);
                       CustomHttpEncoders.OpponatesRank = Convert.ToInt32(db["Rank"][0]);
                       CustomHttpEncoders.OpponatesAvalibleCredits = (int)Convert.ToDouble(db["ACredits"][0]);
                       CustomHttpEncoders.OpponatesAvalibleSpairParts = (int)Convert.ToDouble(db["AParts"][0]);
                       CustomHttpEncoders.OpponatesTimeToBeat = (float)Convert.ToDouble(db["Time"][0]);
                       WriteLine($"Opponate Found. Id: {CustomHttpEncoders.OpponateId}, " +
                           $"Credits: { CustomHttpEncoders.OpponatesAvalibleCredits}, Spair Parts: {CustomHttpEncoders.OpponatesAvalibleSpairParts}" +
                           $"Rank: {CustomHttpEncoders.OpponatesRank}");
                       if (_callBack != null)
                           _callBack(db);
                   }
                   else
                   {
                       CustomHttpEncoders.OpponateId = -1;

                       string output = o as string;
                       if (output.Contains("#nm"))
                       {
                           CreateError(o, "No Match Found");
                       }
                       else
                           CreateError(o, "Connection Error");

                       if (DebugMode)
                       {
                           WriteLine("===Debug Trace===");
                           WriteLine(o as string);
                           WriteLine("===End Debug===");
                       }
                       if (_callBack != null)
                           _callBack(null);
                       return;
                   }

               }));

            _dbRequest.Add("nextmatch", new DatabaseRequest(DbAction.SendAction, "http://riddlersoftgames.co.uk/database/osrh2h/match/findMatch.php", CustomHttpEncoders.NextMatch,
              (object o) =>
              {
                  if (_status != ServerStatus.Ready)
                  {
                      WriteLine("You need to download user data first!");
                      return;
                  }

                  Dictionary<string, List<string>> db = MySqlWebServer.DecodeMySqlDatabase(o as string); //convert to readable data

                  if (db != null)
                  {
                      CustomHttpEncoders.OpponateId = Convert.ToInt32(db["Id"][0]);
                      WriteLine($"Opponate Found. Id: {CustomHttpEncoders.OpponateId}");
                      if (_callBack != null)
                          _callBack(db);
                  }
                  else
                  {
                      CustomHttpEncoders.OpponateId = -1;
                      string output = o as string;
                      if (output.Contains("#nm"))
                      {
                          CreateError(o, "No Match Found");
                      }
                      else
                          CreateError(o, "Connection Error");
                      if (_callBack != null)
                          _callBack(null);
                      return;
                  }

                  if (DebugMode)
                  {
                      WriteLine("===Debug Trace===");
                      WriteLine(o as string);
                      WriteLine("===End Debug===");
                  }

              }));

            _dbRequest.Add("attack", new DatabaseRequest(DbAction.SendAction, "http://riddlersoftgames.co.uk/database/osrh2h/match/attack.php", CustomHttpEncoders.Attack,
             (object o) =>
             {
                 if (_status != ServerStatus.Ready)
                 {
                     WriteLine("You need to download user data first!");
                     return;
                 }

                 string output = o as string;
                 if (output.Contains("#nm"))
                     CreateError(o, "No Match Found");
                 else
                     WriteLine("Attack Started");

                 if (DebugMode)
                 {
                     WriteLine("===Debug Trace===");
                     WriteLine(o as string);
                     WriteLine("===End Debug===");
                 }


             }));

            _dbRequest.Add("aresults", new DatabaseRequest(DbAction.SendAction, "http://riddlersoftgames.co.uk/database/osrh2h/match/results.php", CustomHttpEncoders.AResult,
            (object o) =>
            {
                if (_status != ServerStatus.Ready)
                {
                    WriteLine("You need to download user data first!");
                    return;
                }

                string output = o as string;
                if (output.Contains("#nm"))
                    CreateError(o, "Database Error");
                else
                    WriteLine("Complete");

                if (DebugMode)
                {
                    WriteLine("===Debug Trace===");
                    WriteLine(o as string);
                    WriteLine("===End Debug===");
                }

            }));

            ///this will make the server run its scedualed function
            _dbRequest.Add("forceserverupdate", new DatabaseRequest(DbAction.SendAction, "http://riddlersoftgames.co.uk/database/osrh2h/sced5723.php", CustomHttpEncoders.ForceServerUpdate,
           (object o) =>
           {
               if (_status == ServerStatus.RequireInitalization)
               {
                   WriteLine("You need initalize server first!");
                   return;
               }

               string output = o as string;
               if (output.Contains("#nm"))
                   CreateError(o, "Database Error");
               else
                   WriteLine("Complete");

               if (DebugMode)
               {
                   WriteLine("===Debug Trace===");
                   WriteLine(o as string);
                   WriteLine("===End Debug===");
               }

           }));

            _dbRequest.Add("res", new DatabaseRequest(DbAction.SendAction, "http://riddlersoftgames.co.uk/database/osrh2h/base/change.php", CustomHttpEncoders.Resources,
          (object o) =>
          {
              if (_status == ServerStatus.RequireInitalization)
              {
                  WriteLine("You need initalize server first!");
                 
                  if (_callBack != null)
                      _callBack("e0");
                  return;
              }

              string output = o as string;
              if (output.Contains("#nm"))
                  CreateError(o, "Database Error");
              else
                  WriteLine("Complete");

              if (DebugMode)
              {
                  WriteLine("===Debug Trace===");
                  WriteLine(o as string);
                  WriteLine("===End Debug===");
              }

              _status = ServerStatus.Ready;
              if (_callBack != null)
                  _callBack(output);
          }));

            WriteLine("Done.");
        }

        private static void CreateError(object o, string text)
        {
            WriteLine(text);
            if (OnError != null)
                OnError(o, text);
        }
        /// <summary>
        /// connect to the webserver
        /// </summary>
        /// <param name="url"></param>
        private static void Connect(string data)
        {
            string url = "http://riddlersoftgames.co.uk/database/osrh2h/login.php";
            
            if (_server != null)
            {
                Write("Error Already Connected");
                WriteLine("");
                _callBack("init");
                return;
            }

            
            _server = new DatabaseManager(url, "Database"); //create link to database
            _server.WriteLogToOutputWindow = true;
            _server.OnLoginSuccess = OnLogin;
            _server.OnHTTPSuccesses = OnServerSucess;
            _server.OnHTTPFailure = OnServerError;

            WriteLine("Server Initalized");
            _status = ServerStatus.Initalized;
            _callBack("init");
        }

        private static void Login(string login)
        {
            if (_server == null)
            {
                WriteLine("Error server not Intitalized");
                WriteLine("You must call server.init first");
                _callBack("loginfail");
                return;
            }

            if (login == null)
            {
                WriteLine("Invalid Paramiters: Try server.login user=yourusername pass=youpassword");
                WriteLine("");
                _callBack("loginfail");
                return;
            }

            int passIndex = login.IndexOf("pass=");
            int userIndex = login.IndexOf("user=");
            if (passIndex < 0 || userIndex < 0)
            {
                WriteLine("Invalid Paramiters: Try server.login user=yourusername pass=youpassword");
                WriteLine("");
                _callBack("loginfail");
                return;
            }

            string password = login.Substring(passIndex + 5);
            int end = password.IndexOf(' ');
            if (end == -1)
                end = password.Length;
            password = password.Substring(0, end);

            string username = login.Substring(userIndex + 5);
            end = username.IndexOf(' ');
            if (end == -1)
                end = username.Length;
            username = username.Substring(0, end);

            string id = login.Substring(login.IndexOf("id=") + 3); 

            Dictionary<string, string> loginCreds = new Dictionary<string, string>()
            {
                {"user", username },
                {"password", password },
                {"id", id }
            };

            WriteLine("");
            WriteLine("Logining...");

            bool sucesses = _server.Login(loginCreds).Result;
            if (!sucesses)
                _callBack("login error");
        }

        private static void CreateUser(string login)
        {
            if (_server == null)
            {
                WriteLine("Error server not Intitalized");
                WriteLine("You must call server.init first");
                return;
            }



            WriteLine("");
            WriteLine("Connecting...");

            DatabaseRequest request = new DatabaseRequest(DbAction.SendAction, "http://riddlersoftgames.co.uk/database/osrh2h/createuser.php", 
                CustomHttpEncoders.CreateUser, _callBack);
            request.Type = RequestType.NotRequireLogin;
            _currentRequest = request;
            _server.SendRequest(request, login);
        }
         
        public static void Decode(string command)
        {
            if (_server != null)
                if (_server.WatingForRequest)
                {
                    WriteLine("Server Bussy Try Again");
                    return;
                }


            if (_commands.ContainsKey(command))
            {
                _commands[command].Invoke(command);
                return;
            }

            //seperate command from args
            int index = command.IndexOf(' ');
            if (index < 0)
            {
                WriteLine("Command Arguments not valid");
                _callBack(null);
                return;
            }

            string com = command.Substring(0, index);
            string args = command.Substring(index + 1);

            if (_commands.ContainsKey(com))
            {
                _commands[com].Invoke(args);
            
                return;
            }
            _callBack(null);
        }

        private static bool _isInitialized = false;

        public static void Execute(string command)
        {
            if (!_isInitialized)
            {
                WriteLine("Intitlaizing Server Componates");
                BuildCommandLine();
                BuildSererRequest();
                
                _isInitialized = true;
            }
            Decode(command);
            
           
        }

        static void ReadyForCommand()
        {
  //          WriteLine("");
    //        Write("  >> ");
        }

        static void OnLoginFailed(object o)
        {
            WriteLine("Login Failed");
            ReadyForCommand();

            if (_callBack != null)
                _callBack(null);
        }

        static void OnLogin(object o)
        {
            string data = o as string;
            if (DebugMode)
                WriteLine(data);
            if (data.Contains("s0"))
            {
                WriteLine((string)o);
                WriteLine("Login Successful");
                ReadyForCommand();
                _status = ServerStatus.LoggedIn;
                if (_callBack != null)
                    _callBack(data.Substring(2, data.IndexOf("<br>") - 2));
                return;
            }
            OnLoginFailed(o); //we know login failed so run it
        }

        static void OnServerSucess(object o)
        {
            _currentRequest.Decoder(o);
            string text = o as string;
        }

        static void DatabaseError(ErrorTypes error)
        {
            if (OnError != null)
                OnError(error, error.ToString());
            if (error == ErrorTypes.NotLogedIn) //if server has disconected you
            {

            }
            WriteLine($"Error: {error.ToString()}" );
        }

        static void OnServerError(string log)
        {
        }
    }
}
