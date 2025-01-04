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


namespace AdditionalWL
{
    [BepInPlugin("s649_AWL", "Additional WeightLifting", "0.0.0.1")]
    public class AWLMain : BaseUnityPlugin
    {
        //public static Dictionary<Chara, int> tengatyaDict;

        // 設定項目の定義
        public static ConfigEntry<bool> flagPCGetableExp;
        public static ConfigEntry<bool> flagPetGetableExp;
        public static ConfigEntry<bool> flagInfluenceWeight;
        public static ConfigEntry<bool> isDebugMode;
        public static ConfigEntry<bool> isCheatMode;
        public static ConfigEntry<bool> PCForceAwardExp;
        public static ConfigEntry<float> PCExpMultiplier;
        public static ConfigEntry<float> PCFrequencyMultiplier;
        public static ConfigEntry<bool> PetForceAwardExp;
        public static ConfigEntry<float> PetExpMultiplier;
        public static ConfigEntry<float> PetFrequencyMultiplier;

        private void Start()
        {
            // Dictionaryの初期化 //savedataに保存される？
            //tengatyaDict = new Dictionary<Chara, int>();


            // 設定ファイルのパスを指定
            //string configFilePath = Path.Combine(Paths.ConfigPath, "s649_AWL0000.cfg");

            // 設定項目の初期化
            flagPCGetableExp = Config.Bind("#0General", "FLAG_PCGetableExp", true, "ENABLE PC Getable Exp");
            flagPetGetableExp = Config.Bind("#0General", "FLAG_PetGetableExp", false, "ENABLE Pet Getable Exp");
            flagInfluenceWeight = Config.Bind("#0General", "FLAG_InfluenceWeight", false, "ENABLE Influence Weight");
            isDebugMode = Config.Bind("#1Dev", "DEBUG_ON", false, "ENABLE DEBUG MODE : only Logging");
            isCheatMode = Config.Bind("#2Cheat", "CHEAT_ON", false, "ENABLE CHEAT MODE : if false, cheat options under this won't work");
            PCForceAwardExp = Config.Bind("#2Cheat", "PC_ForceAwardExp", false, "(PC)Force Award Exp : ***cheaty***");
            PCExpMultiplier = Config.Bind("#2Cheat", "PC_ExpMultiplier", 1.0f, "(PC)Exp Multiplier : 0.01 to 100");
            PCFrequencyMultiplier = Config.Bind("#2Cheat", "PC_FrequencyMultiplier", 1.0f, "(PC)Frequency Multiplier : 0.1 to 10");
            PetForceAwardExp = Config.Bind("#2Cheat", "Pet_ForceAwardExp", false, "(Pet)Force Award Exp : ***cheaty***");
            PetExpMultiplier = Config.Bind("#2Cheat", "Pet_ExpMultiplier", 1.0f, "(Pet)Exp Multiplier : 0.01 to 100");
            PetFrequencyMultiplier = Config.Bind("#2Cheat", "Pet_FrequencyMultiplier", 1.0f, "(Pet)Frequency Multiplier : 0.1 to 10");

            

            var harmony = new Harmony("AWLMain");
            new Harmony("AWLMain").PatchAll();
        }
    }
}
