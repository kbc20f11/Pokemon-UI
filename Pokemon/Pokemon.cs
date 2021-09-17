using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// ポケモンの抽象クラスです
/// </summary>
/// 

namespace Pokemon
{
    public abstract class Pokemon
    {
        /* --- フィールド --- */

        // ポケモンID
        private readonly int _id;

        // 名前
        private readonly string _name = string.Empty;

        // アイコンのURL
        private readonly string _iconUrl = string.Empty;

        // タイプ
        private readonly string _type = string.Empty;

        // タイプ(二つ目があれば)
        private readonly string _type2 = string.Empty;

        // hp
        private int _hp;

        // 攻撃力
        private readonly int _attack;

        // 防御力
        private readonly int _defence;

        // とくこう
        private readonly int _sAttack;

        // とくぼう
        private readonly int _sDefence;

        // 素早さ
        private readonly int _speed;

        // 技
        private readonly Move _m1;
        private readonly Move _m2;
        private readonly Move _m3;
        private readonly Move _m4;



        /* --- コンストラクタ --- */

        public Pokemon(int id)
        {
            // ポケモンIdをフィールドに代入
            _id = id;

            // クラスDownloadInfoを初期化
            DownloadInfo di = new DownloadInfo(id);

            // ステータスを計算した後それぞれ代入
            // 名前
            _name = di.GetName();

            // icon
            _iconUrl = di.GetPokemonIcon();

            // タイプ
            if (string.IsNullOrEmpty(di.GetType2()))
                _type = di.GetType();
            else
            {
                _type = di.GetType();
                _type2 = di.GetType2();
            }


            // hp
            _hp = HpResolver(int.Parse(di.GetHp()));

            // 攻撃力
            _attack = AttackResolver(int.Parse(di.GetAttack()));

            // 防御力
            _defence = DefenceResolver(int.Parse(di.GetDefence()));

            // とくこう力
            _sAttack = SAttackResolver(int.Parse(di.GetSAttack()));

            // とくぼう力
            _sDefence = SDefenceResolver(int.Parse(di.GetSDefence()));

            // 素早さ
            _speed = SpeedResolver(int.Parse(di.GetSpeed()));

            // クラスMoveのインスタンスを受け取る
            // インスタンス化した後、フィールドに代入
            _m1 = new Move(di.GetMove1());
            _m2 = new Move(di.GetMove2());
            _m3 = new Move(di.GetMove3());
            _m4 = new Move(di.GetMove4());

        }


        /* --- メソッド --- */

        // 抽象メソッド
        public abstract void Attack(int moveId, Pokemon df);

        // ToStringのオーバーライド
        public override String ToString()
        {

            // local variable for return string
            string retString;

            // check if pokemon has a type or two types
            if (string.IsNullOrEmpty(GetType2()))
            {
                retString = "図鑑No:" + _id + ", 名前:" + _name + ", タイプ:" + _type + ", HP:" + _hp + ", 攻撃:" + _attack + ", 防御:" + _defence
                     + ", とくこう:" + _sAttack + ", とくぼう" + _sDefence + ", すばやさ:" + _speed;
            }
            else
            {
                retString = "図鑑No:" + _id + ", 名前:" + _name + ", タイプ:" + _type + ", " + _type2 + ", HP:" + _hp + ", 攻撃:" + _attack + ", 防御:" + _defence
                         + ", とくこう:" + _sAttack + ", とくぼう" + _sDefence + ", すばやさ:" + _speed;
            }

            // return string
            return retString;

        }

        /* ゲッター */

        // id
        public int GetId()
        {
            return _id;
        }

        // 名前
        public string GetName()
        {
            return _name;
        }

        // アイコンのUrl
        public string GetIconUrl()
        {
            return _iconUrl;
        }
        
        // タイプ
        // 
        public new string GetType()
        {
            return _type;
        }

        // タイプ2
        public string GetType2()
        {
            return _type2;
        }

        // hp
        public int GetHp()
        {
            return _hp;
        }

        // アタック
        public int GetAttack()
        {
            return _attack;
        }

        // ディフェンス
        public int GetDefence()
        {
            return _defence;
        }

        // とくこう
        public int GetSpecialAttack()
        {
            return _sAttack;
        }

        // とくぼう
        public int GetSpecialDefence()
        {
            return _sDefence;
        }

        // 素早さ
        public int GetSpeed()
        {
            return _speed;
        }

        // move1
        public Move GetMove1()
        {
            return _m1;
        }

        // move2
        public Move GetMove2()
        {
            return _m2;
        }
        
        // move3
        public Move GetMove3()
        {
            return _m3;
        }

        // move4
        public Move GetMove4()
        {
            return _m4;
        }

        /* セッター */
        // HP
        public void SetHp(int hp)
        {
            // at least, HP is greater than 0
            _hp = Math.Max(hp, 0);
        }

        // 種族値から実際のステータスを割り出す
        // HPのリゾルバー
        public int HpResolver(int value)
        {
            double resolvedHp = ((((value * 2) + Setting.IV_HP + (Setting.EV_HP / 4)) * Setting.LEVEL) / 100) + (10 + Setting.LEVEL);
            return (int)resolvedHp;
        }

        // 攻撃力のリゾルバー
        public int AttackResolver(int value)
        {
            double resolvedAttack = ((((value * 2) + Setting.IV_ATTACK + (Setting.EV_ATTACK / 4)) * Setting.LEVEL) / 100) + 5;
            return (int)resolvedAttack;
        }

        // 防御力のリゾルバー
        public int DefenceResolver(int value)
        {
            double resolvedDefence = ((((value * 2) + Setting.IV_DEFENCE + (Setting.EV_DEFENCE / 4)) * Setting.LEVEL) / 100) + 5;
            return (int)resolvedDefence;
        }

        // とくこうのリゾルバー
        public int SAttackResolver(int value)
        {
            double resolvedSAttack = ((((value * 2) + Setting.IV_SATTACK + (Setting.EV_SATTACK / 4)) * Setting.LEVEL) / 100) + 5;
            return (int)resolvedSAttack;
        }

        // とくぼうのリゾルバー
        public int SDefenceResolver(int value)
        {
            double resolvedDefence = ((((value * 2) + Setting.IV_SDEFENCE + (Setting.EV_SDEFENCE / 4)) * Setting.LEVEL) / 100) + 5;
            return (int)resolvedDefence;
        }

        // 素早さのリゾルバー
        public int SpeedResolver(int value)
        {
            double resolvedSpeed = ((((value * 2) + Setting.IV_SPEED + (Setting.EV_SPEED / 4)) * Setting.LEVEL) / 100) + 5;
            return (int)resolvedSpeed;
        }



    }
}
