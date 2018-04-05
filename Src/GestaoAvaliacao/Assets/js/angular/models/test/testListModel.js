/* 
* ModelSkillLevel-Model
*/
(function () {
  
	angular.module('services').factory('TestListModel', ['$resource', function ($resource) {

		var model = {

			'loadByUserGroupSearchTest': {
				method: 'GET',
				url: base_url('TestType/LoadByUserGroupSearchTest')
			},
			'searchDisciplinesSaves': {
				method: 'GET',
				url: base_url('Discipline/SearchDisciplinesSaves')
			},
			'searchTests': {
				method: 'GET',
				url: base_url('Test/SearchTests')
			},
			'deleteTest': {
				method: 'POST',
				url: base_url('Test/Delete')
			},
			'searchTestFiles': {
				method: 'GET',
				url: base_url('Test/SearchTestFiles')
			},
			'getTestFiles': {
			method: 'GET',
			url: base_url('Test/GetTestFiles')
			},
			'saveTestFiles': {
			method: 'GET',
			url: base_url('Test/SaveTestFiles')
			},
			'checkFilesExists': {
				method: 'GET',
				url: base_url('Test/CheckFilesExists')
			},
			'saveTestFiles': {
				method: 'POST',
				url: base_url('Test/SaveTestFiles')
			},
			'getCurriculumGradeSimple': {
				method: 'GET',
				url: base_url('Test/GetCurriculumGradeSimple')
			},
			'changeTestVisible': {
			    method: 'GET',
			    url: base_url('Test/ChangeTestVisible')
			},
			'changeOrderTestDown': {
			    method: 'POST',
			    url: base_url('Test/ChangeOrderTestDown')
			},
			'changeOrderTestUp': {
			    method: 'POST',
			    url: base_url('Test/ChangeOrderTestUp')
			},
			'getGroups': {
			    method: 'GET',
			    url: base_url('TestGroup/LoadByPermissionTest')
			},
			'getSubGroup': {
			    method: 'GET',
			    url: base_url('TestGroup/GetTestGroup')
			},
			'changeOrderSubGroup': {
			    method: 'POST',
			    url: base_url('TestGroup/ChangeOrder')
			},
			'changeOrderTest': {
			    method: 'POST',
			    url: base_url('Test/ChangeOrder')
			},
		};
   
		return $resource('', {}, model);

	}]);
})();

