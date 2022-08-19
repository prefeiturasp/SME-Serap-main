; (function (angular, $) {

    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    //~GETTER
    angular
        .module('appMain')
        .controller("AdminAcompanhamentoProvasController", AdminAcompanhamentoProvasController);

    AdminAcompanhamentoProvasController.$inject = ['$scope'];

    function HomeController(ng) { };

})(angular, jQuery);