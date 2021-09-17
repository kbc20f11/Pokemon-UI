using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Runtime.InteropServices;


namespace Pokemon
{

    public partial class Form1 : Form
    {

        /* --- フィールド --- */

        // ピカチュウを格納するためのフィールド
        private Pikachu p;

        // 敵のポケモンを格納するためのフィールド
        private UnknownPokemon up;

        // ピカチュウのマックス体力を格納するためのフィールド
        private double maxHp1;

        // 敵のマックス体力を格納するためのフィールド
        private double maxHp2;

        // 選択肢を格納するフィールド
        private int _choice;

        // 進行度のカウンタ
        private int progressCounter = 0;

        // 撃破数のカウンタ
        private int downCounter = 0;

        // ProgressBar関連
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, uint wParam, IntPtr lParam);
        private const uint WM_USER = 0x400;
        private const uint PBM_SETSTATE = WM_USER + 16;
        private const uint PBST_NORMAL = 0x0001;
        private const uint PBST_ERROR = 0x0002;
        private const uint PBST_PAUSED = 0x0003;


        /* --- コンストラクタ --- */

        public Form1()
        {
            // フォームを初期化
            InitializeComponent();

            // イベントを追加
            // ロードしたとき
            this.Load += Form1_Load;

            // ホバー、リーブ
            this.label1.MouseEnter += label1_Hover;
            this.label1.MouseLeave += label1_Leave;
            this.label2.MouseEnter += label2_Hover;
            this.label2.MouseLeave += label2_Leave;
            this.label3.MouseEnter += label3_Hover;
            this.label3.MouseLeave += label3_Leave;
            this.label4.MouseEnter += label4_Hover;
            this.label4.MouseLeave += label4_Leave;
        }


        /* --- メソッド --- */

        /* イベントドリブン */

        // フォームがロードされたとき
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadBattle();
        }


        // ボタンAがクリックされたとき
        private void buttonA_Click(object sender, EventArgs e)
        {

            if (progressCounter == 0)
            {

                // 戦闘開始メッセージその2
                labelA.Text = p.GetName() + " はどうする?";

                // 選択肢を読み込み
                LoadChoices();


                // 進行度を一つ上げる
                progressCounter++;

            }


            if (progressCounter == 4)
            {

                // 戦闘処理
                SecondStrike(p, up);

                // hp表示を更新
                DisplayHp();

                // 体力バーを更新
                RefleshHealthBar();

                // 進行度を1下げる
                progressCounter--;

                // 元の処理に戻る
                return;
            }

            if (progressCounter == 3) // ターン後処理
            {
                // 勝敗判定
                if (p.GetHp() == 0 || up.GetHp() == 0)
                {
                    Judge();
                    return; // ここで処理を止める
                }

                if (Setting.HANDICAP && maxHp1 > p.GetHp())
                {

                    // 食べ残し(毎ターン16分の1回復)の効果発動
                    p.SetHp((int)Math.Min(Math.Floor((double)(p.GetHp() + maxHp1 / 16)), (int)maxHp1));

                    // hp表示を更新
                    DisplayHp();

                    // 体力バーを更新
                    RefleshHealthBar();

                    // メッセージを送信
                    labelA.Text = p.GetName() + " は食べ残しで少し回復 ▼";

                    // 進行度を1下げる
                    progressCounter--;

                    // 元の処理に戻る
                    return;

                }

                // 進行度を1下げる
                progressCounter--;

            }
            if (progressCounter == 2)
            {

                // メッセージを表示
                labelA.Text = p.GetName() + " はどうする?";

                // ラベルのテキストを再読み込み
                ReloadChoices();

            }

            if (progressCounter == 5)
            {
                // 進行度を0に
                progressCounter = 0;

                //
                label1.Enabled = true;
                label2.Enabled = true;
                label3.Enabled = true;
                label4.Enabled = true;



                // 戦闘処理を読み込む
                LoadBattle();


                // ピカチュウのキャラ画像を読み込む
                pictureBox2.Visible = true;


                // イベントハンドルから抜ける
                return;

            }

            // ゲームオーバーの処理
            if (progressCounter == 6)
            {
                // 終了メッセージ
                labelA.Text = "プログラムを終了します。";

                // 進行度を1つ上げる
                progressCounter++;

                // 呼び出し元に戻る
                return;
            }

            if (progressCounter == 7)
            {
                // 自分自身(= Form1)を閉じる
                this.Close();
            }




        }

        // 選択肢1がクリックされたとき
        private void label1_Click(object sender, EventArgs e)
        {

            // 進行度1
            if (progressCounter == 1)
            {
                // 技をロード
                LoadChoices();

                // 進行度を一つ上げる
                progressCounter++;

                // 元の処理に戻る
                return;
            }

            if (progressCounter == 2)
            {
                // 選択肢をフィールドに代入
                _choice = 1;

                // バトル処理を呼ぶ
                Battle();
            }

        }

        // 選択肢2がクリックされたとき
        private void label2_Click(object sender, EventArgs e)
        {
            // 進行度1
            if (progressCounter == 1)
            {
                // ピカチュウを不可視にする
                pictureBox2.Visible = false;

                // ラベルをクリア
                DeactivateChoices();

                // メッセージを更新
                labelA.Text = "うまく逃げ切れた";


                // カウンタをマックスに
                progressCounter = 5;
            }

            if (progressCounter == 2)
            {
                // 選択肢をフィールドに代入
                _choice = 2;

                // バトル処理を呼ぶ
                Battle();

            }
        }


        // 選択肢3がクリックされたとき
        private void label3_Click(object sender, EventArgs e)
        {
            if (progressCounter == 2)
            {
                // 選択肢をフィールドに代入
                _choice = 3;

                // バトル処理を呼ぶ
                Battle();

            }
        }


        // 選択肢4がクリックされたとき
        private void label4_Click(object sender, EventArgs e)
        {
            if (progressCounter == 2)
            {
                // 選択肢をフィールドに代入
                _choice = 4;

                // バトル処理を呼ぶ
                Battle();

            }
        }

        // マウスホバー時の処理

        // label1
        private void label1_Hover(object sender, EventArgs e)
        {
            label1.BackColor = System.Drawing.SystemColors.ActiveCaption;
        }

        private void label1_Leave(object sender, EventArgs e)
        {
            label1.BackColor = System.Drawing.SystemColors.ActiveBorder;
        }

        // label2
        private void label2_Hover(object sender, EventArgs e)
        {
            label2.BackColor = System.Drawing.SystemColors.ActiveCaption;
        }

        private void label2_Leave(object sender, EventArgs e)
        {
            label2.BackColor = System.Drawing.SystemColors.ActiveBorder;
        }

        // label3
        private void label3_Hover(object sender, EventArgs e)
        {
            label3.BackColor = System.Drawing.SystemColors.ActiveCaption;
        }

        private void label3_Leave(object sender, EventArgs e)
        {
            label3.BackColor = System.Drawing.SystemColors.ActiveBorder;
        }

        // label4
        private void label4_Hover(object sender, EventArgs e)
        {
            label4.BackColor = System.Drawing.SystemColors.ActiveCaption;
        }

        private void label4_Leave(object sender, EventArgs e)
        {
            label4.BackColor = System.Drawing.SystemColors.ActiveBorder;
        }


        /* --- UI操作 --- */
        // 選択肢読み込み
        public void LoadChoices()
        {
            if (progressCounter == 0)
            {
                label1.Text = "1.戦う";
                label2.Text = "2.逃げる";
                label3.Text = "";
                label4.Text = "";
            }
            else
            {
                // 技の名前をロード
                label1.Text = p.GetMove1().GetName();
                label1.Text += "\r\n\r\nタイプ: " + p.GetMove1().GetType();
                label1.Text += ",  威力: " + p.GetMove1().GetPower();
                label2.Text = p.GetMove2().GetName();
                label2.Text += "\r\n\r\nタイプ: " + p.GetMove2().GetType();
                label2.Text += ",  威力: " + p.GetMove2().GetPower();
                label3.Text = p.GetMove3().GetName();
                label3.Text += "\r\n\r\nタイプ: " + p.GetMove3().GetType();
                label3.Text += ",  威力: " + p.GetMove3().GetPower();
                label4.Text = p.GetMove4().GetName();
                label4.Text += "\r\n\r\nタイプ: " + p.GetMove4().GetType();
                label4.Text += ",  威力: " + p.GetMove4().GetPower();
            }
        }

        // 選択肢再読み込み
        public void ReloadChoices()
        {
            // 技をロード
            LoadChoices();

            // クリックを有効にする
            label1.Enabled = true;
            label2.Enabled = true;
            label3.Enabled = true;
            label4.Enabled = true;
        }


        // 選択肢非アクティブ化
        public void DeactivateChoices()
        {
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";
            label4.Text = "";
            label1.Enabled = false;
            label2.Enabled = false;
            label3.Enabled = false;
            label4.Enabled = false;
        }

        // 体力数値処理
        public void DisplayHp()
        {
            label8.Text = p.GetHp().ToString();
        }
       
        // ポケモンが攻撃されたときに点滅させる
        // ピカチュウ
        public void FlushPikachu()
        {
            pictureBox2.Visible = false;
            Thread.Sleep(500);
            pictureBox2.Visible = true;
        }

        // 敵のポケモン
        public void FlushEnemy()
        {
            pictureBox1.Visible = false;
            Thread.Sleep(500);
            pictureBox1.Visible = true;
        }

        public void RefleshHealthBar()
        {
            // 現在の体力を最大体力からみた百分率で表す

            // ピカチュウ
            double tempHp1 = Math.Round(p.GetHp() / maxHp1 * 100);

            // 相手
            double tempHp2 = Math.Round(up.GetHp() / maxHp2 * 100) ;



            // それぞれをプログレスバーに表示
            progressBar1.Value = (int)tempHp2;
            progressBar2.Value = (int)tempHp1;

            // 体力に応じて表示を変える
            if (tempHp2 <= 50)
            {
                ProgressStateYellow(1);
            }
            else
            {
                ProgressStateGreen(1);
            }

            if (tempHp1 <= 50)
            {
                ProgressStateYellow(2);
            }
            else
            {
                ProgressStateGreen(2);
            }

            if (tempHp2 <= 25)
            {
                ProgressStateRed(1);
            }

            if (tempHp1 <= 25)
            {
                ProgressStateRed(2);
            }


        }

        /* --- バトル処理 --- */

        public void Battle()
        {
            // 一ターン目
            FirstStrike(p, up);

            // hp表示を更新
            DisplayHp();

            // 体力バーを更新
            RefleshHealthBar();

            // 勝敗判定
            Judge();

            if (p.GetHp() == 0 || up.GetHp() == 0)
                return;


            // 選択肢を不可視に
            DeactivateChoices();


            // 進行度カウンタを4にする
            progressCounter = 4;
        }

        /* --- 先攻後攻処理 --- */

        // 先攻
        public void FirstStrike(Pikachu a, UnknownPokemon b)
        {
            // どちらが先攻か？
            if(a.GetSpeed() >= b.GetSpeed())
            {
                // 攻撃
                a.Attack(_choice, b);

                // 被弾アニメーション
                FlushEnemy();
            }
            else
            {
                // 攻撃
                b.AutoAttack(a);

                // 被弾アニメーション
                FlushPikachu();
            }
            
        }

        // 後攻
        public void SecondStrike(Pikachu a, UnknownPokemon b)
        {
            // どちらが後攻か？
            if (a.GetSpeed() < b.GetSpeed())
            {
                // 攻撃
                a.Attack(_choice, b);

                // 被弾アニメーション
                FlushEnemy();

            }
            else
            {
                // 攻撃
                b.AutoAttack(a);

                // 被弾アニメーション
                FlushPikachu();
            }

        }

        // 勝敗判定
        public void Judge()
        {
            // ピカチュウ負け
            if (p.GetHp() == 0) 
            {
                // 選択肢を非表示に
                DeactivateChoices();

                // 進行度カウンタを6に
                progressCounter = 6;


                // メッセージ
                labelA.Text += "\r\nピカチュウ は倒れた";
                labelA.Text += "\r\nGame Over";
                labelA.Text += "\r\n" + up.GetName() + "の残りHP: " + up.GetHp();
                labelA.Text += "\r\n撃破数: " + this.downCounter;
            }

            // 相手負け
            if (up.GetHp() == 0)
            {
                // 進行度カウンタを5に
                progressCounter = 5;

                // 選択肢を非表示に
                DeactivateChoices();

                // Aボタンを使えなくする
                // buttonA.Enabled = false;

                // メッセージ
                labelA.Text += "\r\n野生の " + up.GetName() + "は倒れた";
                labelA.Text += "\r\nYou Win!!";
                labelA.Text += "\r\n" + p.GetName() + "の残りHP: " + p.GetHp();

                // 撃破カウンターをインクリメント
                downCounter++;


            }


        }

        public void ProgressStateYellow(int number)
        {
            // ProgressBar1の状態をポーズにする
            if (progressBar1.IsHandleCreated && number ==1)
            {
                SendMessage(new HandleRef(progressBar1, progressBar1.Handle),
                    PBM_SETSTATE, PBST_PAUSED, IntPtr.Zero);

                //Maximumを1増やして、戻す
                progressBar1.Maximum++;
                progressBar1.Maximum--;

            }
            // rogressBar2の状態をポーズにする
            if (progressBar2.IsHandleCreated && number == 2)
            {
                SendMessage(new HandleRef(progressBar2, progressBar2.Handle),
                    PBM_SETSTATE, PBST_PAUSED, IntPtr.Zero);

                //Maximumを1増やして、戻す
                progressBar2.Maximum++;
                progressBar2.Maximum--;

            }
        }

        public void ProgressStateRed(int number)
        {
            // ProgressBar1の状態をエラーにする
            if (progressBar1.IsHandleCreated && number == 1)
            {
                SendMessage(new HandleRef(progressBar1, progressBar1.Handle),
                    PBM_SETSTATE, PBST_ERROR, IntPtr.Zero);

                //Maximumを1増やして、戻す
                progressBar1.Maximum++;
                progressBar1.Maximum--;

            }
            // rogressBar2の状態をエラーにする
            if (progressBar2.IsHandleCreated && number == 2)
            {
                SendMessage(new HandleRef(progressBar2, progressBar2.Handle),
                    PBM_SETSTATE, PBST_ERROR, IntPtr.Zero);

                //Maximumを1増やして、戻す
                progressBar2.Maximum++;
                progressBar2.Maximum--;

            }
        }

        public void ProgressStateGreen(int number)
        {
            // ProgressBar1の状態をノーマルにする
            if (progressBar1.IsHandleCreated && number == 1)
            {
                SendMessage(new HandleRef(progressBar1, progressBar1.Handle),
                    PBM_SETSTATE, PBST_NORMAL, IntPtr.Zero);

                //Maximumを1増やして、戻す
                progressBar1.Maximum++;
                progressBar1.Maximum--;

            }
            // rogressBar2の状態をノーマルにする
            if (progressBar2.IsHandleCreated && number == 2)
            {
                SendMessage(new HandleRef(progressBar2, progressBar2.Handle),
                    PBM_SETSTATE, PBST_NORMAL, IntPtr.Zero);

                //Maximumを1増やして、戻す
                progressBar2.Maximum++;
                progressBar2.Maximum--;

            }
        }

        public void LoadBattle()
        {

            // ステータスの妥当性チェック
            try
            {
                Setting.ValidityCheck();
            }
            catch (Exception ex)
            {
                // エラーのメッセージボックスを出す
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // フォームを閉じる
                this.Close();
            }

            // Randomをインスタンス化
            Random r = new Random();

            // ---

            // ポケモンをインスタンス化してフィールドに代入
            // Pikachu

            if (downCounter == 0)
            p = new Pikachu(Setting.PIKACHU, this);

            // 敵のポケモン
            // 乱数を引数に使ってUnknownPokemonをインスタンス化
            // 攻撃技が4つあるポケモンが当たるまでループ
            do
            {
                up = new UnknownPokemon(r.Next(0, Setting.MAX_POKEMON_COUNT) + 1, this);
            } while (string.IsNullOrEmpty(up.GetMove1().GetName()));

            // すでに撃破したポケモンがいる場合は全回復
            if (downCounter > 0)
                p.SetHp((int)maxHp1);

            // 現状のHP (=マックス体力)をフィールドに代入
            maxHp1 = p.GetHp();
            maxHp2 = up.GetHp();

            // 敵のポケモンの画像をロードする
            pictureBox1.ImageLocation = up.GetIconUrl();

            // 名前のロード
            label10.Text = p.GetName();
            label9.Text = up.GetName();

            // レベルのロード
            label5.Text = Setting.LEVEL.ToString();
            label6.Text = Setting.LEVEL.ToString();

            // HPのロード
            label7.Text = p.GetHp().ToString();
            label8.Text = p.GetHp().ToString();

            // 二体目以上の場合のプログレスバーの初期化処理
            if (downCounter > 0)
            {
                // プログレスバーの値を100に
                progressBar1.Value = 100;
                progressBar2.Value = 100;

                // ステータスを通常に戻す
                SendMessage(new HandleRef(progressBar1, progressBar1.Handle),
                    PBM_SETSTATE, PBST_NORMAL, IntPtr.Zero);

                SendMessage(new HandleRef(progressBar2, progressBar2.Handle),
                    PBM_SETSTATE, PBST_NORMAL, IntPtr.Zero);

                // 最大値を1増やして、戻す
                progressBar1.Maximum++;
                progressBar1.Maximum--;
                progressBar2.Maximum++;
                progressBar2.Maximum--;
            }
            

            // 戦闘開始メッセージ
            labelA.Text = "あ! 野生の" + up.GetName() + "が飛び出してきた! ▼";

        }


    }
}
