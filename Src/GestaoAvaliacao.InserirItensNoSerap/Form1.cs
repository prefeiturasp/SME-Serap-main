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
using EntityFile = GestaoAvaliacao.Entities.File;

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
            var files = new List<EntityFile>();
            var itemFiles = new List<ItemFile>();
            var itemAudios = new List<ItemAudio>();

            Item entity = new Item();
            object auxItem = null;

            try
            {
                item.ItemFiles = itemFiles;
                item.ItemAudios = itemAudios;

                if (item.Id > 0)
                {
                    item.ItemSituation = item.ItemSituation_Id > 0 ? itemSituationBusiness.GetItemSituationById(item.ItemSituation_Id) : null;
                    if ((files != null && files.Count > 0) || !item.ItemSituation.AllowVersion)
                        entity = itemBusiness.Update(item.Id, item, files);
                    else
                        entity = itemBusiness.Update(item.Id, item);

                    if (entity.Validate.IsValid)
                    {
                        auxItem = new
                        {
                            Id = entity.Id,
                            IsRestrict = entity.IsRestrict,
                            ItemCode = entity.ItemCode,
                            ItemCodeVersion = entity.ItemCodeVersion,
                            ItemVersion = entity.ItemVersion,
                            Statement = entity.Statement,
                            Alternatives = entity.Alternatives.Select(a => new
                            {
                                Id = a.Id,
                                Description = a.Description,
                                Order = a.Order,
                                Correct = a.Correct,
                                numeration = a.Numeration,
                                Justificative = a.Justificative,
                                TCTDiscrimination = a.TCTDiscrimination,
                                TCTDificulty = a.TCTDificulty,
                                TCTBiserialCoefficient = a.TCTBiserialCoefficient,
                                State = a.State
                            }).ToList(),
                            Versions = itemBusiness._GetItemVersions(entity.ItemCodeVersion).Select(x => new
                            {
                                codigo = x.ItemCode,
                                versao = x.ItemVersion,
                                aplicado = string.Empty,
                                criacao = x.CreateDate.ToString("dd/MM/yyyy"),
                                provas = string.Empty
                            }),
                            BaseText_Id = entity.BaseText != null ? entity.BaseText.Id : 0,
                            ItemNarrated = entity.ItemNarrated,
                            StudentStatement = entity.StudentStatement,
                            NarrationStudentStatement = entity.NarrationStudentStatement,
                            NarrationAlternatives = entity.NarrationAlternatives
                        };
                    }
                }
                else
                {
                    if (files != null && files.Count > 0)
                        entity = itemBusiness.Save(0, item, files);
                    else
                        entity = itemBusiness.Save(0, item);

                    if (entity.Validate.IsValid)
                    {
                        auxItem = new
                        {
                            Id = entity.Id,
                            IsRestrict = entity.IsRestrict,
                            ItemCode = entity.ItemCode,
                            ItemVersion = entity.ItemVersion,
                            ItemCodeVersion = entity.ItemCodeVersion,
                            Statement = entity.Statement,
                            Alternatives = entity.Alternatives.Select(a => new
                            {
                                Id = a.Id,
                                Description = a.Description,
                                Order = a.Order,
                                Correct = a.Correct,
                                numeration = a.Numeration,
                                Justificative = a.Justificative,
                                TCTDiscrimination = a.TCTDiscrimination,
                                TCTDificulty = a.TCTDificulty,
                                TCTBiserialCoefficient = a.TCTBiserialCoefficient,
                                State = a.State
                            }).ToList(),
                            Versions = new
                            {
                                codigo = entity.ItemCode,
                                versao = entity.ItemVersion,
                                aplicado = string.Empty,
                                criacao = entity.CreateDate.ToString("dd/MM/yyyy"),
                                provas = string.Empty
                            },
                            BaseText_Id = entity.BaseText_Id != null ? entity.BaseText_Id : 0,
                            ItemNarrated = entity.ItemNarrated,
                            StudentStatement = entity.StudentStatement,
                            NarrationStudentStatement = entity.NarrationStudentStatement,
                            NarrationAlternatives = entity.NarrationAlternatives
                        };
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

        private void button2_Click(object sender, EventArgs e)
        {
            var itemBusiness = container.Resolve<IItemBusiness>();
            var itemSituationBusiness = container.Resolve<IItemSituationBusiness>();
            XmlSerializer serializer = new XmlSerializer(typeof(ConteudoItem));
            ConteudoItem result = null;
            using (var fileStream = new FileStream(@"C:\Projetos\SME\SME-Serap-main\Src\GestaoAvaliacao.InserirItensNoSerap\Arquivos\Xml1.xml", FileMode.Open))
            {
                result = (ConteudoItem)serializer.Deserialize(fileStream);
            }

            var alternativas = new List<Alternative>();
            var ordem = 0;
            foreach (var opcao in result.Opcoes)
            {
                alternativas.Add(new Alternative
                {
                    Id = 0,
                    Description = opcao.Enunciado,
                    Order = ordem,
                    Correct = opcao.Correto,
                    Justificative = opcao.Justificativa,
                    Numeration = opcao.IdOpcao,
                    State = 1,
                });
                ordem++;
            }

            var baseText = new BaseText();
            if (!string.IsNullOrEmpty(result.TextoBase.Descricao))
            {
                baseText.Description = result.TextoBase.Descricao;
                baseText.Source = result.TextoBase.Fonte;
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
                ItemCode = result.ItemCode.Code,
                ItemCodeVersion = 0,
                ItemFiles = new List<ItemFile>(),
                ItemCurriculumGrades = new List<ItemCurriculumGrade> { new ItemCurriculumGrade { TypeCurriculumGradeId = result.Ano.Code } },
                ItemLevel_Id = result.Dif,
                ItemNarrated = false,
                ItemSituation_Id = 1,
                ItemSkills = new List<ItemSkill> { new ItemSkill { Skill_Id = result.Hab.Code, Id = 1, OriginalSkill = true }, new ItemSkill { Skill_Id = 2081, Id = 2, OriginalSkill = true } },
                ItemType_Id = 1,
                ItemVersion = 0,
                Keywords = result.Keywords.Code,
                KnowledgeArea_Id = result.AreaDeConhecimento.Code,
                NarrationAlternatives = false,
                NarrationStudentStatement = false,
                Statement = result.Comando,
                StudentStatement = false,
                SubSubject_Id = result.SubAssunto.Code,
                Tips = result.Observacao,
            };
            var files = new List<EntityFile>();
            var itemFiles = new List<ItemFile>();
            var itemAudios = new List<ItemAudio>();

            Item entity = new Item();
            object auxItem = null;

            try
            {
                item.ItemFiles = itemFiles;
                item.ItemAudios = itemAudios;

                if (item.Id > 0)
                {
                    item.ItemSituation = item.ItemSituation_Id > 0 ? itemSituationBusiness.GetItemSituationById(item.ItemSituation_Id) : null;
                    if ((files != null && files.Count > 0) || !item.ItemSituation.AllowVersion)
                        entity = itemBusiness.Update(item.Id, item, files);
                    else
                        entity = itemBusiness.Update(item.Id, item);

                    if (entity.Validate.IsValid)
                    {
                        auxItem = new
                        {
                            Id = entity.Id,
                            IsRestrict = entity.IsRestrict,
                            ItemCode = entity.ItemCode,
                            ItemCodeVersion = entity.ItemCodeVersion,
                            ItemVersion = entity.ItemVersion,
                            Statement = entity.Statement,
                            Alternatives = entity.Alternatives.Select(a => new
                            {
                                Id = a.Id,
                                Description = a.Description,
                                Order = a.Order,
                                Correct = a.Correct,
                                numeration = a.Numeration,
                                Justificative = a.Justificative,
                                TCTDiscrimination = a.TCTDiscrimination,
                                TCTDificulty = a.TCTDificulty,
                                TCTBiserialCoefficient = a.TCTBiserialCoefficient,
                                State = a.State
                            }).ToList(),
                            Versions = itemBusiness._GetItemVersions(entity.ItemCodeVersion).Select(x => new
                            {
                                codigo = x.ItemCode,
                                versao = x.ItemVersion,
                                aplicado = string.Empty,
                                criacao = x.CreateDate.ToString("dd/MM/yyyy"),
                                provas = string.Empty
                            }),
                            BaseText_Id = entity.BaseText != null ? entity.BaseText.Id : 0,
                            ItemNarrated = entity.ItemNarrated,
                            StudentStatement = entity.StudentStatement,
                            NarrationStudentStatement = entity.NarrationStudentStatement,
                            NarrationAlternatives = entity.NarrationAlternatives
                        };
                    }
                }
                else
                {
                    if (files != null && files.Count > 0)
                        entity = itemBusiness.Save(0, item, files);
                    else
                        entity = itemBusiness.Save(0, item);

                    if (entity.Validate.IsValid)
                    {
                        auxItem = new
                        {
                            Id = entity.Id,
                            IsRestrict = entity.IsRestrict,
                            ItemCode = entity.ItemCode,
                            ItemVersion = entity.ItemVersion,
                            ItemCodeVersion = entity.ItemCodeVersion,
                            Statement = entity.Statement,
                            Alternatives = entity.Alternatives.Select(a => new
                            {
                                Id = a.Id,
                                Description = a.Description,
                                Order = a.Order,
                                Correct = a.Correct,
                                numeration = a.Numeration,
                                Justificative = a.Justificative,
                                TCTDiscrimination = a.TCTDiscrimination,
                                TCTDificulty = a.TCTDificulty,
                                TCTBiserialCoefficient = a.TCTBiserialCoefficient,
                                State = a.State
                            }).ToList(),
                            Versions = new
                            {
                                codigo = entity.ItemCode,
                                versao = entity.ItemVersion,
                                aplicado = string.Empty,
                                criacao = entity.CreateDate.ToString("dd/MM/yyyy"),
                                provas = string.Empty
                            },
                            BaseText_Id = entity.BaseText_Id != null ? entity.BaseText_Id : 0,
                            ItemNarrated = entity.ItemNarrated,
                            StudentStatement = entity.StudentStatement,
                            NarrationStudentStatement = entity.NarrationStudentStatement,
                            NarrationAlternatives = entity.NarrationAlternatives
                        };
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


    }
}
