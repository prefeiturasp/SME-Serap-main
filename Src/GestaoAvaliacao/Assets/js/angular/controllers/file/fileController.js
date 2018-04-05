/**
 * function TestController Controller
 * @namespace Controller
 * @author Everton Ferreira - 05/11/2015
 */
(function (angular, $) {

	//~SETTER
	angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

	//~GETTER
	angular
		.module('appMain')
		.controller("FileController", FileController);


	FileController.$inject = ['$rootScope', '$scope', 'FileModel', 'TestListModel', '$notification', '$timeout', '$sce', '$pager', '$compile', '$util'];

	function FileController($rootScope, $scope, FileModel, TestListModel, $notification, $timeout, $sce, $pager, $compile, $util) {

	    var self = this;
	    var params = $util.getUrlParams();

	    /**
		 * @function carrega funções de configuração
		 * @private
		 * @param
		 */
	    $scope.load = function _load() {
	        $notification.clear();
	        configVariaveis();

	    };

	    /**
        * @name configVariaveis
        * @namespace FileController
        * @desc 
        * @param {Object} 
        * @memberOf Controller.FileController
        */
	    function configVariaveis() {

	        $scope.countFilter = 0;
	        $scope.arquivosList = [];
	        $scope.selectedFile = null;
	        $scope.modelFile = { Guid: '' }
	        $scope.modelFile.Guid = $util.getGuid();
	        $scope.messageVinculo = "Ao excluir o arquivo os vínculos com as provas serão removidos, impedindo o arquivo de ser baixado. Deseja continuar?";
	        $scope.messageDelete = "Você tem certeza que deseja excluir o arquivo?";
	        $scope.messege = null;
	        $scope.linkArchive = null;
	        $scope.pageSize = 10;
	        $scope.checkList = [];
	        $scope.ListSaveHistory = [];
	        //variaveis de pesquisa
	        $scope.nameArquivo = "";
	        $scope.dateStart = null;
	        $scope.dateEnd = "";
	        $scope.clickFilter = false;
	        $scope.typeFilter = {
	            linked: false
	        };
	        //variavel de vinculo de arquivos
	        //responsavel por esconder componentes da etapa de vincular arquivos
	        if (params.Id) {

	            $scope.linkArchive = true;
	            $scope.title = "Vincular arquivos";
	            $scope.paginate = $pager(TestListModel.searchTestFiles);
	            // variaveis do vinculo da prova
	            $scope.copyListFile = null;
	            $scope.saveLink = false;
	            $scope.vinculoIndex = null;
	            $scope.testName = "";
	            $scope.totalFiles = 0;
	            $scope.totalTestLinked = 0;
	            $scope.selectedAll = { CheckedAll: false }
	        } else {
	            $scope.linkArchive = false;
	            $scope.title = "Upload de arquivos";
	            $scope.paginate = $pager(FileModel.searchUploadedFiles);
	        }
	        $scope.getListFile();
	        $scope.$watchCollection("[dateStart, dateEnd, typeFilter.linked]", function () {
	            $scope.countFilter = 0;
	            if ($scope.dateStart) $scope.countFilter += 1;
	            if ($scope.dateEnd) $scope.countFilter += 1;
	            if ($scope.typeFilter.linked && $scope.linkArchive) $scope.countFilter += 1;
	        });
	    };

	    /**
        * @name getListFile
        * @namespace FileController
        * @desc Busca a lista de arquivos
        * @param {Object} 
        * @memberOf Controller.FileController
        */
	    $scope.getListFile = function _getListFile(paginate, searchCode) {
	        if (paginate == 'paginate' && params != undefined) listSelectedOptions();
	        if (!searchCode) {
	            var initDate = new Date($scope.dateStart);
	            var finalDate = new Date($scope.dateEnd);

	            if (initDate > finalDate) {
	                $notification['alert']('Data de upload inicial tem que ser maior que a data final.');
	                return;
	            }
	        }
	        var filter = !searchCode ? {
	            StartDate: $scope.dateStart,
	            EndDate: $scope.dateEnd,
	            OwnerType: 7,
	            OwnerId: parseInt(params.Id) || null,
	            ShowLinks: $scope.typeFilter.linked
	        } :
            {
                Description: $scope.nameArquivo,
                OwnerId: parseInt(params.Id) || null
            };
	        $scope.paginate.paginate(filter).then(
				function (result) {
				    if (result.success) {
				        if (result.lista.length > 0) {
				            $scope.paginate.nextPage();
				            $scope.arquivosList = result.lista;
				            $scope.pageSize = result.pageSize;
				            if (!$scope.pages > 0) {
				                $scope.pages = $scope.paginate.totalPages();
				                $scope.totalItens = $scope.paginate.totalItens();
				            }
				            if (params.Id) {
				                $scope.testName = $scope.arquivosList[0].OwnerName;
				                $scope.totalFiles = $scope.arquivosList[0].AllFiles;
				                $scope.totalTestLinked = $scope.arquivosList[0].TestLinks.length;
				                var id = $scope.paginate.currentPage() > 0 ? $scope.paginate.currentPage() - 1 : $scope.paginate.currentPage();
				                // verifica se a opção de tds os arquivos vinculados está selecionada
				                if (!$scope.selectedAll.CheckedAll) {
				                    if ($scope.selectedAll.CheckedAll) {
				                        checkAllFiles($scope.selectedAll.CheckedAll);
				                    }
				                    else {
				                        //compara a lista de arquivos da pagina com a lista de histórico
				                        for (var j = 0; j < $scope.arquivosList.length; j++) {
				                            for (var k = 0; k < $scope.ListSaveHistory.length; k++) {
				                                //compara se os arquivos das listas são os mesmos
				                                if ($scope.arquivosList[j].Id == $scope.ListSaveHistory[k].Id) {
				                                    //se na lista de histórico estiver marcado true tem marca true na lista de arquivos da pagina
				                                    if ($scope.ListSaveHistory[k].Checked)
				                                        $scope.arquivosList[j].Checked = true;
				                                }//if
				                            }//for k
				                        }//for j
				                    }
				                }
				                else if ($scope.checkList[id] == undefined) {
				                    $scope.selectedAll.CheckedAll = false;
				                } else {
				                    checkAllFiles($scope.selectedAll.CheckedAll);
				                }
				                if ($scope.totalTestLinked > 0) {
				                    var cont = 0;
				                    // compara as listas dos arquivos da pagina com a lista dos arquivos vinculaods  
				                    for (var l = 0; l < $scope.arquivosList.length; l++) {
				                        for (var m = 0; m < $scope.arquivosList[0].TestLinks.length; m++) {
				                            // compara os IDs se forem iguais marca como selecionado os arquivos
				                            if ($scope.arquivosList[l].Id == $scope.arquivosList[0].TestLinks[m].File_Id) {
				                                $scope.arquivosList[l].Checked = true;
				                                cont++;
				                            }//if
				                        }
				                    }
				                    if (cont == result.pageSize)//|| $scope.arquivosList.length < result.pageSize
				                        // se todos os arquivos da pagina tiverem selecionados ativa a seleção de todos
				                        $scope.selectedAll.CheckedAll = true;
				                }
				                $scope.copyListFile = angular.copy($scope.arquivosList);
				            }

				        } else {
				            $scope.arquivosList = null;
				        }
				        $scope.typeFilter.selected = false;
				        if (!searchCode && searchCode == undefined) $scope.codigo = "";
				    } else {
				        $notification[result.type ? result.type : 'error'](result.message);
				    }
				});
	    };

	    /**
        * @name searchFile
        * @namespace FileController
        * @desc 
        * @param {Object} 
        * @memberOf Controller.FileController
        */
	    $scope.searchFile = function _searchFile(searchCode) {

	        if (!$scope.saveLink) {
	            $scope.pages = 0;
	            $scope.totalItens = 0;
	            $scope.paginate.indexPage(0);
	            $scope.pageSize = $scope.paginate.getPageSize();
	            $scope.getListFile(null, searchCode);
	        } else {
	            angular.element("#modalAlert").modal({ backdrop: 'static' });
	        }//else

	    };

	    /**
        * @name newSearch
        * @namespace FileController
        * @desc Efetua nova pesquisa quando os vinculos não forem salvos
        * @param {Object} 
        * @memberOf Controller.FileController
        */
	    $scope.newSearch = function () {
	        $scope.pages = 0;
	        $scope.totalItens = 0;
	        $scope.paginate.indexPage(0);
	        $scope.pageSize = $scope.paginate.getPageSize();
	        $scope.getListFile();
	        angular.element('#modalAlert').modal('hide');
	    };

	    /**
        * @name fileUploadSuccess
        * @namespace FileController
        * @desc callback da directive do upload
        * @param {Object} 
        * @memberOf Controller.FileController
        */
	    $scope.fileUploadSuccess = function __fileUploadSuccess(data) {

	        $timeout(function () {
	            angular.element('#' + $scope.modelFile.Guid).css('width', '0%');
	            $scope.modelFile.Path = "";
	        }, 2000);

	        $scope.LimparCampos();
	        $scope.searchFile();
	    };

	    $scope.datepicker = function (id) { $("#" + id).datepicker('show'); };

	    /**
         * @function Validação de inicial maior que final
         * @author julio.silva@mstech.com.br
         * @since 03/10/2016
         */
	    $scope.changeDate = function changeDate() {
	        if ($util.greaterEndDateThanStartDate($scope.dateStart, $scope.dateEnd) === false) {
	            $notification.alert("'Data de Término' deve ser maior que 'Data de Início'.");
	            $scope.dateEnd = "";
	        }
	    };

	    /**
         * POPOVER: Possíveis ações para a prova selecionada
         * na lista de provas encontradas na busca
         */
	    $scope.popovermenu = {
	        title: "empty",
	        content: "empty",
	        baixar: function () {

	            FileModel.checkFileExists({ Id: $scope.selectedFile.Id }, function (result) {

	                if (result.success) {
	                    window.open("/File/DownloadFile?Id=" + $scope.selectedFile.Id, "_self");
	                }
	                else {
	                    $notification.alert("Não foi possível baixar o arquivo selecionado!");
	                }
	            });

	        },
	        excluir: function () {

	            // verifica se o arquivo tem provas vinculada a ele e mostra mensagem diferente caso tenha ou não
	            if ($scope.selectedFile.AllLinks.length > 0)
	                $scope.message = $scope.messageVinculo;
	            else $scope.message = $scope.messageDelete;

	            angular.element("#modalExcluir").modal({ backdrop: 'static' });

	        }
	    };

	    /**
        * @name LimparCampos
        * @namespace FileController
        * @desc limpa os campos de pesquisa
        * @param {Object} 
        * @memberOf Controller.FileController
        */
	    $scope.LimparCampos = function _LimparCampos() {
	        $scope.dateStart = "";
	        $scope.dateEnd = "";
	        $scope.typeFilter.linked = false;
	        $scope.countFilter = 0;
	    };

	    /**
        * @name cancelDeleteFile
        * @namespace FileController
        * @desc Fecha modal de exclusão de arquivo
        * @param
        * @memberOf Controller.FileController
        */
	    $scope.cancelDeleteFile = function _cancelDeleteFile() {

	        angular.element('#modalExcluir').modal('hide');

	    };

	    /**
        * @name deleteFile
        * @namespace FileController
        * @desc Deleta o arquivo selecionado caso ele não esteje vinculado a uma prova
        * @param
        * @memberOf Controller.FileController
        */
	    $scope.deleteFile = function __deleteFile() {

	        FileModel.delete({ Id: $scope.selectedFile.Id },

				function (result) {

				    if (result.success) {
				        angular.element("#modalExcluir").modal("hide");
				        $notification.success(result.message);
				        $scope.selectedFilee = undefined;
				        $scope.searchFile();
				    } else {
				        $notification[result.type ? result.type : 'error'](result.message);
				    }
				},
				function () {
				});

	    };

	    /**
        * @name saveFiles
        * @namespace FileController
        * @desc Cria objeto a ser enviado pro BD com os arquivos vinculados
        * @param {Object} 
        * @memberOf Controller.FileController
        */
	    $scope.saveFiles = function _saveFiles() {

	        var listFileLink = [];
	        checkLitsToCompare();

	        var flag = false
	        for (var l = 0; l < $scope.arquivosList[0].TestLinks.length; l++) {
	            flag = false
	            for (var m = 0; m < $scope.ListSaveHistory.length; m++) {
	                // compara os IDs se forem iguais marca como selecionado os arquivos
	                if ($scope.arquivosList[0].TestLinks[l].File_Id == $scope.ListSaveHistory[m].Id) {
	                    flag = true;
	                    break;
	                }//if
	            }//for k

	            if (!flag) {
	                $scope.arquivosList[0].TestLinks[l].Checked = true;
	                $scope.arquivosList[0].TestLinks[l].Id = $scope.arquivosList[0].TestLinks[l].File_Id
	                $scope.ListSaveHistory.push($scope.arquivosList[0].TestLinks[l]);
	            }//if

	        }//for j

	        //
	        for (var i = 0; i < $scope.arquivosList.length; i++) {
	            for (var n = 0; n < $scope.ListSaveHistory.length; n++) {
	                // compara os IDs 
	                if ($scope.arquivosList[i].Id == $scope.ListSaveHistory[n].Id)
	                    //se elemento da lista não estiver selecionado remove ele da lista de histórico
	                    if (!$scope.arquivosList[i].Checked) {
	                        $scope.ListSaveHistory.splice(n, 1);
	                        break;
	                    }

	            }//for n

	        }//for i

	        //salvas apenas os ID dos arquivos para mandar pro back
	        for (var i = 0; i < $scope.ListSaveHistory.length; i++) {
	            //if ($scope.ListSaveHistory[i].Checked)
	            listFileLink.push({ File_Id: $scope.ListSaveHistory[i].Id });
	        }//if

	        save(listFileLink);
	    };

	    /**
        * @name save
        * @namespace FileController
        * @desc Salva os arquivos vinculados no BD
        * @param {Object} 
        * @memberOf Controller.FileController
        */
	    function save(filesLink) {

	        TestListModel.saveTestFiles({ Id: params.Id, files: filesLink },

				function (result) {

				    if (result.success) {
				        $scope.totalTestLinked = result.TestLinks.length;
				        $scope.saveLink = false;
				        $notification.success(result.message);

				        $scope.searchFile();
				    } else {
				        $notification[result.type ? result.type : 'error'](result.message);
				    }
				});

	    };

	    /**
        * @name activeModalLinked
        * @namespace FileController
        * @desc Ativa o modal com as provas vinculadas ao arquivo selecionado
        * @param {Object}
        * @memberOf Controller.FileController
        */
	    $scope.activeModalLinked = function _activeModalLinked(index, provas) {
	        $scope.vinculoIndex = index;
	        angular.element("#modalVinculos").modal({ backdrop: 'static' });
	    };

	    /**
        * @name cancel
        * @namespace FileController
        * @desc Cancela e dição de vinculos e retorna pra pagina anteior
        * @param {Object}
        * @memberOf Controller.FileController
        */
	    $scope.cancel = function _cancel() {
	        window.location.href = base_url("Test");
	    };

	    /**
        * @name removeSelectGlobal
        * @namespace FileController
        * @desc Desmarca o ckeck de vinculo global quando se desmarca um arquivo
        * @param {Object}
        * @memberOf Controller.FileController
        */
	    $scope.removeSelectGlobal = function _removeSelectGlobal(_file) {
	        $scope.selectedAll.CheckedAll = false;
	        var file = _file;

	        $timeout(function () {
	            var changed = false;
	            for (var i = 0; i < $scope.arquivosList.length; i++) {

	                if ($scope.arquivosList[i].Checked != $scope.copyListFile[i].Checked) {
	                    changed = true;
	                }//if 
	            }//for i

	            if (changed) {
	                $scope.saveLink = true;
	            } else {
	                $scope.saveLink = false;
	            }//else

	            if (!file.Checked) checkLitsToCompare();

	            if ($scope.ListSaveHistory.length != 0)
	                for (var k = 0; k < $scope.ListSaveHistory.length; k++) {
	                    if (file.Id == $scope.ListSaveHistory[k].Id
							&& !file.Checked && $scope.ListSaveHistory[k].Checked) {
	                        $scope.ListSaveHistory.splice(k, 1);
	                    }//if
	                }//for k

	        }, 0)
	    };

	    /**
        * @name listSelectedOptions
        * @namespace FileController
        * @desc Responsavel por controlar a lista de historico 
        * @param {Object} - Dados do arquivo selecionado
        * @memberOf Controller.FileController
        */
	    function listSelectedOptions() {

	        // verifica se a lista esta vazia
	        if ($scope.ListSaveHistory.length == 0) {
	            // preenchendo a lista de histórico
	            for (var i = 0; i < $scope.arquivosList.length; i++) {
	                //verifica se o arquivo foi selecionado
	                if ($scope.arquivosList[i].Checked)
	                    $scope.ListSaveHistory.push($scope.arquivosList[i]);
	            }//if
	        } else {
	            checkLitsToCompare();
	        }//else

	        // pega o valor da paginação atual
	        var id = $scope.paginate.currentPage();
	        // verifica se foi selecionado tds os arquivos da pagina 
	        for (var k = 0; k < $scope.checkList.length; k++) {
	            //compara a pagina atual com a salva na lista
	            //e altera a variavel para true
	            if ($scope.checkList[k] == id) {
	                $scope.selectedAll.CheckedAll = true;
	                return;
	            }//if
	        }//for

	    };

	    /**
        * @name checkLitsToCompare
        * @namespace FileController
        * @desc Compara os arquivo da lista atual da paginação com a lisyta de histórico
        * @param {Object} - Dados do arquivo selecionado
        * @memberOf Controller.FileController
        */
	    function checkLitsToCompare() {

	        var flag = false;

	        // camparação dos arquivos atuais com o da lista de histórico
	        for (var i = 0; i < $scope.arquivosList.length; i++) {
	            flag = false;
	            for (var j = 0; j < $scope.ListSaveHistory.length; j++) {
	                //pesquisa se o arquivo já existe na lista
	                if ($scope.arquivosList[i].Id == $scope.ListSaveHistory[j].Id) {
	                    flag = true;
	                    //break;
	                }//if
	            }//for j

	            //se o arquivo não foi encontrado na lista, se armazena o arquivos 
	            if (!flag) {
	                //verifica se o arquivo esta selecionado e se tiver o salva na lista
	                if ($scope.arquivosList[i].Checked)
	                    $scope.ListSaveHistory.push($scope.arquivosList[i]);
	            }//if

	        }//for i

	    };

	    /**
        * @name changeFile
        * @namespace FileController
        * @desc Armazena os dados do arquivo selecionado pelo usuário na lista de busca 
        * @param {Object} - Dados do arquivo selecionado
        * @memberOf Controller.FileController
        */
	    $scope.changeFile = function (selectedFile) {
	        $scope.selectedFile = selectedFile;
	    };

	    /**
        * @name selectedAllFiles
        * @namespace FileController
        * @desc Seleciona todos os arquivos em exibição na tela
        * @param {Object} - Dados do arquivo selecionado
        * @memberOf Controller.FileController
        */
	    $scope.selectedAllFiles = function _selectedAllFiles() {

	        $scope.saveLink = !$scope.selectedAll.CheckedAll ? true : false;
	        checkAllFiles($scope.selectedAll.CheckedAll);

	        if ($scope.ListSaveHistory.length != 0)
	            for (var j = 0; j < $scope.arquivosList.length; j++) {
	                for (var k = 0; k < $scope.ListSaveHistory.length; k++) {
	                    if ($scope.arquivosList[j].Id == $scope.ListSaveHistory[k].Id
							&& !$scope.arquivosList[j].Checked && $scope.ListSaveHistory[k].Checked) {
	                        $scope.ListSaveHistory.splice(k, 1);
	                    }//if
	                }//for k
	            }//for j

	        var id = $scope.paginate.currentPage() > 0 ? $scope.paginate.currentPage() - 1 : $scope.paginate.currentPage();
	        for (var i = 0; i < $scope.checkList.length; i++) {
	            if ($scope.checkList[i] == id) {
	                $scope.checkList.splice(i, 1);
	                return;
	            }
	        }
	        if (!$scope.selectedAll.CheckedAll)
	            $scope.checkList.push(id);


	    };

	    function checkAllFiles(check) {
	        for (var i = 0; i < $scope.arquivosList.length; i++) {
	            $scope.arquivosList[i].Checked = check;
	        }
	    };

	    /**
		 * @function Forçar a atualização (diggest) do angular
		 * @name safeApply
		 * @namespace FileController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
	    $scope.safeApply = function __safeApply() {
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

	    /**
         * @function Abrir/fechar painel de filtros
		 * @param
         * @returns
		 */
	    $scope.open = function __open() {

	        $('.side-filters').toggleClass('side-filters-animation').promise().done(function a() {

	            if (angular.element(".side-filters").hasClass("side-filters-animation")) {
	                angular.element('body').css('overflow', 'hidden');
	            }
	            else {
	                angular.element('body').css('overflow', 'inherit');
	            }
	        });
	    };

	    /**
         * @function Fechar painel de filtros por click da page
		 * @param
         * @returns
		 */
	    function close(e) {

	        if ($(e.target).parent().hasClass('.side-filters') || $(e.target).parent().hasClass('tag-item') || $(e.target).parent().hasClass('tags'))
	            return;

	        var element_in_painel = false;
	        if (($(e.target).hasClass('datepicker-switch') && e.target.tagName === "TH") ||
	            ($(e.target).hasClass('prev') && e.target.tagName === "TH") ||
                ($(e.target).hasClass('next') && e.target.tagName === "TH") ||
                ($(e.target).hasClass('dow') && e.target.tagName === "TH") ||
	            ($(e.target).hasClass('year') && e.target.tagName === "SPAN") ||
	            ($(e.target).hasClass('month') && e.target.tagName === "SPAN") ||
	            ($(e.target).hasClass('day') && e.target.tagName === "TD") ||
                $(e.target).hasClass('tag-item') || $(e.target).hasClass('tags') ||
	            $(e.target).parent().is("[data-side-filters]") ||
                e.target.hasAttribute('data-side-filters'))
	            return;

	        if (angular.element(".side-filters").hasClass("side-filters-animation")) $scope.open();
	    }; angular.element('body').click(close);

	    $scope.load();

	};

})(angular, jQuery);