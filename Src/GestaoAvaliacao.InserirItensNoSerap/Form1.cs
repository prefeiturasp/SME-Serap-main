using Castle.Windsor;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.MappingDependence;
using GestaoAvaliacao.Services;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Xml.Serialization;
using GestaoAvaliacao.WebProject.Facade;
using EntityFile = GestaoAvaliacao.Entities.File;
using File = System.IO.File;
using System.Configuration;
using System.Linq;

namespace InserirItensNoSerap
{
    public partial class Form1 : Form
    {
        private readonly IWindsorContainer container;

        public Form1()
        {
            container = new WindsorContainer()
                .Install(new BusinessInstaller() { LifestylePerWebRequest = false })
                .Install(new RepositoriesInstaller() { LifestylePerWebRequest = false })
                .Install(new StorageInstaller() { LifestylePerWebRequest = false })
                .Install(new PDFConverterInstaller() { LifestylePerWebRequest = false })
                .Install(new UtilIntaller())
                .Install(new ServiceContainerInstaller());

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var itemBusiness = container.Resolve<IItemBusiness>();
            var itemSituationBusiness = container.Resolve<IItemSituationBusiness>();
            var item = new Item
            {
                Alternatives = new List<Alternative> {
                    new Alternative{
                        Id = 0,
                        Description = "<p>desapareceu.</p>",
                        Order = 0,
                        Correct = false,
                        Justificative= "<p>INCORRETA.\n\tO estudante provavelmente não reconhece\n\to sentido de uma palavra ou expressão em textos escritos e\n\tmultimodais, considerando o contexto e/ou possivelmente acredita que\n\ta expressão “deu as caras” é o mesmo que desaparecer.</p>",
                        Numeration = "A)",
                        State=1,
                    },
                    new Alternative{
                        Id = 0,
                        Description = "<p>reapareceu.</p>",
                        Order = 1,
                        Correct = true,
                        Justificative= "<p>CORRETA.\n\tO estudante provavelmente reconhece\n\to sentido de uma palavra ou expressão em textos escritos e\n\tmultimodais, considerando o contexto.</p>",
                        Numeration = "B)",
                        State=1,
                    },
                    new Alternative{
                        Id = 0,
                        Description = "<p>escondeu.</p>",
                        Order = 2,
                        Correct = false,
                        Justificative= "<p>INCORRETA.\n\tO estudante provavelmente não reconhece\n\to sentido de uma palavra ou expressão em textos escritos e\n\tmultimodais, considerando o contexto e/ou possivelmente acredita que\n\ta expressão “deu as caras” é o mesmo que esconder.</p>",
                        Numeration = "C)",
                        State = 1,
                    },
                    new Alternative{
                        Id = 0,
                        Description = "<p>sumiu.</p>",
                        Order = 3,
                        Correct = false,
                        Justificative = "<p>INCORRETA.\nO estudante provavelmente não reconhece\no sentido de uma palavra ou expressão em textos escritos e\nmultimodais, considerando o contexto e/ou possivelmente acredita que\na expressão “deu as caras” é o mesmo que sumir.</p>",
                        Numeration = "D)",
                        State = 1,
                    },
                },
                BaseText = null,
                BaseText_Id = null,
                EvaluationMatrix_Id = 73,
                Id = 0,
                IsRestrict = false,
                ItemAudios = new List<ItemAudio>(),
                ItemCode = "J0002/1100066",
                ItemCodeVersion = 0,
                ItemFiles = new List<ItemFile>(),
                ItemCurriculumGrades = new List<ItemCurriculumGrade> { new ItemCurriculumGrade { TypeCurriculumGradeId = 2 } },
                ItemLevel_Id = 3,
                ItemNarrated = false,
                ItemSituation_Id = 1,
                ItemSkills = new List<ItemSkill> { new ItemSkill { Skill_Id = 2074, Id = 1, OriginalSkill = true }, new ItemSkill { Skill_Id = 2081, Id = 2, OriginalSkill = true } },
                ItemType_Id = 1,
                ItemVersion = 0,
                Keywords = "lpp_4ano_10",
                KnowledgeArea_Id = 2,
                NarrationAlternatives = false,
                NarrationStudentStatement = false,
                Statement = "<p>Nessa\nmanchete, a expressão “deu as caras” significa que o Sol</p>",
                StudentStatement = false,
                SubSubject_Id = 10,
                Tips = "Observação sobre o item da Prova.",
            };

            Item entity = new Item();

            try
            {
                item.ItemFiles = new List<ItemFile>();
                item.ItemAudios = new List<ItemAudio>();

                if (item.Id > 0)
                {
                    item.ItemSituation = item.ItemSituation_Id > 0 ? itemSituationBusiness.GetItemSituationById(item.ItemSituation_Id) : null;
                    entity = itemBusiness.Update(item.Id, item);
                }
                else
                    entity = itemBusiness.Save(0, item);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = string.Format("Erro ao {0} item.", item.Id > 0 ? "alterar" : "salvar");

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var itemBusiness = container.Resolve<IItemBusiness>();
            var itemSituationBusiness = container.Resolve<IItemSituationBusiness>();
            var skillBusiness = container.Resolve<ISkillBusiness>();
            var knowledgeAreaBusiness = container.Resolve<IKnowledgeAreaBusiness>();
            var disciplineBusiness = container.Resolve<IDisciplineBusiness>();
            var matrisBusiness = container.Resolve<IEvaluationMatrixBusiness>();
            var disciplineWithKnowledgeArea = GetDisciplineWithKnowledgeArea();
            var seriesWithMatriz = GetSeriesWithMatriz();
            var directory = @"C:\Projetos\SME\SME-Serap-main\Src\GestaoAvaliacao.InserirItensNoSerap\Files\Lote2";
            if (!Directory.Exists(directory))
                return;

            var sequencias = GetSequencia();
            foreach (var sequencia in sequencias)
            {
                var fileDirectory = $"{directory}\\{sequencia}.xml";
                ProcessarArquivoXml(itemBusiness, itemSituationBusiness, skillBusiness, knowledgeAreaBusiness, disciplineBusiness, matrisBusiness, disciplineWithKnowledgeArea, seriesWithMatriz, fileDirectory);
            }

            //var files = Directory.GetFiles(directory);
            //foreach (var fileDirectory in files)
            //{
            //    ProcessarArquivoXml(itemBusiness, itemSituationBusiness, skillBusiness, knowledgeAreaBusiness, disciplineBusiness, matrisBusiness, disciplineWithKnowledgeArea, seriesWithMatriz, fileDirectory);
            //}
        }

        private void ProcessarArquivoXml(IItemBusiness itemBusiness, IItemSituationBusiness itemSituationBusiness, ISkillBusiness skillBusiness,
            IKnowledgeAreaBusiness knowledgeAreaBusiness, IDisciplineBusiness disciplineBusiness, IEvaluationMatrixBusiness matrizBusiness,
            List<DisciplineWithKnowledgeArea> disciplineWithKnowledgeArea, List<SerieWithMatriz> seriesWithMatriz, string fileDirectory)
        {
            ConteudoItem result = null;
            var habilidades = new List<ItemSkill>();

            try
            {
                var serializer = new XmlSerializer(typeof(ConteudoItem));
                using (var fileStream = new FileStream(fileDirectory, FileMode.Open))
                {
                    result = (ConteudoItem)serializer.Deserialize(fileStream);
                }

                var knowledgeArea = knowledgeAreaBusiness.Get(result.AreaDeConhecimento.Code);
                var discipline = disciplineBusiness.Get(result.Disciplina.Code);
                var matriz = matrizBusiness.Get(result.Matriz.Code);
                var skill = skillBusiness.Get(result.Hab.Code);

                if (!ValidateFile(result, skill, discipline, matriz, disciplineWithKnowledgeArea, knowledgeArea, seriesWithMatriz, fileDirectory))
                    return;

                habilidades = new List<ItemSkill>
                {
                    new ItemSkill {Skill_Id = skill.Parent.Id, Id = 1, OriginalSkill = true},
                    new ItemSkill {Skill_Id = result.Hab.Code, Id = 2, OriginalSkill = true}
                };
            }
            catch (Exception ex)
            {
                SalvarLog(ex.Message, int.Parse(result.Sequence.ToString()), Path.GetFileName(fileDirectory), "Serializar Arquivo");
                return;
            }

            try
            {
                var entity = new Item();
                var alternativas = SetAlternativas(result);
                var baseText = SetBaseText(result);
                var item = SetItem(result, alternativas, baseText, habilidades);
                item.ItemFiles = new List<ItemFile>();
                item.ItemAudios = new List<ItemAudio>();

                entity = itemBusiness.Save(0, item);

                if (!entity.Validate.IsValid)
                    SalvarLog(entity.Validate.Message, int.Parse(result.Sequence.ToString()), Path.GetFileName(fileDirectory), "Validação ao salvar");
            }
            catch (Exception ex)
            {
                SalvarLog(ex.Message, int.Parse(result.Sequence.ToString()), Path.GetFileName(fileDirectory), "Genérico");
            }
        }

        private bool ValidateFile(ConteudoItem result, Skill skill, Discipline discipline, EvaluationMatrix matriz,
            List<DisciplineWithKnowledgeArea> disciplineWithKnowledgeArea, KnowledgeArea knowledgeArea, List<SerieWithMatriz> seriesWithMatriz, string filePath)
        {
            var retorno = true;
            var sqlQuery = new StringBuilder();
            var parambusiness = container.Resolve<IParameterBusiness>();
            var parameters = parambusiness.GetParamsByPage(2);
            parameters = parameters.Where(p => p.Obligatory == true).ToList();
            var filled = true;

            foreach (var par in parameters)
            {
                if (par.Key == "KEYWORDS" && string.IsNullOrEmpty(result.Keywords.Value))
                {
                    sqlQuery.AppendLine($@"INSERT INTO LogImprotacaoCebraspe 
                        VALUES('Keyword é de preenchimento obrigatório',
                        '{Convert.ToInt32(result.Sequence.ToString())}',
                        '{Path.GetFileName(filePath)}',
                        'Validação ao salvar',GETDATE());");
                    retorno = false;
                }
                else if (par.Key == "STATEMENT" && string.IsNullOrEmpty(result.Comando))
                {
                    sqlQuery.AppendLine($@"INSERT INTO LogImprotacaoCebraspe 
                        VALUES('Comando é de preenchimento obrigatório',
                        '{Convert.ToInt32(result.Sequence.ToString())}',
                        '{Path.GetFileName(filePath)}',
                        'Validação ao salvar',GETDATE());");
                    retorno = false;
                }
            }

            if (knowledgeArea == null)
            {
                sqlQuery.AppendLine($@"INSERT INTO LogImprotacaoCebraspe 
                    VALUES('Area de conhecimento não localizada. KnowledgeAreaId: {result.AreaDeConhecimento.Code}',
                    '{Convert.ToInt32(result.Sequence.ToString())}',
                    '{Path.GetFileName(filePath)}',
                    'Area de conhecimento',GETDATE());");
                retorno = false;
            }

            if ((result.Dif < 0 || result.Dif > 5))
            {
                sqlQuery.AppendLine($@"INSERT INTO LogImprotacaoCebraspe 
                    VALUES('Dificuldade não localizada. Dificuldade: {result.Dif}',
                    '{Convert.ToInt32(result.Sequence.ToString())}',
                    '{Path.GetFileName(filePath)}',
                    'Dificuldade',GETDATE());");
                retorno = false;
            }

            if (discipline == null)
            {
                sqlQuery.AppendLine($@"INSERT INTO LogImprotacaoCebraspe 
                    VALUES('Disciplina não localizada. DisciplineId: {result.Disciplina.Code}',
                    '{Convert.ToInt32(result.Sequence.ToString())}',
                    '{Path.GetFileName(filePath)}',
                    'Disciplina',GETDATE());");
                retorno = false;
            }

            if (!disciplineWithKnowledgeArea.Any(s =>
                s.DisciplinaId == result.Disciplina.Code &&
                s.KnowledgeAreaId == result.AreaDeConhecimento.Code))
            {
                sqlQuery.AppendLine($@"INSERT INTO LogImprotacaoCebraspe 
                    VALUES('A disciplina não possui vínculo com a área de conhecimento informada. KnowledgeAreaId: {result.AreaDeConhecimento.Code}, DisciplineId: {result.Disciplina.Code}',
                    '{Convert.ToInt32(result.Sequence.ToString())}',
                    '{Path.GetFileName(filePath)}',
                    'Disciplina e Area de Conhecimento',GETDATE());");
                retorno = false;
            }

            if (skill == null)
            {
                sqlQuery.AppendLine($@"INSERT INTO LogImprotacaoCebraspe 
                    VALUES('Habilidade nãp localizada. HabilidadeId: {result.Hab.Code}',
                    '{Convert.ToInt32(result.Sequence.ToString())}',
                    '{Path.GetFileName(filePath)}',
                    'Habilidade',GETDATE());");
                retorno = false;
            }

            if (skill!=null && skill.EvaluationMatrix.Id != result.Matriz.Code)
            {
                sqlQuery.AppendLine($@"INSERT INTO LogImprotacaoCebraspe 
                    VALUES('A matriz do arquivo é diferente da Matriz da Habilidade. Id da Matriz do arquivo {result.Matriz.Code}, Id da Matriz de Habilidade {skill.EvaluationMatrix.Id}',
                    '{Convert.ToInt32(result.Sequence.ToString())}',
                    '{Path.GetFileName(filePath)}',
                    'Habilidade e Matriz',GETDATE());");
                retorno = false;
            }

            if (!seriesWithMatriz.Any(s =>
                s.SerieId == result.Ano.Code &&
                s.MatrizId == result.Matriz.Code))
            {
                sqlQuery.AppendLine($@"INSERT INTO LogImprotacaoCebraspe 
                    VALUES('O ano não possui vínculo com a matriz informada. Ano: {result.Ano.Code}, MatrizId: {result.Matriz.Code}',
                    '{Convert.ToInt32(result.Sequence.ToString())}',
                    '{Path.GetFileName(filePath)}',
                    'Ano e Matriz',GETDATE());");

                retorno = false;
            }
            SalvarLog(null, 0, null, null, sqlQuery.ToString());

            return retorno;
        }



        private Item SetItem(ConteudoItem result, List<Alternative> alternativas, BaseText baseText, List<ItemSkill> habilidades)
        {
            return new Item
            {
                Alternatives = alternativas,
                BaseText = baseText,
                BaseText_Id = null,
                EvaluationMatrix_Id = result.Matriz.Code,
                Id = 0,
                IsRestrict = false,
                ItemAudios = new List<ItemAudio>(),
                ItemCode = string.IsNullOrWhiteSpace(result.ItemCode.Code) ? result.Sequence.ToString() : result.ItemCode.Code,
                ItemCodeVersion = 0,
                ItemFiles = new List<ItemFile>(),
                ItemCurriculumGrades = new List<ItemCurriculumGrade> { new ItemCurriculumGrade { TypeCurriculumGradeId = result.Ano.Code } },
                ItemLevel_Id = SetDificuldade(result.Dif),
                ItemNarrated = false,
                ItemSituation_Id = 1,
                ItemSkills = habilidades,
                ItemType_Id = 1,
                ItemVersion = 0,
                Keywords = result.Keywords.Value,
                KnowledgeArea_Id = result.AreaDeConhecimento.Code,
                NarrationAlternatives = false,
                NarrationStudentStatement = false,
                Statement = AdjustEncode(result.Comando),
                StudentStatement = false,
                SubSubject_Id = result.SubAssunto.Code,
                Tips = result.Observacao,
            };
        }

        private long SetDificuldade(byte dif)
        {
            if (dif == 1)
                return 3; // Médio
            else if (dif == 2)
                return 4; //Difícil
            else if (dif == 3)
                return 5; //Muito Difícil
            else if (dif == 4)
                return 1; // Muito Fácil
            else
                return 2; //Fácil
        }

        private string AdjustEncode(string texto)
        {
            var sb = new StringBuilder(texto);
            sb = sb.Replace(";#", "&#");
            return HttpUtility.HtmlDecode(sb.ToString());
        }

        private BaseText SetBaseText(ConteudoItem result)
        {
            var baseText = new BaseText();
            if (!string.IsNullOrEmpty(result.TextoBase.Descricao) || !string.IsNullOrEmpty(result.TextoBase.ImagemItem))
            {
                baseText.Description = string.IsNullOrEmpty(result.TextoBase.Descricao)
                    ? string.Empty
                    : $"{AdjustEncode(result.TextoBase.Descricao)}";
                baseText.Source = AdjustEncode(result.TextoBase.Fonte);
                if (!string.IsNullOrWhiteSpace((result.TextoBase.ImagemItem)))
                {
                    var entityFile = UploadFile(EnumFileType.BaseText, result);
                    baseText.Description = string.IsNullOrEmpty(entityFile.Path)
                        ? baseText.Description + string.Empty
                        : baseText.Description + $"<p><img src=\"{entityFile.Path}\" id=\"{ entityFile.Id}\"></p>";
                }
            }
            else
            {
                baseText = null;
            }
            return baseText;
        }

        private List<Alternative> SetAlternativas(ConteudoItem result)
        {
            var alternativas = new List<Alternative>();
            var ordem = 0;
            foreach (var opcao in result.Opcoes)
            {
                var alternativa = new Alternative
                {
                    Id = 0,
                    Description = string.Empty,
                    Order = ordem,
                    Correct = opcao.Correto,
                    Justificative = AdjustEncode(opcao.Justificativa),
                    Numeration = opcao.IdOpcao,
                    State = 1,
                };

                alternativa.Description = string.IsNullOrEmpty(opcao.Enunciado)
                    ? string.Empty
                    : $"{AdjustEncode(opcao.Enunciado)}";

                if (!string.IsNullOrEmpty(opcao.ImagemAlternativa))
                {
                    var entityFile = UploadFile(EnumFileType.Alternative, result, alternativa);
                    alternativa.Description = string.IsNullOrEmpty(entityFile.Path)
                        ? alternativa.Description + string.Empty
                        : alternativa.Description + $"<p><img src=\"{entityFile.Path}\" id=\"{ entityFile.Id}\"></p>";
                }

                alternativas.Add(alternativa);
                ordem++;
            }

            return alternativas;
        }

        private EntityFile UploadFile(EnumFileType fileType, ConteudoItem result, Alternative alternative = null)
        {
            var fileBusiness = container.Resolve<IFileBusiness>();
            var entity = new EntityFile();
            var fileName = fileType == EnumFileType.BaseText ?
                $"{result.Sequence}.jpeg" :
                $"{result.Sequence}_{alternative.Numeration}.jpeg";
            try
            {
                var upload = new UploadModel();

                var path = $@"C:\Projetos\SME\SME-Serap-main\Src\GestaoAvaliacao.InserirItensNoSerap\Imagens\{fileName}";
                if (!File.Exists(path))
                {
                    SalvarLog($"Imagem não localizada. Nome do arquivo: {fileName}", int.Parse(result.Sequence.ToString()), fileName, "Upload Imagem");
                    return new EntityFile();
                }
                using (FileStream itemFile = System.IO.File.Open(path, FileMode.Open))
                {
                    upload = new UploadModel
                    {
                        ContentLength = (int)itemFile.Length,
                        ContentType = "image/jpeg",
                        InputStream = null,
                        Stream = itemFile,
                        FileName = fileName,
                        VirtualDirectory = "https://hom-serap.sme.prefeitura.sp.gov.br/Files", //VIRTUAL_PATH   
                        PhysicalDirectory = $@"C:\Projects\SME\SME-Serap-main\Src\GestaoAvaliacao\Files",
                        FileType = fileType,
                        UsuId = Guid.Parse("B326764F-FFFE-E911-87E3-782BCB3D2D76")
                    };
                    entity = fileBusiness.Upload(upload);
                    return entity;
                }
            }
            catch (Exception ex)
            {
                SalvarLog($"Erro ao realizar upload da imagem:{ex.Message}", int.Parse(result.Sequence.ToString()), fileName, $"Upload Imagem");
                return new EntityFile();
            }
        }

        private List<DisciplineWithKnowledgeArea> GetDisciplineWithKnowledgeArea()
        {
            var stringDeConexao = Decrypt(ConfigurationManager.ConnectionStrings["GestaoAvaliacao"].ConnectionString);
            using (var conGestaoAvaliacao = new SqlConnection(stringDeConexao))
            {
                conGestaoAvaliacao.Open();
                var sqlQuery = new StringBuilder();
                sqlQuery.Append(@"
                    SELECT KA.Id as KnowledgeAreaId, Ka.Description as KnowledgeArea, DIS.Id as DisciplinaId, DIS.Description Disciplina
                    FROM KnowledgeArea AS KA WITH(NOLOCK) 
	                    INNER JOIN KnowledgeAreaDiscipline AS KAD WITH(NOLOCK) 
		                    ON KAD.KnowledgeArea_Id = KA.Id 
	                    INNER JOIN dbo.Discipline AS DIS WITH(NOLOCK) 
		                    ON DIS.Id = KAD.Discipline_Id 
                    WHERE KA.Id IN (1,2,3,4,5,6,10) 
	                    AND DIS.State = 1 
	                    AND KAD.State = 1 
	                    AND KA.State = 1 
                    GROUP BY KA.Id,Ka.Description, DIS.Id, DIS.Description
                    ORDER BY DIS.Description 
                ");

                var command = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao);
                var reader = command.ExecuteReader();
                var lista = new List<DisciplineWithKnowledgeArea>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        lista.Add(new DisciplineWithKnowledgeArea
                        {
                            DisciplinaId = int.Parse(reader["DisciplinaId"].ToString()),
                            Disciplina = reader["Disciplina"].ToString(),
                            KnowledgeAreaId = int.Parse(reader["KnowledgeAreaId"].ToString()),
                            KnowledgeArea = reader["KnowledgeArea"].ToString()
                        });
                    }
                }
                return lista;
            }
        }

        private List<SerieWithMatriz> GetSeriesWithMatriz()
        {
            var stringDeConexao = Decrypt(ConfigurationManager.ConnectionStrings["GestaoAvaliacao"].ConnectionString);
            using (var conGestaoAvaliacao = new SqlConnection(stringDeConexao))
            {
                conGestaoAvaliacao.Open();
                var sqlQuery = new StringBuilder();
                sqlQuery.Append(@"
                    select distinct emccg.TypeCurriculumGradeId SerieId,
                    (SELECT [tcp_descricao]
	                    FROM GEstaoAvaliacao_SGP..[ACA_TipoCurriculoPeriodo] WITH (NOLOCK) 
	                    where tcp_situacao=1 and tcp_id = emccg.TypeCurriculumGradeId) Serie,
                    emc.EvaluationMatrix_Id as MatrizId
                    from EvaluationMatrixCourseCurriculumGrade emccg WITH(NOLOCK)
                    INNER JOIN EvaluationMatrixCourse emc WITH(NOLOCK) ON emc.Id = emccg.EvaluationMatrixCourse_Id
                    WHERE emc.EvaluationMatrix_Id IN (select Id from EvaluationMatrix where state=1) 
                ");

                var command = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao);
                var reader = command.ExecuteReader();
                var lista = new List<SerieWithMatriz>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        lista.Add(new SerieWithMatriz
                        {
                            SerieId = int.Parse(reader["SerieId"].ToString()),
                            Serie = reader["Serie"].ToString(),
                            MatrizId = int.Parse(reader["MatrizId"].ToString()),
                        });
                    }
                }
                return lista;
            }
        }

        private void SalvarLog(string texto, int sequencia, string nomeDoArquivo, string tipo, string queryText = null)
        {
            var stringDeConexao = Decrypt(ConfigurationManager.ConnectionStrings["GestaoAvaliacao"].ConnectionString);
            using (var conGestaoAvaliacao = new SqlConnection(stringDeConexao))
            {
                conGestaoAvaliacao.Open();
                var sqlQuery = new StringBuilder();
                if (queryText is null)
                    sqlQuery.AppendLine($@"INSERT INTO LogImprotacaoCebraspe VALUES('{texto}','{sequencia}','{nomeDoArquivo}','{tipo}',GETDATE())");
                else
                    sqlQuery.AppendLine(queryText);
                var command = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao);
                command.ExecuteNonQuery();
            }
        }

        private string Decrypt(string value)
        {
            var cripto = new MSTech.Security.Cryptography.SymmetricAlgorithm(MSTech.Security.Cryptography.SymmetricAlgorithm.Tipo.TripleDES);
            return cripto.Decrypt(value);
        }

        private List<int> GetSequencia()
        {
            return new List<int>
            {
                17354,
                17357,
                17359,
                17616,
                17617,
                18271,
                18283,
                19356,
                19448,
                19449
            };
        }
    }

    public class SerieWithMatriz
    {
        public int SerieId { get; set; }
        public string Serie { get; set; }
        public int MatrizId { get; set; }
    }

    public class DisciplineWithKnowledgeArea
    {
        public int KnowledgeAreaId { get; set; }
        public string KnowledgeArea { get; set; }
        public int DisciplinaId { get; set; }
        public string Disciplina { get; set; }
    }
}
