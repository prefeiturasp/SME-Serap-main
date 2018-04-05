;(function (angular, $) {

    'use strict';
  
    //~SETTER
    angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    //~GETTER
    angular
		.module('appMain')
        .controller("ProvaSPController", ProvaSPController);

    //ProvaSPController.$inject = ['$scope', '$notification', 'PageConfigurationModel', '$sce'];

    function ProvaSPController(ng, $notification, PageConfigurationModel, $sce) {

        /**
        * @function - Load
        * @param
        * @public
        */

        
    };

})(angular, jQuery);