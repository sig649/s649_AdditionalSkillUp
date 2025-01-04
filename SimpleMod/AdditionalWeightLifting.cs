using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

using UnityEngine;
using Random = UnityEngine.Random;


namespace ASUPatch
{
    public static bool isAtGlobalMap = false;   //globalmapかどうかの判定

    //[HarmonyPatch]
    public class TickConditionsPatch
    {
        

        //+++++++++++ config loading ++++++++++++++++++++++++++++++++++++++++++
        static bool configPCGetableExp => AdditionalWL.AWLMain.flagPCGetableExp.Value;
        static bool configPetGetableExp => AdditionalWL.AWLMain.flagPetGetableExp.Value;
        static bool configInfluenceWeight => AdditionalWL.AWLMain.flagInfluenceWeight.Value;

        static bool configDebug => AdditionalWL.AWLMain.isDebugMode.Value;
        static bool configCheat => AdditionalWL.AWLMain.isCheatMode.Value;
        static bool configPCFAE => AdditionalWL.AWLMain.PCForceAwardExp.Value;
        static bool configPetFAE => AdditionalWL.AWLMain.PetForceAwardExp.Value;

        static float configPCExpM => AdditionalWL.AWLMain.PCExpMultiplier.Value;
        static float configPetExpM => AdditionalWL.AWLMain.PetExpMultiplier.Value;
        static float configPCFreqM => AdditionalWL.AWLMain.PCFrequencyMultiplier.Value;
        static float configPetFreqM => AdditionalWL.AWLMain.PetFrequencyMultiplier.Value;

        //++++++++++++  メソッド　+++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static void logging(string t)
        {
            UnityEngine.Debug.Log(t);
        }

        public static bool IsGetableExp(Chara c)  //configから読み込む
        {
            if(c.IsPC){
                return configPCGetableExp;
            }else{
                return configPetGetableExp;
            }
        }

        /////seedをベースにr1で抽選 && r1>=seedならtrue
        public static bool IsProbabilityTrue(int r1, int seed)
        { 
            int rsd = UnityEngine.Random.Range(0, seed);
            return (r1 >= rsd) ? true : false;
            //return UnityEngine.Random.Range(0, seed) < r1;
        }


        //[HarmonyPostfix]
        [HarmonyPatch(typeof(Chara), "TickConditions")]
        public static void Postfix(Chara __instance)
        {
            // **********負荷軽減のためのゼロ次抽選*************************************

            //if (UnityEngine.Random.Range(0, 99) >= 50) { return; }  //強制return 毎Tは流石に自粛 //0にすると毎T

            //++++++++++++++++++++++++++++++++++++++++++++++++++++

            //キャラ毎の独自変数置き場だよ　　
            //抽選用天井ガチャの処理
            //Dictionary<Chara, int> tgDict = AdditionalWL.AWLMain.tengatyaDict;
            

            //*********  cの参照 *********************************************
            Chara c = __instance;  //c の読み込み
            bool flagPC, flagPet; // わざわざ二つ用意しているのは後の拡張性の為
            bool flagGetableExp = IsGetableExp(c);  //経験値取得可能か
            // bool flagHasWL = c.elements.HasBase(207); //スキル所持チェック
            Element e = c.elements.GetElement(207);
            bool flagHasWL = (e != null) ? true : false; //スキル所持チェック


            if (!(c.IsPC || c.IsPCParty)) { return; }   //PC or Pet check　　強制return2
            if (c.IsPC) { flagPC = true; flagPet = false; } //flagSet
            else { flagPC = false; flagPet = true; }

            
            //--- c status refference ---------------------------------------------------------------
            int inv, wl, iwr;    //所持重量[g:mS]/重量限界[g:mS]/所持重量比[%]

            inv = c.ChildrenWeight;  //キャラの所持重量[mS:g]
            wl = c.WeightLimit;      //キャラの重量限界[mS]
            iwr = inv * 100 / wl;     //所持重量比  [%]100以上で重荷

            //int w = c.bio.weight;      //[Kg ]  notuse
            int sw = c.SelfWeight;       //[mS:g]体重
            int h = c.bio.height;        //[cm]身長
            int cWLbase = (e != null) ? e.vBase : 0;
            int cWLexp = (e != null) ? e.vExp : 0;

            //------local num-------------------------------------------------------------------
            int r1, r2;     //乱数
            int sd1, sd2;   //シード値

            // -------- local num for gatya -------------------------------------------------
            int minw = (h * h) * 18 / 25;   // [mS:g]   //not use
            int maxw = (h * h) * 24 / 10;   // [mS:g]

            int rmin = sw * 100 / minw;    // [%] 100~333?  //not use
            int rmax = sw * 100 / maxw;    // [%] 100~333?
            //int wrmax = maxw  * 100/ minw ; // [%] const int 333?参考

            

            //----- result----------------------------------------------------
            bool flag1stGatya;  //抽選判定
            int resExp;           //取得経験値(ModExpの処理に準ずる)

            //+++++++++++++configの処理++++++++++++++++++++++++++++++++++++++++++++
            float freqMulti = 1.0f; //乗算用  //configCheat時用
            float expMulti = 1.0f;　//同上

            //強制判定のコンフィグ呼びだし
            bool flagFAE = (flagPC) ? configPCFAE : configPetFAE;

            //freqMulti入力範囲の処理・・・適正でない場合はデフォ値を使う
            freqMulti = (flagPC) ? configPCFreqM : configPetFreqM;
            //範囲異常の処理
            if (freqMulti < 0.1 || freqMulti > 10) { freqMulti = 1.0f; }

            //expMultiの処理
            //入力範囲の処理(0.01～100)・・・適正でない場合はデフォ値を使う
            expMulti = (flagPC) ? configPCExpM : configPetExpM;
            //範囲異常の処理
            if (expMulti < 0.01 || expMulti > 100) { expMulti = 1.0f; }


            // ++++++++ 一次抽選 +++++++++++++++++++++++++++++++++

            sd1 = (iwr > 200) ? 200 : iwr;           // seed1 0~200 の範囲のシード
            r1 = UnityEngine.Random.Range(0, sd1 * sd1);       // 0 ~ 40000 の乱数?


            //チートON && /*FAEON*/なら、r1に頻度倍率を掛ける・・・下限を１としておきます
            if (configCheat) {
                r1 = (int)((r1 * freqMulti) < 1.0f ? 1.0f : r1 * freqMulti);
            }


            //-------- normal gatya -------------------------------
            flag1stGatya = IsProbabilityTrue(r1, 40000);  
                                                         
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            //++++++++  calc for execute +++++++++++++++++++++++++++++++++++++++++
            if (!configInfluenceWeight) {
                sd2 = 100;
                //rmax = 100;
            } else
            {
                sd2 = rmax;
            }
                
            sd2 = (sd2 > 10000) ? 10000 : sd2;    //limiter 0~10000
            r2 = UnityEngine.Random.Range(0, sd2);
            
            resExp = r2 / 100;
            if(resExp < 1) { resExp = 1; }        //最少処理

            //debug
            //logging("resexp:" + resExp.ToString());
            //configCheat true時限定
            if (configCheat) { resExp = (int)(resExp * expMulti); }

            //debug
            //logging("resexp:" + resExp.ToString());


            if (resExp < 1) { resExp = 1; }        //最少1exp
            if (resExp > 1000) { resExp = 1000; }  //経験値上限1000exp

            //bool f2 = (r2 > 10000)? true : false;
            //bool f3 = (r3 == 0)? true : false;


            // **************** execute ****************************************************
            // 成否判定 : どれか一つでもfalseならreturnを回避する
            //if (!configCheat && !flagFAE && !flag1stGatya) { return; } //こっちでもいいような気がするが見やすさの為
            if (!configCheat)//チートしない？
            {
                //チート無効
                if (!flag1stGatya) { goto labelDebugOut; }
            }
            else
            {
                //チート有効
                if (!flag1stGatya && !flagFAE) { goto labelDebugOut; }
            }


            if (!flagHasWL) { goto labelDebugOut; }  //スキル所持？
            if (flagGetableExp) { c.ModExp(207, resExp); }


            

            labelDebugOut:
            // @@@@@@@@ debug_info_output @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            // +++++++ debugtext ++++++++++++++++++++++++++++++++++++++++++++++++++++
            //------debugtext for refference info
            string reft = ("sw:" + sw.ToString());  //体重
            reft += ("/h:" + h.ToString());         //身長
            reft += ("/inv:" + inv.ToString());     //所持
            reft += ("/wl:" + wl.ToString());       //限界
            reft += ("/HasWL:" + flagHasWL.ToString());     //WL持ち？
            reft += ("/baseWL:" + cWLbase.ToString() + "&" + cWLexp.ToString());    //WL値

            //------debugtext for local info
            string loct = ("minw:" + minw.ToString());  //最小体重
            loct += ("/maxw:" + maxw.ToString());       //最大体重
            loct += ("/iwr:" + iwr.ToString());         //所持重量比
            loct += ("/rmin:" + rmin.ToString());       //最小比率
            loct += ("/rmax:" + rmax.ToString());       //最大比率
            //loct += ("/wrmax:" + wrmax.ToString());

            // ------debug text for execute ---------------------------------------------

            string rt = ("flag1stGatya:" + flag1stGatya.ToString());
            rt += ("][resE:" + resExp.ToString());
            rt += ("][sd1:" + sd1.ToString());
            rt += ("][sd2:" + sd2.ToString());
            //rt += ("/f2:" + f2.ToString());
            //rt += ("/f3:" + f2.ToString());

            string gatyaT = ("");
            gatyaT += ("[r1:" + r1.ToString());
            gatyaT += ("][r2:" + r2.ToString());
           // gatyaT += ("][tg:" + tg.ToString());

            //debugtext for config
            string debt = ("DEBUG:" + configDebug.ToString());
            debt += ("][CHEAT:" + configCheat.ToString());
            debt += ("][FORCE:" + flagFAE.ToString());
            debt += ("][EXPM:" + expMulti.ToString());
            debt += ("][FREQ:" + freqMulti.ToString());
            debt += ("][fGETEXP:" + flagGetableExp.ToString());
            debt += ("][InfWei:" + configInfluenceWeight.ToString());


            if (configDebug && IsGetableExp(c))
            {
                logging("[AWL] info output -------------------------------");
                logging("[AWL] Cref :" + c.GetName(NameStyle.Simple) + " ->[" + reft + "]");
                logging("[AWL] Config -> [" + debt + "]");
                logging("[AWL] gatya -> [" + gatyaT + "]");
                logging("[AWL] locals : -> [" + loct + "]");
                logging("[AWL] Result -> [" + rt + "]");
                
            }




            //@@@@@@@@@@@@@@@@ finish @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        }
        
    }
}



///trashbox////////////////////////////////////////////////////////////////////////////////////////
///
//------- tenjo gatya loading---//bug...
            //int tg;
            /*
            //cのdictチェック
            if (!tgDict.ContainsKey(c))
            {
                tgDict[c] = 0;
            }
            tg = tgDict[c];


            tg += r1;
            if (tg > 100)
            {
                flag1stGatya = true;
                tgDict[c] = 0;
            }
            else
            {
                flag1stGatya = false;
                tgDict[c] += tg;
            }
            */



            //if(f2 && f3){ //体重処理をするはずだったもの
            //c.ModWeight(-1 * res2);  //bugの為実装未定
            /*
            int befw = sw;   
            int aftw = befw * (1000 - res2 * 10) / 1000;
            if(aftw < 1) {aftw = 1;} // 念のため
            //c.SelfWeight = aftw;

            if(aftw != befw){

                c.Say((befw - aftw > 0) ? "weight_gain" : "weight_lose", c);
                Debug.Log("[Debug] weightchange? : " + befw.ToString() + " -> " + aftw.ToString());
                Debug.Log("[Debug] cullentweight? : " + c.SelfWeight.ToString() );
            } 
            */
            //}

