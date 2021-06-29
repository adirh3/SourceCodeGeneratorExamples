using System;

namespace CodeGeneratorExamples
{
    [ConvertStatic]
    public static class MyStaticClass
    {
        private static bool _state;
        
        public static bool ToggleState()
        {
            _state = !_state;
            return _state;
        }
    }
}