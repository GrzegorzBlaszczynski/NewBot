using Winebotv2.Actors;
using Winebotv2.MemoryTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Winebotv2.Hunt
{
    public unsafe class DefaultHunt : EmptyHuntSetting
    {
        public int SkillIndex = 0;
        public IObject Target;
        public Vector3 HuntingAreaCenter { get; set; }
        public int Radius { get; set; } = 50;
        private cBot WinFormMenu;


        public DefaultHunt()
        {
        }

        public DefaultHunt(List<MobInfo> monstersToAttackList, Vector3 huntingAreaCenter, int radius, List<Skills> skillList, bool normalAttack, cBot WinForm)
        {
            HuntingAreaCenter = huntingAreaCenter;
            Radius = radius;
            ListOfMonstersToAttack = monstersToAttackList;
            WinFormMenu = WinForm;
            BotSkills = skillList;
            NormalAttack = normalAttack;
            Target = null;
        }

        public override void AddSkill(Skills skill, SkillTypes type)
        {
            base.AddSkill(skill, type);
            WinFormMenu.SkillListUpdate();
        }

        public override void RemoveSkill(Skills skill)
        {
            base.RemoveSkill(skill);
            WinFormMenu.SkillListUpdate();
        }

        public override void Update()
        {
            if (this.SkillIndex < this.BotSkills.Count - 1)
            {
                this.SkillIndex++;
            }
            else
            {
                this.SkillIndex = 0;
            }

            
            List<ushort> buffs = Player.GetPlayer.GetBuffsIdList();
            List<Skills> BotBuff2Use = BotSkills.Where(x => x.SkillType == SkillTypes.Buff && !buffs.Any(b => b == x.skillInfo.ID)).ToList();

            if (BotBuff2Use.Count > 0)
            {
                Player.GetPlayer.CastSkill(BotBuff2Use.FirstOrDefault().SkillIndex);
                Thread.Sleep(100);
            }
            else
            {
                if (Target == null || !NPC.GetNPCsList().Where(x => x.GetType() == typeof(NPC)).Any(x => (long)x.ObjectPointer == (long)Target.ObjectPointer) || ((NPC)Target).Hp <= 0)
                {
                    this.Target = NPC.GetNPCsList().Where(x => x.GetType() == typeof(NPC))
                    .Where(x => ListOfMonstersToAttack.Cast<MobInfo>().Any(y => ((NPC)x).Info != null && y.ID == ((NPC)x).Info.ID))
                    .Where(x => ((NPC)x).CalcDistance(HuntingAreaCenter.X, HuntingAreaCenter.Y, HuntingAreaCenter.Z) < Radius).FirstOrDefault(x => ((NPC)x).Hp > 0);
                }

                if (Target != null)
                {
                    if (this.BotSkills.Count > 0)
                    {
                        Skills Skill2Cast = Player.GetPlayer.GetSkillsList().FirstOrDefault(x => x.skillInfo.ID == this.BotSkills[this.SkillIndex].skillInfo.ID);

                        if (this.BotSkills[this.SkillIndex].SkillType == SkillTypes.AttackSkill)
                        {
                            Player.GetPlayer.CastSkill(Target, Skill2Cast.SkillIndex);
                        }
                    }
                    if (NormalAttack == true)
                        Player.GetPlayer.CastSkill(this.Target);
                }
            }
            base.Update();
        }
    }
}
