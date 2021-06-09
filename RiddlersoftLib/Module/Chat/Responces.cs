using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Modules.Chat
{
    public class Responces
    {
        public List<Responce> GetResponces()
        {
            return _responces;
        }

        private List<Responce> _responces = new List<Responce>();

        public void Add(Responce responce)
        {
            _responces.Add(responce);
        }



        //private Action<Responce> Callback;

        public static Responces Ok()
        {
            Responces r = new Chat.Responces();
            r.Add(new Chat.Responce("Ok"));
            return new Responces();
        }
    }
}
