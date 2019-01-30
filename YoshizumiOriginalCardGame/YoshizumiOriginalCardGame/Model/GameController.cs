using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardLibrary.Models;
using YoshizumiOriginalCardGame.Model.Player;

namespace YoshizumiOriginalCardGame.Model
{
	public class GameController
	{
		public static class InputCommands
		{
			public static readonly string Help = "help";
			public static readonly string Hand = "hand";
			public static readonly string Field = "field";
			public static readonly string Put = "put";
			public static readonly string Pass = "pass";
		}

		public static class SuitPattern
		{
			public static readonly string Spade = "♠";
			public static readonly string Heart = "♥";
			public static readonly string Diamond = "♦";
			public static readonly string Club = "♣";
		}

		private readonly IDictionary<Suit, string> m_cardMap = new Dictionary<Suit, string>
				{
						{ Suit.Spade , SuitPattern.Spade},
						{ Suit.Heart , SuitPattern.Heart},
						{ Suit.Diamond , SuitPattern.Diamond},
						{ Suit.Club , SuitPattern.Club},
				};


		private readonly IDictionary<char, Suit> m_inputCardMap = new Dictionary<char, Suit>
				{
						{ 's', Suit.Spade},
						{ 'h', Suit.Heart},
						{ 'd', Suit.Diamond},
						{ 'c', Suit.Club},
				};

		public GameController()
		{
			this.m_user = new User();
			this.m_opponent1 = new Opponent("Player1");
			this.m_opponent2 = new Opponent("Player2");
			this.m_field = new GameField();

			this.m_playerMap = new Dictionary<int, Func<GamePlayerBase>>
						{
								{ 0, ()=> this.m_user },
								{ 1, ()=> this.m_opponent1 },
								{ 2, ()=> this.m_opponent2 },
						};

			this.m_controller = new Dictionary<string, Func<string>>
						{
								{InputCommands.Help, DoHelp },
								{InputCommands.Hand, this.DoHand },
								{InputCommands.Field, this.DoField },
								{InputCommands.Put, this.DoPut },
								{InputCommands.Pass, this.DoPass },
						};

			this.Initialize();

			this.m_turn = 0;
		}

		#region fields and properties

		/// <summary>
		/// 初期の手札の枚数
		/// </summary>
		private static readonly int m_initialHandsNum = 7;

		/// <summary>
		/// 参加人数
		/// </summary>
		private int GamePlayerNum => this.m_playerMap.Count;

		/// <summary>
		/// 現在のターン番号
		/// </summary>
		private int m_turn;

		/// <summary>
		/// ターンプレイヤーは誰か
		/// </summary>
		public GamePlayerBase TurnPlayer
		{
			get
			{
				if (this.GamePlayerNum <= 0 || !this.m_playerMap.ContainsKey(this.m_turn % this.GamePlayerNum))
				{
					return null;
				}

				return this.m_playerMap[this.m_turn % this.GamePlayerNum]();
			}
		}

		/// <summary>
		/// プレイヤー
		/// </summary>
		private GamePlayerBase m_user;

		/// <summary>
		/// 敵1
		/// </summary>
		private GamePlayerBase m_opponent1;

		/// <summary>
		/// 敵2
		/// </summary>
		private GamePlayerBase m_opponent2;

		/// <summary>
		/// 場
		/// </summary>
		private GameField m_field;

		/// <summary>
		/// プレイヤーの数と同じはずのマップ
		/// </summary>
		private readonly IDictionary<int, Func<GamePlayerBase>> m_playerMap;

		/// <summary>
		/// Ranking
		/// </summary>
		public IList<int> Ranking { get; set; } = new List<int>();

		/// <summary>
		/// ゲームが終了したかどうか
		/// </summary>
		public bool IsGameEnd { get; set; }

		private readonly IDictionary<string, Func<string>> m_controller;

		#endregion

		#region Messages

		/// <summary>
		/// 許可されていないコマンドのメッセージ
		/// </summary>
		private static readonly string UnauthorizedMsg = "許されていないコマンドが入力されました。\nhelpを入力して、使い方を確認してください。\n";

		/// <summary>
		/// ゲーム終了時のメッセージ
		/// </summary>
		private static readonly string EndMsg = "ゲームが終了しました\n";

		/// <summary>
		/// 自分の番を告げるメッセージ
		/// </summary>
		private string YourTurnMsg()
		{
			var str = "あなたの番です\n";

			return str + this.DoField() + this.DoHand();
		}

		/// <summary>
		/// カードの柄
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		private string CreateCardStr(Card card)
		{
			return $"{this.m_cardMap[card.Suit]}の{card.Number}";
		}

		/// <summary>
		/// Playerが上がった時の文字
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		private string CreateGoalStr(GamePlayerBase player)
		{
			return $"{player.Name}が上がりました\n";
		}

		/// <summary>
		/// パス時のメッセージ
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		private string CreatePassStr(GamePlayerBase player)
		{
			return $"{player.Name}がパスしました";
		}

		/// <summary>
		/// 勝者の文字列作成
		/// </summary>
		/// <returns></returns>
		public string CreateRankingStr()
		{
			var str = new StringBuilder("===結果===\n");
			var list = this.Ranking.ToList();
			for (var i = 0; i < list.Count; i++)
			{
				str.Append($"{i + 1}位：{m_playerMap[list[i]]().Name}\n");
			}

			return str.ToString();
		}

		#endregion

		#region methods

		#region InputOutput

		/// <summary>
		/// 入力
		/// </summary>
		/// <param name="input"></param>
		public string Input(string input)
		{
			return !this.m_controller.ContainsKey(input)
					? UnauthorizedMsg
					: this.m_controller[input]();
		}

		/// <summary>
		/// Helpの表示
		/// </summary>
		/// <returns></returns>
		private static string DoHelp()
		{
			return "■使い方\n" +
									 "以下のキーを入力してください。\n" +
									 $"・{InputCommands.Hand}:自分の手札を確認できます\n" +
									 $"・{InputCommands.Field}:場のカードを確認できます\n" +
									 $"・{InputCommands.Put}:手札からカードを出します\n　例：♦の4->d4,♠の1->s1,♣の12->c12,♥の9->h9\n" +
									 $"・{InputCommands.Pass}:パスして次の人に番を回します\n" +
									 $"・{InputCommands.Help}:Helpを確認できます\n";
		}

		/// <summary>
		/// handコマンド
		/// </summary>
		/// <returns></returns>
		private string DoHand()
		{
			var hands = this.GetUserHand();
			return "あなたの手札：\n" + string.Join(",", hands.Select(this.CreateCardStr)) + "\n";
		}

		/// <summary>
		/// fieldコマンド
		/// </summary>
		/// <returns></returns>
		private string DoField()
		{
			return $"場には{this.CreateCardStr(this.GetField())}が出ています\n";
		}

		/// <summary>
		/// putコマンド
		/// </summary>
		/// <returns></returns>
		private string DoPut()
		{
			Card card;

			while (true)
			{
				Console.WriteLine("何を出しますか？");
				var input = Console.ReadLine();
				card = this.CheckFormat(input);
				if (card != null)
				{
					break;
				}

				Console.WriteLine("正しいフォーマットで入力してください");
			}

			var errors = new List<string>();

			if (!this.PutUserHand(card, errors))
			{
				return string.Join("\n", errors);
			}

			if (this.IsGameEnd)
			{
				return EndMsg;
			}

			this.DoAllOppnentTurn();

			if (this.IsGameEnd)
			{
				return EndMsg;
			}

			return this.YourTurnMsg();
		}

		/// <summary>
		/// passコマンド
		/// </summary>
		/// <returns></returns>
		private string DoPass()
		{
			this.DoPassPlayer(this.m_user);

			this.DoAllOppnentTurn();

			if (this.IsGameEnd)
			{
				return EndMsg;
			}

			return this.YourTurnMsg();
		}

		/// <summary>
		/// パスの処理
		/// </summary>
		/// <param name="player"></param>
		private void DoPassPlayer(GamePlayerBase player)
		{
			Console.WriteLine(this.CreatePassStr(player));

			this.m_field.SetFirstPassPlayer(this.TurnPlayer);

			this.NextTurn();

			if (this.m_field.PassHandle(this.TurnPlayer))
			{
				Console.WriteLine("場が流れました");
				Console.WriteLine(this.DoField());
			}
		}

		#endregion

		/// <summary>
		/// 初期化処理
		/// </summary>
		private void Initialize()
		{
			this.FirstDistribute();
		}

		/// <summary>
		/// 初手を配る
		/// </summary>
		private void FirstDistribute()
		{
			// 初期手札枚数分配る
			for (var i = 0; i < m_initialHandsNum; i++)
			{
				// 順番に配る
				for (var j = 0; j < this.GamePlayerNum; j++)
				{
					this.m_playerMap[j % this.GamePlayerNum]().Draw(this.m_field.Deck, 1);
				}
			}
		}

		/// <summary>
		/// プレイヤーの手札を公開
		/// </summary>
		/// <returns></returns>
		private IEnumerable<Card> GetUserHand()
		{
			return this.m_user.Hands;
		}

		/// <summary>
		/// 場のカードを公開
		/// </summary>
		/// <returns></returns>
		private Card GetField()
		{
			return this.m_field.FieldCard;
		}

		/// <summary>
		/// 手番を次に回す
		/// </summary>
		private void NextTurn()
		{
			// 手札がない場合、勝者としてすでにカウントされてるか確認
			if (!this.TurnPlayer.Hands.Any() && !this.Ranking.Contains(this.m_turn))
			{
				Console.WriteLine(CreateGoalStr(this.TurnPlayer));
				this.Ranking.Add(this.m_turn);
			}

			// プレイヤーが1人以下になると終了
			if (this.m_playerMap.Count(x => x.Value().Hands.Any()) < 2)
			{
				// 残りの一人をランキングに混ぜる
				this.Ranking.Add(this.m_playerMap.FirstOrDefault(x => !this.Ranking.Contains(x.Key)).Key);
				this.IsGameEnd = true;
				return;
			}

			this.m_turn = (this.m_turn + 1) % this.GamePlayerNum;

			// 次のターンプレイヤーの手札もなければさらにターンを送る
			if (!this.TurnPlayer.Hands.Any())
			{
				this.NextTurn();
				return;
			}
		}

		private bool PutUserHand(Card card, IList<string> errors)
		{
			if (!this.m_user.HasHand(card))
			{
				errors.Add($"{CreateCardStr(card)}は手札にありません");
				return false;
			}

			if (!this.m_field.CanPutFieldHandsCard(card))
			{
				errors.Add($"{CreateCardStr(card)}は場に出せません");
				return false;
			}

			this.PutCard(this.m_user, card);

			return true;
		}

		private void PutCard(GamePlayerBase player, Card card)
		{
			this.m_field.PutCard(card);
			player.RemoveHand(card);

			Console.WriteLine($"{player.Name}が{CreateCardStr(card)}を場に置きました");
			this.NextTurn();
		}

		/// <summary>
		/// 入力のフォーマットチェック
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		private Card CheckFormat(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return null;
			}
			var suit = input[0];

			// Suitがフォーマット通りか
			if (!m_inputCardMap.ContainsKey(suit))
			{
				return null;
			}

			var numStr = input.Replace(suit.ToString(), "");

			// 数字が1～13の間
			if (!int.TryParse(numStr, out int num) || !(0 < num && num < 14))
			{
				return null;
			}

			return new Card(num, m_inputCardMap[suit]);
		}

		#region Opponent Action

		/// <summary>
		/// すべての敵にターンを回す
		/// </summary>
		private void DoAllOppnentTurn()
		{
			while (!this.IsGameEnd && this.TurnPlayer != this.m_user)
			{
				this.DoOpponentTurn(this.TurnPlayer as Opponent);
			}
		}

		/// <summary>
		/// 敵の行動パターン
		/// </summary>
		/// <param name="opponent"></param>
		private void DoOpponentTurn(Opponent opponent)
		{
			if (opponent == null)
			{
				return;
			}

			var card = opponent.AvailableCard(this.m_field);
			if (card == null)
			{
				this.DoPassPlayer(opponent);
				return;
			}

			this.PutCard(opponent, card);
		}

		#endregion

		#endregion
	}
}
