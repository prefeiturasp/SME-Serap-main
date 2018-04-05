describe('FormItemController.test.js', function () {

    beforeEach(module('services', 'filters', 'directives', 'ngTagsInput', 'appMain'));

    var $controller,
        $notification,
        $rootScope,
        DisciplineModel,
        ItemModel,
        $util, 
        EvaluationMatrixModel,
        ItemLevelModel,
        ItemTypeModel,
        ItemSituationModel,
        SkillModel,
        EvaluationMatrixCourseCurriculumGradeModel,
        ParameterModel;

    beforeEach(inject(function (_$rootScope_, _$controller_, _$notification_,
        _DisciplineModel_, _ItemModel_, _$util_, _EvaluationMatrixModel_, _ItemLevelModel_,
        _ItemTypeModel_, _ItemSituationModel_, _SkillModel_, _EvaluationMatrixCourseCurriculumGradeModel_, 
        _ParameterModel_) {

        $rootScope = _$rootScope_;
        $controller = _$controller_;
        $notification = _$notification_;
        DisciplineModel = _DisciplineModel_;
        ItemModel = _ItemModel_;
        $util = _$util_;
        EvaluationMatrixModel = _EvaluationMatrixModel_;
        ItemLevelModel = _ItemLevelModel_;
        ItemTypeModel = _ItemTypeModel_;
        ItemSituationModel = _ItemSituationModel_;
        SkillModel = _SkillModel_;
        EvaluationMatrixCourseCurriculumGradeModel = _EvaluationMatrixCourseCurriculumGradeModel_;
        ParameterModel = _ParameterModel_;
    }));

    describe('functions defined', function () {
        it('deve testar a existências dos métodos ->', function () {
            var $scope = $rootScope.$new();
            var controller = $controller('FormItemController', { $scope: $scope });
            expect($scope.relacaoDisciplina).toBeDefined();
            expect($scope.setParamDefaultEtapa1).toBeDefined();
            expect($scope.carregarSkills).toBeDefined();
            expect($scope.carregarCascadeSkill).toBeDefined();
            expect($scope.getSubstractSkill).toBeDefined();
            expect($scope.reloadPage).toBeDefined();
            expect($scope.addQtdAlternativas).toBeDefined();
            expect($scope.removeQtdAlternativas).toBeDefined();
            expect($scope.changeItemType).toBeDefined();
            expect($scope.newValueItemType).toBeDefined();
            expect($scope.setValueItemTypeDefault).toBeDefined();
            expect($scope.activeModalText).toBeDefined();
            expect($scope.transformTags).toBeDefined();
            expect($scope.add).toBeDefined();
            expect($scope.saveredirect).toBeDefined();
            expect($scope.save).toBeDefined();
            expect($scope.cancel).toBeDefined();
            expect($scope.edit).toBeDefined();
            expect($scope.setdelete).toBeDefined();
            expect($scope.delete).toBeDefined();
            expect($scope.canceldelete).toBeDefined();
            expect($scope.finish).toBeDefined();
            expect($scope.finishEdit).toBeDefined();
            expect($scope.update).toBeDefined();
            expect($scope.setNewValueTextBase).toBeDefined();
            expect($scope.setNewValueEnunciationTest).toBeDefined();
            expect($scope.reset).toBeDefined();
            expect($scope.createItem).toBeDefined();
            expect($scope.restoreForEdit).toBeDefined();
            expect($scope.editCancel).toBeDefined();
            expect($scope.wrapper).toBeDefined();
            expect($scope.authorizeChangeBaseText).toBeDefined();
            expect($scope.notAuthorizeChangeBaseText).toBeDefined();
            expect($scope.validade).toBeDefined();
            expect($scope.previus).toBeDefined();
            expect($scope.next).toBeDefined();
            expect($scope.travarEtapa1).toBeDefined();
            expect($scope.distratorSelectionControll).toBeDefined();
            expect($scope.controllUp).toBeDefined();
            expect($scope.controllDown).toBeDefined();
            expect($scope.versionControll).toBeDefined();
            expect($scope.getGroupIds).toBeDefined();
            expect($scope.copy).toBeDefined();
            expect($scope.joinKeywords).toBeDefined();
            expect($scope.TCTValidation).toBeDefined();
            expect($scope.TRIValidation).toBeDefined();
            expect($scope.previewPrint).toBeDefined();
            expect($scope.previewPrintBaseText).toBeDefined();
            expect($scope.setEditAlternative).toBeDefined();
            expect($scope.closeEditAlternative).toBeDefined();
        });
    });

    describe('loaded function call', function () {
        it('deve testar a chamada da function loaded() -> load', function () {
            var $scope = $rootScope.$new();
            var controller = $controller('FormItemController', { $scope: $scope });
            $scope.params = {
                id: 1,
                i: 3
            };
            spyOn($notification, "clear");
            spyOn($scope, "config");
            $scope.loaded();
            expect($scope.comommItemID).toBe(3);
            expect($scope.editMode).toBeTruthy();
            expect($scope.createMode).toBeFalsy();
            expect($notification.clear).toHaveBeenCalled();
            expect($scope.config).toHaveBeenCalled();
        });
        it('deve testar a chamada da function loaded() -> create', function () {
            var $scope = $rootScope.$new();
            var controller = $controller('FormItemController', { $scope: $scope });
            spyOn($notification, "clear");
            spyOn($scope, "config");
            $scope.loaded();
            expect($scope.comommItemID).not.toBeDefined();
            expect($scope.createMode).toBeTruthy();
            expect($scope.editMode).toBeFalsy();
            expect($notification.clear).toHaveBeenCalled();
            expect($scope.config).toHaveBeenCalled();
        });
    });

});