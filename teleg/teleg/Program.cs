using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace teleg
{
  internal class Program
  {
    public static void Main(string[] args)
    {
      var client = new TelegramBotClient("6198418939:AAHy8cobgalR-A4pNLSfLNjBNHg6aBOwnv4");
      client.StartReceiving(Update,Error);
      Console.ReadKey();
    }

    async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
      var message = update.Message;
      if (message.Text != null)
      {
        if (message.Text.ToLower().Contains("здорово"))
        {
          await botClient.SendTextMessageAsync(message.Chat.Id,"Здоровей видали");
          return;
        }
      }
    }

    private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
    {
      throw new NotImplementedException();
    }
  }
}