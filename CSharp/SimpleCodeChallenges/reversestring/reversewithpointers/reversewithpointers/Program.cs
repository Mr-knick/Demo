using System;

namespace reversewithpointers
{
    class Program
    {
        static void Main(string[] args)
        {
            unsafe
            {
                static void reverse(char* pMyChar)
                {
                    char* pMyCharStart = pMyChar;
                    char* pMyCharEnding = pMyChar;
                    while (*pMyCharEnding != '\0')
                    {
                        pMyCharEnding += 1;
                    }
                    pMyCharEnding--;

                    for (; pMyChar < pMyCharEnding; pMyChar++, pMyCharEnding --)
                    {
                        char tempChar = *pMyCharEnding;
                        *pMyCharEnding = *pMyChar;
                        *pMyChar = tempChar;
                    }
                }

                string s = "Hello World!";
                char[] MyChar = new char[s.Length + 1];
                for(int i = 0; i < s.Length; i++)
                {
                    MyChar[i] = s[i];
                }
                MyChar[s.Length] = '\0';
                Console.WriteLine(MyChar);

                fixed ( char* pMyChar = &MyChar[0])
                {
                    reverse(pMyChar);
                }
                Console.WriteLine(MyChar);
            }
        }
    }
}
