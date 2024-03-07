namespace ConsoleApp
{
    internal class Program
    {
        static void Main()
        {
            var reader = new DataReader();
            reader.ImportAndPrintData("data.csv");
        }
    }
}
