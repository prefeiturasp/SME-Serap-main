using System.Collections.Generic;

namespace GestaoAvaliacao.API.Models
{
    public class ItemModel
    {
        public int ItemCodeVersion { get; set; }
        public string Statement { get; set; }
        public string DescriptorSentence { get; set; }
        public int? Proficiency { get; set; }
        public long EvaluationMatrix_Id { get; set; }
        public string Keywords { get; set; }
        public string Tips { get; set; }
        public decimal? TRICasualSetting { get; set; }
        public decimal? TRIDifficulty { get; set; }
        public decimal? TRIDiscrimination { get; set; }
        public virtual BaseTextModel BaseText { get; set; }
        public long ItemSituation_Id { get; set; }
        public long ItemType_Id { get; set; }
        public long? ItemLevel_Id { get; set; }
        public string ItemCode { get; set; }
        public int ItemVersion { get; set; }
        public int TypeCurriculumGradeId { get; set; }
        public virtual int[] ItemSkills { get; set; }
        public virtual List<AlternativeModel> Alternatives { get; set; }
        public bool IsRestrict { get; set; }
        public bool? ItemNarrated { get; set; }
        public bool? StudentStatement { get; set; }
        public bool? NarrationStudentStatement { get; set; }
        public bool? NarrationAlternatives { get; set; }
        public long? KnowledgeArea_Id { get; set; }
        public long? SubSubject_Id { get; set; }

        public virtual List<PictureModel> Pictures { get; set; }
    }

    public class BaseTextModel
    {
        public string Description { get; set; }
        public string Source { get; set; }
    }

    public class AlternativeModel
    {
        public string Description { get; set; }

        public bool Correct { get; set; }

        public int Order { get; set; }

        public string Justificative { get; set; }

        public string Numeration { get; set; }
    }
}


//Id: _item.id,
//                ItemCodeVersion: _item.itemCodeVersion,
//                Statement: _item.enunciado.Description,
//                descriptorSentence: _item.sentenca,
//                proficiency: _item.proficiencia,
//                EvaluationMatrix_Id: _item.matriz.Id,
//                Keywords: $scope.joinKeywords(_item.palavrasChave),
//                Tips: _item.dicas,
//                TRICasualSetting: (_item.tri[2].Value != undefined) ? _item.tri[2].Value.toString().replace(".", ",") : undefined,
//                TRIDifficulty: (_item.tri[1].Value != undefined) ? _item.tri[1].Value.toString().replace(".", ",") : undefined,
//                TRIDiscrimination: (_item.tri[0].Value != undefined) ? _item.tri[0].Value.toString().replace(".", ",") : undefined,
//                BaseText: _item.textobase,
//                ItemSituation_Id: _item.statusItem.Id,
//                ItemType_Id: _item.tipoItem.Id,
//                ItemLevel_Id: _item.dificuldade.objDificuldade != undefined ? _item.dificuldade.objDificuldade.Id : undefined,
//                ItemCode: _item.code,
//                ItemVersion: _item.version,
//                ItemCurriculumGrades:
//[{
//TypeCurriculumGradeId: _item.series.selected.Id
//                }],
//                ItemSkills: itemSkills,
//                Alternatives: alternatives,
//                IsRestrict: _item.IsRestrict.value,
//                ItemNarrated: $scope.item.ItemNarrated,
//                StudentStatement: $scope.item.StudentStatement,
//                NarrationStudentStatement: $scope.item.NarrationStudentStatement,
//                NarrationAlternatives: $scope.item.NarrationAlternatives,
//                KnowledgeArea_Id: $scope.area.objArea.Id,
//                SubSubject_Id: $scope.subassunto.Id