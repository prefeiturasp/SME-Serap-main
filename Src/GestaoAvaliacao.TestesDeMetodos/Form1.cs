using Castle.Windsor;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.MappingDependence;
using GestaoAvaliacao.Services;
using GestaoAvaliacao.Util;
using MSTech.CoreSSO.Entities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;
using MSTech.Security.Cryptography;
using GestaoAvaliacao.Entities;

namespace GestaoAvaliacao.TestesDeMetodos
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
            var _reportItemPerformanceBusiness = container.Resolve<IReportItemPerformanceBusiness>();
            var _testCurriculumGradeBusiness = container.Resolve<ITestCurriculumGradeBusiness>();
            var _baseTextBusiness = container.Resolve<IBaseTextBusiness>();
            var usuario = new SYS_Usuario
            {
                usu_id = new Guid("770B9F29-C2A8-E911-87E1-782BCB3D2D76"),
                ent_id = new Guid("D4955BBE-DBE1-4FFA-B63B-1C098518F887"),
                pes_id = new Guid("D4955BBE-DBE1-4FFA-B63B-1C098518F887")
            };

            var grupo = new SYS_Grupo
            {
                gru_id = new Guid("AAD9D772-41A3-E411-922D-782BCB3D218E"),
                vis_id = 1
            };

            //subGroup_id = 39 & tcp_id = 4 & test_id = 0 & uad_id =
            var textoBase = _baseTextBusiness.GetBaxeTestByItemId(11625);

            var retorno1 = _testCurriculumGradeBusiness.GetDistinctCurricumGradeByTestSubGroup_Id(39);

            var retorno = _reportItemPerformanceBusiness.GetPerformanceTree(0, 39, 4, usuario,
                    grupo, null, null, null, false, false);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var pager = new Pager
            {
                CurrentPage = 0,
                PageSize = 100
            };

            var _correctionBusiness = container.Resolve<ICorrectionBusiness>();

            var filter = new StudentResponseFilter
            {
                Test_Id = 519,
                School_Id = 63,
                ttn_id = 0,
                uad_id = null,
                crp_ordem = 0,
                pes_id = new Guid("D4955BBE-DBE1-4FFA-B63B-1C098518F887"),
                usu_id = new Guid("770B9F29-C2A8-E911-87E1-782BCB3D2D76"),
                vis_id = 1,
                sis_id = 204,
                StatusCorrection = "2,3"
            };

            var lttt = _correctionBusiness.LoadOnlySelectedSectionPaginate(ref pager, filter);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var _itemBusiness = container.Resolve<IItemBusiness>();

            var item = new Item { Id = 12434 };
            _itemBusiness.SaveChangeItem(item, 601, 11532);
            _itemBusiness.SaveChangeItem(item, 602, 11532);
        }
    }
}
