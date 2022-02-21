using System;
using System.Collections.Generic;
using ConsoleTables;

namespace CryptoTax.ConsoleApp.Application
{
    public static class Output
    {
        public static void Write(string txt) => Console.Write(txt);

        public static void Write(ConsoleColor color, string txt)
        {
            Console.ForegroundColor = color;
            Console.Write(txt);
            Console.ResetColor();
        }

        public static void WritePrompt() => Write(ConsoleColor.Blue, "[CryptoTax]: ");

        public static void WriteLine(string txt)
        {
            Console.WriteLine(txt);
            Console.ResetColor();
        }

        public static void WriteLine(ConsoleColor color, string txt)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(txt);
            Console.ResetColor();
        }

        public static void WriteError(string txt) => WriteLine(ConsoleColor.DarkRed, txt);

        public static void WriteTable<T>(IEnumerable<T> data) =>
            Console.WriteLine(ConsoleTable.From(data).ToMinimalString().TrimEnd());
    }
}
