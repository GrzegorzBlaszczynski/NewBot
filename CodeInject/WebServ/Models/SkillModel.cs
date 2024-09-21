using System.Collections.Generic;


namespace Winebotv2.WebServ.Models
{
    public class PlayerSkillModel
    {
        public List<SkillInfo> UnUsedSkillList { get; set; }   = new List<SkillInfo>();
        public List<SkillInfo> SkillInUseList { get; set; } = new List<SkillInfo>();
    }
}
