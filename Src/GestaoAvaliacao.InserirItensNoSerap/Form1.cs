using Castle.Windsor;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.MappingDependence;
using GestaoAvaliacao.Services;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using GestaoAvaliacao.WebProject.Facade;
using EntityFile = GestaoAvaliacao.Entities.File;
using File = System.IO.File;

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
            XmlSerializer serializer = new XmlSerializer(typeof(ConteudoItem));
            ConteudoItem result = null;

            using (var fileStream = new FileStream(@"C:\Projetos\SME\SME-Serap-main\Src\GestaoAvaliacao.InserirItensNoSerap\19808.xml", FileMode.Open))
            {
                result = (ConteudoItem)serializer.Deserialize(fileStream);
            }

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
                    Justificative = opcao.Justificativa,
                    Numeration = opcao.IdOpcao,
                    State = 1,
                };

                alternativa.Description = string.IsNullOrEmpty(opcao.Enunciado)
                    ? string.Empty
                    : $"<p>{opcao.Enunciado}</p>";


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

            var baseText = new BaseText();
            if (!string.IsNullOrEmpty(result.TextoBase.Descricao) || !string.IsNullOrEmpty(result.TextoBase.ImagemItem))
            {
                baseText.Description = string.IsNullOrEmpty(result.TextoBase.Descricao)
                    ? string.Empty
                    : $"<p>{result.TextoBase.Descricao}</p>";
                baseText.Source = result.TextoBase.Fonte;
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

            var item = new Item
            {
                Alternatives = alternativas,
                BaseText = baseText,
                BaseText_Id = null,
                EvaluationMatrix_Id = result.Matriz.Code,
                Id = 0,
                IsRestrict = false,
                ItemAudios = new List<ItemAudio>(),
                ItemCode = string.IsNullOrWhiteSpace(result.ItemCode.Code) ? result.ItemCode.Code :,
                ItemCodeVersion = 0,
                ItemFiles = new List<ItemFile>(),
                ItemCurriculumGrades = new List<ItemCurriculumGrade> { new ItemCurriculumGrade { TypeCurriculumGradeId = result.Ano.Code } },
                ItemLevel_Id = result.Dif,
                ItemNarrated = false,
                ItemSituation_Id = 1,
                ItemSkills = new List<ItemSkill> { new ItemSkill { Skill_Id = result.Hab.Code, Id = 1, OriginalSkill = true }, new ItemSkill { Skill_Id = 2081, Id = 2, OriginalSkill = true } },
                ItemType_Id = 1,
                ItemVersion = 0,
                Keywords = result.Keywords.Value,
                KnowledgeArea_Id = result.AreaDeConhecimento.Code,
                NarrationAlternatives = false,
                NarrationStudentStatement = false,
                Statement = result.Comando,
                StudentStatement = false,
                SubSubject_Id = result.SubAssunto.Code,
                Tips = result.Observacao,
            };
            var files = new List<EntityFile>();
            Item entity = new Item();

            try
            {
                item.ItemFiles = new List<ItemFile>();
                item.ItemAudios = new List<ItemAudio>();

                if (item.Id > 0)
                {
                    item.ItemSituation = item.ItemSituation_Id > 0 ? itemSituationBusiness.GetItemSituationById(item.ItemSituation_Id) : null;
                    entity = itemBusiness.Update(item.Id, item);

                    if (entity.Validate.IsValid)
                    {
                    }
                }
                else
                {
                    entity = itemBusiness.Save(0, item);

                    if (entity.Validate.IsValid)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = string.Format("Erro ao {0} item.", item.Id > 0 ? "alterar" : "salvar");

            }
        }

        private EntityFile UploadFile(EnumFileType fileType, ConteudoItem result, Alternative alternative = null)
        {
            var fileBusiness = container.Resolve<IFileBusiness>();
            var entity = new EntityFile();

            try
            {
                var upload = new UploadModel();
                var fileName = fileType == EnumFileType.BaseText ?
                    $"{result.Sequence}.jpeg" :
                    $"{result.Sequence}_{alternative.Numeration}.jpeg";
                var path = $@"C:\Projetos\SME\SME-Serap-main\Src\GestaoAvaliacao.InserirItensNoSerap\{fileName}";
                if (!File.Exists(path))
                {
                    return null;
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
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao realizar o upload da imagem.";
                LogFacade.SaveError(ex);
                return null;
            }
        }
    }
}
