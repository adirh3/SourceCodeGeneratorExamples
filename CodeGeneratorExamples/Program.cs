using System;

namespace CodeGeneratorExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            bool toggleState = MyStaticClass.ToggleState();
            Console.WriteLine(toggleState);
            var myController = new MyController();
            myController.TryRunStuff();
        }
    }
}