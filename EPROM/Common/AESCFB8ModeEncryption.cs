using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class AESCFB8ModeEncryption
    {
        byte[] savedKey = new byte[16];
        byte[] savedIV = new byte[16];
        byte[] CipherBytes;
        byte[] plainBytes;

        public AESCFB8ModeEncryption()
        {
            //Both Keys must be of 16 Characters.
            savedKey = "0123456789ABCDEF".ToList<char>().Select(t => Convert.ToByte(t)).ToArray();
            savedIV = "0123456789ABCDEF".ToList<char>().Select(t => Convert.ToByte(t)).ToArray();
        }

        public string Encrypt(string PlainText)
        {
            try
            {
                plainBytes = Encoding.UTF8.GetBytes(PlainText);
                using (RijndaelManaged Aes128 = new RijndaelManaged())
                {
                    //
                    // Specify a blocksize of 128, and a key size of 128, which make this
                    // instance of RijndaelManaged an instance of AES 128.
                    //
                    Aes128.BlockSize = 128;
                    Aes128.KeySize = 128;

                    //
                    // Specify CFB8 mode
                    //
                    Aes128.Mode = CipherMode.CFB;
                    Aes128.FeedbackSize = 8;
                    Aes128.Padding = PaddingMode.None;
                    //
                    // Generate and save random key and IV.
                    //

                    //Aes128.GenerateKey();
                    //Aes128.GenerateIV();

                    //Aes128.Key.CopyTo(savedKey, 0);
                    //Aes128.IV.CopyTo(savedIV, 0);

                    Aes128.Key = savedKey;
                    Aes128.IV = savedIV;

                    using (var encryptor = Aes128.CreateEncryptor())
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            BinaryWriter bw = null;
                            try
                            {
                                bw = new BinaryWriter(csEncrypt, Encoding.UTF8);
                                bw.Write(plainBytes);
                                CipherBytes = msEncrypt.ToArray();
                                return BitConverter.ToString(CipherBytes).Replace("-", "");
                            }
                            finally
                            {
                                if (bw != null)
                                    bw.Dispose();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string Decrypt(string CipherText)
        {
            try
            {
                CipherBytes = new byte[CipherText.Length / 2];
                for (int i = 0; i < CipherText.Length; i += 2)
                {
                    CipherBytes[i / 2] = Convert.ToByte((CipherText.ElementAt(i).ToString() + CipherText.ElementAt(i + 1).ToString()), 16);
                }
                using (RijndaelManaged Aes128 = new RijndaelManaged())
                {
                    Aes128.BlockSize = 128;
                    Aes128.KeySize = 128;
                    Aes128.Mode = CipherMode.CFB;
                    Aes128.FeedbackSize = 8;
                    Aes128.Padding = PaddingMode.None;

                    Aes128.Key = savedKey;
                    Aes128.IV = savedIV;

                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = Aes128.CreateDecryptor();

                    // Create the streams used for decryption. 
                    using (MemoryStream msEncrypt = new MemoryStream(CipherBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
