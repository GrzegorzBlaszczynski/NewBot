using Winebotv2.MemoryTools;
using Winebotv2.WebServ.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Winebotv2.Actors
{
    internal unsafe class OtherPlayer : IObject, IPlayer
    {
        #region Properties
        public long ObjectPointer { get; set; }
        public ushort ID { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public int MaxHp
        {
            get
            {
                return Rudy.Instance.ReadInt(new UIntPtr((ulong)ObjectPointer + 0xf8));
            }
        }
        public int Hp
        {
            get
            {
                return Rudy.Instance.ReadInt(new UIntPtr((ulong)ObjectPointer + 0xf0));
            }
        }
        public int MaxMp { get; set; }
        public int Mp { get; set; }
        public string Name { get; set; } = "";
        public short BuffCount { get; set; }
        public int modelNaME { get; set; }
        #endregion

        public OtherPlayer(ulong Entry)
        {
            ObjectPointer = (long)Entry;

            X =  Rudy.Instance.ReadFloat(new UIntPtr(Entry) + 0x10);
            Y =  Rudy.Instance.ReadFloat(new UIntPtr(Entry) + 0x14);
            Z =  Rudy.Instance.ReadFloat(new UIntPtr(Entry) + 0x18);
            ID = Rudy.Instance.ReadUShort(new UIntPtr(Entry) + 0x1c);
            Name = Rudy.Instance.ReadString(new UIntPtr(Entry) + 0x9A8);

        }




        public double CalcDistance(IObject targerObject)
        {
            return Math.Sqrt(Math.Pow((targerObject.X / 100) - (this.X / 100), 2) + Math.Pow((targerObject.Y / 100) - (this.Y / 100), 2) + Math.Pow((targerObject.Z / 100) - (this.Z / 100), 2));
        }

        public double CalcDistance(float x, float y, float z)
        {
            return Math.Sqrt(
                  Math.Pow((x / 100) - (this.X / 100), 2)
                + Math.Pow((y / 100) - (this.Y / 100), 2)
                + Math.Pow((z / 100) - (this.Z / 100), 2));
        }

        public override string ToString()
        {
            return $"[{(ID).ToString("X")}] {Name}";
        }

        public string ToWSObject()
        {
            string playerJson =
                JsonConvert.SerializeObject(new PlayerInfoModel()
                {
                    Hp = Hp,
                    MaxHp = MaxHp,
                    Mp = Mp,
                    MaxMp = MaxMp,
                    X = X,
                    Y = Y,
                    Z = Z,
                    BuffCount = BuffCount,
                    Name = Name
                }, Formatting.Indented);

            return playerJson;
        }
    }
}
