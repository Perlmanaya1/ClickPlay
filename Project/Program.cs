using System;
using Algoritem;
using NFugue.Midi;
using NFugue.Patterns;
using NFugue.Playing;
using NFugue.Theory;

namespace Project
{
    class Program
    {
        static void Main(string[] args)
        {
            Algoritem.Algoritem algoritem = new Algoritem.Algoritem();
            algoritem.exec();
            Console.Read();

        }
    }
}
