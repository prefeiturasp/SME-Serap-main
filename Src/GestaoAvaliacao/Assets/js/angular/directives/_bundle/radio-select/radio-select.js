/**
 * AngularJS Diretive - Criar radio button dinâmico.
 * Autor: Julio Silva
 */
(function (angular, $) {

	'use strict';

	angular
		.module('directives')
		.directive('radioSelect', RadioSelect);

	RadioSelect.$inject = ['$timeout'];

	function RadioSelect($timeout) {

		var template = '<form class="radio-select-form" >' +
			'<div ng-class="\'radio radio-primary {{customClass}}\' " ng-repeat="($indexRadioSelect, _list) in radiolist track by $indexRadioSelect">' +
				'<input id="{{radioid}}_{{_list.Description}}_{{$index}}" type="radio" ng-click="select(_list)" ng-checked="internalselected == _list.Id">' +
				'<label for="{{radioid}}_{{_list.Description}}_{{$index}}" data-trigger="hover" data-type="success" data-title="{{_list.Description}}" data-placement="top" bs-tooltip>' +
				   '{{minimizeText(_list.Description, 16)}}' +
				'</label>' +
			'</div>' +
		'</form>';

		return {
			restrict: 'A',
			template: template,
			scope: {
				radiolist: '=',
				radioselected: '=',          
				internalSelected: '&',
				inline: '=',
				radioid: '=',
				minimize: '=',
				changer: '=',
				customClass:'@'
			},
			link: {
				pre: function (scope) {

					var __load = scope.$watch("radioselected", load);

					function load() {
						if (scope.radioselected !== undefined && scope.radioselected !== null) {
							//__load();
							scope.internalselected = scope.radioselected.Id;
							if (scope.changer)
								scope.changer(scope.radioselected);
							
						}
					};
				},
				post: function (scope) {

					scope.select = function (_value) {

						scope.radioselected = _value;
						scope.internalselected = _value.Id;

						if (scope.changer)
							scope.changer(scope.radioselected);
					};

					scope.minimizeText = function (_text, _length) {

						if (scope.minimize == undefined) {
							return _text;
						}
						else if (_text.length > _length) {
							_text = _text.substring(0, _length) + "...";
						}
						return _text;
					};
				}
			}
		};
	};

})(angular, jQuery);