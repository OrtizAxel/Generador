//Axel Ortiz Ricalde

using System;
using System.IO;

namespace Generador
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using(Lenguaje a = new Lenguaje())
                {
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
