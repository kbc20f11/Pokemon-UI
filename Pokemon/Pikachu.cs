using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// ピカチュウのクラスです
/// </summary>

namespace Pokemon
{
    public class Pikachu : Pokemon
    {
        /* --- フィールド --- */
        
        // フォーム1のインスタンスを受け取るためのフィールド
        private readonly Form1 Form1Obj;


        /* --- コンストラクタ --- */
        public Pikachu(int id, Form1 Form1_Obj) : base(id) 
        {
            // recieve the Form1
            Form1Obj = Form1_Obj;
        }

        public override void Attack(int moveId, Pokemon df)
        {
            // ダメージのローカル変数を準備
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

            // ダメージをセット
            df.SetHp(df.GetHp() - damage);

            // 与えたダメージが0以上なら与ダメを表示
            if(damage > 0)
                Form1Obj.labelA.Text += "\r\n" + df.GetName() + "に" + damage + "ダメージを与えた　▼";

        }
    }
}
