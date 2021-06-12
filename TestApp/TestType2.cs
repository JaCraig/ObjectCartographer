using System;

namespace TestApp
{
    internal class TestType2
    {
        public TestType2(int a)
        {
            A = a;
        }

        public TestType2()
        {
        }

        public int A { get; set; }

        public decimal B { get; set; }

        public MyEnum C { get; set; }

        public DateTime? D { get; set; }

        public MyEnum2 E { get; set; }

        public TimeSpan F { get; set; }

        public TestType2 G { get; set; }
    }
}