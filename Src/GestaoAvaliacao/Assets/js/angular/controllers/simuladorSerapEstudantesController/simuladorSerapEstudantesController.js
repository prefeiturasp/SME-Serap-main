; (function (angular, $) {

    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    //~GETTER
    angular
        .module('appMain')
        .controller("SimuladorSerapEstudantesController", SimuladorSerapEstudantesController);

    SimuladorSerapEstudantesController.$inject = ['$scope'];

})(angular, jQuery);