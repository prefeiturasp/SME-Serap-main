using AvaliaMais.FolhaRespostas.Application.Interfaces;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoInicial.Interfaces;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva.Interfaces;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.Util;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace AvaliaMais.FolhaRespostas.Application
{
    public class ExportacaoAppService : IExportacaoAppService
	{
		private readonly IProcessamentoProvaService _procService;
		private readonly IProcessamentoInicialService _procInicialService;
        private readonly IFileBusiness _fileBusiness;
        private readonly IStorage _storage;
		private readonly IParameterBusiness _parameterBusiness;

        public ExportacaoAppService(IProcessamentoProvaService procService, IProcessamentoInicialService processamentoInicialService, 
			IFileBusiness fileBusiness, IStorage storage, IParameterBusiness parameterBusiness)
		{
			_procService = procService;
			_procInicialService = processamentoInicialService;
			_fileBusiness = fileBusiness;
			_storage = storage;
			_parameterBusiness = parameterBusiness;
		}
        
        public EntityFile ExportReport(IEnumerable<DRE> dres, IEnumerable<Escola> escolas, IEnumerable<Turma> turmas, 
			IEnumerable<Aluno> alunos, Quantidade totalRede, string separator, string virtualDirectory, string physicalDirectory, SYS_Usuario usuario)
        {
            EntityFile ret = new EntityFile();
            StringBuilder stringBuilder = new StringBuilder();
            string fileName = string.Empty;
			var IsWarningUpload = bool.Parse(_parameterBusiness.GetParamByKey(EnumParameterKey.WARNING_UPLOAD_BATCH_DETAIL.GetDescription(), usuario.ent_id).Value);
			string conferir = IsWarningUpload ? "Sucesso - questão(ões) nula(s) e/ou rasuradas" : "Conferir";

            if (dres != null)
            {
                fileName = "Rel_processamento_correcao_DRE";
                stringBuilder.Append(string.Format("DRE{0} Qtde. alunos adesão{0} Qtde. alunos identificados{0} Sucesso{0} {1}{0} Ausente{0} Erro{0} Pendente{0}", separator, conferir));
                stringBuilder.AppendLine();

                if (totalRede != null)
                {
                    stringBuilder.Append("Total da rede" + separator);
                    stringBuilder.Append(totalRede.Aderidos + separator);
                    stringBuilder.Append(totalRede.Identificados + separator);
                    stringBuilder.Append(totalRede.Sucesso + separator);
                    stringBuilder.Append(totalRede.Conferir + separator);
                    stringBuilder.Append(totalRede.Ausente + separator);
                    stringBuilder.Append(totalRede.Erro + separator);
                    stringBuilder.Append(totalRede.Pendente + separator);
                    stringBuilder.AppendLine();
                }

                foreach (DRE entity in dres)
                {
                    stringBuilder.Append(entity.Nome + separator);
                    stringBuilder.Append(entity.AlunoStatus.Aderidos + separator);
                    stringBuilder.Append(entity.AlunoStatus.Identificados + separator);
                    stringBuilder.Append(entity.ProcessamentoStatus.Sucesso + separator);
                    stringBuilder.Append(entity.ProcessamentoStatus.Conferir + separator);
                    stringBuilder.Append(entity.ProcessamentoStatus.Ausente + separator);
                    stringBuilder.Append(entity.ProcessamentoStatus.Erro + separator);
                    stringBuilder.Append(entity.ProcessamentoStatus.Pendente + separator);
                    stringBuilder.AppendLine();
                }
            }

            if (escolas != null)
            {
                fileName = "Rel_processamento_correcao_Escola";
                stringBuilder.Append(string.Format("Escola{0} Qtde. alunos adesão{0} Qtde. alunos identificados{0} Sucesso{0} {1}{0} Ausente{0} Erro{0} Pendente{0}", separator, conferir));
                stringBuilder.AppendLine();

                foreach (Escola entity in escolas)
                {
                    stringBuilder.Append(entity.Nome + separator);
                    stringBuilder.Append(entity.AlunoStatus.Aderidos + separator);
                    stringBuilder.Append(entity.AlunoStatus.Identificados + separator);
                    stringBuilder.Append(entity.ProcessamentoStatus.Sucesso + separator);
                    stringBuilder.Append(entity.ProcessamentoStatus.Conferir + separator);
                    stringBuilder.Append(entity.ProcessamentoStatus.Ausente + separator);
                    stringBuilder.Append(entity.ProcessamentoStatus.Erro + separator);
                    stringBuilder.Append(entity.ProcessamentoStatus.Pendente + separator);
                    stringBuilder.AppendLine();
                }
            }

            if (turmas != null)
            {
                fileName = "Rel_processamento_correcao_Turma";
                stringBuilder.Append(string.Format("Turma{0} Qtde. alunos adesão{0} Qtde. alunos identificados{0} Sucesso{0} {1}{0} Ausente{0} Erro{0} Pendente{0}", separator, conferir));
                stringBuilder.AppendLine();

                foreach (Turma entity in turmas)
                {
                    stringBuilder.Append(entity.Nome + separator);
                    stringBuilder.Append(entity.AlunoStatus.Aderidos + separator);
                    stringBuilder.Append(entity.AlunoStatus.Identificados + separator);
                    stringBuilder.Append(entity.ProcessamentoStatus.Sucesso + separator);
                    stringBuilder.Append(entity.ProcessamentoStatus.Conferir + separator);
                    stringBuilder.Append(entity.ProcessamentoStatus.Ausente + separator);
                    stringBuilder.Append(entity.ProcessamentoStatus.Erro + separator);
                    stringBuilder.Append(entity.ProcessamentoStatus.Pendente + separator);
                    stringBuilder.AppendLine();
                }
            }

            if (alunos != null)
            {
                fileName = "Rel_processamento_correcao_Aluno";
                stringBuilder.Append(string.Format("Nº{0} Aluno{0} Data de matrícula{0} Data de saída{0} Situação{0}", separator));
                stringBuilder.AppendLine();

                foreach (Aluno entity in alunos)
                {
                    stringBuilder.Append(entity.Numero + separator);
                    stringBuilder.Append(entity.Nome + separator);
                    stringBuilder.Append((entity.DataMatricula.HasValue ? entity.DataMatricula.Value.ToString("dd/MM/yyyy", new CultureInfo("pt-BR")) : " - ") + separator);
                    stringBuilder.Append((entity.DataSaida.HasValue ? entity.DataSaida.Value.ToString("dd/MM/yyyy", new CultureInfo("pt-BR")) : " - ") + separator);
                    if (entity.Ausente)
                    {
                        stringBuilder.Append("Ausente" + separator);
                    }
                    else
                    {
						var situation = entity.Situacao == Situacao.Conferir ? conferir : entity.Situacao.GetDescription();
						stringBuilder.Append(situation + separator);
                    }
                    stringBuilder.AppendLine();
                }
            }

            var fileContent = stringBuilder.ToString();
            if (!string.IsNullOrEmpty(fileContent))
            {
                byte[] buffer = System.Text.Encoding.Default.GetBytes(fileContent);
                string originalName = string.Format("{0}.csv", fileName);
                string name = string.Format("{0}.csv", Guid.NewGuid());
                string contentType = MimeType.CSV.GetDescription();

                var csvFiles = _fileBusiness.GetAllFilesByType(EnumFileType.ExportReportCorrection, DateTime.Now.AddDays(-1));
                if (csvFiles != null && csvFiles.Count() > 0)
                {
                    _fileBusiness.DeletePhysicalFiles(csvFiles.ToList(), physicalDirectory);
                    _fileBusiness.DeleteFilesByType(EnumFileType.ExportReportCorrection, DateTime.Now.AddDays(-1));
                }

                ret = _storage.Save(buffer, name, contentType, EnumFileType.ExportReportCorrection.GetDescription(), virtualDirectory, physicalDirectory, out ret);
                if (ret.Validate.IsValid)
                {
                    ret.Name = name;
                    ret.ContentType = contentType;
                    ret.OriginalName = StringHelper.Normalize(originalName);
                    ret.OwnerId = 0;
                    ret.ParentOwnerId = 0;
                    ret.OwnerType = (byte)EnumFileType.ExportReportCorrection;
                    ret = _fileBusiness.Save(ret);
                }
            }
            else
            {
                ret.Validate.IsValid = false;
                ret.Validate.Type = ValidateType.alert.ToString();
                ret.Validate.Message = "Os dados ainda não foram gerados.";
            }

            return ret;
        }

        public IEnumerable<DRE> ObterDres(int provaId)
		{
			var dres = _procService.ObterDres(provaId);

            return dres.Select(x => new
			DRE
			{
				_id = x._id,
				DreId = x.DreId,
				Nome = x.Nome,
				AlunoStatus = x.AlunoStatus,
				ProcessamentoStatus = x.ProcessamentoStatus,
				ProvaId = x.ProvaId
			}).ToList();
		}

        public Quantidade ObterQuantidadeDres(int provaId)
        {
            return _procService.QuantidadeDre(provaId);
        }

        public IEnumerable<DRE> ObterDresGestor(int provaId, Guid usuarioId, Guid grupoId)
		{
			var unidadesAdm = ListarUnidadesAdmUsuarioGrupo(usuarioId, grupoId);
			if (unidadesAdm.Count() > 0)
			{
				var dres = _procService.ObterDresGestor(provaId, unidadesAdm);
				return dres.Select(x => new
				DRE
				{
					_id = x._id,
					DreId = x.DreId,
					Nome = x.Nome,
					AlunoStatus = x.AlunoStatus,
					ProcessamentoStatus = x.ProcessamentoStatus,
					ProvaId = x.ProvaId
				}).ToList();
			}
			return Enumerable.Empty<DRE>();
		}

        public Quantidade ObterQuantidadeDresGestor(int provaId, Guid usuarioId, Guid grupoId)
        {
            var unidadesAdm = ListarUnidadesAdmUsuarioGrupo(usuarioId, grupoId);
            if (unidadesAdm.Count() > 0)
            {
                return _procService.QuantidadeDreGestor(provaId, unidadesAdm);
            }
            else
                return null;
        }

        public IEnumerable<Escola> ObterEscolas(int provaId, Guid DreId)
		{
			var escolas = _procService.ObterEscolas(provaId, DreId);
			return escolas.Select(x => new
			Escola
			{
				_id = x._id,
				EscolaId = x.EscolaId,
				EscolaUad = x.EscolaUad,
				Nome = x.Nome,
				AlunoStatus = x.AlunoStatus,
				ProcessamentoStatus = x.ProcessamentoStatus,
				DreId = x.DreId
			}).ToList();

		}

        public IEnumerable<Escola> ObterEscolasDiretor(int provaId, Guid usuario, Guid grupo)
		{
			var unidadesAdm = ListarUnidadesAdmUsuarioGrupo(usuario, grupo);
			if (unidadesAdm.Count() > 0)
			{
				var escolas = _procService.ObterEscolaDiretor(provaId, unidadesAdm);
				return escolas.Select(x => new
				Escola
				{
					_id = x._id,
					EscolaId = x.EscolaId,
					Nome = x.Nome,
					AlunoStatus = x.AlunoStatus,
					ProcessamentoStatus = x.ProcessamentoStatus,
					DreId = x.DreId
				}).ToList();

			}
			return Enumerable.Empty<Escola>();
		}

        public IEnumerable<Turma> ObterTurmas(int provaId, int escolaId)
		{
			var turmas = _procService.ObterTurmas(provaId, escolaId);
			return turmas.Select(x => new
			Turma
			{
				_id = x._id,
				TurmaId = x.TurmaId,
				Nome = x.Nome,
				AlunoStatus = x.AlunoStatus,
				ProcessamentoStatus = x.ProcessamentoStatus,
				EscolaId = x.EscolaId
			}).ToList();
		}

		public IEnumerable<Turma> ObterTurmasProfessor(int provaId, Guid usuarioId)
		{
			var turmasProfessor = ListarTurmasProfessor(usuarioId);
			if (turmasProfessor.Count() > 0)
			{
				var turmas = _procService.ObterTurmasProfessor(provaId, turmasProfessor);

				var resultado = turmas.Where(a => turmasProfessor.Any(b => b == a.TurmaId)).ToList();
				return resultado.Select(x => new
				Turma
				{
					_id = x._id,
					TurmaId = x.TurmaId,
					Nome = x.Nome,
					AlunoStatus = x.AlunoStatus,
					ProcessamentoStatus = x.ProcessamentoStatus,
					EscolaId = x.EscolaId
				});
			}
			return Enumerable.Empty<Turma>();
		}

		public IEnumerable<Aluno> ObterAlunos(int turmaId, int provaId)
		{
			return _procService.ObterAlunos(turmaId, provaId);
		}

		private Guid[] ListarUnidadesAdmUsuarioGrupo(Guid usuario, Guid grupo)
		{
			var dt = MSTech.CoreSSO.BLL.SYS_UsuarioGrupoUABO.GetSelect(usuario, grupo);
			return dt.AsEnumerable().Select(x => x.Field<Guid>("uad_id")).Cast<Guid>().ToArray();
		}

		private int[] ListarTurmasProfessor(Guid usuarioId)
		{
			return _procInicialService.ObterTurmasDoUsuario(usuarioId).ToArray();
		}
	}
}
