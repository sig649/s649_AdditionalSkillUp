using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BepInEx;
using HarmonyLib;

using UnityEngine;
using BepInEx.Configuration;
using System.IO;
using System.Diagnostics;


namespace AdditionalSU
{
    [BepInPlugin("s649_ASU", "Additional SkillUp", "0.1.0.0")]
    public class ASUMain : BaseUnityPlugin
    {
        

        // config entry
        private static ConfigEntry<bool> CE_WL_PC_EnableSkillUp;
        private static ConfigEntry<bool> CE_WL_Pet_EnableSkillUp;
        //private static ConfigEntry<bool> CE_InfluenceWeight;
        private static ConfigEntry<bool> CE_IsDebugMode;
        //private static ConfigEntry<bool> CE_IsCheatMode;
        //private static ConfigEntry<bool> CE_PCForceAwardExp;
        private static ConfigEntry<float> CE_WL_PC_ExpMultiplier;
        //private static ConfigEntry<float> CE_PCFrequencyMultiplier;
        //private static ConfigEntry<bool> CE_PetForceAwardExp;
        private static ConfigEntry<float> CE_WL_Pet_ExpMultiplier;
        //private static ConfigEntry<float> CE_PetFrequencyMultiplier;
        private static ConfigEntry<bool> CE_Stealth_PC_EnableSkillUp;
        private static ConfigEntry<bool> CE_LockPicking_PC_EnableSkillUp;


        // getter
        public static bool Flag_WL_PC_EnableSkillUp => CE_WL_PC_EnableSkillUp.Value;
        public static bool Flag_WL_Pet_EnableSkillUp => CE_WL_Pet_EnableSkillUp.Value;
        //public static bool DoInfluenceWeight => CE_InfluenceWeight.Value;
        public static bool IsDebugMode => CE_IsDebugMode.Value;
        //public static bool IsCheatMode => CE_IsCheatMode.Value;
        //public static bool DoPCForceAwardExp => CE_PCForceAwardExp.Value;
        public static float Float_WL_PC_ExpMultiplier => CE_WL_PC_ExpMultiplier.Value;
        //public static float ParamPCFrequencyMultiplier => CE_PCFrequencyMultiplier.Value;
        //public static bool DoPetForceAwardExp => CE_PetForceAwardExp.Value;
        public static float Float_WL_Pet_ExpMultiplier => CE_WL_Pet_ExpMultiplier.Value;
        //public static float ParamPetFrequencyMultiplier => CE_PetFrequencyMultiplier.Value;
        public static bool Flag_Stealth_PC_EnableSkillUp => CE_Stealth_PC_EnableSkillUp.Value;
        public static bool Flag_LockPicking_PC_EnableSkillUp => CE_LockPicking_PC_EnableSkillUp.Value;



        //internal const string ModOptionsGuid = "evilmask.elinplugins.modoptions";
        public void lg(string t){
            UnityEngine.Debug.Log(t);
        }
        private void Start()
        {
            // Mod Options loading
            /*
            var mod_options_loaded
            foreach (var obj in ModManager.ListPluginObject)
            {
                var plugin = obj as BaseUnityPlugin;
                if (plugin.Info.Metadata.GUID == ModOptionsGuid)
                {
                    // Mod Options is loaded, you can do
                    // registeration now.
                    break;
                }
            }
            */
            // 設定項目の初期化
            CE_WL_PC_EnableSkillUp = Config.Bind("#0_WeightLifting", "WL_PC_Enable_SkillUp", true, "WL :  PC Getable Exp");
            CE_WL_Pet_EnableSkillUp = Config.Bind("#0_WeightLifting", "WL_PetGetableExp", false, "WL :  Pet Getable Exp");
            //CE_InfluenceWeight = Config.Bind("#0General", "FLAG_InfluenceWeight", false, "ENABLE Influence Weight");
            CE_IsDebugMode = Config.Bind("#z_Dev", "DEBUG_ON", false, "ENABLE DEBUG MODE : only Logging");
            //CE_IsCheatMode = Config.Bind("#2Cheat", "CHEAT_ON", false, "ENABLE CHEAT MODE : if false, cheat options under this won't work");
            //CE_PCForceAwardExp = Config.Bind("#2Cheat", "PC_ForceAwardExp", false, "(PC)Force Award Exp : ***cheaty***");
            CE_WL_PC_ExpMultiplier = Config.Bind("#0_WeightLifting", "PC_ExpMultiplier", 1.0f, "(PC)Exp Multiplier : 0.01 to 100");
            //CE_PCFrequencyMultiplier = Config.Bind("#2Cheat", "PC_FrequencyMultiplier", 1.0f, "(PC)Frequency Multiplier : 0.1 to 10");
            //CE_PetForceAwardExp = Config.Bind("#2Cheat", "Pet_ForceAwardExp", false, "(Pet)Force Award Exp : ***cheaty***");
            CE_WL_Pet_ExpMultiplier = Config.Bind("#0_WeightLifting", "Pet_ExpMultiplier", 1.0f, "(Pet)Exp Multiplier : 0.01 to 100");
            //CE_PetFrequencyMultiplier = Config.Bind("#2Cheat", "Pet_FrequencyMultiplier", 1.0f, "(Pet)Frequency Multiplier : 0.1 to 10");
            CE_Stealth_PC_EnableSkillUp = Config.Bind("#1_Stealth", "Stealth_PC_Enable_SkillUp", true, "Stealth :  PC Getable Exp");
            CE_LockPicking_PC_EnableSkillUp = Config.Bind("#2_LockPicking", "LockPicking_PC_Enable_SkillUp", true, "LP :  PC Getable Exp");

            //debugtext for config
            string debt = ("PGE:" + Flag_WL_PC_EnableSkillUp.ToString());
            debt += ("][PeGE:" + Flag_WL_Pet_EnableSkillUp.ToString());
            //debt += ("][DIW:" + DoInfluenceWeight.ToString());
            debt += ("][IDM:" + IsDebugMode.ToString());
            //debt += ("][ICM:" + IsCheatMode.ToString());
            //debt += ("][DPFAE:" + DoPCForceAwardExp.ToString());
            debt += ("][PPEM:" + Float_WL_PC_ExpMultiplier.ToString());
            //debt += ("][PPFM:" + ParamPCFrequencyMultiplier.ToString());
            //debt += ("][DPeFAE:" + DoPetForceAwardExp.ToString());
            debt += ("][PPeEM:" + Float_WL_Pet_ExpMultiplier.ToString());
            //debt += ("][PPeFM:" + ParamPetFrequencyMultiplier.ToString());

            lg("[ASU:Loaded]" + debt);

            var harmony = new Harmony("ASUMain");
            new Harmony("ASUMain").PatchAll();
        }
    }
}
