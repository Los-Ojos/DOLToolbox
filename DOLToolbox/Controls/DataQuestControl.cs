﻿using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using DOL.Database;
using DOLToolbox.Forms;
using DOLToolbox.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOLToolbox.Controls;
using EODModelViewer;
using MySql.Data.MySqlClient;

namespace DOLToolbox.Controls
{
    public partial class DataQuestControl : UserControl
    {
      
        private DBDataQuest _quest;

        string _SourceName, _SourceText, _StepType, _StepText, _StepItemTemplates, _AdvanceText, _TargetName, _TargetText;
        string _CollectItemTemplate, _RewardMoney, _RewardXP, _RewardCLXP, _RewardRp, _RewardBp, _OptionalRewardItemTemplates;
        string _FinalRewardItemTemplates, _QuestDependency, _AllowedClasses;

        public Dictionary<int, string> opt_dictionary;
        public Dictionary<int, string> fin_dictionary;
        public Dictionary<int, string> advtext_dictionary;
        public Dictionary<int, string> colitem_dictionary;
        public Dictionary<int, string> money_dictionary;
        public Dictionary<int, string> xp_dictionary;
        public Dictionary<int, string> clxp_dictionary;
        public Dictionary<int, string> rp_dictionary;
        public Dictionary<int, string> bp_dictionary;
        public Dictionary<int, string> srctext_dictionary;
        public Dictionary<int, string> srcname_dictionary;
        public Dictionary<int, string> stepitem_dictionary;
        public Dictionary<int, string> steptext_dictionary;
        public Dictionary<int, string> trgtname_dictionary;
        public Dictionary<int, string> trgttext_dictionary;
        public Dictionary<int, string> steptype_dictionary;

        public DataQuestControl()
        {
            InitializeComponent();
         
            opt_dictionary = new Dictionary<int, string>();
            fin_dictionary = new Dictionary<int, string>();
            advtext_dictionary = new Dictionary<int, string>();
            colitem_dictionary = new Dictionary<int, string>();
            money_dictionary = new Dictionary<int, string>();
            xp_dictionary = new Dictionary<int, string>();
            clxp_dictionary = new Dictionary<int, string>();
            rp_dictionary = new Dictionary<int, string>();
            bp_dictionary = new Dictionary<int, string>();
            srctext_dictionary = new Dictionary<int, string>();
            srcname_dictionary = new Dictionary<int, string>();
            stepitem_dictionary = new Dictionary<int, string>();
            steptext_dictionary = new Dictionary<int, string>();
            trgtname_dictionary = new Dictionary<int, string>();
            trgttext_dictionary = new Dictionary<int, string>();
            steptype_dictionary = new Dictionary<int, string>();
        }

        private void questSave_Click(object sender, EventArgs e)
        {
            int stepNum = int.Parse(stepNumber.Text);
            int optNum = int.Parse(optNumber.Text);
            int finNum = int.Parse(finNumber.Text);
            if (!steptype_dictionary.ContainsKey(stepNum)) //Adds step data to the dictionary on last step if the forward/back button has not been pressed yet
            {
                advtext_dictionary.Remove(stepNum);
                advtext_dictionary.Add(stepNum, AdvanceText.Text);
                colitem_dictionary.Remove(stepNum);
                colitem_dictionary.Add(stepNum, CollectItem.Text);
                money_dictionary.Remove(stepNum);
                money_dictionary.Add(stepNum, RewardMoney.Text);
                xp_dictionary.Remove(stepNum);
                xp_dictionary.Add(stepNum, RewardXp.Text);
                clxp_dictionary.Remove(stepNum);
                clxp_dictionary.Add(stepNum, RewardCLXp.Text);
                rp_dictionary.Remove(stepNum);
                rp_dictionary.Add(stepNum, RewardRp.Text);
                bp_dictionary.Remove(stepNum);
                bp_dictionary.Add(stepNum, RewardBp.Text);
                srctext_dictionary.Remove(stepNum);
                srctext_dictionary.Add(stepNum, SourceText.Text);
                srcname_dictionary.Remove(stepNum);
                srcname_dictionary.Add(stepNum, SourceName.Text);
                stepitem_dictionary.Remove(stepNum);
                stepitem_dictionary.Add(stepNum, StepItem.Text);
                steptext_dictionary.Remove(stepNum);
                steptext_dictionary.Add(stepNum, StepText.Text);
                trgtname_dictionary.Remove(stepNum);
                trgtname_dictionary.Add(stepNum, TargetName.Text);
                trgttext_dictionary.Remove(stepNum);
                trgttext_dictionary.Add(stepNum, TargetText.Text);
                steptype_dictionary.Remove(stepNum);
                steptype_dictionary.Add(stepNum, StepType.Text);
            }
            if (!opt_dictionary.ContainsKey(optNum))
            {
                opt_dictionary.Remove(stepNum);
                opt_dictionary.Add(stepNum, OptionalReward.Text);
            }
            if (!fin_dictionary.ContainsKey(finNum))
            {
                fin_dictionary.Remove(stepNum);
                fin_dictionary.Add(stepNum, FinalReward.Text);
            }

            try
            {
                #region String conversions                
                _OptionalRewardItemTemplates = String.Join("|", Array.ConvertAll(opt_dictionary.Values.ToArray(), i => i.ToString()));
                _FinalRewardItemTemplates = String.Join("|", Array.ConvertAll(fin_dictionary.Values.ToArray(), i => i.ToString()));
                _AdvanceText = String.Join("|", Array.ConvertAll(advtext_dictionary.Values.ToArray(), i => i.ToString()));
                _CollectItemTemplate = String.Join("|", Array.ConvertAll(colitem_dictionary.Values.ToArray(), i => i.ToString()));
                _RewardMoney = String.Join("|", Array.ConvertAll(money_dictionary.Values.ToArray(), i => i.ToString()));
                _RewardXP = String.Join("|", Array.ConvertAll(xp_dictionary.Values.ToArray(), i => i.ToString()));
                _RewardCLXP = String.Join("|", Array.ConvertAll(clxp_dictionary.Values.ToArray(), i => i.ToString()));
                _RewardRp = String.Join("|", Array.ConvertAll(rp_dictionary.Values.ToArray(), i => i.ToString()));
                _RewardBp = String.Join("|", Array.ConvertAll(bp_dictionary.Values.ToArray(), i => i.ToString()));
                _SourceText = String.Join("|", Array.ConvertAll(srctext_dictionary.Values.ToArray(), i => i.ToString()));
                _SourceName = String.Join("|", Array.ConvertAll(srcname_dictionary.Values.ToArray(), i => i.ToString()));
                _StepItemTemplates = String.Join("|", Array.ConvertAll(stepitem_dictionary.Values.ToArray(), i => i.ToString()));
                _StepText = String.Join("|", Array.ConvertAll(steptext_dictionary.Values.ToArray(), i => i.ToString()));
                _TargetName = String.Join("|", Array.ConvertAll(trgtname_dictionary.Values.ToArray(), i => i.ToString()));
                _TargetText = String.Join("|", Array.ConvertAll(trgttext_dictionary.Values.ToArray(), i => i.ToString()));
                _StepType = String.Join("|", Array.ConvertAll(steptype_dictionary.Values.ToArray(), i => i.ToString()));
                //string acl = String.Join("|", allowedClasses.SelectedItems.Cast<object>().Select(i => i.ToString()));
                //eStepType string replace values:

                StringBuilder stype = new StringBuilder(_StepType);
                stype.Replace("Kill", "0");
                stype.Replace("killFinish", "1");
                stype.Replace("Deliver", "2");
                stype.Replace("deliverFinish", "3");
                stype.Replace("Interact", "4");
                stype.Replace("interactFinish", "5");
                stype.Replace("Whisper", "6");
                stype.Replace("whisperFinish", "7");
                stype.Replace("Search", "8");
                stype.Replace("searchFinish", "9");
                stype.Replace("Collect", "10");
                stype.Replace("collectFinish", "11");
                stype.Replace("RewardQuest", "200");
                _StepType = stype.ToString();

                StringBuilder allcl = new StringBuilder(_AllowedClasses);
                allcl.Replace("Armsman", "2");
                allcl.Replace("Cabalist", "13");
                allcl.Replace("Cleric", "6");
                allcl.Replace("Friar", "10");
                allcl.Replace("Heretic", "33");
                allcl.Replace("Infiltrator", "9");
                allcl.Replace("Mercenary", "11");
                allcl.Replace("Minstrel", "4");
                allcl.Replace("Necromancer", "12");
                allcl.Replace("Paladin", "1");
                allcl.Replace("Reaver", "19");
                allcl.Replace("Scout", "3");
                allcl.Replace("Sorcerer", "8");
                allcl.Replace("Theurgist", "5");
                allcl.Replace("Wizard", "7");
                allcl.Replace("MaulerAlb", "60");
                allcl.Replace("Berserker", "31");
                allcl.Replace("Bonedancer", "30");
                allcl.Replace("Healer", "36");
                allcl.Replace("Hunter", "35");
                allcl.Replace("Runemaster", "29");
                allcl.Replace("Savage", "32");
                allcl.Replace("Shadowblade", "23");
                allcl.Replace("Shaman", "28");
                allcl.Replace("Skald", "24");
                allcl.Replace("Spiritmaster", "27");
                allcl.Replace("Thane", "21");
                allcl.Replace("Valkyrie", "34");
                allcl.Replace("Warlock", "59");
                allcl.Replace("Warrior", "22");
                allcl.Replace("MaulerMid", "61");
                allcl.Replace("Animist", "55");
                allcl.Replace("Bainshee", "39");
                allcl.Replace("Bard", "48");
                allcl.Replace("Blademaster", "43");
                allcl.Replace("Champion", "45");
                allcl.Replace("Druid", "47");
                allcl.Replace("Eldritch", "40");
                allcl.Replace("Enchanter", "41");
                allcl.Replace("Hero", "44");
                allcl.Replace("Mentalist", "42");
                allcl.Replace("Nightshade", "49");
                allcl.Replace("Ranger", "50");
                allcl.Replace("Valewalker", "56");
                allcl.Replace("Vampiir", "58");
                allcl.Replace("Warden", "46");
                allcl.Replace("MaulerHib", "62");
                allcl.Replace("Acolyte", "16");
                allcl.Replace("AlbionRogue", "17");
                allcl.Replace("Disciple", "20");
                allcl.Replace("Elementalist", "15");
                allcl.Replace("Fighter", "14");
                allcl.Replace("Forester", "57");
                allcl.Replace("Guardian", "52");
                allcl.Replace("Mage", "18");
                allcl.Replace("Magician", "51");
                allcl.Replace("MidgardRogue", "38");
                allcl.Replace("Mystic", "36");
                allcl.Replace("Naturalist", "53");
                allcl.Replace("Seer", "37");
                allcl.Replace("Stalker", "54");
                allcl.Replace("Viking", "35");


                 _AllowedClasses = allcl.ToString();
                #endregion

            }
            catch (Exception g)
            {
                MessageBox.Show(g.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DBDataQuest q = new DBDataQuest();
           // q.ID = int.Parse(_ID.Text);
   
            q.Name = _Name.Text;

            q.StartType = (byte)((_StartType.SelectedIndex) -1);
                 if (q.StartType == 6)
                     { q.StartType = 200; } // mannik's lazy workaround.
                 if (q.StartType >= 201 || q.StartType < 0)
                     { q.StartType = 0;
                MessageBox.Show("Quest type entered invald. Replaced with quest type Standard (0), please check DB to correct.", "Invalid Quest Type", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } //Mannik's other lazy workaround.

            q.StartName = _StartName.Text;
            q.StartRegionID = ushort.Parse(_StartRegionID.Text);
            q.AcceptText = _AcceptText.Text;
            q.Description = _Description.Text;
            q.SourceText = _SourceText; //serialized
            q.SourceName = _SourceName;
            q.StepType = _StepType; //serialized
            q.StepText = _StepText; //serialized
            q.StepItemTemplates = _StepItemTemplates; //serialized
            q.AdvanceText = _AdvanceText; //serialized
            q.TargetName = _TargetName; //serialized
            q.TargetText = _TargetText; //serialized
            q.CollectItemTemplate = _CollectItemTemplate; //serialized
            q.MaxCount = byte.Parse(_MaxCount.Text);
            q.MinLevel = byte.Parse(_MinLevel.Text);
            q.MaxLevel = byte.Parse(_MaxLevel.Text);
            q.RewardMoney = _RewardMoney; //serialized
            q.RewardXP = _RewardXP; //serialized
            q.RewardCLXP = _RewardCLXP; //serialized
            q.RewardRP = _RewardRp; //serialized
            q.RewardBP = _RewardBp; //serialized
            q.OptionalRewardItemTemplates = _OptionalRewardItemTemplates;
            q.FinalRewardItemTemplates = _FinalRewardItemTemplates;
            q.FinishText = _FinishText.Text;
            q.QuestDependency = _QuestDependency; //might need to serialize....if quest has multiple dependencies
            q.AllowedClasses = _AllowedClasses; //serialized
            q.ClassType = _ClassType.Text;
            DatabaseManager.Database.AddObject(q);
        }

     

        private void SetupDropdowns()
        {
            ComboboxService.BindQuestType(_StartType);
            ComboboxService.BindQuestStep(StepType);

        }


        private void Clear()
        {
            _quest = null;
            BindingService.ClearData(this);
        }
     
        private void questDelete_Click(object sender, EventArgs e)
        {
            if (_quest == null)
            {
                return;
            }

            var confirmResult = MessageBox.Show(@"Are you sure to delete the selected object",
                @"Confirm Delete!!",
                MessageBoxButtons.YesNo);

            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            
            Clear();
        }

        private void SyncDataQuest()
        {
            BindingService.SyncData(_quest, this);
        }

        private void DataQuestControl_Load_1(object sender, EventArgs e)
        {
            SetupDropdowns();
        }

        #region Step/Reward forward/back buttons

        //Step data forward
        private void stepForward_Click(object sender, EventArgs e)
        {

            int stepNum = int.Parse(stepNumber.Text);

            if (StepType.Text == "" || TargetName.Text == "")
            {
                MessageBox.Show("You cannot proceed until you have selected a step type and target!");
                return;
            }

            #region Step forward data entry

            //AdvanceText.Text
            if (!advtext_dictionary.ContainsKey(stepNum))
            {
                advtext_dictionary.Add(stepNum, AdvanceText.Text);
            }
            else
            {
                advtext_dictionary.Remove(stepNum);
                advtext_dictionary.Add(stepNum, AdvanceText.Text);
            }
            AdvanceText.Text = "";

            //CollectItem.Text
            if (!colitem_dictionary.ContainsKey(stepNum))
            {
                colitem_dictionary.Add(stepNum, CollectItem.Text);
            }
            else
            {
                colitem_dictionary.Remove(stepNum);
                colitem_dictionary.Add(stepNum, CollectItem.Text);
            }
            CollectItem.Text = "";

            //RewardMoney.Text
            if (!money_dictionary.ContainsKey(stepNum))
            {
                money_dictionary.Add(stepNum, RewardMoney.Text);
            }
            else
            {
                money_dictionary.Remove(stepNum);
                money_dictionary.Add(stepNum, RewardMoney.Text);
            }
            RewardMoney.Text = "";

            //RewardXp.Text
            if (!xp_dictionary.ContainsKey(stepNum))
            {
                xp_dictionary.Add(stepNum, RewardXp.Text);
            }
            else
            {
                xp_dictionary.Remove(stepNum);
                xp_dictionary.Add(stepNum, RewardXp.Text);
            }
            RewardXp.Text = "";

            //RewardCLXp.Text
            if (!clxp_dictionary.ContainsKey(stepNum))
            {
                clxp_dictionary.Add(stepNum, RewardCLXp.Text);
            }
            else
            {
                clxp_dictionary.Remove(stepNum);
                clxp_dictionary.Add(stepNum, RewardCLXp.Text);
            }
            RewardCLXp.Text = "";

            //RewardRp.Text
            if (!rp_dictionary.ContainsKey(stepNum))
            {
                rp_dictionary.Add(stepNum, RewardRp.Text);
            }
            else
            {
                rp_dictionary.Remove(stepNum);
                rp_dictionary.Add(stepNum, RewardRp.Text);
            }
            RewardRp.Text = "";

            //RewardBp.Text
            if (!bp_dictionary.ContainsKey(stepNum))
            {
                bp_dictionary.Add(stepNum, RewardBp.Text);
            }
            else
            {
                bp_dictionary.Remove(stepNum);
                bp_dictionary.Add(stepNum, RewardBp.Text);
            }
            RewardBp.Text = "";

            //SourceText.Text
            if (!srctext_dictionary.ContainsKey(stepNum))
            {
                srctext_dictionary.Add(stepNum, SourceText.Text);
            }
            else
            {
                srctext_dictionary.Remove(stepNum);
                srctext_dictionary.Add(stepNum, SourceText.Text);
            }
            SourceText.Text = "";

            //SourceName.Text
            if (!srcname_dictionary.ContainsKey(stepNum))
            {
                srcname_dictionary.Add(stepNum, SourceName.Text);
            }
            else
            {
                srcname_dictionary.Remove(stepNum);
                srcname_dictionary.Add(stepNum, SourceName.Text);
            }
            SourceText.Text = "";

            //StepItem.Text
            if (!stepitem_dictionary.ContainsKey(stepNum))
            {
                stepitem_dictionary.Add(stepNum, StepItem.Text);
            }
            else
            {
                stepitem_dictionary.Remove(stepNum);
                stepitem_dictionary.Add(stepNum, StepItem.Text);
            }
            StepItem.Text = "";

            //StepText.Text
            if (!steptext_dictionary.ContainsKey(stepNum))
            {
                steptext_dictionary.Add(stepNum, StepText.Text);
            }
            else
            {
                steptext_dictionary.Remove(stepNum);
                steptext_dictionary.Add(stepNum, StepText.Text);
            }
            StepText.Text = "";

            //TargetName.Text
            if (!trgtname_dictionary.ContainsKey(stepNum))
            {
                trgtname_dictionary.Add(stepNum, TargetName.Text);
            }
            else
            {
                trgtname_dictionary.Remove(stepNum);
                trgtname_dictionary.Add(stepNum, TargetName.Text);
            }
            TargetName.Text = "";

            //TargetText.Text
            if (!trgttext_dictionary.ContainsKey(stepNum))
            {
                trgttext_dictionary.Add(stepNum, TargetText.Text);
            }
            else
            {
                trgttext_dictionary.Remove(stepNum);
                trgttext_dictionary.Add(stepNum, TargetText.Text);
            }
            TargetText.Text = "";

            //StepType.Text
            if (!steptype_dictionary.ContainsKey(stepNum))
            {
                steptype_dictionary.Add(stepNum, StepType.Text);
            }
            else
            {
                steptype_dictionary.Remove(stepNum);
                steptype_dictionary.Add(stepNum, StepType.Text);
            }
            StepType.Text = "";

            #endregion

            stepNumber.Text = (stepNum + 1).ToString(); //increment label

            #region Step forward check next step

            int stepNext = int.Parse(stepNumber.Text);
            string stepvalue;

            if (advtext_dictionary.ContainsKey(stepNext))
            {
                advtext_dictionary.TryGetValue(stepNext, out stepvalue);
                AdvanceText.Text = stepvalue;
            }
            else AdvanceText.Text = "";

            if (colitem_dictionary.ContainsKey(stepNext))
            {
                colitem_dictionary.TryGetValue(stepNext, out stepvalue);
                CollectItem.Text = stepvalue;
            }
            else CollectItem.Text = "";

            if (money_dictionary.ContainsKey(stepNext))
            {
                money_dictionary.TryGetValue(stepNext, out stepvalue);
                RewardMoney.Text = stepvalue;
            }
            else RewardMoney.Text = "0";

            if (xp_dictionary.ContainsKey(stepNext))
            {
                xp_dictionary.TryGetValue(stepNext, out stepvalue);
                RewardXp.Text = stepvalue;
            }
            else RewardXp.Text = "0";

            if (clxp_dictionary.ContainsKey(stepNext))
            {
                clxp_dictionary.TryGetValue(stepNext, out stepvalue);
                RewardCLXp.Text = stepvalue;
            }
            else RewardCLXp.Text = "0";

            if (rp_dictionary.ContainsKey(stepNext))
            {
                rp_dictionary.TryGetValue(stepNext, out stepvalue);
                RewardRp.Text = stepvalue;
            }
            else RewardRp.Text = "0";

            if (bp_dictionary.ContainsKey(stepNext))
            {
                bp_dictionary.TryGetValue(stepNext, out stepvalue);
                RewardBp.Text = stepvalue;
            }
            else RewardBp.Text = "0";

            if (srctext_dictionary.ContainsKey(stepNext))
            {
                srctext_dictionary.TryGetValue(stepNext, out stepvalue);
                SourceText.Text = stepvalue;
            }
            if (srcname_dictionary.ContainsKey(stepNext))
            {
                srcname_dictionary.TryGetValue(stepNext, out stepvalue);
                SourceName.Text = stepvalue;
            }
            else SourceText.Text = "";

            if (stepitem_dictionary.ContainsKey(stepNext))
            {
                stepitem_dictionary.TryGetValue(stepNext, out stepvalue);
                StepItem.Text = stepvalue;
            }
            else StepItem.Text = "";

            if (steptext_dictionary.ContainsKey(stepNext))
            {
                steptext_dictionary.TryGetValue(stepNext, out stepvalue);
                StepText.Text = stepvalue;
            }
            else StepText.Text = "";

            if (trgtname_dictionary.ContainsKey(stepNext))
            {
                trgtname_dictionary.TryGetValue(stepNext, out stepvalue);
                TargetName.Text = stepvalue;
            }
            else TargetName.Text = "";

            if (trgttext_dictionary.ContainsKey(stepNext))
            {
                trgttext_dictionary.TryGetValue(stepNext, out stepvalue);
                TargetText.Text = stepvalue;
            }
            else TargetText.Text = "";

            if (steptype_dictionary.ContainsKey(stepNext))
            {
                steptype_dictionary.TryGetValue(stepNext, out stepvalue);
                StepType.Text = stepvalue;
            }
            else StepType.Text = "";

            #endregion
        }

        //Step data back
        private void stepBack_Click(object sender, EventArgs e)
        {
            int stepNum = int.Parse(stepNumber.Text);

            if (stepNum == 1) //return if already at step 1, there ain't no step 0
            {
                return;
            }

            //remove step altogether if mandatory fields are not entered
            if (TargetName.Text == "" || StepType.Text == "")
            {
                advtext_dictionary.Remove(stepNum);
                colitem_dictionary.Remove(stepNum);
                money_dictionary.Remove(stepNum);
                xp_dictionary.Remove(stepNum);
                clxp_dictionary.Remove(stepNum);
                rp_dictionary.Remove(stepNum);
                bp_dictionary.Remove(stepNum);
                srctext_dictionary.Remove(stepNum);
                srcname_dictionary.Remove(stepNum);
                stepitem_dictionary.Remove(stepNum);
                steptext_dictionary.Remove(stepNum);
                trgtname_dictionary.Remove(stepNum);
                trgttext_dictionary.Remove(stepNum);
                steptype_dictionary.Remove(stepNum);
            }
            else
            //Needed to commit data to m_dictionary when the back button is clicked
            {
                advtext_dictionary.Remove(stepNum);
                advtext_dictionary.Add(stepNum, AdvanceText.Text);
                colitem_dictionary.Remove(stepNum);
                colitem_dictionary.Add(stepNum, CollectItem.Text);
                money_dictionary.Remove(stepNum);
                money_dictionary.Add(stepNum, RewardMoney.Text);
                xp_dictionary.Remove(stepNum);
                xp_dictionary.Add(stepNum, RewardXp.Text);
                clxp_dictionary.Remove(stepNum);
                clxp_dictionary.Add(stepNum, RewardCLXp.Text);
                rp_dictionary.Remove(stepNum);
                rp_dictionary.Add(stepNum, RewardRp.Text);
                bp_dictionary.Remove(stepNum);
                bp_dictionary.Add(stepNum, RewardBp.Text);
                srctext_dictionary.Remove(stepNum);
                srctext_dictionary.Add(stepNum, SourceText.Text);
                srcname_dictionary.Remove(stepNum);
                srcname_dictionary.Add(stepNum, SourceName.Text);
                stepitem_dictionary.Remove(stepNum);
                stepitem_dictionary.Add(stepNum, StepItem.Text);
                steptext_dictionary.Remove(stepNum);
                steptext_dictionary.Add(stepNum, StepText.Text);
                trgtname_dictionary.Remove(stepNum);
                trgtname_dictionary.Add(stepNum, TargetName.Text);
                trgttext_dictionary.Remove(stepNum);
                trgttext_dictionary.Add(stepNum, TargetText.Text);
                steptype_dictionary.Remove(stepNum);
                steptype_dictionary.Add(stepNum, StepType.Text);
            }


            stepNumber.Text = (stepNum - 1).ToString();
            string stepvalue;
            stepNum--;

            //Previous step data check
            if (advtext_dictionary.ContainsKey(stepNum))
            {
                advtext_dictionary.TryGetValue(stepNum, out stepvalue);
                AdvanceText.Text = stepvalue;
            }
            else AdvanceText.Text = "";

            if (colitem_dictionary.ContainsKey(stepNum))
            {
                colitem_dictionary.TryGetValue(stepNum, out stepvalue);
                CollectItem.Text = stepvalue;
            }
            else CollectItem.Text = "";

            if (money_dictionary.ContainsKey(stepNum))
            {
                money_dictionary.TryGetValue(stepNum, out stepvalue);
                RewardMoney.Text = stepvalue;
            }
            else RewardMoney.Text = "0";

            if (xp_dictionary.ContainsKey(stepNum))
            {
                xp_dictionary.TryGetValue(stepNum, out stepvalue);
                RewardXp.Text = stepvalue;
            }
            else RewardXp.Text = "0";

            if (clxp_dictionary.ContainsKey(stepNum))
            {
                clxp_dictionary.TryGetValue(stepNum, out stepvalue);
                RewardCLXp.Text = stepvalue;
            }
            else RewardCLXp.Text = "0";

            if (rp_dictionary.ContainsKey(stepNum))
            {
                rp_dictionary.TryGetValue(stepNum, out stepvalue);
                RewardRp.Text = stepvalue;
            }
            else RewardRp.Text = "0";

            if (bp_dictionary.ContainsKey(stepNum))
            {
                bp_dictionary.TryGetValue(stepNum, out stepvalue);
                RewardBp.Text = stepvalue;
            }
            else RewardBp.Text = "0";

            if (srctext_dictionary.ContainsKey(stepNum))
            {
                srctext_dictionary.TryGetValue(stepNum, out stepvalue);
                SourceText.Text = stepvalue;
            }
            else SourceText.Text = "";

            if (srcname_dictionary.ContainsKey(stepNum))
            {
                srcname_dictionary.TryGetValue(stepNum, out stepvalue);
                SourceName.Text = stepvalue;
            }
            else SourceName.Text = "";

            if (stepitem_dictionary.ContainsKey(stepNum))
            {
                stepitem_dictionary.TryGetValue(stepNum, out stepvalue);
                StepItem.Text = stepvalue;

            }
            else StepItem.Text = "";

            if (steptext_dictionary.ContainsKey(stepNum))
            {
                steptext_dictionary.TryGetValue(stepNum, out stepvalue);
                StepText.Text = stepvalue;
            }
            else StepText.Text = "";

            if (trgtname_dictionary.ContainsKey(stepNum))
            {
                trgtname_dictionary.TryGetValue(stepNum, out stepvalue);
                TargetName.Text = stepvalue;
            }
            else TargetName.Text = "";

            if (trgttext_dictionary.ContainsKey(stepNum))
            {
                trgttext_dictionary.TryGetValue(stepNum, out stepvalue);
                TargetText.Text = stepvalue;
            }
            else TargetText.Text = "";

            if (steptype_dictionary.ContainsKey(stepNum)) //wtf...enum can get bent
            {
                steptype_dictionary.TryGetValue(stepNum, out stepvalue);
                StepType.Text = stepvalue;
            }
            else StepType.Text = "";
        }

        //Final Reward forward dictionary
        private void finrewardForward_Click(object sender, EventArgs e)
        {
            if (FinalReward.Text == "")
            {
                MessageBox.Show("You can't add nothing as a reward!", "PEBKAC");
                return;
            }

            int finNum = int.Parse(finNumber.Text);

            if (!fin_dictionary.ContainsKey(finNum)) //If the reward data is not in the dictionary...check for step 1, then add
            {
                fin_dictionary.Add(finNum, FinalReward.Text);
            }
            else //If the reward data is in the dictionary...check if the values match and add if they don't
            {
                string finvalue;
                fin_dictionary.TryGetValue(finNum, out finvalue);

                if (finvalue != FinalReward.Text)
                {
                    fin_dictionary.Remove(finNum);
                    fin_dictionary.Add(finNum, FinalReward.Text);
                }
            }

            finNumber.Text = (finNum + 1).ToString();

            //Check if next step contains data
            int finNext = int.Parse(finNumber.Text);
            if (fin_dictionary.ContainsKey(finNext))
            {
                string finvalue;
                fin_dictionary.TryGetValue(finNext, out finvalue);
                FinalReward.Text = finvalue;
            }
            else FinalReward.Text = "";

        }

        //Final Reward back Dictionary
        private void finrewardBack_Click(object sender, EventArgs e)
        {
            int finNum = int.Parse(finNumber.Text);

            if (finNum == 1) //return if already at step 1, there ain't no step 0
            {
                return;
            }

            if (FinalReward.Text != "") //Add data on back click
            {
                string finvalue;
                fin_dictionary.TryGetValue(finNum, out finvalue);
                if (finvalue != FinalReward.Text) //check if data is different from dictionary data
                {
                    fin_dictionary.Remove(finNum);
                    fin_dictionary.Add(finNum, FinalReward.Text);
                }
            }
            else //There are no breaks in reward data, so a blank box cannot exist between populated blocks
            {
                if (fin_dictionary.ContainsKey(finNum + 1))
                {
                    MessageBox.Show("You must remove the items listed after the current item to remove this one!", "Error");
                    return;
                }
                else
                {
                    fin_dictionary.Remove(finNum);
                }
            }


            //Pull previous step data
            string finback;
            int fincheck = int.Parse(finNumber.Text);
            fincheck--;
            fin_dictionary.TryGetValue(fincheck, out finback);
            FinalReward.Text = finback;

            finNumber.Text = (finNum - 1).ToString(); //finally, decrement fin label
        }

        //Optional Reward forward dictionary
        private void optrewardForward_Click(object sender, EventArgs e)
        {
            if (OptionalReward.Text == "")
            {
                MessageBox.Show("You can't add nothing as a reward!", "PEBKAC");
                return;
            }

            int optNum = int.Parse(optNumber.Text);

            if (optNum == 8)
            {
                opt_dictionary.Remove(optNum);
                opt_dictionary.Add(optNum, OptionalReward.Text);
                MessageBox.Show("Last optional item added", "8 Optional Rewards Max");
                return;
            }

            if (!opt_dictionary.ContainsKey(optNum)) //If the reward data is not in the dictionary...check for step 1, then add
            {
                opt_dictionary.Add(optNum, OptionalReward.Text);
            }
            else //If the reward data is in the dictionary...check if the values match and add if they don't
            {
                string optvalue;
                opt_dictionary.TryGetValue(optNum, out optvalue);

                if (optvalue != OptionalReward.Text)
                {
                    opt_dictionary.Remove(optNum);
                    opt_dictionary.Add(optNum, OptionalReward.Text);
                }
            }

            optNumber.Text = (optNum + 1).ToString();

            //Check if next step contains data
            int optNext = int.Parse(optNumber.Text);
            if (opt_dictionary.ContainsKey(optNext))
            {
                string optvalue;
                opt_dictionary.TryGetValue(optNext, out optvalue);
                OptionalReward.Text = optvalue;
            }
            else OptionalReward.Text = "";

        }

        //Optional Reward back dictionary
        private void optrewardBack_Click(object sender, EventArgs e)
        {
            int optNum = int.Parse(optNumber.Text);

            if (optNum == 1) //return if already at step 1, there ain't no step 0
            {
                return;
            }

            if (OptionalReward.Text != "") //Add data on back click
            {
                string optvalue;
                opt_dictionary.TryGetValue(optNum, out optvalue);
                if (optvalue != OptionalReward.Text)
                {
                    opt_dictionary.Remove(optNum);
                    opt_dictionary.Add(optNum, OptionalReward.Text);
                }
            }
            else
            {
                if (opt_dictionary.ContainsKey(optNum + 1))
                {
                    MessageBox.Show("You must remove the items listed after the current item to remove this one!", "Error");
                    return;
                }
                else
                {
                    opt_dictionary.Remove(optNum);
                }
            }

            //Pull previous step data
            string optback;
            int optcheck = int.Parse(optNumber.Text);
            optcheck--;
            opt_dictionary.TryGetValue(optcheck, out optback);
            OptionalReward.Text = optback;

            optNumber.Text = (optNum - 1).ToString(); //finally, decrement opt label
        }

        #endregion

    }
}
