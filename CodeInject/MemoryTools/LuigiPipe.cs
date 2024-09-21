using Winebotv2.Actors;
using System.Numerics;
using System.IO.Pipes;
using System.IO;
using System.Threading;

namespace Winebotv2.MemoryTools
{
    public class LuigiPipe
    {
        public static LuigiPipe Instance
        {
            get
            {
                return _instance;
            }
        }
        private static LuigiPipe _instance = new LuigiPipe();

        public NamedPipeClientStream client { get; set; }

        private Thread _messageProcesThread { get; set; }

        public bool isReady = false;
        StreamReader _reader;
        StreamWriter _writer;

        public string SendFunction(string command)
        {
            _writer.WriteLine(command);
            _writer.Flush();
           return _reader.ReadLine();
        }

        public void SendProcedure(string command)
        {
            _writer.WriteLine(command);
            _writer.Flush();
        }



        public void OpenPipe(int id)
        {
            client = new NamedPipeClientStream($"Luigi{id}");
            client.Connect();

            _reader = new StreamReader(client);
            _writer = new StreamWriter(client);
            string read = _reader.ReadLine();
            Parser(read);

        }

        private LuigiPipe()
        {

            /*  _messageProcesThread = new Thread(new ThreadStart(() =>
              {
                  using (var reader = new StreamReader(client))
                  {
                      while (true)
                      {
                          string read = reader.ReadLine();
                          Parser(read);
                      }
                  }
              }));
            _messageProcesThread.Start();*/
        }

        public void Parser(string message)
        {
            string[] pushedValues = message.Split(';');

            foreach (string pushedValue in pushedValues)
            {
                string[] keyAndValue = pushedValue.Split(':');

                if (keyAndValue[0]=="BaseAddres")
                {
                    GameHackFunc.Game.ClientData.BaseAddres = long.Parse(keyAndValue[1]);
                }
                else if (keyAndValue[0]== "GameBaseAddres"){
                    GameHackFunc.Game.ClientData.GameBaseAddres = long.Parse(keyAndValue[1]);
                }
                else if (keyAndValue[0] == "PlayerAddres")
                {
                    GameHackFunc.Game.ClientData.PlayerAddres = long.Parse(keyAndValue[1]);
                }
                else if (keyAndValue[0] == "YouCanEnter")
                {
                    isReady = true;
                }

            }
        }


        public void PickUp(Item item)
        {
            using(var writer = new StreamWriter(client))
            {
                writer.WriteLine($"PickUp:{item.ObjectPointer}");
            }
        }
        public void CastSpell(IObject target, int skillIndex)
        {

        }

      /*  public void MoveToPoint(Vector2 position);
        public void CastSpell(int skillIndex);
        public void Attack(int targedID);
        public void ItemUse(long itemAddr)*/
    }
}
