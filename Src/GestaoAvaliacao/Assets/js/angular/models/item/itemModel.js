/* 
* Item-Model
*/
(function () {
	angular.module('services').factory('ItemModel', ['$resource', function ($resource) {

		// Model
		var model = {

			'load': {
				method: 'GET',
				url: base_url('Item/Load')
			},
			'save': {
				method: 'POST',
				url: base_url('Item/Save')
			},
			'delete': {
				method: 'POST',
				url: base_url('Item/Delete')
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
			'getItemSummaryByTestItens': {
			    method: 'GET',
			    url: base_url('Item/GetItemSummaryByTestItens')
			},
			'getItemById': {
				method: 'GET',
				url: base_url('Item/GetItemById')
			},
			'getGradeByItem': {
				method: 'GET',
				url: base_url('Item/GetGradeByItem')
			},
			'getMatrixByItem': {
				method: 'GET',
				url: base_url('Item/GetMatrixByItem')
			},
			'getSimpleMatrixByItem': {
				method: 'GET',
				url: base_url('Item/GetSimpleMatrixByItem')
			},
			'getBaseTextByItem': {
				method: 'GET',
				url: base_url('Item/GetBaseTextByItem')
			},
			'getAddItemInfos': {
				method: 'GET',
				url: base_url('Item/GetAddItemInfos')
			},
			'previewPrint': {
			    method: 'GET',
				url: base_url('Item/PreviewPrintItem')
			},
			'validateItemCode': {
			    method: 'GET',
			    url: base_url('Item/ValidateItemCode')
			}

		};

		// Retorna o serviço       
		return $resource('', {}, model);

	}]);
})();

