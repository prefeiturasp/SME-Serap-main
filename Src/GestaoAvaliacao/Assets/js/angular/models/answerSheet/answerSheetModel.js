/* 
* AnswerSheet-Model
*/
(function () {
	angular.module('services').factory('AnswerSheetModel', ['$resource', function ($resource) {		
		var model = {
			'load': {
				method: 'GET',
				url: base_url('AnswerSheet/Load')
			},
			'find': {
				method: 'GET',
				url: base_url('AnswerSheet/Find')
			},
			'search': {
				method: 'GET',
				url: base_url('AnswerSheet/Search')
			},
			'save': {
				method: 'POST',
				url: base_url('AnswerSheet/Save')
			},
			'delete': {
				method: 'POST',
				url: base_url('AnswerSheet/Delete')
			},
			'saveBatch': {
				method: 'POST',
				url: base_url('AnswerSheet/SaveBatch')
			},
			'findBatch': {
				method: 'GET',
				url: base_url('AnswerSheet/FindBatch')
			},
			'exportAnswerSheetData': {
				method: 'GET',
				url: base_url('AnswerSheet/ExportAnswerSheetData')
			},
			'getBatchAnswerSheetDetail': {
				method: 'GET',
				url: base_url('AnswerSheet/GetBatchAnswerSheetDetail')
			},
			'sendToProcessing': {
				method: 'GET',
				url: base_url('AnswerSheet/SendToProcessing')
			},
			'getProcessingList': {
				method: 'GET',
				url: base_url('AnswerSheet/GetProcessingList')
			},
			'getSituationList': {
				method: 'GET',
				url: base_url('AnswerSheet/GetSituationList')
			},
			'generateAnswerSheet': {
				method: 'GET',
				url: base_url('AnswerSheet/GenerateAnswerSheet')
			},
			'searchTestLot': {
				method: 'GET',
				url: base_url('AnswerSheet/SearchTestLot')
			},
			'saveLot': {
				method: 'POST',
				url: base_url('AnswerSheet/SaveLot')
			},
			'deleteLot': {
			    method: 'POST',
			    url: base_url('AnswerSheet/DeleteLot')
			},
			'generateLotAgain': {
				method: 'POST',
				url: base_url('AnswerSheet/GenerateLotAgain')
			},
			'searchLotList': {
			    method: 'GET',
			    url: base_url('AnswerSheet/SearchLotList')
			},
			'getAnswerSheetLotSituationList': {
			    method: 'GET',
			    url: base_url('AnswerSheet/GetAnswerSheetLotSituationList')
			},
			'searchLotFiles': {
			    method: 'GET',
			    url: base_url('AnswerSheet/SearchLotFiles')
			},
			'searchAdheredTests': {
			    method: 'GET',
			    url: base_url('AnswerSheet/SearchAdheredTests')
			},
			'loadByUserGroupSearchTest': {
			    method: 'GET',
			    url: base_url('TestType/LoadByUserGroupSearchTest')
			},
			'searchLotHistory': {
			    method: 'GET',
			    url: base_url('AnswerSheet/SearchLotHistory')
			},
			'getStudentBySection': {
			    method: 'GET',
			    url: base_url('AnswerSheet/GetStudentBySection')
			},
			'getStudentFile': {
			    method: 'GET',
			    url: base_url('AnswerSheet/GetStudentFile')
			},
			'getAuthorize': {
			    method: 'GET',
			    url: base_url('AnswerSheet/GetAuthorize')
			},
			'getUploadQueueStatus': {
			    method: 'GET',
			    url: base_url('AnswerSheet/GetUploadQueueStatus')
			},
			'getUploadQueueStatusDreSchool': {
			    method: 'GET',
			    url: base_url('AnswerSheet/GetUploadQueueStatusDreSchool')
			},		    
			'getUploadQueueTop': {
			    method: 'GET',
			    url: base_url('AnswerSheet/GetUploadQueueTop')
			},
			'getSituationLot': {
			    method: 'GET',
			    url: base_url('AnswerSheet/GetSituationLot')
			},
			'deleteBatchQueueAndFiles': {
			    method: 'POST',
			    url: base_url('AnswerSheet/DeleteBatchQueueAndFiles')
			},
			'deleteBatchFilesError': {
			    method: 'POST',
			    url: base_url('AnswerSheet/DeleteBatchFilesError')
			}
            , 'deleteFileById': {
                method: 'POST',
                url: base_url('AnswerSheet/DeleteFileById')
            }
            
		};
		   
		return $resource('', {}, model);
	}]);
})();

