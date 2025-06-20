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


namespace S649ASU
{//>begin namespaceMain
    namespace TCPatch
    {//>>begin namespaceSub
        
        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            //----entry-------------------
            
            private static bool enableASU_WL => Main.cf_Allow_F01_WL;
            private static bool enableASU_Stealth => Main.cf_Allow_F02_Stealth;

            private static bool enable_CWForceValue => Main.cf_Enable_CWForceValue;
            private static int freq_WL_Base => Main.cf_FreqWLBase;

            //method--------------------------------------------------------------------
            internal static void Log(string text, int lv = 0){
                Main.Log(text,lv);
            }
            //private static bool IsGlobalMap(){
            //    return (EClass.pc.currentZone.id == "ntyris") ? true : false;
            //}
            //private string SName(Chara c){
            //    return Main.SName(c);
            //}
            private static int Gatya(int seed)
            {
                int res = 0;
                int border;
                while(seed > 0)
                {
                    border = (int)(100 * UnityEngine.Random.Range(0f,1f));
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
                string text = "[s649-ASU:2]TC";
                if(c.IsPC && enableASU_WL && c.HasElement(Main.ID_WL)){
                    ph = c.burden.GetPhase();//重荷レベルに応じて0-9の値
                    //bd = c.GetBurden(c.held);
                    if(ph >= StatsBurden.Burden){
                        Element eWL = c.elements.GetElement(Main.ID_WL);
                        ticket = Gatya(ph * freq_WL_Base);
                        bd = (enable_CWForceValue)?  (c.ChildrenWeight / c.WeightLimit) : ph;
                        exp = (enable_CWForceValue)? (1 + bd) : (1+ ph*ph);
                        exp *= ticket;
                        
                        if(ticket > 0)
                        {
                            c.ModExp(Main.ID_WL, exp);
                            text += "/c:PC/";

                            text += "/CW:" + c.ChildrenWeight.ToString();
                            text += "/WL:" + c.WeightLimit.ToString();
                            
                            text += "/ph:" + ph.ToString();
                            text += "/exp:" + exp.ToString();
                            text += "/ticket:" + ticket.ToString();
                            
                            text += "/bd:" + bd.ToString();
                            text += "/B:" + eWL.vBase.ToString();
                            text += "/Ex:" + eWL.vExp.ToString();
                            
                            //text += "/bd2:" + bd2.ToString();
                            Log(text,2);
                        }
                    }
                } else if(!c.IsPC && c.IsPCParty && enableASU_WL && c.HasElement(Main.ID_WL)){
                    //bd2 = c.GetBurden();
                    ph = c.ChildrenWeight * 10 / c.WeightLimit;
                    //bd = c.ChildrenWeight * 10 / c.WeightLimit;
                    //if(EClass.rnd((bd2 >= 10)? 0 : (10-bd2)*(10-bd2)) == 0){

                        if(ph >= 5 && c.HasElement(Main.ID_WL))
                        {
                            Element eWL = c.elements.GetElement(Main.ID_WL);
                            ticket = Gatya(ph * freq_WL_Base);
                            bd = ph;
                            exp = bd * 2 * ticket;
                            c.ModExp(Main.ID_WL, exp);

                            ///string text = "[ASU]WL";
                            text += "/c:" + Main.SName(c) + "/";
                            
                            text += "/CW:" + c.ChildrenWeight.ToString();
                            text += "/WL:" + c.WeightLimit.ToString();
                            
                            text += "/ph:" + ph.ToString();
                            text += "/exp:" + exp.ToString();
                            text += "/ticket:" + ticket.ToString();
                            
                            text += "/bd:" + bd.ToString();
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
                if(!c.IsPC || !enableASU_Stealth || !c.HasElement(Main.ID_Stealth)){return;}
                //if(){return;}
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
            resultSt = (youAreSeen == 0) ? countStealthAnother - c.stealthSeen : 0;
            if(resultSt > 0){
                c.ModExp(Main.ID_Stealth, resultSt);
            }
            string stext = ("[Stealth]Charas : " + EClass._map.charas.Count.ToString());
                       stext += (" [Stsn:" + c.stealthSeen.ToString() + "]");
                       stext += (" [YAsn:" + youAreSeen.ToString() + "]");
                        stext += (" [CSA:" + countStealthAnother.ToString() + "]");
                        stext += (" [RS:" + resultSt.ToString() + "]");
                        //stext += ("/HasSt:" + flagHasStealth.ToString());
                        //stext += ("/bSt:" + cSteBase.ToString() + "&" + cSteExp.ToString());
            if(resultSt > 0){Log(stext,1);}

                
            }//<<<<end method
        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//>end namespaceMain