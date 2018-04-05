using GestaoAvaliacao.Entities;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.MongoEntities.DTO
{
    public class TestAverageItemPerformanceDTO
    {
        public Guid Dre_id { get; set; }
        public int Esc_id { get; set; }
        public long Item_id { get; set; }
        public long Discipline_Id { get; set; }
        public int Order { get; set; }
        public double Media { get; set; }
        public bool Revoked { get; set; }
    }

    public class TestAverageItens
    {
        public Guid DreId { get; set; }
        public string DreName { get; set; }
        public int EscId { get; set; }
        public string EscName { get; set; }
        public List<TestAverageItemPerformanceDTO> Items { get; set; }
    }

    public class TestAverageItensViewModel
    {
        public List<TestAverageItens> lista { get; set; }
        public List<TestAverageItemPerformanceDTO> MediasSME { get; set; }
        public bool success { get; set; }
    }

    public class PerformanceItemViewModel
    {
        public double media { get; set; }
        public int level { get; set; }
        public long? escolaSelecionada { get; set; }
        public Guid? dreSelecionada { get; set; }
        public String erro { get; set; }
        public List<DrePerformanceViewModel> dres { get; set; }
        public List<DisciplinePerformanceViewModel> disciplinas { get; set; }
        public List<TestResult> tests { get; set; }
        public long test_id { get; set; }
    }

    public class SkillsTreeViewModel
    {
        public ModelSkillLevel modelSkillLevels { get; set; }
    }

    public class DrePerformanceViewModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public double media { get; set; }
        public List<SchoolPerformanceViewModel> escolas { get; set; }
        public List<DisciplinePerformanceViewModel> disciplinas { get; set; }
    }

    public class SchoolPerformanceViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public double media { get; set; }
        public List<TeamPerformanceViewModel> turmas { get; set; }
        public List<DisciplinePerformanceViewModel> disciplinas { get; set; }
    }

    public class TeamPerformanceViewModel
    {
        public long id { get; set; }
        public long test_id { get; set; }
        public string name { get; set; }
        public double media { get; set; }
        public List<CorrectionResults> alunos { get; set; }
        public List<DisciplinePerformanceViewModel> disciplinas { get; set; }
    }

    [Serializable]
    public class DisciplinePerformanceViewModel 
    {
        public long id { get; set; }
        public string name { get; set; }
        public double media { get; set; }
        public int hits { get; set; }
        public int attempts { get; set; }
        public List<ItemPerformanceViewModel> itens { get; set; }

    }
    [Serializable]
    public class ItemPerformanceViewModel
    {
        public long id { get; set; }
        public string description { get; set; }
        public string itemCode { get; set; }
        public string baseText { get; set; }
        public string statement { get; set; }
        public double media { get; set; }
        public int order { get; set; }
        public bool revoked { get; set; }
        public long discipline_id { get; set; }
        public string discipline_name { get; set; }
        public List<ItemFile> videos { get; set; }
        public List<AlternativesHitsViewModel> alternativas { get; set; }
        public List<SkillsViewModel> habilidades { get; set; }
    }
    [Serializable]
    public class AlternativesHitsViewModel
    {
        public long id { get; set; }
        public string description { get; set; }
        public string numeration { get; set; }
        public string justificative { get; set; }
        public double media { get; set; }
        public int order { get; set; }
        public int hits { get; set; }
        public bool correct { get; set; }

    }
    [Serializable]
    public class SkillsViewModel
    {
        public long id { get; set; }
        public string code { get; set; }
        public string description { get; set; }

    }




}
