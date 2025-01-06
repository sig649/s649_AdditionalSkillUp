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
    [HarmonyPatch]
    public class PatchAct {
        internal const int ID_WL = 207;
        internal const int ID_Stealth = 152;

        //+++++++++++ config loading ++++++++++++++++++++++++++++++++++++++++++
        static bool configPCGetableExp => AdditionalSU.ASUMain.IsPlayerGetableExp;
        static bool configPetGetableExp => AdditionalSU.ASUMain.IsPetGetableExp;
        static bool configInfluenceWeight => AdditionalSU.ASUMain.DoInfluenceWeight;

        static bool configDebug => AdditionalSU.ASUMain.IsDebugMode;
        static bool configCheat => AdditionalSU.ASUMain.IsCheatMode;
        static bool configPCFAE => AdditionalSU.ASUMain.DoPCForceAwardExp;
        static bool configPetFAE => AdditionalSU.ASUMain.DoPetForceAwardExp;

        static float configPCExpM => AdditionalSU.ASUMain.ParamPCExpMultiplier;
        static float configPetExpM => AdditionalSU.ASUMain.ParamPetExpMultiplier;
        static float configPCFreqM => AdditionalSU.ASUMain.ParamPCFrequencyMultiplier;
        static float configPetFreqM => AdditionalSU.ASUMain.ParamPetFrequencyMultiplier;

        //++++++++++++  メソッド　+++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static void logging(string t)
        {
            UnityEngine.Debug.Log(t);
        }

        public static bool isGetableExp(Chara c)  //configから読み込む
        {
            if(c.IsPC){
                return configPCGetableExp;
            }else{
                return configPetGetableExp;
            }
        }

        /////seedをベースにr1で抽選 && r1>=seedならtrue  ///return をintに変更
        public static int isProbabilityTrue(int r1, int seed)
        { 
            int rsd = UnityEngine.Random.Range(0, seed);
            return (r1 >= rsd) ? 1 : 0;
            //return UnityEngine.Random.Range(0, seed) < r1;
        }

        public static bool isOnGlobalMap(){
            return (EClass.pc.currentZone.id == "ntyris") ? true : false;
        }
        public static int rng(int min, int max){
            return UnityEngine.Random.Range(min, max);
        }

        public static float getExpMulti(Chara c){
            float f = (c.IsPC) ? configPCExpM : configPetExpM;
            //範囲異常の処理
            //if (f < 0.01 || f > 100) { f = 1.0f; }
            if(f < 0.01){ f = 0.01f; } else {
                if(f > 100){ f = 100.0f; }
            }
            return f;
        }

        public static float getFreqMulti(Chara c){
            float f = (c.IsPC) ? configPCFreqM : configPetFreqM;
            //範囲異常の処理
            //if (f < 0.1 || f > 10) { f = 1.0f; }
            if(f < 0.1){ f = 0.1f; } else {
                if(f > 10){ f = 10.0f; }
            }
            return f;
        }
        


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Chara), "TickConditions")]
        public static void TickConditions_PostPatch(Chara __instance)
        {
            // **********負荷軽減のためのゼロ次抽選*************************************
            //logging("Fooked : TC" + isOnGlobalMap().ToString());
            if(isOnGlobalMap()){
                if(rng(0,9) != 0){ return; }
            }
            //if (UnityEngine.Random.Range(0, 99) >= 50) { return; }  //強制return 毎Tは流石に自粛 //0にすると毎T

            
            Chara c = __instance;  //c の読み込み
            bool flagPC = c.IsPC; 
            if (!(c.IsPC || c.IsPCParty)) { return; }   //PC or Pet check　　強制return2
            //logging("C is " + c.ToString());
            //+++++++++++++configの処理++++++++++++++++++++++++++++++++++++++++++++
            float freqMulti = getFreqMulti(c); //乗算用  //configCheat時用
            float expMulti = getExpMulti(c);  //同上

            bool flagFAE = (flagPC) ? configPCFAE : configPetFAE;//強制判定のコンフィグ呼びだし
            bool flagGetableExp = isGetableExp(c);  //経験値取得可能か

            //*********  cの参照 *********************************************
            
            
            
            
            Element eWL = c.elements.GetElement(ID_WL);
            Element eStealth = c.elements.GetElement(ID_Stealth);
            bool flagHasWL = (eWL != null) ? true : false; //スキル所持チェック
            bool flagHasStealth = (eStealth != null) ? true : false; //スキル所持チェック

            //logging("C ref finish");
           
            //if (c.IsPC) { flagPC = true; flagPet = false; } //flagSet
            //else { flagPC = false; flagPet = true; }

            

            //--- c status refference ---------------------------------------------------------------
            int inv, wl, iwr;    //所持重量[g:mS]/重量限界[g:mS]/所持重量比[%]

            inv = c.ChildrenWeight;  //キャラの所持重量[mS:g]
            wl = c.WeightLimit;      //キャラの重量限界[mS]
            iwr = inv * 100 / wl;     //所持重量比  [%]100以上で重荷

            //int w = c.bio.weight;      //[Kg ]  notuse
            int sw = c.SelfWeight;       //[mS:g]体重
            int h = c.bio.height;        //[cm]身長
            int cWLbase = (eWL != null) ? eWL.vBase : 0;
            int cWLexp = (eWL != null) ? eWL.vExp : 0;
            int cSteBase = (eStealth != null) ? eStealth.vBase : 0;
            int cSteExp = (eStealth != null) ? eStealth.vExp : 0;



            //------local num-------------------------------------------------------------------
            int r1, r2;     //乱数
            int sd1, sd2;   //シード値

            // -------- local num for gatya -----------------------//vanillaのModweightの処理を参考に
            int minw = (h * h) * 18 / 25;   // [mS:g]   //not use
            int maxw = (h * h) * 24 / 10;   // [mS:g]

            int rmin = sw * 100 / minw;    // [%] 100~333?  //not use
            int rmax = sw * 100 / maxw;    // [%] 100~333?
            //int wrmax = maxw  * 100/ minw ; // [%] const int 333?参考

            //----- gatya time----------------------------------------------------
            //logging("before gatya");
            //bool flag1stGatya;  //抽選判定
            int gatyaTickets;
            if(flagHasWL){
                gatyaTickets = (isOnGlobalMap()) ? 10 : 1; //抽選回数 //globalmapなら10連ガチャとする
            } else {gatyaTickets = 0;}
            
            int atari1stGatya = 0;    //抽選結果:当たり回数
            int resExp = 0;           //取得経験値(ModExpの処理に準ずる)

            // ++++++++ 一次抽選 +++++++++++++++++++++++++++++++++
            const int IWRLIMIT = 200;  //seed1の上限
            sd1 = (iwr > IWRLIMIT) ? IWRLIMIT : iwr;           // seed1 0~200 の範囲のシード

            for(int i = 0; i < gatyaTickets; i++){
                
                r1 = rng(0, sd1 * sd1);       // 0 ~ 40000 の乱数?

                //チートON なら、r1に頻度倍率を掛ける・・・下限を１としておきます
                if (configCheat) {
                    r1 = (int)((r1 * freqMulti) < 1.0f ? 1.0f : r1 * freqMulti);
                }

                //-------- normal gatya -------------------------------
                if(flagFAE){
                    atari1stGatya += 1;
                } else {
                    atari1stGatya += isProbabilityTrue(r1, IWRLIMIT * IWRLIMIT); 
                }
            }
            
            //logging("aftergatya");                                             
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

            float fExp = 0.0f;
            for(int i = 0; i < atari1stGatya; i++){
                fExp += (float)rng(0, sd2);
            }

            if (configCheat) { resExp = (int)(fExp * expMulti); }
                else {resExp = (int)fExp;}
            resExp /= 100;
            
            
            if (resExp < 1) { resExp = 1; }        //最少1exp
            if (resExp > 1000) { resExp = 1000; }  //経験値上限1000exp

            //logging("before exe");
            // **************** execute ****************************************************
            // 成否判定 : 
            if (atari1stGatya == 0 || !flagHasWL) { goto labelDebugOut; } //こっちでもいいような気がするが見やすさの為->こっちにした
            /*
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
            */

            //if (!flagHasWL) { goto labelDebugOut; }  //スキル所持？
            if (flagGetableExp) { c.ModExp(207, resExp); }


            

            labelDebugOut:
            // @@@@@@@@ debug_info_output @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            
            //------debugtext for refference info
            string reft = ("sw:" + sw.ToString());  //体重
            reft += ("/h:" + h.ToString());         //身長
            reft += ("/inv:" + inv.ToString());     //所持
            reft += ("/wl:" + wl.ToString());       //限界
            reft += ("/HasWL:" + flagHasWL.ToString());     //WL持ち？
            reft += ("/bWL:" + cWLbase.ToString() + "&" + cWLexp.ToString());    //WL値
            reft += ("/HasSt:" + flagHasStealth.ToString());
            reft += ("/bSt:" + cSteBase.ToString() + "&" + cSteExp.ToString());
            //------debugtext for local info
            //string loct = ("minw:" + minw.ToString());  //最小体重
            //loct += ("/maxw:" + maxw.ToString());       //最大体重
            //loct += ("/iwr:" + iwr.ToString());         //所持重量比
            //loct += ("/rmin:" + rmin.ToString());       //最小比率
            //loct += ("/rmax:" + rmax.ToString());       //最大比率
            //loct += ("/wrmax:" + wrmax.ToString());

            // ------debug text for execute ---------------------------------------------

            string rt = ("atari:" + atari1stGatya.ToString());
            rt += ("][resE:" + resExp.ToString());
            rt += ("][sd1:" + sd1.ToString());
            rt += ("][sd2:" + sd2.ToString());
            //rt += ("/f2:" + f2.ToString());
            //rt += ("/f3:" + f2.ToString());

            //string gatyaT = ("");
            //gatyaT += ("[r1:" + r1.ToString());
            //gatyaT += ("][r2:" + r2.ToString());
           // gatyaT += ("][tg:" + tg.ToString());

            
            //logging("before deb out");

            if (configDebug && isGetableExp(c))
            {
                logging("[AWL] info output -------------------------------");
                //logging("[AWL] Config -> [" + debt + "]");
                logging("[AWL] Cref :" + c.GetName(NameStyle.Simple) + " ->[" + reft + "]");
                //logging("[AWL] gatya -> [" + gatyaT + "]");
                //logging("[AWL] locals : -> [" + loct + "]");
                logging("[AWL] Result -> [" + rt + "]");
                
            }




            //@@@@@@@@@@@@@@@@ finish @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        /*
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Zone), "Activate")]
        public static void Activate_PostPatch(Zone __instance){
            Zone z = __instance;
            //isAtGlobalMap = (z.getName(NameStyle.Simple) == )? : ;
            //logging("[ASU] At:Activate : " + z.GetName(NameStyle.Simple));

        }
        */

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

