using System;
using System.Data.SQLite;
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
            // Создаем БД и таблицу при запуске программы
            SQLiteConnection.CreateFile("mydatabase.db");
            var m_dbConnection = new SQLiteConnection("Data Source=mydatabase.db;Version=3;");
            m_dbConnection.Open();
            var sql_create_table =
                "CREATE TABLE IF NOT EXISTS table_name (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(50), money INT)";
            var create_table_cmd = new SQLiteCommand(sql_create_table, m_dbConnection);
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
                    new KeyboardButton("Игра 3 🏃‍")
                },
                new[]
                {
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
                    var m_dbConnection = new SQLiteConnection("Data Source=mydatabase.db;Version=3;");
                    m_dbConnection.Open();
                    var sql_insert_user = "INSERT INTO table_name (name, money) VALUES ('" + message.From.Username +
                                          "', 1000)";
                    var insert_user_cmd = new SQLiteCommand(sql_insert_user, m_dbConnection);
                    insert_user_cmd.ExecuteNonQuery();
                    m_dbConnection.Close();

                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        $"Добро пожаловать в мир азарта и развлечений,{message.From.FirstName}! 🎲🎰🃏 Сегодня у нас уникальный шанс попытать удачу и выиграть крупный джекпот! 💰💵💸",
                        replyMarkup: keyboard1);
                    break;

                case "Ознакомиться с правилами для игры 1 📖":

                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        "Если выпадает 1, вы получаете 600, в противном случае теряете 100.");
                    break;
                case "Ознакомиться с правилами для игры 2 📖":

                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        "Тройка удачи - это игра, в которой участник делает ставку на выпадение трех чисел от 1 до 3.\n Если все три числа совпадают, игрок выигрывает джекпот в размере 900 рублей.\n Если два числа совпадают, игрок получает выигрыш в 128 рублей.\n В случае, если все три числа разные, игрок проигрывает ставку. Удачи!");
                    break;
                case "Ознакомиться с правилами для игры 3 📖":

                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        "Бегущий человек - игра, в которой игрок должен пробежать заданную дистанцию, зарабатывая деньги за каждый метр.\n Если баланс игрока становится меньше или равен нулю, игра заканчивается.\n Если игрок успешно пробегает заданную дистанцию, его баланс обновляется в базе данных.");
                    break;
                case "Просмотреть баланс 💳":
                    // Получаем баланс пользователя из БД
                    var m_dbConnection2 = new SQLiteConnection("Data Source=mydatabase.db;Version=3;");
                    m_dbConnection2.Open();
                    var sql_select_user = "SELECT money FROM table_name WHERE name='" + message.From.Username + "'";
                    var select_user_cmd = new SQLiteCommand(sql_select_user, m_dbConnection2);
                    var result = select_user_cmd.ExecuteScalar();
                    m_dbConnection2.Close();

                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Ваш баланс: {result} руб.",
                        replyMarkup: keyboard1);
                    break;

                case "Игра 1 🎲":
                    Ruletka(botClient, message);
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
            var m_dbConnection = new SQLiteConnection("Data Source=mydatabase.db;Version=3;");
            m_dbConnection.Open();
            var sql_select_user = "SELECT money FROM table_name WHERE name='" + message.From.Username + "'";
            var select_user_cmd = new SQLiteCommand(sql_select_user, m_dbConnection);
            var result = select_user_cmd.ExecuteScalar();

            // Если у пользователя недостаточно средств, отправляем сообщение о нехватке средств и завершаем функцию
            if ((int)result < 100)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "К сожалению, на вашем балансе недостаточно средств для участия в игре. 💸🙁");
                m_dbConnection.Close();
                return;
            }

            // Обновляем баланс пользователя в БД
            var new_balance = (int)result - 100;
            var sql_update_user = "UPDATE table_name SET money=" + new_balance + " WHERE name='" +
                                  message.From.Username + "'";
            var update_user_cmd = new SQLiteCommand(sql_update_user, m_dbConnection);
            update_user_cmd.ExecuteNonQuery();
            m_dbConnection.Close();

            var random = new Random();
            var rand_num = random.Next(1, 7);

            if (rand_num == 1)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    $"Выпало число {rand_num}. Поздравляем, вы выиграли! 🥳 Вам начислено 600 руб. 💰");
                new_balance += 600;
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    $"Выпало число {rand_num}. К сожалению, вы проиграли. 😔");
            }

            // Обновляем баланс пользователя в БД
            m_dbConnection.Open();
            sql_update_user = "UPDATE table_name SET money=" + new_balance + " WHERE name='" + message.From.Username +
                              "'";
            update_user_cmd = new SQLiteCommand(sql_update_user, m_dbConnection);
            update_user_cmd.ExecuteNonQuery();
            m_dbConnection.Close();
        }


        private static async Task Kazino(ITelegramBotClient botClient, Message message)
        {
            // Получаем баланс пользователя из БД
            var m_dbConnection = new SQLiteConnection("Data Source=mydatabase.db;Version=3;");
            m_dbConnection.Open();
            var sql_select_user = "SELECT money FROM table_name WHERE name='" + message.From.Username + "'";
            var select_user_cmd = new SQLiteCommand(sql_select_user, m_dbConnection);
            var result = select_user_cmd.ExecuteScalar();

            // Если у пользователя недостаточно средств, отправляем сообщение о нехватке средств и завершаем функцию
            if ((int)result < 100)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "К сожалению, на вашем балансе недостаточно средств для участия в игре. 💸🙁");
                m_dbConnection.Close();
                return;
            }

            // Обновляем баланс пользователя в БД
            var new_balance = (int)result - 100;
            var sql_update_user = "UPDATE table_name SET money=" + new_balance + " WHERE name='" +
                                  message.From.Username + "'";
            var update_user_cmd = new SQLiteCommand(sql_update_user, m_dbConnection);
            update_user_cmd.ExecuteNonQuery();

            var random = new Random();
            var a = random.Next(1, 4);
            var b = random.Next(1, 4);
            var c = random.Next(1, 4);

            if (a == b && b == c)
            {
                new_balance += 1000;
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    $"Ура! Вы выиграли джекпот 🤑🎉\n Числа {a}, {b}, {c}. Вам начислено 900 руб.\n Баланс: {new_balance} руб. 💰");
            }
            else if (a == b || b == c || a == c)
            {
                new_balance += 128;
                var equalNumber = a == b ? a : b == c ? b : c;
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    $"Вы выиграли 🎊\n Дважды выпало число {equalNumber}. Числа {a}, {b}, {c}.\n Вам начислено 28 руб. Баланс: {new_balance} руб. 😁");
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    $"К сожалению, Вы проиграли 🙁\n Числа {a}, {b}, {c}\n Но не расстраивайтесь, в следующий раз обязательно повезет! Баланс: {new_balance} руб. 💸");
            }


            // Обновляем баланс пользователя в БД
            sql_update_user = "UPDATE table_name SET money=" + new_balance + " WHERE name='" + message.From.Username +
                              "'";
            update_user_cmd = new SQLiteCommand(sql_update_user, m_dbConnection);
            update_user_cmd.ExecuteNonQuery();
            m_dbConnection.Close();
        }

        private static async Task RunGame(ITelegramBotClient botClient, Message message)
        {
            // Получаем баланс пользователя из БД
            var connection = new SQLiteConnection("Data Source=mydatabase.db;Version=3;");
            connection.Open();
            var sqlSelectUser = $"SELECT money FROM table_name WHERE name='{message.From.Username}'";
            var selectUserCmd = new SQLiteCommand(sqlSelectUser, connection);
            var balance = Convert.ToInt32(selectUserCmd.ExecuteScalar());
            if (balance < 100)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "К сожалению, на вашем балансе недостаточно средств для участия в игре. 💸🙁");
                connection.Close();
                return;
            }

            // Позиция человека
            var position = 0;
            var random = new Random();
            // Дистанция, которую нужно пробежать
            var distance = random.Next(1, 11);
            // Отправляем сообщение с правилами игры и текущим балансом
            await botClient.SendTextMessageAsync(message.Chat.Id,
                $"Добро пожаловать в игру \"Бегущий человек\"! 🏃‍♂️  \nВаш текущий баланс: {balance} руб 💰💰💰.");
            balance -= 100;
            // Флаг для проверки, закончена ли игра
            var gameOver = false;

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

                    var sqlUpdateUser =
                        $"UPDATE table_name SET money={balance} WHERE name='{message.From.Username}'";
                    var updateUserCmd = new SQLiteCommand(sqlUpdateUser, connection);
                    updateUserCmd.ExecuteNonQuery();

                    gameOver = true;
                }
                else
                {
                    // Генерируем случайное число, которое определяет, сколько денег получит пользователь за текущий шаг

                    var money = random.Next(10, 50);

                    // Обновляем баланс пользователя
                    balance += money;

                    // Отправляем сообщение с результатами текущего шага
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        $"Вы прошли {position} метров и заработали {money} руб. 💰\nВаш текущий баланс: {balance} руб.");

                    // Обновляем баланс пользователя в БД
                    var sqlUpdateUser =
                        $"UPDATE table_name SET money={balance} WHERE name='{message.From.Username}'";
                    var updateUserCmd = new SQLiteCommand(sqlUpdateUser, connection);
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