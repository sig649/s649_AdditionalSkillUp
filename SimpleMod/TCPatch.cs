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


namespace S649ASU
{//>begin namespaceMain
    namespace TCPatch
    {//>>begin namespaceSub
        
        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            //----entry-------------------
            internal const int ID_WL = 207;
            internal const int ID_Stealth = 152;
            private static bool enableASU_WL => Main.cf_Allow_F01_WL;
            private static bool enableASU_Stealth => Main.cf_Allow_F02_Stealth;

            private static bool rule_CWForceValue => Main.cf_Rule_CWForceValue;
            private static int freq_WL_Base => Main.cf_FreqWLBase;

            //method--------------------------------------------------------------------
            internal static void Log(string text, int lv = 0){
                PatchMain.Log(text,lv);
            }
            private static bool IsGlobalMap(){
                return (EClass.pc.currentZone.id == "ntyris") ? true : false;
            }
            private static string SName(Chara c){
                return c.GetName(NameStyle.Simple);
            }
            private int Gatya(int seed)
            {
                int res = 0;
                int border;
                while(seed > 0)
                {
                    border = 100 * UnityEngine.Random.Range(0f,1f);
                    if(seed > border){res++;}
                    seed -= 100;
                }
                return res;
            }
            private static bool CanGetStealthExpArea()
            {
                string cz = EClass.pc.currentZone.id;
                if(EClass.pc.currentZone.IsTown){return true;}
                if(cz == "startSite" || cz == "instance_music"){return true;}
            
                return false;
            }
            //--------------------------------------------------------------------------
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Chara), "TickConditions")]
            internal static void TickConditions_PostPatch(Chara __instance)
            {//<<<<begin method
                //nakami
                Chara c = __instance;
                //if(IsGlobalMap()){return;}
                //if(!enableASU_WL){return;}
                int ph,exp,ticket,bd;
                string text = "[ASU]TC";
                if(c.IsPC && enableASU_WL){
                    ph = c.burden.GetPhase();
                    //bd = c.GetBurden(c.held);
                    if(ph >= StatsBurden.Burden && c.HasElement(ID_WL)){
                        Element eWL = c.elements.GetElement(ID_WL);
                        ticket = Gatya(ph * freq_WL_Base);
                        bd = (rule_CWForceValue)?  (c.ChildrenWeight * 10 / c.WeightLimit) : ph;
                        exp = 1 + bd * bd;
                        
                        if(ticket > 0)
                        {
                            c.ModExp(ID_WL, exp * ticket);
                            text += "/c:PC/";
                            //text += "/bd:" + bd2.ToString();
                            text += "/B:" + eWL.vBase.ToString();
                            text += "/Ex:" + eWL.vExp.ToString();
                            
                            //text += "/bd2:" + bd2.ToString();
                            Log(text,2);
                        }
                    }
                } else if(!c.IsPC && c.IsPCParty && enableASU_WL){
                    //bd2 = c.GetBurden();
                    ph = c.ChildrenWeight * 10 / c.WeightLimit;
                    //bd = c.ChildrenWeight * 10 / c.WeightLimit;
                    //if(EClass.rnd((bd2 >= 10)? 0 : (10-bd2)*(10-bd2)) == 0){

                        if(ph >= 5 && c.HasElement(ID_WL))
                        {
                            Element eWL = c.elements.GetElement(ID_WL);
                            ticket = Gatya(ph * freq_WL_Base);
                            bd = ph;
                            c.ModExp(ID_WL, bd * 2 * ticket);

                            ///string text = "[ASU]WL";
                            text += "/c:" + SName(c) + "/";
                            text += "/bd:" + bd2.ToString();
                            text += "/B:" + eWL.vBase.ToString();
                            text += "/Ex:" + eWL.vExp.ToString();

                            Log(text, 2);
                        }
                    //}
                }
                //-------------------------------------------------------------------------------------
                int countStealthAnother = 0;
                int resultSt = 0;
                int youAreSeen = 0;
                if(!c.IsPC){return;}
                if(!enableASU_Stealth){return;}
                if(UnityEngine.Random.Range(0,10) != 0){return;}
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
            string text = ("[Stealth]Charas : " + EClass._map.charas.Count.ToString());
                       text += (" [Stsn:" + c.stealthSeen.ToString() + "]");
                       text += (" [YAsn:" + youAreSeen.ToString() + "]");
                        text += (" [CSA:" + countStealthAnother.ToString() + "]");
                        text += (" [RS:" + resultSt.ToString() + "]");
                        text += ("/HasSt:" + flagHasStealth.ToString());
                        text += ("/bSt:" + cSteBase.ToString() + "&" + cSteExp.ToString());
            if(resultSt > 0){Log(text,1);}

                
            }//<<<<end method
        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//>end namespaceMain