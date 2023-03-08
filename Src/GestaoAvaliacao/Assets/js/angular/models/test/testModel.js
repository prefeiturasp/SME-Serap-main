/* 
* ModelSkillLevel-Model
*/
(function () {
    angular.module('services').factory('TestModel', ['$resource', function ($resource) {

        // Model
        var model = {

            // CHAMADA ETAPA 1
            'loadByLevelEducationModality': {
                method: 'GET',
                url: base_url('/CurriculumGrade/LoadByLevelEducationModality')
            },
            'loadSituation': {
                method: 'GET',
                url: base_url('ItemSituation/Load')
            },
            'loadLevels': {
                method: 'GET',
                url: base_url('ItemLevel/Load')
            },

            'searchItems': {
                method: 'GET',
                url: base_url('Item/SearchItems')
            },

            'getBaseTextItems': {
                method: 'GET',
                url: base_url('Item/GetBaseTextItems')
            },

            'getItemSummaryById': {
                method: 'GET',
                url: base_url('Item/GetItemSummaryById')
            },

            'getItemSummaryByIdTest': {
                method: 'GET',
                url: base_url('Item/GetItemSummaryByIdTest')
            },

            'save': {
                method: 'POST',
                url: base_url('Test/Save')
            },

            'loadByUserGroup': {
                method: 'GET',
                url: base_url('TestType/LoadByUserGroup')
            },
            'loadTest': {
                method: 'GET',
                url: base_url('Test/GetTestById')
            },
            'searchDisciplinesSaves': {
                method: 'GET',
                url: base_url('Discipline/SearchDisciplinesSaves')
            },
            'getAll': {
                method: 'GET',
                url: base_url('PerformanceLevel/GetAll')
            },
            'findTest': {
                method: 'GET',
                url: base_url('TestType/FindTest')
            },
            /////////////////////////////////////////////
            // CHAMADAS DA ETAPA 2

            'saveTestTaiCurriculumGrade': {
                method: 'POST',
                url: base_url('Test/TestTaiCurriculumGradeSave')
            },

            'loadTestTaiCurriculumGrade': {
                method: 'GET',
                url: base_url('Test/GetListTestTaiCurriculumGrade')
            },

            'carregaAreaConhecimento': {
                method: 'GET',
                url: base_url('Item/loadallknowledgeareaactive')
            },

            'loadLevelEducation': {
                method: 'GET',
                url: base_url('LevelEducation/Load')
            },

            'loadModality': {
                method: 'GET',
                url: base_url('Modality/Load')
            },

            'getComboByDiscipline': {
                method: 'GET',
                url: base_url('EvaluationMatrix/GetComboByDiscipline')
            },
            'getByMatriz': {
                method: 'GET',
                url: base_url('Skill/GetByMatrix')
            },
            'getByParent': {
                method: 'GET',
                url: base_url('Skill/GetByParent')
            },
            // CHAMADAS PARA BLOCOS
            'loadBlock': {
                method: 'GET',
                url: base_url('Block/GetTestBlocks')
            },
            'visualizar': {
                method: 'GET',
                url: base_url('Block/GetBlockItens')
            },
            'getBlockKnowledgeAreas': {
                method: 'GET',
                url: base_url('Block/GetBlockKnowledgeAreas')
            },
            'getBlocksByItensTests': {
                method: 'GET',
                url: base_url('Block/GetBlocksByItensTests')
            },
            'getItemVersions': {
                method: 'GET',
                url: base_url('Item/GetItemVersions')
            },
            'saveChangeItem': {
                method: 'POST',
                url: base_url('Item/SaveChangeItem')
            },
            'removerBlock': {
                method: 'POST',
                url: base_url('Block/RemoveBlockItem')
            },

            'searchBlock': {
                method: 'GET',
                url: base_url('Block/SearchBlockItem')
            },

            'saveBlock': {
                method: 'POST',
                url: base_url('Block/Save')
            },

            'deleteBlock': {
                method: 'POST',
                url: base_url('Block/Delete')
            },

            'deleteBlockItems': {
                method: 'POST',
                url: base_url('Block/DeleteBlockItems')
            },            

            'saveKnowLedgeAreaOrder': {
                method: 'POST',
                url: base_url('Block/SaveKnowLedgeAreaOrder')
            },

            'loadBlockChains': {
                method: 'GET',
                url: base_url('Block/GetTestBlockChains')
            },

            'searchBlockChain': {
                method: 'GET',
                url: base_url('Block/SearchBlockChainItem')
            },

            'saveBlockChain': {
                method: 'POST',
                url: base_url('BlockChain/Save')
            },

            'deleteBlockChain': {
                method: 'DELETE',
                url: base_url('BlockChain/RemoveBlockChainItem')
            },

            'deleteBlockChainItems': {
                method: 'DELETE',
                url: base_url('BlockChain/DeleteBlockChainItems')
            },
            /////////////////////////////////////////////
            // CHAMADAS DA ETAPA 4
            'getAllByTest': {
                method: 'GET',
                url: base_url('Booklet/GetAllByTest')
            },
            'getHTMLTest': {
                method: 'GET',
                url: base_url('Booklet/GetHTMLObjTest')
            },
            'generateTest': {
                method: 'GET',
                url: base_url('Booklet/GenerateTest')
            },
            'finallyTest': {
                method: 'POST',
                url: base_url('Test/FinallyTest')
            },
            /////////////////////////////////////////////

            'getTestByDate': {
                method: 'GET',
                url: base_url('Test/GetTestByDate')
            },
            'getInfoTestReport': {
                method: 'GET',
                url: base_url('Test/GetInfoTestReport')
            },
            'getInfoTestCurriculumGrade': {
                method: 'GET',
                url: base_url('Test/GetInfoTestCurriculumGrade')
            },
            'getInfoUadReport': {
                method: 'GET',
                url: base_url('Test/GetInfoUadReport')
            },
            'getInfoEscReport': {
                method: 'GET',
                url: base_url('Test/GetInfoEscReport')
            },
            'getInfoTurReport': {
                method: 'GET',
                url: base_url('Test/GetInfoTurReport')
            },
            'checkFilesExists': {
                method: 'GET',
                url: base_url('Booklet/CheckFilesExists')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();
