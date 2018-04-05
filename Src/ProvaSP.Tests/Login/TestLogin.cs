using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProvaSP.Model.Entidades;
using ProvaSP.Data;

namespace ProvaSP.Tests.Login
{
    [TestClass]
    public class TestLogin
    {
        [TestMethod]
        public void LoginUsuarioSemSenha()
        {
            string usu_login = "5477671";
            string usu_senha = "";

            Usuario usuario = DataUsuario.RetornarUsuario(usu_login, usu_senha);

            Assert.AreEqual(usu_login, usuario.usu_login);

        }
    }
}
