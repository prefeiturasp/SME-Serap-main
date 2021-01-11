(function (angular, $) {

    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives']);

    //~GETTER
    angular
        .module('appMain')
        .controller("HomeAlunoElectronicTestController", HomeAlunoElectronicTestController);


    HomeAlunoElectronicTestController.$inject = ['$scope', '$notification', '$location', '$anchorScroll', '$util', '$timeout', '$window'];


    function HomeAlunoElectronicTestController(ng, $notification, $location, $anchorScroll, $util, $timeout, $window) {

        function Init() {
            $notification.clear();
        };

        Init();

    };
})(angular, jQuery);