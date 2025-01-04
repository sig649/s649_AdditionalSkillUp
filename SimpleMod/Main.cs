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
    [BepInPlugin("s649_AWL", "Additional SkillUp", "0.1.0.0")]
    public class ASUMain : BaseUnityPlugin
    {
        

        // config entry
        private static ConfigEntry<bool> CE_IsPlayerGetableExp;
        private static ConfigEntry<bool> CE_IsPetGetableExp;
        private static ConfigEntry<bool> CE_InfluenceWeight;
        private static ConfigEntry<bool> CE_IsDebugMode;
        private static ConfigEntry<bool> CE_IsCheatMode;
        private static ConfigEntry<bool> CE_PCForceAwardExp;
        private static ConfigEntry<float> CE_PCExpMultiplier;
        private static ConfigEntry<float> CE_PCFrequencyMultiplier;
        private static ConfigEntry<bool> CE_PetForceAwardExp;
        private static ConfigEntry<float> CE_PetExpMultiplier;
        private static ConfigEntry<float> CE_PetFrequencyMultiplier;

        // getter
        public static bool IsPlayerGetableExp { get { return CE_IsPlayerGetableExp.Value; } };
        public static bool IsPetGetableExp { get { return CE_IsPetGetableExp.Value; } };
        public static bool DoInfluenceWeight { get { return CE_InfluenceWeight.Value; } };
        public static bool IsDebugMode { get { return CE_IsDebugMode.Value; } };
        public static bool IsCheatMode { get { return CE_IsCheatMode.Value; } };
        public static bool DoPCForceAwardExp { get { return CE_PCForceAwardExp.Value; } };
        public static float ParamPCExpMultiplier { get { return CE_PCExpMultiplier.Value; } };
        public static float ParamPCFrequencyMultiplier { get { return CE_PCFrequencyMultiplier.Value; } };
        public static bool DoPetForceAwardExp { get { return CE_PetForceAwardExp.Value; } };
        public static float ParamPetExpMultiplier { get { return CE_PetExpMultiplier.Value; } };
        public static float ParamPetFrequencyMultiplier { get { return CE_PetFrequencyMultiplier.Value; } };


        private void Start()
        {

            // 設定項目の初期化
            CE_IsPlayerGetableExp = Config.Bind("#0General", "FLAG_PCGetableExp", true, "ENABLE PC Getable Exp");
            CE_IsPetGetableExp = Config.Bind("#0General", "FLAG_PetGetableExp", false, "ENABLE Pet Getable Exp");
            CE_InfluenceWeight = Config.Bind("#0General", "FLAG_InfluenceWeight", false, "ENABLE Influence Weight");
            CE_IsDebugMode = Config.Bind("#1Dev", "DEBUG_ON", false, "ENABLE DEBUG MODE : only Logging");
            CE_IsCheatMode = Config.Bind("#2Cheat", "CHEAT_ON", false, "ENABLE CHEAT MODE : if false, cheat options under this won't work");
            CE_PCForceAwardExp = Config.Bind("#2Cheat", "PC_ForceAwardExp", false, "(PC)Force Award Exp : ***cheaty***");
            CE_PCExpMultiplier = Config.Bind("#2Cheat", "PC_ExpMultiplier", 1.0f, "(PC)Exp Multiplier : 0.01 to 100");
            CE_PCFrequencyMultiplier = Config.Bind("#2Cheat", "PC_FrequencyMultiplier", 1.0f, "(PC)Frequency Multiplier : 0.1 to 10");
            CE_PetForceAwardExp = Config.Bind("#2Cheat", "Pet_ForceAwardExp", false, "(Pet)Force Award Exp : ***cheaty***");
            CE_PetExpMultiplier = Config.Bind("#2Cheat", "Pet_ExpMultiplier", 1.0f, "(Pet)Exp Multiplier : 0.01 to 100");
            CE_PetFrequencyMultiplier = Config.Bind("#2Cheat", "Pet_FrequencyMultiplier", 1.0f, "(Pet)Frequency Multiplier : 0.1 to 10");

            

            var harmony = new Harmony("AWLMain");
            new Harmony("AWLMain").PatchAll();
        }
    }
}
