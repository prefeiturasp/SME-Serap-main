; (function (angular, $) {

    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    //~GETTER
    angular
        .module('appMain')
        .controller("AdminSerapEstudantesController", AdminSerapEstudantesController);

    AdminSerapEstudantesController.$inject = ['$scope'];

    function HomeController(ng) {};

})(angular, jQuery);