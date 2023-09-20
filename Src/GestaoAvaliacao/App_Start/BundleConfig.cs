using System.Web.Optimization;

namespace GestaoAvaliacao.App_Start
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            string angularVersion = "angular-1.4.9";
            string chartsVersion = "charts-2.0";

            #region Vendor (JS)

            bundles.Add(
                new ScriptBundle("~/bundles/vendor_js")
                .Include("~/Assets/js/Vendor/jquery-2.1.1.js")
                .Include("~/Assets/js/Vendor/easytimer.min.js")
                .Include("~/Assets/js/Vendor/jquery-ui.js")
                .Include("~/Assets/js/Vendor/plg-notify.min.js")
                .Include("~/Assets/js/Vendor/bootstrap-3.2.0.js")
                .Include("~/Assets/js/vendor/moment.js")
                .Include("~/Assets/js/vendor/{version}/angular.js".Replace("{version}", angularVersion))
                .Include("~/Assets/js/vendor/{version}/angular-route.js".Replace("{version}", angularVersion))
                .Include("~/Assets/js/vendor/{version}/angular-resource.js".Replace("{version}", angularVersion))
                .Include("~/Assets/js/vendor/{version}/angular-animate.js".Replace("{version}", angularVersion))
                .Include("~/Assets/js/vendor/{version}/angular-sanitize.js".Replace("{version}", angularVersion))
                .Include("~/Assets/js/vendor/{version}/i18n/angular-locale_pt-br.js".Replace("{version}", angularVersion))
                );
            #endregion

            #region Vendor (CSS)

            bundles.Add(
                new StyleBundle("~/bundles/vendor_css")
                .Include("~/Assets/css/vendor/bootstrap.css")
                .Include("~/Assets/css/vendor/bootstrap-theme.css")
                .Include("~/Assets/css/vendor/font-awesome.css", new CssRewriteUrlTransform())
                .Include("~/Assets/css/style.css")
                .Include("~/Assets/css/vendor/awesome-bootstrap-checkbox.css")
                );

            #endregion

            #region Angular

            bundles.Add(
                new ScriptBundle("~/bundles/angular_js")
                //modules
                .Include("~/Assets/js/angular/directives/directives.js")
                .Include("~/Assets/js/angular/services/services.js")
                .Include("~/Assets/js/angular/filters/filters.js")
                //filtros
                .Include("~/Assets/js/angular/filters/_bundle/between/between.js")
                .Include("~/Assets/js/angular/filters/_bundle/minimize/minimize.js")
                .Include("~/Assets/js/angular/filters/_bundle/tagToString/tagToString.js")
                .Include("~/Assets/js/angular/filters/_bundle/trustedHtml/trustedHtml.js")
                .Include("~/Assets/js/angular/filters/_bundle/changeBlankSpace/changeBlankSpace.js")
                .Include("~/Assets/js/angular/filters/_bundle/moment/moment.js")
                .Include("~/Assets/js/angular/filters/_bundle/capitalize/capitalize.js")
                //components
                .Include("~/Assets/js/angular/directives/_bundle/item-brief/item-brief.js")
                .Include("~/Assets/js/angular/directives/_bundle/fieldinteger/fieldinteger.js")
                .Include("~/Assets/js/angular/services/ApiSetting.js")
                .Include("~/Assets/js/angular/services/authenticationinterceptor.service.js")
                .Include("~/Assets/js/angular/services/_bundle/notification/notification.js")
                .Include("~/Assets/js/angular/directives/_bundle/alert/alert.js")
                .Include("~/Assets/js/angular/directives/_bundle/preloading/preloading.js")
                .Include("~/Assets/js/angular/directives/_bundle/ngShowLoading/ngShowLoading.js")
                .Include("~/Assets/js/angular/directives/_bundle/popover/helpers/dimensions.js")
                .Include("~/Assets/js/angular/directives/_bundle/popover/tooltip.js")
                .Include("~/Assets/js/angular/directives/_bundle/popover/popover.js")
                .Include("~/Assets/js/angular/directives/_bundle/ng-title/ng-title.js")
                .Include("~/Assets/js/angular/directives/_bundle/writemaths/rangy-core.js")
                .Include("~/Assets/js/angular/directives/_bundle/writemaths/textinputs_jquery.js")
                .Include("~/Assets/js/angular/directives/_bundle/writemaths/writemaths.js")
                .Include("~/Assets/js/angular/directives/_bundle/writemaths/writemaths-directive.js")
                //menu
                .Include("~/Assets/js/angular/directives/_bundle/menu/menu.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/angular_css")
                .Include("~/Assets/js/angular/directives/_bundle/preloading/preloading.css")
                .Include("~/Assets/js/angular/directives/_bundle/alert/alert.css")
                .Include("~/Assets/js/angular/directives/_bundle/writemaths/writemaths.css")
                .Include("~/Assets/css/vendor/angular-motion/angular-motion.css")
                .Include("~/Assets/css/vendor/angular-motion/modules/fade-and-slide.css")
                .Include("~/Assets/css/vendor/angular-motion/modules/fade.css")
                .Include("~/Assets/css/vendor/angular-motion/modules/flip.css")
                .Include("~/Assets/css/vendor/angular-motion/modules/popover-fade.css")
            );

            #endregion

            #region Select2

            bundles.Add(
                new ScriptBundle("~/bundles/select2_js")
                //modules
                .Include("~/Assets/js/vendor/select2/select2.js")
                .Include("~/Assets/js/vendor/select2/select2.full.js")
                .Include("~/Assets/js/vendor/select2/select2.full.min.js")
                .Include("~/Assets/js/vendor/select2/select2.min.js")
            );

            bundles.Add(
               new StyleBundle("~/bundles/select2_css")
               //modules
               .Include("~/Assets/css/vendor/select2/select2.css")
               .Include("~/Assets/css/vendor/select2/select2.min.css")
           );

            #endregion

            #region Home

            bundles.Add(
                new ScriptBundle("~/bundles/Home_js")
                .Include("~/Assets/js/angular/controllers/home/homeController.js")
                .Include("~/Assets/js/angular/models/pageConfiguration/pageConfigurationModel.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/Home_css")
            );

            #endregion

            #region Account

            bundles.Add(
                new StyleBundle("~/bundles/Account_css")
                .Include("~/Assets/css/style.css")
            );

            #endregion

            #region Error

            bundles.Add(
                new StyleBundle("~/bundles/Error_css")
                .Include("~/Assets/css/style.css")
            );

            #endregion

            #region AbsenceReason

            #region (List) Listagem

            bundles.Add(
               new ScriptBundle("~/bundles/AbsenceReason_List_js")
               .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
               .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
               .Include("~/Assets/js/angular/models/absenceReason/absenceReasonModel.js")
               .Include("~/Assets/js/angular/controllers/absenceReason/listAbsenceReasonController.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/AbsenceReason_List_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region (Form) Cadastro/Edição

            bundles.Add(
               new ScriptBundle("~/bundles/AbsenceReason_Form_js")
               .Include("~/Assets/js/angular/services/_bundle/util/util.js")
               .Include("~/Assets/js/angular/models/absenceReason/absenceReasonModel.js")
               .Include("~/Assets/js/angular/controllers/absenceReason/formAbsenceReasonController.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/AbsenceReason_Form_css")
            );
            #endregion

            #endregion

            #region ItemLevel

            #region (List)
            bundles.Add(
                new ScriptBundle("~/bundles/ItemLevel_List_js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/models/itemLevel/itemLevelModel.js")
                .Include("~/Assets/js/angular/controllers/itemLevel/listItemLevelController.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/ItemLevel_List_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region (Form)
            bundles.Add(
                new ScriptBundle("~/bundles/ItemLevel_Form_js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/models/itemLevel/itemLevelModel.js")
                .Include("~/Assets/js/angular/controllers/itemLevel/formItemLevelController.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/ItemLevel_Form_css")
            );
            #endregion

            #endregion

            #region CorrelatedSkill

            #region (Form/List)
            bundles.Add(
            new ScriptBundle("~/bundles/CorrelatedSkill_Form_js")
            .Include("~/Assets/js/angular/services/_bundle/util/util.js")
            .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
            .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
            .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
            .Include("~/Assets/js/angular/models/correlatedSkill/correlatedSkillModel.js")
            .Include("~/Assets/js/angular/models/evaluationMatrix/evaluationMatrixModel.js")
            .Include("~/Assets/js/angular/models/skill/skillModel.js")
            .Include("~/Assets/js/angular/controllers/correlatedSkill/formCorrelatedSkillController.js")
        );

            bundles.Add(
                new StyleBundle("~/bundles/CorrelatedSkill_Form_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #endregion

            #region Discipline

            #region (List)
            bundles.Add(
                new ScriptBundle("~/bundles/Discipline_List_js")
                    .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                    .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                    .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                    .Include("~/Assets/js/angular/models/discipline/disciplineModel.js")
                    .Include("~/Assets/js/angular/models/Integration/levelEducation/levelEducationModel.js")
                    .Include("~/Assets/js/angular/controllers/discipline/listDisciplineController.js")
                );

            bundles.Add(
                new StyleBundle("~/bundles/Discipline_List_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region (Form)
            bundles.Add(
                new ScriptBundle("~/bundles/Discipline_Form_js")
                    .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                    .Include("~/Assets/js/angular/models/discipline/disciplineModel.js")
                    .Include("~/Assets/js/angular/models/Integration/levelEducation/levelEducationModel.js")
                    .Include("~/Assets/js/angular/controllers/discipline/formDisciplineController.js")
                );

            bundles.Add(
                new StyleBundle("~/bundles/Discipline_Form_css")
            );
            #endregion

            #endregion

            #region TestType

            #region (List)
            bundles.Add(
                new ScriptBundle("~/bundles/TestType_List_js")

                .Include("~/Assets/js/angular/controllers/testType/listTestTypeController.js")
                .Include("~/Assets/js/angular/models/testType/testTypeModel.js")
                .Include("~/Assets/js/angular/models/formatType/formatTypeModel.js")
                .Include("~/Assets/js/angular/models/answerSheet/answerSheetModel.js")
                .Include("~/Assets/js/angular/models/itemLevel/itemLevelModel.js")
                .Include("~/Assets/js/angular/models/testTypeItemLevel/testTypeItemLevelModel.js")
                .Include("~/Assets/js/angular/models/Integration/course/courseModel.js")
                .Include("~/Assets/js/angular/models/Integration/curriculumGrade/curriculumGradeModel.js")
                .Include("~/Assets/js/angular/models/Integration/levelEducation/levelEducationModel.js")
                .Include("~/Assets/js/angular/models/Integration/modality/modalityModel.js")
                .Include("~/Assets/js/angular/models/testTypeCourse/testTypeCourseModel.js")
                .Include("~/Assets/js/angular/models/testTypeCourseCurriculumGrade/testTypeCourseCurriculumGradeModel.js")
                .Include("~/Assets/js/angular/models/modelTestModel/modelTestModel.js")
                .Include("~/Assets/js/angular/models/itemType/itemTypeModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/TestType_List_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region (Form)
            bundles.Add(
                new ScriptBundle("~/bundles/TestType_Form_js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/controllers/testType/formTestTypeController.js")
                .Include("~/Assets/js/angular/models/testType/testTypeModel.js")
                .Include("~/Assets/js/angular/models/formatType/formatTypeModel.js")
                .Include("~/Assets/js/angular/models/answerSheet/answerSheetModel.js")
                .Include("~/Assets/js/angular/models/itemLevel/itemLevelModel.js")
                .Include("~/Assets/js/angular/models/testTypeItemLevel/testTypeItemLevelModel.js")
                .Include("~/Assets/js/angular/models/Integration/course/courseModel.js")
                .Include("~/Assets/js/angular/models/Integration/curriculumGrade/curriculumGradeModel.js")
                .Include("~/Assets/js/angular/models/Integration/levelEducation/levelEducationModel.js")
                .Include("~/Assets/js/angular/models/Integration/modality/modalityModel.js")
                .Include("~/Assets/js/angular/models/testTypeCourse/testTypeCourseModel.js")
                .Include("~/Assets/js/angular/models/testTypeCourseCurriculumGrade/testTypeCourseCurriculumGradeModel.js")
                .Include("~/Assets/js/angular/models/modelTestModel/modelTestModel.js")
                .Include("~/Assets/js/angular/models/itemType/itemTypeModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                .Include("~/Assets/js/angular/directives/_bundle/radio-select/radio-select.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/TestType_Form_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")

            );
            #endregion

            #endregion

            #region Test

            #region Test_Index

            bundles.Add(
                new ScriptBundle("~/bundles/Test_Index_js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/controllers/test/testListController.js")
                .Include("~/Assets/js/angular/models/test/testListModel.js")
                .Include("~/Assets/js/angular/models/file/fileModel.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/models/testType/testTypeModel.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/Test_Index_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")

            );

            #endregion

            #region Test_IndexForm

            bundles.Add(
            new ScriptBundle("~/bundles/Test_IndexForm_js")
                .Include("~/Assets/js/angular/controllers/test/testController.js")
                .Include("~/Assets/js/angular/models/test/testModel.js")
                .Include("~/Assets/js/angular/models/ItemType/ItemTypeModel.js")
                .Include("~/Assets/js/angular/models/Integration/modality/modalityModel.js")
                .Include("~/Assets/js/angular/models/testType/testTypeModel.js")
                .Include("~/Assets/js/angular/models/evaluationMatrix/evaluationMatrixModel.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")

                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/vendor/compressor/compressor.js")
                .Include("~/Assets/js/angular/directives/_bundle/uploader/uploader.js")
                .Include("~/Assets/js/angular/directives/_bundle/uploader/upload.js")

                .Include("~/Assets/js/vendor/redactor/redactor.js")
                .Include("~/Assets/js/vendor/redactor/fontfamily.js")
                .Include("~/Assets/js/vendor/redactor/fontcolor.js")
                .Include("~/Assets/js/vendor/redactor/fontsize.js")
                .Include("~/Assets/js/vendor/redactor/table.js")
                .Include("~/Assets/js/vendor/redactor/clips.js")
                .Include("~/Assets/js/angular/directives/_bundle/redactor-directive/redactor-directive.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/directives/_bundle/tags-input/ng-tags-input.js")
                .Include("~/Assets/js/angular/models/testGroup/testGroupModel.js")
                .Include("~/Assets/js/angular/models/NumberItemsAplicationTai/numberItemsAplicationTaiModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/collapse/collapse.js")

            );

            bundles.Add(
                new StyleBundle("~/bundles/Test_IndexForm_css")
                .Include("~/Assets/js/angular/directives/_bundle/tags-input/ng-tags-input.css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/vendor/redactor/redactor.css")
                .Include("~/Assets/js/vendor/redactor/mathLatex.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")

            );

            #endregion

            #region Test_IndexAdministrate

            bundles.Add(
                new ScriptBundle("~/bundles/Test_IndexAdministrate_js")

                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/models/test/testAdministrateModel.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/controllers/test/testAdministrateController.js")
                .Include("~/Assets/js/angular/models/answerSheet/answerSheetModel.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/Test_IndexAdministrate_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );

            #endregion

            #region Test_IndexStudentResponses

            bundles.Add(
                new ScriptBundle("~/bundles/Test_IndexStudentResponses_js")

                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/models/test/testAdministrateModel.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/controllers/test/testResponsesController.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/Test_IndexStudentResponses_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );

            #endregion

            #region Test_IndexImport

            bundles.Add(
                new ScriptBundle("~/bundles/Test_IndexImport_js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/directives/_bundle/ng-change-file/ng-change-file.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/models/test/testImportExportModel.js")
                .Include("~/Assets/js/angular/controllers/test/testImportController.js")
                .Include("~/Assets/js/angular/models/file/fileModel.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/Test_IndexImport_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );

            #endregion

            #region Test_IndexReport

            bundles.Add(
                new ScriptBundle("~/bundles/Test_IndexReport_js")
                .Include("~/Assets/js/angular/controllers/test/testReportController.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/Test_IndexReport_css")
            );


            #endregion

            #region  Test_IndexRevoke
            bundles.Add(
                new ScriptBundle("~/bundles/Test_IndexRevoke_js")
                .Include("~/Assets/js/angular/controllers/test/testRevokeController.js")
                .Include("~/Assets/js/angular/models/test/testRevokeModel.js")
                // pager
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")

            );

            bundles.Add(
                new StyleBundle("~/bundles/Test_IndexRevoke_css")
            );
            #endregion

            #region  Test_IndexRequestRevoke
            bundles.Add(
                new ScriptBundle("~/bundles/Test_IndexRequestRevoke_js")
                .Include("~/Assets/js/angular/controllers/test/testRequestRevokeController.js")
                .Include("~/Assets/js/angular/models/test/testRequestRevokeModel.js")
                .Include("~/Assets/js/angular/models/item/itemModel.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")

                // pager
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/Test_IndexRequestRevoke_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
             );

            #endregion

            #region Test_IndexPermission

            bundles.Add(
                new ScriptBundle("~/bundles/Test_IndexPermission_js")

                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/models/test/testPermissionModel.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/controllers/test/testPermissionController.js")
                .Include("~/Assets/js/angular/models/answerSheet/answerSheetModel.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/Test_IndexPermission_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );

            #endregion

            #endregion

            #region ItemType_Index

            bundles.Add(
                new ScriptBundle("~/bundles/ItemType_Index_js")
                .Include("~/Assets/js/angular/controllers/itemType/itemTypeListController.js")
                .Include("~/Assets/js/angular/models/itemType/ItemTypeModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/ItemType_Index_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")

            );
            #endregion

            #region ItemType_IndexForm

            bundles.Add(
                new ScriptBundle("~/bundles/ItemType_IndexForm_js")
                .Include("~/Assets/js/angular/controllers/itemType/itemTypeFormController.js")
                .Include("~/Assets/js/angular/models/itemType/ItemTypeModel.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/ItemType_IndexForm_css")
            );
            #endregion

            #region File

            #region File_Index
            bundles.Add(
                new ScriptBundle("~/bundles/File_Index_js")
                .Include("~/Assets/js/angular/directives/_bundle/ng-change-file/ng-change-file.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/directives/_bundle/uploader/uploader.js")
                .Include("~/Assets/js/angular/directives/_bundle/uploader/upload.js")
                .Include("~/Assets/js/angular/directives/_bundle/uploader/video-upload.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/controllers/file/fileController.js")
                .Include("~/Assets/js/angular/models/file/fileModel.js")
                .Include("~/Assets/js/angular/models/test/testListModel.js")
                .Include("~/Assets/js/vendor/compressor/compressor.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/File_Index_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #endregion

            #region Item

            #region (List)
            bundles.Add(
                new ScriptBundle("~/bundles/Item_List_js")
                .Include("~/Assets/js/angular/controllers/item/listItemController.js")
                .Include("~/Assets/js/angular/models/item/itemModel.js")
                .Include("~/Assets/js/angular/models/discipline/disciplineModel.js")
                .Include("~/Assets/js/angular/models/evaluationMatrix/evaluationMatrixModel.js")
                .Include("~/Assets/js/angular/models/itemSituation/itemSituationModel.js")
                .Include("~/Assets/js/angular/models/skill/skillModel.js")
                .Include("~/Assets/js/angular/models/EvaluationMatrixCourseCurriculumGrade/EvaluationMatrixCourseCurriculumGradeModel.js")
                .Include("~/Assets/js/angular/models/parameter/parameterModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/redactor-directive/redactor-directive.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/directives/_bundle/radio-select/radio-select.js")
                .Include("~/Assets/js/angular/directives/_bundle/rating/rating-directive.js")
                .Include("~/Assets/js/angular/directives/_bundle/tags-input/ng-tags-input.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/Item_List_css")
                .Include("~/Assets/js/angular/directives/_bundle/rating/rating.css")
                .Include("~/Assets/js/angular/directives/_bundle/tags-input/ng-tags-input.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region (Form)
            bundles.Add(
            new ScriptBundle("~/bundles/Item_Form_js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/controllers/item/formItemController.js")
                .Include("~/Assets/js/angular/models/item/itemModel.js")
                .Include("~/Assets/js/angular/models/discipline/disciplineModel.js")
                .Include("~/Assets/js/angular/models/subject/subjectModel.js")
                .Include("~/Assets/js/angular/models/evaluationMatrix/evaluationMatrixModel.js")
                .Include("~/Assets/js/angular/models/itemLevel/itemLevelModel.js")
                .Include("~/Assets/js/angular/models/itemSituation/itemSituationModel.js")
                .Include("~/Assets/js/angular/models/itemType/itemTypeModel.js")
                .Include("~/Assets/js/angular/models/skill/skillModel.js")
                .Include("~/Assets/js/angular/models/EvaluationMatrixCourseCurriculumGrade/EvaluationMatrixCourseCurriculumGradeModel.js")
                .Include("~/Assets/js/angular/models/parameter/parameterModel.js")
                .Include("~/Assets/js/vendor/compressor/compressor.js")
                .Include("~/Assets/js/vendor/redactor/redactor.js")
                .Include("~/Assets/js/vendor/redactor/fontfamily.js")
                .Include("~/Assets/js/vendor/redactor/fontcolor.js")
                .Include("~/Assets/js/vendor/redactor/fontsize.js")
                .Include("~/Assets/js/vendor/redactor/table.js")
                .Include("~/Assets/js/vendor/redactor/clips.js")
                .Include("~/Assets/js/vendor/redactor/imagemanager.js")
                .Include("~/Assets/js/vendor/redactor/mathLatex.js")
                .Include("~/Assets/js/angular/directives/_bundle/redactor-directive/redactor-directive.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                .Include("~/Assets/js/angular/directives/_bundle/radio-select/radio-select.js")
                .Include("~/Assets/js/angular/directives/_bundle/rating/rating-directive.js")
                .Include("~/Assets/js/angular/directives/_bundle/tags-input/ng-tags-input.js")
                .Include("~/Assets/js/angular/directives/_bundle/uploader/uploader.js")
                .Include("~/Assets/js/angular/directives/_bundle/uploader/upload.js")
                .Include("~/Assets/js/angular/directives/_bundle/uploader/video-upload.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/Item_Form_css")
                .Include("~/Assets/js/vendor/redactor/redactor.css")
                .Include("~/Assets/js/vendor/redactor/mathLatex.css")
                .Include("~/Assets/js/angular/directives/_bundle/rating/rating.css")
                .Include("~/Assets/js/angular/directives/_bundle/tags-input/ng-tags-input.css")

            );
            #endregion

            #endregion

            #region EvaluationMatrix

            #region EvaluationMatrix_Index
            bundles.Add(
               new ScriptBundle("~/bundles/EvaluationMatrix_Index_js")
                    .Include("~/Assets/js/angular/controllers/evaluationMatrix/evaluationMatrixListController.js")
                    .Include("~/Assets/js/angular/models/evaluationMatrix/evaluationMatrixModel.js")
                    .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                    .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                    .Include("~/Assets/js/angular/directives/_bundle/modal-alert/modal-alert.js")
                );
            bundles.Add(
                new StyleBundle("~/bundles/EvaluationMatrix_Index_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/modelEvaluationMatrix.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region EvaluationMatrix_IndexForm
            bundles.Add(
                new ScriptBundle("~/bundles/EvaluationMatrix_IndexForm_js")
                    .Include("~/Assets/js/angular/models/modelEvaluationMatrix/ModelEvaluationMatrixModel.js")
                    .Include("~/Assets/js/angular/models/modelSkillLevel/ModelSkillLevelModel.js")
                    .Include("~/Assets/js/angular/models/skill/skillModel.js")
                    .Include("~/Assets/js/angular/models/discipline/disciplineModel.js")
                    .Include("~/Assets/js/angular/models/Integration/curriculumGrade/curriculumGradeModel.js")
                    .Include("~/Assets/js/angular/models/Integration/course/courseModel.js")
                    .Include("~/Assets/js/angular/models/Integration/levelEducation/levelEducationModel.js")
                    .Include("~/Assets/js/angular/models/Integration/modality/modalityModel.js")
                    .Include("~/Assets/js/angular/models/evaluationMatrixCourse/evaluationMatrixCourseModel.js")
                    .Include("~/Assets/js/angular/models/EvaluationMatrixCourseCurriculumGrade/EvaluationMatrixCourseCurriculumGradeModel.js")
                    .Include("~/Assets/js/angular/models/cognitiveCompetence/cognitiveCompetenceModel.js")
                    .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                    .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                    .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                    .Include("~/Assets/js/angular/directives/_bundle/modal-alert/modal-alert.js")
                    .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                    .Include("~/Assets/js/angular/directives/_bundle/radio-select/radio-select.js")
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                    .Include("~/Assets/js/angular/controllers/evaluationMatrix/evaluationMatrixController.js")
                    .Include("~/Assets/js/angular/models/evaluationMatrix/evaluationMatrixModel.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/EvaluationMatrix_IndexForm_css")
                    .Include("~/Assets/js/angular/directives/_bundle/page/modelEvaluationMatrix.css")
                    .Include("~/Assets/js/angular/directives/_bundle/page/page.css")

            );
            #endregion

            #endregion

            #region ModelEvaluationMatrix

            #region ModelEvaluationMatrix_Index
            bundles.Add(
               new ScriptBundle("~/bundles/ModelEvaluationMatrix_Index_js")
                .Include("~/Assets/js/angular/controllers/modelEvaluationMatrix/modelEvaluationMatrixListController.js")
                .Include("~/Assets/js/angular/models/modelEvaluationMatrix/ModelEvaluationMatrixModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/ModelEvaluationMatrix_Index_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region ModelEvaluationMatrix_IndexForm

            bundles.Add(
                new ScriptBundle("~/bundles/ModelEvaluationMatrix_IndexForm_js")
                    //Controller e model js
                    .Include("~/Assets/js/angular/controllers/modelEvaluationMatrix/modelEvaluationMatrixController.js")
                    .Include("~/Assets/js/angular/models/modelEvaluationMatrix/ModelEvaluationMatrixModel.js")
                    // Radio
                    .Include("~/Assets/js/angular/directives/_bundle/radio-select/radio-select.js")
                    //directiva util
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/ModelEvaluationMatrix_IndexForm_css")
            );
            #endregion

            #endregion

            #region ReportItem

            bundles.Add(
                new ScriptBundle("~/bundles/ReportItem_js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/models/skill/skillModel.js")
                .Include("~/Assets/js/angular/models/reportItem/reportItemModel.js")
                .Include("~/Assets/js/angular/models/discipline/disciplineModel.js")
                .Include("~/Assets/js/angular/models/evaluationMatrix/evaluationMatrixModel.js")
                .Include("~/Assets/js/angular/models/itemSituation/itemSituationModel.js")
                .Include("~/Assets/js/vendor/FileSaver.js")
                .Include("~/Assets/js/angular/controllers/reportItem/reportItemController.js")
                .Include("~/Assets/js/angular/models/Integration/levelEducation/levelEducationModel.js")
                .Include("~/Assets/js/vendor/{version}/charts-2.0.js".Replace("{version}", chartsVersion))
                );

            bundles.Add(
                new StyleBundle("~/bundles/ReportItem_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")

            );
            #endregion

            #region ReportTest

            #region PerformanceItem
            bundles.Add(
                new ScriptBundle("~/bundles/ReportTest_PerformanceItem_js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/directives/_bundle/reportFilters/reportFilters.js")
                .Include("~/Assets/js/angular/models/reportTest/reportTestModel.js")
                .Include("~/Assets/js/angular/controllers/reportTest/performanceItemController.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/ReportTest_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region PerformanceSchool
            bundles.Add(
                new ScriptBundle("~/bundles/ReportTest_PerformanceSchool_js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/directives/_bundle/reportFilters/reportFilters.js")
                .Include("~/Assets/js/angular/controllers/reportTest/performanceSchoolController.js")
                .Include("~/Assets/js/angular/models/reportTest/reportTestModel.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/ReportTest_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region GraphicPerformanceSchool
            bundles.Add(
                new ScriptBundle("~/bundles/ReportTest_GraphicPerformanceSchool_js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/directives/_bundle/reportFilters/reportFilters.js")
                .Include("~/Assets/js/angular/controllers/reportTest/graphicPerformanceSchoolController.js")
                .Include("~/Assets/js/angular/models/reportTest/reportTestModel.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/ReportTest_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region PerformanceSkill
            bundles.Add(
                new ScriptBundle("~/bundles/ReportTest_PerformanceSkill_js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/directives/_bundle/reportFilters/reportFilters.js")
                .Include("~/Assets/js/angular/controllers/reportTest/performanceSkillController.js")
                .Include("~/Assets/js/angular/models/reportTest/reportTestModel.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/File_Index_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #endregion

            #region ReportCorretion

            #region ReportDRE
            bundles.Add(
                new ScriptBundle("~/bundles/ReportCorrection_Index_js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/models/reportCorrection/reportCorrectionModel.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/models/test/testModel.js")
                .Include("~/Assets/js/angular/controllers/reportCorrection/reportDREController.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/ReportCorrection_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")

            );

            #endregion

            #region ReportSchool
            bundles.Add(
                new ScriptBundle("~/bundles/ReportCorrection_IndexSchool_js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/models/reportCorrection/reportCorrectionModel.js")
                .Include("~/Assets/js/angular/models/test/testModel.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/controllers/reportCorrection/reportSchoolController.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/ReportCorrection_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );

            #endregion

            #region ReportSection
            bundles.Add(
                new ScriptBundle("~/bundles/ReportCorrection_IndexSection_js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/models/reportCorrection/reportCorrectionModel.js")
                .Include("~/Assets/js/angular/models/test/testModel.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/controllers/reportCorrection/reportSectionController.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/ReportCorrection_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );

            #endregion

            #region ReportStudent
            bundles.Add(
                new ScriptBundle("~/bundles/ReportCorrection_IndexStudent_js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/models/reportCorrection/reportCorrectionModel.js")
                .Include("~/Assets/js/angular/models/test/testModel.js")
                .Include("~/Assets/js/angular/controllers/reportCorrection/reportStudentController.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/ReportCorrection_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );

            #endregion

            #endregion

            #region ReportAnswerSheet

            #region FollowUpIdentification
            bundles.Add(
                new StyleBundle("~/bundles/ReportAnswerSheet_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region ReportAnswerSheetDRE
            bundles.Add(
                new ScriptBundle("~/bundles/ReportAnswerSheet_FollowUpIdentificationDRE_js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/models/reportAnswerSheet/reportAnswerSheetModel.js")
                .Include("~/Assets/js/angular/controllers/reportAnswerSheet/reportAnswerSheetDREController.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/ReportAnswerSheet_FollowUpIdentificationDRE_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );

            #endregion

            #region ReportAnswerSheetSchool
            bundles.Add(
                new ScriptBundle("~/bundles/ReportAnswerSheet_FollowUpIdentificationSchool_js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/models/reportAnswerSheet/reportAnswerSheetModel.js")
                .Include("~/Assets/js/angular/controllers/reportAnswerSheet/reportAnswerSheetSchoolController.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/ReportAnswerSheet_FollowUpIdentificationSchool_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );

            #endregion

            #region ReportAnswerSheetFiles
            bundles.Add(
                new ScriptBundle("~/bundles/ReportAnswerSheet_FollowUpIdentificationFiles_js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/models/reportAnswerSheet/reportAnswerSheetModel.js")
                .Include("~/Assets/js/angular/models/file/fileModel.js")
                .Include("~/Assets/js/angular/models/test/testModel.js")
                .Include("~/Assets/js/angular/controllers/reportAnswerSheet/reportAnswerSheetFilesController.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/ReportAnswerSheet_FollowUpIdentificationFiles_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );

            #endregion

            #endregion

            #region PerformanceLevel

            #region (List)
            bundles.Add(
                    new ScriptBundle("~/bundles/PerformanceLevel_List_js")
                    .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                    .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                    .Include("~/Assets/js/angular/models/performanceLevel/performanceLevelModel.js")
                    .Include("~/Assets/js/angular/controllers/performanceLevel/listPerformanceLevelController.js")
                );

            bundles.Add(
                new StyleBundle("~/bundles/PerformanceLevel_List_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region (Form)
            bundles.Add(
                new ScriptBundle("~/bundles/PerformanceLevel_Form_js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/models/performanceLevel/performanceLevelModel.js")
                .Include("~/Assets/js/angular/controllers/performanceLevel/formPerformanceLevelController.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/PerformanceLevel_Form_css")
            );
            #endregion

            #endregion

            #region Parameter

            bundles.Add(
                new ScriptBundle("~/bundles/Parameter_js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/directives/_bundle/collapse/collapse.js")
                .Include("~/Assets/js/angular/models/parameter/parameterModel.js")
                .Include("~/Assets/js/angular/controllers/parameter/parameterController.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/Parameter_css")
                .Include("~/Assets/css/vendor/angular-motion/modules/collapse.css")
                .Include("~/Assets/css/vendor/angular-motion/modules/flip.css")
            );
            #endregion

            #region CognitiveCompetence

            #region (List) Listagem
            bundles.Add(
                new ScriptBundle("~/bundles/CognitiveCompetence_List_js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/models/cognitiveCompetence/cognitiveCompetenceModel.js")
                .Include("~/Assets/js/angular/controllers/cognitiveCompetence/listCognitiveCompetenceController.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/CognitiveCompetence_List_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region (Form) Cadastro/Edição
            bundles.Add(
                new ScriptBundle("~/bundles/CognitiveCompetence_Form_js")
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                    .Include("~/Assets/js/angular/models/cognitiveCompetence/cognitiveCompetenceModel.js")
                    .Include("~/Assets/js/angular/controllers/cognitiveCompetence/formCognitiveCompetenceController.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/CognitiveCompetence_Form_css")
            );
            #endregion

            #endregion CognitiveCompetence

            #region ModelTest_IndexForm
            bundles.Add(
                new ScriptBundle("~/bundles/ModelTest_IndexForm_js")
                //upload simples
                .Include("~/Assets/js/angular/directives/_bundle/uploader/uploader.js")
                .Include("~/Assets/js/angular/directives/_bundle/uploader/upload.js")
                //compressor de imagem
                .Include("~/Assets/js/vendor/compressor/compressor.js")
                // Redactor
                .Include("~/Assets/js/vendor/redactor/redactor.js")
                .Include("~/Assets/js/vendor/redactor/fontfamily.js")
                .Include("~/Assets/js/vendor/redactor/fontcolor.js")
                .Include("~/Assets/js/vendor/redactor/fontsize.js")
                .Include("~/Assets/js/vendor/redactor/table.js")
                .Include("~/Assets/js/vendor/redactor/clips.js")
                .Include("~/Assets/js/vendor/redactor/imagemanager.js")
                .Include("~/Assets/js/angular/directives/_bundle/redactor-directive/redactor-directive.js")
                //directiva modal
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                //directiva util
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                //models
                .Include("~/Assets/js/angular/models/modelTestModel/modelTestModel.js")
                //controller
                .Include("~/Assets/js/angular/controllers/modelTest/modelTestController.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/ModelTest_IndexForm_css")
                //Redactor
                .Include("~/Assets/js/vendor/redactor/redactor.css")
                .Include("~/Assets/js/vendor/redactor/clips.css")
            );
            #endregion ModelTest_IndexForm

            #region ModelTest_Index

            bundles.Add(
                new ScriptBundle("~/bundles/ModelTest_Index_js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/models/modelTestModel/modelTestModel.js")
                .Include("~/Assets/js/angular/controllers/modelTest/modelTestListController.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
            );

            bundles.Add(
                new ScriptBundle("~/bundles/ModelTest_Index_css")
               .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
           );
            #endregion ModelTest_Index

            #region Adherence
            bundles.Add(
                new ScriptBundle("~/bundles/Adherence_js")
                    .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                    .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                    .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                    .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                    .Include("~/Assets/js/angular/models/adherence/adherenceApiModel.js")
                    .Include("~/Assets/js/angular/controllers/adherence/adherenceController.js")
                );

            bundles.Add(
                new StyleBundle("~/bundles/Adherence_css")
                    .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
                );
            #endregion

            #region Correction

            #region Correction_Index
            bundles.Add(
                new ScriptBundle("~/bundles/Correction_Index_js")
                    .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                    .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                    .Include("~/Assets/js/angular/models/correction/correctionModel.js")
                    .Include("~/Assets/js/angular/models/discipline/disciplineModel.js")
                    .Include("~/Assets/js/angular/models/test/testModel.js")
                    .Include("~/Assets/js/angular/models/item/itemModel.js")
                    .Include("~/Assets/js/angular/models/test/testAdministrateModel.js")
                    .Include("~/Assets/js/vendor/{version}/charts-2.0.js".Replace("{version}", chartsVersion))
                    .Include("~/Assets/js/vendor/{version}/horizontalBarLineDrawer.js".Replace("{version}", chartsVersion))
                    .Include("~/Assets/js/vendor/FileSaver.js")
                    .Include("~/Assets/js/angular/controllers/correction/correctionResultController.js")
                    .Include("~/Assets/js/angular/directives/_bundle/reportFilters/reportFilters.js")
                );

            bundles.Add(
                new StyleBundle("~/bundles/Correction_Index_css")
                );
            #endregion

            #region Correction_IndexForm
            bundles.Add(
                new ScriptBundle("~/bundles/Correction_IndexForm_js")
                    .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                    .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                    .Include("~/Assets/js/angular/models/correction/correctionModel.js")
                    .Include("~/Assets/js/angular/models/correction/correctionApiModel.js")
                    .Include("~/Assets/js/angular/models/absenceReason/absenceReasonModel.js")
                    .Include("~/Assets/js/angular/models/answerSheet/answerSheetModel.js")
                    .Include("~/Assets/js/angular/controllers/correction/correctionController.js")
                );

            bundles.Add(
                new StyleBundle("~/bundles/Correction_IndexForm_css")
                );
            #endregion

            #endregion

            #region AnswerSheet

            #region AnswerSheet_IndexBatchDetails
            bundles.Add(
            new ScriptBundle("~/bundles/AnswerSheet_IndexBatchDetails_js")
                .Include("~/Assets/js/vendor/compressor/compressor.js")
                .Include("~/Assets/js/angular/directives/_bundle/uploader/uploader.js")
                .Include("~/Assets/js/angular/directives/_bundle/uploader/upload.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/directives/_bundle/notification-filter/notification.filter.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/models/test/testListModel.js")

                //datepicker
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/models/answerSheet/answerSheetModel.js")
                .Include("~/Assets/js/angular/models/parameter/parameterModel.js")
                .Include("~/Assets/js/angular/models/file/fileModel.js")
                .Include("~/Assets/js/angular/controllers/answerSheet/batchDetailsController.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/AnswerSheet_IndexBatchDetails_css")

                //datepicker
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region AnswerSheet_IndexBatchDetailsLot
            bundles.Add(
            new ScriptBundle("~/bundles/AnswerSheet_IndexBatchDetailsLot_js")
                .Include("~/Assets/js/vendor/compressor/compressor.js")
                .Include("~/Assets/js/angular/directives/_bundle/uploader/uploader.js")
                .Include("~/Assets/js/angular/directives/_bundle/uploader/upload.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/directives/_bundle/notification-filter/notification.filter.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/models/test/testListModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/collapse/collapse.js")

                //datepicker
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/models/answerSheet/answerSheetModel.js")
                .Include("~/Assets/js/angular/models/parameter/parameterModel.js")
                .Include("~/Assets/js/angular/models/file/fileModel.js")
                .Include("~/Assets/js/angular/controllers/answerSheet/batchDetailsLotController.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/AnswerSheet_IndexBatchDetailsLot_css")

                //datepicker
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region AnswerSheet_AnswerSheetLot
            bundles.Add(
                new ScriptBundle("~/bundles/AnswerSheet_AnswerSheetLot_js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/models/answerSheet/answerSheetModel.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/models/file/fileModel.js")
                .Include("~/Assets/js/angular/controllers/answerSheet/answerSheetLotController.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/AnswerSheet_AnswerSheetLot_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region Index_AnswerSheetStudent
            bundles.Add(
                new ScriptBundle("~/bundles/AnswerSheet_IndexAnswerSheetStudent_js")
                    .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                    .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                    .Include("~/Assets/js/angular/models/correction/correctionModel.js")
                    .Include("~/Assets/js/angular/models/correction/correctionApiModel.js")
                    .Include("~/Assets/js/angular/models/absenceReason/absenceReasonModel.js")
                    .Include("~/Assets/js/angular/models/answerSheet/answerSheetModel.js")
                    .Include("~/Assets/js/angular/controllers/answerSheet/answerSheetStudentController.js")
                );

            bundles.Add(
                new StyleBundle("~/bundles/AnswerSheet_IndexAnswerSheetStudent_css")
                );
            #endregion

            #region ReportStudentPerformance
            bundles.Add(
                new ScriptBundle("~/bundles/ReportStudentPerformance_Index_js")
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                    .Include("~/Assets/js/angular/models/reportStudentPerformance/reportStudentPerformanceModel.js")
                    .Include("~/Assets/js/angular/controllers/reportStudentPerformance/reportStudentPerformanceController.js")
                );

            bundles.Add(
                 new StyleBundle("~/bundles/ReportStudentPerformance_css")
                );
            #endregion

            #region ReportTestPerformanceDRE
            bundles.Add(
                    new ScriptBundle("~/bundles/ReportTestPerformance_IndexDRE_js")
                        .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                        .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                        .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                        .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                        .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                        .Include("~/Assets/js/angular/models/reportTestPerformance/reportTestPerformanceModel.js")
                        .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                        .Include("~/Assets/js/angular/models/test/testModel.js")
                        .Include("~/Assets/js/angular/models/discipline/disciplineModel.js")
                        .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                        .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                        .Include("~/Assets/js/angular/controllers/reportTestPerformance/reportTestPerformanceDREController.js")
                    );

            bundles.Add(
                new StyleBundle("~/bundles/ReportTestPerformance_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
                );
            #endregion

            #region ReportTestPerformanceSchool
            bundles.Add(
                    new ScriptBundle("~/bundles/ReportTestPerformance_IndexSchool_js")
                        .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                        .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                        .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                        .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                        .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                        .Include("~/Assets/js/angular/models/reportTestPerformance/reportTestPerformanceModel.js")
                        .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                        .Include("~/Assets/js/angular/models/test/testModel.js")
                        .Include("~/Assets/js/angular/models/discipline/disciplineModel.js")
                        .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                        .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                        .Include("~/Assets/js/angular/controllers/reportTestPerformance/reportTestPerformanceSchoolController.js")
                    );

            bundles.Add(
                new StyleBundle("~/bundles/ReportTestPerformance_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
                );
            #endregion

            #region ReportItemPerformanceDRE
            bundles.Add(
                new ScriptBundle("~/bundles/ReportItemPerformance_IndexDRE_js")
                    .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                    .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                    .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                    .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                    .Include("~/Assets/js/angular/models/reportItemPerformance/reportItemPerformanceModel.js")
                    .Include("~/Assets/js/angular/models/reportPerformance/reportPerformanceModel.js")
                    .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                    .Include("~/Assets/js/angular/models/evaluationMatrix/evaluationMatrixModel.js")
                    .Include("~/Assets/js/angular/models/skill/skillModel.js")
                    .Include("~/Assets/js/angular/models/test/testModel.js")
                    .Include("~/Assets/js/angular/models/discipline/disciplineModel.js")
                    .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                    .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                    .Include("~/Assets/js/vendor/FileSaver.js")
                    .Include("~/Assets/js/angular/controllers/reportItemPerformance/reportItemPerformanceDREController.js")
                    .Include("~/Assets/js/vendor/{version}/charts-2.0.js".Replace("{version}", chartsVersion))
                    .Include("~/Assets/js/vendor/{version}/horizontalBarLineDrawer.js".Replace("{version}", chartsVersion))
                );

            bundles.Add(
                new StyleBundle("~/bundles/ReportItemPerformance_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
                );
            #endregion

            #region ReportItemPerformanceSchool
            bundles.Add(
                new ScriptBundle("~/bundles/ReportItemPerformance_IndexSchool_js")
                    .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                    .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                    .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                    .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                    .Include("~/Assets/js/angular/models/reportItemPerformance/reportItemPerformanceModel.js")
                    .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                    .Include("~/Assets/js/angular/models/test/testModel.js")
                    .Include("~/Assets/js/angular/models/discipline/disciplineModel.js")
                    .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                    .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                    .Include("~/Assets/js/angular/controllers/reportItemPerformance/reportItemPerformanceSchoolController.js")
                );

            bundles.Add(
                new StyleBundle("~/bundles/ReportItemPerformance_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
                );
            #endregion

            #region ReportItemChoice
            bundles.Add(
                new ScriptBundle("~/bundles/ReportItemChoice_Index_js")
                    .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                    .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                    .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                    .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                    .Include("~/Assets/js/angular/models/reportItemChoice/reportItemChoiceModel.js")
                    .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                    .Include("~/Assets/js/angular/models/test/testModel.js")
                    .Include("~/Assets/js/angular/models/discipline/disciplineModel.js")
                    .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                    .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                    .Include("~/Assets/js/angular/controllers/reportItemChoice/reportItemChoiceController.js")
                );

            bundles.Add(
                new StyleBundle("~/bundles/ReportItemChoice_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
                );
            #endregion

            #endregion

            #region TestGroup

            #region (List)

            bundles.Add(
               new ScriptBundle("~/bundles/TestGroup_List_js")
                .Include("~/Assets/js/angular/controllers/testGroup/listTestGroupController.js")
                .Include("~/Assets/js/angular/models/testGroup/testGroupModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/TestGroup_List_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region (Form)

            bundles.Add(
                new ScriptBundle("~/bundles/TestGroup_Form_js")
                    //Controller e model js
                    .Include("~/Assets/js/angular/controllers/testGroup/formTestGroupController.js")
                    .Include("~/Assets/js/angular/models/testGroup/testGroupModel.js")
                    // Radio
                    .Include("~/Assets/js/angular/directives/_bundle/radio-select/radio-select.js")
                    //directiva util
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/TestGroup_Form_css")
            );
            #endregion

            #endregion

            #region KnowledgeArea

            #region (List)

            bundles.Add(
               new ScriptBundle("~/bundles/KnowledgeArea_List_js")
                .Include("~/Assets/js/angular/controllers/knowledgeArea/listKnowledgeAreaController.js")
                .Include("~/Assets/js/angular/models/knowledgeArea/knowledgeAreaModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/KnowledgeArea_List_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region (Form)

            bundles.Add(
                new ScriptBundle("~/bundles/KnowledgeArea_Form_js")
                    //Controller e model js
                    .Include("~/Assets/js/angular/controllers/knowledgeArea/formKnowledgeAreaController.js")
                    .Include("~/Assets/js/angular/models/knowledgeArea/knowledgeAreaModel.js")
                    .Include("~/Assets/js/angular/models/discipline/disciplineModel.js")
                    .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                    // Radio
                    .Include("~/Assets/js/angular/directives/_bundle/radio-select/radio-select.js")
                    //directiva util
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/KnowledgeArea_Form_css")
            );
            #endregion

            #endregion

            #region Subject

            #region (List)

            bundles.Add(
               new ScriptBundle("~/bundles/Subject_List_js")
                .Include("~/Assets/js/angular/controllers/subject/listSubjectController.js")
                .Include("~/Assets/js/angular/models/subject/subjectModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/directives/_bundle/collapse/collapse.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/Subject_List_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #region (Form)

            bundles.Add(
                new ScriptBundle("~/bundles/Subject_Form_js")
                    //Controller e model js
                    .Include("~/Assets/js/angular/controllers/subject/formSubjectController.js")
                    .Include("~/Assets/js/angular/models/subject/subjectModel.js")
                    // Radio
                    .Include("~/Assets/js/angular/directives/_bundle/radio-select/radio-select.js")
                    //directiva util
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/Subject_Form_css")
            );
            #endregion

            #endregion

            #region ReportPerformance

            #region ReportPerformance_Index
            bundles.Add(
                new ScriptBundle("~/bundles/ReportPerformance_Index_js")
                    .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                    .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                    .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                    .Include("~/Assets/js/angular/models/reportPerformance/reportPerformanceModel.js")
                    .Include("~/Assets/js/vendor/{version}/charts-2.0.js".Replace("{version}", chartsVersion))
                    .Include("~/Assets/js/vendor/FileSaver.js")
                    .Include("~/Assets/js/angular/controllers/reportPerformance/reportPerformanceController.js")
                    .Include("~/Assets/js/angular/directives/_bundle/reportFilters/reportFilters.js")
                    .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                    .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                );

            bundles.Add(
                new StyleBundle("~/bundles/ReportPerformance_Index_css")
                    .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
                    .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                );
            #endregion

            #endregion

            #region ResponseChangeLog

            #region (Index)

            bundles.Add(
               new ScriptBundle("~/bundles/ResponseChangeLog_Index_js")
                .Include("~/Assets/js/angular/controllers/responseChangeLog/responseChangeLogController.js")
                .Include("~/Assets/js/angular/models/responseChangeLog/responseChangeLogModel.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/models/test/testModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/ResponseChangeLog_Index_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );
            #endregion

            #endregion

            #region ElectronicTest

            #region (Index)

            bundles.Add(
               new ScriptBundle("~/bundles/ElectronicTest_Index_js")
                .Include("~/Assets/js/angular/controllers/electronicTest/indexElectronicTestController.js")
                .Include("~/Assets/js/angular/models/electronicTest/electronicTestModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
            //.Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/ElectronicTest_Index_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
            );
            #endregion

            #region (Form)

            bundles.Add(
                new ScriptBundle("~/bundles/ElectronicTest_Form_js")
                    //Controller e model js
                    .Include("~/scripts/jquery.signalR-2.4.1.min.js")
                    .Include("~/scripts/signalRBundle.js")
                    .Include("~/Assets/js/angular/controllers/electronicTest/formElectronicTestController.js")
                    .Include("~/Assets/js/angular/models/electronicTest/electronicTestModel.js")
                    .Include("~/Assets/js/angular/controllers/electronicTest/sessions/SessionManager.js")
                    .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                    //directiva util
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/ElectronicTest_Form_css")
            );
            #endregion

            #region HomeAluno

            bundles.Add(
               new ScriptBundle("~/bundles/ElectronicTest_HomeAluno_js")
                .Include("~/Assets/js/angular/controllers/electronicTest/homeAlunoElectronicTestController.js")
                .Include("~/Assets/js/angular/models/electronicTest/electronicTestModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
            //.Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/ElectronicTest_HomeAluno_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
            );

            #endregion

            #endregion

            #region ElectronicTestResult

            #region (Index)

            bundles.Add(
                new ScriptBundle("~/bundles/ElectronicTestResult_js")
                    //Controller e model js
                    .Include("~/Assets/js/angular/controllers/electronicTestResult/indexElectronicTestResultController.js")
                    .Include("~/Assets/js/angular/models/electronicTestResult/electronicTestResultModel.js")
                    //directiva util
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/ElectronicTestResult_css")
            );
            #endregion

            #endregion

            #region StudentsResult

            #region StudentResults_Index

            bundles.Add(
                new ScriptBundle("~/bundles/StudentResults_js")
                .Include("~/Assets/js/angular/controllers/StudentResults/studentResultsListController.js")
                .Include("~/Assets/js/angular/models/StudentResults/studentResultsListModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/StudentResults_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")

            );

            #endregion

            #endregion

            #region StudentResultGRaphics

            #region StudentResultsGraphics_Index
            bundles.Add(
                new ScriptBundle("~/bundles/StudentResultsGraphics_js")
                    .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                    .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                    .Include("~/Assets/js/angular/models/studentResultsGraphics/studentResultsGraphicsModel.js")
                    .Include("~/Assets/js/angular/models/discipline/disciplineModel.js")
                    .Include("~/Assets/js/angular/models/test/testModel.js")
                    .Include("~/Assets/js/angular/models/item/itemModel.js")
                    .Include("~/Assets/js/angular/models/test/testAdministrateModel.js")
                    .Include("~/Assets/js/vendor/{version}/charts-2.0.js".Replace("{version}", chartsVersion))
                    .Include("~/Assets/js/vendor/{version}/horizontalBarLineDrawer.js".Replace("{version}", chartsVersion))
                    .Include("~/Assets/js/vendor/FileSaver.js")
                    .Include("~/Assets/js/angular/controllers/studentResultsGraphics/studentResultsGraphicsResultController.js")
                    .Include("~/Assets/js/angular/directives/_bundle/reportFilters/reportFilters.js")
                );

            bundles.Add(
                new StyleBundle("~/bundles/StudentResultsGraphics_css")
                );
            #endregion

            #endregion

            #region PageConfiguration

            #region (Index)

            bundles.Add(
               new ScriptBundle("~/bundles/PageConfiguration_List_js")
                .Include("~/Assets/js/angular/controllers/pageConfiguration/pageConfigurationListController.js")
                .Include("~/Assets/js/angular/models/pageConfiguration/pageConfigurationModel.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/PageConfiguration_List_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
            );
            #endregion

            #region (Form)

            bundles.Add(
                new ScriptBundle("~/bundles/PageConfiguration_Form_js")
                    //Controller e model js
                    .Include("~/Assets/js/angular/controllers/pageConfiguration/pageConfigurationFormController.js")
                    .Include("~/Assets/js/angular/models/pageConfiguration/pageConfigurationModel.js")
                    .Include("~/Assets/js/vendor/compressor/compressor.js")
                    .Include("~/Assets/js/vendor/redactor/redactor.js")
                    .Include("~/Assets/js/vendor/redactor/fontfamily.js")
                    .Include("~/Assets/js/vendor/redactor/fontcolor.js")
                    .Include("~/Assets/js/vendor/redactor/fontsize.js")
                    .Include("~/Assets/js/vendor/redactor/table.js")
                    .Include("~/Assets/js/vendor/redactor/clips.js")
                    .Include("~/Assets/js/vendor/redactor/imagemanager.js")
                    .Include("~/Assets/js/vendor/redactor/mathLatex.js")
                    .Include("~/Assets/js/angular/directives/_bundle/redactor-directive/redactor-directive.js")
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                    .Include("~/Assets/js/angular/directives/_bundle/uploader/uploader.js")
                    .Include("~/Assets/js/angular/directives/_bundle/uploader/upload.js")
                    .Include("~/Assets/js/angular/directives/_bundle/uploader/video-upload.js")
                    .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                    .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                    .Include("~/Assets/js/angular/directives/_bundle/radio-select/radio-select.js")
                    .Include("~/Assets/js/angular/directives/_bundle/rating/rating-directive.js")
                    .Include("~/Assets/js/angular/directives/_bundle/tags-input/ng-tags-input.js")
                    //directiva util
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/PageConfiguration_Form_css")
            );
            #endregion

            #endregion

            #region AdministrativeUnitType

            #region (Index)

            bundles.Add(
               new ScriptBundle("~/bundles/AdministrativeUnitType_Index_js")
                .Include("~/Assets/js/angular/controllers/administrativeUnitType/administrativeUnitTypeController.js")
                .Include("~/Assets/js/angular/models/administrativeUnitType/administrativeUnitTypeModel.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/directives/_bundle/listBox/listBox.js")
            //.Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/AdministrativeUnitType_Index_css")
                .Include("~/Assets/js/angular/directives/_bundle/listBox/listBox.css")
            );
            #endregion

            #endregion

            #region ProvaSP

            bundles.Add(
                new ScriptBundle("~/bundles/ProvaSP_js")
                .Include("~/Assets/js/angular/controllers/provaSp/provaSpController.js")
                .Include("~/Assets/js/angular/models/pageConfiguration/pageConfigurationModel.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/Home_css")
            );

            #endregion

            #region StudentTestSession

            #region StudentTestSession_Index

            bundles.Add(
                new ScriptBundle("~/bundles/StudentTestSession_js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/models/test/testAdministrateModel.js")
                .Include("~/Assets/js/angular/models/adherence/adherenceModel.js")
                .Include("~/Assets/js/angular/controllers/studentTestSession/studentTestSessionController.js")
                .Include("~/Assets/js/angular/models/studentTestSession/studentTestSessionModel.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/StudentTestSession_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );

            #endregion

            #region AdminSerapEstudantes

            bundles.Add(
                new ScriptBundle("~/bundles/AdminSerapEstudantes_js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/controllers/adminSerapEstudantes/adminSerapEstudantesController.js")
                .Include("~/Assets/js/angular/controllers/adminSerapEstudantes/adminAcompanhamentoProva.js")

            );

            bundles.Add(
                new StyleBundle("~/bundles/AdminSerapEstudantes_css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );

            #endregion

            #region SimuladorSerapEstudantes

            bundles.Add(
                new ScriptBundle("~/bundles/SimuladorSerapEstudantes_js")
                    .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                    .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                    .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                    .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                    .Include("~/Assets/js/angular/controllers/simuladorSerapEstudantes/simuladorSerapEstudantesController.js")
            );

            bundles.Add(
                new StyleBundle("~/bundles/SimuladorSerapEstudantes_css")
                    .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );

            #endregion

            #endregion

            #region ImportarResultadosPSP

            bundles.Add(
                new ScriptBundle("~/bundles/ImportarResultadosPSP_js")
                .Include("~/Assets/js/angular/directives/_bundle/ng-change-file/ng-change-file.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/directives/_bundle/uploader/uploader.js")
                .Include("~/Assets/js/angular/directives/_bundle/uploader/upload.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")                
                .Include("~/Assets/js/angular/models/file/fileModel.js")
                .Include("~/Assets/js/angular/models/test/testListModel.js")
                .Include("~/Assets/js/vendor/compressor/compressor.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/controllers/importarResultadosPSP/importarResultadosPSPController.js")
                .Include("~/Assets/js/angular/models/importarResultadosPSP/importarResultadosPSPModel.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/ImportarResultadosPSP_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );

            #endregion

            #region RelatorioEstudos

            bundles.Add(
                new ScriptBundle("~/bundles/ReportStudies_js")
                .Include("~/Assets/js/angular/directives/_bundle/ng-change-file/ng-change-file.js")
                .Include("~/Assets/js/angular/directives/_bundle/modal/modal.js")
                .Include("~/Assets/js/angular/services/_bundle/util/util.js")
                .Include("~/Assets/js/angular/directives/_bundle/uploader/uploader.js")
                .Include("~/Assets/js/angular/directives/_bundle/uploader/upload.js")
                .Include("~/Assets/js/angular/directives/_bundle/checkbox-group/ckeckbox-group.js")
                .Include("~/Assets/js/vendor/datepicker/datepicker.js")
                .Include("~/Assets/js/angular/directives/_bundle/datepicker/datepicker-directive.js")
                .Include("~/Assets/js/angular/models/file/fileModel.js")
                .Include("~/Assets/js/angular/models/test/testListModel.js")
                .Include("~/Assets/js/vendor/compressor/compressor.js")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.js")
                .Include("~/Assets/js/angular/services/_bundle/pager/services.js")
                .Include("~/Assets/js/angular/controllers/ReportStudies/reportStudiesController.js")
                .Include("~/Assets/js/angular/models/ReportStudies/reportStudiesModel.js")
            );
            bundles.Add(
                new StyleBundle("~/bundles/ReportStudies_css")
                .Include("~/Assets/js/vendor/datepicker/datepicker.css")
                .Include("~/Assets/js/angular/directives/_bundle/page/page.css")
            );

            #endregion

        }
    }
}