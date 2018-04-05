/**
 * @function Redactor
 * @namespace Directive
 * @autor: Julio Cesar da Silva: 23/07/2015
 * uso: <textarea ng-model="content" redactor></textarea>
 *   additional options:
 *       -> redactor: hash (pass in a redactor options hash)
 *   callbacks 
 *       -> callbaks de retorno.    
 */
(function (angular, $) {

	'use strict';

	var redactorOptions = {};

	angular.module('directives')
		.constant('redactorOptions', redactorOptions)
		.directive('redactor', $Redactor);

	$Redactor.$inject = ['$timeout', '$notification', '$window'];

	function $Redactor($timeout, $notification, $window) {

		// suporte
		var helper = {
			supportFileReader: !!($window.FileReader),
			supportCanvas: !!($window.CanvasRenderingContext2D)
		};
	
		var _default = {
			loaded: false,
			imageConfig: {
				extensions: ['jpg', 'png', 'gif'],
				gifCompression: false,
				maxSizeFile: 1024,
				quality: 100,
				maxResolutionHeight: 300,
				maxResolutionWidth: 300
			}
		};

		var config = _default;

		var __directive = {
			restrict: 'A',
			require: 'ngModel',
			scope: {
				images: '=',
				tipo: '@'
			},
			link: {
				pre: __pre,
				post: __post
			}
		};

		function __pre (scope, element, attrs, ngModel) {

			scope.safeApply = function safeApply() {
				var $scope, fn, force = false;
				if (arguments.length === 1) {
					var arg = arguments[0];
					if (typeof arg === 'function') {
						fn = arg;
					} else {
						$scope = arg;
					}
				} else {
					$scope = arguments[0];
					fn = arguments[1];
					if (arguments.length === 3) {
						force = !!arguments[2];
					}
				}
				$scope = $scope || this;
				fn = fn || function () { };

				if (force || !$scope.$$phase) {
					$scope.$apply ? $scope.$apply(fn) : $scope.apply(fn);
				} else {
					fn();
				}
			};

			if (config.loaded == false) {
				config.loaded = true;
				config.imageConfig.extensions = [];
				var params = Parameters.General;
				for (var key in params) {
					switch (key) {
						case "JPEG":
						case "GIF":
						case "JPG":
						case "PNG":
						case "BMP": (params[key].toLowerCase() === 'true') ? config.imageConfig.extensions.push(key) : ''; break;
						case "IMAGE_GIF_COMPRESSION": config.imageConfig.gifCompression = params[key].toLowerCase(); break;
						case "IMAGE_MAX_SIZE_FILE": config.imageConfig.maxSizeFile = parseInt(params[key]); break;
						case "IMAGE_QUALITY": config.imageConfig.quality = parseInt(params[key]); break;
						case "IMAGE_MAX_RESOLUTION_HEIGHT": config.imageConfig.maxResolutionHeight = parseInt(params[key]); break;
						case "IMAGE_MAX_RESOLUTION_WIDTH": config.imageConfig.maxResolutionWidth = parseInt(params[key]); break;
					}
				}
			}
		};

		function __post (scope, element, attrs, ngModel) {

			function uploadSuccess(image, json) {
				$(image).attr('id', json.idFile);
				scope.images.push({
					Id: json.idFile,
					OwnerId: undefined,
					OwnerType: scope.tipo,
					ParentOwnerId: 0
				});
			};
		   
			function uploadError(json) {
				$notification.error(json);
				scope.safeApply();
			};

			function deleteImage( url, image ) {

				this.opts.buffer   = [];
				this.opts.rebuffer = [];
					
				var id = $(image).attr('id');
				for (var i = (scope.images.length-1); i >= 0 ; i--) {
		
					if (scope.images[i].Id == parseInt(id)) {
						scope.images.splice(i, 1);
					}
				}
			};

			function imageRules(message) {
				$notification.alert("Atenção", message);
				scope.safeApply();
			};

			function getTypeFile() {
				return scope.tipo;
			};

			var updateModel = function updateModel(value) {
				$timeout(function () {
					scope.$apply(function () {
						ngModel.$setViewValue(value);
					});
				});
			},

			options = {
				plugins: ['table', 'fontfamily', 'fontcolor', 'fontsize', 'clips', 'imagemanager', 'mathLatex'],
				//img config.
				imageConfig: config.imageConfig,
				uploadType: 'customSender',
				imageUpload: base_url("File/Upload"),
				undoRedoImageControll: [],
				//callbacks
				changeCallback: updateModel,
				imageUploadCallback: uploadSuccess,
				imageUploadErrorCallback: uploadError,
				imageDeleteCallback: deleteImage,
				imageRulesCallback: imageRules,
				getTypeFileCallback: getTypeFile,
				//idioma
				lang: 'pt_br',
				langs: {
					pt_br: {
						html: 'Ver HTML',
						video: 'V&iacute;deo',
						image: 'Imagem',
						table: 'Tabela',
						link: 'Link',
						link_insert: 'Inserir link...',
						link_edit: 'Edit link',
						unlink: 'Remover link',
						formatting: 'Estilos',
						paragraph: 'Par&aacute;grafo',
						quote: 'Cita&ccedil;&atilde;o',
						code: 'C&oacute;digo',
						header1: 'T&iacute;tulo 1',
						header2: 'T&iacute;tulo 2',
						header3: 'T&iacute;tulo 3',
						header4: 'T&iacute;tulo 4',
						header5: 'T&iacute;tulo 5',
						bold: 'Negrito',
						italic: 'It&aacute;lico',
						fontcolor: 'Cor da fonte',
						backcolor: 'Cor do fundo',
						unorderedlist: 'Lista n&atilde;o ordenada',
						orderedlist: 'Lista ordenada',
						outdent: 'Remover identa&ccedil;&atilde;o',
						indent: 'Identar',
						cancel: 'Cancelar',
						insert: 'Inserir',
						save: 'Salvar',
						_delete: 'Remover',
						insert_table: 'Inserir tabela',
						add_cell: 'Adicionar coluna',
						delete_cell: 'Remover coluna',
						add_row: 'Adicionar linha',
						delete_row: 'Remover linha',
						thead: 'Cabe&ccedil;alho',
						delete_table: 'Remover tabela',
						insert_row_above: 'Adicionar linha acima',
						insert_row_below: 'Adicionar linha abaixo',
						insert_column_left: 'Adicionar coluna &agrave; esquerda',
						insert_column_right: 'Adicionar coluna da direita',
						delete_column: 'Remover Coluna',
						rows: 'Linhas',
						columns: 'Colunas',
						add_head: 'Adicionar cabe&ccedil;alho',
						delete_head: 'Remover cabe&ccedil;alho',
						title: 'T&iacute;tulo',
						image_position: 'Posi&ccedil;&atilde;o',
						none: 'Nenhum',
						left: 'Esquerda',
						right: 'Direita',
						image_web_link: 'Link para uma imagem',
						text: 'Texto',
						mailto: 'Email',
						web: 'URL',
						video_html_code: 'C&oacute;digo de incorpora&ccedil;&atilde;o',
						file: 'Arquivo',
						upload: 'Upload',
						download: 'Download',
						choose: 'Escolha',
						or_choose: 'Ou escolha',
						drop_file_here: 'Arraste um arquivo at&eacute; aqui',
						align_left: 'Alinhar a esquerda',
						align_center: 'Centralizar',
						align_right: 'Alinhar a direita',
						align_justify: 'Justificar',
						horizontalrule: 'Linha horizontal',
						fullscreen: 'Tela cheia',
						deleted: 'Removido',
						anchor: '&Acirc;nchora',
						link_new_tab: 'Abrir link em nova janela/aba',
						underline: 'Sublinhado',
						alignment: 'Alinhamento',
						filename: 'Nome (opcional)',
						edit: 'Editar',
						center: 'Center'
					}
				}
			},
				
			additionalOptions = attrs.redactor ? scope.$eval(attrs.redactor) : {}, editor,
			$_element = angular.element(element);
			angular.extend(options, redactorOptions, additionalOptions);

			//update redactor plugin
			$timeout(function () {
				editor = $_element.redactor(options);
				ngModel.$render();
			});

			//render on redactor
			ngModel.$render = function () {
				if (angular.isDefined(editor)) {
					$timeout(function () {
						var value = '';
						if (ngModel.$viewValue) {
							value = ngModel.$viewValue.replace(/\s\s/g, '&nbsp;&nbsp;').replace(/&nbsp;\s/g, '&nbsp;&nbsp;')
						}

						$_element.redactor('code.set', value);
					});
				}
			};
		};

		return __directive;
	};

})(angular, jQuery);