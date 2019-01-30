using System.Linq;
using CardLibrary.Models;

namespace YoshizumiOriginalCardGame.Model.Player
{
	public class Opponent : GamePlayerBase
	{
		public Opponent(string playerName) : base(playerName)
		{
		}

		/// <summary>
		/// 出せるカードがあるか
		/// </summary>
		/// <param name="fieldCard"></param>
		/// <returns></returns>
		public Card AvailableCard(GameField fieldCard)
		{
			return this.Hands.FirstOrDefault(x => fieldCard.CanPutFieldHandsCard(x));
		}
	}
}
