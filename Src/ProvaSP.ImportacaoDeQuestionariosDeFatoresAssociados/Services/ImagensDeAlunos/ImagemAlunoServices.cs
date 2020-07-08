using ImportacaoDeQuestionariosSME.Data.Repositories.ImagensAlunos;
using ImportacaoDeQuestionariosSME.Domain.Abstractions;
using ImportacaoDeQuestionariosSME.Domain.ImagensAlunos;
using ImportacaoDeQuestionariosSME.Services.ImagensDeAlunos.Dtos;
using ImportacaoDeQuestionariosSME.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.ImagensDeAlunos
{
    public class ImagemAlunoServices : IImagemAlunoServices
    {
        private readonly IImagemAlunoRepository _imagemAlunoRepository;

        public ImagemAlunoServices()
        {
            _imagemAlunoRepository = new ImagemAlunoRepository();
        }

        public async Task ImportarAsync(ImportacaoDeImagemAlunoDto dto)
        {
            if (dto is null)
            {
                dto = new ImportacaoDeImagemAlunoDto();
                dto.AddErro("O DTO é nulo.");
                return;
            }

            try
            {
                var dtImagens = CsvManager.GetCsvFile(dto.CaminhoDaPlanilha);
                if (dtImagens.Rows.Count <= 0)
                {
                    dto.AddErro("Não existem regitros na planilha para exportação.");
                    return;
                }

                var entities = dtImagens
                    .AsEnumerable()
                    .Select(row => new ImagemAluno
                    {
                        AluMatricula = row["CD_ALUNO_SME"].ToString(),
                        AluNome = row["NOME"].ToString(),
                        AreaConhecimentoId = 4,
                        Caminho = FormatCaminhoDaImagem(row["IMAGEM"].ToString()),
                        Edicao = dto.Ano,
                        EscCodigo = row["CD_UNIDADE_EDUCACAO"].ToString(),
                        Pagina = int.Parse(row["PAGINA"].ToString()),
                        Questao = "Redação"
                    })
                    .ToList();

                AdjustEntities(entities);

                await _imagemAlunoRepository.InsertAsync(entities);
            }
            catch (Exception ex)
            {
                dto.AddErro(ex.InnerException?.Message ?? ex.Message);
            }
        }

        private string FormatCaminhoDaImagem(string caminhoDaImagem)
        {
            caminhoDaImagem = caminhoDaImagem.Replace("\\", "/");

            var indice = caminhoDaImagem.IndexOf("IMAGEM/") + 7;
            caminhoDaImagem = $"IMAGENS/2019/{caminhoDaImagem.Substring(indice, caminhoDaImagem.Length - indice)}";

            return caminhoDaImagem;
        }

        private void AdjustEntities(IList<ImagemAluno> entities)
        {
            var duplicates = entities
                .GroupBy(x => x.AluMatricula)
                .Where(x => x.Count() > 1)
                .ToList();

            foreach (var grouping in duplicates)
                foreach (var entity in grouping)
                    entities.Remove(entity);
        }
    }
}