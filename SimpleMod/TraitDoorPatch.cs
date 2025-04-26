using BepInEx;
using HarmonyLib;

using UnityEngine;
using BepInEx.Configuration;
//using System.IO;
//using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using s649ASU.PatchMain;
//using Log = PatchMain.Log;

namespace s649ASU
{//namespace main
    namespace TraitDoorPatch
    {//namespace sub
        [HarmonyPatch(typeof(TraitDoor))]
        internal class PatchTrait 
        {
            [HarmonyPrefix]
            [HarmonyPatch(nameof(TraitDoor.TryOpen))]
            static void FookPreExe(TraitDoor __instance, Chara c, ref bool __state){
                __state = __instance.IsOpen() ? true : false;
            }
        
            [HarmonyPostfix]
            [HarmonyPatch(nameof(TraitDoor.TryOpen))]
            static void FookPostExe(TraitDoor __instance, Chara c, bool __state){
                if(!Main.cf_Allow_F03_LockPicking){return;}
                if(!__state && __instance.IsOpen())
                {
                    if(c.IsPC && Range(0,4) == 0)
                    { 
                        Main.Log("[ASU]TraitDoor.Close->Open!" + c.ToString(),1);
                        c.ModExp(ID_DoorOpen, Main.cf_ExpLPBase);
                    }
                }
            
            }
            private static int Range(int a,int b)
            {
                return UnityEngine.Random.Range(a,b);
            }
        }
    }//namespace sub
}//namespace main