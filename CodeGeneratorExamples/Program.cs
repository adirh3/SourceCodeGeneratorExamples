using System;
using System.Threading.Tasks;
using CodeGeneratorExamplesGenerated;

namespace CodeGeneratorExamples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            bool toggleState = MyStaticClass.ToggleState();
            Console.WriteLine(toggleState);
            
            var myController = new MyController();
            myController.RunStuff();

            var myHeavyClass = new MyHeavyClass();
            await myHeavyClass.DoHeavyStuff();

        }
    }
}