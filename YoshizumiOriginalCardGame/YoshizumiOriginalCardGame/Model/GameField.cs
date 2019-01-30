using System.Collections.Generic;
using System.Linq;
using CardLibrary.Models;
using YoshizumiOriginalCardGame.Model.Player;

namespace YoshizumiOriginalCardGame.Model
{
	public class GameField
	{
		public GameField()
		{
			this.Deck = new Deck();
			this.m_cemetery = new List<Card>();

			this.Initialize();
		}

		#region fields and properties

		/// <summary>
		/// 山札
		/// </summary>
		public Deck Deck { get; private set; }

		/// <summary>
		/// 場
		/// </summary>
		public Card FieldCard { get; set; }

		/// <summary>
		/// 流れた場
		/// </summary>
		private readonly IList<Card> m_cemetery;

		private GamePlayerBase m_passStartPlayer;

		#endregion

		#region methods

		/// <summary>
		/// 初期化
		/// </summary>
		private void Initialize()
		{
			this.PutFieldDeckCard();
		}

		/// <summary>
		/// 山札のカードを場に置く
		/// </summary>
		private void PutFieldDeckCard()
		{
			if (!this.Deck.Cards.Any())
			{
				return;
			}

			var card = this.Deck.Cards.FirstOrDefault();
			this.FieldCard = card;
			this.Deck.Cards.Remove(card);
		}

		/// <summary>
		/// 場のカードを変更
		/// </summary>
		private void ChangeField()
		{
			if (!this.Deck.Cards.Any())
			{
				this.Refresh();
			}

			this.m_cemetery.Add(this.FieldCard);
			this.PutFieldDeckCard();
		}

		/// <summary>
		/// 墓地をシャッフルして山札にする
		/// </summary>
		private void Refresh()
		{
			// ないはずだが一応クリア
			this.Deck.Cards.Clear();
			this.Deck.Cards.AddRange(this.m_cemetery);
			this.Deck.Shuffle();
			this.m_cemetery.Clear();
		}

		/// <summary>
		/// 手札を出すことができるか
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public bool CanPutFieldHandsCard(Card card)
		{
			return this.CheckSuitRule(card) || this.CheckNumRule(card);
		}

		/// <summary>
		/// 場にカードを置く
		/// </summary>
		/// <param name="card"></param>
		public void PutCard(Card card)
		{
			this.m_cemetery.Add(this.FieldCard);
			this.FieldCard = card;
			this.m_passStartPlayer = null;
		}

		/// <summary>
		/// Suitが正しいか判断
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		private bool CheckSuitRule(Card card)
		{
			return this.FieldCard.Suit == card.Suit;
		}

		/// <summary>
		/// 数字のルールに適応してるか
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		private bool CheckNumRule(Card card)
		{
			// 数字が場の+-1以内か、13と1の関係
			return this.FieldCard.Number == card.Number - 1
						 || this.FieldCard.Number == card.Number
						 || this.FieldCard.Number == card.Number + 1
						 || (this.FieldCard.IsKing && card.IsAce)
						 || (this.FieldCard.IsAce && card.IsKing);
		}

		/// <summary>
		/// PassPlayerが存在していない場合挿入
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public void SetFirstPassPlayer(GamePlayerBase player)
		{
			if (this.m_passStartPlayer == null)
			{
				this.m_passStartPlayer = player;
			}
		}

		/// <summary>
		/// 全員がパスしたとき、場のカードを変える
		/// </summary>
		/// <param name="player"></param>
		public bool PassHandle(GamePlayerBase player)
		{
			if (this.m_passStartPlayer == null)
			{
				this.m_passStartPlayer = player;
				return false;
			}

			if (this.m_passStartPlayer != player)
			{
				return false;
			}

			this.ChangeField();
			this.m_passStartPlayer = null;
			return true;
		}

		#endregion
	}
}
