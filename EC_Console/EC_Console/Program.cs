using System;
using System.Numerics;
using System.Threading;

namespace EC_Console
{
    class Program
    {
        private static Lenstra Lenstra = new Lenstra();

        static void Main(string[] args)
        {
            #region Для покрытия тестами
            //var ec = new EllepticCurve(1, 1, 23);
            //var p1 = ec.CreatePoint(0, 1);
            //var p2 = ec.Sum(p1, p1);

            //for (int i = 0; i < 100; i++)
            //{
            //    p2 = ec.Sum(p2, p1);
            //    if(!p2.Equals(ec.Mult(i+3,p1)))
            //        throw new Exception("qqweqweqwe");
            //    Console.WriteLine(p2);
            //}
            #endregion

            
            //var divider = Lenstra.GetDivider(BigInteger.Parse("50521984138040381699131985921"));
            for(var i =0; i< 10; i++)
                new Thread(test).Start();

        }

        static void test()
        {
            //throw new GCDFoundException(5);
        }
    }
}
