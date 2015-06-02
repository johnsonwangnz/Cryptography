using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace PaddingOracle
{
    class Program
    {
        static void Main(string[] args)
        {
            var OriginalHex =
                "f20bdba6ff29eed7b046d1df9fb7000058b1ffb4210a580f748b4ac714c001bd4a61044426fb515dad3f21f18aa577c0bdf302936266926ff37dbf7035d5eeb4";
            var urlStub = "http://crypto-class.appspot.com/po?er=";

            // convert originalhex into bytes
            var OriginalBytes = HexStringToByteArray(OriginalHex);

            var numberOfblocks = OriginalBytes.Length/16;

            var messageText = new byte[(numberOfblocks - 1)*16];

            for (int i = 1; i <= numberOfblocks - 1; i++)
         //   for (int i = numberOfblocks - 1; i <= numberOfblocks - 1; i++) // last block only
          
            {
                Console.WriteLine("Cracking block {0}", i);
                // To get cypher text  cipherblock[i-1]
                var cipherText = new byte[16];
                Buffer.BlockCopy(OriginalBytes, (i-1)*16, cipherText,0,16);

                var messageGuessText = new byte[16];
                var paddingText = new byte[16];
           
                for (int n1 = 0; n1 < 16; n1++)
                {
                    messageGuessText[n1] = 0;
                    paddingText[n1] = 0;
                }

           
                // nth byte of messageblock[i]
                for (int j=15; j>=0; j--)
                {

                    Console.WriteLine("Cracking block {0} character {1}..", i, j);
                    // Set up padding text
                    for (int n2 = 15; n2 >= j; n2--)
                    {
                        paddingText[n2] = (byte)(16 - j);
                    }

                    for (int k = 127; k >= 0; k--)
                    {
                        Console.Write("\r  Working our on block {0} character {1} testing: {2}", i, j,k);
                        // set up messageGuessText 
                        messageGuessText[j] =(byte)k;

                        var messageXor = XOR(paddingText, messageGuessText);
                        var newCipherText = XOR(cipherText, messageXor);
                        // Send new cipher text to server to test
                        var newCompleteCipherText = new byte[32];
                        // Copy original 
                        Buffer.BlockCopy(newCipherText, 0, newCompleteCipherText, 0, 16);
                        // Replace decoding block
                        Buffer.BlockCopy(OriginalBytes, i*16, newCompleteCipherText, 16, 16);
                   
                        var isPaddingCorrect =  IsPaddingValidAsync(
                            urlStub + ByteArrayTohexString(newCompleteCipherText));



                        if (isPaddingCorrect)
                        {

                            if (i == numberOfblocks - 1)
                            {
                                // The last block, if k=01 and testing 01 position, it will cancel out with testing position
                                // what is left is original message with the padding part, normal message will not be like this
                                // just ignore
                                if (k <= 16 && k>1)
                                {
                                    // this is the padding
                                    for (int n3 = 1; n3 <= k - 1; n3++)
                                    {
                                        j--;
                                        messageGuessText[j] = (byte)k;
                                        

                                    }
                                }

                            }
                            Console.WriteLine("Found block {0} character {1} is {2}", i, j, k);
                            break;
                        }

                        if (k == 00)
                        {
                            // did not find 
                            Console.WriteLine("Did not find {0}th charracter for {1}th block", j,i);
                            throw new Exception("Should not happen!");

                        }

                    }

                }

                // copy the whole message block to large message block
                Buffer.BlockCopy(messageGuessText, 0, messageText, (i-1)*16,16);

                Console.WriteLine("Block {0} is {1}", i, Encoding.ASCII.GetString(messageGuessText));
            }

            var plainText =Encoding.ASCII.GetString(messageText);

            Console.WriteLine("Whole message is {0}", plainText);

            Console.ReadKey();
        }

        private static bool IsPaddingValidAsync(string url)
        {
            var result = false;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response =  client.GetAsync(url).Result;

                result = response.StatusCode == HttpStatusCode.NotFound ||
                    response.StatusCode== HttpStatusCode.OK;
            }

            return result;
        }

        private static byte[] HexStringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        private static string ByteArrayTohexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        //truncate to the shortest
        private static byte[] XOR(byte[] buffer1, byte[] buffer2)
        {
            var length = buffer1.Length <= buffer2.Length
                ? buffer1.Length
                : buffer2.Length;

            var result = new byte[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = (byte)(buffer1[i] ^ buffer2[i]);
            }
            return result;

        }
    }
}
