using Dapper;
using GestaoAvaliacao.FGVIntegration.Business;
using GestaoAvaliacao.FGVIntegration.Logging;
using GestaoAvaliacao.FGVIntegration.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace GestaoAvaliacao.FGVIntegration.Data
{
    public class DatabaseRepository : LoggingBase, IDatabaseRepository
    {

        private readonly MSTech.Security.Cryptography.SymmetricAlgorithm cripto;
        private readonly string dbConnectionString;
        private readonly int currentYear;

        public DatabaseRepository()
        {
            cripto = new MSTech.Security.Cryptography.SymmetricAlgorithm(MSTech.Security.Cryptography.SymmetricAlgorithm.Tipo.TripleDES);
            dbConnectionString = ConfigurationHelper.BuscarConnectionString("GestaoAvaliacao_SGP");
            currentYear = DateTime.Today.Year;
        }

        public ICollection<Pessoa> BuscarPessoas(ICollection<string> pCodigosRF)
        {
            try
            {
                using (IDbConnection dbConn = GetConnection())
                {
                    dbConn.Open();
                    var sql = @"
                        SELECT
                            u.usu_login as CodigoRF,
	                        u.usu_email as Email,
	                        p.pes_nome as Nome,
	                        pd.psd_numero as CPF,
	                        (CASE p.pes_sexo WHEN 1 THEN @sexoMasculino WHEN 2 THEN @sexoFeminino ELSE @sexoMasculino END) as Sexo,
	                        p.pes_dataNascimento as DataNascimento
                        FROM CoreSSO.dbo.SYS_Usuario u WITH (NOLOCK)
                          JOIN CoreSSO.dbo.PES_Pessoa p WITH (NOLOCK)                ON p.pes_id = u.pes_id
                          LEFT JOIN CoreSSO.dbo.PES_PessoaDocumento pd WITH (NOLOCK) ON pd.pes_id = p.pes_id AND pd.tdo_id IN(SELECT tdo_id FROM CoreSSO.dbo.SYS_TipoDocumentacao WHERE tdo_sigla = 'cpf')
                        WHERE u.usu_login IN @codigosRF
                          AND u.usu_situacao <> @situacao";

                    var registros = dbConn.Query<Pessoa>(sql, new
                    {
                        codigosRF = pCodigosRF,
                        situacao = (byte)Enums.SituacaoRegistro.EXCLUIDO,
                        sexoMasculino = ((char)Enums.Sexo.MASCULINO).ToString(),
                        sexoFeminino = ((char)Enums.Sexo.FEMININO).ToString(),
                    });
                    return registros.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Erro ao recuperar todas as escolas", ex);
                throw new ApplicationException("Erro ao recuperar todas as escolas", ex);
            }

        }

        public Pessoa BuscarPessoa(string pCodigoRF)
        {
            return BuscarPessoas(new string[] { pCodigoRF }).FirstOrDefault();
        }

        public ICollection<Escola> BuscarEscolas(ICollection<string> pCodigoEscolas)
        {
            try
            {
                using (IDbConnection dbConn = GetConnection())
                {
                    dbConn.Open();
                    var sql = @"
                        SELECT
                            e.esc_codigo as CodigoDaEscola,
	                        e.esc_nome as NomeEscola
                        FROM GestaoAvaliacao_SGP.dbo.ESC_Escola e WITH (NOLOCK) 
                        WHERE e.esc_situacao = @situacao
                          AND e.esc_nome like 'EMEFM%'"
                        + (pCodigoEscolas == null || !pCodigoEscolas.Any() ? string.Empty : "\r\n AND e.esc_codigo IN @escolas")
                        + "\r\n ORDER BY e.esc_nome";

                    var registros = dbConn.Query<Escola>(sql, new
                    {
                        situacao = (byte)Enums.SituacaoRegistro.ATIVO,
                        escolas = (pCodigoEscolas == null || !pCodigoEscolas.Any() ? null : pCodigoEscolas),
                    });
                    return registros.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Erro ao recuperar todas as escolas", ex);
                throw new ApplicationException("Erro ao recuperar todas as escolas", ex);
            }
        }

        public ICollection<Turma> BuscarTurmas(ICollection<Escola> pEscolas)
        {
            try
            {
                using (IDbConnection dbConn = GetConnection())
                {
                    dbConn.Open();
                    var sql = @"
                        SELECT
                            e.esc_codigo as CodigoDaEscola,
	                        SUBSTRING(t.tur_codigo, 4, 1) as Serie,
	                        t.tur_id as CodigoDaTurma,
	                        t.tur_codigo as NomeDaTurma,
	                        CASE tt.ttn_id
		                        WHEN 6 THEN @turnoManha --Manhã
		                        WHEN 5 THEN @turnoTarde --Tarde
		                        WHEN 2 THEN @turnoTarde --Vespertino = Tarde
		                        WHEN 3 THEN @turnoNoite --Noite
		                        ELSE @turnoManha --Integral/Intermediário = Manhã
	                        END as Turno
                        FROM GestaoAvaliacao_SGP.dbo.TUR_Turma t WITH (NOLOCK)
                          JOIN GestaoAvaliacao_SGP.dbo.ACA_CalendarioAnual ca WITH (NOLOCK) 	ON ca.cal_id = t.cal_id
                          JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaCurriculo tc WITH (NOLOCK) 	    ON tc.tur_id = t.tur_id
                          JOIN GestaoAvaliacao_SGP.dbo.ACA_TipoCurriculoPeriodo tcp WITH (NOLOCK) ON tcp.tcp_id = tc.tcp_id
                          JOIN GestaoAvaliacao_SGP.dbo.ESC_Escola e WITH (NOLOCK) 			    ON t.esc_id = e.esc_id
                          JOIN GestaoAvaliacao_SGP.dbo.ACA_TipoTurno tt WITH (NOLOCK) 		    ON tt.ttn_id = t.ttn_id
                        WHERE t.tur_situacao <> @situacao
                          AND ca.cal_ano = @ano
                          AND tcp.tcp_id IN (38, 39, 40) --1, 2 e 3 séries
                          AND tcp.tne_id IN (@tipoEnsinoMedio) --Ensino Médio"
                        + (pEscolas == null || !pEscolas.Any() ? string.Empty : "\r\n AND e.esc_codigo IN @escolas")
                        + "\r\n ORDER BY e.esc_nome, t.tur_codigo";

                    var registros = dbConn.Query<Turma>(sql, new
                    {
                        turnoManha = ((char)Enums.Turno.MANHA).ToString(),
                        turnoTarde = ((char)Enums.Turno.TARDE).ToString(),
                        turnoNoite = ((char)Enums.Turno.NOITE).ToString(),
                        situacao = (byte)Enums.SituacaoRegistro.EXCLUIDO,
                        ano = currentYear,
                        tipoEnsinoMedio = (byte)Enums.TipoNivelEnsino.ENSINO_MEDIO,
                        escolas = (pEscolas == null || !pEscolas.Any() ? null : pEscolas.Select(e => e.CodigoDaEscola)),
                    });
                    return registros.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Erro ao recuperar todas as turmas", ex);
                throw new ApplicationException("Erro ao recuperar todas as turmas", ex);
            }
        }

        public ICollection<Professor> BuscarProfessores(ICollection<Escola> pEscolas)
        {
            try
            {
                using (IDbConnection dbConn = GetConnection())
                {
                    dbConn.Open();
                    var sql = @"
                        SELECT DISTINCT
                            e.esc_codigo as CodigoDaEscola,
	                        e.esc_nome as NomeEscola,
	                        p.pes_id as IdDoProfessor,
                            p.pes_nome as NomeDoProfessor,
	                        --u.usu_email as EmailDoProfessor,
                            convert(char(36), p.pes_id) + '@sme.prefeitura.sp.gov.br' as EmailDoProfessor,
	                        p.pes_nome as Nome,
	                        pd.psd_numero as CPF,
	                        (CASE p.pes_sexo WHEN 1 THEN @sexoMasculino WHEN 2 THEN @sexoFeminino ELSE @sexoMasculino END) as Sexo,
	                        p.pes_dataNascimento as DataNascimento
                        FROM GestaoAvaliacao_SGP.dbo.ACA_Docente d WITH (NOLOCK)
                          JOIN CoreSSO.dbo.PES_Pessoa p WITH (NOLOCK) 						    ON p.pes_id = d.pes_id
                          JOIN CoreSSO.dbo.SYS_Usuario u WITH (NOLOCK) 						    ON p.pes_id = u.pes_id
                          JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaDocente td WITH (NOLOCK) 		ON td.doc_id = d.doc_id
                          JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaDisciplina tud WITH (NOLOCK) 	ON tud.tud_id = td.tud_id
                          JOIN GestaoAvaliacao_SGP.dbo.TUR_Turma t WITH (NOLOCK) 				ON t.tur_id = tud.tur_id 
                          JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaCurriculo tc WITH (NOLOCK) 	    ON tc.tur_id = t.tur_id
                          JOIN GestaoAvaliacao_SGP.dbo.ACA_TipoCurriculoPeriodo tcp WITH (NOLOCK) ON tcp.tcp_id = tc.tcp_id
                          JOIN GestaoAvaliacao_SGP.dbo.ESC_Escola e WITH (NOLOCK) 			    ON t.esc_id = e.esc_id 
                          JOIN GestaoAvaliacao_SGP.dbo.ACA_CalendarioAnual ca WITH (NOLOCK) 	ON ca.cal_id = t.cal_id
                          LEFT JOIN CoreSSO.dbo.PES_PessoaDocumento pd WITH (NOLOCK) 			ON pd.pes_id = p.pes_id AND pd.tdo_id IN (SELECT tdo_id FROM CoreSSO.dbo.SYS_TipoDocumentacao WHERE tdo_sigla = 'cpf')
                        WHERE d.doc_situacao <> @situacao
                          AND td.tdt_situacao  <> @situacao
                          AND tud.tud_situacao <> @situacao
                          AND t.tur_situacao <> @situacao
                          AND ca.cal_ano = @ano
                          AND tcp.tcp_id IN (38, 39, 40) --1, 2 e 3 séries
                          AND tcp.tne_id IN (@tipoEnsinoMedio)"
                        + (pEscolas == null || !pEscolas.Any() ? string.Empty : "\r\n AND e.esc_codigo IN @escolas")
                        + "\r\n ORDER BY e.esc_nome, p.pes_nome";

                    var registros = dbConn.Query<Professor>(sql, new
                    {
                        sexoMasculino = ((char)Enums.Sexo.MASCULINO).ToString(),
                        sexoFeminino = ((char)Enums.Sexo.FEMININO).ToString(),
                        situacao = (byte)Enums.SituacaoRegistro.EXCLUIDO,
                        ano = currentYear,
                        tipoEnsinoMedio = (byte)Enums.TipoNivelEnsino.ENSINO_MEDIO,
                        escolas = (pEscolas == null || !pEscolas.Any() ? null : pEscolas.Select(e => e.CodigoDaEscola)),
                    });
                    return registros.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Erro ao recuperar todos os professores", ex);
                throw new ApplicationException("Erro ao recuperar todos os professores", ex);
            }
        }

        public ICollection<ProfessorTurma> BuscarProfessoresTurmas(ICollection<Escola> pEscolas)
        {
            try
            {
                using (IDbConnection dbConn = GetConnection())
                {
                    dbConn.Open();
                    var sql = @"
                        SELECT
	                        --u.usu_email as EmailDoProfessor,
                            --convert(char(36), p.pes_id) + '@sme.prefeitura.sp.gov.br' as EmailDoProfessor,
	                        p.pes_nome as NomeDoProfessor,
	                        t.tur_id as CodigoDaTurma,
	                        t.tur_codigo as NomeDaTurma,
	                        (CASE tud.tud_nome 
	                          WHEN 'Arte' THEN @discArt
	                          WHEN 'Biologia' THEN @discBio
	                          WHEN 'Filosofia' THEN @discFil
	                          WHEN 'Física' THEN @discFis
	                          WHEN 'Geografia' THEN @discGeo
	                          WHEN 'História' THEN @discHis
	                          WHEN 'Língua espanhola' THEN @discEsp
	                          WHEN 'Língua inglesa' THEN @discIng
	                          WHEN 'Língua portuguesa' THEN @discPort
	                          WHEN 'Matemática' THEN @discMat
	                          WHEN 'Química' THEN @discQui
	                          WHEN 'Sociologia' THEN @discSoc
	                        END) as Disciplina
                        FROM GestaoAvaliacao_SGP.dbo.ACA_Docente d WITH (NOLOCK)
                          JOIN CoreSSO.dbo.PES_Pessoa p WITH (NOLOCK) 						    ON p.pes_id = d.pes_id
                          JOIN CoreSSO.dbo.SYS_Usuario u WITH (NOLOCK) 						    ON p.pes_id = u.pes_id
                          JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaDocente td WITH (NOLOCK) 		ON td.doc_id = d.doc_id
                          JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaDisciplina tud WITH (NOLOCK) 	ON tud.tud_id = td.tud_id
                          JOIN GestaoAvaliacao_SGP.dbo.TUR_Turma t WITH (NOLOCK) 				ON t.tur_id = tud.tur_id 
                          JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaCurriculo tc WITH (NOLOCK) 	    ON tc.tur_id = t.tur_id
                          JOIN GestaoAvaliacao_SGP.dbo.ACA_TipoCurriculoPeriodo tcp WITH (NOLOCK) ON tcp.tcp_id = tc.tcp_id
                          JOIN GestaoAvaliacao_SGP.dbo.ESC_Escola e WITH (NOLOCK) 			    ON t.esc_id = e.esc_id 
                          JOIN GestaoAvaliacao_SGP.dbo.ACA_CalendarioAnual ca WITH (NOLOCK) 	ON ca.cal_id = t.cal_id
                        WHERE d.doc_situacao <> @situacao
                          AND td.tdt_situacao  <> @situacao
                          AND tud.tud_situacao <> @situacao
                          AND t.tur_situacao <> @situacao
                          AND ca.cal_ano = @ano
                          AND tcp.tcp_id IN (38, 39, 40) --1, 2 e 3 séries
                          AND tcp.tne_id IN (@tipoEnsinoMedio)"
                        + (pEscolas == null || !pEscolas.Any() ? string.Empty : "\r\n AND e.esc_codigo IN @escolas")
                        + "\r\n ORDER BY e.esc_nome, p.pes_nome, t.tur_codigo";

                    var registros = dbConn.Query<ProfessorTurma>(sql, new
                    {
                        discArt = Enums.Disciplina.ART.ToString(),
                        discBio = Enums.Disciplina.BIO.ToString(),
                        discFil = Enums.Disciplina.FIL.ToString(),
                        discFis = Enums.Disciplina.FIS.ToString(),
                        discGeo = Enums.Disciplina.GEO.ToString(),
                        discHis = Enums.Disciplina.HIS.ToString(),
                        discEsp = Enums.Disciplina.ESP.ToString(),
                        discIng = Enums.Disciplina.ING.ToString(),
                        discPort = Enums.Disciplina.PORT.ToString(),
                        discMat = Enums.Disciplina.MAT.ToString(),
                        discQui = Enums.Disciplina.QUI.ToString(),
                        discSoc = Enums.Disciplina.SOC.ToString(),
                        situacao = (byte)Enums.SituacaoRegistro.EXCLUIDO,
                        ano = currentYear,
                        tipoEnsinoMedio = (byte)Enums.TipoNivelEnsino.ENSINO_MEDIO,
                        escolas = (pEscolas == null || !pEscolas.Any() ? null : pEscolas.Select(e => e.CodigoDaEscola)),
                    });
                    return registros.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Erro ao recuperar todas as turmas dos professores", ex);
                throw new ApplicationException("Erro ao recuperar todas as turmas dos professores", ex);
            }
        }

        public ICollection<Aluno> BuscarAlunos(ICollection<Escola> pEscolas)
        {
            try
            {
                using (IDbConnection dbConn = GetConnection())
                {
                    dbConn.Open();
                    var sql = @"
                        SELECT DISTINCT
                            e.esc_codigo as CodigoDaEscola,
	                        e.esc_nome as NomeEscola,
	                        p.pes_id IdDoAluno,
	                        --u.usu_email as EmailDoAluno,
	                        p.pes_nome as Nome,
	                        pd.psd_numero as CPF,
	                        null as RG,
	                        (CASE p.pes_sexo WHEN 1 THEN @sexoMasculino WHEN 2 THEN @sexoFeminino ELSE @sexoMasculino END) as Sexo,
	                        p.pes_dataNascimento as DataNascimento,
	                        t.tur_id as CodigoDaTurma,
	                        t.tur_codigo as NomeDaTurma
                        FROM GestaoAvaliacao_SGP.dbo.MTR_MatriculaTurma mt WITH (NOLOCK)
                          JOIN GestaoAvaliacao_SGP.dbo.ACA_Aluno a WITH (NOLOCK) 			ON mt.alu_id = a.alu_id
                          JOIN CoreSSO.dbo.PES_Pessoa p WITH (NOLOCK)						ON a.pes_id = p.pes_id 
                          JOIN CoreSSO.dbo.SYS_Usuario u WITH (NOLOCK)						ON p.pes_id = u.pes_id
                          JOIN GestaoAvaliacao_SGP.dbo.TUR_Turma t WITH (NOLOCK) 			ON t.tur_id = mt.tur_id 
                          JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaCurriculo tc  WITH (NOLOCK)	ON tc.tur_id = t.tur_id
                          JOIN GestaoAvaliacao_SGP.dbo.ACA_TipoCurriculoPeriodo tcp WITH (NOLOCK) ON tcp.tcp_id = tc.tcp_id
                          JOIN GestaoAvaliacao_SGP.dbo.ESC_Escola e WITH (NOLOCK) 			ON t.esc_id = e.esc_id 
                          JOIN GestaoAvaliacao_SGP.dbo.ACA_CalendarioAnual ca WITH (NOLOCK) ON ca.cal_id = t.cal_id
                          LEFT JOIN CoreSSO.dbo.PES_PessoaDocumento pd WITH (NOLOCK) 		ON pd.pes_id = p.pes_id AND pd.tdo_id IN (SELECT tdo_id FROM CoreSSO.dbo.SYS_TipoDocumentacao WHERE tdo_sigla = 'cpf')
                        WHERE a.alu_situacao <> @situacao
                          AND mt.mtu_situacao = @situacaoMatriculaAtiva
                          AND t.tur_situacao <> @situacao
                          AND u.usu_situacao <> @situacao
                          AND ca.cal_ano = @ano
                          AND tcp.tcp_id IN (38, 39, 40) --1, 2 e 3 séries
                          AND tcp.tne_id IN (@tipoEnsinoMedio)"
                        + (pEscolas == null || !pEscolas.Any() ? string.Empty : "\r\n AND e.esc_codigo IN @escolas")
                        + "\r\n ORDER BY e.esc_nome, t.tur_codigo, p.pes_nome";

                    var registros = dbConn.Query<Aluno>(sql, new
                    {
                        sexoMasculino = ((char)Enums.Sexo.MASCULINO).ToString(),
                        sexoFeminino = ((char)Enums.Sexo.FEMININO).ToString(),
                        situacao = (byte)Enums.SituacaoRegistro.EXCLUIDO,
                        situacaoMatriculaAtiva = 1,
                        ano = currentYear,
                        tipoEnsinoMedio = (byte)Enums.TipoNivelEnsino.ENSINO_MEDIO,
                        escolas = (pEscolas == null || !pEscolas.Any() ? null : pEscolas.Select(e => e.CodigoDaEscola)),
                    });
                    return registros.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Erro ao recuperar todas os alunos", ex);
                throw new ApplicationException("Erro ao recuperar todas os alunos", ex);
            }
        }

        private IDbConnection GetConnection()
        {
            return new SqlConnection(DecryptTripleDES(dbConnectionString));
        }

        private string DecryptTripleDES(string value)
        {
            return cripto.Decrypt(value);
        }

    }
}