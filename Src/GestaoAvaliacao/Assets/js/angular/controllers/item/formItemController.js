/**
 * function Cadastro/Edição item
 * @namespace Controller
 * @author Everton Ferreira - 21/10/2015
 * @author Julio Cesar da Silva - 03/03/2015
 */
(function (angular, $) {

    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives', 'ngTagsInput']);

    //~GETTER
    angular
        .module('appMain')
        .controller("FormItemController", FormItemController);

    FormItemController.$inject = ['$window', '$util', 'ItemModel', 'DisciplineModel', 'EvaluationMatrixModel', 'ItemLevelModel', 'ItemTypeModel', 'ItemSituationModel', 'SkillModel', 'EvaluationMatrixCourseCurriculumGradeModel', 'ParameterModel', 'SubjectModel', '$scope', '$rootScope', '$notification', '$timeout'];

    function FormItemController($window, $util, ItemModel, DisciplineModel, EvaluationMatrixModel, ItemLevelModel, ItemTypeModel, ItemSituationModel, SkillModel, EvaluationMatrixCourseCurriculumGradeModel, ParameterModel, SubjectModel, $scope, $rootScope, $notification, $timeout) {

        $scope.params = $util.getUrlParams();

        $scope.loaded = function _loaded() {

            $notification.clear();
            $scope.comommItemID = $scope.params.i;

            if ($scope.params.id) {
                $scope.editMode = true;
                $scope.createMode = false;
                $scope.itemEditID = $scope.params.id;
                $scope.itemloaded = {
                    etapa1: false,
                    etapa2: false,
                    etapa3: false
                };
                $scope.config();
            }
            else {
                $scope.createMode = true;
                $scope.editMode = false;
                $scope.config();
            }

            $(".comboAreaConhecimento").select2(
                {
                    multiple: false,
                    placeholder: "Selecione uma área de conhecimento",
                    width: '100%',
                    ajax: {
                        url: "loadallknowledgeareaactive",
                        dataType: 'json',
                        data: function (params, page) {
                            return {
                                description: params.term
                            };
                        },
                        processResults: function (data, page) {
                            return { results: data };
                        }
                    }
                })

            $(".comboComponenteCurricular").select2({
                placeholder: "Selecione um componente curricular"
            });

            $(".comboMatriz").select2({
                placeholder: "Selecione uma matriz"
            });

            loadSubject();
        };

        $scope.config = function __config() {

            $scope.currentJustificative;
            $scope.currentAlternative;
            $scope.editInternal = false;
            $scope.existeAssunto = false;
            $scope.currentEditItem = undefined;
            $scope.navigation = 1;
            $scope.version_visibility = false;
            $scope.timerstamp;
            $scope.loadingEditSkills = false;
            $scope.loadingEditSkillsInternal = false;
            $scope.tabAltTCT = 'alternativa';
            $scope.admin = getIsRestrict();
            $scope.valuePrev = null;
            $scope.skillsCopy = null;
            $scope.ShowItemNarrated = false;
            $scope.loaderSkillSerie = {
                skill: false,
                serie: false,
                dificuldade: false,
            };
            configInternalObjs();
        };

        function configInternalObjs() {

            $scope.listextensionsVideo = ['video/mp4', 'video/webm', 'video/ogg', 'application/ogg', 'video/x-flv', 'application/x-mpegURL', 'video/MP2T', 'video/3gpp', 'video/quicktime', 'video/x-msvideo', 'video/x-ms-wmv'];
            $scope.listextensionsAudio = ['audio/mpeg', 'audio/mp4', 'audio/mp3', 'audio/vnd.wav', 'audio/x-ms-wma', 'audio/ogg'];
            $scope.listextensionsImage = ['image/jpeg', 'image/png', 'image/gif', 'image/bmp'];
            $scope.EnumState = EnumState; // -> razor
            $scope.IsValidCode = undefined;

            $scope.configLabels = {
                initial_orientation: Parameters.Item.INITIAL_ORIENTATION.Value,
                basetext_orientation: Parameters.Item.BASETEXT_ORIENTATION.Value,
                initial_statement: Parameters.Item.INITIAL_STATEMENT.Value
            };
            $scope.skillsCopy = null;
            $scope.area = {
                masterLabel: "Área de conhecimento",
                lock: false,
                objArea: undefined,
                indexArea: undefined,
                lista: [],
                Id: undefined
            };
            $scope.materia = {
                masterLabel: "Componente curricular",
                lock: false,
                objMateria: undefined,
                indexMateria: undefined,
                lista: [],
                Id: undefined
            };
            $scope.item = {
                ItemNarrated: false,
                NarrationStudentStatement: false,
                StudentStatement: false,
                NarrationAlternatives: false,
                NarrationInitialStatement: false
            };
            $scope.modeloMatriz = {
                masterLabel: "Modelo matriz",
                Id: undefined,
                Description: undefined
            };
            $scope.matriz = {
                masterLabel: "Matriz",
                lock: false,
                objMatriz: undefined,
                indexMatriz: undefined,
                lista: [],
                Id: undefined
            };
            $scope.nivel = {
                masterLabel: "Nível de ensino",
                Id: undefined,
                Description: undefined,
                TypeLevelEducationId: undefined
            };
            $scope.series = {
                masterLabel: "Selecione o(s) " + Parameters.Item.ITEMCURRICULUMGRADE.Value + "(s)",
                selected: undefined,
                lista: []
            };
            $scope.curriculumGradeLabel = Parameters.Item.ITEMCURRICULUMGRADE.Value;
            $scope.textobase = {
                Id: 0,
                Description: "",
                History: undefined,
                Authorize: false,
                Source: "",
                Files: [],
                InitialOrientation: "",
                InitialStatement: "",
                BaseTextOrientation: "",
                StudentBaseText: false,
                NarrationStudentBaseText: false,
                NarrationInitialStatement: false
            };
            $scope.skills = [];
            $scope.statusItem = {
                masterLabel: "Situação do item",
                lock: false,
                objStatusItem: undefined,
                indexStatusItem: undefined,
                lista: []
            };
            $scope.tipoItem = {
                masterLabel: "Tipo do item",
                lock: true,
                objTipoItem: undefined,
                indexTipoItem: undefined,
                lista: []
            };
            $scope.sigiloItem = {
                masterLabel: "Sigilo do item",
                lock: true,
                value: false
            };
            $scope.dificuldade = {
                masterLabel: "Dificuldade sugerida",
                lock: false,
                objDificuldade: undefined,
                indexDificuldade: undefined,
                lista: []
            };
            $scope.assunto = {
                Id: undefined,
                Description: undefined,
                Subsubject_Id: undefined,
                Subsubject_Description: undefined
            };
            $scope.subassunto = {
                Id: undefined,
                Description: undefined
            };

            $scope.sentenca = "";
            $scope.proficiencia = "";
            angular.element('.proficiencia').on('input', function (event) {
                this.value = this.value.replace(/[^0-9]/g, '');
            });
            $scope.palavrasChave = [];
            $scope.dicas = "";
            $scope.enunciado = {
                Id: 0,
                Description: undefined,
                Files: []
            };
            var tct = [{ Id: 0, preDescription: "Discriminação", Value: undefined, postDescription: "-1 até 1" },
            { Id: 1, preDescription: "Proporção de acertos", Value: undefined, postDescription: "0 até 1" },
            { Id: 2, preDescription: "Coeficiente bisserial", Value: undefined, postDescription: "-1 até 1" }];
            $scope.tri = [{ Id: 0, preDescription: "Discriminação", Value: undefined, postDescription: "0 até 10" },
            { Id: 1, preDescription: "Dificuldade", Value: undefined, postDescription: " -99999.999 até 99999.999" },
            { Id: 2, preDescription: "Acerto casual", Value: undefined, postDescription: "0 até 1" }];
            $scope.distratores = [{ Id: 0, Description: "A)", selecionado: false, texto: "", tct: JSON.parse(JSON.stringify(tct)), justificativa: { Id: 0, Description: "", Files: [] }, ordem: 0, Files: [], State: 1 },
            { Id: 0, Description: "B)", selecionado: false, texto: "", tct: JSON.parse(JSON.stringify(tct)), justificativa: { Id: 0, Description: "", Files: [] }, ordem: 1, Files: [], State: 1 },
            { Id: 0, Description: "C)", selecionado: false, texto: "", tct: JSON.parse(JSON.stringify(tct)), justificativa: { Id: 0, Description: "", Files: [] }, ordem: 2, Files: [], State: 1 },
            { Id: 0, Description: "D)", selecionado: false, texto: "", tct: JSON.parse(JSON.stringify(tct)), justificativa: { Id: 0, Description: "", Files: [] }, ordem: 3, Files: [], State: 1 },
            { Id: 0, Description: "E)", selecionado: false, texto: "", tct: JSON.parse(JSON.stringify(tct)), justificativa: { Id: 0, Description: "", Files: [] }, ordem: 4, Files: [], State: 1 },
            { Id: 0, Description: "F)", selecionado: false, texto: "", tct: JSON.parse(JSON.stringify(tct)), justificativa: { Id: 0, Description: "", Files: [] }, ordem: 5, Files: [], State: 1 },
            { Id: 0, Description: "G)", selecionado: false, texto: "", tct: JSON.parse(JSON.stringify(tct)), justificativa: { Id: 0, Description: "", Files: [] }, ordem: 6, Files: [], State: 1 },
            { Id: 0, Description: "H)", selecionado: false, texto: "", tct: JSON.parse(JSON.stringify(tct)), justificativa: { Id: 0, Description: "", Files: [] }, ordem: 7, Files: [], State: 1 },
            { Id: 0, Description: "I)", selecionado: false, texto: "", tct: JSON.parse(JSON.stringify(tct)), justificativa: { Id: 0, Description: "", Files: [] }, ordem: 8, Files: [], State: 1 },
            { Id: 0, Description: "J)", selecionado: false, texto: "", tct: JSON.parse(JSON.stringify(tct)), justificativa: { Id: 0, Description: "", Files: [] }, ordem: 9, Files: [], State: 1 }
            ];
            $scope.code = undefined;
            $scope.version = 0;
            $scope.versions = {
                masterLabel: "",
                lista: []
            };
            $scope.itens = [];

            $scope.videos = [];
            $scope.convertedVideoThumbailToView = "";
            $scope.convertedVideoToView = "";

            $scope.audios = [];

            $scope.alterouMatriz = false;
            $scope.$watchCollection('itens', $scope.travarEtapa1);
            $scope.parameters = {};
            getParameters(2);
            getParameters(4);
        };


        /**
         * @function - Salva os elementos no servidor
         * @public
        * @params {integer} index Posição do elemento clicado.
        * @return {boolean} se elemento é o ultimo da listaModal
        */
        $scope.last = function (index) {

            return index === $scope.videos.length - 1;
        };

        /**
         * @function - Altera um subassunto do assunto.
         * @public
        * @params {integer} index Posição do elemento clicado.
        */
        $scope.altModal = function (index) {

            var q = $scope.videos[index];

            if (q.Path)
                q.lock = !q.lock;

            if (q.lock)
                aplicaCSS(q, 2);
            else
                aplicaCSS(q, 3);
        };

        /**
         * @function - Adiciona um novo subassunto para o assunto.
         * @public
         * @params
        */
        $scope.addModal = function () {

            //Tranca(lock) se não estiver 
            if ($scope.videos[$scope.ultimo])
                if (!$scope.videos[$scope.ultimo].lock)
                    $scope.altModal($scope.ultimo);

            var q = new Segmento();

            q.Level = $scope.videos.length;

            $scope.videos.push(q);

            $scope.ultimo = $scope.videos.length - 1;
        };

        /**
         * @function - Deleta um subassunto do assunto.
         * @public
         * @params
        */
        $scope.delModal = function () {

            angular.element("#modalVideos").modal('hide');

            var id = $scope.videos[$scope.itemDeletado].Id;

            var q = $scope.videos[$scope.itemDeletado];

            q.Description = undefined;
            q.Id = 0;
            q.lock = undefined;

            delete q.Description;
            delete q.Id;
            delete q.lock;

            q = null;
            $scope.videos[$scope.itemDeletado] = null;
            $scope.videos.splice($scope.itemDeletado, 1);

            $scope.itemDeletado = undefined;

            if ($scope.videos.length < 1)
                $scope.addModal();// inicia um elemento
            else
                $scope.ultimo = $scope.videos.length - 1;
        };

        /**
        * @function - Cria nova lista para edição
        * @public
        * @params
        */
        function newModal() {

            $scope.videos = [];

            if ($scope.videos.length < 1) {
                $scope.videos.push(new Segmento());
            }

            $scope.ultimo = $scope.videos.length - 1;
        };

        /**
         * @function - Salva os elementos no servidor
         * @public
        * @params {integer} index Posição do elemento clicado.
        * @return {boolean} se elemento é o ultimo da listaModal
        */
        $scope.lastAudio = function (index) {

            return index === $scope.audios.length - 1;
        };

        /**
         * @function - Altera um audio
         * @public
        * @params {integer} index Posição do elemento clicado.
        */
        $scope.altModalAudio = function (index) {

            var q = $scope.audios[index];

            if (q.Path)
                q.lock = !q.lock;

            if (q.lock)
                aplicaCSS(q, 2);
            else
                aplicaCSS(q, 3);
        };

        /**
         * @function - Adiciona um novo subassunto para o assunto.
         * @public
         * @params
        */
        $scope.addModalAudio = function () {

            //Tranca(lock) se não estiver 
            if ($scope.audios[$scope.ultimo])
                if (!$scope.audios[$scope.ultimo].lock)
                    $scope.altModalAudio($scope.ultimo);

            var q = new Segmento();

            q.Level = $scope.audios.length;

            $scope.audios.push(q);

            $scope.ultimo = $scope.audios.length - 1;
        };

        /**
         * @function - Deleta áudio
         * @public
         * @params
        */
        $scope.delModalAudio = function () {

            angular.element("#modalAudios").modal('hide');

            var id = $scope.audios[$scope.itemDeletado].Id;

            var q = $scope.audios[$scope.itemDeletado];

            q.Description = undefined;
            q.Id = 0;
            q.lock = undefined;

            delete q.Description;
            delete q.Id;
            delete q.lock;

            q = null;
            $scope.audios[$scope.itemDeletado] = null;
            $scope.audios.splice($scope.itemDeletado, 1);

            $scope.itemDeletado = undefined;


            if ($scope.audios.length < 1)
                $scope.addModalAudio();// inicia um elemento
            else
                $scope.ultimo = $scope.audios.length - 1;
        };

        /**
        * @function - Cria nova lista para edição
        * @public
        * @params
        */
        function newModalAudio() {

            $scope.audios = [];

            if ($scope.audios.length < 1) {
                $scope.audios.push(new Segmento());
            }

            $scope.ultimo = $scope.audios.length - 1;
        };



        /**
        * @function Object
        * @private
        * @param {Object} current
        */
        function Segmento(obj) {

            //File
            this.File = {
                Id: 0,
                Name: undefined,
                Path: undefined
            };
            //Thumbnail
            this.Thumbnail = {
                Id: 0,
                Name: undefined,
                Path: undefined
            };

            this.class = 'form-control';
        };

        /**
        * @function - configura o item a ser deletado
        * @public
        * @params {int} i
        */
        $scope.callModal = function (i) {

            $scope.itemDeletado = i;
            angular.element("#modalVideos").modal({ backdrop: 'static' });
        };

        /**
      * @function - configura o item a ser deletado
      * @public
      * @params {int} i
      */
        $scope.callModalAudio = function (i) {

            $scope.itemDeletado = i;
            angular.element("#modalAudios").modal({ backdrop: 'static' });
        };

        /**
          * @function - Chama a modal de vizualização do vídeo convertido
          * @public
          * @params {string} filePath, {string} thumbnailPath
        */
        $scope.callModalViewConvertedVideo = function (filePath, thumbnailPath) {

            if (filePath == "") return;

            $scope.convertedVideoThumbailToView = thumbnailPath;
            $scope.convertedVideoToView = filePath;
            angular.element("#modalViewConvertedVideo").modal({ backdrop: 'static' });
        };

        /**
          * @function - Fecha a modal de vizualização do vídeo convertido
          * @public
        */
        $scope.closeModalViewConvertedVideo = function () {

            let convertedVideoPlayer = document.getElementById("convertedVideoPlayer");
            convertedVideoPlayer.pause();

            $scope.convertedVideoThumbailToView = "";
            $scope.convertedVideoToView = "";
        };

        /**
        * @function - Aplica CSS
        * @private
        * @params
        */
        function aplicaCSS(obj, t) {
            if (t === 1) {
                obj.class = $scope.iWrong;
            }
            else if (t === 2) {
                obj.class = $scope.iLock;
            }
            else {
                obj.class = $scope.iNormal;
            }
        };

        function getParameters(id) {

            ParameterModel.getParameters({ Id: id }, function (result) {
                if (result.success) {
                    try {
                        var categories = result.lista;
                        for (var i = 0, len = categories.length; i < len; i++) {

                            for (var j = 0, leng = categories[i].Parameters.length; j < leng; j++) {

                                $scope.parameters[categories[i].Parameters[j].Key] = categories[i].Parameters[j];
                            }
                        }

                        if (id != 4) {
                            if ($scope.editMode) {
                                carregarEditarItem();
                            }
                            else if ($scope.createMode && $scope.comommItemID != undefined) {
                                carregarTextoBase();
                            }
                            else if ($scope.createMode) {
                                carregarDisciplinas();
                            }
                        } else {
                            if ($scope.parameters.SHOW_ITEM_NARRATED.Value == "True")
                                $scope.showItemNarrated = true;
                            else $scope.showItemNarrated = false;
                        }
                    }
                    catch (e) {
                        $notification.error("Erro no retorno dos parâmetros da página de cadastro de itens.")
                    }
                }
                else {
                    $notification.error("Não foi possível carregar os parâmetros.")
                }
            });
        };

        function adminCarregamento(_navigation) {

            switch (_navigation) {

                case 1: break;
                case 2:
                    if ($scope.editMode) {
                        if ($scope.itemloaded.etapa2 == false) {
                            carregarEditarItemEtapa2();
                        }
                    }
                    break;
                case 3:
                    if ($scope.dificuldade.lista.length == 0) {
                        carregarDificuldade();
                    }
                    if ($scope.statusItem.lista.length == 0) {
                        carregarSituacaoItem();
                    }
                    if ($scope.tipoItem.lista.length == 0) {
                        carregarTipoItem();
                    }
                    if ($scope.alterouMatriz || $scope.skills.length < 1) {
                        $scope.alterouMatriz = false;
                        $scope.carregarSkills($scope.trigger.id);
                        carregarSeries($scope.trigger.id);
                    }
                    $scope.trigger.change = true;
                    break;
            }

            $('html, body').animate({ scrollTop: '0px' }, 300);
        };

        function carregarTextoBase() {

            if ($scope.comommItemID != undefined) {

                ItemModel.getAddItemInfos({ itemId: $scope.comommItemID }, function (result) {
                    if (result.success) {

                        $scope.textobase.Id = result.lista.BaseText.Id;
                        $scope.textobase.Description = result.lista.BaseText.Description;
                        $scope.textobase.Source = result.lista.BaseText.Source != null ? result.lista.BaseText.Source : undefined;
                        $scope.textobase.Files = result.lista.BaseText.Files;
                        $scope.textobase.History = $scope.textobase.Description;
                        $scope.comommTextBase = {
                            EvaluationMatrix: result.lista.EvaluationMatrix.Id,
                            Discipline: result.lista.EvaluationMatrix.Discipline.Id
                        };
                        carregarDisciplinas();
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
        };

        function carregarEditarItem() {

            ItemModel.getMatrixByItem({ itemId: $scope.itemEditID }, function (result) {
                if (result.success) {

                    setParamEditarItemEtapa1(result.lista);

                    carregarDisciplinas();

                    return;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        function carregarDisciplinas() {

            DisciplineModel.loadWithMatrix(function (result) {
                if (result.success) {
                    if ($scope.editMode) {
                        $scope.materia.lista = result.lista;
                        for (var i = 0; i < result.lista.length; i++) {
                            if (result.lista[i].Id == $scope.materia.objMateria.Id) {
                                $scope.materia.objMateria = result.lista[i];
                                break;
                            }
                        }
                    }
                    else {
                        $scope.materia.lista = result.lista;
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        function carregarDificuldade() {

            ItemLevelModel.load(function (result) {

                if (result.success) {

                    $scope.dificuldade.lista = result.lista;

                    $scope.loaderSkillSerie.dificuldade = true;

                    if ($scope.editMode) {
                        if ($scope.itemloaded.etapa3 == false) {
                            if ($scope.loaderSkillSerie.skill == true && $scope.loaderSkillSerie.serie == true && $scope.loaderSkillSerie.dificuldade == true) {
                                carregarEditarItemEtapa3();
                            }
                        }
                    }

                    return;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        function carregarSituacaoItem() {

            ItemSituationModel.load(function (result) {

                if (result.success) {

                    $scope.statusItem.lista = result.lista;

                    return;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        function carregarTipoItem() {

            ItemTypeModel.load(function (result) {

                if (result.success) {

                    $scope.tipoItem.lista = result.lista;
                    try {
                        for (var i = 0; i < $scope.tipoItem.lista.length; i++) {
                            if ($scope.tipoItem.lista[i].IsDefault) {
                                $scope.tipoItem.objTipoItem = $scope.tipoItem.lista[i];
                                $scope.valuePrev = $scope.tipoItem.objTipoItem;
                                $scope.itemTypeQtd = $scope.tipoItem.objTipoItem;


                                for (var d = 0; d < $scope.distratores.length; d++) {
                                    if (d < $scope.tipoItem.objTipoItem.QuantityAlternative) {
                                        $scope.distratores[d].State = 1;
                                    } else {
                                        $scope.distratores[d].State = 3;
                                    }
                                }

                                break;
                            }
                        }
                    } catch (error) { }

                    return;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        function carregarSeries(_idMatriz) {

            if (!_idMatriz) return;

            EvaluationMatrixCourseCurriculumGradeModel.getCurriculumGradesByMatrix({ evaluationMatrixId: _idMatriz }, function (result) {

                if (result.success) {

                    var _list = [];
                    for (var key in result.lista) {
                        _list.push({
                            Id: result.lista[key].TypeCurriculumGrade.Id,
                            Description: result.lista[key].TypeCurriculumGrade.Description
                        });
                    }

                    $scope.series.lista = _list;

                    try { $scope.series.selected = $scope.series.lista[0]; } catch (error) { }

                    $scope.loaderSkillSerie.serie = true;

                    if ($scope.editMode) {
                        if ($scope.itemloaded.etapa3 == false) {
                            if ($scope.loaderSkillSerie.skill == true && $scope.loaderSkillSerie.serie == true && $scope.loaderSkillSerie.dificuldade == true) {
                                carregarEditarItemEtapa3();
                            }
                        }
                    }

                    return;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        habilitarComboComponenteCurricular();
        habilitarComboMatriz();
        habilitarComboAssunto();

        $('.comboComponenteCurricular').on("select2:select", function (e) {
            carregarDefaultEtapa1();
            $(".comboMatriz").empty().trigger('change');
            habilitarComboMatriz();
        });

        $('.comboAreaConhecimento').on("select2:select", function (e) {
            $(".comboComponenteCurricular").empty().trigger('change');
            habilitarComboComponenteCurricular();
        });

        $('.comboAssunto').on("select2:select", function (e) {
            $(".comboSubassunto").empty().trigger('change');
            habilitarComboAssunto();
        });

        $scope.carregaComponenteCurricular = function __carregaComponenteCurricular() {
            $(".comboComponenteCurricular").select2(
                {
                    placeholder: "Selecione um componente curricular",
                    width: '100%',
                    ajax: {
                        url: "loaddisciplinebyknowledgearea",
                        dataType: 'json',
                        data: function (params, page) {
                            return {
                                description: params.term,
                                knowledgeAreas: ($('.comboAreaConhecimento').val()).toString()
                            };
                        },
                        processResults: function (data, page) {
                            return { results: data };
                        }
                    }
                });
        };

        function habilitarComboComponenteCurricular() {

            var areaConhecimento = $('.comboAreaConhecimento').val();

            if (areaConhecimento === null || areaConhecimento === "") {
                $(".comboComponenteCurricular").prop("disabled", true);
            }
            else {
                $(".comboComponenteCurricular").prop("disabled", false);
            }
        }

        function habilitarComboMatriz() {

            var componente = $('.comboComponenteCurricular').val();

            if (componente === null || componente === "") {
                $(".comboMatriz").prop("disabled", true);
            }
            else {
                $(".comboMatriz").prop("disabled", false);
            }
        }

        function habilitarComboAssunto() {

            var assunto = $('.comboAssunto').val();

            if (assunto === null || assunto === "") {
                $(".comboSubassunto").prop("disabled", true);
            }
            else {
                $(".comboSubassunto").prop("disabled", false);
            }
        }

        function carregarDefaultEtapa1() {

            try {
                $scope.materia.objMateria = ($.grep($scope.materia.lista, function (e) { return e.Id == $scope.materia.Id; }))[0];

                EvaluationMatrixModel.loadByMatriz({ Id: $scope.materia.Id }, function (result) {
                    if (result.success) {
                        if ($scope.materia != undefined) {
                            $scope.matriz.lista = result.lista;
                            $scope.matriz.objMatriz = result.lista[0];
                        }
                        if ($scope.createMode && !$scope.params.i) {

                            if ($scope.comommItemID != undefined) {

                                for (var i = 0; i < $scope.matriz.lista.length; i++) {

                                    if ($scope.matriz.lista[i].Id == $scope.comommTextBase.EvaluationMatrix) {
                                        $scope.matriz.objMatriz = $scope.matriz.lista[i];
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                        $scope.nivel.masterLabel = "";
                    }
                });
            }
            catch (error) {
                $notification.error("Não há nenhuma matriz cadastrada!");
            }
        };

        function carregarEditarItemEtapa2() {

            ItemModel.getBaseTextByItem({ itemId: $scope.itemEditID }, function (result) {

                if (result.success) {

                    $scope.textobase.InitialOrientation = result.InitialOrientation;
                    $scope.textobase.InitialStatement = result.InitialStatement;
                    $scope.textobase.BaseTextOrientation = result.BaseTextOrientation;
                    $scope.textobase.StudentBaseText = result.StudentBaseText;
                    $scope.textobase.NarrationStudentBaseText = result.NarrationStudentBaseText;
                    $scope.textobase.NarrationInitialStatement = result.NarrationInitialStatement;
                    $scope.textobase.Source = result.Source;

                    if (result.lista != undefined)
                        setParamEditarItemEtapa2(result.lista);

                    return;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        function carregarEditarItemEtapa3() {

            ItemModel.getItemById({ itemId: $scope.itemEditID }, function (result) {

                if (result.success) {

                    setParamEditarItemEtapa3(result.lista);

                    return;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        function setParamEditarItemEtapa1(_result) {

            $scope.itemloaded.etapa1 = true;
            $scope.materia.objMateria = { Id: _result.Id, Description: _result.Description, LevelEducationDescription: _result.TypeLevelEducation.Description };
            $(".comboComponenteCurricular").prop("disabled", true);
            $scope.modeloMatriz.Id = _result.EvaluationMatrix.ModelEvaluationMatrix.Id;
            $scope.modeloMatriz.Description = _result.EvaluationMatrix.ModelEvaluationMatrix.Description;
            $scope.matriz.objMatriz = { Id: _result.EvaluationMatrix.Id, Description: _result.EvaluationMatrix.Description };
            $(".comboMatriz").prop("disabled", true);
            $scope.matriz.lista = [$scope.matriz.objMatriz];
            $scope.nivel.Id = _result.TypeLevelEducation.Id;
            $scope.nivel.Description = _result.TypeLevelEducation.Description;
            $scope.nivel.TypeLevelEducationId = _result.TypeLevelEducation.TypeLevelEducationId;
            $scope.item.ItemNarrated = _result.ItemNarrated;
            $scope.area.objArea = { Id: _result.KnowledgeArea.Id, Description: _result.KnowledgeArea.Description };


            $scope.trigger = {
                change: true,
                id: $scope.matriz.objMatriz.Id
            };
        };

        function setParamEditarItemEtapa2(_result) {

            $scope.itemloaded.etapa2 = true;
            $scope.textobase = JSON.parse(JSON.stringify(_result));
            $scope.textobase.History = $scope.textobase.Description;
            $scope.textobase.Authorize = false;
        };

        function setParamEditarItemEtapa3(_result) {

            if (_result.Subsubject != null) {

                SubjectModel.loadSubjectBySubsubject({ idSubsubject: _result.Subsubject.Id }, function (result) {
                    if (result.success) {
                        $scope.assunto = result.assunto;
                        $scope.existeAssunto = true;

                        $scope.itemloaded.etapa3 = true;
                        $scope.loadingEditSkills = true;
                        $scope.queue = {
                            data: _result.ItemSkills,
                            currentLevel: 0
                        };
                        loadListSkillsEdit();
                        loadSubject();
                        $scope.sentenca = _result.descriptorSentence != null ? _result.descriptorSentence : undefined;
                        $scope.proficiencia = _result.proficiency != null ? _result.proficiency : undefined;
                        $scope.enunciado.Description = _result.Statement != null ? (_result.Statement.Description != null ? _result.Statement.Description : undefined) : undefined;
                        $scope.enunciado.Files = _result.Statement != null ? $scope.copy(_result.Statement.Files) : undefined;
                        $scope.palavrasChave = _result.Keywords != null ? $scope.transformTags(_result.Keywords) : undefined;
                        $scope.dicas = _result.Tips != null ? _result.Tips : undefined;
                        $scope.videos = _result.Videos;

                        if ($scope.videos.length <= 0) {
                            newModal();
                        }

                        $scope.audios = _result.Audios;

                        if ($scope.audios.length <= 0) {
                            newModalAudio();
                        }

                        if (_result.ItemType != null) {
                            var arr = jQuery.grep($scope.tipoItem.lista, function (n, i) {
                                return (n.Id == _result.ItemType.Id);
                            });

                            $scope.tipoItem.objTipoItem = arr[0];
                            $scope.itemTypeQtd = $scope.tipoItem.objTipoItem;

                        }
                        if (_result.ItemSituation != null) {
                            var arr = jQuery.grep($scope.statusItem.lista, function (n, i) {
                                return (n.Id == _result.ItemSituation.Id);
                            });
                            $scope.statusItem.objStatusItem = arr[0];
                        }

                        $scope.item.NarrationStudentStatement = _result.NarrationStudentStatement
                        $scope.item.StudentStatement = _result.StudentStatement;
                        $scope.item.NarrationAlternatives = _result.NarrationAlternatives;
                        $scope.dificuldade.objDificuldade = _result.ItemLevel != null ? _result.ItemLevel : undefined;
                        $scope.series.selected = _result.ItemCurriculumGrades != null ? _result.ItemCurriculumGrades[0] : undefined;
                        $scope.versions.lista = [];
                        $scope.versions.lista = _result.Versions;
                        $scope.code = _result.ItemCode;
                        $scope.ItemCodeVersion = _result.ItemCodeVersion;
                        $scope.version = _result.ItemVersion;
                        $scope.sigiloItem.value = _result.IsRestrict != null ? _result.IsRestrict : false;
                        $scope.tri[0].Value = _result.TRIDiscrimination != null ? _result.TRIDiscrimination.toString() : undefined;
                        $scope.tri[1].Value = _result.TRIDifficulty != null ? _result.TRIDifficulty.toString() : undefined;
                        $scope.tri[2].Value = _result.TRICasualSetting != null ? _result.TRICasualSetting.toString() : undefined;
                        $scope.subassunto = { Id: _result.Subsubject.Id, Description: _result.Subsubject.Description };

                        for (var key = 0; key < $scope.distratores.length; key++) {

                            if (_result.Alternatives[key] != undefined) {

                                $scope.distratores[key].Id = _result.Alternatives[key].Id;
                                $scope.distratores[key].selecionado = _result.Alternatives[key].Correct;
                                $scope.distratores[key].texto = _result.Alternatives[key].Description;
                                $scope.distratores[key].Files = $scope.copy(_result.Alternatives[key].Files);
                                $scope.distratores[key].justificativa.Description = _result.Alternatives[key].Justificative.Description;
                                $scope.distratores[key].justificativa.Files = $scope.copy(_result.Alternatives[key].Justificative.Files);
                                $scope.distratores[key].ordem = key;
                                $scope.distratores[key].State = _result.Alternatives[key].State;
                                $scope.distratores[key].tct = [{ Id: 0, preDescription: "Discriminação", Value: _result.Alternatives[key].TCTDiscrimination, postDescription: "-1 até 1" },
                                { Id: 1, preDescription: "Proporção de acertos", Value: _result.Alternatives[key].TCTDificulty, postDescription: "0 até 1" },
                                { Id: 2, preDescription: "Coeficiente bisserial", Value: _result.Alternatives[key].TCTBiserialCoefficient, postDescription: "-1 até 1" }];

                            } else if ($scope.tipoItem.objTipoItem.QuantityAlternative == 4) {
                                $scope.distratores[key].State = 3;
                                $scope.typeItemQtd = 1;
                            }
                        }
                    }
                });
            }
            else {
                $scope.existeAssunto = false;
                $scope.itemloaded.etapa3 = true;
                $scope.loadingEditSkills = true;
                $scope.queue = {
                    data: _result.ItemSkills,
                    currentLevel: 0
                };
                loadListSkillsEdit();
                loadSubject();
                $scope.sentenca = _result.descriptorSentence != null ? _result.descriptorSentence : undefined;
                $scope.proficiencia = _result.proficiency != null ? _result.proficiency : undefined;
                $scope.enunciado.Description = _result.Statement != null ? (_result.Statement.Description != null ? _result.Statement.Description : undefined) : undefined;
                $scope.enunciado.Files = _result.Statement != null ? $scope.copy(_result.Statement.Files) : undefined;
                $scope.palavrasChave = _result.Keywords != null ? $scope.transformTags(_result.Keywords) : undefined;
                $scope.dicas = _result.Tips != null ? _result.Tips : undefined;
                $scope.videos = _result.Videos;

                if ($scope.videos.length <= 0) {
                    newModal();
                }

                $scope.audios = _result.Audios;

                if ($scope.audios.length <= 0) {
                    newModalAudio();
                }

                if (_result.ItemType != null) {
                    var arr = jQuery.grep($scope.tipoItem.lista, function (n, i) {
                        return (n.Id == _result.ItemType.Id);
                    });

                    $scope.tipoItem.objTipoItem = arr[0];
                    $scope.itemTypeQtd = $scope.tipoItem.objTipoItem;

                }
                if (_result.ItemSituation != null) {
                    var arr = jQuery.grep($scope.statusItem.lista, function (n, i) {
                        return (n.Id == _result.ItemSituation.Id);
                    });
                    $scope.statusItem.objStatusItem = arr[0];
                }

                $scope.item.NarrationStudentStatement = _result.NarrationStudentStatement
                $scope.item.StudentStatement = _result.StudentStatement;
                $scope.item.NarrationAlternatives = _result.NarrationAlternatives;
                $scope.dificuldade.objDificuldade = _result.ItemLevel != null ? _result.ItemLevel : undefined;
                $scope.series.selected = _result.ItemCurriculumGrades != null ? _result.ItemCurriculumGrades[0] : undefined;
                $scope.versions.lista = [];
                $scope.versions.lista = _result.Versions;
                $scope.code = _result.ItemCode;
                $scope.ItemCodeVersion = _result.ItemCodeVersion;
                $scope.version = _result.ItemVersion;
                $scope.sigiloItem.value = _result.IsRestrict != null ? _result.IsRestrict : false;
                $scope.tri[0].Value = _result.TRIDiscrimination != null ? _result.TRIDiscrimination.toString() : undefined;
                $scope.tri[1].Value = _result.TRIDifficulty != null ? _result.TRIDifficulty.toString() : undefined;
                $scope.tri[2].Value = _result.TRICasualSetting != null ? _result.TRICasualSetting.toString() : undefined;

                for (var key = 0; key < $scope.distratores.length; key++) {

                    if (_result.Alternatives[key] != undefined) {

                        $scope.distratores[key].Id = _result.Alternatives[key].Id;
                        $scope.distratores[key].selecionado = _result.Alternatives[key].Correct;
                        $scope.distratores[key].texto = _result.Alternatives[key].Description;
                        $scope.distratores[key].Files = $scope.copy(_result.Alternatives[key].Files);
                        $scope.distratores[key].justificativa.Description = _result.Alternatives[key].Justificative.Description;
                        $scope.distratores[key].justificativa.Files = $scope.copy(_result.Alternatives[key].Justificative.Files);
                        $scope.distratores[key].ordem = key;
                        $scope.distratores[key].State = _result.Alternatives[key].State;
                        $scope.distratores[key].tct = [{ Id: 0, preDescription: "Discriminação", Value: _result.Alternatives[key].TCTDiscrimination, postDescription: "-1 até 1" },
                        { Id: 1, preDescription: "Proporção de acertos", Value: _result.Alternatives[key].TCTDificulty, postDescription: "0 até 1" },
                        { Id: 2, preDescription: "Coeficiente bisserial", Value: _result.Alternatives[key].TCTBiserialCoefficient, postDescription: "-1 até 1" }];

                    } else if ($scope.tipoItem.objTipoItem.QuantityAlternative == 4) {
                        $scope.distratores[key].State = 3;
                        $scope.typeItemQtd = 1;
                    }
                }
            }
        };

        function loadSubject() {
            $(".comboAssunto").select2(
                {
                    placeholder: "Selecione um assunto",
                    width: '100%',
                    overflow: scroll,
                    ajax: {
                        url: "loadallsubjects",
                        dataType: 'json',
                        data: function (params, page) {
                            return {
                                description: params.term
                            };
                        },
                        processResults: function (data, page) {
                            return {
                                results: data
                            };
                        }
                    }
                });

            $(".comboSubassunto").select2({
                placeholder: "Selecione um subassunto"
            });
        }

        $scope.carregaSubassunto = function __carregaSubassunto() {
            $(".comboSubassunto").select2(
                {
                    placeholder: "Selecione um subassunto",
                    width: '100%',
                    overflow: scroll,
                    ajax: {
                        url: "loadsubsubjectbysubject",
                        dataType: 'json',
                        data: function (params, page) {
                            return {
                                description: params.term,
                                subjects: ($('.comboAssunto').val()).toString()
                            };
                        },
                        processResults: function (data, page) {
                            return {
                                results: data
                            };
                        }
                    }
                });
        };


        function loadListSkillsEdit(parentId) {

            if (parentId == undefined) {
                var skill;
                var arr = jQuery.grep($scope.skills[$scope.queue.currentLevel].lista, function (n, i) {
                    return (n.Id == $scope.queue.data[$scope.queue.currentLevel].Skill.Id);
                });
                skill = arr[0];

                $scope.skills[$scope.queue.currentLevel].objSkill = skill;
                $scope.queue.currentLevel += 1;

                if ($scope.queue.currentLevel < $scope.queue.data.length) {
                    loadListSkillsEdit($scope.queue.data[$scope.queue.currentLevel].Skill.Parent);
                }
                else {
                    $scope.loadingEditSkills = false;
                }
            }
            else {

                SkillModel.getByParent({
                    Id: parentId
                }, function (result) {

                    if (result.success) {

                        var list = [];
                        for (var i = 0; i < result.lista.length; i++) {
                            list.push({
                                Description: result.lista[i].Skills.Description,
                                Id: result.lista[i].Skills.Id,
                                LastLevel: result.lista[i].Skills.LastLevel,
                                ParentId: parentId != undefined ? parentId : 0
                            })
                        }

                        $scope.skills[$scope.queue.currentLevel].lista = list;

                        var skill;
                        var arr = jQuery.grep($scope.skills[$scope.queue.currentLevel].lista, function (n, i) {
                            return (n.Id == $scope.queue.data[$scope.queue.currentLevel].Skill.Id);
                        });
                        skill = arr[0];

                        $scope.skills[$scope.queue.currentLevel].objSkill = skill;
                        $scope.queue.currentLevel += 1;
                        $scope.skillsCopy = angular.copy($scope.skills);
                        if ($scope.queue.currentLevel < $scope.queue.data.length) {
                            loadListSkillsEdit($scope.queue.data[$scope.queue.currentLevel].Skill.Parent)
                        }
                        else {
                            $scope.loadingEditSkills = false;
                        }
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
        };

        $scope.relacaoDisciplina = function __relacaoDisciplina() {

            $scope.modeloMatriz.id = undefined;
            $scope.modeloMatriz.Description = "";
            $scope.matriz.objMatriz = undefined;
            $scope.matriz.lista = [];
            $scope.nivel.Id = undefined;
            $scope.nivel.Description = undefined;
            $scope.nivel.TypeLevelEducationId = undefined;

            if ($scope.materia.Id === undefined) return;

            if ($scope.createMode) {

                $(".comboMatriz").select2(
                    {
                        placeholder: "Selecione uma matriz",
                        width: '100%',
                        ajax: {
                            url: "loadmatrizbydiscipline",
                            dataType: 'json',
                            data: function (params, page) {
                                return {
                                    description: params.term,
                                    discipline: ($scope.materia.Id).toString()
                                };
                            },
                            processResults: function (data, page) {
                                return {
                                    results: data
                                };
                            }
                        }
                    });

                //EvaluationMatrixModel.load({ Id: $scope.materia.objMateria.Id }, function (result) {
                //    if (result.success) {
                //        if ($scope.materia.objMateria != undefined)
                //            carregarDefaultEtapa1(result.lista);
                //    }
                //    else {
                //        $notification[result.type ? result.type : 'error'](result.message);
                //        $scope.nivel.masterLabel = "";
                //    }
                //});
            }
        };

        $scope.setParamDefaultEtapa1 = function __setParamDefaultEtapa1() {
            $scope.modeloMatriz.Description = undefined;
            $scope.nivel.Id = undefined;
            $scope.nivel.Description = undefined;
            $scope.nivel.TypeLevelEducationId = undefined;

            if ($scope.matriz.objMatriz === undefined) return;

            $scope.alterouMatriz = true;
            $scope.skills = null;

            if ($scope.createMode) {

                $scope.modeloMatriz.Id = $scope.matriz.objMatriz.ModelEvaluationMatrix.Id;
                $scope.modeloMatriz.Description = $scope.matriz.objMatriz.ModelEvaluationMatrix.Description;
                $scope.nivel.Id = $scope.matriz.objMatriz.Discipline.TypeLevelEducation.Id;
                $scope.nivel.Description = $scope.matriz.objMatriz.Discipline.TypeLevelEducation.Description;
                $scope.nivel.TypeLevelEducationId = $scope.matriz.objMatriz.Discipline.TypeLevelEducation.TypeLevelEducationId;
                $scope.matriz.objMatriz.Id = $scope.matriz.Id;
            }
            $scope.trigger = {
                change: true,
                id: $scope.matriz.objMatriz.Id
            };
        };

        $scope.carregarSkills = function __carregarSkills() {

            if ($scope.matriz.objMatriz.Id !== undefined) {

                carregarSeries();
                $scope.skills = [];
                SkillModel.getByMatriz({
                    Id: $scope.matriz.objMatriz.Id
                },
                    function (result) {

                        if (result.success) {
                            var formatedList = [];
                            for (var key in result.lista) {

                                formatedList.push({
                                    Id: result.lista[key].ModelSkillLevels.Id,
                                    Description: result.lista[key].ModelSkillLevels.Description,
                                    objSkill: {
                                        Id: undefined, Description: '--Selecione--'
                                    },
                                    show: false,
                                    lista: $scope.getSubstractSkill(result.lista[key].ModelSkillLevels.Skills)
                                });
                            }
                            $scope.skills = formatedList;
                            $scope.loaderSkillSerie.skill = true;

                            if ($scope.editMode) {
                                if ($scope.itemloaded.etapa3 == false) {
                                    if ($scope.loaderSkillSerie.skill == true && $scope.loaderSkillSerie.serie == true && $scope.loaderSkillSerie.dificuldade == true) {
                                        carregarEditarItemEtapa3();
                                    }
                                }
                            }
                        }
                        else {
                            $notification[result.type ? result.type : 'error'](result.message);
                        }
                    });
            }
        };

        $scope.carregarSkill = function __carregarSkill($indexSkill) {

            $(".comboAssunto_" + $indexSkill).select2(
                {
                    placeholder: "Selecione uma informação",
                    width: '100%',
                    ajax: {
                        url: "loadallknowledgeareaactive",
                        dataType: 'json',
                        data: function (params, page) {
                            return {
                                description: params.term
                            };
                        },
                        processResults: function (data, page) {
                            return {
                                results: data
                            };
                        }
                    }
                }
            );
        };

        $scope.carregarCascadeSkill = function __carregarCascadeSkill($indexSkill, node) {

            for (var a = 0; a < $scope.skills.length; a++) {
                if (a > $indexSkill) {
                    $scope.skills[a].lista = [{
                        Id: undefined, Description: "--Selecione--"
                    }];
                    $scope.skills[a].objSkill = {
                        Id: undefined, Description: "--Selecione--"
                    };
                }
            }

            var _nextIndex = $indexSkill + 1;

            if (_nextIndex < $scope.skills.length && node.objSkill.Id > 0) {

                SkillModel.getByParent({
                    Id: node.objSkill.Id
                },
                    function (result) {
                        if (result.success) {

                            var _skills = [];
                            for (var key in result.lista)
                                _skills.push(result.lista[key].Skills);
                            $scope.skills[_nextIndex].lista = $scope.getSubstractSkill(_skills);
                        }
                        else {
                            $notification[result.type ? result.type : 'error'](result.message);
                        }
                    });
            }
        };

        $scope.getSubstractSkill = function __getSubstractSkill(_lista) {

            var substracts = [];

            for (var i = 0; i < _lista.length; i++) {
                if (_lista[i].Description.length > 20) {
                    substracts.push({
                        Id: _lista[i].Id,
                        Description: _lista[i].Description,
                        Substract: _lista[i].Description.substring(0, 20) + "...",
                        LastLevel: _lista[i].LastLevel
                    });
                }
                else {
                    substracts.push({
                        Id: _lista[i].Id,
                        Description: _lista[i].Description,
                        Substract: _lista[i].Description,
                        LastLevel: _lista[i].LastLevel
                    });
                }
            }

            substracts.unshift({
                Id: undefined, Description: '--Selecione--'
            });

            return substracts;
        };

        $scope.reloadPage = function __reloadPage(url) {
            $window.location.href = '/Item/Form';
        };

        $scope.addQtdAlternativas = function __addQtdAlternativas(qtd) {
            var codigo = 64;
            var letra;

            for (var i = 0; i < qtd; i++) {
                codigo++;
                letra = String.fromCharCode(codigo);
                $scope.distratores.push({
                    Id: 0, Description: letra + ")", selecionado: false, texto: "", tct: JSON.parse(JSON.stringify(tct)), justificativa: { Id: 0, Description: "", Files: [] }, ordem: i, Files: [], State: 1
                })
            }
        };

        $scope.removeQtdAlternativas = function __removeQtdAlternativas(qtd) {

            while (qtd < $scope.distratores.length) {
                $scope.distratores.pop();
            }
        };

        $scope.changeItemType = function __changeItemType(opc) {

            if (opc != null) {
                for (var d = 0; d < $scope.distratores.length; d++) {
                    if (d < opc.QuantityAlternative) {
                        $scope.distratores[d].State = 1;
                    } else {
                        $scope.distratores[d].State = 3;
                    }
                }

                if (opc.QuantityAlternative != $scope.itemTypeQtd.QuantityAlternative) {
                    angular.element('#ConfirmModal').modal('show');
                }


                //if (opc.QuantityAlternative == 5) {
                //    $scope.distratores.map(a => a.State = 1);
                //    $scope.distratores[$scope.distratores.length - 1].State = 1;
                //    $scope.valuePrev = $scope.tipoItem.objTipoItem;
                //    $scope.typeItemQtd = 0;
                //}
                //else if (opc.QuantityAlternative == 0) {
                //    $scope.distratores.map(a => a.State = 3);

                //    $scope.typeItemQtd = 2;
                //}
                //else if (opc.QuantityAlternative == 4) {
                //    $scope.distratores.map(a => a.State = 1);
                //    $scope.distratores[$scope.distratores.length - 1].State = 3;
                //    $scope.typeItemQtd = 1;

                //    if (opc.QuantityAlternative != $scope.itemTypeQtd.QuantityAlternative && $scope.tipoItem.objTipoItem.Id != 3) {
                //        angular.element('#ConfirmModal').modal('show');
                //    }
                //}

                //$scope.itemTypeQtd = opc
            }
        };

        $scope.newValueItemType = function __newValueItemType() {   
            $scope.valuePrev = $scope.itemTypeQtd;
            $scope.itemTypeQtd = $scope.tipoItem.objTipoItem;
        };

        $scope.setValueItemTypeDefault = function __setValueItemTypeDefault() {

            $scope.tipoItem.objTipoItem = $scope.itemTypeQtd;

            for (var d = 0; d < $scope.distratores.length; d++) {
                if (d < $scope.tipoItem.objTipoItem.QuantityAlternative) {
                    $scope.distratores[d].State = 1;
                } else {
                    $scope.distratores[d].State = 3;
                }
            }          
        };

        $scope.activeModalText = function __activeModalText(label, text) {

            $scope.textSelected = {
                Description: label,
                TextDescription: text.Description
            };
            angular.element("#modalTextSelect").modal({
                backdrop: 'static'
            });
        };

        $scope.transformTags = function __transformTags(_tags) {

            var arrTags = [];
            var arrSplited = _tags.split(";")
            for (var key in arrSplited) {
                arrSplited[key] = {
                    text: arrSplited[key].replace(";", "")
                };
                arrTags.push(arrSplited[key]);
            }
            return arrTags;
        };

        $scope.add = function __add() {
            if ($scope.validade()) {
                $scope.save();
            }
        };

        $scope.saveredirect = function __saveredirect() {
            if ($scope.validade()) {
                $scope.save();
            }
        };

        $scope.save = function __save() {

            var item = $scope.createItem();

            var wrapper = $scope.wrapper(item);

            ItemModel.save({
                item: wrapper.item, files: wrapper.files, itemFiles: $scope.videos, itemAudios: $scope.audios
            },

                function (result) {

                    if (result.success) {

                        item.id = result.Item.Id;
                        item.code = result.Item.ItemCode;
                        item.itemCodeVersion = result.Item.ItemCodeVersion;
                        item.version = result.Item.ItemVersion;
                        item.versions.lista = [];
                        item.versions.lista.push(result.Item.Versions);
                        item.textobase.Id = result.Item.BaseText_Id;
                        $scope.textobase.Id = result.Item.BaseText_Id;

                        for (var t in item.textobase.Files) {
                            item.textobase.Files[t].ParentOwnerId = result.Item.Id;
                        }
                        for (var e in item.enunciado.Files) {
                            item.enunciado.Files[e].ParentOwnerId = result.Item.Id;
                        }

                        for (var i = 0; i < result.Item.Alternatives.length; i++) {
                            if (result.Item.Alternatives[i].Order == item.distratores[i].ordem) {
                                item.distratores[i].Id = result.Item.Alternatives[i].Id;

                                for (var a in item.distratores[i].Files) {
                                    item.distratores[i].Files[a].ParentOwnerId = result.Item.Id;
                                }

                                for (var j in item.distratores[i].justificativa.Files) {
                                    item.distratores[i].justificativa.Files[j].ParentOwnerId = result.Item.Id;
                                }
                            }
                        }

                        $scope.itens.push(item);
                        $notification.success("", "Item adicionado com sucesso!");
                        if (!$scope.item.ItemNarrated)
                            $scope.reset();

                        if ($scope.item.ItemNarrated)
                            $timeout(function () {
                                $scope.next(4);
                            }, 500);

                        return;
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
        };

        $scope.cancel = function __cancel() {
            $window.location.href = '/Item/List';
        };

        $scope.edit = function __edit(_item) {

            SubjectModel.loadSubjectBySubsubject({ idSubsubject: _item.subassunto.Id }, function (result) {
                if (result.success) {
                    $scope.assunto = result.assunto;
                    $scope.existeAssunto = true;

                    $scope.editInternal = true;
                    $scope.currentEditItem = angular.copy(_item);
                    $scope.restoreForEdit();
                    $scope.navigation = 3;

                    $timeout(function () {
                        $scope.loadingEditSkillsInternal = false;
                        $scope.loadingEditSkills = false;
                    }, 1000);
                }
            });

        };

        $scope.setdelete = function __setdelete(_item) {
            $scope.targetDelete = _item;
            angular.element("#modal").modal({
                backdrop: 'static'
            });
        };

        $scope.delete = function __delete() {

            ItemModel.delete({
                Id: $scope.targetDelete.id
            },

                function (result) {

                    if (result.success) {
                        angular.element("#modal").modal("hide");
                        $notification.success("Item excluído com sucesso!");
                        $scope.itens.splice($scope.targetDelete.index, 1);
                        $scope.targetDelete = undefined;
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                },
                function () {
                });

        };

        $scope.canceldelete = function __canceldelete() {
            $scope.targetDelete = undefined;
            angular.element('#modal').modal('hide');
        };

        $scope.finish = function __finish() {
            $window.location.href = '/Item/List';
        };

        $scope.finishEdit = function __finishEdit() {
            if ($scope.validade())
                $scope.update();
        };

        $scope.update = function __update() {

            var item = $scope.createItem();

            if ($scope.editMode)
                item.id = $scope.itemEditID;
            else if ($scope.editInternal)
                item.id = $scope.currentEditItem.id;

            var wrapper = $scope.wrapper(item);
            ItemModel.save({
                item: wrapper.item, files: wrapper.files, itemFiles: $scope.videos, itemAudios: $scope.audios
            },

                function (result) {

                    if (result.success) {

                        if ($scope.editInternal) {
                            $scope.editInternal = false;
                            item.id = result.Item.Id;
                            item.code = result.Item.ItemCode;
                            item.itemCodeVersion = result.Item.ItemCodeVersion;
                            item.version = result.Item.ItemVersion;
                            item.versions.lista = [];
                            item.versions.lista = result.Item.Versions;
                            item.index = $scope.currentEditItem.index;

                            for (var t in item.textobase.Files) {
                                item.textobase.Files[t].ParentOwnerId = result.Item.Id;
                            }
                            for (var e in item.enunciado.Files) {
                                item.enunciado.Files[e].ParentOwnerId = result.Item.Id;
                            }

                            for (var i = 0; i < result.Item.Alternatives.length; i++) {
                                if (result.Item.Alternatives[i].Order == item.distratores[i].ordem) {
                                    item.distratores[i].Id = result.Item.Alternatives[i].Id;

                                    for (var a in item.distratores[i].Files) {
                                        item.distratores[i].Files[a].ParentOwnerId = result.Item.Id;
                                    }

                                    for (var j in item.distratores[i].justificativa.Files) {
                                        item.distratores[i].justificativa.Files[j].ParentOwnerId = result.Item.Id;
                                    }
                                }
                            }
                            $scope.itens[$scope.currentEditItem.index] = $scope.copy(item);
                            $scope.currentEditItem = null;
                            $scope.navigation = 4;
                            $scope.reset();
                        }
                        else {
                            $window.location.href = '/Item/List';
                            $notification.success("Item atualizado com sucesso!");
                        }

                        return;
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
        };

        $scope.setNewValueTextBase = function __setNewValue() {
            if ($scope.textobase.StudentBaseText)
                $scope.textobase.NarrationStudentBaseText = false;
        };

        $scope.setNewValueEnunciationTest = function __setNewValue() {
            if ($scope.item.StudentStatement)
                $scope.item.NarrationStudentStatement = false;
        };

        $scope.reset = function __reset() {

            $scope.enunciado.Description = undefined;
            $scope.sentenca = undefined;
            $scope.proficiencia = undefined;
            $scope.palavrasChave = [];
            $scope.dicas = undefined;
            $scope.statusItem.objStatusItem = undefined;
            $scope.dificuldade.objDificuldade = undefined;
            $scope.code = undefined;
            $scope.ItemCodeVersion = undefined;
            $scope.version = undefined;
            $scope.versions.lista = [];
            $scope.sigiloItem.value = false;
            $scope.assunto = undefined;
            $scope.subassunto = undefined;
            $scope.videos = [];
            $scope.audios = [];
            newModal();
            newModalAudio();

            $(".comboAssunto").select2("val", "");
            $(".comboSubassunto").select2("val", "");
            $(".comboAssunto").text("");
            $(".comboSubassunto").text("");
            $(".comboAssunto").prepend("<option></option>");
            $(".comboSubassunto").prepend("<option></option>");

            loadSubject();

            for (var i = 0; i < $scope.skills.length; i++) {
                $scope.skills[i].objSkill = undefined;
            }

            for (var j = 0; j < $scope.distratores.length; j++) {
                $scope.distratores[j].selecionado = false;
                $scope.distratores[j].texto = "";
                $scope.distratores[j].justificativa.Description = "";

                for (var key in $scope.distratores[j].tct) {
                    $scope.distratores[j].tct[key].Value = undefined;
                }
            }

            for (var key in $scope.tri) {
                $scope.tri[key].Value = undefined;
            }
        };

        $scope.createItem = function __createItem() {

            var item = {
                id: 0,
                itemCodeVersion: $scope.ItemCodeVersion,
                index: $scope.itens.length,
                code: $scope.code,
                version: $scope.version,
                versions: JSON.parse(JSON.stringify($scope.versions)),
                disciplina: JSON.parse(JSON.stringify($scope.materia.objMateria)),
                modelMatriz: JSON.parse(JSON.stringify($scope.modeloMatriz)),
                matriz: JSON.parse(JSON.stringify($scope.matriz.objMatriz)),
                series: JSON.parse(JSON.stringify($scope.series)),
                textobase: JSON.parse(JSON.stringify($scope.textobase)),
                skills: JSON.parse(JSON.stringify($scope.skills)),
                statusItem: JSON.parse(JSON.stringify($scope.statusItem.objStatusItem)),
                tipoItem: JSON.parse(JSON.stringify($scope.tipoItem.objTipoItem)),
                palavrasChave: JSON.parse(JSON.stringify($scope.palavrasChave)),
                sentenca: $scope.sentenca,
                proficiencia: $scope.proficiencia,
                dificuldade: JSON.parse(JSON.stringify($scope.dificuldade)),
                dicas: $scope.dicas,
                tri: JSON.parse(JSON.stringify($scope.tri)),
                enunciado: JSON.parse(JSON.stringify($scope.enunciado)),
                distratores: JSON.parse(JSON.stringify($scope.distratores)),
                IsRestrict: JSON.parse(JSON.stringify($scope.sigiloItem)),
                knowledgeArea: JSON.parse(JSON.stringify($scope.area.objArea)),
                subassunto: JSON.parse(JSON.stringify($scope.subassunto)),
                assunto: $scope.assunto,
                videos: $scope.videos,
                audios: $scope.audios
            };

            return item;
        };

        $scope.restoreForEdit = function __restoreForEdit() {

            $scope.loadingEditSkillsInternal = true;
            $scope.loadingEditSkills = true;
            $scope.version_visibility = true;
            $scope.versions.lista = [];

            if (!$scope.currentEditItem)
                return;

            if ($scope.currentEditItem.skills) {

                $scope.skills = JSON.parse(JSON.stringify($scope.currentEditItem.skills));

                for (var j = 0; j < $scope.skills.length; j++) {
                    for (var i = 0; i < $scope.skills[j].lista.length; i++) {
                        if ($scope.skills[j].lista[i].Id == $scope.skills[j].objSkill.Id)
                            $scope.skills[j].objSkill = $scope.skills[j].lista[i];
                    }
                }
            }

            if ($scope.currentEditItem.tri)
                $scope.tri = JSON.parse(JSON.stringify($scope.currentEditItem.tri));

            if ($scope.currentEditItem.distratores)
                $scope.distratores = JSON.parse(JSON.stringify($scope.currentEditItem.distratores));

            if ($scope.currentEditItem.sentenca)
                $scope.sentenca = JSON.parse(JSON.stringify($scope.currentEditItem.sentenca));

            if ($scope.currentEditItem.proficiencia)
                $scope.proficiencia = JSON.parse(JSON.stringify($scope.currentEditItem.proficiencia));

            if ($scope.currentEditItem.tipoItem)
                for (var i = 0; i < $scope.tipoItem.lista.length; i++) {
                    if ($scope.tipoItem.lista[i].Id == $scope.currentEditItem.tipoItem.Id)
                        $scope.tipoItem.objTipoItem = $scope.tipoItem.lista[i]
                    $scope.valuePrev = $scope.tipoItem.objTipoItem;
                }

            if ($scope.currentEditItem.statusItem)
                for (var j = 0; j < $scope.statusItem.lista.length; j++) {
                    if ($scope.statusItem.lista[j].Id == $scope.currentEditItem.statusItem.Id)
                        $scope.statusItem.objStatusItem = $scope.statusItem.lista[j]
                }

            if ($scope.currentEditItem.dificuldade)
                if ($scope.currentEditItem.dificuldade.objDificuldade)
                    $scope.dificuldade.objDificuldade = JSON.parse(JSON.stringify($scope.currentEditItem.dificuldade.objDificuldade));

            if ($scope.currentEditItem.dicas)
                $scope.dicas = JSON.parse(JSON.stringify($scope.currentEditItem.dicas));

            if ($scope.currentEditItem.palavrasChave)
                $scope.palavrasChave = JSON.parse(JSON.stringify($scope.currentEditItem.palavrasChave));

            if ($scope.currentEditItem.enunciado)
                $scope.enunciado = JSON.parse(JSON.stringify($scope.currentEditItem.enunciado));

            if ($scope.currentEditItem.code)
                $scope.code = JSON.parse(JSON.stringify($scope.currentEditItem.code));

            if ($scope.currentEditItem.itemCodeVersion)
                $scope.ItemCodeVersion = JSON.parse(JSON.stringify($scope.currentEditItem.itemCodeVersion));

            if ($scope.currentEditItem.series)
                $scope.series = JSON.parse(JSON.stringify($scope.currentEditItem.series));

            if ($scope.currentEditItem.version)
                $scope.version = JSON.parse(JSON.stringify($scope.currentEditItem.version));

            if ($scope.currentEditItem.versions)
                $scope.versions = JSON.parse(JSON.stringify($scope.currentEditItem.versions));

            if ($scope.currentEditItem.IsRestrict)
                $scope.sigiloItem = JSON.parse(JSON.stringify($scope.currentEditItem.IsRestrict));

            if ($scope.currentEditItem.palavrasChave)
                $scope.palavrasChave = JSON.parse(JSON.stringify($scope.currentEditItem.palavrasChave));

            if ($scope.currentEditItem.subassunto) {
                $scope.subassunto = JSON.parse(JSON.stringify($scope.currentEditItem.subassunto));
                $scope.subassunto.Description = $scope.assunto.Subsubject_Description;
            }

        };

        $scope.editCancel = function __editCancel() {

            $scope.version_visibility = false;
            $scope.editInternal = false;
            $scope.currentEditItem = null;
            $scope.navigation = 4;
            $scope.reset();
        };

        $scope.wrapper = function __wrapper(_item) {

            var itemSkills = [];
            for (var key in _item.skills) {
                itemSkills.push({
                    Id: _item.skills[key].Id,
                    OriginalSkill: true,
                    Skill_Id: _item.skills[key].objSkill.Id
                });
            }
            var files = [];
            files = $.merge(files, _item.textobase.Files);
            files = $.merge(files, _item.enunciado.Files);
            var alternatives = [];
            for (var key in _item.distratores) {
                var ownerId = _item.distratores[key].ordem;
                if (_item.distratores[key].Id > 0)
                    ownerId = _item.distratores[key].Id;
                for (var a in _item.distratores[key].Files) {
                    _item.distratores[key].Files[a].OwnerId = ownerId;
                }

                for (var j in _item.distratores[key].justificativa.Files) {
                    _item.distratores[key].justificativa.Files[j].OwnerId = ownerId;
                }
                files = $.merge(files, _item.distratores[key].Files);
                files = $.merge(files, _item.distratores[key].justificativa.Files);

                for (var v = 0; v < $scope.distratores[key].tct.length; v++) {

                    if ($scope.distratores[key].tct[v].Value != undefined)
                        $scope.distratores[key].tct[v].Value.toString().replace(".", ",")
                    else
                        $scope.distratores[key].tct[v].Value = undefined;
                }
                alternatives.push({
                    Id: _item.distratores[key].Id,
                    Description: _item.distratores[key].texto,
                    Order: key,
                    Correct: _item.distratores[key].selecionado,
                    Justificative: _item.distratores[key].justificativa.Description,
                    numeration: _item.distratores[key].Description,
                    State: _item.distratores[key].State,
                    TCTBiserialCoefficient: $scope.distratores[key].tct[2].Value,
                    TCTDificulty: $scope.distratores[key].tct[1].Value,
                    TCTDiscrimination: $scope.distratores[key].tct[0].Value
                });
            }

            var wrapper = {
                Id: _item.id,
                ItemCodeVersion: _item.itemCodeVersion,
                Statement: _item.enunciado.Description,
                descriptorSentence: _item.sentenca,
                proficiency: _item.proficiencia,
                EvaluationMatrix_Id: _item.matriz.Id,
                Keywords: $scope.joinKeywords(_item.palavrasChave),
                Tips: _item.dicas,
                TRICasualSetting: (_item.tri[2].Value != undefined) ? _item.tri[2].Value.toString().replace(".", ",") : undefined,
                TRIDifficulty: (_item.tri[1].Value != undefined) ? _item.tri[1].Value.toString().replace(".", ",") : undefined,
                TRIDiscrimination: (_item.tri[0].Value != undefined) ? _item.tri[0].Value.toString().replace(".", ",") : undefined,
                BaseText: _item.textobase,
                ItemSituation_Id: _item.statusItem.Id,
                ItemType_Id: _item.tipoItem.Id,
                ItemLevel_Id: _item.dificuldade.objDificuldade != undefined ? _item.dificuldade.objDificuldade.Id : undefined,
                ItemCode: _item.code,
                ItemVersion: _item.version,
                ItemCurriculumGrades: [{
                    TypeCurriculumGradeId: _item.series.selected.Id
                }],
                ItemSkills: itemSkills,
                Alternatives: alternatives,
                IsRestrict: _item.IsRestrict.value,
                ItemNarrated: $scope.item.ItemNarrated,
                StudentStatement: $scope.item.StudentStatement,
                NarrationStudentStatement: $scope.item.NarrationStudentStatement,
                NarrationAlternatives: $scope.item.NarrationAlternatives,
                KnowledgeArea_Id: $scope.area.objArea.Id,
                SubSubject_Id: $scope.subassunto.Id
            };

            return {
                item: wrapper, files: files
            };
        }

        $scope.authorizeChangeBaseText = function __authorizeChangeBaseText(i) {

            angular.element('#mudarTextoBase').modal("hide");
            $scope.textobase.Authorize = true;
            $scope.next(3);
        };

        $scope.notAuthorizeChangeBaseText = function __notAuthorizeChangeBaseText() {

            angular.element('#mudarTextoBase').modal("hide");
            $scope.textobase.Description = $scope.textobase.History;
        };

        $scope.validade = function __validade(_nextStep) {

            switch (_nextStep) {

                case 2:
                    if ($scope.area.Id == undefined && $scope.area.objArea.Description == "") {
                        $notification.alert("*Área de conhecimento é uma seleção obrigatória.");
                        return false;
                    }
                    if ($scope.materia.Id == undefined && $scope.materia.objMateria.Description == "") {
                        $notification.alert("*Componente Curricular é uma seleção obrigatória.");
                        return false;
                    }

                    if ($scope.matriz.Id == undefined && $scope.matriz.objMatriz.Description == "") {
                        $notification.alert("*Matriz é uma seleção obrigatória.");
                        return false;
                    }

                    break;

                case 3:

                    if ($scope.parameters.BASETEXT.State == $scope.EnumState.ativo && $scope.parameters.BASETEXT.Obligatory) {

                        if (($scope.textobase.Description == "" ||
                            $scope.textobase.Description == "<p><br></p>" ||
                            $scope.textobase.Description == null)) {

                            $notification.alert("Atenção", $scope.parameters.BASETEXT.Value + " é um campo obrigatório!");
                            return false;
                        }
                    }

                    if ($scope.parameters.SOURCE.State == $scope.EnumState.ativo && $scope.parameters.SOURCE.Obligatory) {


                        if ($scope.textobase.Source == undefined || $scope.textobase.Source.length == 0) {
                            $notification.alert("Atenção", $scope.parameters.SOURCE.Value + " é um campo obrigatório!");
                            return false;
                        }
                    }

                    if ($scope.textobase.History != undefined && $scope.textobase.Authorize == false) {

                        if ($scope.textobase.History != $scope.textobase.Description) {

                            angular.element('#mudarTextoBase').modal({
                                backdrop: 'static'
                            });
                            return false;
                        }
                    }

                    if ($scope.item.ItemNarrated) {

                        if ($scope.parameters.INITIAL_ORIENTATION.State == $scope.EnumState.ativo && $scope.parameters.INITIAL_ORIENTATION.Obligatory) {

                            if ($scope.textobase.InitialOrientation == undefined || $scope.textobase.InitialOrientation.length == 0) {
                                $notification.alert("Atenção", $scope.parameters.INITIAL_ORIENTATION.Value + " é um campo obrigatório!");
                                return false;
                            }
                        }

                        if ($scope.parameters.INITIAL_STATEMENT.State == $scope.EnumState.ativo && $scope.parameters.INITIAL_STATEMENT.Obligatory) {

                            if ($scope.textobase.InitialStatement == undefined || $scope.textobase.InitialStatement.length == 0) {
                                $notification.alert("Atenção", $scope.parameters.INITIAL_STATEMENT.Value + " é um campo obrigatório!");
                                return false;
                            }
                        }

                        if ($scope.parameters.BASETEXT_ORIENTATION.State == $scope.EnumState.ativo && $scope.parameters.BASETEXT_ORIENTATION.Obligatory) {

                            if ($scope.textobase.BaseTextOrientation == undefined || $scope.textobase.BaseTextOrientation.length == 0) {
                                $notification.alert("Atenção", $scope.parameters.BASETEXT_ORIENTATION.Value + " é um campo obrigatório!");
                                return false;
                            }
                        }
                    }

                    if (($scope.textobase.Description == "" ||
                        $scope.textobase.Description == "<p><br></p>" ||
                        $scope.textobase.Description == null)) {

                        if (($scope.textobase.Source != undefined || $scope.textobase.Source != null || $scope.textobase.Source != "") && ($scope.textobase.Source != null && $scope.textobase.Source.length != 0)
                            ||
                            ($scope.item.ItemNarrated && ($scope.textobase.BaseTextOrientation != undefined || $scope.textobase.BaseTextOrientation != null || $scope.textobase.BaseTextOrientation != "") && ($scope.textobase.BaseTextOrientation != null && $scope.textobase.BaseTextOrientation.length != 0))) {

                            $notification.alert("Atenção", "O campo " + $scope.parameters.BASETEXT.Value + " não pode ficar em branco, pois existe(m) campo(s) preenchido(s) referente(s) a(ao) " + $scope.parameters.BASETEXT.Value + ".<br/>Por favor, verifique.");
                            return false;
                        }
                    }

                    break;

                case 4:
                    //visualizar
                    break;

                default:

                    if ($scope.parameters.CODE.State == $scope.EnumState.ativo && $scope.parameters.CODE.Obligatory) {

                        if ($scope.code == "" || $scope.code == undefined) {
                            $notification.alert($scope.parameters.CODE.Value + " é um campo de preenchimento obrigatório.");
                            return false;
                        }
                    }

                    if ($scope.parameters.DESCRIPTORSENTENCE.State == $scope.EnumState.ativo && $scope.parameters.DESCRIPTORSENTENCE.Obligatory) {

                        if ($scope.sentenca == "" || $scope.sentenca == undefined) {
                            $notification.alert($scope.parameters.DESCRIPTORSENTENCE.Value + " é um campo de preenchimento obrigatório.");
                            return false;
                        }
                    }

                    if ($scope.skills.length == 0) {
                        $notification.alert("Não há nenhuma habilidade cadastrada.");
                        return false;
                    }

                    for (var k in $scope.skills) {
                        if ($scope.skills[k].objSkill == undefined || ($scope.skills[k].objSkill && !$scope.skills[k].objSkill.Id)) {
                            $notification.alert($scope.skills[k].Description + " é um campo de seleção obrigatória.");
                            return false;
                        }
                    }

                    if ($scope.assunto === undefined || $scope.assunto.Id === undefined) {
                        $notification.alert("Assunto é um campo de preenchimento obrigatório.");
                        return false;
                    }

                    if ($scope.subassunto === undefined || $scope.subassunto.Id === undefined || $scope.subassunto.Id === null) {
                        $notification.alert("Subassunto é um campo de preenchimento obrigatório.");
                        return false;
                    }

                    if ($scope.parameters.ITEMCURRICULUMGRADE.State == $scope.EnumState.ativo && $scope.series.selected == undefined) {
                        $notification.alert($scope.parameters.ITEMCURRICULUMGRADE.Value + " é um campo de preenchimento obrigatório.");
                        return false;
                    }

                    if ($scope.parameters.ITEMLEVEL.State == $scope.EnumState.ativo && $scope.parameters.ITEMLEVEL.Obligatory) {

                        if ($scope.dificuldade.objDificuldade == undefined) {
                            $notification.alert($scope.parameters.ITEMLEVEL.Value + " é um campo de preenchimento obrigatório.");
                            return false;
                        }
                    }

                    if ($scope.parameters.KEYWORDS.State == $scope.EnumState.ativo && $scope.parameters.KEYWORDS.Obligatory) {

                        if ($scope.palavrasChave.length == 0 || $scope.palavrasChave == undefined) {
                            $notification.alert($scope.parameters.KEYWORDS.Value + " é um campo de preenchimento obrigatório.");
                            return false;
                        }
                    }

                    if ($scope.parameters.PROFICIENCY.State == $scope.EnumState.ativo && $scope.parameters.PROFICIENCY.Obligatory) {

                        if ($scope.proficiencia == undefined || $scope.proficiencia == "" || $scope.proficiencia == null) {

                            $notification.alert($scope.parameters.PROFICIENCY.Value + " é um campo de preenchimento obrigatório.");
                            return false;
                        }

                        if (!isNaN(parseInt($scope.proficiencia))) {
                            if (parseInt($scope.proficiencia) < 100 || parseInt($scope.proficiencia) > 500) {
                                $notification.alert($scope.parameters.PROFICIENCY.Value + " deve conter valores entre 100 e 500");
                                return false;
                            }
                        }
                    }

                    if ($scope.parameters.ITEMSITUATION.State == $scope.EnumState.ativo && $scope.statusItem.objStatusItem == undefined) {
                        $notification.alert($scope.parameters.ITEMSITUATION.Value + " é um campo de preenchimento obrigatório.");
                        return false;
                    }

                    if ($scope.parameters.ITEMTYPE.State == $scope.EnumState.ativo && $scope.tipoItem.objTipoItem == undefined) {
                        $notification.alert($scope.parameters.ITEMTYPE.Value + " é um campo de preenchimento obrigatório.");
                        return false;
                    }

                    if ($scope.parameters.TIPS.State == $scope.EnumState.ativo && $scope.parameters.TIPS.Obligatory) {

                        if ($scope.dicas == "" || $scope.dicas == undefined) {
                            $notification.alert($scope.parameters.TIPS.Value + " é um campo de preenchimento obrigatório.");
                            return false;
                        }
                    }

                    if ($scope.parameters.STATEMENT.State == $scope.EnumState.ativo && $scope.parameters.STATEMENT.Obligatory) {

                        if ($scope.enunciado.Description == undefined || $scope.enunciado.Description == "") {
                            $notification.alert($scope.parameters.STATEMENT.Value + " é um campo de preenchimento obrigatório.");
                            return false;
                        }
                    }

                    if ($scope.parameters.ISRESTRICT.State == $scope.EnumState.ativo && $scope.parameters.ISRESTRICT.Obligatory && $scope.admin) {

                        if ($scope.sigiloItem.value == undefined || $scope.sigiloItem.value == null) {
                            $notification.alert($scope.parameters.ISRESTRICT.Value + " é um campo de preenchimento obrigatório.");
                            return false;
                        }
                    }
                    var selecionado = false;
                    for (var key in $scope.distratores) {
                        if ($scope.distratores[key].State == 1) {
                            if ($scope.parameters.ALTERNATIVES.State == $scope.EnumState.ativo && $scope.parameters.ALTERNATIVES.Obligatory) {
                                if (key < ($scope.distratores.length - $scope.typeItemQtd))
                                    if ($scope.distratores[key].texto == "") {
                                        $notification.alert($scope.parameters.ALTERNATIVES.Value + " é um campo de preenchimento obrigatório.");
                                        return false;
                                    }
                            }

                            if ($scope.parameters.JUSTIFICATIVE.State == $scope.EnumState.ativo && $scope.parameters.JUSTIFICATIVE.Obligatory) {

                                if ($scope.distratores[key].justificativa.Description == "") {
                                    $notification.alert($scope.parameters.JUSTIFICATIVE.Value + " é um campo de preenchimento obrigatório.");
                                    return false;
                                }
                            }

                            if ($scope.distratores[key].selecionado) {
                                selecionado = true;
                            }

                            if ($scope.parameters.TCT.State == $scope.EnumState.ativo && $scope.parameters.TCT.Obligatory) {

                                for (var t = 0; t < $scope.distratores[key].tct.length; t++) {

                                    if ($scope.distratores[key].tct[t].Value == undefined || $scope.distratores[key].tct[t].Value == null || $scope.distratores[key].tct[t].Value.length == 0) {

                                        $notification.alert($scope.parameters.TCT.Value + " são campos obrigatórios.");
                                        return false;
                                    }
                                }
                            }
                            var parceFloat, p;
                            for (p = 0; p < $scope.distratores[key].tct.length; p++) {

                                if ($scope.distratores[key].tct[p].Value != null && $scope.distratores[key].tct[p].Value != undefined) {

                                    $scope.distratores[key].tct[p].Value = '' + $scope.distratores[key].tct[p].Value;

                                    parceFloat = parseFloat($scope.distratores[key].tct[p].Value.replace(',', '.'));

                                    if (!isNaN(parceFloat) && isFinite($scope.distratores[key].tct[p].Value.replace(',', '.')) == false) {
                                        $notification.alert("TCT: \"" + $scope.distratores[key].tct[p].preDescription + "\" formato inválido");
                                        return false;
                                    }

                                    if ($scope.distratores[key].tct[p].preDescription == "Discriminação") {
                                        if (parceFloat < -1 || parceFloat > 1) {
                                            $notification.alert("TCT: \"" + $scope.distratores[key].tct[p].preDescription + "\" o valor deve ser " + $scope.distratores[key].tct[p].postDescription);
                                            return false;
                                        }
                                    }
                                    else if ($scope.distratores[key].tct[p].preDescription == "Proporção de acertos") {
                                        if (parceFloat < 0 || parceFloat > 1) {
                                            $notification.alert("TCT: \"" + $scope.distratores[key].tct[p].preDescription + "\" o valor deve ser " + $scope.distratores[key].tct[p].postDescription);
                                            return false;
                                        }
                                    }
                                    else if ($scope.distratores[key].tct[p].preDescription == "Coeficiente bisserial") {
                                        if (parceFloat < -1 || parceFloat > 1) {
                                            $notification.alert("TCT: \"" + $scope.distratores[key].tct[p].preDescription + "\" o valor deve ser " + $scope.distratores[key].tct[p].postDescription);
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if ($scope.tipoItem.objTipoItem.Id != 3) {
                        if ($scope.parameters.ALTERNATIVES.State == $scope.EnumState.ativo && $scope.parameters.ALTERNATIVES.Obligatory) {

                            if (selecionado == false) {
                                $notification.alert("É necessário selecionar uma alternativa como correta.");
                                return false;
                            }
                        }
                    }



                    if ($scope.parameters.TRI.State == $scope.EnumState.ativo && $scope.parameters.TRI.Obligatory) {

                        for (var y = 0; y < $scope.tri.length; y++) {

                            if ($scope.tri[y].Value == undefined || $scope.tri[y].Value == null || $scope.tri[y].Value.length == 0) {

                                $notification.alert($scope.parameters.TRI.Value + " são campos obrigatórios.");
                                return false;
                            }
                        }
                    }

                    for (var a = 0; a < $scope.tri.length; a++) {

                        if ($scope.tri[a].Value != null && $scope.tri[a].Value != undefined) {

                            if (!isNaN(parseFloat($scope.tri[a].Value.replace(',', '.'))) && isFinite($scope.tri[a].Value.replace(',', '.')) == false) {
                                $notification.alert("TRI: \"" + $scope.tri[a].preDescription + "\" o valor deve ser " + $scope.tri[a].postDescription);
                                return false;
                            }

                            if ($scope.tri[a].preDescription == "Discriminação") {
                                if (parseFloat($scope.tri[a].Value.replace(',', '.')) < 0 || parseFloat($scope.tri[a].Value.replace(',', '.')) > 10) {
                                    $notification.alert("TRI: \"" + $scope.tri[a].preDescription + "\" o valor deve ser " + $scope.tri[a].postDescription);
                                    return false;
                                }
                            }
                            else if ($scope.tri[a].preDescription == "Dificuldade") {
                                if (parseFloat($scope.tri[a].Value.replace(',', '.')) <= -100000 || parseFloat($scope.tri[a].Value.replace(',', '.')) >= 100000) {
                                    $notification.alert("TRI: \"" + $scope.tri[a].preDescription + "\" o valor deve ser " + $scope.tri[a].postDescription);
                                    return false;
                                }
                            }
                            else if ($scope.tri[a].preDescription == "Acerto casual") {
                                if (parseFloat($scope.tri[a].Value.replace(',', '.')) < 0 || parseFloat($scope.tri[a].Value.replace(',', '.')) > 1) {
                                    $notification.alert("TRI: \"" + $scope.tri[a].preDescription + "\" o valor deve ser " + $scope.tri[a].postDescription);
                                    return false;
                                }
                            }
                        }

                    }

                    for (var v = 0; v < $scope.videos.length; v++) {
                        if ($scope.videos[v].File.Path === undefined && $scope.videos[v].Thumbnail.Path !== undefined) {
                            $notification.alert("É necessário selecionar um vídeo.");
                            return false;
                        }

                        if ($scope.videos[v].File.Path !== undefined && $scope.videos[v].Thumbnail.Path === undefined) {
                            $notification.alert("É necessário selecionar um thumbnail.");
                            return false;
                        }
                    }

                    break;
            }

            return true;
        };

        $scope.previus = function __previus(_navigation) {
            $scope.navigation = _navigation;
            adminCarregamento(_navigation);
        };

        $scope.next = function __next(_navigation) {

            if (_navigation == 1) {
                loaded();
                return;
            }
            else if (_navigation == 3 && !$scope.editMode) {
                newModal();
                newModalAudio();
            }

            if ($scope.validade(_navigation)) {
                $scope.navigation = _navigation;
                adminCarregamento(_navigation);
            }

        };

        $scope.travarEtapa1 = function __travarEtapa1() {
            if ($scope.itens.length > 0) {
                $(".comboComponenteCurricular").prop("disabled", true);
                $(".comboMatriz").prop("disabled", true);
            }
            else {
                var areaConhecimento = $('.comboAreaConhecimento').val();

                if (areaConhecimento === null || areaConhecimento === "") {
                    $(".comboComponenteCurricular").prop("disabled", true);
                }
                else {
                    $(".comboComponenteCurricular").prop("disabled", false);
                }

                var comboComponenteCurricular = $('.comboComponenteCurricular').val();

                if (comboComponenteCurricular === null || comboComponenteCurricular === "") {
                    $(".comboMatriz").prop("disabled", true);
                }
                else {
                    $(".comboMatriz").prop("disabled", false);
                }
            }
        };

        $scope.distratorSelectionControll = function __distratorSelectionControll(_distrator) {
            for (var key in $scope.distratores) {
                if (angular.equals(_distrator, $scope.distratores[key]))
                    $scope.distratores[key].selecionado = true;
                else
                    $scope.distratores[key].selecionado = false;
            }
        };

        $scope.controllUp = function __controllUp(_mode, _index) {

            if (_mode === 'show') {
                if (_index == 0)
                    return false;
                else
                    return true;
            }
            else if (_mode === 'action') {

                var distrTemp = $scope.copy($scope.distratores[_index]);
                $scope.distratores[_index] = $scope.copy($scope.distratores[(_index - 1)]);
                $scope.distratores[(_index - 1)] = $scope.copy(distrTemp);

                var desc = $scope.distratores[_index].Description;
                $scope.distratores[_index].Description = $scope.distratores[(_index - 1)].Description;
                $scope.distratores[(_index - 1)].Description = desc;
            }
        };

        $scope.controllDown = function __controllDown(_mode, _index) {

            if (_mode === 'show') {
                if (_index == ($scope.distratores.length - 1))
                    return false;
                else
                    return true;
            }
            else if (_mode === 'action') {

                var distrTemp = $scope.copy($scope.distratores[_index]);
                $scope.distratores[_index] = $scope.copy($scope.distratores[(_index + 1)]);
                $scope.distratores[(_index + 1)] = $scope.copy(distrTemp);

                var desc = $scope.distratores[_index].Description;
                $scope.distratores[_index].Description = $scope.distratores[(_index + 1)].Description;
                $scope.distratores[(_index + 1)].Description = desc;
            }
        };

        $scope.versionControll = function __versionControll(_state) {
            $scope.version_visibility = _state;
        };

        $scope.getGroupIds = function __getGroupIds(_group) {
            var groupID = [];
            for (key in _group) {
                groupID.push(_group[key].Id);
            }
            return groupID;
        };

        $scope.copy = function __copy(oldObj) {
            var newObj = oldObj;
            if (oldObj && typeof oldObj === 'object') {
                newObj = Object.prototype.toString.call(oldObj) === "[object Array]" ? [] : {
                };
                for (var i in oldObj) {
                    newObj[i] = $scope.copy(oldObj[i]);
                }
            }
            return newObj;
        }

        $scope.joinKeywords = function __joinKeywords(_tags) {
            var keywords = "";
            for (var i = 0; i < _tags.length; i++) {

                if (i < (_tags.length - 1))
                    keywords = keywords.concat(_tags[i].text + ";");
                else
                    keywords = keywords.concat(_tags[i].text);
            }
            return keywords;
        };

        $scope.TCTValidation = function __TCTValidation(distIndex, index) {

            if ($scope.distratores[distIndex].tct[index].preDescription == "Discriminação")
                $scope.distratores[distIndex].tct[index].Value = $scope.distratores[distIndex].tct[index].Value.replace(/[^0-9.,-]/g, "");
            else if ($scope.distratores[distIndex].tct[index].preDescription == "Proporção de acertos")
                $scope.distratores[distIndex].tct[index].Value = $scope.distratores[distIndex].tct[index].Value.replace(/[^0-9.,]/g, "");
            else if ($scope.distratores[distIndex].tct[index].preDescription == "Coeficiente bisserial")
                $scope.distratores[distIndex].tct[index].Value = $scope.distratores[distIndex].tct[index].Value.replace(/[^0-9.,-]/g, "");
        };

        $scope.TRIValidation = function __TRIValidation(index) {

            if ($scope.tri[index].preDescription == "Discriminação")
                $scope.tri[index].Value = $scope.tri[index].Value.replace(/[^0-9.,]/g, "");
            else if ($scope.tri[index].preDescription == "Proporção de acertos")
                $scope.tri[index].Value = $scope.tri[index].Value.replace(/[^0-9.,-]/g, "");
            else if ($scope.tri[index].preDescription == "Acerto casual")
                $scope.tri[index].Value = $scope.tri[index].Value.replace(/[^0-9.,]/g, "");
        };

        $scope.previewPrint = function __previewPrint(item) {
            window.open("/Item/PreviewPrintItem?id=" + item.id);
        };

        $scope.previewPrintBaseText = function __previewPrintBaseText(item) {
            window.open("/Item/PreviewPrintBaseText?id=" + item.Id);
        };

        $scope.focusCode = function __focusCode() {
            $scope.invalidCode = undefined;
        };

        $scope.blurCode = function __blurCode(_code) {
            $timeout(function () {
                if (_code != undefined) {
                    _code = $scope.code = _code.replace(new RegExp(/[^A-z\u00C0-\u00ff\s\x20-x40]+/g), '');
                    ItemModel.validateItemCode({
                        ItemCode: _code, ItemId: $scope.itemEditID
                    }, function (result) {
                        if (result.success) {
                            $scope.invalidCode = result.invalid;
                        }
                        else {
                            $scope.invalidCode = true;
                            $notification[result.type ? result.type : 'error'](result.message);
                        }
                    });
                }
            }, 0);
        };

        $scope.blurTips = function __blurTips(_tips) {
            $scope.dicas = _tips.replace(new RegExp(/[^A-z\u00C0-\u00ff\s\x20-x40]+/g), '');
        };

        /**
         * @function Editar alternativa
         * @param {Object} alternative
         * @returns
         */
        $scope.setEditAlternative = function __setEditAlternative(alternative) {
            $scope.currentAlternative = alternative;
            angular.element('#modalAlternativa').modal({
                backdrop: 'static'
            });
        };

        /**
         * @function Finalizar editar alternativa
         * @param {Object} alternative
         * @returns
         */
        $scope.closeEditAlternative = function __closeEditAlternative() {
            $scope.currentAlternative = undefined;
            angular.element('#modalAlternativa').modal('hide');
        };

        $scope.loaded();
    };

})(angular, jQuery);