// See https://aka.ms/new-console-template for more information

void kaz(float balance)
{
    int a;
    int b;
    int c;
    Random random = new Random();
    a = random.Next(1, 4);
    b = random.Next(1, 4);
    c = random.Next(1, 4);

    if (a == b & b == c)
    {
        
        balance = balance * 27;
    }
    else if (a == b || b == c || a == c)
    {
        balance = balance * 27/3;
    }
    else
    {
        Console.WriteLine("Ты проиграл");
    }

}