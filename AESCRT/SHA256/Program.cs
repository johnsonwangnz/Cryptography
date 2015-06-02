

using System;
using System.CodeDom;
using System.Configuration;
using System.IO;
using System.Text;


namespace SHA256
{
    class Program
    {
        static void Main(string[] args)
        {

           

            var pathSource = ConfigurationManager.AppSettings["fileName"];

           // Test(pathSource);

            var blockSize = 1024;
            var hashSize = 32; 

            // Initialize a SHA256 hash object.
            var mySHA256 = System.Security.Cryptography.SHA256Managed.Create();

            try
            {
                //Read the file
                using (FileStream fsSource = new FileStream(pathSource,
                    FileMode.Open, FileAccess.Read))
                {
                    var fileSize = fsSource.Length;
                    // To get the number of blocks

                    var sizeOfLastBlock =(int)(fileSize%blockSize);

                    var numberOfBlocks = (int)((fileSize - sizeOfLastBlock)/blockSize) + 1;

                    var hashArray = new byte[numberOfBlocks][];

                  
                    for (var i = numberOfBlocks-1; i >=0; i--)
                    {
                        var inputBlock = new byte[blockSize + hashSize];

                        fsSource.Seek(i*blockSize, SeekOrigin.Begin);
                        // read file content
                        if (i == numberOfBlocks-1)
                        {
                            // last block
                            inputBlock = new byte[sizeOfLastBlock];

                       
              

                            var bytesRead = fsSource.Read(inputBlock, 0, sizeOfLastBlock);
                            if (bytesRead != sizeOfLastBlock)
                            {
                                // failed to read
                                throw new Exception("File read error");
                            }

                        }
                        else
                        {

                            var bytesRead = fsSource.Read(inputBlock, 0, blockSize);

                            if (bytesRead != blockSize)
                            {
                                // failed to read
                                throw new Exception(string.Format("File read error, reading block : {0} and actual bytes read {1} ", i, bytesRead));
                            }
                            
                            //append hash of previous block
                            Buffer.BlockCopy(hashArray[i+1],0, inputBlock, blockSize, hashSize);


                        }

                        var hash = mySHA256.ComputeHash(inputBlock);
                        var hashHex = ByteArrayTohexString(hash);
                        hashArray[i] = new byte[hashSize];

                        Buffer.BlockCopy(hash, 0, hashArray[i],0,hashSize);

                    }

                      // print the hash0
                    var hash0Hex = ByteArrayTohexString(hashArray[0]);

                    Console.WriteLine(hash0Hex);
 
                }
              

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            Console.ReadKey();

        }



        private static string ByteArrayTohexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }


        private static void Test(string fileName)
        {
            var rawEx1 = CreateSpecialByteArray(1024, 0x11);
            var rawEx2 = CreateSpecialByteArray(1024, 0x22);
            var rawEx3 = CreateSpecialByteArray(1024, 0x33);
            var rawEx4 = CreateSpecialByteArray(773, 0x44);

            // To generate the file
            // Create a new stream to write to the file
           var  writer = new BinaryWriter(File.OpenWrite(fileName));

            // Writer raw data                
            writer.Write(rawEx1);
            writer.Write(rawEx2);
            writer.Write(rawEx3);
            writer.Write(rawEx4);

            writer.Flush();
            writer.Close();

          
            // Initialize a SHA256 hash object.
            var mySHA256 = System.Security.Cryptography.SHA256Managed.Create();

            // to get hash4
            var hash4 = mySHA256.ComputeHash(rawEx4);
            var has4Hex = ByteArrayTohexString(hash4);

            var inputblock3 = new byte[1056];
            Buffer.BlockCopy(rawEx3,0,inputblock3,0,1024);
            Buffer.BlockCopy(hash4, 0 , inputblock3,1024, 32);

            // to get hash3
            var hash3 = mySHA256.ComputeHash(inputblock3);
            var has3Hex = ByteArrayTohexString(hash3);
            
            Console.WriteLine(has3Hex);
           

        }

        public static byte[] CreateSpecialByteArray(int length, byte value)
        {
            var arr = new byte[length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
            return arr;
        }

    }
}
