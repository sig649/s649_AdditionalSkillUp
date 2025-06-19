using System;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using static s649ElinLog.ElinLog;
using s649ASU.Core;


namespace s649ASU
{//namespace main
    namespace APPatch
    {//namespace sub
        [HarmonyPatch]
        internal class PatchExe
        {//class[@@@@@@@@@@]
         //----nakami-------------------
            private static readonly string modNS = "APP";
            [HarmonyPostfix]
            [HarmonyPatch(typeof(AttackProcess), "ModExpAtk")]
            internal static void MEAPostPatch(AttackProcess __instance, int ele, int mod)
            {
                //nakami
                ClearLogStack();
                string title = "AP.MEA";
                LogStack("[" + modNS + "/" + title + "]");

                PatchCore(__instance, ele, mod, true);


                

            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(AttackProcess), "ModExpDef")]
            internal static void MEDPostPatch(AttackProcess __instance, int ele, int mod)
            {
                //nakami
                ClearLogStack();
                string title = "AP.MED";
                LogStack("[" + modNS + "/" + title + "]");

                PatchCore(__instance, ele, mod, false);
            }

            private static void PatchCore(AttackProcess attackProcess, int ele, int mod, bool isAtk)
            {
                List<string> checkThings = new();
                string checktext = "";
                Chara c_user;
                bool isParasite;
                bool isRiding;


                try
                {
                    c_user = attackProcess.CC;
                }
                catch (NullReferenceException ex)
                {
                    LogError("CharaCheckFailed");
                    //checktext = string.Join("/", checkThings);
                    //LogError(checktext);
                    Debug.Log(ex.Message);
                    Debug.Log(ex.StackTrace);
                    return;
                }
                if (!c_user.IsPC) { return; }
                try
                {
                    //c_user = __instance.CC;
                    checkThings.Add("C:" + StrConv(c_user));
                    isParasite = c_user.parasite != null;
                    isRiding = c_user.ride != null;
                    checkThings.Add("iP:" + StrConv(isParasite));
                    checkThings.Add("iR:" + StrConv(isRiding));
                    checkThings.Add("Ele:" + StrConv(ele));
                    checkThings.Add("mod:" + StrConv(mod));
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
                checktext = string.Join("/", checkThings);
                LogDeep(checktext);

                //exe
                int eleID = isAtk ? Main.ID_Riding : Main.ID_Parasite;
                c_user.ModExp(eleID, 1);
                LogDeep("AddExp:" + StrConv(eleID));
            }

        }//class[@@@@@@@@@@]
}//namespace sub
}//namespace main