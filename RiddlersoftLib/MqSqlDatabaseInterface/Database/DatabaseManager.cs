using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MySqlDI.Database
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warining")]
    public class DatabaseManager : MySqlWebServer
    {
        public delegate void _pageError(ErrorTypes error);
        public _pageError OnPageError;

        public string LoginUserName, LoginPassword;

        public int Medals, Wins, Losses, Draws, Credits, SpairParts, Rank, Level, Protection;

        public DatabaseManager(string serverUrl, string ServerName) : base(serverUrl, ServerName)
        {
        }

        private string GenerateRandomPassword()
        {
            Random rd = new Random();
            Random rd2 = new Random(rd.Next(99999999));
            int x = 0;
            while (x < 999999999)
            {
                x++;
            };
            Random rd3 = new Random(rd2.Next(99999999));

            string str = "";
            for (int i = 0; i < 10; i++)
            {
                str += rd.Next(9);
                str += rd2.Next(99);
                str += rd3.Next(9);
            }
            return str;
        }

        public async void DownloadBasicData()
        {
            await SendData("http://riddlersoftgames.co.uk/database/osrh2h/getuserinfo.php", HttpMethod.Post, null,
                RequestType.Normal, DbAction.DownloadBasicData);
        }

        public void SendRequest(DatabaseRequest request, string args)
        {
            RequestData result = request.Encoder(args);
            if (result == null)
            {
                CreatePageError(ErrorTypes.InvalidRequest);
                return;
            }

            try
            {
                SendData(request.URL + result.Get, HttpMethod.Post, result.Post,
                   request.Type, request.ActionType);
            }
            catch
            {
                _waitingForRequest = false;
                CreatePageError(ErrorTypes.WebError);
            }
        }

        public void SendRequest(DatabaseRequest request)
        {
           SendRequest(request, null);
        }
        
        public async void CreateNewUser(string name)
        {
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                { "username", name },
                { "password", GenerateRandomPassword() }
            };

            await SendData("http://riddlersoftgames.co.uk/database/osrh2h/createuser.php", HttpMethod.Post, data, RequestType.Normal, DbAction.CreateUser);
        }

        public override object DecodePage(HttpResponseMessage responce, string htmlPage, object mydata)
        {
            try
            {
                DbAction dbRequest = (DbAction)mydata;

                switch (dbRequest)
                {
                    case DbAction.CreateUser:
                        string id = htmlPage.Substring(0, htmlPage.Length - 1);
                        return id;

                    case DbAction.DownloadBasicData:
                        Dictionary<string, List<string>> database = MySqlWebServer.DecodeMySqlDatabase(htmlPage); //convert to readable data
                        Medals = Convert.ToInt32(database["Medals"][0]);
                        Credits = Convert.ToInt32(database["Credits"][0]);
                        Losses = Convert.ToInt32(database["Losses"][0]);
                        Draws = Convert.ToInt32(database["Draws"][0]);
                        Wins = Convert.ToInt32(database["Wins"][0]);
                        SpairParts = Convert.ToInt32(database["SpairParts"][0]);
                        Rank = Convert.ToInt32(database["Rank"][0]);
                        Level = Convert.ToInt32(database["Level"][0]);
                        Protection = Convert.ToInt32(database["Protection"][0]);
                        return database;

                    case DbAction.SendAction:
                        return htmlPage;
                }
            }
            catch
            { }

            return true;
        }

        public override void CreatePageError(ErrorTypes error)
        {
            if (OnPageError != null)
                OnPageError(error);
        }
    }
}
