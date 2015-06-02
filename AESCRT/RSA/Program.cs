

using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace RSA
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start to crack now...");
            Challenge3();
            Console.ReadKey();
        }

        private static BigInteger SQRT1(BigInteger N)
        {
            if (N == 0)
            {
                return 0;
            }

            BigInteger n1 = (N >> 1) + 1;
            BigInteger n2 = (n1 + (N / n1)) >> 1;

            while (n2 < n1)
            {
                n1 = n2;
                n2 = (n1 + (N / n1)) >> 1;
            }

            return n1;
        }


        private static void Challenge1()
        {
            BigInteger N = BigInteger.Parse(
                "17976931348623159077293051907890247336179769789423065727343008115" +
                "77326758055056206869853794492129829595855013875371640157101398586" +
                "47833778606925583497541085196591615128057575940752635007475935288" +
                "71082364994994077189561705436114947486504671101510156394068052754" +
                "0071584560878577663743040086340742855278549092581");

            BigInteger A = SQRT1(N) + 1;


            BigInteger p,q;

            bool Found = TestA(A, N, out p, out q);


            if (Found)
            {
                var answer = p.ToString();
                Console.WriteLine("Find the p {0}", answer);
                //Save it to file
                StreamWriter file = new StreamWriter("challenge1.txt", true);
                file.WriteLine("Challenge1 is :");
                file.WriteLine("p is:");
                file.WriteLine(p.ToString());

                file.WriteLine("q is:");
                file.WriteLine(q.ToString());

                file.Close();
            }
            else
            {
                Console.WriteLine("Not Found.");
            }

        }

        private static void Challenge2()
        {
            BigInteger N = BigInteger.Parse(
                "6484558428080716696628242653467722787263437207069762630604390703787" +
                "9730861808111646271401527606141756919558732184025452065542490671989" +
                "2428844841839353281972988531310511738648965962582821502504990264452" +
                "1008852816733037111422964210278402893076574586452336833570778346897" +
                "15838646088239640236866252211790085787877");

            BigInteger nRoot = SQRT1(N);

            int difference = (int)Math.Pow(2, 20);

            BigInteger p,q;

            for (int k = 0; k < difference; k++)
            {
                BigInteger A = nRoot + k;

                Console.Write("\r  Working on k {0} of {1} ", k, difference);

                bool found = TestA(A, N, out p, out q);

                if (found)
                {
                    var answer = p.ToString();
                    Console.WriteLine("Find the p {0}", answer);
                    //Save it to file
                    StreamWriter file = new StreamWriter("challenge2.txt", true);
                    file.WriteLine("Challenge2 is :");
                    file.WriteLine("p is:");
                    file.WriteLine(p.ToString());

                    file.WriteLine("q is:");
                    file.WriteLine(q.ToString());

                    file.Close();
                    break;
                }

            }


        }


        private static bool TestA(BigInteger A, BigInteger N, out BigInteger p, out BigInteger q)
        {
            BigInteger ASquresAndN = A * A - N;

            BigInteger X = SQRT1(ASquresAndN);

            p = A - X;

            q = A + X;

            BigInteger N1 = p * q;

            return N1 == N;

        }

        private static BigInteger ModInv(BigInteger a, BigInteger b)
        {
            BigInteger b0, t, q;
            b0 = b;

            BigInteger x0 = 0;
            BigInteger x1 = 1;
	        if (b == 1) return 1;

            while (a > 1)
            {
                q = a/b;
                t = b;
                b = a%b;
                a = t;
                t = x0;
                x0 = x1 - q*x0;
                x1 = t;
            }

            if (x1 < 0)
            {
                x1 += b0;
            }

            return x1;
        }

        private static void Challenge4()
        {
            BigInteger p = BigInteger.Parse(
                "13407807929942597099574024998205846127479365820592393377723561443721764030073662768891111614362326998675040546094339320838419523375986027530441562135724301");
            BigInteger q = BigInteger.Parse(
                "13407807929942597099574024998205846127479365820592393377723561443721764030073778560980348930557750569660049234002192590823085163940025485114449475265364281");

             BigInteger N = BigInteger.Parse(
                "17976931348623159077293051907890247336179769789423065727343008115" +
                "77326758055056206869853794492129829595855013875371640157101398586" +
                "47833778606925583497541085196591615128057575940752635007475935288" +
                "71082364994994077189561705436114947486504671101510156394068052754" +
                "0071584560878577663743040086340742855278549092581");


            BigInteger OrderOfN = (p - 1)*(q - 1);

            BigInteger publicKeyE = BigInteger.Parse("65537");

            BigInteger privateKeyD = ModInv(publicKeyE, OrderOfN);

            var modED = (publicKeyE*privateKeyD)%OrderOfN;

            BigInteger c = BigInteger.Parse(
                "22096451867410381776306561134883418017410069787892831071731839143676135600120538004282329650473509424343946219751512256465839967942889460764542040581564748988013734864120452325229320176487916666402997509188729971690526083222067771600019329260870009579993724077458967773697817571267229951148662959627934791540"
                );

            // RSA decode
            BigInteger plaintextint = BigInteger.ModPow(c, privateKeyD, N);

            // convert it to byes array

            Byte[] plainTextBytes = plaintextint.ToByteArray();
            
            int fPosition = -1;

            StringBuilder sb = new StringBuilder();

            for (int k = plainTextBytes.Length - 1; k >= 0; k--)
            {
                if (fPosition > 0)
                {
                    sb.Append(System.Text.Encoding.ASCII.GetString(new byte[] {plainTextBytes[k]}));

                }
                if (k == plainTextBytes.Length - 1)
                {
                    // it must be x02
                    if (plainTextBytes[k] != 0x02)
                    {
                        throw new Exception("Wrong message: first byte not correct");
                    }
         
                }

                if (plainTextBytes[k] == 0x00)
                {
                    fPosition = k;
                }
            }

            if (fPosition < 0)
            {
                throw new Exception("Wrong message: can not find FF positon");
            }

            Console.WriteLine("Find the plain text {0}", sb.ToString());
            //Save it to file
            StreamWriter file = new StreamWriter("challenge4.txt", true);
            file.WriteLine("Challenge4 is :");
            file.WriteLine(sb.ToString());

            file.Close();

        }

        private static void Challenge3()
        {
            BigInteger N = BigInteger.Parse(
                "72006226374735042527956443552558373833808445147399984182665305798191" +
                "63556901883377904234086641876639384851752649940178970835240791356868" +
                "77441155132015188279331812309091996246361896836573643119174094961348" +
                "52463970788523879939683923036467667022162701835329944324119217381272" +
                "9276147530748597302192751375739387929");

            BigInteger N6 = 6*N ;
            BigInteger A = 2*SQRT1(N6)+1; // a= 3p+2q as 3p+2q/2 not necessarily an integer

            BigInteger BSquared = A * A - 4*N6;// where B is 3p-2q

            BigInteger B = SQRT1(BSquared);

            

            BigInteger p3 = (A + B) /2 ;

            BigInteger q2 = (A - B) /2;

            BigInteger p3timesq2 = p3 * q2 ;

              BigInteger q = p3 / 2;
              BigInteger p = q2 / 3;
              
               // BigInteger p = p3 / 3;
               // BigInteger q = q2 / 2;
                if (N == p * q)
                {
                    BigInteger answer = p < q ? p : q;

                    Console.WriteLine("Find the p {0}", answer);
                    //Save it to file
                    StreamWriter file = new StreamWriter("challenge3.txt", true);
                    file.WriteLine("Challenge3 is :");
                    file.WriteLine(answer);

                    file.Close();
                }
                else
                {
                    Console.WriteLine("Find the 6n but not n");
                }

        }

        public static BigInteger SQRT2(BigInteger n)
        {
            if (n == 0) return 0;
            if (n > 0)
            {
                int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n, 2)));
                BigInteger root = BigInteger.One << (bitLength / 2);

                while (!isSqrt(n, root))
                {
                    root += n / root;
                    root /= 2;
                }

                return root;
            }

            throw new ArithmeticException("NaN");
        }

        private static Boolean isSqrt(BigInteger n, BigInteger root)
        {
            BigInteger lowerBound = root * root;
            BigInteger upperBound = (root + 1) * (root + 1);

            return (n >= lowerBound && n < upperBound);
        }
    }
}
