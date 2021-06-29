using System;

namespace CodeGeneratorExamples
{
    [TryCatch]
    public partial class MyController
    {
        public int RunStuff()
        {
            if (DateTime.Now.Minute == 6)
                throw new InvalidOperationException("Try to be considerate about time!");

            return 5;
        }
        
    }
}