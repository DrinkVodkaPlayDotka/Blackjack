using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace teleg
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var client = new TelegramBotClient("6198418939:AAHy8cobgalR-A4pNLSfLNjBNHg6aBOwnv4");
            client.StartReceiving(Update, Error);
            await Task.Delay(-1);
        }

        private static async Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;

            if (message?.Text == null) return;
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[]
                {
                    new KeyboardButton("Вернуться в главное меню")
                }
            });
            var keyboard1 = new ReplyKeyboardMarkup(new[]
            {
                new[]
                {
                    new KeyboardButton("Игра1"),
                    new KeyboardButton("Игра2"),
                    new KeyboardButton("Правила для игры1"),
                    new KeyboardButton("Правила для игры2")
                }
            });
            
            switch (message.Text)
            {
                case "/start":
                case "Вернуться в главное меню":

                    await botClient.SendTextMessageAsync(message.Chat.Id, "Добрый день,хотите проиграть свою машину ?", replyMarkup: keyboard1);
                    break;

                case "Правила для игры2":

                    await botClient.SendTextMessageAsync(message.Chat.Id, "Слоты - это очень простая игра, где нужно только смотреть!\r\n\r\nЧто может вам выпасть:\r\n\r\n- \U0001F365\U0001F365\U0001F365 = (Ваши деньги)*2\r\n\r\n- \U0001F366\U0001F366\U0001F366  = (Ваши деньги)*3", replyMarkup: keyboard);
                    break;
                case "Правила для игры1":
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Блэкджек - это карточная игра, в которой игроки играют против дилера, пытаясь набрать карты, сумма очков которых равна или близка к 21, но не больше.\r\n\r\nОсновные правила блэкджека:\r\n\r\n- Каждый игрок ставит ставку и получает по две карты, дилер получает одну карту.\r\n- Карты с номиналом от 2 до 10 имеют такую же ценность, туз может считаться как 1 или 11, а карты с лицами (король, дама, валет) стоят по 10 очков.\r\n- Игроки могут взять дополнительные карты, чтобы приблизиться к 21, но не более 21.\r\n- Если у игрока больше 21 очка, он проигрывает.\r\n- После того, как все игроки взяли дополнительные карты, дилер получает свои карты. Дилер обязан брать дополнительные карты до тех пор, пока его очки не достигнут 17 и более очков, после чего он останавливается.\r\n- Если у дилера больше 21 очка, то все оставшиеся игроки выигрывают.\r\n- Если у игрока и дилера равное количество очков, то ставки возвращаются игрокам.\r\n- Если у игрока больше очков, чем у дилера, и он не набрал больше 21, то он выигрывает, и его ставка удваивается.\r\n- Если у игрока набрался блэкджек (21 очко с двумя картами), то он выигрывает 1,5 раза свою ставку.", replyMarkup: keyboard);
                    
                    break;
                
            }
        }

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            Console.WriteLine(arg2);
            return Task.CompletedTask;
        }
    }
}