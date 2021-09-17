using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/// <summary>
/// 技のクラスです
/// </summary>

namespace Pokemon
{
    public class Move
    {
        /* --- 定数 --- */

        // 正規表現パターン

        // 名前
        private const string regexName = @"名前:(.*?),";

        // タイプ
        private const string regexType = @"タイプ:(.*?),";

        // 物理/特殊
        private const string regexPhysics = @"物/特:(.*?),";

        // 威力
        private const string regexPower = @"威力:(.*?),";

        // 命中率
        private const string regexAccuracy = @"命中率:(.*?),";

        // 説明文
        private const string regexDescription = @"説明:(.*?)";



        /* --- フィールド --- */


        // 名前
        private readonly string _name = string.Empty;

        // タイプ
        private readonly string _type = string.Empty;

        // 物理/特殊
        private readonly bool _isPhysics;

        // 威力
        private readonly int _power;

        // 命中率
        private readonly int _accuracy;

        // 説明文
        private readonly string _description = string.Empty;


        /* --- コンストラクタ --- */


        public Move(string moveData)
        {
            // 名前を抽出
            Match mName = Regex.Match(moveData, regexName);
            if (mName.Success)
                _name = mName.Groups[1].Value;

            // タイプを抽出
            Match mType = Regex.Match(moveData, regexType);
            if (mType.Success)
                _type = mType.Groups[1].Value;

            // 物理/特殊を抽出
            Match mPhysics = Regex.Match(moveData, regexPhysics);
            if (mPhysics.Success)
            {
                string temp = mPhysics.Groups[1].Value;
                if (temp.Equals("物理"))
                    _isPhysics = true;
                else
                    _isPhysics = false;
            }

            // 威力を抽出
            Match mPower = Regex.Match(moveData, regexPower);
            if (mPower.Success)
                _power = int.Parse(mPower.Groups[1].Value);

            // 命中率を抽出
            Match mAccuracy = Regex.Match(moveData, regexAccuracy);
            if (mAccuracy.Success)
                _accuracy = int.Parse(mAccuracy.Groups[1].Value);

            // 説明文を抽出
            Match mDescription = Regex.Match(moveData, regexDescription);
            if (mDescription.Success)
                _description = mDescription.Groups[1].Value;

        }


        /* --- メソッド --- */

        // ゲッタ

        // 名前のゲッタ
        public string GetName()
        {
            return _name;
        }

        // タイプのゲッタ
        public new string GetType()
        {
            return _type.Trim();
        }

        // 物理/特殊のゲッタ
        public bool GetPhysics()
        {
            return _isPhysics;
        }

        // 威力のゲッタ
        public int GetPower()
        {
            return _power;
        }

        // 命中率のゲッタ
        public int GetAccuracy()
        {
            return _accuracy;
        }

        // 説明文のゲッタ
        public string GetDescription()
        {
            return _description;
        }

        // ToString()のオーバーライド
        public override string ToString()
        {
            String physics;
            if (_isPhysics)
            {
                physics = "物理";
            }
            else
            {
                physics = "特殊";
            }

            return _name + ", " + "タイプ:" + _type + ", " + physics + " ," + "威力:" + _power + ", " + "命中:" + _accuracy;
        }

    }
}
