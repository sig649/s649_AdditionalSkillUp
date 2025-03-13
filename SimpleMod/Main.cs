using System;
using System.IO;
using System.Diagnostics;
using BepInEx;
using HarmonyLib;

using UnityEngine;
using BepInEx.Configuration;

using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using s649ASU.Main;

namespace s649ASU
{//>begin namespaceMain
    namespace Main
    {//>>begin namespaceSub
        [BepInPlugin("s649_ASU", "Additional SkillUp", "1.0.0.0")]
        public class PatchMain : BaseUnityPlugin
        {//>>>begin class:PatchExe
        //////-----Config Entry---------------------------------------------------------------------------------- 
            private static ConfigEntry<bool> CE_AllowFunction01WL;
            private static ConfigEntry<int> CE_LogLevel;
            
        //config--------------------------------------------------------------------------------------------------------------
            public static bool cf_Allow_F01_WL =>  CE_AllowFunction01WL.Value;
            public static int cf_LogLevel =>  CE_LogLevel.Value;
            
            //method------------------------------------------------------------------------------------------------------------------------------------------------------------
            internal void LoadConfig()
            {
                CE_AllowFunction01WL = Config.Bind("#00-General","AllowF01WL", true, "Allow control of function 01-WL");
                CE_LogLevel = Config.Bind("#zz-Debug","LogLevel", 0, "for debug use");

            }
            internal static void Log(string text, int lv = 0)
            {
                if(cf_LogLevel >= lv)
                {
                    Debug.Log(text);
                }
            }
            private void Start()
            {//>>>>begin method:Start
                LoadConfig();
                var harmony = new Harmony("PatchMain");
                new Harmony("PatchMain").PatchAll();
            }//<<<<end method:Start
            //private string TorF(bool b){return (b)? "T": "F";}

            
        }//<<<end class:Main
    }//<<end namespaceSub
}//<end namespaceMain

//------------template--------------------------------------------------------------------------------------------
/*
//------------namespace class--------------------------------------------------------------------------------------------
namespace NAMAE-MAIN
{//>begin namespaceMain
    namespace NAMAE-SUB
    {//>>begin namespaceSub
        //--nakami----------------------
    }//<<end namespaceSub
}//>end namespaceMain

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
