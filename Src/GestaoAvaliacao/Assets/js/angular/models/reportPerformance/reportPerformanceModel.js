/**
 * function Correction Model
 * @namespace Model
 * @author Julio Cesar Silva - 25/11/2015
 */
(function () {

	angular.module('services').factory('ReportPerformanceModel', ['$resource', function ($resource) {

		var model = {

			'getAuthorize': {
				method: 'GET',
				url: base_url('Test/GetAuthorize')
			},
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
			'getCurriculumGradeByTestId': {
				method: 'GET',
				url: base_url('Test/GetCurriculumGradeByTestId')
			},
			'getTestsBySubGroup': {
				method: 'GET',
				url: base_url('Test/GetTestsBySubGroup')
			},
			'getDRESimple': {
				method: 'GET',
				url: base_url('Adherence/GetDRESimple')
			},
			'getSchoolsSimple': {
				method: 'GET',
				url: base_url('Adherence/GetSchoolsSimple')
			},
			'getGroups': {
				method: 'GET',
				url: base_url('TestGroup/LoadByPermissionTest')
			},
			'getSubGroup': {
				method: 'GET',
				url: base_url('TestGroup/GetTestGroup')
			},
			'getDistinctCurricumGradeByTestSubGroup_Id': {
				method: 'GET',
				url: base_url('TestGroup/GetDistinctCurricumGradeByTestSubGroup_Id')
			},
			'getDisciplineByTestId': {
				method: 'GET',
				url: base_url('Discipline/LoadComboByTest')
			},
			'getDisciplinesByTestSubGroup_Id': {
				method: 'GET',
				url: base_url('Discipline/GetDisciplinesByTestSubGroup_Id')
			},
			'GetAveragesByTest': {
				method: 'GET',
				url: base_url('Correction/GetAveragesByTest')
			},
			'GetAveragesByTestSubGroup_Id': {
				method: 'GET',
				url: base_url('Correction/GetAveragesByTestSubGroup_Id')
			}

			
		};
	
		return $resource('', {}, model);

	}]);
})();

