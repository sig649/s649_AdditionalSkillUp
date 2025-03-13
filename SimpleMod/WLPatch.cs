using BepInEx;
using HarmonyLib;

using UnityEngine;
using BepInEx.Configuration;
//using System.IO;
//using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using s649ASU.Main;
//using Log = PatchMain.Log;


namespace S649ASU
{//>begin namespaceMain
    namespace WLPatch
    {//>>begin namespaceSub
        //--nakami----------------------
        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            //----nakami-------------------
            internal const int ID_WL = 207;
            private static bool Func_Allowed => PatchMain.cf_Allow_F01_WL;
            internal static void Log(string text, int lv = 0){
                PatchMain.Log(text,lv);
            }
            private static bool IsGlobalMap(){
                return (EClass.pc.currentZone.id == "ntyris") ? true : false;
            }
            private static string SName(Chara c){
                return c.GetName(NameStyle.Simple);
            }
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Chara), "TickConditions")]
            internal static void TickConditions_PostPatch(Chara __instance)
            {//<<<<begin method
                //nakami
                Chara c = __instance;
                if(IsGlobalMap()){return;}
                if(!Func_Allowed){return;}
                int bd1,bd2;
                string text = "[ASU]WL";
                if(c.IsPC){
                    bd1 = c.burden.GetPhase();
                    bd2 = c.GetBurden(c.held);

                    if(bd1 >= StatsBurden.Burden){
                        if(c.HasElement(ID_WL)){
                            Element eWL = c.elements.GetElement(ID_WL);
                            c.ModExp(ID_WL, bd1 * bd1);
                            text += "/c:PC/";
                            text += "/bd:" + bd1.ToString();
                            text += "/B:" + eWL.vBase.ToString();
                            text += "/Ex:" + eWL.vExp.ToString();
                            
                            //text += "/bd2:" + bd2.ToString();
                            Log(text,2);
                        }
                    }
                } else if(!c.IsPC && c.IsPCParty){
                    //bd2 = c.GetBurden();
                    bd2 = c.ChildrenWeight * 10 / c.WeightLimit;
                    if(EClass.rnd((bd2 >= 10)? 0 : 10-bd2) == 0){
                        if(c.HasElement(ID_WL))
                        {
                            Element eWL = c.elements.GetElement(ID_WL);
                            c.ModExp(ID_WL, bd2 * bd2);
                            ///string text = "[ASU]WL";
                            text += "/c:" + SName(c) + "/";
                            text += "/bd:" + bd2.ToString();
                            text += "/B:" + eWL.vBase.ToString();
                            text += "/Ex:" + eWL.vExp.ToString();

                            Log(text, 1);
                        }
                    }
                }
                
                return;
            }//<<<<end method
        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//>end namespaceMain