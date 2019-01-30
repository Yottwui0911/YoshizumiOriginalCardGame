using System.Linq;
using CardLibrary.Models;

namespace YoshizumiOriginalCardGame.Model.Player
{
	public class GamePlayerBase : PlayerBase
	{
		public override string Name { get; }

		public GamePlayerBase(string playerName)
		{
			this.Name = playerName;
		}

		/// <summary>
		/// 手札を持っているかどうか
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public bool HasHand(Card card)
		{
			return this.Hands.Any(x => x.Number == card.Number && x.Suit == x.Suit);
		}

		/// <summary>
		/// 手札からカードを削除
		/// </summary>
		/// <param name="card"></param>
		public void RemoveHand(Card card)
		{
			var hand = this.Hands.FirstOrDefault(x => x.Number == card.Number && x.Suit == card.Suit);
			this.Hands.Remove(hand);
		}

		/// <summary>
		/// 山札からカードをドローする
		/// </summary>
		/// <param name="deck"></param>
		/// <param name="count"></param>
		public void Draw(Deck deck, int count)
		{
			for (var i = 0; i < count; i++)
			{
				var card = deck.Cards.FirstOrDefault();
				this.Hands.Add(card);
				deck.Cards.Remove(card);
			}
		}
	}
}
