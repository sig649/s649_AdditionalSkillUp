using System;
using System.IO;
using System.Diagnostics;
using BepInEx;
using HarmonyLib;

using UnityEngine;
using BepInEx.Configuration;

using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using s649ASU.PatchMain;
//using s649ASU.Main;
namespace s649ASU
{//namespace main
    namespace UseAbilityPatch
    {//namespace sub
        [HarmonyPatch]
        internal class UAPExe
        {//class[@@@@@@@@@@]
            //---entry------------------------------
            
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
                string dt = "[ASU-UA]";
                dt += Kakomu("Act:" + a.ToString());
                dt += Kakomu("tc:" + Main.SName(tc));
                dt += Kakomu("pos:" + ((pos != null)? pos.ToString() : "-"));
                dt += Kakomu("pt:" + TorF(pt));
                Main.Log(dt,1);
                
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