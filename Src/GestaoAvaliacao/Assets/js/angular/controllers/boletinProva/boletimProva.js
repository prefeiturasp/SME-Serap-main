; (function (angular, $) {

    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    //~GETTER
    angular
        .module('appMain')
        .controller("BoletimProvaController", BoletimProvaController);

    BoletimProvaController.$inject = ['$scope'];

    function HomeController(ng) { };

})(angular, jQuery);