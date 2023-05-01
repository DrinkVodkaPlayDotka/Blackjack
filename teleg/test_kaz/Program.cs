using System;

class Program {
    static void Main(string[] args) {
        int money = 1000;
        int step = 0;

        while (true)
        {
            Console.WriteLine($"You have ${money}");
            Console.Write("Do you want to go on? (y/n) ");
            string go_on = Console.ReadLine().ToLower();

            if (go_on == "n")
            {
                break;
            }

            Random random = new Random();
            int value = random.Next(0, 11);
            if (step == value)
            {
                money -= 100;
                Console.WriteLine("You lose!");
                break;
                
            }
            else
            {
                Console.WriteLine($"Your win, money:{money}");
            }

        }

        Console.WriteLine($"Thanks for playing! You ended with ${money}.");
    }
}