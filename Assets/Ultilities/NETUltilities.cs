using System;

public static class NETUltilities
{
    private static Random random = new Random();

    public static int GetRandomInt(int from = 0, int to = 10)
    {
        return random.Next(from, to); // to : Exclusive;
    }
}
