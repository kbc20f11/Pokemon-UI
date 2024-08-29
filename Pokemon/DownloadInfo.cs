using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium.Chrome;


/// <summary>
/// ポケモンの情報をダウンロードしてくるクラスです
/// </summary>

namespace Pokemon
{
    public class DownloadInfo
    {
        /* --- 定数 --- */

        // UserAgents
        private const string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36";

        // URLs
        private const string pokemonInfoUrl = "https://yakkun.com/swsh/zukan/n<pokemonId>";
        private const string pokemonImageUrl = "https://78npc3br.user.webaccel.jp/poke/icon96/n<pokemonId>.gif";

        // エンコーディング
        private const string encoding = "EUC-JP";

        // 正規表現パターン
        private const string regexName = @"<title>(.*?)｜ポケモン図鑑(.*?)｜ポケモン徹底攻略</title>";
        private const string regexType = @"<tr class=""center""><td class=""c1"">タイプ</td><td><ul class=""type"">((?:<li><a href="".*?""><img src="".*?"" alt=""(.*?)""></a></li>)+)</ul></td></tr>";
        private const string regexHp = @"HP</td>(.*?)&nbsp;(\d+)<span (.*?)</span></td></tr>";
        private const string regexAttack = @"こうげき</td>(.*?)&nbsp;(\d+)<span (.*?)</span></td></tr>";
        private const string regexDefence = @"ぼうぎょ</td>(.*?)&nbsp;(\d+)<span (.*?)</span></td></tr>";
        private const string regexSAttack = @"とくこう</td>(.*?)&nbsp;(\d+)<span (.*?)</span></td></tr>";
        private const string regexSDefence = @"とくぼう</td>(.*?)&nbsp;(\d+)<span (.*?)</span></td></tr>";
        private const string regexSpeed = @"すばやさ</td>(.*?)&nbsp;(\d+)<span (.*?)</span></td></tr>";
        private const string regexMove = @"data-power=""(\d+)""><td class=""move_condition_cell"">(.*?)</td><td colspan=""7"" class=""move_name_cell""><a href=""\./search/\?move=(\d+)"">(.*?)</a>(.*?)</td></tr>(\s*?)<tr class=""move_detail_row""><td><span class=""type (.*?)"">(.*?)</span></td><td class=""(.*?)""><span class=""(.*?)"">(.*?)</span></td>(.*?)<td>(\d+)</td>(.*?)</td><td>(\d+)</td><td>(\d+)</td><td>(.*?)</td><td class=""move_ex_cell"">(.*?)</td></tr>";
        private const string regexMove2 = @"data-power=""(\d+)"">(.*?)</td><td colspan=""7"" class=""move_name_cell""><a href=""\./search/\?move=(\d+)"">(.*?)</a></td></tr>(\s*?)<tr class=""move_detail_row""><td><span class=""type(.*?)"">(.*?)</span></td><td class=""(.*?)""><span class=""(.*?)"">(.*?)</span></td><td>(\d+)</td><td>(\d+)</td><td>(\d+)</td><td>(.*?)</td><td class=""move_ex_cell"">(.*?)</td></tr>";


        /* --- フィールド --- */

        private readonly string _pokemonId = string.Empty;
        private readonly string _name = string.Empty;
        private readonly string _type = string.Empty;
        private readonly string _type2 = string.Empty;
        private readonly string _hp = string.Empty;
        private readonly string _attack = string.Empty;
        private readonly string _defence = string.Empty;
        private readonly string _sAttack = string.Empty;
        private readonly string _sDefence = string.Empty;
        private readonly string _speed = string.Empty;
        private readonly string _move1 = string.Empty;
        private readonly string _move2 = string.Empty;
        private readonly string _move3 = string.Empty;
        private readonly string _move4 = string.Empty;


        /* --- コンストラクタ --- */

        public DownloadInfo(int pokemonId)
        {
            // ポケモンIDをフィールドに代入
            _pokemonId = pokemonId.ToString();

            // ダウンロードした文字列を格納するローカル変数htmlを用意
            string html = string.Empty;

            // セレニウムを非表示で実行
            var options = new ChromeOptions();
            // ヘッドレスモードで実行
            //options.AddArgument("--headless");
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            options.AddArgument("--window-position=-32000,-32000");

            // インターネットからポケモンデータを取得
            using (var driver = new ChromeDriver(service, options))
            {
                try
                {
                    driver.Navigate().GoToUrl(GetPokemonUrl());
                    html = driver.PageSource;
                    Debug.WriteLine("あ" + html);


                }
                finally
                {
                    driver.Dispose();
                }
            }


            // パターンに一致したところをそれぞれ抜き出す

            // 名前
            Match mName = Regex.Match(html, regexName);
            if (mName.Success)
            _name = mName.Groups[1].Value;

            // タイプ
            var matches = Regex.Matches(html, regexType);

            foreach (Match match in matches)
            {
                var typeMatches = Regex.Matches(match.Groups[1].Value, @"alt=""(.*?)""");
                int index = 1;

                foreach (Match typeMatch in typeMatches)
                {
                    if (index == 1)
                    {
                        _type = typeMatch.Groups[1].Value;
                        index++;
                    }
                    else if (index == 2)
                    {
                        _type2 = typeMatch.Groups[1].Value;
                    }
                }
            }

            // HP
            Match mHp = Regex.Match(html, regexHp);
            if (mHp.Success)
                _hp = mHp.Groups[2].Value;

            // 攻撃
            Match mAttack = Regex.Match(html, regexAttack);
            if (mAttack.Success)
                _attack = mAttack.Groups[2].Value;

            // 防御
            Match mDefence = Regex.Match(html, regexDefence);
            if (mDefence.Success)
                _defence = mDefence.Groups[2].Value;

            // とくこう
            Match mSAttack = Regex.Match(html, regexSAttack);
            if (mSAttack.Success)
                _sAttack = mSAttack.Groups[2].Value;

            // とくぼう
            Match mSDefence = Regex.Match(html, regexSDefence);
            if (mSDefence.Success)
                _sDefence = mSDefence.Groups[2].Value;

            // 素早さ
            Match mSpeed = Regex.Match(html, regexSpeed);
            if (mSpeed.Success)
                _speed = mSpeed.Groups[2].Value;

            // 技
            // ポケモンのすべてのわざをスキャン
            MatchCollection mc = Regex.Matches(html, regexMove);

            // 技2
            // ポケモンのわざ2をスキャン
            MatchCollection mc2 = Regex.Matches(html, regexMove2);

            // stringbuilderを初期化
            StringBuilder sb = new StringBuilder();

            // 技の配列を用意
            var moveList  = new List<string>();


            // そのポケモンが持っているすべての技を配列に格納
            for (int i = 0; i< mc.Count; i++)
            {
                // ひとつづつappend
                sb.Append("名前:");
                sb.Append(mc[i].Groups[4].Value);
                sb.Append(", タイプ: ");
                sb.Append(mc[i].Groups[8].Value);
                sb.Append(", 物/特:");
                sb.Append(mc[i].Groups[11].Value);
                sb.Append(", 威力:");
                sb.Append(mc[i].Groups[13].Value);
                sb.Append(", 命中率:");
                sb.Append(mc[i].Groups[15].Value);
                sb.Append(", 説明:");
                sb.Append(mc[i].Groups[18].Value);

                // リストに格納
                moveList.Add(sb.ToString());

                // stringbuilderをクリア
                sb.Clear();

            }

            // そのポケモンが持っているすべての技2を配列に格納
            for (int i = 0; i < mc2.Count; i++)
            {
                // ひとつづつappend
                sb.Append("名前:");
                sb.Append(mc2[i].Groups[4].Value);
                sb.Append(", タイプ: ");
                sb.Append(mc2[i].Groups[7].Value);
                sb.Append(", 物/特:");
                sb.Append(mc2[i].Groups[10].Value);
                sb.Append(", 威力:");
                sb.Append(mc2[i].Groups[12].Value);
                sb.Append(", 命中率:");
                sb.Append(mc2[i].Groups[13].Value);
                sb.Append(", 説明:");
                sb.Append(mc2[i].Groups[15].Value);

                // リストに格納
                moveList.Add(sb.ToString());

                // stringbuilderをクリア
                sb.Clear();

            }


            // すべての技が4つない場合
            if (mc.Count + mc2.Count < 4)
                return;

            // すべての技が4つしかなくその中で重複がある場合
            if ((mc.Count + mc2.Count == 4) && (moveList[0].Equals(moveList[1]) || moveList[0].Equals(moveList[2]) || moveList[0].Equals(moveList[3]) || moveList[1].Equals(moveList[2]) || moveList[1].Equals(moveList[3]) || moveList[2].Equals(moveList[3])))
                return;

            // 技を乱数で選ぶ
            // Randomを初期化
            Random r = new Random();

            // 乱数を添え時に使い重複がないようにフィールドに格納
            do
            {
                _move1 = moveList[r.Next(0, moveList.Count)];
                _move2 = moveList[r.Next(0, moveList.Count)];
                _move3 = moveList[r.Next(0, moveList.Count)];
                _move4 = moveList[r.Next(0, moveList.Count)];
            } while (_move1.Equals(_move2) || _move1.Equals(_move3) || _move1.Equals(_move4) || _move2.Equals(_move3) || _move2.Equals(_move4) || _move3.Equals(_move4));

            moveList = null;

            sb = null;
        }



        /* --- メソッド --- */

        private String GetPokemonUrl()
        {
            return pokemonInfoUrl.Replace("<pokemonId>", _pokemonId);
        }
        public String GetPokemonIcon()
        {
            return pokemonImageUrl.Replace("<pokemonId>", _pokemonId);
        }

        public String GetPokemonId()
        {
            return _pokemonId;
        }

        public String GetName()
        {
            return _name;
        }

        public new String GetType()
        {
            Debug.WriteLine("あ" + _type);
            return _type;
        }

        public String GetType2()
        {
            return _type2;
        }


        public String GetHp()
        {
            return _hp;
        }

        public String GetAttack()
        {
            return _attack;
        }

        public String GetDefence()
        {
            return _defence;
        }

        public String GetSAttack()
        {
            return _sAttack;
        }

        public String GetSDefence()
        {
            return _sDefence;
        }

        public String GetSpeed()
        {
            return _speed;
        }

        public String GetMove1()
        {
            return _move1;
        }

        public String GetMove2()
        {
            return _move2;
        }

        public String GetMove3()
        {
            return _move3;
        }

        public String GetMove4()
        {
            return _move4;
        }





    }
}
