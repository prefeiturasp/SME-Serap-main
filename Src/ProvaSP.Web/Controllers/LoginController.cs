using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Http;
using ProvaSP.Model.Entidades;
using System.Globalization;
using ProvaSP.Data;

namespace ProvaSP.Web.Controllers
{
    public class LoginController : ApiController
    {
        
        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection formData)
        {
            string usu_login = formData["usu_login"].ToString();
            string usu_senha = formData["usu_senha"].ToString();

            
            if (SHA512_Potencial(usu_login, usu_senha))
            {
                usu_senha = "";
            }

            if (usu_senha!="")
            {
                usu_senha = CriptografarSenhaHex_SHA512(usu_senha);
            }

            var usuario = DataUsuario.RetornarUsuario(usu_login, usu_senha);
            if (usuario!= null && usuario.Aluno)
            {
                var dadosAluno = DataAluno.GetAluno("2018", usu_login.ToUpper().Replace("RA", ""));
                if (dadosAluno != null)
                {
                    usuario.Turma = dadosAluno.tur_codigo;
                    usuario.Ano = dadosAluno.AnoEscolar;
                }
            }
            var usuarioJson = Newtonsoft.Json.JsonConvert.SerializeObject(usuario);

            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(usuarioJson, Encoding.UTF8, "application/json");
            return response;
        }

        /*
        public string Get(string usu_login, string usu_senha)
        {
            return SHA512_Potencial(usu_login,usu_senha).ToString();
        }
        */

        private bool SHA512_Potencial(string p1, string p2)
        {
            string potencial = CriptografarSenhaSHA512(p1.Substring(p1.Length - 4, 4));
            potencial = CriptografarSenhaSHA512_Hex(potencial);
            return potencial == p2;
        }

        private string CriptografarSenhaSHA512(string senha)
        {
            try
            {
                byte[] senhaByte = System.Text.Encoding.Unicode.GetBytes(senha);

                System.Security.Cryptography.SHA512 sha512 = new System.Security.Cryptography.SHA512Managed();

                string pwd = Convert.ToBase64String(sha512.ComputeHash(senhaByte));

                return pwd.TrimStart('/');
            }
            catch
            {
                throw;
            }
        }

        private string CriptografarSenhaHex_SHA512(string senha)
        {
            try
            {
                byte[] senhaByte = ConvertHexStringToByteArray(senha);
                
                string pwd = Convert.ToBase64String(senhaByte);

                return pwd.TrimStart('/');
            }
            catch
            {
                throw;
            }
        }



        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new Exception(String.Format("The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] HexAsBytes = new byte[hexString.Length / 2];
            for (int index = 0; index < HexAsBytes.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                HexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return HexAsBytes;
        }




        private string CriptografarSenhaSHA512_Hex(string usu_senha)
        {
                byte[] senhaArBytes = null;
                for (int i = 1; i <= 10; i++)
                {
                    try
                    {
                        senhaArBytes = Convert.FromBase64String(usu_senha);
                        i = 10;
                    }
                    catch
                    {
                        usu_senha = "/" + usu_senha;
                    }
                }


                var sbSenhaHex = new StringBuilder();
                foreach (byte b in senhaArBytes)
                {
                    sbSenhaHex.Append(String.Format("{0:x2}", b));
                }
                usu_senha = sbSenhaHex.ToString();
                return usu_senha;
                
        }
    }
}
