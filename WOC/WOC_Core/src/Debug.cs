using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WOC_Core
{
    public class LOG
    {
        public static void Print()
        {
#if UNITY
            Debug.Log();
#else
            Console.WriteLine();
#endif
        }

        public static void Print(float value)
        {
#if UNITY
            Debug.Log(value);
#else
            Console.WriteLine(value);
#endif
        }

        public static void Print(int value)
        {
#if UNITY
            Debug.Log(value);
#else
            Console.WriteLine(value);
#endif
        }

        public static void Print(uint value)
        {
#if UNITY
            Debug.Log(value);
#else
            Console.WriteLine(value);
#endif
        }

        public static void Print(long value)
        {
#if UNITY
            Debug.Log(value);
#else
            Console.WriteLine(value);
#endif
        }

        public static void Print(ulong value)
        {
#if UNITY
            Debug.Log(value);
#else
            Console.WriteLine(value);
#endif
        }

        public static void Print(object value)
        {
#if UNITY
            Debug.Log(value);
#else
            Console.WriteLine(value);
#endif
        }

        public static void Print(string value)
        {
#if UNITY
            Debug.Log(value);
#else
            Console.WriteLine(value);
#endif
        }

        public static void Print(string format, object arg0)
        {
#if UNITY
            Debug.Log(format, arg0);
#else
            Console.WriteLine(format, arg0);
#endif
        }

        public static void Print(string format, object arg0, object arg1, object arg2)
        {
#if UNITY
            Debug.Log(format, arg0, arg1, arg2);
#else
            Console.WriteLine(format, arg0, arg1, arg2);
#endif
        }

        public static void Print(string format, object arg0, object arg1, object arg2, object arg3)
        {
#if UNITY
            Debug.Log(format, arg0, arg1, arg2, arg3);
#else
            Console.WriteLine(format, arg0, arg1, arg2, arg3);
#endif
        }

        public static void Print(string format, params object[] arg)
        {
#if UNITY
            Debug.Log(format, arg);
#else
            Console.WriteLine(format, arg);
#endif
        }

        public static void Print(char[] buffer, int index, int count)
        {
#if UNITY
            Debug.Log(buffer, index, count);
#else
            Console.WriteLine(buffer, index, count);
#endif
        }

        public static void Print(decimal value)
        {
#if UNITY
            Debug.Log(value);
#else
            Console.WriteLine(value);
#endif
        }

        public static void Print(char[] buffer)
        {
#if UNITY
            Debug.Log(buffer);
#else
            Console.WriteLine(buffer);
#endif
        }

        public static void Print(char value)
        {
#if UNITY
            Debug.Log(value);
#else
            Console.WriteLine(value);
#endif
        }

        public static void Print(bool value)
        {
#if UNITY
            Debug.Log(value);
#else
            Console.WriteLine(value);
#endif
        }

        public static void Print(string format, object arg0, object arg1)
        {
#if UNITY
            Debug.Log(format, arg0, arg1);
#else
            Console.WriteLine(format, arg0, arg1);
#endif
        }

        public static void Print(double value)
        {
#if UNITY
            Debug.Log(value);
#else
            Console.WriteLine(value);
#endif
        }

    }
}
