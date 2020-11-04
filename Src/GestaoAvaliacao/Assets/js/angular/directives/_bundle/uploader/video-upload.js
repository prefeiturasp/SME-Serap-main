/**
 * @function campo do tipo vide-upload
 * @namespace Directive
 * @autor: Carlos Augusto Ferreira Dias: 11/08/2020
 */
(function (angular, $) {

    'use strict';

    angular.module('directives')
        .directive('videoUpload', UploadVideo)
        .config(Config);

    Config.$inject = [];
    UploadVideo.$inject = ['$http', '$notification', '$util'];

    function Config() {
        XMLHttpRequest.prototype.setRequestHeader = (function (sup) {
            return function (header, value) {
                if ((header === "__XHR__") && angular.isFunction(value))
                    value(this);
                else
                    sup.apply(this, arguments);
            };
        })(XMLHttpRequest.prototype.setRequestHeader);
    };

    function UploadVideo($http, $notification, $util) {
        debugger;
        var __template = '<div class="upload-group" ng-class="customclass">' +
                            '<div ng-if="labelName && field.name"><label>Último arquivo selecionado:</label><br><label>{{field.name}}</label><br><br></div>' +
							'<input type="text" placeholder="Clique no ícone para adicionar um vídeo" class="form-control input-upload" ng-model="component.File.Path" disabled="true">' +
							'<span class="input-group-btn fake-upload">' +
								'<i class="material-icons">file_upload</i>' +
								'<input type="file" title="Arquivo" uploader="field" id="file_uploader_{{component.File.Guid}}" ng-disabled="look"/>' +
                            '</span>' +	
                            '<span ng-if="uploading" title="Convertendo vídeo para o formato WEBM.." id="converting_video_loader_{{component.File.Guid}}" class="loader"></span>' +
							'<div class="progress">' +
							  '<div id="{{component.File.Guid}}" class="progress-bar progress-bar-success active" role="progressbar" style="width: 0%">' +
							  '</div>' +
							'</div>' +
						'</div>';

        var _directive = {
            restrict: 'A',
            template: __template,
            scope: {
            	component: '=',
            	type: '@',
            	callback: '=',
            	uploadingStatusCallback: '=',
            	trash: '=',
            	look: '=',
            	labelName: '=',
            	listextensions: '=',
            	placeholder: '@',
            	url: '@',
            	customclass: "@"
            },
            link: _link
        };

        function _link($scope, element, attr) {

            $scope.field = undefined;
            $scope.$watch('field', videoUploading, false);

        	/**
			 * @function Remove
			 * @param
			 * @returns
			 */
            $scope.remove = function __remove() {
            	$scope.progressup = 0;
            	angular.element('#' + $scope.component.File.Guid).css('width', $scope.progressup + '%');
            	clearField();
            	if ($scope.component.File.Id !== undefined && $scope.component.File.Id !== null) {
            		var form = new FormData();
                    form.append('id', $scope.component.File.Id);
                    form.append('convertedFileId', $scope.component.ConvertedFile.Id);
            		$http.post($util.getWindowLocation('/File/DeleteVideoAsync'), form, {
            			transformRequest: angular.identity,
            			headers: { 'Content-Type': undefined }
            		})
					.success(function (data, status) {

						if (data.success) {
							$notification.success(data.message);
							$scope.component.File.Id = undefined;
							$scope.component.File.Path = undefined;
						}
						else {
							$notification[data.type ? data.type : 'error'](data.message);
						}
					})
					.error(function (data, status) {
						
					});
            	}
            };

            /**
			 * @function Upload de vídeo
			 * @param
			 * @return   
			 */
            function videoUploading() {
                if ($scope.field !== null && $scope.field !== undefined) {
                    if (checkTypeFile()) {
                        $scope.progressup = 0;
                        $scope.uploading = true;
                        if ($scope.uploadingStatusCallback) $scope.uploadingStatusCallback(true);
                        var form = new FormData();
                        form.append('file', $scope.field);
                        $http.post($util.getWindowLocation('/File/UploadVideoAsync'), form, {
                            transformRequest: angular.identity,
                            headers: {
                                'Content-Type': undefined,
                                '__XHR__': function () {
                                    return function (xhr) {
                                        xhr.upload.addEventListener("progress", function (event) {
                                            debugger;
                                            $scope.progressup = parseInt(((event.loaded / event.total) * 100));
                                            if ($scope.progressup < 99)
                                                angular.element('#' + $scope.component.File.Guid).css('width', $scope.progressup + '%');
                                        });
                                    };
                                }
                            }
                        })
                        .success(function (data, status) {
                            if (data.success) {
                                $notification.success(data.message);
                                $scope.component.File.Id = data.idFile;
                                $scope.component.File.Path = data.filelink;

                                if ($scope.component.ConvertedFile === undefined) $scope.component.ConvertedFile = {};
                                $scope.component.ConvertedFile.Id = data.idConvertedFile;
                                $scope.component.ConvertedFile.Path = data.convertedVideoLink;
                                clear(data);
                            }
                            else {
                                $notification[data.type ? data.type : 'error'](data.message);
                                clear(data);
                            }
                        })
                        .error(function (data, status) {
                            $notification.error(data);
                            clear(data);
                        });
                    } else {
                        clear();
                    }
                }
            };

            /**
             * @function Clear when error
             * @param
             * @returns
             */
            function clear(data) {
                $scope.uploading = false;
                $scope.progressup = 0;
                $scope.field = undefined;
                if ($scope.callback && data)
                    if (data.success)
                        $scope.callback(data);
                if ($scope.uploadingStatusCallback) $scope.uploadingStatusCallback(false);
                try {
                    $('#file_uploader_' + $scope.component.File.Guid).val(null);
                    angular.element('#' + $scope.component.File.Guid).css('width', $scope.progressup + '%');
                    component.File.Path = "";
                } catch (e) { }
            };


        	/**
			 * @function conversor de bytes
			 * @param num - bytes a serem convertidos
			 * @return   
			 */
            function kmgtbytes(num) {
            	if (num > 0) {
            		if (num < 1024) { return [num, 'Byte'] }
            		if (num < 1048576) { return [parseInt(num / 1024), 'KB'] }
            		if (num < 1073741824) { return [parseInt(num / 1024 / 1024), 'MB'] }
            		if (num < 1099511600000) { return [parseInt(num / 1024 / 1024 / 1024), 'GB'] }
            		return [num / 1024 / 1024 / 1024 / 1024, "TB"]
            	}
            	return num
            };

        	/**
			 * @function Valida o tamanh e o tipo de arquivos permitidos a serem salvos
			 * @name checkTypeFile
			 * @namespace upload
			 * @memberOf Directive
			 * @param
			 * @return   
			 */
            function checkTypeFile() {
            	var allowExtension = Parameters.General;
            	if ($scope.field.type == "application/x-msdownload") {
            		$notification['alert']("Não é permitido enviar esse tipo de arquivo.");
            		return false;
            	}
            	//verifica a listas de extensoões vinda do html,
            	//para saber quais tipos de extensões é permitido salvar
            	if ($scope.listextensions != undefined) {

            		var flag = false;
            		for (var i = 0; i < $scope.listextensions.length; i++) {
            			if ($scope.field.type == $scope.listextensions[i]) flag = true;
            		}

            		if (!flag) $notification['alert']("Não é permitido enviar esse tipo de arquivo.");
            		return flag;
            	}

            	var parameterSize = kmgtbytes(parseInt(allowExtension.FILE_MAX_SIZE) * 1024);
            	var tamanhoArquivo = parseInt($scope.field.size);
                var fileSize = kmgtbytes(tamanhoArquivo);

            	if (fileSize[1] == parameterSize[1]) {
            		if (fileSize[0] > parameterSize[0]) {
            			$notification['alert']("Tamanho do arquivo excede o permitido (" + parameterSize[0] + parameterSize[1] + ")!");
            			return false;
            		}
            	} else if (fileSize[1] != parameterSize[1]) {
            		if (fileSize[1] == 'GB' || fileSize[1] == 'TR') {
            			$notification['error']("Tamanho do arquivo excede o permitido (" + parameterSize[0] + parameterSize[1]+")!");
            			return false;
            		}
                }   
                
                return true;
            };
        };

        return _directive;
    };

})(angular, jQuery);