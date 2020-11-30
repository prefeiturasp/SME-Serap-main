using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AtualizarCriptografiaDosUsuarios
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var usuariosDo4ao6EnsinoFunDamental = GetUsuarios4ao6EnsinoFundamental();
            var scriptSqlDo4ao6 = GerarScriptDosUsuarios(usuariosDo4ao6EnsinoFunDamental);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var usuariosDo7ao9EnsinoFunDamental = GetUsuarios7ao9EnsinoFundamental();
            var scriptSqlDo7ao9 = GerarScriptDosUsuarios(usuariosDo7ao9EnsinoFunDamental);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var usuarios = GetUsuariosGeral();
            var scriptsSql = GerarScriptDosUsuarios(usuarios);
        }

        private string GerarScriptDosUsuarios(List<Usuario> usuarios)
        {
            foreach (var usuario in usuarios)
            {
                var matricula = usuario.Matricula.Trim();
                var senhaDescriptografada = matricula.Substring(matricula.Length - 4);
                if (Criptografia.EqualsSenha(senhaDescriptografada, usuario.SenhaAtual, 1)) continue;
                usuario.NovaSenha = Criptografia.CriptografarSenhaTripleDES(senhaDescriptografada);
                usuario.AtualizarSenha = true;
            }
            var count = usuarios.Where(s => s.AtualizarSenha).Count();

            var sql = new StringBuilder();
            foreach (var usuario in usuarios.Where(s => s.AtualizarSenha).ToList())
            {
                sql.AppendLine($"UPDATE CoreSSO..Sys_Usuario SET usu_senha = '{usuario.NovaSenha}', usu_dataAlteracaoSenha = GETDATE() WHERE usu_id = '{usuario.UsuarioId}';");
            }

            return sql.ToString();
        }

        private List<Usuario> GetUsuarios7ao9EnsinoFundamental()
        {
            using (var connCoreSSO = new SqlConnection(Decrypt(ConfigurationManager.ConnectionStrings["CoreSSO"].ConnectionString)))
            {
                connCoreSSO.Open();
                var sqlQuery = new StringBuilder();
                var listaDeUsuarios = new List<Usuario>();
                sqlQuery.AppendLine(@"
                    SELECT 
	                    usuario.usu_id UsuarioId, 
	                    aluno.alu_matricula Matricula,
	                    usuario.usu_senha SenhaAtual
                    FROM CoreSSO..Sys_Usuario usuario
                    INNER JOIN GestaoAvaliacao_SGP..Aca_Aluno aluno ON aluno.pes_id = usuario.pes_id
                    INNER JOIN GestaoAvaliacao_SGP..AlunosETurmasDo7Ao9EnsinoFundamental DePara ON dePara.cd_aluno = aluno.alu_matricula
                    Where usuario.usu_criptografia = 1
                    ORDER BY usu_dataCriacao DESC
                ");
                var commandCoreSSO = new SqlCommand(sqlQuery.ToString(), connCoreSSO);
                using (var drUsuarios = commandCoreSSO.ExecuteReader())
                {
                    var dtResultado = new DataTable();
                    dtResultado.Load(drUsuarios);
                    listaDeUsuarios.AddRange(dtResultado.AsEnumerable().Select(linha => new Usuario
                    {
                        UsuarioId = linha.Field<Guid>("UsuarioId"),
                        Matricula = linha.Field<String>("Matricula"),
                        SenhaAtual = linha.Field<String>("SenhaAtual")
                    }).ToList());
                }
                return listaDeUsuarios;
            }
        }

        private List<Usuario> GetUsuarios4ao6EnsinoFundamental()
        {
            using (var connCoreSSO = new SqlConnection(Decrypt(ConfigurationManager.ConnectionStrings["CoreSSO"].ConnectionString)))
            {
                connCoreSSO.Open();
                var sqlQuery = new StringBuilder();
                var listaDeUsuarios = new List<Usuario>();
                sqlQuery.AppendLine(@"
                    SELECT 
	                    usuario.usu_id UsuarioId, 
	                    aluno.alu_matricula Matricula,
	                    usuario.usu_senha SenhaAtual
                    FROM CoreSSO..Sys_Usuario usuario
                    INNER JOIN GestaoAvaliacao_SGP..Aca_Aluno aluno ON aluno.pes_id = usuario.pes_id
                    INNER JOIN GestaoAvaliacao_SGP..AlunosETurmasDePara DePara ON dePara.cd_aluno = aluno.alu_matricula
                    Where usuario.usu_criptografia = 1
                    ORDER BY usu_dataCriacao DESC
                ");

                var commandCoreSSO = new SqlCommand(sqlQuery.ToString(), connCoreSSO);
                using (var drUsuarios = commandCoreSSO.ExecuteReader())
                {
                    var dtResultado = new DataTable();
                    dtResultado.Load(drUsuarios);
                    listaDeUsuarios.AddRange(dtResultado.AsEnumerable().Select(linha => new Usuario
                    {
                        UsuarioId = linha.Field<Guid>("UsuarioId"),
                        Matricula = linha.Field<String>("Matricula"),
                        SenhaAtual = linha.Field<String>("SenhaAtual")
                    }).ToList());
                }
                return listaDeUsuarios;
            }
        }

        private List<Usuario> GetUsuariosGeral()
        {
            using (var connCoreSSO = new SqlConnection(Decrypt(ConfigurationManager.ConnectionStrings["CoreSSO"].ConnectionString)))
            {
                connCoreSSO.Open();
                var sqlQuery = new StringBuilder();
                var listaDeUsuarios = new List<Usuario>();
                var dataAtual = "2020-11-27 00:00:00";

                sqlQuery.AppendLine($@"
                    SELECT 
	                    usuario.usu_id UsuarioId, 
	                    aluno.alu_matricula Matricula,
	                    usuario.usu_senha SenhaAtual
                    FROM CoreSSO..Sys_Usuario  usuario
                    INNER JOIN GestaoAvaliacao_SGP..Aca_Aluno aluno ON aluno.pes_id = usuario.pes_id
                    WHERE 
	                    LTRIM(RTRIM(usuario.usu_login)) like '%RA%' AND 
	                    (usu_dataAlteracao>='{dataAtual}' or 
	                    usu_dataAlteracaoSenha>='{dataAtual}')
                    ORDER BY usu_dataCriacao desc
                ");

                var commandCoreSSO = new SqlCommand(sqlQuery.ToString(), connCoreSSO);
                using (var drUsuarios = commandCoreSSO.ExecuteReader())
                {
                    var dtResultado = new DataTable();
                    dtResultado.Load(drUsuarios);
                    listaDeUsuarios.AddRange(dtResultado.AsEnumerable().Select(linha => new Usuario
                    {
                        UsuarioId = linha.Field<Guid>("UsuarioId"),
                        Matricula = linha.Field<String>("Matricula"),
                        SenhaAtual = linha.Field<String>("SenhaAtual")
                    }).ToList());
                }
                return listaDeUsuarios;
            }
        }

        private string Decrypt(string value)
        {
            var cripto = new MSTech.Security.Cryptography.SymmetricAlgorithm(MSTech.Security.Cryptography.SymmetricAlgorithm.Tipo.TripleDES);
            return cripto.Decrypt(value);
        }
    }
    public class Usuario
    {
        public Guid UsuarioId { get; set; }
        public string Matricula { get; set; }
        public string SenhaAtual { get; set; }
        public string NovaSenha { get; set; }
        public bool AtualizarSenha { get; set; }
    }
}
