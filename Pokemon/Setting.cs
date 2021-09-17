using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon
{
    public class Setting
    {
        /* --- フィールド --- */


        /* --- 全般 --- */

        public static int PIKACHU = 25; // ピカチュウのポケモンID

        public static int MAX_POKEMON_COUNT = 893; // ポケモンIDの最大値

        public static int LEVEL = 50; // バトルさせるポケモンのレベル

        public static bool HANDICAP = true; // ピカチュウにハンデ(食べ残し)を持たせるかどうか


        /* --- 努力値 --- */

        // HP
        public static int EV_HP = 0;

        // 攻撃
        public static int EV_ATTACK = 0;

        // 防御
        public static int EV_DEFENCE = 0;

        // とくこう
        public static int EV_SATTACK = 0;

        // とくぼう 
        public static int EV_SDEFENCE = 0;

        // 素早さ
        public static int EV_SPEED = 0;


        /* --- 個体値 --- */

        // HP
        public static int IV_HP = 31;

        // 攻撃
        public static int IV_ATTACK = 31;

        // 防御
        public static int IV_DEFENCE = 31;

        // とくこう
        public static int IV_SATTACK = 31;

        // とくぼう
        public static int IV_SDEFENCE = 31;

        // 素早さ
        public static int IV_SPEED = 31;


        /* --- コンストラクタ　--- */
        private Setting() { }


        /* --- メソッド --- */
        // セッティングの妥当性チェック
        public static void ValidityCheck() 
        {

            // レベルは1～100以外の範囲をとりえない
            if (LEVEL < 1 || LEVEL > 100)
                throw new InvalidStatusException("Setting.csのレベルの値がおかしいです");

            // 個体値制限
            // 各々の個体値の上限は31
            if (IV_HP > 31 || IV_ATTACK > 31 || IV_DEFENCE > 31 || IV_SATTACK > 31 || IV_SDEFENCE > 31 || IV_SPEED > 31)
                throw new InvalidStatusException("ポケモンの個体値が異常です：個体値は最大で31です");

            //  各々の努力値はMAX:252かつすべての要素の合計値が510以下
            if (EV_HP > 252 || EV_ATTACK > 252 || EV_DEFENCE > 252 || EV_SATTACK > 252 || EV_SDEFENCE > 252 || EV_SPEED > 252 || (EV_HP + EV_ATTACK + EV_DEFENCE + EV_SATTACK + EV_SDEFENCE + EV_SPEED > 510))
                throw new InvalidStatusException("ポケモンの努力値が異常です");


        }


    }
}
