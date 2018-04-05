/**
 * function AdherenceModel Model
 * @namespace Model
 * @author Luís Maron - 05/11/2015
 */
(function () {

	'use strict';

	angular.module('services')
		.factory('AdherenceModel', AdherenceModel);

	AdherenceModel.$inject = ['$resource'];

	function AdherenceModel($resource) {

	    var model = {

		    'getDRESimple': {
	            method: 'GET',
	            url: base_url('Adherence/GetDRESimple')
		    },
		    'getAdheredDreSimple': {
		        method: 'GET',
		        url: base_url('Adherence/GetAdheredDreSimple')
		    },
		    'getSchoolsSimple': {
		        method: 'GET',
		        url: base_url('Adherence/GetSchoolsSimple')
		    },
		    'getAdheredSchoolSimple': {
		        method: 'GET',
		        url: base_url('Adherence/GetAdheredSchoolSimple')
		    },
		    'getShiftSimple': {
		        method: 'GET',
		        url: base_url('Adherence/GetShiftSimple')
		    },
		    'getSchoolsGrid': {
		        method: 'GET',
		        url: base_url('Adherence/GetSchoolsGrid')
		    },
		    'getSelectedSchool': {
		        method: 'GET',
		        url: base_url('Adherence/GetSelectedSchool')
		    },
		    'getSectionGrid': {
		        method: 'GET',
		        url: base_url('Adherence/GetSectionGrid')
		    },
		    'getSelectedSection': {
		        method: 'GET',
		        url: base_url('Adherence/GetSelectedSection')
		    },
		    'getStudentsGrid': {
		        method: 'GET',
		        url: base_url('Adherence/GetStudentsGrid')
		    },
		    'getSelectedStudents': {
		        method: 'GET',
		        url: base_url('Adherence/GetSelectedStudents')
		    },
		    'getCurriculumGradeSimple': {
		        method: 'GET',
		        url: base_url('Adherence/GetCurriculumGradeSimple')
		    },
		    'getTotalSelected': {
		        method: 'GET',
		        url: base_url('Adherence/GetTotalSelected')
		    },
		    'switchAllAdhrered': {
		        method: 'POST',
		        url: base_url('Adherence/SwitchAllAdhrered')
		    }
		};

		return $resource('', {}, model);
	};

})();
