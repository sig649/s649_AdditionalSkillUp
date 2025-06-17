using BepInEx;
using HarmonyLib;

using UnityEngine;
using BepInEx.Configuration;
//using System.IO;
//using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using s649ASU.Core;
//using Log = PatchMain.Log;

namespace s649ASU
{//namespace main
    namespace TraitDoorPatch
    {//namespace sub
        [HarmonyPatch(typeof(TraitDoor))]
        internal class PatchTrait 
        {
            private static TraitDoor instance;
            private static string StrHarmonyPatchClass = "TraitDoor";
            private static string StrFookMethod = "";
            
            private static bool Enable_LP_ASU => Main.cf_Allow_F03_LockPicking;
            private static int FreqLPValue => Main.cf_FreqLPValue;

            [HarmonyPrefix]
            [HarmonyPatch(nameof(TraitDoor.TryOpen))]
            static void FookPreExe(TraitDoor __instance, Chara c, ref bool __state){
                instance = __instance;
                __state = __instance.IsOpen();
            }
        
            [HarmonyPostfix]
            [HarmonyPatch(nameof(TraitDoor.TryOpen))]
            static void FookPostExe(TraitDoor __instance, Chara c, bool __state){
                StrFookMethod = "TryOpen:post";

                if(__instance != instance) { return; }//nennnotame
                if (!__state && __instance.IsOpen())
                {
                    bool chance = Chance(FreqLPValue);
                    string dstr = "/" + StrHarmonyPatchClass + "/" + StrFookMethod;
                    dstr += "/C:" + c.NameSimple;
                    dstr += "/Allow:" + StrTF(Enable_LP_ASU);
                    dstr += "/LP:" + StrTF(c.HasElement(Main.ID_DoorOpen));
                    dstr += "/Frq:" + FreqLPValue.ToString();
                    dstr += "/TF:" + chance.ToString();

                    if (c.IsPC && Enable_LP_ASU && c.HasElement(Main.ID_DoorOpen) && chance)
                    { 
                        //Main.Log("[ASU]TraitDoor.Close->Open!" + c.ToString(),1);
                        c.ModExp(Main.ID_DoorOpen, Main.cf_ExpLPBase);
                    }
                    if (c.IsPC) { Main.Log(dstr, 1); }
                    
                }
            
            }
            //private static int Lower(int a,int b)
            //{
            //    return (a < b)? a: b;
            //}
            public static bool Chance(int num)
            {
                if (num <= 0) return false;
                if (num >= 100) return true;

                //Random rand = Random();
                return EClass.rnd(100) < num;
            }
            private static string StrTF(bool b)
            {
                return (b)? "T" : "F";
            }
        }
    }//namespace sub
}//namespace main