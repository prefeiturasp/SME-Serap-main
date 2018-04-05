using Dapper;
using ProvaSP.Data.Funcionalidades;
using ProvaSP.Model.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProvaSP.Data
{
    public static class DataUsuario
    {
        public static List<Usuario> RetornarBaseUsuarioOffline()
        {
            Console.WriteLine("Recuperando base de logins offline... (aguarde aproximadamente 1 minuto)");

            var baseUsuario = new List<Usuario>();

            using (var conn = new SqlConnection(StringsConexao.CoreSSO))
            {

                var listaUsuarioVsGrupos = new List<Usuario>();

                /*
                EXEMPLO DE RETORNO:

                usu_id	                                usu_login	usu_senha	                                                                                usu_criptografia	Split	gru_id	                                gru_nome	        uad_sigla	esc_codigo
                ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                FE26694E-12F2-E111-A89D-00155D02E702	1170155	    Cu9SQ3zRJhwMXojFJrNHcUYE6cSrVWV/Q4G43Aj78EwyQzLgfFfWSkZ1I3PtH2pdNCu/7Oxkb6XGRKERYwtuDQ==	3	                split	067D9B21-A1FF-E611-9541-782BCB3D218E	Professor	        BT	        092703
                03B413F3-27B6-E111-B597-00155D02E702	1180461	    eD+nwy16XTiL722+Vs79N2rtVs9xiRbEKKajIbxPUQc8ErmH1fk2ZD6oGGGCmy20BPwzGzgXFdKudRt99jyaRg==	3	                split	26552002-FD66-4D63-9FA7-E9B3993D110D	Diretor Escolar	    SA	        093173
                F805A688-8A9D-E611-9541-782BCB3D218E	1180827	    U9yatFRqrEEWQDUruwn6NjJ4Xd/W9d2UDbakuZIp1JEVDP0F4QW5+q2DtHeR5sed8B4tlqKtxRbIXDOZt9qOXg==	3	                split	A0B86A81-F233-E711-9541-782BCB3D218E	Supervisão Escolar	JT	
                */
                conn.Query<Usuario, Grupo, Usuario>(
                            @"
                                SELECT
                                    CAST(u.usu_id AS varchar(40)) AS usu_id, u.usu_login, u.usu_senha, u.usu_criptografia,
                                    'split' AS Split,
                                    CAST(g.gru_id AS varchar(40)) AS gru_id,
                                    g.gru_nome,
                                    CASE
                                        WHEN g.gru_id IN(@AdmSerapDRE, @AdministradorSerapDRE, @TecnicoDRE, @Supervisor) THEN /* uad_sigla COM BASE NA RELAÇÃO DIRETA DA PESSOA COM A DRE */
                                            (
                                                SELECT TOP 1 uad_sigla
                                                FROM SYS_UnidadeAdministrativa
                                                WHERE uad_id IN(
                                                        SELECT TOP 1 uad_id
                                                        FROM SYS_UsuarioGrupoUA
                                                        WHERE gru_id=UG.gru_id AND usu_id=u.usu_id)
                                            )
                                        WHEN g.gru_id IN(@Diretor, @Coordenador, @Professor) THEN /* uad_sigla COM BASE NA ESCOLA ASSOCIADA À PESSOA */
                                            (
                                                SELECT TOP 1 u_n4.uad_sigla /* <-CÓDIGO DA DRE */
				                                FROM SYS_UnidadeAdministrativa u_n1
				                                JOIN SYS_UnidadeAdministrativa u_n2 ON u_n2.uad_id = u_n1.uad_idSuperior
				                                JOIN SYS_UnidadeAdministrativa u_n3 ON u_n3.uad_id = u_n2.uad_idSuperior
				                                JOIN SYS_UnidadeAdministrativa u_n4 ON u_n4.uad_id = u_n3.uad_idSuperior
				                                WHERE u_n1.uad_codigo=	(
											                                SELECT TOP 1 uad_codigo /* <-CÓDIGO DA ESCOLA */
											                                FROM SYS_UnidadeAdministrativa ua_n1
											                                WHERE uad_id IN(
													                                SELECT TOP 1 uad_id
													                                FROM SYS_UsuarioGrupoUA
											                                WHERE gru_id=g.gru_id AND usu_id=u.usu_id)
										                                )
                                            )
                                    END AS uad_sigla,
                                    CASE
                                        WHEN g.gru_id IN(@Diretor, @Coordenador, @Professor) THEN
			                                (
				                                SELECT TOP 1 uad_codigo /* <-CÓDIGO DA ESCOLA */
				                                FROM SYS_UnidadeAdministrativa ua_n1
				                                WHERE uad_id IN(
						                                SELECT TOP 1 uad_id
						                                FROM SYS_UsuarioGrupoUA
				                                WHERE gru_id=g.gru_id AND usu_id=u.usu_id)
			                                )
                                        ELSE ''
                                    END AS esc_codigo
                                FROM SYS_Usuario u
                                JOIN SYS_UsuarioGrupo ug ON u.usu_id = ug.usu_id
                                JOIN SYS_Grupo g ON g.gru_id = ug.gru_id
                                WHERE u.usu_situacao=1 AND g.gru_situacao=1 AND ug.usg_situacao=1 AND sis_id=204 AND g.gru_id IN(
                                                                                        @Administrador, @AdministradorNTA, @AdmSerapCOPEDLeitura, @GestaoSERApLeitura, --NÍVEL SME:
                                                                                        @AdmSerapDRE, @AdministradorSerapDRE, @TecnicoDRE, @Supervisor, --NÍVEL DRE
                                                                                        @Diretor, @Coordenador, @Professor /* NÍVEL ESCOLA */)

                                ORDER BY u.usu_login /* ORDENAÇÃO EM usu_login PARA PERMITIR PESQUISA BINÁRIA Θ(log2 n) NO PROCESSO DE LOGIN OFFLINE */
                                ",
                        map: (_usuarioEntry, _grupoEntry) =>
                        {
                            bool novoUsuario = true;
                            Usuario usuarioRetorno = null;  
                            if (listaUsuarioVsGrupos.Any(x => x.usu_id == ""))
                            {
                                novoUsuario = false;
                                usuarioRetorno = listaUsuarioVsGrupos.First(x => x.usu_id == "");
                            }
                            else
                                usuarioRetorno = _usuarioEntry;
                            

                            if (_grupoEntry != null)
                            {
                                usuarioRetorno.grupos.Add(_grupoEntry);
                            }

                            if (novoUsuario)
                            {
                                listaUsuarioVsGrupos.Add(usuarioRetorno);
                            }

                            return _usuarioEntry;
                        },
                        splitOn: "Split",
                        param: new
                        {
                            Administrador = new DbString() { Value = Grupo.Perfil.Administrador, IsAnsi = true, Length = 40 },
                            AdministradorNTA = new DbString() { Value = Grupo.Perfil.AdministradorNTA, IsAnsi = true, Length = 40 },
                            AdmSerapCOPEDLeitura = new DbString() { Value = Grupo.Perfil.AdmSerapCOPEDLeitura, IsAnsi = true, Length = 40 },
                            GestaoSERApLeitura = new DbString() { Value = Grupo.Perfil.GestaoSERApLeitura, IsAnsi = true, Length = 40 },
                            AdmSerapDRE = new DbString() { Value = Grupo.Perfil.AdmSerapDRE, IsAnsi = true, Length = 40 },
                            AdministradorSerapDRE = new DbString() { Value = Grupo.Perfil.AdministradorSerapDRE, IsAnsi = true, Length = 40 },
                            TecnicoDRE = new DbString() { Value = Grupo.Perfil.TecnicoDRE, IsAnsi = true, Length = 40 },
                            Supervisor = new DbString() { Value = Grupo.Perfil.Supervisor, IsAnsi = true, Length = 40 },
                            Diretor = new DbString() { Value = Grupo.Perfil.Diretor, IsAnsi = true, Length = 40 },
                            Coordenador = new DbString() { Value = Grupo.Perfil.Coordenador, IsAnsi = true, Length = 40 },
                            Professor = new DbString() { Value = Grupo.Perfil.Professor, IsAnsi = true, Length = 40 }
                        });


                int iLinha = 0;
                int iTotalLinhas = listaUsuarioVsGrupos.Count(); //dt.Rows.Count;

                float totalProcessado = 0;
                float totalProcessadoAnterior = -1;

                //foreach (DataRow drow in dt.Rows)
                foreach(Usuario usuario in listaUsuarioVsGrupos)
                {
                    iLinha++;

                    totalProcessado = iLinha * 100 / iTotalLinhas;
                    Console.WriteLine(string.Format("Total processado: {0}% - {1} de {2}", iLinha * 100 / iTotalLinhas, iLinha, iTotalLinhas));
                    totalProcessadoAnterior = totalProcessado;

                    string usu_senha = usuario.usu_senha;
                    string usu_criptografia = usuario.usu_criptografia;

                    if (usu_criptografia.Equals("3"))
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
                    }
                    else if (usu_criptografia.Equals("1"))
                    {
                        try
                        {
                            usu_senha = new MSTech.Security.Cryptography.SymmetricAlgorithm(MSTech.Security.Cryptography.SymmetricAlgorithm.Tipo.TripleDES).Decrypt(usu_senha);
                            var ue = new UnicodeEncoding();
                            byte[] senhaArBytes = ue.GetBytes(usu_senha);
                            var encriptador = new SHA512Managed();
                            senhaArBytes = encriptador.ComputeHash(senhaArBytes);
                            usu_senha = arrayBytesParaHashHexString(senhaArBytes);
                        }
                        catch
                        {
                            //throw new Exception(string.Format("Não foi possível descriptografar a senha do usuário {0}", usuario.usu_id));
                        }
                    }

                    int usu_login_int;
                    if (Int32.TryParse(usuario.usu_login, out usu_login_int))
                    {
                        usuario.usu_login = usu_login_int.ToString("X");
                    }

                    usuario.usu_senha = usu_senha.Substring(0,4); //Redução do tamanho do HASH

                    baseUsuario.Add(usuario);
                }

            }


            return baseUsuario;
        }

        private static string arrayBytesParaHashHexString(byte[] senhaArBytes)
        {
            var sbSenhaHex = new StringBuilder();
            foreach (byte b in senhaArBytes)
            {
                sbSenhaHex.Append(String.Format("{0:x2}", b));
            }
            return sbSenhaHex.ToString();
        }

        public static Usuario RetornarUsuario(string usu_login, string usu_senha, SqlConnection conn = null)
        {
            Usuario usuarioRetorno = null;
            
            try
            {
                bool fecharConexao = true;

                if (conn != null)
                    fecharConexao = false;
                else
                {
                    conn = new SqlConnection(StringsConexao.CoreSSO);
                    conn.Open();
                }

                conn.Query<Usuario, Grupo, Usuario>(
                            @"
                                SELECT
                                    u.usu_login, CAST(u.usu_id AS varchar(40)) AS usu_id,
                                    'split' AS Split,
                                    CAST(g.gru_id AS varchar(40)) AS gru_id,
                                    g.gru_nome
                                FROM SYS_Grupo g
                                LEFT JOIN SYS_UsuarioGrupo ug ON g.gru_id = ug.gru_id
                                LEFT JOIN SYS_Usuario u ON u.usu_id = ug.usu_id
                                WHERE u.usu_login = @usu_login AND (u.usu_senha = @usu_senha OR @usu_senha='') AND u.usu_situacao=1 AND g.gru_situacao=1 AND ug.usg_situacao=1 AND sis_id=204 --SERAp
                                ",
                        map: (_usuarioEntry, _grupoEntry) =>
                        {
                            if (usuarioRetorno == null)
                            {
                                usuarioRetorno = _usuarioEntry;
                            }

                            if (_grupoEntry != null)
                            {
                                usuarioRetorno.grupos.Add(_grupoEntry);
                            }

                            return _usuarioEntry;
                        },
                        splitOn: "Split",
                        param: new
                        {
                            usu_login = new DbString() { Value = usu_login, IsAnsi = true, Length = 500 },
                            usu_senha = new DbString() { Value = usu_senha, IsAnsi = true, Length = 256 }
                        });

                if (fecharConexao)
                {
                    conn.Close();
                    conn.Dispose();
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            
            //ASSOCIAÇÃO DAS DREs E ESCOLAS RELACIONADAS A ESSE USUÁRIO:
            if (usuarioRetorno!=null)
            {
                foreach (var grupo in usuarioRetorno.grupos)
                {
                    if (grupo.AcessoNivelDRE)
                    {
                        var uad_sigla = DataDRE.RecuperarCodigoDRE(grupo.gru_id, usuarioRetorno.usu_id);
                        grupo.uad_sigla = uad_sigla;
                    }
                    else if (grupo.AcessoNivelEscola)
                    {
                        var esc_codigo = DataEscola.RecuperarCodigoEscola(grupo.gru_id, usuarioRetorno.usu_id);
                        grupo.uad_sigla = DataDRE.RecuperarCodigoDRE(esc_codigo);
                        grupo.esc_codigo = esc_codigo;
                    }
                }
            }
            

            return usuarioRetorno;
        }

        public static Usuario RetornarUsuario(string usu_id)
        {
            return RetornarUsuario(usu_id, "");
        }

            
    }
}
