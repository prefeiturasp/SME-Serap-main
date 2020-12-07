using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace AtualizarCriptografiaDosUsuarios
{
    /// <summary>
    /// Summary description for Criptografia.
    /// </summary>
    public class Criptografia
    {
        private static byte[] tdesKey = new byte[] { 107, 8, 82, 60, 113, 135, 190, 128, 188, 51, 238, 120, 59, 135, 57, 140, 107, 8, 82, 60, 113, 135, 190, 128 };
        private static byte[] tdesIV = new byte[] { 113, 135, 190, 128, 186, 217, 34, 47 };

        public static string CriptografarSenhaSHA512(string senha)
        {
            byte[] senhaByte = Encoding.Unicode.GetBytes(senha);

            SHA512 sha512 = SHA512.Create();

            string pwd = Convert.ToBase64String(sha512.ComputeHash(senhaByte));

            return pwd.TrimStart('/');
        }

        /// <summary>
        /// Compara duas senhas - Uma que já está criptografada e outra não
        /// </summary>
        /// <param name="senha1">Senha digitada pelo usuário</param>
        /// <param name="senha2">Senha já criptografada do BD</param>
        /// <param name="tipo">1 = TripleDES / 3 = SHA512</param>
        /// <returns>true caso sejam iguais as senhas</returns>
        public static bool EqualsSenha(string senha1, string senha2, byte? tipo)
        {
            if (tipo == 1)
                return CriptografarSenhaTripleDES(senha1).Equals(senha2);
            else
                return CriptografarSenhaSHA512(senha1).Equals(senha2);
        }

        public static string CriptografarSenhaTripleDES(string senha)
        {
            byte[] plainByte = ASCIIEncoding.ASCII.GetBytes(senha);
            MemoryStream ms = new MemoryStream();
            SymmetricAlgorithm sym = TripleDES.Create();
            CryptoStream encStream = new CryptoStream(ms, sym.CreateEncryptor(tdesKey, tdesIV), CryptoStreamMode.Write);
            encStream.Write(plainByte, 0, plainByte.Length);
            encStream.FlushFinalBlock();
            byte[] cryptoByte = ms.ToArray();
            return Convert.ToBase64String(cryptoByte);
        }

        public static string DescriptografarSenhaTripleDES(string senha)
        {
            byte[] cryptoByte = Convert.FromBase64String(senha);
            var sym = TripleDES.Create();
            MemoryStream ms = new MemoryStream(cryptoByte, 0, cryptoByte.Length);
            CryptoStream cs = new CryptoStream(ms, sym.CreateDecryptor(tdesKey, tdesIV), CryptoStreamMode.Read);
            var ret = _ReadBytes(cs);
            return ASCIIEncoding.ASCII.GetString(ret);
        }

        private static byte[] _ReadBytes(Stream s)
        {
            int length = 10000000;
            byte[] buffer = new byte[length];
            int bytesLidos = length;
            using (MemoryStream ms = new MemoryStream())
            {
                while (bytesLidos == length)
                {
                    bytesLidos = s.Read(buffer, 0, length);
                    ms.Write(buffer, 0, bytesLidos);
                }

                return ms.ToArray();
            }
        }

    }
}
