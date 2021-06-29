using System.Threading.Tasks;

namespace CodeGeneratorExamples
{
    [BenchmarkAttribute]
    public class MyHeavyClass
    {
        public async Task DoHeavyStuff()
        {
            // Start benchmark
            await Task.Delay(100);
            // End benchmark
            
            // Start benchmark
            await Task.Delay(1000);
            // End benchmark
        }
    }
}