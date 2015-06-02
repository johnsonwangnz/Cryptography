



using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace DiscreteLogModulo
{
    class Program
    {
        private static string p =
           "13407807929942597099574024998205846127479365820592393377723561443721764030073546976801874298166903427690031858186486050853753882811946569946433649006084171";

        private static string g =
            "11717829880366207009516117596335367088558084999998952205599979459063929499736583746670572176471460312928594829675428279466566527115212748467589894601965568";

        private static string h =
            "3239475104050450443565264378728065788649097520952449527834792452971981976143292558073856937958553180532878928001494706097394108577585732452307673444020333";

        private static void Main(string[] args)
        {
          
            BigInteger bigInteger_p = BigInteger.Parse(p);
            BigInteger bigInteger_g = BigInteger.Parse(g);
            BigInteger bigInteger_h = BigInteger.Parse(h);

            int Half = 20;
          
            /**
            BigInteger bigInteger_p = BigInteger.Parse("1073676287");
            BigInteger bigInteger_g = BigInteger.Parse("1010343267");
            BigInteger bigInteger_h = BigInteger.Parse("857348958");

            int Half = 10;
             **/
            Console.WriteLine("Started from {0}", DateTime.Now);

            MeetTheHalfWay(bigInteger_p,bigInteger_g,bigInteger_h,Half);
            Console.WriteLine("Finished from {0}", DateTime.Now);

            Console.ReadKey();
        }

        private static void MeetTheHalfWay(BigInteger bigInteger_p, BigInteger bigInteger_g,
            BigInteger bigInteger_h, int half)
        {

            int upLimit = (int)Math.Pow(2, half);

            BigInteger B = new BigInteger(upLimit);

            var map_gx1 = new BigInteger[upLimit+1];
            
            BigInteger hModP = bigInteger_h % bigInteger_p;
            BigInteger pMinus2 = bigInteger_p - 2;

     
            Console.WriteLine("Starting to build table...");

            // build the table
            for (int n = 0; n <=upLimit ; n++)
            {

                BigInteger gx1nInverse = BigInteger.ModPow(bigInteger_g, pMinus2 * n, bigInteger_p);
                
                var leftn = (hModP * gx1nInverse) % bigInteger_p;
                map_gx1[n] = leftn;

                Console.Write("\r  Working on x1 {0} ", n);
                
            }

            Console.WriteLine("Table is built, now computer the right...");

        
            
            // calculate the right

            for (int k = 0; k <= upLimit; k++)
            {
                var power =new BigInteger(k)*B;

                BigInteger gbRaised = BigInteger.ModPow(bigInteger_g, power, bigInteger_p);

                Console.Write("\r  Working on x0 {0} ", k);
           
                var x1 = Array.IndexOf(map_gx1, gbRaised);
                
                if (x1 >= 0)
                {

                    Console.WriteLine("Found x1 {0}", x1);
                    Console.WriteLine("x0 {0}", k);

                    var x = new BigInteger(k)*B + x1;

                    var result = x.ToString();

                    Console.WriteLine(result);

                    break;
                }

                if (k == upLimit)
                {
                    Console.WriteLine("Not found");
                }
            }

        }
        
    }
}
