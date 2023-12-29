using System;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ClassLibrary;


class Program
{
    static void Main()
    {
        BitMatrixGenerator generator = new BitMatrixGenerator(10);
        generator.DisplayMatrix();
        generator.FindFreeAreas();
        generator.ShowAreas();
        Console.ReadLine();
    }
}