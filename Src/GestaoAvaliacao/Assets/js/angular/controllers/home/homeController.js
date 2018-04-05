;(function (angular, $) {

    'use strict';
  
    //~SETTER
    angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    //~GETTER
    angular
		.module('appMain')
		.controller("HomeController", HomeController);

    HomeController.$inject = ['$scope', '$notification', 'PageConfigurationModel', '$sce'];

    function HomeController(ng, $notification, PageConfigurationModel, $sce) {

        /**
        * @function - Load
        * @param
        * @public
        */
        function load() {
            ng.pageConfigurationTextoPrincipal = [];
            ng.pageConfigurationLinkAcessoExterno = [];
            ng.pageConfigurationFerramentaDestaque = [];
            ng.pageConfigurationFerramenta = [];
            ng.pageConfigurationVideo = [];

            PageConfigurationModel.loadAll(function (result) {
                if (result.success) {

                    ng.pageConfigurationTextoPrincipal = result.pageConfigurationTextoPrincipal;
                    ng.pageConfigurationLinkAcessoExternoDestaque = result.pageConfigurationLinkAcessoExternoDestaque;
                    ng.pageConfigurationLinkAcessoExterno = result.pageConfigurationLinkAcessoExterno;
                    ng.pageConfigurationFerramentaDestaque = result.pageConfigurationFerramentaDestaque;
                    ng.pageConfigurationFerramenta = result.pageConfigurationFerramenta;
                    ng.pageConfigurationVideoDestaque = result.pageConfigurationVideoDestaque;
                    ng.pageConfigurationVideo = result.pageConfigurationVideo;

                } else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        }

        ng.trustSrc = function (src) {
            return $sce.trustAsResourceUrl(src);
        }

        ng.loadVideo = function (video) {
            ng.pageConfigurationVideoDestaque = video;
        }

        load();
    };

})(angular, jQuery);