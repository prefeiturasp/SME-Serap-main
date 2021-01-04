using Castle.Windsor;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.MappingDependence;
using GestaoAvaliacao.Services;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
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

            var item = new Item();
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
