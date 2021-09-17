using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon
{
	class DamageCalc {
		/* --- フィールド --- */

		// ダメージ
		private readonly int _damage;

		// 攻撃者の名前
		private readonly string _attackerName = string.Empty;

		// form1のインスタンスを格納するためのフィールド
		private readonly Form1 _form1Obj;

		/* --- コンストラクタ --- */

		public DamageCalc(Move m, Pokemon at, Pokemon df, Form1 Form1_Obj)
        {
			// 攻撃者の名前をフィールドに代入
			_attackerName = at.GetName();

			// フォーム1のインスタンスをフィールドに代入する
			_form1Obj = Form1_Obj;

			/* --- ダメージ計算 --- */

			/*
			 * ダメージ = 攻撃側のレベル × 2 ÷ 5 ＋ 2 → 切り捨て(Math.floor)
			 * × 物理技の威力 × 攻撃側のこうげき ÷ 防御側のぼうぎょ → 切り捨て(Math.floor)
			 * ÷ 50 ＋ 2 → 切り捨て(Math.floor)
			 * × 乱数(0.85, 0.86, …… ,0.99, 1.00 のいずれか) → 切り捨て(Math.floor)
			 * × タイプ一致わざ (1.5) → 五捨五超入(Math.roundで代用)
			 * × 急所 (1.5) → 五捨五超入(Math.roundで代用)
			 * × 防御側のポケモンとわざの相性  → 切り捨て(Math.floor)
			 * × 命中したかしないか(0か1) (いわゆる論理積?)
			 */

			// 計算で用いる一時的なローカル変数を宣言
			double tempDamage1;
			double tempDamage2;
			double tempDamage3;
			double tempDamage4;
			double tempDamage5;
			double tempDamage6;
			double tempDamage7;
			double cm;


			// 技が外れたらそこでリターンして、以降の処理を行わない
			if (AccuracyRate(m) == 0)  // miss
			{
				_damage = 0;
				return; // stop
			}

			// ---
			tempDamage1 = Math.Floor((double)((Setting.LEVEL * 2 / 5) + 2));

			// 技の種類が物理か特殊かで場合分け
			if (m.GetPhysics())
			{
				tempDamage2 = Math.Floor(tempDamage1 * m.GetPower() * at.GetAttack() / df.GetDefence());
			}
			else
			{
				tempDamage2 = Math.Floor(tempDamage1 * m.GetPower() * at.GetSpecialAttack() / df.GetSpecialDefence());
			}



			// ---
			tempDamage3 = Math.Floor(tempDamage2 / 50 + 2);

			// ---
			tempDamage4 = Math.Floor(tempDamage3 * Rand());

			// ---
			tempDamage5 = Math.Round(tempDamage4 * SameTypeAttackBonus(at, m));

			// ---
			tempDamage6 = Math.Round(tempDamage5 * CriticalMultiplier());

			// ---
			// 相性倍率を計算する

			// タイプが1つか2つあるかで場合分け
			if (string.IsNullOrEmpty(df.GetType2()))
			{
				cm = ChemistryMultiplier(m, df.GetType());
			}
			else
			{
				cm = ChemistryMultiplier(m, df.GetType()) * ChemistryMultiplier(m, df.GetType2());
			}


			// 効果判定
			if (cm >= 2)
			{
				_form1Obj.labelA.Text += "\n効果は抜群だ";
			}
			if (cm > 0 && cm <= 0.5)
			{
				_form1Obj.labelA.Text += "\n効果はいまひとつのようだ";
			}
			if (cm == 0.0d)
			{
				_form1Obj.labelA.Text += "\n効果はないようだ　▼";
			}

			// 結果的なダメージを四捨五入
			tempDamage7 = Math.Floor(tempDamage6 * cm);

			// フィールドに代入
			_damage = (int)(tempDamage7);


		}


		/* --- Methods --- */

		// 乱数ダメージを生成する
		public double Rand()
		{

			// 乱数ダメージの振れ幅は0～15
			Random r = new Random();
			double num = (double)r.Next(0, 16);

			// ---
			return num / 100 + 0.85;
		}

		// タイプ一致ボーナス
		public double SameTypeAttackBonus(Pokemon at, Move m)
		{

			// デフォルトの倍率
			double stab = 1.0d;

			// ポケモンが攻撃技と同じタイプを持っている場合
			// type1
			if (at.GetType().Equals(m.GetType()))
				stab = 1.5d;

			// 2つめのタイプ
			if (at.GetType2() != null)
			{
				if (at.GetType2().Equals(m.GetType()))
					stab = 1.5d;
			}

			// 倍率を返す
			return stab;
		}

		// 命中率
		public double AccuracyRate(Move m)
		{

			// 倍率を１か0かに変換
			Random r = new Random();
			if (r.Next(0, 100) + 1 > m.GetAccuracy())
			{
			    _form1Obj.labelA.Text += "\n" + _attackerName + "の攻撃は外れた　▼";
				return 0.0d;  // miss
			}
			else
			{
				return 1.0d;  // hit
			}

		}

		// 相性倍率
		public double ChemistryMultiplier(Move m, String dfType)
		{

			// デフォルト
			double multiplier = 1.0d; 

			// ノーマル
			if (m.GetType().Equals("ノーマル"))
			{
				if (dfType.Equals("いわ") || dfType.Equals("はがね"))
					multiplier = 0.5d;
				if (dfType.Equals("ゴースト"))
					multiplier = 0.0d;
			}

			// ほのお
			if (m.GetType().Equals("ほのお"))
			{
				if (dfType.Equals("くさ") || dfType.Equals("こおり") || dfType.Equals("むし") || dfType.Equals("はがね"))
					multiplier = 2.0d;
				if (dfType.Equals("ほのお") || dfType.Equals("みず") || dfType.Equals("いわ") || dfType.Equals("ドラゴン"))
					multiplier = 0.5d;
			}

			// みず
			if (m.GetType().Equals("みず"))
			{
				if (dfType.Equals("ほのお") || dfType.Equals("じめん") || dfType.Equals("いわ"))
					multiplier = 2.0d;
				if (dfType.Equals("みず") || dfType.Equals("くさ") || dfType.Equals("ドラゴン"))
					multiplier = 0.5d;
			}

			// でんき
			if (m.GetType().Equals("でんき"))
			{
				if (dfType.Equals("みず") || dfType.Equals("ひこう"))
					multiplier = 2.0d;
				if (dfType.Equals("でんき") || dfType.Equals("くさ") || dfType.Equals("ドラゴン"))
					multiplier = 0.5d;
				if (dfType.Equals("じめん"))
					multiplier = 0.0d;
			}

			// くさ
			if (m.GetType().Equals("くさ"))
			{
				if (dfType.Equals("みず") || dfType.Equals("じめん") || dfType.Equals("いわ"))
					multiplier = 2.0d;
				if (dfType.Equals("ほのお") || dfType.Equals("くさ") || dfType.Equals("どく") || dfType.Equals("ひこう") || dfType.Equals("むし") || dfType.Equals("ドラゴン") || dfType.Equals("はがね"))
					multiplier = 0.5d;
			}

			// こおり
			if (m.GetType().Equals("こおり"))
			{
				if (dfType.Equals("くさ") || dfType.Equals("じめん") || dfType.Equals("ひこう") || dfType.Equals("ドラゴン"))
					multiplier = 2.0d;
				if (dfType.Equals("ほのお") || dfType.Equals("みず") || dfType.Equals("こおり") || dfType.Equals("はがね"))
					multiplier = 0.5d;
			}

			// かくとう
			if (m.GetType().Equals("かくとう"))
			{
				if (dfType.Equals("ノーマル") || dfType.Equals("こおり") || dfType.Equals("いわ") || dfType.Equals("あく") || dfType.Equals("	はがね"))
					multiplier = 2.0d;
				if (dfType.Equals("どく") || dfType.Equals("ひこう") || dfType.Equals("エスパー") || dfType.Equals("むし") || dfType.Equals("フェアリー"))
					multiplier = 0.5d;
				if (dfType.Equals("ゴースト"))
				{
					multiplier = 0.0d;
				}
			}

			// どく
			if (m.GetType().Equals("どく"))
			{
				if (dfType.Equals("くさ") || dfType.Equals("フェアリー"))
					multiplier = 2.0d;
				if (dfType.Equals("どく") || dfType.Equals("じめん") || dfType.Equals("いわ") || dfType.Equals("ゴースト"))
					multiplier = 0.5d;
				if (dfType.Equals("はがね"))
					multiplier = 0.0d;
			}

			// じめん
			if (m.GetType().Equals("じめん"))
			{
				if (dfType.Equals("ほのお") || dfType.Equals("でんき") || dfType.Equals("どく") || dfType.Equals("いわ") || dfType.Equals("はがね"))
					multiplier = 2.0d;
				if (dfType.Equals("くさ") || dfType.Equals("むし"))
					multiplier = 0.5d;
				if (dfType.Equals("ひこう"))
					multiplier = 0.0d;
			}

			// ひこう
			if (m.GetType().Equals("ひこう"))
			{
				if (dfType.Equals("くさ") || dfType.Equals("かくとう") || dfType.Equals("むし"))
					multiplier = 2.0d;
				if (dfType.Equals("でんき") || dfType.Equals("いわ") || dfType.Equals("はがね"))
					multiplier = 0.5d;
			}

			// エスパー
			if (m.GetType().Equals("エスパー"))
			{
				if (dfType.Equals("かくとう") || dfType.Equals("どく"))
					multiplier = 2.0d;
				if (dfType.Equals("エスパー") || dfType.Equals("はがね"))
					multiplier = 0.5d;
				if (dfType.Equals("あく"))
					multiplier = 0.0d;
			}

			// むし
			if (m.GetType().Equals("むし"))
			{
				if (dfType.Equals("くさ") || dfType.Equals("エスパー") || dfType.Equals("あく"))
					multiplier = 2.0d;
				if (dfType.Equals("ほのお") || dfType.Equals("かくとう") || dfType.Equals("どく") || dfType.Equals("ひこう") || dfType.Equals("ゴースト") || dfType.Equals("はがね") || dfType.Equals("フェアリー"))
					multiplier = 0.5d;
			}

			// いわ
			if (m.GetType().Equals("いわ"))
			{
				if (dfType.Equals("ほのお") || dfType.Equals("こおり") || dfType.Equals("ひこう") || dfType.Equals("むし"))
					multiplier = 2.0d;
				if (dfType.Equals("かくとう") || dfType.Equals("じめん") || dfType.Equals("はがね"))
					multiplier = 0.5d;
			}

			// ゴースト
			if (m.GetType().Equals("ゴースト"))
			{
				if (dfType.Equals("エスパー") || dfType.Equals("ゴースト"))
					multiplier = 2.0d;
				if (dfType.Equals("あく"))
					multiplier = 0.5d;
				if (dfType.Equals("ノーマル"))
					multiplier = 0.0d;
			}

			// ドラゴン
			if (m.GetType().Equals("ドラゴン"))
			{
				if (dfType.Equals("ドラゴン"))
					multiplier = 2.0d;
				if (dfType.Equals("はがね"))
					multiplier = 0.5d;
				if (dfType.Equals("フェアリー"))
					multiplier = 0.0d;
			}

			// あく
			if (m.GetType().Equals("あく"))
			{
				if (dfType.Equals("エスパー") || dfType.Equals("ゴースト"))
					multiplier = 2.0d;
				if (dfType.Equals("かくとう") || dfType.Equals("あく") || dfType.Equals("フェアリー"))
					multiplier = 0.0d;
			}

			// はがね
			if (m.GetType().Equals("はがね"))
			{
				if (dfType.Equals("こおり") || dfType.Equals("いわ") || dfType.Equals("フェアリー"))
					multiplier = 2.0d;
				if (dfType.Equals("ほのお") || dfType.Equals("みず") || dfType.Equals("でんき") || dfType.Equals("はがね"))
					multiplier = 0.5d;
			}

			// フェアリー
			if (m.GetType().Equals("フェアリー"))
			{
				if (dfType.Equals("かくとう") || dfType.Equals("ドラゴン") || dfType.Equals("あく"))
					multiplier = 2.0d;
				if (dfType.Equals("ほのお") || dfType.Equals("どく") || dfType.Equals("はがね"))
					multiplier = 0.5d;
			}
			

			return multiplier;
		}

		// 急所倍率計算
		public double CriticalMultiplier()
		{

			// 急所に当たる処理 (1/24)
			Random r = new Random();
			int rand = r.Next(0, 24); 
			if (rand == 23)
			{
				_form1Obj.labelA.Text += "\n急所にあたった";
				return 1.5d;
			}
			else
			{
				return 1.0d;
			}

		}


		/* ゲッタ */

		// ダメージのゲッタ
		public int GetDamage()
		{
			return _damage;
		}
	}
}
