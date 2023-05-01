using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Data.SQLite;
using Telegram.Bot.Types.Enums;

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
                    new KeyboardButton("Вернуться в меню 🔙")
                }
            });
            var keyboard1 = new ReplyKeyboardMarkup(new[]
            {
                new[]
                {
                    new KeyboardButton("Игра 1 🎲"),
                    new KeyboardButton("Игра 2 🎰"),
                    new KeyboardButton("Игра 3 🏃‍"),
                    new KeyboardButton("Ознакомиться с правилами для игры 1 📖"),
                    new KeyboardButton("Ознакомиться с правилами для игры 2 📖"),
                    new KeyboardButton("Ознакомиться с правилами для игры 3 📖")
                },
                new[]
                {
                    new KeyboardButton("Просмотреть баланс 💳")
                }
            });

            
            switch (message.Text)
            {
                case "/start":
                case "Вернуться в меню 🔙":

                    // Добавляем пользователя в таблицу с начальным значением "money = 1000"
                    SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=mydatabase.db;Version=3;");
                    m_dbConnection.Open();
                    string sql_insert_user = "INSERT INTO table_name (name, money) VALUES ('" + message.From.Username + "', 1000)";
                    SQLiteCommand insert_user_cmd = new SQLiteCommand(sql_insert_user, m_dbConnection);
                    insert_user_cmd.ExecuteNonQuery();
                    m_dbConnection.Close();

                    await botClient.SendTextMessageAsync(message.Chat.Id, "Добро пожаловать в мир азарта и развлечений! 🎲🎰🃏 Сегодня у нас уникальный шанс попытать удачу и выиграть крупный джекпот! 💰💵💸", replyMarkup: keyboard1);
                    break;

                case "Ознакомиться с правилами для игры 1 📖":

                    await botClient.SendTextMessageAsync(message.Chat.Id, "rule 1");
                    break;
                case "Ознакомиться с правилами для игры 2 📖":

                    await botClient.SendTextMessageAsync(message.Chat.Id, "rule 2");
                    break;
                case "Ознакомиться с правилами для игры 3 📖":

                    await botClient.SendTextMessageAsync(message.Chat.Id, "rule 3");
                    break;
                case "Просмотреть баланс 💳":
                    // Получаем баланс пользователя из БД
                    SQLiteConnection m_dbConnection2 = new SQLiteConnection("Data Source=mydatabase.db;Version=3;");
                    m_dbConnection2.Open();
                    string sql_select_user = "SELECT money FROM table_name WHERE name='" + message.From.Username + "'";
                    SQLiteCommand select_user_cmd = new SQLiteCommand(sql_select_user, m_dbConnection2);
                    var result = select_user_cmd.ExecuteScalar();
                    m_dbConnection2.Close();

                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Ваш баланс: {result} руб.", replyMarkup: keyboard1);
                    break;

                case "Игра 1 🎲":
                    Ruletka(botClient,message);
                    break;
                case "Игра 2 🎰":
                    Kazino(botClient, message);
                    break;
                case "Игра 3 🏃‍":
                    RunGame(botClient, message);
                    break;
            }   
        }

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            Console.WriteLine(arg2);
            return Task.CompletedTask;
        }
        private static async Task Ruletka(ITelegramBotClient botClient, Message message)
        {
            // Получаем баланс пользователя из БД
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=mydatabase.db;Version=3;");
            m_dbConnection.Open();
            string sql_select_user = "SELECT money FROM table_name WHERE name='" + message.From.Username + "'";
            SQLiteCommand select_user_cmd = new SQLiteCommand(sql_select_user, m_dbConnection);
            var result = select_user_cmd.ExecuteScalar();

            // Если у пользователя недостаточно средств, отправляем сообщение о нехватке средств и завершаем функцию
            if ((int)result < 100)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "К сожалению, на вашем балансе недостаточно средств для участия в игре. 💸🙁");
                m_dbConnection.Close();
                return;
            }

            // Обновляем баланс пользователя в БД
            int new_balance = (int)result - 100;
            string sql_update_user = "UPDATE table_name SET money=" + new_balance + " WHERE name='" + message.From.Username + "'";
            SQLiteCommand update_user_cmd = new SQLiteCommand(sql_update_user, m_dbConnection);
            update_user_cmd.ExecuteNonQuery();
            m_dbConnection.Close();

            Random random = new Random();
            int rand_num = random.Next(1, 7);

            if (rand_num == 1)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Выпало число {rand_num}. Поздравляем, вы выиграли! 🥳 Вам начислено 600 руб. 💰");
                new_balance += 600;
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Выпало число {rand_num}. К сожалению, вы проиграли. 😔");
            }

            // Обновляем баланс пользователя в БД
            m_dbConnection.Open();
            sql_update_user = "UPDATE table_name SET money=" + new_balance + " WHERE name='" + message.From.Username + "'";
            update_user_cmd = new SQLiteCommand(sql_update_user, m_dbConnection);
            update_user_cmd.ExecuteNonQuery();
            m_dbConnection.Close();
            return;
        }


        private static async Task Kazino(ITelegramBotClient botClient, Message message)
        {
            // Получаем баланс пользователя из БД
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=mydatabase.db;Version=3;");
            m_dbConnection.Open();
            string sql_select_user = "SELECT money FROM table_name WHERE name='" + message.From.Username + "'";
            SQLiteCommand select_user_cmd = new SQLiteCommand(sql_select_user, m_dbConnection);
            var result = select_user_cmd.ExecuteScalar();

            // Если у пользователя недостаточно средств, отправляем сообщение о нехватке средств и завершаем функцию
            if ((int)result < 100)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "К сожалению, на вашем балансе недостаточно средств для участия в игре. 💸🙁");
                m_dbConnection.Close();
                return;
            }

            // Обновляем баланс пользователя в БД
            int new_balance = (int)result - 100;
            string sql_update_user = "UPDATE table_name SET money=" + new_balance + " WHERE name='" + message.From.Username + "'";
            SQLiteCommand update_user_cmd = new SQLiteCommand(sql_update_user, m_dbConnection);
            update_user_cmd.ExecuteNonQuery();

            Random random = new Random();
            int a = random.Next(1, 4);
            int b = random.Next(1, 4);
            int c = random.Next(1, 4);

            if (a == b && b == c)
            {
                new_balance += 900;
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Ура! Вы выиграли джекпот 🤑🎉\n Числа {a}, {b}, {c}. Вам начислено 900 руб.\n Баланс: {new_balance} руб. 💰");
            }
            else if (a == b || b == c || a == c)
            {
                new_balance +=128;
                int equalNumber = a == b ? a : (b == c ? b : c);
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Вы выиграли 🎊\n Дважды выпало число { equalNumber}. Числа {a}, {b}, {c}.\n Вам начислено 128 руб. Баланс: {new_balance} руб. 😁");
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"К сожалению, Вы проиграли 🙁\n Числа {a}, {b}, {c}\n Но не расстраивайтесь, в следующий раз обязательно повезет! Баланс: {new_balance} руб. 💸");
            }


            // Обновляем баланс пользователя в БД
            sql_update_user = "UPDATE table_name SET money=" + new_balance + " WHERE name='" + message.From.Username + "'";
            update_user_cmd = new SQLiteCommand(sql_update_user, m_dbConnection);
            update_user_cmd.ExecuteNonQuery();
            m_dbConnection.Close();
        }

        private static async Task RunGame(ITelegramBotClient botClient, Message message)
        {
            // Получаем баланс пользователя из БД
            SQLiteConnection connection = new SQLiteConnection("Data Source=mydatabase.db;Version=3;");
            connection.Open();
            string sqlSelectUser = $"SELECT money FROM table_name WHERE name='{message.From.Username}'";
            SQLiteCommand selectUserCmd = new SQLiteCommand(sqlSelectUser, connection);
            int balance = Convert.ToInt32(selectUserCmd.ExecuteScalar());
            if (balance < 100)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "К сожалению, на вашем балансе недостаточно средств для участия в игре. 💸🙁");
                connection.Close();
                return;
            }

            // Позиция человека
            int position = 0;
            Random random = new Random();
            // Дистанция, которую нужно пробежать
            int distance = random.Next(1, 11);
            // Отправляем сообщение с правилами игры и текущим балансом
            await botClient.SendTextMessageAsync(message.Chat.Id,
                $"Добро пожаловать в игру \"Бегущий человек\"! 🏃‍♂️ \nЦель игры - пробежать  и заработать как можно больше денег. \nВаш текущий баланс: {balance} руб 💰💰💰.");
            balance -= 100;
            // Флаг для проверки, закончена ли игра
            bool gameOver = false;

            // Игровой цикл
            while (!gameOver)
            {
                // Человек делает шаг
                position++;

                // Если человек достиг дистанции, игра закончена
                if (position == distance)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        $"Поздравляем! Вы пробежали дистанцию в {position} метров  ");

                    // Обновляем баланс пользователя в БД

                    string sqlUpdateUser =
                        $"UPDATE table_name SET money={balance} WHERE name='{message.From.Username}'";
                    SQLiteCommand updateUserCmd = new SQLiteCommand(sqlUpdateUser, connection);
                    updateUserCmd.ExecuteNonQuery();

                    gameOver = true;
                }
                else
                {
                    // Генерируем случайное число, которое определяет, сколько денег получит пользователь за текущий шаг

                    int money = random.Next(10, 50);

                    // Обновляем баланс пользователя
                    balance += money;

                    // Отправляем сообщение с результатами текущего шага
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        $"Вы прошли {position} метров и заработали {money} руб. 💰\nВаш текущий баланс: {balance} руб.");

                    // Обновляем баланс пользователя в БД
                    string sqlUpdateUser =
                        $"UPDATE table_name SET money={balance} WHERE name='{message.From.Username}'";
                    SQLiteCommand updateUserCmd = new SQLiteCommand(sqlUpdateUser, connection);
                    updateUserCmd.ExecuteNonQuery();

                    // Если у пользователя закончился баланс, игра закончена
                    if (balance <= 0)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id,
                            "К сожалению, у вас закончились деньги. Игра окончена. 💸");
                        gameOver = true;
                    }
                    else
                    {
                        // Ждем 2 секунды перед следующим шагом
                        await Task.Delay(2000);
                    }
                }
            }

            connection.Close();

        }




    }
}
