﻿

namespace Winebotv2.WebServ.Models
{
   public class NPCModel
    {
        public string Name { get; set; } = "";
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public int Hp { get; set; }
        public int MaxHp { get; set; }


        public string GetProcHP()
        {
            return $"{((float)Hp) * 100.0f / ((float)MaxHp)}".Replace(",", ".");
        }
    }
}
