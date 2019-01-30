using System;
using System.Text;
using YoshizumiOriginalCardGame.Model;

namespace YoshizumiOriginalCardGame
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var controller = new GameController();
			Console.OutputEncoding = Encoding.UTF8;
			Console.WriteLine("ようこそ。吉住の部屋へ");
			Console.WriteLine(controller.Input(GameController.InputCommands.Help));
			Console.WriteLine(controller.Input(GameController.InputCommands.Field));
			Console.WriteLine(controller.Input(GameController.InputCommands.Hand));

			while (!controller.IsGameEnd)
			{
				Console.WriteLine("コマンドを入力してください");
				var input = Console.ReadLine();
				Console.WriteLine(controller.Input(input));
			}

			Console.WriteLine(controller.CreateRankingStr());

			Console.ReadKey();
		}
	}
}
