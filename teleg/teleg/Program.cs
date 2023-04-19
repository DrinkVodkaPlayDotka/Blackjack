using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Data.SQLite;

namespace teleg
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            // Создаем БД и таблицу при запуске программы
            SQLiteConnection.CreateFile("mydatabase.db");
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=mydatabase.db;Version=3;");
            m_dbConnection.Open();
            string sql_create_table = "CREATE TABLE IF NOT EXISTS table_name (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(50), money INT)";
            SQLiteCommand create_table_cmd = new SQLiteCommand(sql_create_table, m_dbConnection);
            create_table_cmd.ExecuteNonQuery();
            m_dbConnection.Close();

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
                },
                new[]
                {
                    new KeyboardButton("Посмотреть мой баланс")
                }
            });

            
            switch (message.Text)
            {
                case "/start":
                case "Вернуться в главное меню":

                    // Добавляем пользователя в таблицу с начальным значением "money = 1000"
                    SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=mydatabase.db;Version=3;");
                    m_dbConnection.Open();
                    string sql_insert_user = "INSERT INTO table_name (name, money) VALUES ('" + message.From.Username + "', 1000)";
                    SQLiteCommand insert_user_cmd = new SQLiteCommand(sql_insert_user, m_dbConnection);
                    insert_user_cmd.ExecuteNonQuery();
                    m_dbConnection.Close();

                    await botClient.SendTextMessageAsync(message.Chat.Id, "Добрый день,хотите проиграть свою машину ?", replyMarkup: keyboard1);
                    break;

                case "Правила для игры2":

                    await botClient.SendTextMessageAsync(message.Chat.Id, "rule 1");
                    break;
                case "Правила для игры1":

                    await botClient.SendTextMessageAsync(message.Chat.Id, "rule 2");
                    break;
                case "Посмотреть мой баланс":
                    // Получаем баланс пользователя из БД
                    SQLiteConnection m_dbConnection2 = new SQLiteConnection("Data Source=mydatabase.db;Version=3;");
                    m_dbConnection2.Open();
                    string sql_select_user = "SELECT money FROM table_name WHERE name='" + message.From.Username + "'";
                    SQLiteCommand select_user_cmd = new SQLiteCommand(sql_select_user, m_dbConnection2);
                    var result = select_user_cmd.ExecuteScalar();
                    m_dbConnection2.Close();

                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Ваш баланс: {result} руб.", replyMarkup: keyboard);
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
