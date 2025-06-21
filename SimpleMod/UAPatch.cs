using System;
//using System.IO;
//using System.Diagnostics;
using BepInEx;
using HarmonyLib;

using UnityEngine;
//using BepInEx.Configuration;

//using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using s649ASU.Core;
using s649ElinLog;
using static s649ElinLog.ElinLog;
//using s649ASU.Main;
namespace s649ASU
{//namespace main
    namespace UseAbilityPatch
    {//namespace sub
        [HarmonyPatch]
        internal class UAPExe
        {//class[@@@@@@@@@@]
            //---entry------------------------------
            private static readonly string modNS = "UAP";
            //----nakami-------------------
            //AmbiguousMatchException対策
            //new Type[] { typeof(string), typeof(Card), typeof(Point), typeof(bool)}
            //new Type[] { typeof(Act), typeof(Card), typeof(Point), typeof(bool)}

            [HarmonyPostfix]
            [HarmonyPatch(typeof(Chara), "UseAbility",new Type[] { typeof(string), typeof(Card), typeof(Point), typeof(bool)})]
            internal static void UAPostfixA(Chara __instance, ref string idAct, ref Card tc, ref Point pos, ref bool pt)
            {

                //nakami
                //string dt = "[ASU-UA]";
                //dt += Kakomu("idAct:" + idAct);
                //dt += Kakomu("tc:" + Main.SName(tc));
                //dt += Kakomu("pos:" + ((pos != null)? pos.ToString() : "-"));
                //dt += Kakomu("pt:" + TorF(pt));
                //Main.Log(dt,1);
                
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(Chara), "UseAbility", new Type[] { typeof(Act), typeof(Card), typeof(Point), typeof(bool)})]
            internal static void UAPostfixB(Chara __instance, ref Act a, ref Card tc, ref Point pos, ref bool pt)
            {
                //nakami
                string title = "C.UA";
                ClearLogStack();
                LogStack("[" + modNS + "/" + title + "]");
                List<string> checkThings = new();
                string checktext = "";
                Chara c_user = __instance;
                try
                {
                    checkThings.Add("C:" + StrConv(c_user));
                    checkThings.Add("Act:" + StrConv(a));
                    checkThings.Add("tc:" + StrConv(tc));
                    checkThings.Add("P:" + StrConv(pos));
                }
                catch (NullReferenceException ex)
                {
                    LogError("ArgCheckFailed for NullPo");
                    checktext = string.Join("/", checkThings);
                    LogError(checktext);
                    Debug.Log(ex.Message);
                    Debug.Log(ex.StackTrace);
                    return;
                }
                if (!c_user.IsPC) { return; }
                checktext = string.Join("/", checkThings);
                LogDeep(checktext);
                
                /*
                string dt = "[ASU-UA]";
                dt += "C:" + StrConv(__instance);
                dt += Kakomu("Act:" + a.ToString());
                dt += Kakomu("tc:" + Main.SName(tc));
                dt += Kakomu("pos:" + ((pos != null)? pos.ToString() : "-"));
                dt += Kakomu("pt:" + TorF(pt));
                Main.Log(dt,1);
                */
                if (a is Spell) 
                {
                    LogOther("Act is Spell");
                    if (c_user.IsPC) 
                    {
                        //LogOther("isPC");
                        if (pt)
                        {
                            if (tc != null) { c_user.ModExp(Main.ID_ControlMana, 1); }
                            
                            c_user.ModExp(Main.ID_ManaCapacity, 1);
                        }
                        else 
                        {
                            //c_user.ModExp(Main.ID_ControlMana, 1);
                            c_user.ModExp(Main.ID_ManaCapacity, 1);
                        }
                        LogOther("ASU Done");
                    }
                }
            }

            //methods---------------------------------------------------------
            private static string TorF(bool b){return (b)?"T":"F";}
            private static string Kakomu(string s){return "/"+s+"/";}
            //private static string SName(Card c){
            //    return Main.SName(c);
            //}
            //private static string SName(Chara c){
            //    return Main.SName(c);
            //}

        }//class[@@@@@@@@@@]
    }//namespace sub
}//namespace main