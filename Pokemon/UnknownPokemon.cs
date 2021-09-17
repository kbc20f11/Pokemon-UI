using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pokemon
{
    public class UnknownPokemon : Pokemon
    {
        /* --- フィールド --- */

        // Form1を受け取るためのフィールド
        private readonly Form1 Form1Obj;


        /* --- コンストラクタ --- */
        public UnknownPokemon(int id, Form1 Form1_Obj) : base(id)
        {
            // Form1を受け取り
            Form1Obj = Form1_Obj;
        }


        /* --- メソッド --- */
        public override void Attack(int moveId, Pokemon df)
        {
            // ダメージを格納するためのローカル変数を準備
            int damage = 0;

            // 入力によって場合分け
            switch (moveId)
            {
                case 1:
                    Form1Obj.labelA.Text = GetName() + "の" + GetMove1().GetName();
                    DamageCalc dc1 = new DamageCalc(GetMove1(), this, df, Form1Obj);
                    damage = dc1.GetDamage();
                    break;
                case 2:
                    Form1Obj.labelA.Text = GetName() + "の" + GetMove2().GetName();
                    DamageCalc dc2 = new DamageCalc(GetMove2(), this, df, Form1Obj);
                    damage = dc2.GetDamage();
                    break;
                case 3:
                    Form1Obj.labelA.Text = GetName() + "の" + GetMove3().GetName();
                    DamageCalc dc3 = new DamageCalc(GetMove3(), this, df, Form1Obj);
                    damage = dc3.GetDamage();
                    break;
                case 4:
                    Form1Obj.labelA.Text = GetName() + "の" + GetMove4().GetName();
                    DamageCalc dc4 = new DamageCalc(GetMove4(), this, df, Form1Obj);
                    damage = dc4.GetDamage();
                    break;
            }

            // HPをセット
            df.SetHp(df.GetHp() - damage);

            // 与えたダメージが0より大きい場合は与ダメを表示
            if (damage > 0)
                Form1Obj.labelA.Text += "\r\n" + df.GetName() + "に" + damage + "ダメージを与えた ▼";

        }


        // 自動攻撃メソッド
        public void AutoAttack(Pokemon df)
        {

            // 1～4の乱数を生成する
            Random r = new Random();
            int moveId = r.Next(0, 4) + 1;

            // 生成した乱数を技IDに指定して攻撃メソッドを呼び出す
            this.Attack(moveId, df);
        }
    }
}
