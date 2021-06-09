using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net;

namespace MySqlDI
{
    /// <summary>
    /// used to keep track of what type of request the server has issued
    /// </summary>
    public enum RequestType { Login = 0, Normal = 1, NotRequireLogin }

    public enum ErrorTypes { NotLogedIn, InvalidRequest, None, Unknownen, WebError}
    /// <summary>
    /// a class to handle any and all server interactions
    /// </summary>
    public abstract class MySqlWebServer
    {
        /// <summary>
        /// keeps track of cookies
        /// </summary>
        private CookieContainer cookie = new CookieContainer();

        /// <summary>
        /// a dictionary of error messeges and there releative enums
        /// </summary>
        private Dictionary<string, ErrorTypes> _errorList = new Dictionary<string, ErrorTypes>()
        {
            { "login required", ErrorTypes.NotLogedIn },
            { "error to be defined", ErrorTypes.InvalidRequest },
        };

        /// <summary>
        /// the type of request
        /// </summary>
        protected RequestType _requestType;
        /// <summary>
        /// the name for this database
        /// </summary>
        public string Name;

        /// <summary>
        /// weather any webserver is currently waiting for a request or not
        /// </summary>
        protected static bool _waitingForRequest;
        public bool WatingForRequest { get { return _waitingForRequest; } }

        /// <summary>
        /// keeps a log of all activity
        /// </summary>
        protected string _log;
        public string Log { get { return _log; } }


        /// <summary>
        /// keeps a log of how much bandwidth this webserver has used
        /// </summary>
        protected int _bandwidthUsed;
        public int BandwidthUsed { get { return _bandwidthUsed; } }

        /// <summary>
        /// keeps a log of the total amount of bandwith used by the entire application 
        /// within all WebServers
        /// </summary>
        protected static int _appBandwidthUsed;
        public static int AppBandwidthUsed { get { return _appBandwidthUsed; } }

        /// <summary>
        /// this will fire when there is a problem loading the page
        /// </summary>
        /// <param name="log"></param>
        public delegate void _onHTTPFailure(string log);
        public _onHTTPFailure OnHTTPFailure;

        /// <summary>
        /// this fires when the page loads successfully
        /// </summary>
        /// <param name="o"></param>
        public delegate void __onHTTPSuccesses(object o);
        public __onHTTPSuccesses OnHTTPSuccesses;

        /// <summary>
        /// this fires when the page loads successfully
        /// </summary>
        /// <param name="o"></param>
        public delegate void __onLoginSuccess(object o);
        public __onLoginSuccess OnLoginSuccess;


        protected HttpClientHandler _webHandler;

        /// <summary>
        /// handles all webrequests
        /// </summary>
        protected HttpClient _webRequest;

        /// <summary>
        /// the servers url
        /// </summary>
        protected string _serverAddress;
        public string ServerAddress { get { return _serverAddress; } }

        /// <summary>
        /// debug option turn it on to output log to output window
        /// </summary>
        public bool WriteLogToOutputWindow = false;
        /// <summary>
        /// if we are connected to the server or not
        /// </summary>
        public bool IsConnected
        {
            get; protected set;
        }

        /// <summary>
        /// create the database
        /// </summary>
        /// <param name="serverUrl"></param>
        public MySqlWebServer(string serverUrl, string ServerName)
        {
            _serverAddress = serverUrl; //set the url
            Name = ServerName;
            //make the web request
            _webHandler = new HttpClientHandler()
            {
                UseCookies = true,
                UseDefaultCredentials = false,
                CookieContainer = cookie,
                
            };
            _webRequest = new HttpClient(_webHandler); //inizalize the http client
            WriteLineToLog("Server " + Name + " Inizalized!");
            
        }


        /// <summary>
        /// estimates the current bandwith used
        /// </summary>
        /// <param name="page"></param>
        private void UpdateUsedBandwidth(string page)
        {
            //estimate the amount of bandwith used
            //this estimation does not include everything
            int estimatedPageSize = page.Length * 8; //saying that each char is a byte
            _bandwidthUsed += estimatedPageSize; //update the bandwith used for this class
            _appBandwidthUsed += estimatedPageSize; //update the total bandwith used by the app for all webrequests
        }

        /// <summary>
        /// creates a http request
        /// </summary>
        /// <param name="address">the web adress</param>
        /// <param name="method">the methord POST or GET</param>
        /// <returns></returns>
        public HttpRequestMessage CreateWebRequest(string address, HttpMethod method)
        {
            return new HttpRequestMessage(method, address);
            //set the address
        }

        public Dictionary<string, string> LoginCredentials { get; protected set; }

        /// <summary>
        /// logs the user in to the session
        /// </summary>
        /// <param name="username">the username to login with</param>
        /// <returns></returns>
        public virtual async Task<bool> Login(Dictionary<string, string> data)
        {
            LoginCredentials = data; //save login information
            if (_webRequest.BaseAddress == null)
                _webRequest.BaseAddress = new Uri(_serverAddress);
            return await SendData(_serverAddress, HttpMethod.Post, data, RequestType.Login, null);
        }

        public void Disconnect()
        {
            IsConnected = false;
        }
        
        /// <summary>
        /// this is run when the request was compleated sucessfully
        /// </summary>
        /// <param name="result"></param>
        protected void OnRequestFullfulled(HttpResponseMessage result, object mydata)
        {
            try
            {
                WriteToLog($" >> Request Sent");
                result.EnsureSuccessStatusCode(); //make sure there is a result
                
                if (_requestType == RequestType.Login) //if we were trying to connect to the server
                {
                    if (IsConnected) //if we are not connect
                    {
                        WriteLineToLog("Already connect can not connect again");
                        
                        if (OnHTTPFailure != null)
                            OnHTTPFailure(_log);
                    }
                    else
                    {
                       
                        WriteLineToLog("Connected to " + Name);
                        DecodeLogin(result, mydata);
                    }
                   
                    _waitingForRequest = false;
                    return; //exit as we dont need to decode the page we have connect to
                }

                WriteToLog($" >> Decoding Page");
                PreDecodePage(result, mydata);
                
            }
            catch (Exception x)
            {
                WriteLineToLog(" ========UNHANDLED EXCEPTION=======\n"+ x); //output to the log
                if (OnHTTPFailure != null)
                    OnHTTPFailure(_log);
                
            }

            _waitingForRequest = false;

        }

        protected virtual async void DecodeLogin(HttpResponseMessage result, object mydata)
        {
            string page = await result.Content.ReadAsStringAsync(); //get the page
            if (OnLoginSuccess != null)
            {
                if (page.Contains("s0"))
                    IsConnected = true; //sucess
                OnLoginSuccess(page);
            }
        }

        /// <summary>
        /// start the prossesing of the receved web page
        /// </summary>
        /// <param name="result"></param>
        private async void PreDecodePage(HttpResponseMessage result, object mydata)
        {
            string page = await result.Content.ReadAsStringAsync(); //get the page
            UpdateUsedBandwidth(page); //update bandwith used

            //now we check for errors on page
            int i = page.IndexOf("#error-=#");
            if (i >= 0) //if error
            {
                i += 9;
                string msg = page.Substring(i, page.Length - i);
                ErrorTypes errmsg = ErrorTypes.None;
                if (_errorList.ContainsKey(msg))
                    errmsg = _errorList[msg];
                else
                    errmsg = ErrorTypes.Unknownen;
                CreatePageError(errmsg); //through page error
            }
            else
            {
                object o = DecodePage(result, page, mydata); //decode the page
                if (OnHTTPSuccesses != null)
                    OnHTTPSuccesses(o);
            }
        }

        /// <summary>
        /// decodes the webpage
        /// </summary>
        /// <param name="responce">the http responce from the server</param>
        /// <param name="htmlPage">a string containing the webpage</param>
        /// <returns></returns>
        public abstract object DecodePage(HttpResponseMessage responce, string htmlPage, object mydata);
      
        /// <summary>
        /// handles page errors
        /// </summary>
        /// <param name="error"></param>
        public abstract void CreatePageError(ErrorTypes error);    

        /// <summary>
        /// sends data to the server
        /// </summary>
        /// <param name="url">the url that we want to access</param>
        /// <param name="method">the methord to send data POST or GET</param>
        /// <param name="data">the data to send</param>
        /// <param name="requesttype">the request type</param>
        /// <returns></returns>
        protected async Task<bool> SendData(string url, HttpMethod method, Dictionary<string, string> data, RequestType requesttype, object mydata)
        {
            WriteLineToLog($"---Sending new command: {url}");
            if (requesttype != RequestType.NotRequireLogin)
                if (!IsConnected && requesttype != RequestType.Login)
                {
                    WriteLineToLog("Error you must be connected to server to send data");
                    
                    return false; //dont send data
                }

            if (_waitingForRequest) //here is the check to see if the server is bussy or not
            {
               WriteLineToLog("Server is bussy try again later");
               
                return false;
            }
            
            _waitingForRequest = true;

            _requestType = requesttype; //set the type of request
            HttpRequestMessage request = CreateWebRequest(url, method);
            
            if (data != null)
            {
                FormUrlEncodedContent encodedData = new FormUrlEncodedContent(data);
                request.Content = encodedData;
            }
            else
                request.Content = null;

            WriteToLog($" >> Sender Request");
            OnRequestFullfulled(await _webRequest.SendAsync(request, HttpCompletionOption.ResponseContentRead), mydata); //get the result
            
            return true;
        }

        /// <summary>
        /// writes to the log
        /// </summary>
        /// <param name="msg">the text to add to the log</param>
        protected void WriteToLog(string msg)
        {
            if (WriteLogToOutputWindow)
                System.Diagnostics.Debug.WriteLine(msg);
            _log += msg;
        }

        /// <summary>
        /// writes to the log and creates a new line after it has finised
        /// </summary>
        /// <param name="msg">the text to add to the log</param>
        protected void WriteLineToLog(string msg)
        {
            if (WriteLogToOutputWindow)
                System.Diagnostics.Debug.WriteLine(msg);
            _log += msg;
            _log += "\n ";
        }

        /// <summary>
        /// clears the log
        /// </summary>
        protected void ClearLog()
        {
            _log = "";
        }

        //=======================static functoins=========================

        private static string DecodeRow(string htmlPage, ref int textPointer)
        {
            int rowStart = htmlPage.IndexOf("<tr>", textPointer); //get row start
            if (rowStart < 0)
                return null;
            int rowEnd = htmlPage.IndexOf("</tr>", rowStart);
            textPointer = rowEnd;
            return htmlPage.Substring(rowStart, rowEnd - rowStart);
        }

        private static List<string> GetHeaders(string row)
        {
            if (row == null)
                return new List<string>();
            List<string> headers = new List<string>();
            int tPointer = row.IndexOf("<th>");
            if (tPointer >= 0)
                tPointer += 4;
            int ePointer;
            while (tPointer >= 0 && tPointer < row.Length) //make sure the pointer is always in range of the string
            {
                ePointer = row.IndexOf("</th>", tPointer);

                if (ePointer >= 0 && ePointer < row.Length)
                {
                    headers.Add(row.Substring(tPointer, ePointer - tPointer));
                    tPointer = row.IndexOf("<th>", ePointer);
                    if (tPointer >= 0)
                        tPointer += 4;
                }
                else
                    tPointer = -1;

            }
            return headers;
        }

        private static List<string> GetCells(string row)
        {
            List<string> cells = new List<string>();
            int tPointer = row.IndexOf("<td>");
            if (tPointer >= 0)
                tPointer += 4;
            int ePointer;
            while (tPointer >= 0 && tPointer < row.Length) //make sure the pointer is always in range of the string
            {
                ePointer = row.IndexOf("</td>", tPointer);

                if (ePointer >= 0 && ePointer < row.Length)
                {
                    cells.Add(row.Substring(tPointer, ePointer - tPointer));
                    tPointer = row.IndexOf("<td>", ePointer);
                    if (tPointer >= 0)
                        tPointer += 4;
                }
                else
                    tPointer = -1;

            }
            return cells;
        }

        public static Dictionary<string, List<string>> DecodeMySqlDatabase(string page)
        {
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();

            int textPointer = 0; //keeps track of the rows

            string row = DecodeRow(page,ref textPointer);

            List<string> headers = GetHeaders(row);

            if (headers == null || headers.Count == 0)
                return null;

            foreach (string str in headers)
                data.Add(str, new List<string>()); //add all the headers

            List<string> tableData = new List<string>();
            while (textPointer >= 0 && textPointer < page.Length)
            {
                tableData.Clear();
                row = DecodeRow(page, ref textPointer); //decode next row
                if (row == null)
                    return data;
                tableData = GetCells(row);

                for (int i =0; i < tableData.Count; i++)
                {
                    data[headers[i]].Add(tableData[i]); //add the data
                }
            }
            return data;
        }
    }


}
