/**
 * function modal default
 * @namespace Directive
 * @author julio cesar da silva - 14/05/2015
 */
(function (angular, $) {

    'use strict';

    angular.module('directives')
           .directive('modal', modal);

    function modal() {
    	var _template = '<div class="modal fade">' +
                            '<div class="modal-dialog" ng-style="dialogStyle">' +
                                '<div class="modal-content">' +
                                    '<div class="modal-header">' +
                                        '<button ng-hide="btnclose" type="button" class="close" data-dismiss="modal" aria-label="Close" ng-click="onclosemodal()"><span aria-hidden="true">&times;</span></button>' +
                                        '<h4 class="modal-title" ng-bind-html="modalTitle"></h4>' +
                                    '</div>' +
                                    '<div class="modal-body">' +
                                        '<div ng-transclude></div>' +
                                    '</div>' +
                                    /*'<div class="modal-footer">' +
                                        '<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>' +
                                        '<button type="button" class="btn btn-primary">Save changes</button>' +
                                    '</div>' +*/
                                '</div>' +
                            '</div>' +
                        '</div>';

    	var _directive = {
    		restrict: 'EA',
    		scope: {
    			modalTitle: '@',
    			method: '&onComplete',
    			onclosemodal: '&onclosemodal',
    			btnclose: '='
    		},
    		replace: true,
    		transclude: true,
    		link: _link,
    		template: _template
    	};

    	function _link(scope, element, attrs) {
    		var method = scope.method();

    		if (method) {
    			element.on('shown.bs.modal', function (e) {
    				method();
    			});
    		}    		    		

    		scope.dialogStyle = {};

    		if (attrs.size) {
    		    element.find(".modal-dialog").removeClass("modal-" + attrs.size);
    		    element.find(".modal-dialog").addClass("modal-" + attrs.size);
    		}

    		if (attrs.width)
    			scope.dialogStyle.width = attrs.width;

    		if (attrs.height)
    			scope.dialogStyle.height = attrs.height;
    	}

    	return _directive;
    };

})(angular, jQuery);