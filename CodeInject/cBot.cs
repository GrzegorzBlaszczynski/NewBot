using Winebotv2.Actors;
using Winebotv2.BotStates;
using Winebotv2.Hunt;
using Winebotv2.Modules;
using Winebotv2.PickupFilters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using Winebotv2.UIPanels;
using Winebotv2.UIPanels.Module_Panels;
using System.IO;
using Winebotv2.MemoryTools;

namespace Winebotv2
{
    public unsafe partial class cBot : Form
    {
        public static BotContext BotContext = new BotContext();
        bool BotState = false;
        public List<InvItem> ConsumeItems = new List<InvItem>();

        public cBot()
        {
            InitializeComponent();

            BotContext.Start(
                     new HuntState(
                         new HealerHunt(lMonster2Attack.Items.Cast<MobInfo>().ToList(),
                         new Vector3(float.Parse(tXHuntArea.Text), float.Parse(tYHuntArea.Text),
                         float.Parse(tZHuntArea.Text)), int.Parse(tHuntRadius.Text), lUseSkill.Items.OfType<Skills>().ToList(), lPlayers2Heal.Items.OfType<IObject>().ToList(), 
                         int.Parse(tHealWhenProc.Text), cNormalAttackEnable.Checked, this)));
            BotContext.Stop();
        }

        private void bSkillRefresh_Click(object sender, EventArgs e)
        {
            lSkillList.Items.Clear();
            var activeSkills = Player.GetPlayer.GetSkillsList().Where(x => x.skillInfo.Type != "Passive").ToArray();
            lSkillList.Items.AddRange(activeSkills);

            comboBox5.Items.Clear();
            comboBox5.Items.AddRange(Player.GetPlayer.GetSkillsList().ToArray());
        }

        private void bSkillAdd_Click(object sender, EventArgs e)
        {
            if (lSkillList.SelectedIndex != -1)
            {
                Skills selectedSkill = (Skills)lSkillList.SelectedItem;
                if (selectedSkill.skillInfo.Type == "Offensive")
                {
                    BotContext.GetState<HuntState>("HUNT").HuntInstance.AddSkill(selectedSkill, SkillTypes.AttackSkill);
                    lUseSkill.Items.Add(selectedSkill);

                    lSkillList.Items.Remove(selectedSkill);
                }
                else
                {
                    MessageBox.Show("Only offensive skills can be added.", "Invalid Skill Type", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void bSkillRemove_Click(object sender, EventArgs e)
        {
            if (lUseSkill.SelectedItem != null)
                BotContext.GetState<HuntState>("HUNT").HuntInstance.RemoveSkill((Skills)lUseSkill.SelectedItem);
        }

        public void SkillListUpdate()
        {
            lUseSkill.Items.Clear();
            lHealSkills.Items.Clear();
            lBuffs.Items.Clear();
            lUseSkill.Items.AddRange(BotContext.GetState<HuntState>("HUNT").HuntInstance.BotSkills.Where(x => x.SkillType == SkillTypes.AttackSkill).ToArray());
            lHealSkills.Items.AddRange(BotContext.GetState<HuntState>("HUNT").HuntInstance.BotSkills.Where(x => x.SkillType == SkillTypes.HealTarget).ToArray());
            lBuffs.Items.AddRange(BotContext.GetState<HuntState>("HUNT").HuntInstance.BotSkills.Where(x => x.SkillType == SkillTypes.Buff).ToArray());
        }

        public void PorionSettingsUpdate()
        {

        }
        bool tdelete = false;
        private void timer2_Tick(object sender, EventArgs e)
        {
            BotContext.Update();
            listBox1.Items.Clear();
            lNearItemsList.Items.Clear();
            listBox1.Items.AddRange(NPC.GetNPCsList().ToArray());
            lNearItemsList.Items.AddRange(Player.GetPlayer.GetAllItemsAroundPlayerList().ToArray());
            Random rand = new Random();
            timer2.Interval = rand.Next(500, 1000);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            lFullMonsterList.Items.Clear();
            lFullMonsterList.Items.AddRange(DataBase.GameDataBase.MonsterDatabase.Where(x => x.Name != "" && x.Name.ToUpper().Contains(tSearchMobTextBox.Text.ToUpper())).ToArray());
        }

        private void bAddMonster2Attack_Click(object sender, EventArgs e)
        {
            if (lFullMonsterList.SelectedItem != null && lMonster2Attack.Items.Cast<MobInfo>().FirstOrDefault(x => x.ID == ((MobInfo)lFullMonsterList.SelectedItem).ID) == null)
                lMonster2Attack.Items.Add(lFullMonsterList.SelectedItem);
        }

        private void bRemoveMonster2Attack_Click(object sender, EventArgs e)
        {
            if (lMonster2Attack.SelectedIndex != -1)
                lMonster2Attack.Items.Remove(lMonster2Attack.SelectedItem);
        }

        private void bSetArea_Click(object sender, EventArgs e)
        {
            tXHuntArea.Text = (Player.GetPlayer.X).ToString();
            tYHuntArea.Text = (Player.GetPlayer.Y).ToString();
            tZHuntArea.Text = (Player.GetPlayer.Z).ToString();
        }


        private void cbHealHPItem_DropDown(object sender, EventArgs e)
        {
            cbHealHPItem.Items.Clear();
            cbHealHPItem.Items.AddRange(Player.GetPlayer.GetConsumableItemsFromInventory(cbHealHPItem.Items.OfType<InvItem>().ToList()).Where(x=>x.ItemType==0xA).ToArray());
        }

        private void cbHealMPItem_DropDown(object sender, EventArgs e)
        {
            cbHealMPItem.Items.Clear();
            cbHealMPItem.Items.AddRange(Player.GetPlayer.GetConsumableItemsFromInventory(cbHealMPItem.Items.OfType<InvItem>().ToList()).Where(x => x.ItemType == 0xA).ToArray());
        }


        private Skills GetSkillFromConfigFile(string line)
        {
            string[] data = line.Split(';');

            return new Skills(DataBase.GameDataBase.SkillDatabase.FirstOrDefault(x => x.ID == int.Parse(data[1])), (SkillTypes)int.Parse(data[0]));
        }

        private void LoadConfig(string name)
        {
            StreamReader config = new StreamReader(name + ".skills.txt");

            lUseSkill.Items.Clear();
            lBuffs.Items.Clear();
            lHealSkills.Items.Clear();

            while (!config.EndOfStream)
            {
                Skills skill = GetSkillFromConfigFile(config.ReadLine());

                switch (skill.SkillType)
                {
                    case SkillTypes.AttackSkill:
                        BotContext.GetState<HuntState>("HUNT").HuntInstance.AddSkill(skill, SkillTypes.AttackSkill);
                        lUseSkill.Items.Add(skill);
                        break;
                    case SkillTypes.Buff:
                        BotContext.GetState<HuntState>("HUNT").HuntInstance.AddSkill(skill, SkillTypes.Buff);
                        lBuffs.Items.Add(skill);
                        break;
                    case SkillTypes.HealTarget:
                        BotContext.GetState<HuntState>("HUNT").HuntInstance.AddSkill(skill, SkillTypes.HealTarget);
                        lHealSkills.Items.Add(skill);
                        cEnableHealParty.Checked = true;
                        break;

                }
            }
            config.Close();

            config = new StreamReader(name + ".mobs.txt");
            lMonster2Attack.Items.Clear();
            while (!config.EndOfStream)
            {
                int id = int.Parse(config.ReadLine());
                lMonster2Attack.Items.Add(DataBase.GameDataBase.MonsterDatabase.FirstOrDefault(x => x.ID == id));
            }
            config.Close();



            if (File.Exists(name + ".AutoPotion.txt"))
            {
                cbHealHPItem.Items.Clear();
                cbHealMPItem.Items.Clear();
                config = new StreamReader(name + ".AutoPotion.txt");
                string[] data = config.ReadLine().Split(';');

                if (data.Length < 3)
                {
                    config.Close();
                    return;
                }


                int itemId = int.Parse(data[2]);
                InvItem ihp = Player.GetPlayer.GetConsumableItemsFromInventory(new List<InvItem>()).FirstOrDefault(x => x.ItemData == itemId && x.ItemType == 0xA);

                if (ihp == null)
                {
                    MessageBox.Show($"There is no longer Hp potion avaiable in inventory: {DataBase.GameDataBase.UsableItemsDatabase.FirstOrDefault(x => x.ID == itemId).DisplayName}");
                    config.Close();
                    return;
                }
                cbHealHPItem.SelectedIndex = cbHealHPItem.Items.Add(ihp);
                tHPPotionUseProc.Text = data[0];
                tHpDurr.Text = data[1];
                data = config.ReadLine().Split(';');

                itemId = int.Parse(data[2]);
                InvItem imp = Player.GetPlayer.GetConsumableItemsFromInventory(new List<InvItem>()).FirstOrDefault(x => x.ItemData == itemId && x.ItemType == 0xA);

                if (ihp == null)
                {
                    MessageBox.Show($"There is no longer Mp potion avaiable in inventory: {DataBase.GameDataBase.UsableItemsDatabase.FirstOrDefault(x => x.ID == itemId).DisplayName}");
                    config.Close();
                    return;
                }
                else
                {
                    cbHealMPItem.SelectedIndex = cbHealMPItem.Items.Add(imp);
                    tMPPotionUseProc.Text = data[0];
                    tMpDurr.Text = data[1];
                }
                cAutoPotionEnabled.Checked = true;
                config.Close();
            }
        }

        private void SaveConfig(string configName)
        {
            StreamWriter wConfig = new StreamWriter(configName + ".skills.txt", false);

            foreach (Skills skill in lUseSkill.Items)
            {
                wConfig.WriteLine((int)skill.SkillType + ";" + skill.skillInfo.ID);
            }
            foreach (Skills skill in lHealSkills.Items)
            {
                wConfig.WriteLine((int)skill.SkillType + ";" + skill.skillInfo.ID);
            }
            foreach (Skills skill in lBuffs.Items)
            {
                wConfig.WriteLine((int)skill.SkillType + ";" + skill.skillInfo.ID);
            }
            wConfig.Close();

            wConfig = new StreamWriter(configName + ".mobs.txt", false);

            foreach (MobInfo mob in lMonster2Attack.Items)
            {
                wConfig.WriteLine(mob.ID);
            }
            wConfig.Close();


            if (cbHealHPItem.SelectedItem != null)
            {
                wConfig = new StreamWriter(configName + ".AutoPotion.txt", false);

                wConfig.WriteLine($"{tHPPotionUseProc.Text};{tHpDurr.Text};{((InvItem)cbHealHPItem.SelectedItem).ItemData}");
                wConfig.WriteLine($"{tMPPotionUseProc.Text};{tMpDurr.Text};{((InvItem)cbHealMPItem.SelectedItem).ItemData}");
            }

            wConfig.Close();
        }

        private void bHuntToggle_Click_1(object sender, EventArgs e)
        {
            if (BotState == false)
            {
                List<Skills> SkillList = new List<Skills>();
                SkillList.AddRange(lUseSkill.Items.Cast<Skills>().ToArray());
                SkillList.AddRange(lHealSkills.Items.Cast<Skills>().ToArray());
                if (comboBox5.SelectedIndex != -1)
                {
                    (comboBox5.SelectedItem as Skills).SkillType  = SkillTypes.Revive;
                    SkillList.Add(comboBox5.SelectedItem as Skills);
                }
                SkillList.AddRange(lBuffs.Items.Cast<Skills>().ToArray());

                if (cEnableHealParty.Checked)
                {
                    BotContext.Start(new HuntState(
                        new HealerHunt(lMonster2Attack.Items.Cast<MobInfo>().ToList(),
                        new Vector3(float.Parse(tXHuntArea.Text), float.Parse(tYHuntArea.Text),
                        float.Parse(tZHuntArea.Text)), int.Parse(tHuntRadius.Text), SkillList, lPlayers2Heal.Items.OfType<IObject>().ToList(), int
                        .Parse(tHealWhenProc.Text), cNormalAttackEnable.Checked, this)
                       ));
                }
                else
                {
                    DefaultHunt hunt = new DefaultHunt(lMonster2Attack.Items.Cast<MobInfo>().ToList(),
                            new Vector3(float.Parse(tXHuntArea.Text), float.Parse(tYHuntArea.Text),
                            float.Parse(tZHuntArea.Text)), int.Parse(tHuntRadius.Text), SkillList, cNormalAttackEnable.Checked, this);

                    if (comboBox2.SelectedIndex != -1)
                        hunt.AddModule((comboBox2.SelectedItem as IModuleUI).GetModule());

                    BotContext.Start(
                       new HuntState(hunt
                           ));
                }
            }
            else
            {
                BotContext.Stop();
            }
            BotState = !BotState;
            bHuntToggle.Text = BotState ? "Stop" : "Start";
        }

        private void cFilterMaterials_CheckedChanged(object sender, EventArgs e)
        {
            if (cFilterMaterials.Checked)
            {
                ((QuickFilter)BotContext.Filter).AddToPick(ItemType.Material);
            }
            else
            {
                ((QuickFilter)BotContext.Filter).RemoveFromPick(ItemType.Material);
            }
        }

        private void cFilterArmor_CheckedChanged(object sender, EventArgs e)
        {
            if (cFilterArmor.Checked)
            {
                ((QuickFilter)BotContext.Filter).AddToPick(ItemType.ChestArmor);
            }
            else
            {
                ((QuickFilter)BotContext.Filter).RemoveFromPick(ItemType.ChestArmor);
            }
        }

        private void cFilterGloves_CheckedChanged(object sender, EventArgs e)
        {
            if (cFilterGloves.Checked)
            {
                ((QuickFilter)BotContext.Filter).AddToPick(ItemType.Gloves);
            }
            else
            {
                ((QuickFilter)BotContext.Filter).RemoveFromPick(ItemType.Gloves);
            }
        }

        private void cFilterHat_CheckedChanged(object sender, EventArgs e)
        {
            if (cFilterHat.Checked)
            {
                ((QuickFilter)BotContext.Filter).AddToPick(ItemType.Hat);
            }
            else
            {
                ((QuickFilter)BotContext.Filter).RemoveFromPick(ItemType.Hat);
            }
        }

        private void cFilterShoes_CheckedChanged(object sender, EventArgs e)
        {
            if (cFilterShoes.Checked)
            {
                ((QuickFilter)BotContext.Filter).AddToPick(ItemType.Shoes);
            }
            else
            {
                ((QuickFilter)BotContext.Filter).RemoveFromPick(ItemType.Shoes);
            }
        }

        private void cFilterUsable_CheckedChanged(object sender, EventArgs e)
        {
            if (cFilterUsable.Checked)
            {
                ((QuickFilter)BotContext.Filter).AddToPick(ItemType.UsableItem);
            }
            else
            {
                ((QuickFilter)BotContext.Filter).RemoveFromPick(ItemType.UsableItem);
            }
        }

        private void cFilterWeapon_CheckedChanged(object sender, EventArgs e)
        {
            if (cFilterWeapon.Checked)
            {
                ((QuickFilter)BotContext.Filter).AddToPick(ItemType.Weapon);
            }
            else
            {
                ((QuickFilter)BotContext.Filter).RemoveFromPick(ItemType.Weapon);
            }
        }

        private void cFilterShield_CheckedChanged(object sender, EventArgs e)
        {
            if (cFilterShield.Checked)
            {
                ((QuickFilter)BotContext.Filter).AddToPick(ItemType.Shield);
            }
            else
            {
                ((QuickFilter)BotContext.Filter).RemoveFromPick(ItemType.Shield);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AdvancedFilterForm advFilterWindow = new AdvancedFilterForm(BotContext.Filter);
            advFilterWindow.ShowDialog();
        }

        private void cAdvanceEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (!cAdvanceEnable.Checked)
            {
                BotContext.Filter = new QuickFilter();
                SimpleFilterGroup.Controls.OfType<CheckBox>().ToList().ForEach(c => c.Checked = false);
            }
            else
            {
                BotContext.Filter = new AdvancedFilter();
            }
            SimpleFilterGroup.Enabled = !cAdvanceEnable.Checked;
            bAdvancedFilter.Enabled = cAdvanceEnable.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (MobInfo mob in lFullMonsterList.Items)
            {
                if (!lMonster2Attack.Items.Cast<MobInfo>().Any(x => x.ID == mob.ID))
                    lMonster2Attack.Items.Add(mob);
            }
        }


        private void cAutoPotionEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (!cAutoPotionEnabled.Checked)
            {
                BotContext.RemoveModule("AUTOPOTION");
            }
            else
            {
                if (cbHealHPItem.SelectedIndex != -1 && cbHealMPItem.SelectedIndex != -1)
                {
                    AutoPotionModule autoPotion = BotContext.GetModule<AutoPotionModule>("AUTOPOTION");
                    if (autoPotion == null)
                        autoPotion = (AutoPotionModule)BotContext.AddModule(new AutoPotionModule());

                    autoPotion.SetAutoHPpotion(int.Parse(tHPPotionUseProc.Text), int.Parse(tHpDurr.Text), (InvItem)cbHealHPItem.SelectedItem);
                    autoPotion.SetAutoMPpotion(int.Parse(tMPPotionUseProc.Text), int.Parse(tMpDurr.Text), (InvItem)cbHealMPItem.SelectedItem);
                }
                else
                {
                    throw new Exception("Please Select both item in Autopotion option if you want to have it enabled");
                }
            }
        }



        private unsafe void cBot_Load(object sender, EventArgs e)
        {


            this.Text = $"WineBot Character: {Player.GetPlayer.Name}";

            comboBox2.Items.Add(new BackToCenterPanel(lMonster2Attack));
            comboBox2.Items.Add(new GoToPlayerPanel(lMonster2Attack, tXHuntArea, tYHuntArea, tHuntRadius));


            string[] configFiles = Directory.GetDirectories(DataBase.DataPath + "Profiles");

            foreach (var file in configFiles)
            {
                string filename = file.Substring(file.LastIndexOf('\\') + 1);
                comboBox3.Items.Add(filename);
            }

            tProfileName.Text = Player.GetPlayer.Name;

            if (comboBox3.Items.Count != 0)
            {
                comboBox3.SelectedItem = comboBox3.Items.OfType<string>().FirstOrDefault(x => (string)x == Player.GetPlayer.Name);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (lSkillList.SelectedIndex != -1)
                BotContext.GetState<HuntState>("HUNT").HuntInstance.AddSkill((Skills)lSkillList.SelectedItem, SkillTypes.Buff);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (lBuffs.SelectedItem != null)
                BotContext.GetState<HuntState>("HUNT").HuntInstance.RemoveSkill((Skills)lBuffs.SelectedItem);
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(NPC.GetNPCsList().Where(x => x.GetType() == typeof(Player) || x.GetType() == typeof(OtherPlayer)).ToArray());
        }

  
        private NPC npc;

        private void timer3_Tick(object sender, EventArgs e)
        {
         
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel2.Controls.Clear();
            panel2.Controls.Add((UserControl)comboBox2.SelectedItem);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(DataBase.DataPath + tProfileName.Text))
            {
                SaveConfig(DataBase.DataPath + "\\Profiles\\" + tProfileName.Text);
            }
            else
            {
                Directory.CreateDirectory(DataBase.DataPath + "\\Profiles\\" + tProfileName.Text);
                SaveConfig(DataBase.DataPath + "\\Profiles\\" + tProfileName.Text + "\\" + tProfileName.Text);
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            LoadConfig(DataBase.DataPath + "\\Profiles\\" + (string)comboBox3.SelectedItem + "\\" + (string)comboBox3.SelectedItem);
        }


        private void tHealWhenProc_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if key press is a number
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void tHealWhenProc_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(tHealWhenProc.Text, out int value))
            {
                if (value < 0 || value > 100)
                {
                    MessageBox.Show("Please enter a number between 0 and 100.");
                    tHealWhenProc.Text = "90";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(tHealWhenProc.Text))
                {
                    MessageBox.Show("Please enter a valid number.");
                    tHealWhenProc.Text = "90";
                }
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            healskills.Items.Clear();
            healskills.Items.AddRange(Player.GetPlayer.GetSkillsList().Where(x => x.skillInfo.Type != "Passive" &&
                                                                                  x.skillInfo.Type != "Unknow" &&
                                                                                  x.skillInfo.Type != "Summon" &&
                                                                                  x.skillInfo.Type != "Buff" &&
                                                                                  x.skillInfo.Type != "Offensive").ToArray());

  
            comboBox5.Items.Clear();
            comboBox5.Items.AddRange(
                      Player.GetPlayer.GetSkillsList()
                      .Where(skill => skill.skillInfo.Type == "Revive")
                      .ToArray()
            );
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            lPlayersList.Items.Clear();
            lPlayersList.Items.AddRange(NPC.GetNPCsList().Where(x => x.GetType() == typeof(Player) || x.GetType() == typeof(OtherPlayer)).ToArray());
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
                Skills skill = (Skills)healskills.SelectedItem;
                skill.SkillType = SkillTypes.HealTarget;
                lHealSkills.Items.Add((skill));
        }
        private void button7_Click_1(object sender, EventArgs e)
        {
            lPlayers2Heal.Items.Add(lPlayersList.SelectedItem);
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            lPlayers2Heal.Items.Remove(lPlayers2Heal.SelectedItem);
        }

        private void button3_Click_2(object sender, EventArgs e)
        {
            lHealSkills.Items.Remove(lHealSkills.SelectedItem);
        }

        private void healskills_MouseDown(object sender, MouseEventArgs e)
        {
            button23.Enabled = true;
        }

        private void healskills_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (healskills.SelectedIndex != -1)
            {
                button1.Enabled = true; 
            }
            else
            {
                button1.Enabled = false; 
            }
        }

        private void lHealSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lHealSkills.SelectedIndex != -1)
            {
                button3.Enabled = true; 
            }
            else
            {
                button3.Enabled = false;
            }
        }

        private void lPlayersList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lPlayersList.SelectedIndex != -1)
            {
                button7.Enabled = true;
            }
            else
            {
                button7.Enabled = false;
            }
        }

        private void lPlayers2Heal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lPlayers2Heal.SelectedIndex != -1)
            {
                button6.Enabled = true; 
            }
            else
            {
                button6.Enabled = false;
            }
        }

        private void button20_Click_1(object sender, EventArgs e)
        {
            lPlayers2Heal.Items.Clear();
        }

        private void healskills_MouseHover(object sender, EventArgs e)
        {
            Statustext.Text = "Available Skills";
        }

        private void lPlayersList_MouseHover(object sender, EventArgs e)
        {
            Statustext.Text = "Available Players";
        }

        private void lPlayers2Heal_MouseHover(object sender, EventArgs e)
        {
            Statustext.Text = "Players you sellected to Heal";
        }

        private void lHealSkills_MouseHover(object sender, EventArgs e)
        {
            Statustext.Text = "Healing Skills you sellected to use";
        }

        private void cEnableHealParty_MouseHover(object sender, EventArgs e)
        {
            if (cEnableHealParty.Checked != true)
            {
                Statustext.Text = "PartyHeal is Disabled";
            }
            else
            {
                Statustext.Text = "PartyHeal is Enabled";
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            lUseSkill.Items.Clear();
        }

        private void button25_Click(object sender, EventArgs e)
        {
            lBuffs.Items.Clear();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage3"])
            {
                healskills.Items.Clear();
                healskills.Items.AddRange(Player.GetPlayer.GetSkillsList().Where(x => x.skillInfo.Type != "Passive" &&
                                                                                      x.skillInfo.Type != "Unknow" &&
                                                                                      x.skillInfo.Type != "Summon" &&
                                                                                      x.skillInfo.Type != "Buff" &&
                                                                                      x.skillInfo.Type != "Offensive").ToArray());

                comboBox5.Items.Clear();
                comboBox5.Items.AddRange(
                    Player.GetPlayer.GetSkillsList()
                        .Where(skill => skill.skillInfo.Type == "Revive")
                        .ToArray());

            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            lMonster2Attack.Items.Clear();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            lSkillList.Items.Clear();
            var buffSkills = Player.GetPlayer.GetSkillsList()
                                            .Where(x => x.skillInfo.Type == "Offensive")
                                            .ToArray();
            lSkillList.Items.AddRange(buffSkills);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            lSkillList.Items.Clear();
            var buffSkills = Player.GetPlayer.GetSkillsList()
                                            .Where(x => x.skillInfo.Type == "Buff")
                                            .ToArray();
            lSkillList.Items.AddRange(buffSkills);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            lSkillList.Items.Clear();
            var activeSkills = Player.GetPlayer.GetSkillsList().Where(x => x.skillInfo.Type != "Passive").ToArray();
            lSkillList.Items.AddRange(activeSkills);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            float radius;// = (tHuntRadius.Text);//100.0f;
            if (!float.TryParse(tHuntRadius.Text, out radius)) return;

            lMonster2Attack.Items.Clear();

    

            var nearbyMonsters = NPC.GetNPCsList()
                                                .Where(npc => npc.GetType() == typeof(NPC)) 
                                                .Where(npc => Player.GetPlayer.CalcDistance(npc) < radius
                                                           && !((NPC)npc).Info.Name.ToUpper().Contains("(NPC)")      
                                                           && !((NPC)npc).Info.Name.ToUpper().Contains("(SUMMON)"))   
                                                .ToArray();


            foreach (NPC mob in nearbyMonsters)
            {
                if (!lFullMonsterList.Items.Cast<MobInfo>().Any(x => x.ID == mob.Info.ID))
                {
                    lFullMonsterList.Items.Add(mob.Info);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lMonster2Attack.Items.Clear();

            float radius = 100.0f;

            try
            {
                var nearbyMonsters = NPC.GetNPCsList()
                                        .OfType<NPC>() 
                                        .Where(npc =>
                                        {
                                            try
                                            {
                                                return Player.GetPlayer.CalcDistance(npc) < radius
                                                       && npc.Info != null
                                                       && !npc.Info.Name.ToUpper().Contains("(NPC)") 
                                                       && !npc.Info.Name.ToUpper().Contains("(SUMMON)"); 
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine($"Error processing NPC: {ex.Message}");
                                                return false;
                                            }
                                        })
                                        .ToArray();

                foreach (var mob in nearbyMonsters)
                {
                    if (!lMonster2Attack.Items.Cast<MobInfo>().Any(x => x.ID == mob.Info.ID))
                    {
                        lMonster2Attack.Items.Add(mob.Info);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

        }


        private void tHuntRadius_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Only Numbers input

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {

                e.Handled = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                FollowModule followmodule;
                followmodule = BotContext.GetModule<FollowModule>("FOLLOW");

                if (followmodule == null)
                {
                    followmodule = new FollowModule((comboBox1.SelectedItem as OtherPlayer).Name);
                    BotContext.AddModule(followmodule);
                }
            }
            else
            {
                BotContext.RemoveModule("FOLLOW");
            }

        }

        private void button22_Click(object sender, EventArgs e)
        {
            cbHealHPItem.Items.Clear();
            cbHealHPItem.Items.AddRange(Player.GetPlayer.GetConsumableItemsFromInventory(cbHealHPItem.Items.OfType<InvItem>().ToList()).ToArray());

        }



        private void comboBox7_DropDown_1(object sender, EventArgs e)
        {
            comboBox7.Items.Clear();
            comboBox7.Items.AddRange(Player.GetPlayer.GetConsumableItemsFromInventory(new List<InvItem>()).ToArray());
        }

        private void button29_Click(object sender, EventArgs e)
        {
            listBox6.Items.Clear();
            listBox6.Items.AddRange(Player.GetPlayer.GetConsumableItemsFromInventory(new List<InvItem>()).ToArray());
        }

        private void button30_Click(object sender, EventArgs e)
        {
            if (listBox6.SelectedItem == null || item2RepairList.Items.OfType<InvItem>().Any(x=>x.ObjectPointer == (listBox6.SelectedItem as InvItem).ObjectPointer)) return;

            item2RepairList.Items.Add(listBox6.SelectedItem);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            if (item2RepairList.SelectedItem == null) return;
            item2RepairList.Items.Remove(item2RepairList.SelectedItem);
        }
    }
}







