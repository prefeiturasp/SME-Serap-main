/**
 * function Controller para listagem de configuração de página
 * @namespace Controller
 */
(function (angular, $) {

    'use strict';

    //~SETTER
    angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    //~GETTER
    angular
		.module('appMain')
		.controller("PageConfigurationFormController", PageConfigurationFormController);


    PageConfigurationFormController.$inject = ['PageConfigurationModel', '$scope', '$rootScope', '$notification', '$location', '$window', '$util'];


    /**
	 * @function Controller listagem de configuração da página
	 * @name PageConfigurationController
	 * @namespace Controller
	 * @memberOf appMain
	 * @param {Object} PageConfigurationModel
	 * @param {Object} ng
	 * @param {Object} rootScope
	 * @param {Object} $notification
	 * @param {Object} $pager
	 * @param {Object} $location
	 */
    function PageConfigurationFormController(PageConfigurationModel, ng, rootScope, $notification, $location, $window, $util) {

        /**
		 * @function - Load
		 * @param
		 * @private
		 */
        function loadPage() {

            ng.listextensionsVideo = ['video/mp4', 'video/webm', 'video/ogg', 'application/ogg', 'video/x-flv', 'application/x-mpegURL', 'video/MP2T', 'video/3gpp', 'video/quicktime', 'video/x-msvideo', 'video/x-ms-wmv'];
            ng.listextensionsImage = ['image/jpeg', 'image/png', 'image/gif', 'image/bmp'];
            ng.selectedObjCategory;
            ng.categoryList = [];

            ng.PageConfiguration = {
                Category: undefined,
                CategoryCombo: {
                    Id: undefined,
                    Description: undefined,
                },
                Title: undefined,
                Description: undefined,
                ButtonDescription: undefined,
                Link: undefined,
                FileIllustrativeImage: {
                    Id: undefined,
                    Path: undefined,
                    Guid: $util.getGuid()
                },
                FileVideo: {
                    Id: undefined,
                    Path: undefined,
                    Guid: $util.getGuid()
                },
                Featured: false
            };

            ng.disableComboCategory = false;

            PageConfigurationModel.getCategoryList(function (result) {
                if (result.success) {
                    ng.categoryList = result.lista;

                    if (ng.params.Id > 0) {
                        loadPageConfiguration(ng.params.Id);
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        ng.selectFeaturedLinkExterno = function () {
            ng.PageConfiguration.Featured = !ng.PageConfiguration.Featured;
        };

        ng.selectFeaturedVideo = function () {
            ng.PageConfiguration.Featured = !ng.PageConfiguration.Featured;
        };

        ng.limparCampos = function () {
            ng.PageConfiguration.Title = undefined;
            ng.PageConfiguration.Description = undefined;
            ng.PageConfiguration.ButtonDescription = undefined;
            ng.PageConfiguration.Link = undefined;
            ng.PageConfiguration.FileIllustrativeImage = {
                Id: undefined,
                Path: undefined,
                Guid: $util.getGuid()
            };
            ng.PageConfiguration.FileVideo = {
                Id: undefined,
                Path: undefined,
                Guid: $util.getGuid()
            };
            ng.PageConfiguration.Featured = false;
        };

        /**
        * @function Carrega parametros quando acessar pagina
        * @private
        * @param {Object} current
        */
        function loadPageConfiguration(id) {

            if (id != undefined && id > 0) {
                var bd = { Id: id };

                //Pega elemento caso exista, senão cria um novo
                PageConfigurationModel.find(bd, function (result) {

                    if (result.success) {

                        ng.disableComboCategory = true;

                        ng.PageConfiguration = result.pageConfiguration;
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
            else {
                $notification[result.type ? result.type : 'error'](result.message);
            }
        };

        /**
        * @function Dispara salvar para a Etapa atual
        * @private
        * @param item: elemento que esta sendo validado
        */
        ng.salvar = salvar;
        function salvar() {

            if (ng.validate()) {

                ng.PageConfiguration.Category = ng.PageConfiguration.CategoryCombo.Id;

                PageConfigurationModel.save(ng.PageConfiguration, function (result) {

                    if (result.success) {

                        ng.PageConfiguration = undefined;

                        $notification.success(result.message);
                        $window.location.href = '/PageConfiguration/List';
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
        }

        ng.validate = function __validate() {

            if (ng.PageConfiguration != undefined) {

                if (ng.PageConfiguration.CategoryCombo.Id == 1) {

                    if (ng.PageConfiguration.Title === undefined || ng.PageConfiguration.Title === '') {
                        $notification.alert('Título é obrigatório.');
                        return false;
                    }

                    if (ng.PageConfiguration.Description === undefined || ng.PageConfiguration.Description === '') {
                        $notification.alert('Descrição é obrigatório.');
                        return false;
                    }
                }
                else if (ng.PageConfiguration.CategoryCombo.Id == 2) {
                    if (ng.PageConfiguration.Title === undefined || ng.PageConfiguration.Title === '') {
                        $notification.alert('Título é obrigatório.');
                        return false;
                    }

                    if (ng.PageConfiguration.Description === undefined || ng.PageConfiguration.Description === '') {
                        $notification.alert('Descrição é obrigatório.');
                        return false;
                    }

                    if (ng.PageConfiguration.Link === undefined || ng.PageConfiguration.Link === '') {
                        $notification.alert('Link é obrigatório.');
                        return false;
                    }

                    if(!validateUrl(ng.PageConfiguration.Link))
                    {
                        $notification.alert('Link está em um formato inválido.');
                        return false;
                    }
                }
                else if (ng.PageConfiguration.CategoryCombo.Id == 3) {
                    if (ng.PageConfiguration.Title === undefined || ng.PageConfiguration.Title === '') {
                        $notification.alert('Título é obrigatório.');
                        return false;
                    }

                    if (ng.PageConfiguration.Description === undefined || ng.PageConfiguration.Description === '') {
                        $notification.alert('Descrição é obrigatório.');
                        return false;
                    }

                    if (ng.PageConfiguration.ButtonDescription === undefined || ng.PageConfiguration.ButtonDescription === '') {
                        $notification.alert('Texto do botão é obrigatório.');
                        return false;
                    }

                    if (ng.PageConfiguration.Link === undefined || ng.PageConfiguration.Link === '') {
                        $notification.alert('Link é obrigatório.');
                        return false;
                    }

                    if (ng.PageConfiguration.FileIllustrativeImage.Id === undefined || ng.PageConfiguration.FileIllustrativeImage.Id === null) {
                        $notification.alert('Ícone é obrigatório.');
                        return false;
                    }

                    if (!validateUrl(ng.PageConfiguration.Link)) {
                        $notification.alert('Link está em um formato inválido.');
                        return false;
                    }
                }
                else if (ng.PageConfiguration.CategoryCombo.Id == 4) {
                    if (ng.PageConfiguration.Title === undefined || ng.PageConfiguration.Title === '') {
                        $notification.alert('Título é obrigatório.');
                        return false;
                    }

                    if (ng.PageConfiguration.Link === undefined || ng.PageConfiguration.Link === '') {
                        $notification.alert('Link é obrigatório.');
                        return false;
                    }

                    if (ng.PageConfiguration.FileIllustrativeImage.Id === undefined || ng.PageConfiguration.FileIllustrativeImage.Id === null) {
                        $notification.alert('Ícone é obrigatório.');
                        return false;
                    }

                    if (!validateUrl(ng.PageConfiguration.Link)) {
                        $notification.alert('Link está em um formato inválido.');
                        return false;
                    }
                }
                else if (ng.PageConfiguration.CategoryCombo.Id == 5) {
                    if (ng.PageConfiguration.Title === undefined || ng.PageConfiguration.Title === '') {
                        $notification.alert('Título é obrigatório.');
                        return false;
                    }

                    if (ng.PageConfiguration.Description === undefined || ng.PageConfiguration.Description === '') {
                        $notification.alert('Descrição é obrigatório.');
                        return false;
                    }

                    if (ng.PageConfiguration.FileVideo.Id === undefined || ng.PageConfiguration.FileVideo.Id === 0) {
                        $notification.alert('Vídeo é obrigatório.');
                        return false;
                    }

                    if (ng.PageConfiguration.FileIllustrativeImage.Id === undefined || ng.PageConfiguration.FileIllustrativeImage.Id === null) {
                        $notification.alert('Thumbnail é obrigatório.');
                        return false;
                    }
                }
            }
            return true;
        };

        //inicialização
        ng.params = $util.getUrlParams();
        loadPage();

        function validateUrl(value) {
            var regex = /(http|https):\/\/(\w+:{0,1}\w*)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%!\-\/]))?/;
            if (!regex.test(value)) {
                return false;
            } else {
                return true;
            }
        }

    };


})(angular, jQuery);