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
    namespace StealthPatch
    {//namespace sub
        [HarmonyPatch]
        internal class StealthPExe
        {//class[@@@@@@@@@@]
            public static bool CanGetStealthExpArea()
            {
                string cz = EClass.pc.currentZone.id;
                if(EClass.pc.currentZone.IsTown){return true;}
                if(cz == "startSite" || cz == "instance_music"){return true;}
            
                return false;
            }
            //----nakami-------------------
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Chara), "TickConditions")]
            internal static void TickConditions_PostPatch(Chara __instance)
            {
                int countStealthAnother = 0;
                int resultSt = 0;
                int youAreSeen = 0;
                if(!c.IsPC){return;}
                if(!flagStealthPCGetableExp){return;}
                if(rng(0,12) != 0){return;}
                if(c.enemy != null || !CanGetStealthExpArea()){return;}  //c は戦闘中ではない//エリア指定
                int numNearCharas = 0;
                for (int i = 0; i < EClass._map.charas.Count; i++){
                    Chara tg = EClass._map.charas[i];
                
                    //logging("tg:[" + tg.GetName(NameStyle.Simple) + "/" + tg.IsPCParty.ToString() + "/" + tg.CanSee(c) + "]");
                    if (tg == c || tg.IsHostile(c)|| tg.IsPCParty || !tg.CanSee(c))
                    {
                        continue;
                    }
                    int num2 = tg.Dist(c);
                    int num3 = tg.GetSightRadius();
                    if (num2 > num3)
                    {
                        continue;
                    }
                    numNearCharas++;
                    if(!EClass.pc.CanSeeLos(tg)) {
                        if(tg.hostility == Hostility.Friend || tg.hostility == Hostility.Ally){
                            countStealthAnother++;
                        }
                } else {
                    youAreSeen++;
                }
                
            }
            resultSt = countStealthAnother - c.stealthSeen - youAreSeen * youAreSeen;
            if(resultSt > 0){
                c.ModExp(ID_Stealth, resultSt * numNearCharas);
            }
            string text = ("[ASU]Charas : " + EClass._map.charas.Count.ToString());
                       text += (" [Stsn:" + c.stealthSeen.ToString() + "]");
                       text += (" [YAsn:" + youAreSeen.ToString() + "]");
                        text += (" [CSA:" + countStealthAnother.ToString() + "]");
                        text += (" [RS:" + resultSt.ToString() + "]");
                        text += ("/HasSt:" + flagHasStealth.ToString());
                        text += ("/bSt:" + cSteBase.ToString() + "&" + cSteExp.ToString());
            if(configDebug && resultSt > 0){logging(text);}
       

        
        
            }
        }//class[@@@@@@@@@@]
    }//namespace sub
}//namespace main