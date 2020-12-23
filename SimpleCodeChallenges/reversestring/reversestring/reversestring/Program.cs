using System;

namespace reversestring
{
    class reverse
    {
        public string s { get; set; }

        public void reversestring(string InputString)
        {
            int length = InputString.Length;
            int indexOfInput = 0;
            char[] TempCharArray = new char[length];
            for(int i = length-1;i>=0;i--)
            {
                TempCharArray[i] = InputString[indexOfInput];
                indexOfInput++;
            }
            string NewTempString = new string(TempCharArray);
            s = NewTempString;
        }
    }
    class Program
    {

        static void Main(string[] args)
        {
            reverse rs = new reverse();
            rs.reversestring("Hello World!");
            Console.WriteLine(rs.s);
        }
    }
}
