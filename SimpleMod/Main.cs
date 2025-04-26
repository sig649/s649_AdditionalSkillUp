using System;
using System.IO;
using System.Diagnostics;
using BepInEx;
using HarmonyLib;

using UnityEngine;
using BepInEx.Configuration;

using Debug = UnityEngine.Debug;
using System.Collections.Generic;
//using s649ASU.Main;

namespace s649ASU
{//>begin namespaceMain
    namespace PatchMain
    {//>>begin namespaceSub
        [BepInPlugin("s649_ASU", "Additional SkillUp", "1.0.0.0")]
        public class Main : BaseUnityPlugin
        {// class[Main]
        //////-----Config Entry---------------------------------------------------------------------------------- 
            private static ConfigEntry<bool> CE_AllowFunction01WL;//WeightLiftingのASU
            private static ConfigEntry<bool> CE_AllowFunction02Stealth;//StealthのASU
            private static ConfigEntry<bool> CE_AllowFunction03DoorOpen;//LockPickingのASU

            private static ConfigEntry<bool> CE_Rule_ChildrenWeightForceValue;//所持重量(CW)が経験値の入手量に影響するかどうか。WMを入れている方向け
            private static ConfigEntry<int> CE_Freq_WL_Base;//WLのASU頻度の基本値
            private static ConfigEntry<int> CE_Exp_LP_Base;//LockPickingのASUのEXPの基本値
            
            private static ConfigEntry<int> CE_LogLevel;
            
        //config--------------------------------------------------------------------------------------------------------------
            public static bool cf_Allow_F01_WL =>  CE_AllowFunction01WL.Value;
            public static bool cf_Allow_F02_Stealth =>  CE_AllowFunction02Stealth.Value;
            public static bool cf_Allow_F03_LockPicking =>  CE_AllowFunction03DoorOpen.Value;

            public static bool cf_Rule_CWForceValue =>  CE_Rule_ChildrenWeightForceValue.Value;
            public static int cf_FreqWLBase 
            {
		        get {return Mathf.Clamp(CE_Freq_WL_Base.Value,1,100);}
            }
            public static int cf_ExpLPBase 
            {
		        get {return Mathf.Clamp(CE_Exp_LP_Base.Value,1,10);}
            }
            
            public static int cf_LogLevel =>  CE_LogLevel.Value;
            
            //Loading------------------------------------------------------------------------------------------------------------------------------------------------------------
            internal void LoadConfig()
            {
                CE_AllowFunction01WL = Config.Bind("#00-General","AllowF01WL", true, "Allow ASU control of function 01-WeightLifting");
                CE_AllowFunction02Stealth = Config.Bind("#00-General","AllowF02Stealth", true, "Allow ASU control of function 02-Stealth");
                CE_AllowFunction03DoorOpen = Config.Bind("#00-General","AllowF03LockPicking", true, "Allow ASU control of function 03-LockPicking");
  
                CE_Freq_WL_Base = Config.Bind("#01-WeightLifting","FreqBaseValue", 20, "Base value for frequency of obtaining EXP");
                CE_Rule_ChildrenWeightForceValue = Config.Bind("#01-WeightLifting","ChildrenWeightForceValue", true, "Whether ChildrenWeight affects the amount of experience available");
                CE_Exp_LP_Base = Config.Bind("#03-LockPicking","ExpBaseLockPicking", 1, "Base value for Lockpicking of obtaining EXP");

                CE_LogLevel = Config.Bind("#zz-Debug","LogLevel", 0, "for debug use");

            }
            
            private void Start()
            {//>>>>begin method:Start
                LoadConfig();
                var harmony = new Harmony("Main");
                new Harmony("Main").PatchAll();
            }//<<<<end method:Start
            //----methods----------------------------------------------------------------------------
            internal static void Log(string text, int lv = 0)
            {
                if(cf_LogLevel >= lv)
                {
                    Debug.Log(text);
                }
            }

            //private string TorF(bool b){return (b)? "T": "F";}
            internal static string SName(Chara c){
                return (c!= null)? c.GetName(NameStyle.Simple) : "-";
            }
            internal static string SName(Card c){
                return (c!= null)? c.GetName(NameStyle.Simple) : "-";
            }
        }//<<<end class:Main
    }//<<end namespaceSub
}//<end namespaceMain

//------------template--------------------------------------------------------------------------------------------
/*
//------------namespace class--------------------------------------------------------------------------------------------
namespace s649ASU
{//namespace main
    namespace sub@@@@@@@@@@@@
    {//namespace sub
        [HarmonyPatch]
        internal class @@@@@@@@@@@
        {//class[@@@@@@@@@@]
            //----nakami-------------------
        }//class[@@@@@@@@@@]
    }//namespace sub
}//namespace main

[HarmonyPatch]
internal class PatchExe
{//>begin class:PatchExe
    //----nakami-------------------
}//<end class:PatchExe

//----method--------------------------------------------------------------

[HarmonyPrefix]
[HarmonyPatch(typeof(ClassName), "MethodName")]
internal static bool WakariyasuiName()
{   //begin:method-@@@@@@@@@@@@
   
}   //end:method-@@@@@@@@@@@@

[HarmonyPostfix]
[HarmonyPatch(typeof(ClassName), "MethodName")]
internal static void WakariyasuiName()
{
    //nakami
}

//Harmony Patch Argument list
__result
__instance

//config entry
CE_aaa = Config.Bind("category","name", param, "description");

//---debug logging-------------------------------------------------------
if(PatchMain.configDebugLogging)
{
    string text = "";
}
//text += "[aaa:" + xxx.ToString() +"]"; 
//Debug.Log(text);


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
*/

//////trash box//////////////////////////////////////////////////////////////////////////////////////////////////
///
