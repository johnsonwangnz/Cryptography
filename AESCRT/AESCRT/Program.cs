
using System;
using System.Security.Cryptography;
using System.Text;

namespace AESCRT
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Enter AES Key in hex...");
            // var key = Console.ReadLine();
            var key = "36f18357be4dbd77f050515c73fcf9f2";
        

            var keyBytes = HexStringToByteArray(key);

            //  Console.WriteLine("Enter plain text in hex, can be null if it is for decyption...");
            //  var plainText = Console.ReadLine();


            Console.WriteLine("Enter Cipher text in hex, can be null if it is for encryption...");
            //   var cipherTextAndIV = Console.ReadLine();
            var cipherTextAndIV =
                "69dda8455c7dd4254bf353b773304eec0ec7702330098ce7f7520d1cbbb20fc388d1b0adb5054dbd7370849dbf0b88d393f252e764f1f5f7ad97ef79d59ce29f5f51eeca32eabedd9afa9329";
            // var cipherTextAndIV =
             //   "770b80259ec33beb2561358a9f2dc617e46218c0a53cbeca695ae45faa8952aa0e311bde9d4e01726d3184c34451";

            if (!string.IsNullOrWhiteSpace(cipherTextAndIV))
            {
                // To get the IV 32 characters =16 bytes in hex
                var ivText = cipherTextAndIV.Substring(0, 32);
                var cipherText = cipherTextAndIV.Substring(32, cipherTextAndIV.Length - 32);

                var iv = HexStringToByteArray(ivText);

                var cipherBytes = HexStringToByteArray(cipherText);


                var decipheredBytes = CRTEncryptBytes(cipherBytes, keyBytes, iv);

                
                var messageHexString = ByteArrayTohexString(decipheredBytes);

                var plainText = System.Text.Encoding.ASCII.GetString(decipheredBytes);
                Console.WriteLine(string.Format("Plain text in Hex is {0}", messageHexString));
                Console.WriteLine(string.Format("Plain text is {0}", plainText));


            }



            Console.WriteLine(string.Format("Key is {0}", key));



            Console.WriteLine("Press any key to return..");

            Console.ReadKey();

        }

        private static byte[] HexStringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars/2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i/2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        private static string ByteArrayTohexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length*2);
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


        //Call it with higher bound
        private static void IncrementAtIndex(byte[] array, int index)
        {

            if (array[index] == byte.MaxValue)
            {
                array[index] = 0;
                if (index > 0)
                    IncrementAtIndex(array, index - 1);
            }
            else
            {
                array[index]++;
            }
        }


        //AES, key 128bits/16 bytes, block size 128 bits
        //Both CRT encrypt and decrypt using AES Ecrypt of increamental iv
        private static byte[] AESEnrypt(byte[] key, byte[] plainBytes)
        {
            using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider())
            {
                myAes.Key = key;
                myAes.Mode = CipherMode.ECB; // no iv
                myAes.Padding = PaddingMode.None;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = myAes.CreateEncryptor(myAes.Key, null);

                var outputBuffer = new Byte[16];

                // Create the streams used for decryption. 
                decryptor.TransformBlock(plainBytes, 0, 16, outputBuffer, 0);


                return outputBuffer;

            }

        }

        // decrypt using the same function
        private static byte[] CRTEncryptBytes(byte[] plainBytes, byte[] key, byte[] iv)
        {
            // divid it into blocks then start to do CTR
            var cipheredBytes = new byte[plainBytes.Length];

            for (int i = 0; i < plainBytes.Length; i += 16)
            {


                if (i + 16 <= plainBytes.Length)
                {
                    // get the encryptuion of iv
                    var plainBlockBytes = new byte[16];

                    Buffer.BlockCopy(plainBytes, i, plainBlockBytes, 0, 16);

                    var encryptedBytes = AESEnrypt(key, iv);

                    // increase iv
                    IncrementAtIndex(iv, 15);

                    var xoredBytes = XOR(plainBlockBytes, encryptedBytes);

                    Buffer.BlockCopy(xoredBytes, 0, cipheredBytes, i, 16);

                }
                else
                {
                    // get the encryption of iv
                    var cipherBlockBytes = new byte[plainBytes.Length - i];

                    Buffer.BlockCopy(plainBytes, i, cipherBlockBytes, 0, plainBytes.Length - i);

                    var decryptedBytes = AESEnrypt(key, iv);

                    var xoredBytes = XOR(cipherBlockBytes, decryptedBytes);

                    Buffer.BlockCopy(xoredBytes, 0, cipheredBytes, i, plainBytes.Length - i);
                }



            }

            return cipheredBytes;

        }
        
    }
}
