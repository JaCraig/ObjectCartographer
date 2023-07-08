using Microsoft.Extensions.DependencyInjection;
using System.Dynamic;
using System.Text;

namespace ObjectCartographer.Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ServiceProvider? Collection = new ServiceCollection().AddCanisterModules()?.BuildServiceProvider();
            if (Collection is null)
                return;

            // Create an object of type TestType2 from an object of type TestType1 and copy the
            // value of A
            TestType2 Result = new TestType1 { A = 10 }.To<TestType2>();
            Console.WriteLine(Result.A);

            // Copy the value of A from an object of type TestType1 to an object of type TestType2
            TestType2 Result2 = new TestType1 { A = 20 }.To(new TestType2());
            Console.WriteLine(Result2.A);

            // Convert the string "Example String" to a byte array and then back to a string
            var Result3 = "Example String".To<byte[]>();
            Console.WriteLine(Encoding.UTF8.GetString(Result3));
            Console.WriteLine(Result3.To<string>());

            // Convert an ExpandoObject to a Dictionary<string, object>
            dynamic ExpandoTest = new ExpandoObject();
            ExpandoTest.A = 55;
            ExpandoTest.B = "Test";
            dynamic Result4 = ((ExpandoObject)ExpandoTest).To<Dictionary<string, object>>();
            Console.WriteLine(Result4["A"]);
            Console.WriteLine(Result4["B"]);
        }
    }

    /// <summary>
    /// Test class 1
    /// </summary>
    internal class TestType1
    {
        /// <summary>
        /// Gets or sets a.
        /// </summary>
        /// <value>a.</value>
        public int A { get; set; }
    }

    /// <summary>
    /// Test class 2
    /// </summary>
    internal class TestType2
    {
        /// <summary>
        /// Gets or sets a.
        /// </summary>
        /// <value>a.</value>
        public int A { get; set; }
    }
}