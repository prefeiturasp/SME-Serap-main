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
            var lote = "Lote1";
            var directory = $@"C:\Projetos\SME\SME-Serap-main\Src\GestaoAvaliacao.InserirItensNoSerap\Files\\{lote}";
            if (!Directory.Exists(directory))
                return;

            var sequencias = GetSequenciaLote1();
            foreach (var sequencia in sequencias)
            {
                var fileDirectory = $"{directory}\\{sequencia}.xml";
                ProcessarArquivoXml(itemBusiness, itemSituationBusiness, skillBusiness, knowledgeAreaBusiness, disciplineBusiness, matrisBusiness, disciplineWithKnowledgeArea, seriesWithMatriz, fileDirectory, lote);
            }

            //var files = Directory.GetFiles(directory);
            //foreach (var fileDirectory in files)
            //{
            //    ProcessarArquivoXml(itemBusiness, itemSituationBusiness, skillBusiness, knowledgeAreaBusiness, disciplineBusiness, matrisBusiness, disciplineWithKnowledgeArea, seriesWithMatriz, fileDirectory, lote);
            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var itemBusiness = container.Resolve<IItemBusiness>();
            var itemSituationBusiness = container.Resolve<IItemSituationBusiness>();
            var skillBusiness = container.Resolve<ISkillBusiness>();
            var knowledgeAreaBusiness = container.Resolve<IKnowledgeAreaBusiness>();
            var disciplineBusiness = container.Resolve<IDisciplineBusiness>();
            var matrisBusiness = container.Resolve<IEvaluationMatrixBusiness>();
            var disciplineWithKnowledgeArea = GetDisciplineWithKnowledgeArea();
            var seriesWithMatriz = GetSeriesWithMatriz();
            var lote = "Lote2";
            var directory = $@"C:\Projetos\SME\SME-Serap-main\Src\GestaoAvaliacao.InserirItensNoSerap\Files\\{lote}";
            var sequencias = GetSequenciaLote2();
            foreach (var sequencia in sequencias)
            {
                var fileDirectory = $"{directory}\\{sequencia}.xml";
                ProcessarArquivoXml(itemBusiness, itemSituationBusiness, skillBusiness, knowledgeAreaBusiness, disciplineBusiness, matrisBusiness, disciplineWithKnowledgeArea, seriesWithMatriz, fileDirectory, lote);
            }

            //var files = Directory.GetFiles(directory);
            //foreach (var fileDirectory in files)
            //{
            //    ProcessarArquivoXml(itemBusiness, itemSituationBusiness, skillBusiness, knowledgeAreaBusiness, disciplineBusiness, matrisBusiness, disciplineWithKnowledgeArea, seriesWithMatriz, fileDirectory, lote);
            //}
        }

        private void ProcessarArquivoXml(IItemBusiness itemBusiness, IItemSituationBusiness itemSituationBusiness, ISkillBusiness skillBusiness,
            IKnowledgeAreaBusiness knowledgeAreaBusiness, IDisciplineBusiness disciplineBusiness, IEvaluationMatrixBusiness matrizBusiness,
            List<DisciplineWithKnowledgeArea> disciplineWithKnowledgeArea, List<SerieWithMatriz> seriesWithMatriz, string fileDirectory, string lote)
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

                AjustesPosImportacaoDeKeyword(result);
                AjustesAnoDasMatrizesPosImportacao(result);

                if (!ValidateFile(result, skill, discipline, matriz, disciplineWithKnowledgeArea, knowledgeArea, seriesWithMatriz, fileDirectory, lote))
                    return;

                habilidades = new List<ItemSkill>
                {
                    new ItemSkill {Skill_Id = skill.Parent.Id, Id = 1, OriginalSkill = true},
                    new ItemSkill {Skill_Id = result.Hab.Code, Id = 2, OriginalSkill = true}
                };
            }
            catch (Exception ex)
            {
                var filename = Path.GetFileName(fileDirectory).ToString();
                SalvarLog(ex.Message.Substring(0, 30), int.Parse(filename.Substring(0, filename.Length - 4)), filename, "Serializar Arquivo", null, lote);
                return;
            }

            try
            {
                var entity = new Item();
                var alternativas = SetAlternativas(result, lote);
                var baseText = SetBaseText(result, lote);
                var item = SetItem(result, alternativas, baseText, habilidades);
                item.ItemFiles = new List<ItemFile>();
                item.ItemAudios = new List<ItemAudio>();

                entity = itemBusiness.Save(0, item);

                if (!entity.Validate.IsValid)
                    SalvarLog(entity.Validate.Message, int.Parse(result.Sequence.ToString()), Path.GetFileName(fileDirectory), "Validação ao salvar", null, lote);
            }
            catch (Exception ex)
            {
                SalvarLog(ex.Message, int.Parse(result.Sequence.ToString()), Path.GetFileName(fileDirectory), "Genérico", null, lote);
            }
        }

        private void AjustesAnoDasMatrizesPosImportacao(ConteudoItem result)
        {
            var matriz162 = GetSequencesMatriz162();
            var matriz163 = GetSequencesMatriz163();
            var matriz164 = GetSequencesMatriz164();
            var matriz167 = GetSequencesMatriz167();

            if (matriz162.Any(a => a == result.Sequence))
            {
                result.Ano.Code = 7;
            }
            else if (matriz163.Any(a => a == result.Sequence))
            {
                result.Ano.Code = 8;
            }
            else if (matriz164.Any(a => a == result.Sequence))
            {
                result.Ano.Code = 9;
            }
            else if (matriz167.Any(a => a == result.Sequence))
            {
                result.Ano.Code = 3;
            }
        }

        private List<int> GetSequencesMatriz162()
        {
            return new List<int>() {
                17244,
                21189,
                19403,
                19157,
                19311,
                18069,
                18636,
                19195,
                21190,
                19207,
                18775,
                19586,
                21193,
                21195,
                19143,
                19405,
                19152,
                19375,
                19199,
                20542,
                19313,
                18559,
                20555,
                19150,
                18972,
                21194,
                19582,
                18059,
                21191,
                17166,
                19160,
                19131,
                19161,
                20545,
                19177,
                19170,
                19315,
                21196,
                19025,
                19203,
                19163,
                17408,
                19643,
                18519,
                19107,
                19171,
                19204,
                19205,
                20543,
                19179,
                19202,
                20547,
                19371,
                19133,
                19141,
                19561,
                20548,
                21187,
                18840,
                18535,
                19106,
                19186,
                19146,
                21188,
                19309
            };
        }

        private List<int> GetSequencesMatriz163()
        {
            return new List<int>() {
                19714,
                19104,
                19073,
                17918,
                17723,
                19707,
                19096,
                17917,
                19682,
                18997,
                19110,
                19672,
                17703,
                18531,
                18557,
                17983,
                18381,
                18311,
                18597,
                17985,
                19710,
                18330,
                17970,
                18325,
                19670,
                19680,
                19667,
                19713,
                18071,
                19681,
                18520,
                19673,
                18334,
                19603,
                19017,
                19671,
                18269,
                17950,
                17986,
                18308,
                18320,
                18075,
                19669,
                17920,
                18329,
                18403,
                19708,
                19233,
                18609,
                18570,
                18068,
                19004,
                17921,
                18521,
                18337,
                17713,
                18603,
                19694,
                18538,
                21446,
                18186,
                18539,
                18126,
                19059,
                18338
            };
        }

        private List<int> GetSequencesMatriz164()
        {
            return new List<int>() {
                20920,
                21065,
                20578,
                20574,
                21068,
                18192,
                18836,
                21334,
                20569,
                20929,
                20571,
                20583,
                20925,
                18167,
                20980,
                21307,
                21170,
                18231,
                21171,
                21326,
                18883,
                21173,
                20577,
                20928,
                21172,
                18188,
                18823,
                20573,
                18166,
                21174,
                21069,
                18169,
                18815,
                20978,
                20568,
                20575,
                19604,
                18343,
                18164,
                18806,
                21175,
                20570,
                20567,
                20926,
                20582,
                18841,
                18833,
                20972,
                20974,
                18194,
                18829,
                20581,
                20933,
                18236,
                20572,
                18173,
                18228,
                18848,
                18859,
                20576,
                20927,
                18213,
                18812,
                21294,
                21327,
            };
        }

        private List<int> GetSequencesMatriz167()
        {
            return new List<int>() {
                18437,
                18306,
                19380,
                19383,
                18239,
                20946,
                18047,
                21243,
                19382,
                17436,
                20918,
                20951,
                18246,
                18042,
                18054,
                18061,
                19384,
                19379,
                18165,
                17433,
                19378,
                17438,
                18045,
                18051,
                18057
            };
        }


        private void AjustesPosImportacaoDeKeyword(ConteudoItem result)
        {
            result.Keywords.Value = string.IsNullOrWhiteSpace(result.Keywords.Value) ? "CEBRASPE_20" : result.Keywords.Value;
        }

        private bool ValidateFile(ConteudoItem result, Skill skill, Discipline discipline, EvaluationMatrix matriz,
            List<DisciplineWithKnowledgeArea> disciplineWithKnowledgeArea, KnowledgeArea knowledgeArea,
            List<SerieWithMatriz> seriesWithMatriz, string filePath, string lote)
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
                        'Validação ao salvar',GETDATE(),'{lote}');");
                    retorno = false;
                }
                else if (par.Key == "STATEMENT" && string.IsNullOrEmpty(result.Comando))
                {
                    sqlQuery.AppendLine($@"INSERT INTO LogImprotacaoCebraspe 
                        VALUES('Comando é de preenchimento obrigatório',
                        '{Convert.ToInt32(result.Sequence.ToString())}',
                        '{Path.GetFileName(filePath)}',
                        'Validação ao salvar',GETDATE(),'{lote}');");
                    retorno = false;
                }
            }

            if (knowledgeArea == null)
            {
                sqlQuery.AppendLine($@"INSERT INTO LogImprotacaoCebraspe 
                    VALUES('Area de conhecimento não localizada. KnowledgeAreaId: {result.AreaDeConhecimento.Code}',
                    '{Convert.ToInt32(result.Sequence.ToString())}',
                    '{Path.GetFileName(filePath)}',
                    'Area de conhecimento',GETDATE(),'{lote}');");
                retorno = false;
            }

            if ((result.Dif < 0 || result.Dif > 5))
            {
                sqlQuery.AppendLine($@"INSERT INTO LogImprotacaoCebraspe 
                    VALUES('Dificuldade não localizada. Dificuldade: {result.Dif}',
                    '{Convert.ToInt32(result.Sequence.ToString())}',
                    '{Path.GetFileName(filePath)}',
                    'Dificuldade',GETDATE(),'{lote}');");
                retorno = false;
            }

            if (discipline == null)
            {
                sqlQuery.AppendLine($@"INSERT INTO LogImprotacaoCebraspe 
                    VALUES('Disciplina não localizada. DisciplineId: {result.Disciplina.Code}',
                    '{Convert.ToInt32(result.Sequence.ToString())}',
                    '{Path.GetFileName(filePath)}',
                    'Disciplina',GETDATE(),'{lote}');");
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
                    'Disciplina e Area de Conhecimento',GETDATE(),'{lote}');");
                retorno = false;
            }

            if (skill == null)
            {
                sqlQuery.AppendLine($@"INSERT INTO LogImprotacaoCebraspe 
                    VALUES('Habilidade nãp localizada. HabilidadeId: {result.Hab.Code}',
                    '{Convert.ToInt32(result.Sequence.ToString())}',
                    '{Path.GetFileName(filePath)}',
                    'Habilidade',GETDATE(),'{lote}');");
                retorno = false;
            }

            if (skill != null && skill.EvaluationMatrix.Id != result.Matriz.Code)
            {
                sqlQuery.AppendLine($@"INSERT INTO LogImprotacaoCebraspe 
                    VALUES('A matriz do arquivo é diferente da Matriz da Habilidade. Id da Matriz do arquivo {result.Matriz.Code}, Id da Matriz de Habilidade {skill.EvaluationMatrix.Id}',
                    '{Convert.ToInt32(result.Sequence.ToString())}',
                    '{Path.GetFileName(filePath)}',
                    'Habilidade e Matriz',GETDATE(),'{lote}');");
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
                    'Ano e Matriz',GETDATE(),'{lote}');");

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

        private BaseText SetBaseText(ConteudoItem result, string lote)
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
                    var entityFile = UploadFile(EnumFileType.BaseText, result, null, lote);
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

        private List<Alternative> SetAlternativas(ConteudoItem result, string lote)
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
                    var entityFile = UploadFile(EnumFileType.Alternative, result, alternativa, lote);
                    alternativa.Description = string.IsNullOrEmpty(entityFile.Path)
                        ? alternativa.Description + string.Empty
                        : alternativa.Description + $"<p><img src=\"{entityFile.Path}\" id=\"{ entityFile.Id}\"></p>";
                }

                alternativas.Add(alternativa);
                ordem++;
            }

            return alternativas;   
        }

        private EntityFile UploadFile(EnumFileType fileType, ConteudoItem result, Alternative alternative = null, string lote = null)
        {
            var fileBusiness = container.Resolve<IFileBusiness>();
            var entity = new EntityFile();
            var fileName = fileType == EnumFileType.BaseText ?
                $"{result.Sequence}.jpeg" :
                $"{result.Sequence}_{alternative.Numeration}.jpeg";
            try
            {
                var upload = new UploadModel();

                var path = $@"C:\Projetos\SME\SME-Serap-main\Src\GestaoAvaliacao.InserirItensNoSerap\Imagens\BKP\{fileName}";
                if (!File.Exists(path))
                {
                    SalvarLog($"Imagem não localizada. Nome do arquivo: {fileName}", int.Parse(result.Sequence.ToString()), fileName, "Upload Imagem", null, lote);
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
                SalvarLog($"Erro ao realizar upload da imagem:{ex.Message}", int.Parse(result.Sequence.ToString()), fileName, $"Upload Imagem", null, lote);
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

        private void SalvarLog(string texto, int sequencia, string nomeDoArquivo, string tipo, string queryText = null, string lote = null)
        {
            var stringDeConexao = Decrypt(ConfigurationManager.ConnectionStrings["GestaoAvaliacao"].ConnectionString);
            using (var conGestaoAvaliacao = new SqlConnection(stringDeConexao))
            {
                conGestaoAvaliacao.Open();
                var sqlQuery = new StringBuilder();
                if (queryText is null)
                    sqlQuery.AppendLine($@"INSERT INTO LogImprotacaoCebraspe VALUES('{texto}', '{sequencia}', '{nomeDoArquivo}', '{tipo}', GETDATE(), '{lote}')");
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
        private List<int> GetSequenciaLote1()
        {
            return new List<int>
            {
               17459,
17538,
17615,
17806,
17807,
17922,
17924,
17926,
17940,
17942,
17943,
17945,
17946,
17947,
17951,
17955,
18024,
18141,
18149,
18153,
18166,
18169,
18227,
18251,
18256,
18267,
18274,
18301,
18322,
18326,
18341,
18346,
18372,
18396,
18410,
18418,
18426,
18432,
18474,
18477,
18486,
18572,
18587,
18592,
18594,
18598,
18604,
18626,
18628,
18638,
18695,
18726,
18758,
18805,
18828,
18831,
18839,
18847,
18864,
18873,
18879,
18885,
18889,
18892,
18899,
18903,
18905,
18910,
18912,
18915,
18917,
18920,
18923,
18930,
18933,
18941,
18953,
19000,
19026,
19047,
19113,
19187,
19190,
19191,
19213,
19224,
19225,
19226,
19297,
19309,
19311,
19328,
19329,
19568,
19569,
19614,
19626,
19627,
19629,
19631,
19632,
19644,
19669,
19671,
19672,
19673,
19710,
19713,
19905,
20012,
20013,
20014,
20017,
20056,
20065,
20067,
20068,
20069,
20083,
20123,
20423,
20425,
20427,
20543,
20545,
20548,
20555,
20573,
20574,
20575,
20576,
20577,
20581,
20583,
20606,
20841,
20842,
20843,
20884,
20889,
21029,
21031,
21032,
21033,
21036,
21037,
21212,
23166,
            };
        }

        private List<int> GetSequenciaLote2()
        {
            return new List<int>
            {
18005,
18827,
18904,
18926,
19139,
19570,
19572,
19573,
19575,
20066,
20887,
23161,
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
