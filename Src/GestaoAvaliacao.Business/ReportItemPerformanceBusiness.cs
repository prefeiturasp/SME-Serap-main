using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.Entities.Projections;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using GestaoAvaliacao.Util;
using GestaoEscolar.IBusiness;
using MSTech.CoreSSO.BLL;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Business
{
    public class ReportItemPerformanceBusiness : IReportItemPerformanceBusiness
    {
        private readonly ICorrectionResultsRepository _correctionResultsRepository;
        private readonly IBlockBusiness _blockBusiness;
        private readonly IAdherenceBusiness _adherenceBusiness;
        private readonly IFileBusiness _fileBusiness;
        private readonly IStorage _storage;
        private readonly IStudentCorrectionBusiness _studentCorrectionBusiness;
        private readonly IDisciplineBusiness _disciplineBusiness;
        private readonly IItemBusiness _itemBusiness;
        private readonly ITestBusiness _testBusiness;
        private readonly IESC_EscolaBusiness _escolaBusiness;
        private readonly IItemFileBusiness itemFileBusiness;

        public ReportItemPerformanceBusiness(ICorrectionResultsRepository correctionResultsRepository,
            IBlockBusiness blockBusiness, IAdherenceBusiness adherenceBusiness, IFileBusiness fileBusiness, IStorage storage,
            IStudentCorrectionBusiness studentCorrectionBusiness, IDisciplineBusiness disciplineBusiness, IItemBusiness itemBusiness,
            ITestBusiness testBusiness, IESC_EscolaBusiness escolaBusiness, IItemFileBusiness itemFileBusiness)
        {
            _correctionResultsRepository = correctionResultsRepository;
            _blockBusiness = blockBusiness;
            _adherenceBusiness = adherenceBusiness;
            _fileBusiness = fileBusiness;
            _storage = storage;
            _studentCorrectionBusiness = studentCorrectionBusiness;
            _disciplineBusiness = disciplineBusiness;
            _itemBusiness = itemBusiness;
            _testBusiness = testBusiness;
            _escolaBusiness = escolaBusiness;
            this.itemFileBusiness = itemFileBusiness;
        }

        public List<TestAverageItemPerformanceDTO> GetTestAverageItemPerformanceGeneral(long test_Id)
        {
            return _correctionResultsRepository.GetTestAverageItemPerformanceGeneral(test_Id);
        }

        public List<TestAverageItemPerformanceDTO> GetTestAverageItemPerformanceByDre(List<long> test_Id)
        {
            return _correctionResultsRepository.GetTestAverageItemPerformanceByDre(test_Id);
        }

        public List<TestAverageItemPerformanceDTO> GetTestAverageItemPerformanceBySchool(long test_Id, Guid dre_id)
        {
            return _correctionResultsRepository.GetTestAverageItemPerformanceBySchool(test_Id, dre_id);
        }

        public TestAverageItensViewModel ObterDresDesempenhoItem(long provaId, long? discipline_id, SYS_Usuario usuario, SYS_Grupo grupo)
        {
            var dres = _adherenceBusiness.LoadDreSimpleAdherence(provaId, usuario, grupo);

            List<long> lstTest = new List<long>();
            lstTest.Add(provaId);

            List<TestAverageItemPerformanceDTO> medias = GetTestAverageItemPerformanceByDre(lstTest);

            List<TestAverageItemPerformanceDTO> mediasSME = GetTestAverageItemPerformanceGeneral(Convert.ToInt64(provaId));

            IEnumerable<ItemWithOrderAndRevoked> blocos = _blockBusiness.GetTestItemBlocks(provaId);

            List<TestAverageItemPerformanceDTO> mediasParaOrdenacao =
                (from dre in dres
                 join media in medias on new Guid(dre.EntityId) equals media.Dre_id
                 join bloco in blocos on media.Item_id equals bloco.Item_Id
                 where (discipline_id == null || media.Discipline_Id == discipline_id)
                 select new TestAverageItemPerformanceDTO
                 {
                     Discipline_Id = media.Discipline_Id,
                     Dre_id = media.Dre_id,
                     Item_id = media.Item_id,
                     Order = bloco.Order,
                     Media = Convert.ToDouble(Math.Round(media.Media, 2)),
                     Revoked = bloco.Revoked
                 }).ToList();

            List<TestAverageItemPerformanceDTO> mediasSMEParaOrdenacao =
                (from mediaSME in mediasSME
                 join bloco in blocos on mediaSME.Item_id equals bloco.Item_Id
                 where (discipline_id == null || mediaSME.Discipline_Id == discipline_id)
                 select new TestAverageItemPerformanceDTO
                 {
                     Discipline_Id = mediaSME.Discipline_Id,
                     Item_id = mediaSME.Item_id,
                     Order = bloco.Order,
                     Media = Convert.ToDouble(Math.Round(mediaSME.Media, 2)),
                     Revoked = bloco.Revoked
                 }).ToList();

            List<TestAverageItens> lstMediasOrdenadas = (from media in mediasParaOrdenacao
                                                         group media by media.Dre_id into x
                                                         select new TestAverageItens
                                                         {
                                                             DreId = x.Key,
                                                             Items = x.ToList().Select(s => new TestAverageItemPerformanceDTO { Dre_id = s.Dre_id, Item_id = s.Item_id, Order = s.Order, Media = s.Media, Revoked = s.Revoked }).OrderBy(p => p.Order).ToList()
                                                         }).ToList();

            List<TestAverageItemPerformanceDTO> lstSMEOrdenada = mediasSMEParaOrdenacao.OrderBy(p => p.Order).ToList();

            List<TestAverageItemPerformanceDTO> lstTestAverageItemPerformanceDTO = new List<TestAverageItemPerformanceDTO>();
            //GetPerformanceTree(provaId, usuario, grupo);
            int ordem = 0;
            foreach (var bloco in blocos.Where(p => discipline_id == null || p.Discipline_Id == discipline_id))
            {
                TestAverageItemPerformanceDTO testAverageItemPerformanceDTO = new TestAverageItemPerformanceDTO();
                testAverageItemPerformanceDTO.Media = 0;
                testAverageItemPerformanceDTO.Item_id = bloco.Item_Id;
                testAverageItemPerformanceDTO.Discipline_Id = bloco.Discipline_Id;
                testAverageItemPerformanceDTO.Order = ordem;
                testAverageItemPerformanceDTO.Revoked = bloco.Revoked;
                lstTestAverageItemPerformanceDTO.Add(testAverageItemPerformanceDTO);

                if (lstSMEOrdenada.Count != blocos.Count(p => discipline_id == null || p.Discipline_Id == discipline_id))
                {
                    lstSMEOrdenada.Add(testAverageItemPerformanceDTO);
                }

                ordem++;
            }

            List<TestAverageItens> query = (from dre in dres
                                            join media in lstMediasOrdenadas on new Guid(dre.EntityId) equals media.DreId
                                            select new TestAverageItens { DreId = new Guid(dre.EntityId), DreName = dre.Description, Items = (media != null ? media.Items : lstTestAverageItemPerformanceDTO) })
                                            .ToList();

            return new TestAverageItensViewModel { success = true, lista = query, MediasSME = lstSMEOrdenada };
        }

        public TestAverageItensViewModel ObterEscolasDesempenhoItem(long provaId, long? discipline_id, Guid DreId, SYS_Usuario usuario, SYS_Grupo grupo)
        {
            var escolas = _adherenceBusiness.LoadSchoolSimpleAdherence(provaId, usuario, grupo, DreId);
            List<TestAverageItemPerformanceDTO> medias = GetTestAverageItemPerformanceBySchool(Convert.ToInt64(provaId), DreId);
            IEnumerable<ItemWithOrderAndRevoked> blocos = _blockBusiness.GetTestItemBlocks(provaId);

            List<TestAverageItemPerformanceDTO> mediasOrdenadas =
                  (from media in medias
                   join bloco in blocos on media.Item_id equals bloco.Item_Id
                   where (discipline_id == null || media.Discipline_Id == discipline_id)
                   select new TestAverageItemPerformanceDTO
                   {
                       Esc_id = media.Esc_id,
                       Item_id = media.Item_id,
                       Discipline_Id = media.Discipline_Id,
                       Order = bloco.Order,
                       Media = Convert.ToDouble(Math.Round(media.Media, 2)),
                       Revoked = bloco.Revoked
                   }).ToList();


            List<TestAverageItens> lstTestGroupByTest = mediasOrdenadas.GroupBy(u => u.Esc_id).Select((n) => new TestAverageItens { EscId = n.Key, Items = n.ToList().Select(s => new TestAverageItemPerformanceDTO { Esc_id = s.Esc_id, Item_id = s.Item_id, Order = s.Order, Media = Convert.ToDouble(Math.Round(s.Media, 2)), Revoked = s.Revoked }).OrderBy(p => p.Order).ToList() }).ToList();

            List<TestAverageItemPerformanceDTO> lstTestAverageItemPerformanceDTO = new List<TestAverageItemPerformanceDTO>();

            int ordem = 0;
            foreach (var bloco in blocos)
            {
                TestAverageItemPerformanceDTO testAverageItemPerformanceDTO = new TestAverageItemPerformanceDTO();
                testAverageItemPerformanceDTO.Media = 0;
                testAverageItemPerformanceDTO.Item_id = bloco.Item_Id;
                testAverageItemPerformanceDTO.Discipline_Id = bloco.Discipline_Id;
                testAverageItemPerformanceDTO.Order = ordem;
                testAverageItemPerformanceDTO.Revoked = bloco.Revoked;
                lstTestAverageItemPerformanceDTO.Add(testAverageItemPerformanceDTO);
                ordem++;
            }

            List<TestAverageItens> query = (from escola in escolas
                                            join media in lstTestGroupByTest on Convert.ToInt32(escola.EntityId) equals media.EscId
                                            select new TestAverageItens { EscId = Convert.ToInt32(escola.EntityId), EscName = escola.Description, Items = (media != null ? media.Items : lstTestAverageItemPerformanceDTO) })
                                            .ToList();

            return new TestAverageItensViewModel { success = true, lista = query };
        }

        public PerformanceItemViewModel GetPerformanceTree(long test_id, long subGroup_id, long tcp_id, SYS_Usuario usuario, SYS_Grupo grupo, Guid? dre_id, int? esc_id, Guid? uad_id, bool? export = false, bool? showBaseText = true)
        {
            PerformanceItemViewModel result = new PerformanceItemViewModel();
            result.tests = new List<TestResult>();
            List<GestaoEscolar.Entities.ESC_Escola> schoolsVisUa = new List<GestaoEscolar.Entities.ESC_Escola>();

            if (grupo.vis_id == (byte)EnumSYS_Visao.UnidadeAdministrativa && esc_id == null)
            {
                schoolsVisUa = _escolaBusiness.LoadSimple(usuario, grupo, Guid.Empty).ToList();
                if (schoolsVisUa != null && schoolsVisUa.Count() > 0)
                {
                    esc_id = schoolsVisUa.FirstOrDefault().esc_id;
                    uad_id = schoolsVisUa.FirstOrDefault().uad_idSuperiorGestao;
                    dre_id = schoolsVisUa.FirstOrDefault().uad_idSuperiorGestao;
                }
            }

            List<CorrectionResults> correctionResults = new List<CorrectionResults>();

            List<CorrectionResults> correctionResultsSME = new List<CorrectionResults>();

            List<AdheredEntityDTO> dres = new List<AdheredEntityDTO>();
            List<TestAverageItemPerformanceDTO> items = new List<TestAverageItemPerformanceDTO>();
            List<TestResult> tests = new List<TestResult>();
            List<TeamsDTO> teamsByTcpId = new List<TeamsDTO>();
            List<long> parTest = new List<long>();

            if (subGroup_id > 0)
            {
                tests = _testBusiness.GetTestsBySubGroupTcpId(subGroup_id, tcp_id).ToList();
                if (tests != null)
                {
                    if (test_id == 0)
                        test_id = tests.FirstOrDefault().TestId;

                    result.tests.AddRange(tests);
                    //dres.AddRange(_adherenceBusiness.GetAdheredDreSimpleReportItem(lstTest, usuario, grupo, 0, 0).Distinct().ToList());
                }
            }

            result.test_id = test_id;
            parTest.Add(test_id);

            dres.AddRange(_adherenceBusiness.GetAdheredDreSimple(test_id, usuario, grupo, 0, 0).ToList());

            if (uad_id != null || dre_id != null)
            {
                dres = dres.FindAll(p => new Guid(p.EntityId) == (dre_id == null ? uad_id : dre_id));
            }

            items.AddRange(Task.Run(() => _correctionResultsRepository.GetTestAverageItemPerformanceByDre(parTest)).Result.ToList());
            teamsByTcpId.AddRange(_adherenceBusiness.GetSectionByTestAndTcpId(parTest, uad_id, esc_id, tcp_id == 0 ? (long?)null : tcp_id, usuario, grupo));
            correctionResultsSME.AddRange(Task.Run(() => _correctionResultsRepository.GetByTest(parTest)).Result);
            correctionResults.AddRange(correctionResultsSME.Where(x => (teamsByTcpId.Any(t => t.tur_id == x.Tur_id)) || tcp_id == 0));

            #region Monta estrutura com as informações de itens e disciplinas, porém sem as médias
            List<DisciplinePerformanceViewModel> disciplinesViewModel = new List<DisciplinePerformanceViewModel>();
            var blocks = _blockBusiness.GetBlocksByItensTests(parTest);

            foreach (var discipline in items.Select(x => x.Discipline_Id).Distinct())
            {
                DisciplinePerformanceViewModel discViewModel = new DisciplinePerformanceViewModel();

                discViewModel.id = discipline;
                discViewModel.name = _disciplineBusiness.Get(discipline).Description;

                discViewModel.itens = new List<ItemPerformanceViewModel>();

                List<long> lstItens = items.Where(x => x.Discipline_Id == discipline).Select(x => x.Item_id).Distinct().ToList();

                var itemNovo = _itemBusiness.GetItemSummaryById(parTest, lstItens);

                var itemVideos = itemFileBusiness.GetVideosByLstItemId(lstItens);

                foreach (var item in items.Where(x => x.Discipline_Id == discipline).Select(x => x.Item_id).Distinct())
                {
                    ItemPerformanceViewModel itemViewModel = new ItemPerformanceViewModel();
                    var entity = itemNovo.FirstOrDefault(p => p.Id == item);

                    if (test_id > 0)
                    {
                        entity.BlockItems = blocks.Where(p => p.Test_Id == test_id).FirstOrDefault().BlockItems;
                    }
                    else
                    {
                        entity.BlockItems = blocks.FirstOrDefault(p => p.BlockItems.Any(q => q.Item_Id == item)).BlockItems;
                    }

                    itemViewModel.id = item;
                    itemViewModel.itemCode = entity.ItemCode;
                    itemViewModel.statement = entity.Statement;
                    itemViewModel.order = entity.BlockItems.Where(i => i.State == (Byte)EnumState.ativo && i.Item_Id == item).FirstOrDefault().Order;
                    itemViewModel.revoked = entity.Revoked == null ? false : entity.Revoked.Value;
                    if(showBaseText ?? true)
                        itemViewModel.baseText = entity.BaseText == null ? "<p>O item não possui texto base.</p>" : entity.BaseText.Description;
                    itemViewModel.habilidades = new List<SkillsViewModel>();
                    itemViewModel.discipline_id = entity.EvaluationMatrix.Discipline.Id;
                    itemViewModel.discipline_name = entity.EvaluationMatrix.Discipline.Description;
                    itemViewModel.videos = itemVideos.Where(p=>p.Item_Id == item).ToList();

                    foreach (var skill in entity.ItemSkills.Where(x => x.State == 1))
                    {
                        SkillsViewModel habilidade = new SkillsViewModel();
                        habilidade.id = skill.Skill.Id;
                        habilidade.code = skill.Skill.Code;
                        habilidade.description = skill.Skill.Description;

                        itemViewModel.habilidades.Add(habilidade);
                    }

                    itemViewModel.alternativas = new List<AlternativesHitsViewModel>();
                    foreach (var alternative in entity.Alternatives)
                    {
                        AlternativesHitsViewModel alternativa = new AlternativesHitsViewModel();
                        alternativa.id = alternative.Id;
                        alternativa.description = alternative.Description;
                        alternativa.justificative = alternative.Justificative;
                        alternativa.order = alternative.Order;
                        alternativa.correct = alternative.Correct;
                        alternativa.numeration = alternative.Numeration;

                        itemViewModel.alternativas.Add(alternativa);
                    }
                    discViewModel.itens.Add(itemViewModel);
                }
                disciplinesViewModel.Add(discViewModel);
            }
            #endregion

            result.disciplinas = CalcAveragesByDiscipline(disciplinesViewModel, correctionResultsSME);
            result.media = result.disciplinas.Select(x => x.attempts).Sum() > 0 ? Math.Round(((double)result.disciplinas.Select(x => x.hits).Sum() / result.disciplinas.Select(x => x.attempts).Sum()) * 100, 2) : 0;
            result.dres = new List<DrePerformanceViewModel>();

            if (result.disciplinas.Count > 0)
            {
                foreach (var dre in dres)
                {
                    DrePerformanceViewModel dreViewModel = new DrePerformanceViewModel();

                    dreViewModel.id = new Guid(dre.EntityId);
                    dreViewModel.name = dre.Description;

                    var dreAlunos = correctionResults.Where(x => x.Dre_id.ToString().ToUpper() == dreViewModel.id.ToString().ToUpper()).ToList();

                    //var dreAlunos = correctionResults.Where(x => x.Dre_id.ToString().ToUpper() == dreViewModel.id.ToString().ToUpper() &&
                    //                                         x.Answers.Any(a => result.disciplinas.Any(d => d.id == a.Discipline_Id))).ToList();

                    if (dreAlunos.Count() > 0)
                    {
                        dreViewModel.disciplinas = CalcAveragesByDiscipline(disciplinesViewModel, dreAlunos);
                        dreViewModel.media = dreViewModel.disciplinas.Select(x => x.attempts).Sum() > 0 ? Math.Round(((double)dreViewModel.disciplinas.Select(x => x.hits).Sum() / dreViewModel.disciplinas.Select(x => x.attempts).Sum()) * 100, 2) : 0;
                        dreViewModel.escolas = new List<SchoolPerformanceViewModel>();

                        List<AdheredEntityDTO> schools = new List<AdheredEntityDTO>();
                        if (test_id > 0)
                        {
                            schools.AddRange(_adherenceBusiness.GetAdheredSchoolSimple(test_id, dreViewModel.id, usuario, grupo, 0, 0).Where(x => Convert.ToInt32(x.EntityId) == esc_id || esc_id == null).ToList());
                        }
                        else
                        {
                            schools.AddRange(_adherenceBusiness.GetAdheredSchoolSimpleReportItem(parTest, dreViewModel.id, usuario, grupo, 0, 0).Where(x => Convert.ToInt32(x.EntityId) == esc_id || esc_id == null).ToList());
                        }

                        foreach (var school in schools)
                        {
                            SchoolPerformanceViewModel escola = new SchoolPerformanceViewModel();
                            escola.id = Convert.ToInt32(school.EntityId);
                            escola.name = school.Description;

                            var escCorrections = correctionResults.Where(x => x.Esc_id == escola.id).ToList();

                            if (escCorrections.Count() > 0)
                            {
                                escola.disciplinas = CalcAveragesByDiscipline(disciplinesViewModel, escCorrections);
                                escola.media = escola.disciplinas.Select(x => x.attempts).Sum() > 0 ? Math.Round(((double)escola.disciplinas.Select(x => x.hits).Sum() / escola.disciplinas.Select(x => x.attempts).Sum()) * 100, 2) : 0;

                                escola.turmas = new List<TeamPerformanceViewModel>();

                                List<AdheredEntityDTO> sections = new List<AdheredEntityDTO>();
                                sections.AddRange(teamsByTcpId.Where(p => p.esc_id.Value == escola.id).Select(q => new AdheredEntityDTO { EntityId = q.tur_id.ToString(), Description = q.tur_codigo, Test_Id = q.test_id }).Distinct());
                                sections = sections.FindAll(p => correctionResults.Any(x => x.Test_id == p.Test_Id && x.Tur_id == Convert.ToInt64(p.EntityId)));

                                if (esc_id != null && dre_id != null)
                                {
                                    var results = sections.GroupBy(u => u.EntityId).Select(grp => grp.FirstOrDefault()).ToList();

                                    foreach (var section in results)
                                    {
                                        TeamPerformanceViewModel turma = new TeamPerformanceViewModel();

                                        turma.id = Convert.ToInt64(section.EntityId);
                                        turma.test_id = section.Test_Id;
                                        turma.name = section.Description;
                                        turma.disciplinas = CalcAveragesByDiscipline(disciplinesViewModel, correctionResults.Where(x => x.Tur_id == turma.id).ToList());
                                        turma.media = turma.disciplinas.Select(x => x.attempts).Sum() > 0 ? Math.Round(((double)turma.disciplinas.Select(x => x.hits).Sum() / turma.disciplinas.Select(x => x.attempts).Sum()) * 100, 2) : 0;

                                        escola.turmas.Add(turma);
                                    }
                                    if (escola.turmas.Count() > 0)
                                        dreViewModel.escolas.Add(escola);
                                }
                                else if (sections.Count > 0)
                                {
                                    dreViewModel.escolas.Add(escola);
                                }
                            }
                        }

                        if (dreViewModel.escolas.Count() > 0)
                            result.dres.Add(dreViewModel);
                    }
                }
            }

            result.level = 1;
            if (esc_id != null && dre_id != null)
            {
                result.level = 3;
                result.escolaSelecionada = Convert.ToInt64(esc_id);
                result.dreSelecionada = dre_id;
            }
            else if (grupo.vis_id == (byte)EnumSYS_Visao.UnidadeAdministrativa)
            {
                var schools = schoolsVisUa;
                if (schools != null && schools.Count() > 0)
                {
                    var escola = schools.FirstOrDefault();
                    if (result.dres.Where(x => x.escolas.Any(y => y.id == Convert.ToInt64(escola.esc_id))).Count() > 0)
                    {
                        result.escolaSelecionada = Convert.ToInt64(escola.esc_id);
                        result.dreSelecionada = new Guid(dres[0].EntityId);
                        result.level = 3;
                    }
                }
            }
            else if (grupo.vis_id == (byte)EnumSYS_Visao.Gestao)
            {
                var uad = SYS_UsuarioGrupoUABO.GetSelect(usuario.usu_id, grupo.gru_id).Rows[0]["uad_id"];
                result.level = 2;
                result.dreSelecionada = new Guid(uad.ToString());
            }

            if (result.dres.Count() == 0)
                result.erro = "Sem dados disponíveis para esta prova/usuário.";

           return result;
        }

        private List<DisciplinePerformanceViewModel> CalcAveragesByDiscipline(IEnumerable<DisciplinePerformanceViewModel> disciplines, List<CorrectionResults> correctionResults)
        {
            //cria um clone das disciplinas, para setar em cada nivel da arvore de desempenho
            List<DisciplinePerformanceViewModel> result = disciplines.Select(d => new DisciplinePerformanceViewModel
            {
                id = d.id,
                name = d.name,
                itens = d.itens.Select(i => new ItemPerformanceViewModel
                {
                    id = i.id,
                    description = i.description,
                    baseText = i.baseText,
                    statement = i.statement,
                    itemCode = i.itemCode,
                    order = i.order,
                    habilidades = i.habilidades,
                    discipline_id = i.discipline_id,
                    discipline_name = i.discipline_name,
                    alternativas = i.alternativas.Select(a => new AlternativesHitsViewModel
                    {
                        id = a.id,
                        correct = a.correct,
                        description = a.description,
                        justificative = a.justificative,
                        order = a.order,
                        numeration = a.numeration
                    }).ToList(),
                    videos = i.videos
                }).ToList()
            }).ToList();

            foreach (var discipline in result)
            {
                foreach (var item in discipline.itens)
                {
                    int hitsItem = 0;
                    int quantidadeAlunos = 0;
                    double media = 0;
                    var quantidadeitens = 0;
                    foreach (var correction in correctionResults)
                    {
                        quantidadeAlunos = quantidadeAlunos + correction.Students.Where(s => s.Performance != null).Count();

                        foreach (var s in correction.Students)
                        {
                            hitsItem = hitsItem + (s.Alternatives != null ? s.Alternatives.Where(a => a.Item_Id == item.id && a.Correct).Count() : 0);
                        }

                        foreach (var average in correction.Statistics.Averages.Where(p => p.Item_Id == item.id))
                        {
                            media = media + average.Average;
                            quantidadeitens = quantidadeitens + 1;
                        }
                    }

                    foreach (var correction in correctionResults)
                    {
                        foreach (var alternative in item.alternativas)
                        {
                            alternative.hits = alternative.hits + correction.Students.Where(x => (x.Alternatives != null ? x.Alternatives.Where(a => a.Item_Id == item.id && a.Alternative_Id == alternative.id).Count() : 0) > 0).Count();
                            alternative.media = quantidadeAlunos == 0 ? 0 : Math.Round(((double)alternative.hits / quantidadeAlunos) * 100, 2);
                        }
                    }

                    item.media = correctionResults.Count() == 0 ? 0 : Math.Round(((double)hitsItem / quantidadeAlunos) * 100, 2);
                    discipline.hits = discipline.hits + hitsItem;
                    discipline.attempts = discipline.attempts + quantidadeAlunos;
                }
                discipline.media = discipline.attempts == 0 ? 0 : Math.Round(((double)discipline.hits / discipline.attempts) * 100, 2);
            }

            return result;
        }

        public EntityFile ExportReportDre(List<DrePerformanceViewModel> lista, List<DisciplinePerformanceViewModel> mediasSME, TypeReportsPerformanceExport typeExport, string separator, string virtualDirectory, string physicalDirectory, SYS_Usuario usuario, long? discipline_id)
        {
            EntityFile ret = new EntityFile();
            StringBuilder stringBuilder = new StringBuilder();
            string fileName = string.Empty;

            if (lista != null)
            {
                if (typeExport == TypeReportsPerformanceExport.Dre)
                {
                    int quantidadeItens = lista.FirstOrDefault().disciplinas.Where(p => discipline_id == null || p.id == discipline_id).FirstOrDefault().itens.Count;

                    StringBuilder sb = new StringBuilder();
                    sb.Append("DRE{0}");
                   
                        foreach (var item in lista.FirstOrDefault().disciplinas.Where(p => discipline_id == null || p.id == discipline_id).FirstOrDefault().itens.OrderBy(p=>p.order))
                        {
                            sb.Append("Item " + (item.order + 1) + "{0}");
                        }
                    
                    string itens = sb.ToString();

                    fileName = "Rel_Item_Performance_DRE";

                    sb = new StringBuilder();
                    sb.Append("Geral{0}");

                    DisciplinePerformanceViewModel mediasSMEDisciplina = mediasSME.Where(p => discipline_id == null || p.id == discipline_id).FirstOrDefault();

                    foreach (ItemPerformanceViewModel item in mediasSMEDisciplina.itens.OrderBy(p => p.order))
                    {
                        sb.Append(item.media + "%{0}");
                    }
                    string mediasItensSME = sb.ToString();

                    stringBuilder.Append(string.Format(itens, separator));
                    stringBuilder.AppendLine();
                    stringBuilder.Append(string.Format(mediasItensSME, separator));
                    stringBuilder.AppendLine();

                    foreach (DrePerformanceViewModel dreItens in lista)
                    {

                        sb = new StringBuilder();
                        sb.Append(dreItens.name + "{0}");

                        foreach (var item in dreItens.disciplinas.Where(p => discipline_id == null || p.id == discipline_id).FirstOrDefault().itens.OrderBy(p => p.order))
                        {
                            sb.Append(item.media + "%{0}");
                        }
                        string mediasItens = sb.ToString();

                        stringBuilder.Append(string.Format(mediasItens, separator));
                        stringBuilder.AppendLine();
                    }
                }              
            }

            var fileContent = stringBuilder.ToString();
            if (!string.IsNullOrEmpty(fileContent))
            {
                byte[] buffer = System.Text.Encoding.Default.GetBytes(fileContent);
                string originalName = string.Format("{0}.csv", fileName);
                string name = string.Format("{0}.csv", Guid.NewGuid());
                string contentType = MimeType.CSV.GetDescription();

                var csvFiles = _fileBusiness.GetAllFilesByType(EnumFileType.ExportReportItemPerformance, DateTime.Now.AddDays(-1));
                if (csvFiles != null && csvFiles.Count() > 0)
                {
                    _fileBusiness.DeletePhysicalFiles(csvFiles.ToList(), physicalDirectory);
                    _fileBusiness.DeleteFilesByType(EnumFileType.ExportReportItemPerformance, DateTime.Now.AddDays(-1));
                }

                ret = _storage.Save(buffer, name, contentType, EnumFileType.ExportReportItemPerformance.GetDescription(), virtualDirectory, physicalDirectory, out ret);
                if (ret.Validate.IsValid)
                {
                    ret.Name = name;
                    ret.ContentType = contentType;
                    ret.OriginalName = StringHelper.Normalize(originalName);
                    ret.OwnerId = 0;
                    ret.ParentOwnerId = 0;
                    ret.OwnerType = (byte)EnumFileType.ExportReportItemPerformance;
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

        public EntityFile ExportReport(List<TestAverageItens> lista, List<TestAverageItemPerformanceDTO> mediasSME, TypeReportsPerformanceExport typeExport, string separator, string virtualDirectory, string physicalDirectory, SYS_Usuario usuario, long? discipline_id)
        {
            EntityFile ret = new EntityFile();
            StringBuilder stringBuilder = new StringBuilder();
            string fileName = string.Empty;

            if (lista != null)
            {
                int quantidadeItens = lista.FirstOrDefault().Items.Count;

                StringBuilder sb = new StringBuilder();
                sb.Append("Escola{0}");
                foreach (TestAverageItemPerformanceDTO testAverageItemPerformanceDTO in lista.FirstOrDefault().Items)
                {
                    sb.Append("Item " + (testAverageItemPerformanceDTO.Order + 1) + "{0}");
                }
                string itens = sb.ToString();

                fileName = "Rel_Item_Performance_Escola";
                stringBuilder.Append(string.Format(itens, separator));
                stringBuilder.AppendLine();

                foreach (TestAverageItens testAverageItens in lista)
                {
                    sb = new StringBuilder();
                    sb.Append(testAverageItens.EscName + "{0}");
                    foreach (TestAverageItemPerformanceDTO testAverageItemPerformanceDTO in testAverageItens.Items)
                    {
                        sb.Append(testAverageItemPerformanceDTO.Media + "%{0}");
                    }
                    string mediasItens = sb.ToString();

                    stringBuilder.Append(string.Format(mediasItens, separator));
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

                var csvFiles = _fileBusiness.GetAllFilesByType(EnumFileType.ExportReportItemPerformance, DateTime.Now.AddDays(-1));
                if (csvFiles != null && csvFiles.Count() > 0)
                {
                    _fileBusiness.DeletePhysicalFiles(csvFiles.ToList(), physicalDirectory);
                    _fileBusiness.DeleteFilesByType(EnumFileType.ExportReportItemPerformance, DateTime.Now.AddDays(-1));
                }

                ret = _storage.Save(buffer, name, contentType, EnumFileType.ExportReportItemPerformance.GetDescription(), virtualDirectory, physicalDirectory, out ret);
                if (ret.Validate.IsValid)
                {
                    ret.Name = name;
                    ret.ContentType = contentType;
                    ret.OriginalName = StringHelper.Normalize(originalName);
                    ret.OwnerId = 0;
                    ret.ParentOwnerId = 0;
                    ret.OwnerType = (byte)EnumFileType.ExportReportItemPerformance;
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
    }
}
