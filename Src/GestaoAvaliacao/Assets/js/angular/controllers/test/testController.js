/**
 * function TestController Controller
 * @namespace Controller
 * @author Alexandre Calil B. Paravani - 14/05/2015
 * @author julio.silva@mstech.com.br - 07/11/2016
 */
(function (angular, $) {

    angular
        .module('appMain', ['services', 'filters', 'directives', 'tooltip', 'ngTagsInput']);

    angular
        .module('appMain')
        .controller("TestController", TestController);

    TestController.$inject = ['$scope', '$util', '$notification', '$pager', 'TestModel', 'ItemTypeModel', 'ModalityModel', 'TestTypeModel', 'TestGroupModel', 'NumberItemsAplicationTaiModel', '$window', 'EvaluationMatrixModel'];

    /**
     * @function Controller para criação de prova
     * @param {Object} ng
     * @param {Object} $notification
     * @param {Object} $pager
     * @param {Object} TestModel
     * @param {Object} ItemTypeModel
     * @param {Object} ModalityModel
     * @param {Object} TestTypeModel
     * @returns
     */
    function TestController(ng, $util, $notification, $pager, TestModel, ItemTypeModel, ModalityModel, TestTypeModel, TestGroupModel, NumberItemsAplicationTaiModel, $window, EvaluationMatrixModel) {

        ng.params = $util.getUrlParams();
        var self = this;

        /*$scope.listextensionsImage = ['image/jpeg', 'image/png', 'image/gif', 'image/bmp'];*/

        function load() {
            $notification.clear();
            configVariaveis();
        };

        /**
         * @function Configura wizard( breadcomb interno da prova )
         * @param {?} q
         * @returns
         */
        function configuraWizard(q) {
            if (q) ng.temBIB = q;
            var arr = [];
            arr.push(self.wizards[0]);
            arr.push(self.wizards[1]);
            if (ng.temBIB === null) return;
            arr.push(self.wizards[2]);
            ng.ultimo = 3;
            //if (!ng.temBIB) {
            //    ng.ultimo = 3;
            //    arr.push(self.wizards[3]);
            //}
            //else {
            //    ng.ultimo = 4;
            //    arr.push(self.wizards[2]);
            //    arr.push(self.wizards[4]);
            //}
            ng.wizardTAI = self.wizards[0];
            var wizardAnoItens = angular.copy(self.wizards[1]);
            wizardAnoItens.Description = 'Ano(s) dos itens da amostra';
            ng.listaWizardTAI.push(ng.wizardTAI);
            ng.listaWizardTAI.push(wizardAnoItens);

            ng.listaWizards = arr;
        };

        /**
         * @function Configura variaveis do escopo, globais e locais
         * @param {string} filter
         * @return
         */
        ng.editar = function __editar(id) {
            ng.editMode = false;
            if (id > -1) {
                ng.params = id;
                ng.editMode = true;
            }
            load();
        };

        /**
         * @function Configura variaveis do escopo, globais e locais
         * @param {string} filter
         * @return
         */
        function configVariaveis() {
            ng.listextensionsImage = ['image/jpeg', 'image/png', 'image/gif', 'image/bmp'];
            ng.EnumFrequencyApplication = {
                Yearly: 1,
                Semiannual: 2,
                Bimonthly: 3,
                Monthly: 4,
            };
            //Chamadas utilizada na Etapa 1
            self.etapa1 = {
                tipoProva: TestModel.loadByUserGroup,
                componenteCurricular: TestModel.searchDisciplinesSaves,
                dadosProva: TestModel.findTest,
                niveis: TestModel.getAll,
                prova: TestModel.loadTest,
                save: TestModel.save,
                bComponente: false,
                bTipoProva: false,
                uintWatchProva: null,
            };
            self.situacaoList = [
                { Id: 1, Description: "Pendente", Style: "icone-pendente material-icons situacao", Icon: 'remove_circle_outline' },
                { Id: 2, Description: "Cadastrada", Style: "icone-cadastrar material-icons situacao", Icon: 'radio_button_unchecked' },
                { Id: 3, Description: "Em andamento", Style: "icone-andamento material-icons situacao", Icon: 'timelapse' },
                { Id: 4, Description: "Aplicada", Style: "icone-aplicar material-icons situacao", Icon: 'check_circle' }
            ];
            //Chamadas utilizada na Etapa 2
            self.etapa2 = {
                salvar: TestModel.saveBlock,
                remover: TestModel.deleteBlock,
                salvarKnowLedgeAreaOrder: TestModel.saveKnowLedgeAreaOrder,
                paginacao: TestModel.searchBlock,
                nivelEnsino: TestModel.loadLevelEducation,
                modalidade: TestModel.loadModality,
                matrix: TestModel.getComboByDiscipline,
                filtroSituacao: TestModel.loadSituation,
                filtroDificuldades: TestModel.loadLevels,
                filtroPeriodos: TestModel.loadByLevelEducationModality,
                pegarMatrix: TestModel.getByMatriz,
                pegarHabilidade: TestModel.getByParent,
                blocos: TestModel.loadBlock,
                itensBloco: TestModel.visualizar,
                blockKnowledgeAreas: TestModel.getBlockKnowledgeAreas,
                itensVersoes: TestModel.GetItemVersions

            };
            //Chamadas utilizada na Etapa 3
            self.etapa3 = {
            };
            //Chamadas utilizada na Etapa 4
            self.etapa4 = {
                cadernos: TestModel.getAllByTest,
                html: TestModel.getHTMLTest,
                gerar: TestModel.generateTest,
                finalizar: TestModel.finallyTest,
            };
            // Dados do wizards
            self.wizards = [
                { Number: 1, Description: 'Cadastro Prova' },
                { Number: 2, Description: 'Selecionar itens' },
                { Number: 3, Description: 'Gerar provas' },
            ];
            ng.labels = {
                tipo: 'Tipo de prova',
                descricao: 'Descrição da prova',
                componente: 'Componente Curricular',
                nivelEnsino: 'Nível de ensino',
                frequencyApplication: 'Frequência de aplicação',
                periodo: Parameters.Item.ITEMCURRICULUMGRADE.Value + '(s) de aplicação',
                cronograma: 'Cronograma',
                inicioAplicaco: 'Início da aplicação',
                inicioDownload: 'Início do download',
                finalAplicacao: 'Final da aplicação',
                inicioCorrecao: 'Início da correção',
                finalCorrecao: 'Final da correção',
                quantidadeItens: 'Quantidade de itens',
                contextoProva: 'Contexto da prova',
                sugeridoTipoProva: 'Sugeridos pelo tipo de prova',
                personalizado: 'Personalizado',
                total: 'Total',
                bib: 'BIB',
                quantidadeBlocos: 'Quantidade de cadernos',
                e1_itensBlocos: 'Itens por caderno',
                niveis: 'Níveis de desempenho',
                matriz: 'Matriz',
                matrizAvaliacao: 'Matriz de avaliação',
                keywords: 'Palavra-chave',
                multidiscipline: 'Multidisciplinar',
                knowledgeAreaBlock: 'Gabarito com blocos de área de conhecimento',
                electronicTest: 'Prova eletrônica',
                showVideoFiles: 'Exibir conteúdo de vídeo',
                showJustificate: 'Exibir justificativa',
                showAudioFiles: 'Exibir conteúdo de áudio',
                showTestTAI: 'Aplicação em TAI',
                numberItemsTestTAI: 'Nº itens na amostra',
                informationTestTAI: 'Informações Teste TAI',
                advanceWithoutAnswering: 'Permitir avançar sem responder',
                backToPreviousItem: 'Permitir voltar ao item anterior',
                showOnSerapEstudantes: 'Exibir no Serap Estudantes',
                numberSynchronizedResponseItems: 'Qtde de itens para sincronização Resposta',
                showTestContext: 'Apresentar contexto da prova',
                tempoDeProva: 'Tempo de Prova',
                temBIB: 'Prova com BIB'
            };
            ng.curriculumGradeLabel = Parameters.Item.ITEMCURRICULUMGRADE.Value;
            //Lista de escolha 
            ng.listasSimNao = [
                { Description: 'Sim', Value: true },
                { Description: 'Não', Value: false }
            ];
            //Lista de wizards atuais
            ng.listaWizards = [];
            ng.wizardTAI = null;
            ng.listaWizardTAI = [];
            ng.etapaAtual = 1;
            ng.alterouEtapaAtual = false;
            ng.mostrarTela = false;
            ng.temBIB = false;
            ng.showFlagBIB = false;
            ng.modalAnterior = null;
            ng.provaPDF = null;
            ng.selecItensProxCaderno = false;
            ng.proximoBloco = null;
            //Controla breadcumb dos passos
            ng.navigation = 1;
            ng.itensTotais = 0;
            ng.ItemType = null;
            ng.TestTypeItemType = null;
            ng.ItemTypeList = [];
            ng.BtnSaveDisabled = false;
            ng.filesTest = null;
            ng.gerarFolhaResposta = true;
            ng.gerarFolhaTooltip = null;
            ng.publicFeedback = false;
            ng.showTestContext = false;
            ng.testContexts = [];
            ng.anosItensAmostraProvaTai = [];
            //Funções para configurar Etapas
            nivelDesempenhoCarregar();
            initEtapa1();
        };

        /**
        * @function Configura variaveis do escopo, globais e locais da ETAPA 1
        * @private
        * @param
        */
        function initEtapa1() {
            if (ng.editMode) ng.etapaAtual = 2;
            self.etapa1.alterou = false;
            //Padrao para preload
            ng.e1_listaTipoProva = [];
            ng.grupoSubgrupoList = [];
            ng.e1_grupoSubgrupo = null;
            ng.e1_tempoDeProva = null;
            ng.tempoDeProvaList = [];
            ng.provaId = 0;
            //ComboBox tipo prova
            ng.e1_cbTipoProva = null;
            //Descrição da prova
            ng.e1_testDescription = null;
            //Senha da prova
            ng.e1_testPassword = null;
            //Descrição da prova
            ng.e1_NivelEnsino = null;
            //Padrao para preload
            ng.e1_listaComponenteCurricular = [];
            //ComboBox componente curricular
            ng.e1_cbComponenteCurricular = null;
            ng.e1_nItensTestTAI = null;
            ng.e1_nItensTestTAIList = [];
            ng.showTestTAI = false;
            ng.advanceWithoutAnswering = false;
            ng.backToPreviousItem = false;
            // switch gerar folha de resposta
            ng.e1_folhaResp = false;
            ng.e1_folhaRespLock = false;
            ng.frequencyApplicationList = [];
            //Lista de Períodos de aplicação
            ng.e1_listaPeriodos = [];
            //Evita sobrescrita dos dados da prova
            ng.periodoEditMode;
            //Lista de periodos escolhidos
            ng.e1_listaPeriodosChecked = [];
            ng.e1_inicioDownload = null;
            // Data  da aplicação
            ng.e1_aplicacao = {
                Inicio: null,
                Final: null
            };
            ng.applicationActiveOrDone = false;
            // Data da correção
            ng.e1_correcao = {
                Inicio: null,
                Final: null
            };
            //Usar quantidade de itens
            ng.e1_qtdItens = null;
            ng.e1_listaQntItens = null;
            ng.e1_radios = 3;
            ng.isKnowledgeAreaBlock = false;
            ng.isElectronicTest = false;
            ng.showOnSerapEstudantes = false;
            ng.numberSynchronizedResponseItems = null;
            ng.showVideoFiles = false;
            ng.showAudioFiles = false;
            ng.showJustificate = false;
            //Lista de dificuldades do tipo de prova
            ng.e1_listaDificuldades = [];
            ng.Global = false;
            //Usar Nível de desempenho
            ng.e1_cbNiveisDesempenho = false;
            ng.e1_inpQntItens = null;
            ng.e1_listaDesempenho = ng.listasSimNao;
            ng.e1_listaNiveis = [];
            //Usar BIB
            ng.e1_cbBIB = null;
            ng.e1_listaBIB = ng.listasSimNao;
            ng.e1_qtdBlocos = null;
            ng.e1_listaQtdBlocos = null;
            ng.e1_itensBlocos = null;
            ng.e1_listaItensBlocos = null;
            ng.testId = null;
            ng.e1_formato_findTest = false;
            ng.params = {
                Id: ng.params
            };
            ng.provaId = ng.params.Id || 0;
            tipoProvaCarregar();
            // Modal contexto
            e1_criarObjetoDadosModalContexto();
            ng.e1_itemParaDeletarDaListaTestContex = '';
            loadNumberItemsAplicationTai();
        };

        /**
         * @function - Salvar
         * @param {Object} list - lista de valores que preencherá o combo
         * @param {Object} opcao - opcao a ser procurada dentro da lista
         * @public
         */
        function setValuesComb(list, opcao) {
            for (var k = 0; k < list.length; k++) {
                if (list[k].Description == opcao.Description) {
                    return list[k];
                };
            };
        };

        /**
        * @function Carrega itens amostra prova TAI
        * @private
        * @param
        */
        function loadNumberItemsAplicationTai() {
            NumberItemsAplicationTaiModel.loadAll({}, function (result) {
                if (result.success) {
                    ng.e1_nItensTestTAIList = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
        * @function Carrega tipo de prova
        * @private
        * @param
        */
        function tipoProvaCarregar() {
            self.etapa1.tipoProva(function (r) {
                ng.bTipoProva = true;
                carregaGrupoSubgrupo();
                if (r.success) {
                    //Detecta se prova selecionada permite BIB
                    ng.showFlagBIB = angular.copy(r.Bib);
                    //Configura breadcomb da prova
                    configuraWizard(ng.showFlagBIB);
                    r = r.lista;
                    ng.e1_listaTipoProva = angular.copy(r.testTypeList);
                    ng.e1_tipoNivelEnsino = angular.copy(r.TypeLevelEducation);
                    ng.tempoDeProvaList = angular.copy(r.temposDeProva);
                    //Exibe tela assim que terminar de carregar
                    if (!ng.editMode) {
                        ng.mostrarTela = true;
                        ng.situacao = procurarElementoEm([{ Id: r.TestSituation }], self.situacaoList)[0];
                    }
                    else {
                        //Carrega dados da prova
                        provaCarregar();
                    }
                } else {
                    if (r.type && r.message)
                        $notification[r.type ? r.type : 'error'](r.message);
                    configuraWizard(false);
                    ng.mostrarTela = true;
                    return false;
                }
            });
        };

        /**
        * @function Tratamento para alterações de tipo de prova
        * @private
        * @param
        */
        ng.tipoProvaMudou = tipoProvaMudou;
        function tipoProvaMudou() {
            if (!ng.e1_cbTipoProva) return;
            if (!ng.editMode) {
                if (ng.e1_cbTipoProva.Bib) {
                    ng.showFlagBIB = true;
                    ng.temBIB = false;
                } else {
                    ng.showFlagBIB = false;
                    ng.temBIB = false;
                }
                //ng.temBIB = ng.e1_cbTipoProva.Bib;

                configuraWizard(ng.temBIB);


                ng.e1_cbComponenteCurricular = null;
                ng.frequencyApplication = null;
                /*ng.e1_cbBIB = null;*/
                ng.e1_radios = 3
                if (ng.testId != ng.e1_cbTipoProva.Id) {
                    ng.e1_folhaRespLock = false;
                }
                else {
                    if (ng.ItemType == null || ng.ItemType == undefined) {
                        ng.e1_folhaRespLock = true;
                        ng.gerarFolhaResposta = false;
                    }
                }
            }
            periodoCarregar(ng.e1_cbTipoProva.Id);
            loadFrequencyApplication();
            if (ng.mostrarTela)
                ng.alterouEtapaAtual = self.etapa1.alterou = true;
        };

        /**
         * @function carregar frequências de aplicação
         * @param {object} _callback
         * @returns
         */
        function loadFrequencyApplication(_callback) {
            if (!ng.e1_cbTipoProva || !ng.e1_cbTipoProva.FrequencyApplication) return;
            if (ng.e1_cbTipoProva.FrequencyApplication == ng.EnumFrequencyApplication.Yearly) {
                ng.frequencyApplication = ng.e1_cbTipoProva.FrequencyApplication;
                return;
            }
            TestTypeModel.getFrequencyApplicationChildList({ parentId: ng.e1_cbTipoProva.FrequencyApplication }, function (result) {
                if (result.success) {
                    ng.frequencyApplicationList = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
                if (_callback) _callback();
            });
        };

        /**
        * @function Carrega dados de componente curricular
        * @private
        * @param
        */
        ng.validarDescription = validarDescription;
        function validarDescription() {
            if (ng.mostrarTela)
                ng.alterouEtapaAtual = self.etapa1.alterou = true;

            ng.e1_testDescription = ng.e1_testDescription.replace(/[¨´`~!@#$%^&*()|+\=?;:'",.<>\{\}\[\]\\\/]/gi, '');
        };

        ng.validarPassword = validarPassword;
        function validarPassword() {
            ng.alterouEtapaAtual = self.etapa1.alterou = true;
        }




        /**
        * @function Carrega dados de componente curricular
        * @private
        * @param
        */
        function ComponenteCurricularCarregar(tipoNivelEnsino) {
            if (tipoNivelEnsino || tipoNivelEnsino == 0) {
                self.etapa1.componenteCurricular({ typeLevelEducation: tipoNivelEnsino.Id }, ComponenteCurricularCarregado);
            }
            else {
                $notification.alert('Este tipo de prova não possui nível ensino cadastrado.')
            }
        };

        /**
        * @function Tratamento após ter recebido dados componente curricular
        * @private
        * @param r = resposta do servidor 
        */
        function ComponenteCurricularCarregado(r) {

            if (r.success) {
                if (ng.editMode)
                    return;

                ng.e1_listaComponenteCurricular = angular.copy(r.lista);

                if (ng.e1_cbComponenteCurricular)
                    ng.e1_cbComponenteCurricular = procurarElementoEm([ng.e1_cbComponenteCurricular], ng.e1_listaComponenteCurricular)[0];

                ng.bComponente = true;
            } else {
                if (r.type && r.message)
                    $notification[r.type ? r.type : 'error'](r.message);
                return false;
            }
        };

        /**
        * @function Tratamento para alterações de componente curricular
        * @private
        * @param
        */
        ng.e1_ComponenteCurricularMudou = e1_ComponenteCurricularMudou;
        function e1_ComponenteCurricularMudou() {

            if (!ng.e1_cbComponenteCurricular)
                return;

            ng.e2_ListaComboBox = null;
            if (ng.mostrarTela) ng.alterouEtapaAtual = self.etapa1.alterou = true;
        };

        /**
        * @function Tratamento para alterações de prova eletronica
        * @private
        * @param
        */

        ng.e1_GrupoSubgrupoMudou = e1_GrupoSubgrupoMudou;
        function e1_GrupoSubgrupoMudou() {

            if (ng.mostrarTela) ng.alterouEtapaAtual = self.etapa1.alterou = true;

            if (!ng.e1_grupoSubgrupo)
                return;
        };

        /**
        * @function Tratamento para alterações qtde itens aplicação em TAI
        * @private
        * @param
        */

        ng.e1_nItensTestTAIMudou = e1_nItensTestTAIMudou;
        function e1_nItensTestTAIMudou() {

            if (ng.mostrarTela) ng.alterouEtapaAtual = self.etapa1.alterou = true;

            if (!ng.e1_nItensTestTAI)
                return;
        };

        ng.e1_TempoDeProvaMudou = e1_TempoDeProvaMudou;
        function e1_TempoDeProvaMudou() {

            if (ng.mostrarTela) ng.alterouEtapaAtual = self.etapa1.alterou = true;

            if (!ng.e1_tempoDeProva)
                return;
        };

        /**
        * @function Tratamento para alterações de componente curricular
        * @private
        * @param
        */
        function periodoCarregar(id) {
            self.etapa1.dadosProva({ Id: id }, function (r) {
                if (r.success) {
                    r = r.testType;
                    ng.testId = r.Id;
                    //Detecta se prova selecionada permite BIB
                    ng.e1_folhaResp = true;
                    ng.e1_listaPeriodos = angular.copy(r.TypeCurriculumGrade);
                    ng.TestTypeItemType = r.ItemType;
                    ng.ItemType = r.ItemType;
                    if (ng.ItemType == null || ng.ItemType == undefined) {
                        ng.e1_folhaRespLock = true;
                        ng.gerarFolhaResposta = false;
                    }

                    ng.carregaItemType();

                    //Dados da prova
                    if (ng.editMode) {
                        if (ng.e1_radios === 2) {
                            if (r.FormatType != null)
                                ng.e1_formato = r.FormatType;
                            ng.e1_listaDificuldades = (angular.copy(r.TestTypeItemLevel));
                            if (ng.e1_formato != null) {
                                if (ng.e1_formato.Description === "Porcentagem")
                                    processarDificuldades(angular.copy(r.TestTypeItemLevel));
                                else
                                    ng.e1_qtdItens = contarItens();
                            }
                        }
                        ng.e1_listaPeriodosChecked = procurarElementoEm(ng.e1_listaPeriodosChecked, ng.e1_listaPeriodos);
                        ng.editMode = false;

                    }
                    else if (!ng.e1_formato_findTest) {
                        if (r.FormatType)
                            ng.e1_formato = r.FormatType;
                        else
                            ng.e1_formato = { Description: "Quantidade", Id: 1 };
                        if (ng.e1_formato.Description === "Porcentagem")
                            processarDificuldades(angular.copy(r.TestTypeItemLevel));
                        else
                            ng.e1_listaDificuldades = (angular.copy(r.TestTypeItemLevel));
                        ng.e1_qtdItens = contarItens();
                        ng.e1_inpQntItens = null;
                    }
                    ng.e1_formato_findTest = false;
                    ng.e1_cbBIB = (r.Bib && ng.temBIB) === true ? ng.e1_listaBIB[0] : ng.e1_listaBIB[1];
                    ng.e1_tipoNivelEnsino = angular.copy(r.TypeLevelEducation || 0);
                    ComponenteCurricularCarregar(ng.e1_tipoNivelEnsino);
                    dificuldadeRemoverZeros();
                    if (ng.e1_listaPeriodos.length === 0) {
                        $notification.alert('Este tipo de prova não possui ' + ng.curriculumGradeLabel + '(s) cadastrado(s).')
                    }
                } else {
                    if (r.type && r.message)
                        $notification[r.type ? r.type : 'error'](r.message);
                    ng.mostrarTela = true;
                    return false;
                }
            });
        };

        /**
         * @function Carrega Item Type
         * @param {Object} result
         * @public
         */
        ng.carregaItemType = carregaItemType;
        function carregaItemType(result) {
            ItemTypeModel.loadTestType({}, function (result) {
                if (result.success) {
                    ng.ItemTypeList = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
        * @function Tratamento para alterações de componente curricular
        * @private
        * @param
        */
        function nivelDesempenhoCarregar() {
            self.etapa1.niveis(nivelDesempenhoCarregado);
        };

        /**
        * @function Tratamento para alterações de componente curricular
        * @private
        * @param
        */
        function nivelDesempenhoCarregado(r) {
            if (r.success) {
                if (r.lista != undefined)
                    ng.e1_listaNiveis = angular.copy(r.lista);
                else ng.e1_listaNiveis = angular.copy(r.Lista);
            } else {
                if (r.type && r.message)
                    $notification[r.type ? r.type : 'error'](r.message);
                return false;
            }
        };

        ng.e1_inicioDownloadMudou = e1_inicioDownloadMudou;
        function e1_inicioDownloadMudou() {
            if (ng.mostrarTela)
                ng.alterouEtapaAtual = self.etapa1.alterou = true;
        };

        /**
        * @function Tratamento para alterações de componente curricular
        * @private
        * @param
        */
        ng.e1_nivelDesempenhoMudou = e1_nivelDesempenhoMudou;
        function e1_nivelDesempenhoMudou() {
            if (ng.mostrarTela)
                ng.alterouEtapaAtual = self.etapa1.alterou = true;
        };

        ng.selectNivelDesempenho = function () {
            ng.e1_cbNiveisDesempenho = !ng.e1_cbNiveisDesempenho;
            e1_nivelDesempenhoMudou();
            if (!ng.e1_cbNiveisDesempenho) {
                for (var i = 0; i < ng.e1_listaNiveis.length; i++) {
                    ng.e1_listaNiveis[i].Value1 = null;
                    ng.e1_listaNiveis[i].Value2 = null;
                }
            }

        };

        ng.selectProvaEletronica = function () {
            ng.isElectronicTest = !ng.isElectronicTest;
            self.etapa1.alterou = true;
        };
        ng.selectShowOnSerapEstudantes = function () {
            ng.showOnSerapEstudantes = !ng.showOnSerapEstudantes;
            ng.numberSynchronizedResponseItems = 2;
            self.etapa1.alterou = true;
        };

        ng.validaQtdItensSincronizacao = function () {

            self.etapa1.alterou = true;
        };

        ng.selectShowTestContext = function () {
            ng.showTestContext = !ng.showTestContext;
            self.etapa1.alterou = true;
        };

        ng.selectTemBIB = function () {
            ng.temBIB = !ng.temBIB;
            self.etapa1.alterou = true;
        };

        ng.selectShowVideoFiles = function () {
            ng.showVideoFiles = !ng.showVideoFiles;
            self.etapa1.alterou = true;
        };


        ng.selectAdvanceWithoutAnswering = function () {
            ng.advanceWithoutAnswering = !ng.advanceWithoutAnswering;
            self.etapa1.alterou = true;
        };


        ng.selectBackToPreviousItem = function () {
            ng.backToPreviousItem = !ng.backToPreviousItem;
            self.etapa1.alterou = true;
        };



        ng.selectShowAudioFiles = function () {
            ng.showAudioFiles = !ng.showAudioFiles;
            self.etapa1.alterou = true;
        };

        ng.selectShowTestTAI = function () {
            ng.showTestTAI = !ng.showTestTAI;

            if (!ng.showTestTAI && ng.e1_cbTipoProva.Bib)
                ng.showFlagBIB = true;
            else
                ng.showFlagBIB = false;

            if (ng.temBIB)
                ng.temBIB = false;
            self.etapa1.alterou = true;
        };

        ng.selectShowJustificate = function () {
            ng.showJustificate = !ng.showJustificate;
            self.etapa1.alterou = true;
        };

        ng.selectMultidiscipline = function () {
            ng.e1_cbComponenteCurricular = 0;
            ng.isMultidiscipline = !ng.isMultidiscipline;
            self.etapa1.alterou = true;
        };

        ng.selectKnowledgeAreaBlock = function () {
            ng.isKnowledgeAreaBlock = !ng.isKnowledgeAreaBlock;
            self.etapa1.alterou = true;
        };

        ng.validarND1 = validarND1;
        function validarND1(i) {
            validarTotalItens(ng.e1_listaNiveis[i], 'Value1');
            if (ng.mostrarTela)
                ng.alterouEtapaAtual = self.etapa1.alterou = true;
        };

        ng.validarND2 = validarND2;
        function validarND2(i) {
            validarTotalItens(ng.e1_listaNiveis[i], 'Value2');
            if (ng.mostrarTela)
                ng.alterouEtapaAtual = self.etapa1.alterou = true;
        };

        /**
         * @function Tratamento para alterações de componente frequência de aplicação
        * @private
        * @param
        */
        ng.changeFrequencyApplication = function __changeFrequencyApplication() {
            if (ng.mostrarTela)
                ng.alterouEtapaAtual = self.etapa1.alterou = true;
        };

        /**
        * @function Tratamento para alterações de componente curricular
        * @private
        * @param
        */
        ng.radioSelect = radioSelect;
        function radioSelect() {

            if (ng.e1_radios == 1) {
                ng.itensTotais = parseInt(ng.e1_qtdItens);

                ng.e1_inpQntItens = null;
            }
            else {
                if (ng.e1_formato.Description === "Porcentagem")
                    ng.e1_qtdItens = null;

                ng.itensTotais = parseInt(ng.e1_inpQntItens);
            }

            if (ng.mostrarTela && !ng.editMode)
                ng.alterouEtapaAtual = self.etapa1.alterou = true;

        };

        /**
        * @function Tratamento para lista de dificuldades, modela objeto para funcionar com porcentagem e quantidade
        * @private
        * @param
        */
        function processarDificuldades(lista, porcent) {

            var i = 0, m = lista.length, l = 0;

            if (m)
                for (i; i < m; i++) {
                    // Tratamento para TestItemLevel
                    if (lista[i].PercentValue !== undefined) {
                        if (lista[i].PercentValue > 0)
                            lista[i].Description += " (" + (lista[i].PercentValue) + "%)";
                    }// Tratamento para TestTypeItemLevel
                    else {
                        //Quando alterar tipo de prova
                        if (lista[i].Value > 0)
                            lista[i].Description += " (" + (lista[i].Value) + "%)";

                        if (!lista[i].PercentValue)
                            lista[i].PercentValue = 0;

                        lista[i].Value ^= lista[i].PercentValue;
                        lista[i].PercentValue ^= lista[i].Value;
                        lista[i].Value ^= lista[i].PercentValue;
                    }
                }

            ng.e1_listaDificuldades = lista;
        };

        /**
        * @function Trata qualquer alteração nos itens de dificuldade ( Quantidade itens sugeridos )
        * @private
        * @param
        */
        ng.dificuldadeMudou = dificuldadeMudou;
        function dificuldadeMudou(item, campo) {

            if (item && !campo) {
                contarItens(item);
                validarTotalItens(item);
            }

            else if (item && campo) {
                contarItens(item);
                item.inputWrong = "";
                validarTotalItens(item, 'Value');
            }

            if (ng.mostrarTela)
                ng.alterouEtapaAtual = self.etapa1.alterou = true;

        };

        /**
        * @function Conta a quantidade de itens por dificuldade
        * @private
        * @param
        */
        ng.contarItens = contarItens;
        function contarItens(edit) {

            var i = 0, w = 0;
            for (i; i < ng.e1_listaDificuldades.length; i++) {
                if (ng.e1_listaDificuldades[i].Value) {

                    if (ng.e1_listaDificuldades[i].Value === 0) {
                        ng.e1_listaDificuldades[i].Value = null;
                        continue;
                    }

                    w += parseInt(ng.e1_listaDificuldades[i].Value || ng.e1_listaDificuldades[i].Value.Description);
                }
            }

            ng.e1_qtdItens = w;

            if (ng.mostrarTela && !ng.editMode)
                ng.alterouEtapaAtual = self.etapa1.alterou = true;

            radioSelect();
            return w;
        };

        /**
        * @function Valida entrada de números
        * @private
        * @param
        */
        ng.validarTotalItens = validarTotalItens;
        function validarTotalItens(elm, prop) {
            if (elm && prop) {
                elm[prop] = elm[prop].replace(/[^0-9]/g, "");
            }
            else if (this[elm]) {
                this[elm] = this[elm].replace(/[^0-9]/g, "");
            }
        };

        ng.validarItensBlocos = function (value) {

            self.etapa1.alterou = true;
            //} else {
            //    $notification.alert("A quantidade de itens deve ser maior que 0.");
            //}
        };

        ng.validarBlocos = function (value) {
            self.etapa1.alterou = true;
        };

        /**
        * @function Distribui porcentagem do total de itens desejado
        * @private
        * @Lexx@param
        */
        ng.porcentagemDistribuir = porcentagemDistribuir;
        function porcentagemDistribuir(total) {
            total = ng.itensTotais = parseInt(ng.e1_qtdItens);

            var i = 0, m = ng.e1_listaDificuldades.length, l = ng.e1_listaDificuldades, str;

            for (i; i < m; i++) {

                if (l[i].PercentValue || l[i].PercentValue == 0) {

                    l[i].Value = parseFloat(((total || 0) * l[i].PercentValue * 0.01).toFixed(2));
                    str = ('' + l[i].Value);

                    if (str.indexOf(".") > -1) {

                        if (str.substring(str.indexOf("."), str.length) !== ".00")
                            l[i].inputWrong = "inputWrong";
                    }
                    else {
                        l[i].inputWrong = "";
                    }
                }
            }

            if (ng.mostrarTela && !ng.editMode)
                ng.alterouEtapaAtual = self.etapa1.alterou = true;


            dificuldadeRemoverZeros();
        };

        /**
        * @function Remove valores 0 da lista de dificuldade
        * @private
        * @param
        */
        function dificuldadeRemoverZeros() {

            var i = 0, m = ng.e1_listaDificuldades.length, l = ng.e1_listaDificuldades;

            for (i; i < m; i++) {
                if (!ng.e1_listaDificuldades[i].Value || l[i].Value == 0)
                    ng.e1_listaDificuldades[i].Value = null;
            }

        };

        /**
        * @function Executa conjunto de funções quando alterar 'e1_qtdItens'
        * @private
        * @param
        */
        ng.radio1Validar = radio1Validar;
        function radio1Validar() {
            validarTotalItens('e1_qtdItens');
            porcentagemDistribuir();
            radioSelect();
        };

        ng.aplicaDownloadInicio = aplicaDownloadInicio;
        function aplicaDownloadInicio() {
            $("#e1_inicioDownload").datepicker('show');
        };

        /**
        * @function Aplica valor do datapick no input
        * @private
        * @param
        */
        ng.aplicaAplicacaoInicio = aplicaAplicacaoInicio;
        function aplicaAplicacaoInicio() {
            $("#e1_aplicacaoInicio").datepicker('show');
        };

        /**
        * @function Aplica valor do datapick no input
        * @private
        * @param
        */
        ng.aplicaAplicacaoFinal = aplicaAplicacaoFinal;
        function aplicaAplicacaoFinal() {
            $("#e1_aplicacaoFinal").datepicker('show');
        };

        /**
        * @function Aplica valor do datapick no input
        * @private
        * @param
        */
        ng.aplicaCorrecaoInicio = aplicaCorrecaoInicio;
        function aplicaCorrecaoInicio() {
            $("#e1_correcaoInicio").datepicker('show');
        };

        /**
        * @function Aplica valor do datapick no input
        * @private
        * @param
        */
        ng.aplicaCorrecaoFinal = aplicaCorrecaoFinal;
        function aplicaCorrecaoFinal() {
            $("#e1_correcaoFinal").datepicker('show');
        };

        /**
        * @function Valida se os intervalos das datas estão corretos
        * @private
        * @param
        */
        ng.validarData = validarData;
        function validarData() {

            var
                a1 = new Date(ng.e1_aplicacao.Inicio),
                a2 = new Date(ng.e1_aplicacao.Final),
                c1 = new Date(ng.e1_correcao.Inicio),
                c2 = new Date(ng.e1_correcao.Final),
                q = (a1 && a2),
                w = (c1 && ng.e1_correcao.Final);

            if (ng.e1_aplicacao.Inicio === "Invalid Date") {
                $notification.alert("O campo '" + ng.labels.inicioAplicaco + "' contém uma data inválida.");
                return false
            }


            if (ng.e1_aplicacao.Final === "Invalid Date") {
                $notification.alert("O campo '" + ng.labels.finalAplicacao + "' contém uma data inválida.");
                return false
            }


            if (ng.e1_correcao.Inicio === "Invalid Date") {
                $notification.alert("O campo '" + ng.labels.inicioCorrecao + "' contém uma data inválida.");
                return false
            }


            if (ng.e1_correcao.Final === "Invalid Date") {
                $notification.alert("O campo '" + ng.labels.finalCorrecao + "' contém uma data inválida.");
                return false
            }


            if (!ng.e1_tempoDeProva) {
                $notification.alert("O campo '" + ng.labels.tempoDeProva + "' é obrigatório.");
                return false;
            }

            if (ng.showOnSerapEstudantes && !ng.e1_inicioDownload) {
                $notification.alert("O campo '" + ng.labels.inicioDownload + "' é obrigatório.");
                return false;
            }

            if (!ng.e1_aplicacao.Inicio) {
                $notification.alert("O campo '" + ng.labels.inicioAplicaco + "' é obrigatório.");
                return false;
            }

            if (!ng.e1_aplicacao.Final) {
                $notification.alert("O campo '" + ng.labels.finalAplicacao + "' é obrigatório.");
                return false;
            }


            if (!ng.e1_correcao.Inicio) {
                $notification.alert("O campo '" + ng.labels.inicioCorrecao + "' é obrigatório.");
                return false;
            }


            if (!ng.e1_correcao.Final) {
                $notification.alert("O campo '" + ng.labels.finalCorrecao + "' é obrigatório.");
                return false;
            }



            if (q) {
                if (a1 > a2) {
                    $notification.alert("O final da aplicação não pode começar antes do início da aplicação."); return false;
                }

                if (ng.e1_inicioDownload) {
                    if (new Date(ng.e1_aplicacao.Inicio) < new Date(ng.e1_inicioDownload)) {
                        $notification.alert("O iníco da aplicação não pode começar antes da data de início do download."); return false;
                    }
                }
            }


            if (w)
                if (new Date(ng.e1_correcao.Inicio) > new Date(ng.e1_correcao.Final)) {
                    $notification.alert("O final da correção não pode começar antes do início da correção."); return false;
                }

            if (q && w)
                if (
                    new Date(ng.e1_aplicacao.Final) > new Date(ng.e1_correcao.Final) ||
                    new Date(ng.e1_aplicacao.Inicio) > new Date(ng.e1_correcao.Inicio) ||
                    new Date(ng.e1_aplicacao.Final) > new Date(ng.e1_correcao.Inicio) ||
                    new Date(ng.e1_aplicacao.Inicio) > new Date(ng.e1_correcao.Final)
                ) {
                    $notification.alert("A correção não pode começar antes do final da aplicação."); return false;
                }

            return true;
        };

        /**
        * @function Valida se os intervalos dos níveis estão corretos
        * @private
        * @param item: elemento que esta sendo validado
        */
        function validarCampoDesempenho(item, valida) {

            if (typeof ng.e1_listaNiveis[item].Value1 === 'string') {
                ng.e1_listaNiveis[item].Value1 = ng.e1_listaNiveis[item].Value1.replace(/[^0-9]/g, "");
            }


            if (typeof ng.e1_listaNiveis[item].Value2 === 'string')
                ng.e1_listaNiveis[item].Value2 = ng.e1_listaNiveis[item].Value2.replace(/[^0-9]/g, "");

            if (valida)
                if (ng.e1_listaNiveis[item].Value1 > e1_listaNiveis[item].Value2) {
                    return $notification.error("O " + item.Description + " possui valores errados.");
                }
        };

        /**
        * @function Valida se os intervalos dos níveis estão corretos
        * @private
        * @param item: elemento que esta sendo validado
        */
        ng.e1_callModal = e1_callModal;
        function e1_callModal(id) {
            if (ng.e1_cbTipoProva.Block && !id) {
                angular.element("#modalTipoProva").modal({ backdrop: 'static' });
            }
            else {

                ng.e1_cbTipoProva.Block = ng.e1_cbTipoProva.Block === undefined ? false : !ng.e1_cbTipoProva.Block;
            }
        };

        ng.folhaResp_callModal = function _folhaResp_callModal() {
            angular.element("#modalGerarFolhaProva").modal({ backdrop: 'static' });
        };

        /**
        * @function Salva dados da Etapa 1
        * @private
        * @param 
        */
        function etapa1Salvar() {
            if (!self.etapa1.alterou)
                return $notification.alert('Não houve alteração na prova.');
            if (!ng.e1_formato)
                ng.e1_formato = { Description: "Quantidade", Id: 1 };
            var model = {
                "Id": ng.provaId,
                "Description": ng.e1_testDescription,
                "Password": ng.e1_testPassword,
                "TestType": ng.e1_cbTipoProva,
                "Discipline": ng.e1_cbComponenteCurricular,
                "Bib": ng.temBIB,
                "NumberItemsBlock": ng.temBIB ? parseInt(ng.e1_itensBlocos) : 0,
                "NumberBlock": ng.temBIB ? parseInt(ng.e1_qtdBlocos) : 0,
                "NumberItem": ng.e1_radios == 2 ? ng.itensTotais : 0,
                "FormatType": ng.e1_radios !== 2 ? ng.e1_formato : null,
                "ApplicationStartDate": ng.e1_aplicacao.Inicio,
                "ApplicationEndDate": ng.e1_aplicacao.Final,
                "DownloadStartDate": ng.e1_inicioDownload,
                "CorrectionStartDate": ng.e1_correcao.Inicio,
                "CorrectionEndDate": ng.e1_correcao.Final,
                "TestPerformanceLevels": ng.e1_cbNiveisDesempenho ? popularNiveis() : null,
                "TestItemLevels": ng.e1_radios == 1 ? validarDificuldade(ng.e1_listaDificuldades) : null,
                "UsuId": null,
                "FrequencyApplication": ng.frequencyApplication,
                "TestSituation": 0,
                "TestCurriculumGrades": validarPeriodos(),
                "Multidiscipline": ng.isMultidiscipline,
                "KnowledgeAreaBlock": ng.isKnowledgeAreaBlock,
                "ElectronicTest": ng.isElectronicTest,
                "ShowOnSerapEstudantes": ng.showOnSerapEstudantes,
                "NumberSynchronizedResponseItems": ng.numberSynchronizedResponseItems,
                "ShowVideoFiles": ng.showVideoFiles,
                "ShowTestContext": ng.showTestContext,
                "ShowAudioFiles": ng.showAudioFiles,
                "ShowJustificate": ng.showJustificate,
                "TestSubGroup": ng.e1_grupoSubgrupo,
                "TestTime": ng.e1_tempoDeProva,
                "TestContexts": ng.testContexts,
                "TestTAI": ng.showTestTAI,
                "NumberItemsAplicationTai": ng.e1_nItensTestTAI,
                "AdvanceWithoutAnswering": ng.advanceWithoutAnswering,
                "BackToPreviousItem": ng.backToPreviousItem
            };

            self.etapa1.save(model, etapa1Salvou);
        };

        /**
        * @function Tratamento para depous que Salvou dados da Etapa 1
        * @private
        * @param 
        */
        function etapa1Salvou(r) {

            if (r.success) {

                var param = {
                    Id: r.TestID
                };

                infoProva(param);

                if (ng.provaId === 0)
                    ng.provaId = r.TestID;

                ng.etapaAtual = 2;
                $notification.success('Prova salva com sucesso!');

                ng.alterouEtapaAtual = self.etapa1.alterou = false;
                ng.applicationActiveOrDone = r.ApplicationActiveOrDone;
                ng.situacao = procurarElementoEm([{ Id: r.TestSituation }], self.situacaoList)[0];

            } else {
                if (r.type && r.message)
                    $notification[r.type ? r.type : 'error'](r.message);
                return false;
            }
        };

        /**
        * @function Valida as periodos escolhidos( TestCurriculumGrades )
        * @private
        * @param 
        */
        function validarPeriodos() {

            var arr = [];
            for (var i = 0; i < ng.e1_listaPeriodosChecked.length; i++) {
                if (ng.e1_listaPeriodosChecked[i]) {
                    arr[i] = {};

                    arr[i].Id = ng.e1_listaPeriodosChecked[i].Id;

                    arr[i].TypeCurriculumGradeId = ng.e1_listaPeriodosChecked[i].Id;
                }
            }
            return arr;
        };

        /**
        * @function Valida as dificuldades( TestItemLevels )
        * @private
        * @param 
        */
        function validarDificuldade() {
            var arr = [];
            for (var i = 0; i < ng.e1_listaDificuldades.length; i++) {
                if (ng.e1_listaDificuldades[i]) {

                    arr[i] = {};

                    if (ng.editMode) {
                        arr[i].Id = ng.e1_listaDificuldades[i].Id;
                    }

                    arr[i].Value = parseFloat(ng.e1_listaDificuldades[i].Value) || null;

                    if (ng.e1_formato.Description === 'Porcentagem')
                        arr[i].PercentValue = ng.e1_listaDificuldades[i].PercentValue || null;

                    arr[i].ItemLevel = { Id: ng.e1_listaDificuldades[i].IdItem }
                }
            }
            return arr;
        };

        /**
        * @function Valida as porcentagem de itens
        * @private
        * @param 
        */
        function validarPorcentagemItens() {

            for (var i = 0; i < ng.e1_listaDificuldades.length; i++) {

                if (ng.e1_listaDificuldades[i].inputWrong === "inputWrong") {
                    return false;
                }

            }

            return true;
        };

        /**
        * @function Verifica as níveis de desempenho( TestPerformanceLevels )
        * @private
        * @param 
        */
        function validarNivelDesempenho() {
            var arr = [], a, c = 0, m = ng.e1_listaNiveis.length;

            for (var i = 0; i < m; i++) {

                a = (ng.e1_listaNiveis[i]);

                a.Value1 = parseInt(a.Value1);
                a.Value2 = parseInt(a.Value2);

                if (a.Value2 < a.Value1)
                    return 1; // final maior q inicial

                if (!parseInt(a.Value1) || !parseInt(a.Value2))
                    c++;
            }

            if (c >= m)
                return 2;// todos vazios


            return 0;
        };

        /**
        * @function Valida as níveis de desempenho( TestPerformanceLevels )
        * @private
        * @param 
        */
        function popularNiveis() {

            var arr = [];

            for (var i = 0; i < ng.e1_listaNiveis.length; i++) {
                if (ng.e1_listaNiveis[i]) {


                    arr[i] = {};


                    arr[i].Value1 = (ng.e1_listaNiveis[i].Value1) || null;
                    arr[i].Value2 = (ng.e1_listaNiveis[i].Value2) || null;

                    if (ng.e1_listaNiveis[i].PerformanceLevelId) {
                        arr[i].PerformanceLevel = { Id: ng.e1_listaNiveis[i].PerformanceLevelId }
                    }
                    else {
                        arr[i].PerformanceLevel = { Id: ng.e1_listaNiveis[i].Id }
                    }

                    arr[i].Id = ng.e1_listaNiveis[i].Id;
                }
            }

            return arr;
        };

        /**
        * @function valida dados da etapa 1
        * @private
        * @param item: elemento que esta sendo validado
        */
        function validarEtapa1() {

            if (!ng.e1_cbTipoProva) {
                $notification.alert('O campo "' + ng.labels.tipo + '" é obrigatório.');
                return false;
            }

            if (ng.showOnSerapEstudantes) {

                if (ng.numberSynchronizedResponseItems < 2) {
                    $notification.alert('O campo "' + ng.labels.numberSynchronizedResponseItems + '"deve ser maior ou igual a dois.');
                    return false;
                }
            }

            if (!ng.e1_testDescription) {
                $notification.alert('O campo "' + ng.labels.descricao + '" é obrigatório.');
                return false;
            }

            if (!ng.e1_cbComponenteCurricular && !ng.isMultidiscipline) {
                $notification.alert('O campo "' + ng.labels.componente + '" é obrigatório.');
                return false;
            }

            if (ng.showTestTAI && !ng.e1_nItensTestTAI) {
                $notification.alert('O campo "' + ng.labels.numberItemsTestTAI + '" é obrigatório.');
                return false;
            }

            if (!ng.frequencyApplication) {
                $notification.alert('O campo "' + ng.labels.frequencyApplication + '" é obrigatório.');
                return false;
            }

            if (ng.e1_listaPeriodos && ng.e1_listaPeriodos.length < 1) {
                $notification.alert('Este tipo de prova não possui ' + ng.curriculumGradeLabel + '(s) cadastrado(s).');
                return false;
            }

            if (ng.e1_listaPeriodosChecked && ng.e1_listaPeriodosChecked.length < 1) {
                $notification.alert('O campo "' + ng.labels.periodo + '" é obrigatório.');
                return false;
            }


            if (validarData() === false)
                return false;

            if (ng.temBIB && parseInt(ng.e1_itensBlocos) < 1) {
                $notification.alert('O campo "' + ng.labels.e1_itensBlocos + '" não pode ser menor ou igual a 0.');
                return false;
            }

            if (ng.e1_cbTipoProva.Block) {
                if (ng.temBIB && (ng.e1_cbTipoProva.BlockItem > ng.e1_itensBlocos)) {
                    $notification.alert('O campo "' + ng.labels.e1_itensBlocos + '" é menor que o total de itens já selecionados ( ' + ng.e1_cbTipoProva.BlockItem + ' ).');
                    return false;
                }

                if (!ng.temBIB && (ng.e1_cbTipoProva.BlockItem > ng.itensTotais)) {
                    $notification.alert('O campo "' + ng.labels.quantidadeItens + '" é menor que o total de itens já selecionados ( ' + ng.e1_cbTipoProva.BlockItem + ' ).');
                    return false;
                }

            }


            if (!ng.temBIB && !ng.showTestTAI && (ng.e1_radios == 3 || !ng.itensTotais)) {
                $notification.alert('O campo "' + ng.labels.quantidadeItens + '" é obrigatório.');
                return false;
            }

            if (!ng.temBIB && (ng.e1_radios == 1 && !validarPorcentagemItens())) {
                $notification.alert('No campo dificuldade do item não é permitido valor fracionado.');
                return false;
            }

            if (ng.temBIB)
                if (ng.e1_cbBIB) {

                    if (!ng.e1_qtdBlocos) {
                        $notification.alert('O campo "' + ng.labels.quantidadeBlocos + '" é obrigatório.');
                        return false;
                    }

                    if (!ng.e1_itensBlocos) {
                        $notification.alert('O campo "' + ng.labels.e1_itensBlocos + '" é obrigatório.');
                        return false;
                    }
                }


            if (ng.e1_cbNiveisDesempenho) {
                //	$notification.alert('O campo "' + ng.labels.niveis + '" é obrigatório.');

                var retorno = validarNivelDesempenho();

                if (retorno === 1)
                    //validar se tem algum elemento com valor
                    $notification.alert('o primeiro campo em "' + ng.labels.niveis + '" deve ser menor ou igual ao segundo campo.');

                if (retorno === 2)
                    //Validar se tem algum elemento com valor
                    $notification.alert('Preencha  os ' + ng.labels.niveis + '.');

                if (retorno) return false;
            }








            return true;
        };

        /**
        * @function valida dados da etapa 1
        * @private
        * @param elm = objeto a ser procurado 
        * @param arr = lista de objetos 
        */
        function procurarElementoEm(elm, arr) {

            var i;

            if (elm['length'] && elm['length'] > 0) {

                var a = [], q = 0;

                //Varre itens e array comparando Id, se for igual guarda num array a referencia do elemento do array
                for (i = 0; i < elm.length; i++) {
                    for (q = 0; q < arr.length; q++) {
                        if (elm[i] != null) {
                            if (elm[i].Id === arr[q].Id) {
                                a.push(arr[q]);
                                break;
                            }
                        }
                    }
                }
                if (a.length < 1)
                    return elm;
                return a;
            }
            else {

                if (elm['length'] && elm['length'] === 1)
                    elm = elm[0];
                i = arr.indexOf(elm);

                if (i < 0 || !i || i !== 0)
                    return null;

                return arr[i];
            }
        };

        function provaCarregar() {
            self.etapa1.prova(ng.params, function (r) {
                if (r.success) {
                    r = r.lista;
                    ng.params = r.Id;
                    ng.temBIB = r.Bib;
                    ng.showFlagBIB = r.Bib;
                    ng.e1_cbTipoProva = procurarElementoEm([r.TestType], ng.e1_listaTipoProva)[0];
                    ng.e1_grupoSubgrupo = procurarElementoEm([r.TestSubGroup], ng.grupoSubgrupoList)[0];
                    ng.e1_tempoDeProva = procurarElementoEm([r.TempoDeProva], ng.tempoDeProvaList)[0];
                    ng.Global = r.TestType.Global;
                    tipoProvaMudou();
                    ng.e1_cbTipoProva.Block = r.BlockItem > 0 || r.NumberBlock;
                    ng.e1_cbTipoProva.BlockItem = r.BlockItem || r.NumberItemsBlock;
                    ng.e1_itensBlocos = r.NumberItemsBlock;
                    ng.e1_qtdBlocos = r.NumberBlock;
                    ng.e1_testDescription = r.Description;
                    ng.e1_testPassword = r.Password;
                    ng.e1_cbComponenteCurricular = procurarElementoEm([r.Discipline], ng.e1_listaComponenteCurricular)[0];
                    ng.isMultidiscipline = r.Multidiscipline;
                    ng.isKnowledgeAreaBlock = r.KnowledgeAreaBlock;
                    ng.isElectronicTest = r.ElectronicTest;
                    ng.showOnSerapEstudantes = r.ShowOnSerapEstudantes;
                    ng.numberSynchronizedResponseItems = r.NumberSynchronizedResponseItems;
                    ng.showTestContext = r.ShowTestContext;
                    ng.showVideoFiles = r.ShowVideoFiles;
                    ng.showAudioFiles = r.ShowAudioFiles;
                    ng.showTestTAI = r.ShowTestTAI;
                    if (ng.showTestTAI) {
                        ng.e1_nItensTestTAI = procurarElementoEm([r.NumberItemsAplicationTai], ng.e1_nItensTestTAIList)[0];

                        if (ng.e1_nItensTestTAI) {                            
                            ng.advanceWithoutAnswering = r.AdvanceWithoutAnswering;
                            ng.backToPreviousItem = r.BackToPreviousItem;
                        }
                    }
                    ng.showJustificate = r.ShowJustificate;
                    e1_formato_findTest = true;
                    ng.e1_folhaResp = true;
                    ng.frequencyApplication = r.FrequencyApplication;
                    ng.e1_listaPeriodosChecked = r.TestCurriculumGrades;
                    ng.e1_inicioDownload = r.DownloadStartDate;

                    if (r.TestContexts.length > 0) {
                        ng.testContexts = r.TestContexts;
                    }

                    //Cronograma
                    ng.e1_aplicacao = {
                        Inicio: r.ApplicationStartDate,
                        Final: r.ApplicationEndDate
                    };
                    ng.applicationActiveOrDone = r.ApplicationActiveOrDone;
                    ng.e1_correcao = {
                        Inicio: r.CorrectionStartDate,
                        Final: r.CorrectionEndDate
                    };
                    ng.e1_formato = r.FormatType;
                    //Quantidade de itens
                    if (r.TestItemLevels.length > 0) {
                        ng.e1_radios = 1;
                        ng.e1_listaDificuldades = r.TestItemLevels;
                        ng.e1_qtdItens = contarItens();
                        if (ng.e1_formato) {
                            if (ng.e1_formato.Description === "Porcentagem") {
                                processarDificuldades(ng.e1_listaDificuldades);
                            }
                        }
                    }
                    else {
                        ng.e1_radios = 2;
                        ng.e1_inpQntItens = r.NumberItem;
                    }
                    ng.itensTotais = parseInt(ng.e1_qtdItens || ng.e1_inpQntItens || 0);

                    if (ng.temBIB) {
                        ng.itensTotais = parseInt(ng.e1_itensBlocos) * parseInt(ng.e1_qtdBlocos);
                    }
                    //BIB - Níveis de desempenho
                    if (r.TestPerformanceLevels.length > 0) {
                        ng.e1_cbNiveisDesempenho = true;
                        nivelDesempenhoCarregado({ Lista: r.TestPerformanceLevels, success: true });
                    }
                    else
                        ng.e1_cbNiveisDesempenho = false;
                    if (ng.navigation === 2)
                        initEtapa2();
                    else {
                        ng.mostrarTela = true;
                        ng.situacao = procurarElementoEm([{ Id: r.TestSituation }], self.situacaoList)[0];
                    }
                    ng.publicFeedback = r.PublicFeedback;
                } else {
                    if (r.type && r.message)
                        $notification[r.type ? r.type : 'error'](r.message);
                    delete ng.params;
                    ng.editMode = false;
                    ng.mostrarTela = true;
                    return;
                }
            });
        };

        function infoProva(params) {
            self.etapa1.prova(params, infoProvaGlobal)
        };

        function infoProvaGlobal(r) {
            ng.Global = r.lista.TestType.Global;
            ng.testContexts = r.lista.TestContexts;
        };

        function initModalAdicao() {
            ng.e2_blockAtual;

            // Filtro
            ng.e2_CodigoItem;

            ng.e2_ListaComboBox;
            ng.e2_ListaComboBoxSelected = [];

            // Nivel de ensino
            ng.e2_NivelEnsino;
            ng.e2_ListaNivelEnsino;
            // Modalidade
            ng.e2_Modalidade;
            ng.e2_ListaModalidade;
            ng.esconderNivelEnsinoModalidade;


            // Situação
            //ng.e2_ListaSituacao = [{ Description: 'Ativo' }, { Description: 'Inativo' }, ];
            //ng.e2_ListaSituacaoChecked = [];

            // Periodo
            ng.e2_ListaPeriodo;
            ng.e2_ListaPeriodoChecked = [];

            // Dificuldade
            ng.e2_ListaDificuldade;
            ng.e2_ListaDificuldadeChecked = [];

            // Proficiencia
            ng.e2_Proficiencia = {
                Inicio: null,
                Final: null,
            };

            ng.keyWords = [];


            // Busca
            ng.e2_BtnPesquisar;

            ng.e2_ResultadoBusca = [];

            ng.e2_ListaItemSelecionados = [];

            ng.e2_ListaKnowledgeAreaSelecionadas = [];

            ng.e2_ListaItemVersoes = [];

            self.etapa2.selecionados = [];

            //Deve guardar qual elemento foi 'checkado' com base na paginação das busca ( analisar desemepnho )
            ng.e2_ListaItemCheckedCache = [];


            // Paginação
            ng.paginate = $pager(self.etapa2.paginacao);
            ng.e2_Pagina = 0;
            ng.e2_TotalPaginas = 0;
            ng.e2_PageSize = 10;
            ng.paginate.indexPage(0);
        }

        /**
        * @function Configura variaveis do escopo, globais e locais da ETAPA 2
        * @private
        * @param
        */
        function initEtapa2() {

            ng.escondeModal = false;
            ng.e2_matrizAvaliacao = null;
            ng.e2_matrizAvaliacaoList = [];
            ng.e2_listAnoItensTai = [];
            ng.e2_dadosModalAnoItensAmostraTai = null;
            ng.e2_itemParaDeletarDaListaAnosItensAmostraProvaTai = '';
            ng.e2_itemParaAlterarDaListaAnosItensAmostraProvaTai = null;

            ng.e2_listaComponenteCurricular = [];
            ng.e2_cbComponenteCurricular

            // ETAPA 2
            blocosCarregar();

            ng.e2_ItensAtuais = 0;
            ng.e2_Navegacao = 1;
            /////////// FIM ETAPA 2


            // MODAL DE ADIÇÃO
            initModalAdicao();
            /////////////////// FIM  MODAL DE ADIÇÃO


            // LISTAGEM DE ITENS
            ng.e2_ListaModal;
            //////////////////// FIM LISTAGEM DE ITENS

            createView();

            nivelEnsinoCarregar();

            carregaModality();

            //TAI
            var $matrizAvaliacao = $(".matrizAvaliacao");
            $matrizAvaliacao.select2();

            e2_ComponenteCurricularCarregar(ng.e1_tipoNivelEnsino);
            carregaListAnoItensTai();

            self.etapa2.carregou = true;
        };

        ng.e2_matrizAvaliacaoMudou = e2_matrizAvaliacaoMudou;
        function e2_matrizAvaliacaoMudou() {
            if (ng.mostrarTela) ng.alterouEtapaAtual = self.etapa2.alterou = true;

            if (!ng.e2_matrizAvaliacao)
                return;
        };

        ng.e2_anoItensAmostraTaiMudou = e2_anoItensAmostraTaiMudou;
        function e2_anoItensAmostraTaiMudou() {

            if (ng.mostrarTela) ng.alterouEtapaAtual = self.etapa2.alterou = true;

            if (!ng.e2_dadosModalAnoItensAmostraTai.Ano)
                return;
        };

        /**
        * @function Carrega caderno com blocos
        * @private
        * @param
        */
        function blocosCarregar() {
            self.etapa2.blocos({ Id: ng.provaId }, blocosCarregado);
        };

        /**
        * @function Tratamento para dados do caderno
        * @private
        * @param r = resposta do servidor
        */
        function blocosCarregado(r) {

            if (r.success === false) {

                // Bloco contem ID - Description - ItensCount

                ng.cadernos = [];
                if (ng.temBIB) {
                    for (var b = 1; b <= ng.e1_qtdBlocos; b++) {
                        ng.cadernos.push({
                            Description: b,
                            ItensCount: 0,
                            Id: 0,
                            Total: ng.e1_itensBlocos,
                            Resto: ng.e1_itensBlocos,
                            SelectedItens: [],
                            QtdeKnowledgeArea: 0
                        });
                    }
                } else {
                    ng.cadernos = [{
                        Description: 'A',
                        ItensCount: 0,
                        Id: 0,
                        Total: ng.itensTotais,
                        Resto: ng.itensTotais,
                        SelectedItens: [],
                        QtdeKnowledgeArea: 0
                    }];
                }


                ng.e2_blockAtual = ng.cadernos[0];
            }
            else {
                r = angular.copy(r.lista);

                ng.cadernos = angular.copy(r);
                var bloco;
                for (var q = 0; q < ng.cadernos.length; q++) {
                    bloco = ng.cadernos[q];

                    if (!ng.temBIB)
                        bloco.Total = ng.itensTotais;
                    else
                        bloco.Total = ng.e1_itensBlocos;

                    bloco.Resto = bloco.Total - bloco.ItensCount;
                }

                //Como nesta fase não teremos BIB havera somente 1 carderno
                if (!ng.temBIB) {
                    ng.e2_blockAtual = ng.cadernos[0];
                } else {
                    if (ng.cadernos.length < ng.e1_qtdBlocos) {
                        const cadernos = [];
                        for (var b = 1; b <= ng.e1_qtdBlocos; b++) {
                            cadernos.push({
                                Description: String(b),
                                ItensCount: 0,
                                Id: 0,
                                Total: ng.e1_itensBlocos,
                                Resto: ng.e1_itensBlocos,
                                SelectedItens: [],
                                QtdeKnowledgeArea: 0
                            });
                        }
                        const cadernosSemIds = cadernos.filter(caderno => {
                            const temCadernoIdIgual = ng.cadernos.find(cad => cad.Description === caderno.Description);
                            if (temCadernoIdIgual) {
                                return false;
                            }
                            return true;
                        });
                        ng.cadernos = cadernosSemIds.concat(ng.cadernos);
                        // ORDENAR CADERNOS!
                        const indice = 'Description';
                        const ordenar = (a, b) => {
                            return a[indice] - b[indice];
                        };
                        ng.cadernos.sort(ordenar);
                    }
                }
            }

            contarItensSelecionados();

            if (ng.navigation === ng.ultimo)
                initEtapa4();
            else
                ng.mostrarTela = true;
        };

        /**
        * @function Carregar dados do nivel de ensino
        * @private
        * @param
        */
        function nivelEnsinoCarregar() {
            TestModel.loadLevelEducation({}, nivelEnsinoCarregado);
        };

        function carregaModality() {
            ng.e2_Modalidade = null;
            ModalityModel.load({}, function (result) {
                if (result.success) {
                    ng.e2_ListaModalidade = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        function carregaListAnoItensTai() {
            ng.e2_listAnoItensTai = ng.e1_listaPeriodos;
        }

        /**
        * @function Tratamentos resultado do nivel de ensino
        * @private
        * @param r = resposta do servidor
        */
        function nivelEnsinoCarregado(r) {

            if (r.success) {
                ng.e2_ListaNivelEnsino = angular.copy(r.lista);

                //ng.e2_NivelEnsino = ng.e2_ListaNivelEnsino[ng.e1_tipoNivelEnsino];
                ng.e2_NivelEnsino = ng.e2_ListaNivelEnsino[1];
            } else {
                if (r.type && r.message)
                    $notification[r.type ? r.type : 'error'](r.message);
                return false;
            }
        };

        /**
        * @function Carrega todos os itens selecionados
        * @private
        * @param
        */

        var itensCache = [];
        var page = 0;
        var pageItens = 10;

        function e2_itensCarregar() {
            addEventkeyUp();
            if (ng.e2_blockAtual.Id) {
                inicarCarregamentoDosItensPaginados();
            }
            //self.etapa2.itensBloco({ Id: ng.e2_blockAtual.Id }, e2_itensCarregado);
        };

        function inicarCarregamentoDosItensPaginados() {
            page = 0;
            pageItens = 10;
            itensCache = [];

            // debugger;
            carregarItensPorPagina();
        };

        function carregarItensPorPagina() {
            self.etapa2.itensBloco({ Id: ng.e2_blockAtual.Id, page, pageItens }, validarResultado);
        }

        function validarResultado(result) {
            if (!result || !result.success) {
                $notification.alert('Não há itens carregados');
                return;
            }

            if (result.lista instanceof Array) {
                if (result.lista <= 0) {
                    e2_itensCarregado(itensCache);
                }
                else {
                    itensCache = itensCache.concat(result.lista);
                    page++;
                    carregarItensPorPagina();
                }
            }
            else {
                e2_itensCarregado(itensCache);
            }
        };

        /**
        * @function Carrega todos as áreas de conhecimento do bloco.
        * @private
        * @param
        */
        function e2_knowledgeAreasCarregar() {
            addEventkeyUp();
            if (ng.e2_blockAtual.Id)
                self.etapa2.blockKnowledgeAreas({ Id: ng.e2_blockAtual.Id }, e2_knowledgeAreasCarregado);

        };


        /**
        * @function Tratamento para todos itens selecionados
        * @private
        * @param r = resposta do servidor
        */
        function e2_itensCarregado(lista) {

            if (lista instanceof Array && lista.length > 0) {
                self.etapa2.selecionados = angular.copy(lista);
                ng.e2_ListaItemSelecionados = ng.e2_blockAtual.SelectedItens = angular.copy(lista);
                ng.e2_blockAtual.QtdeKnowledgeArea = calculaQtdeKnowledgeArea(ng.e2_blockAtual.SelectedItens);
                e2_ResultadoSelecionados();
            }
            else {
                self.etapa2.selecionados = [];
                ng.e2_ListaItemSelecionados = ng.e2_blockAtual.SelectedItens = [];
                ng.e2_blockAtual.QtdeKnowledgeArea = 0;
                e2_ResultadoSelecionados();
            }
        };

        /**
        * @function Tratamento para todas as áreas de conhecimento selecionadas
        * @private
        * @param r = resposta do servidor
        */
        function e2_knowledgeAreasCarregado(r) {

            if (!r || !r.success) {
                $notification.alert('Não há área de conhecimento carregadas');
                return false;
            }

            if (r.lista instanceof Array) {
                self.etapa2.knowledgeAreasSelecionadas = angular.copy(r.lista);
                ng.e2_ListaKnowledgeAreaSelecionadas = angular.copy(r.lista);
            }
            else {
                self.etapa2.knowledgeAreasSelecionadas = [];
                ng.e2_ListaKnowledgeAreaSelecionadas = [];
            }
        };

        function contarItensSelecionados() {
            var i = 0, bloco, e;

            if (ng.cadernos)
                for (var q = 0; q < ng.cadernos.length; q++) {
                    bloco = ng.cadernos[q];
                    i += (bloco.ItensCount);
                }

            if (ng.temBIB) {

                let cadMaiorItens = 0;
                if (ng.cadernos.length) {
                    ng.cadernos.forEach(cad => {
                        if (cad.ItensCount > cadMaiorItens) {
                            cadMaiorItens = cad.ItensCount;
                        }
                    })
                }
                ng.e1_cbTipoProva.BlockItem = cadMaiorItens;
            } else {
                ng.e2_ItensAtuais = i;
                ng.e1_cbTipoProva.BlockItem = i;
            }
        };

        /**
        * @function Carrega todos os itens selecionados
        * @private
        * @param id = Id do item a ser deletado
        */
        ng.e2_itemDeletar = e2_itemDeletar;
        function e2_itemDeletar(item) {

            return;
            if (!ng.e2_ListaModal)
                return;

            e2_CheckedItem(item);

            //self.etapa2.remover({ BlockId: ng.e2_blockAtual.Id, ItemId: item.Id }, e2_itemDeletado);
        };

        /**
        * @function Carrega todos os itens selecionados
        * @private
        * @param r = resposta do servidor
        */
        function e2_itemDeletado(r) {

            if (r.success) {
                $notification.success('O item foi removido com sucesso.');
            }
            else {
                if (r.type && r.message)
                    $notification[r.type ? r.type : 'error'](r.message);
            }
        };

        ng.filtrosCarregar = filtrosCarregar;

        /**
        * @function Carrega dados dos filtros
        * @private
        * @param r = resposta do servidor
        */
        function filtrosCarregar() {

            if (!ng.e2_showFiltrosDificuldade)
                TestModel.loadLevels({}, filtrosDificuldadeCarregado);

            if (ng.e2_NivelEnsino && ng.e2_Modalidade)
                self.etapa2.filtroPeriodos({ LevelEducationID: ng.e2_NivelEnsino.Id, Modality: ng.e2_Modalidade.Id }, filtroPeriodosCarregado);

            if (ng.e1_cbComponenteCurricular)
                matrizCarregar(ng.e1_cbComponenteCurricular.Id)

            if (ng.isMultidiscipline) {
                ng.e2_showFiltrosMatriz = true;
                exibirModal();
            }
        };

        /**
        * @function Carrega dados dos filtros de dificuldade
        * @private
        * @param r = resposta do servidor
        */
        function filtrosDificuldadeCarregado(r) {

            ng.e2_ListaDificuldade = r.lista;

            ng.e2_showFiltrosDificuldade = true;

            exibirModal();
        };

        /**
        * @function Carrega dados dos filtros de periodo
        * @private
        * @param r = resposta do servidor
        */
        function filtroPeriodosCarregado(r) {

            if (r.success) {
                ng.e2_ListaPeriodoChecked = [];
                ng.e2_ListaPeriodo = r.lista;
                exibirModal();
            } else {
                if (r.type && r.message)
                    $notification[r.type ? r.type : 'error'](r.message);
                return false;
            }
        };

        /**
        * @function Tratamento para quando qualquer filtro se alterar
        * @private
        * @param r = resposta do servidor
        */
        ng.filtrosMudou = filtrosMudou;
        function filtrosMudou(filtro) {

            if (!filtro) {
                return;
            }

            if (!filtro.tipo) {
                if (filtro.lista[0].Description) {
                    filtro.tipo = 0;
                }
                else if (filtro.lista[0].ModelSkillLevels) {
                    filtro.tipo = 1;
                }
                else if (filtro.lista[0].Skills) {
                    filtro.tipo = 2;
                }

                //return;
            }


            e2_limpaCombos(filtro.tipo + 1);

            if (!filtro.model) {
                return;
            }

            if (!filtro.lista || filtro.lista.length < 1) {
                return;
            }


            if (filtro.tipo === 0) {
                matrizDisciplinaCarregar(filtro, filtro.tipo);
            }
            else if (filtro.tipo > 0) {
                habilidadesCarregar(filtro, filtro.tipo);
            }



        };

        /**
        * @function Carrega matriz
        * @private
        * @param 
        */
        function matrizCarregar(id) {
            TestModel.getComboByDiscipline({ Id: id }, matrizCarregada);
        };

        /**
        * @function Tratamento de dados para matriz 
        * @private
        * @param 
        */
        function matrizCarregada(r) {

            if (r.success) {
                ng.e2_ListaComboBox = [];

                ng.e2_ListaComboBox[0] = ({ tipo: 0, model: null, lista: r.lista, Description: ng.labels.matriz });

                ng.e2_showFiltrosMatriz = true;

                exibirModal();
            } else {

                ng.e2_ListaComboBox = [];

                if (r.type && r.message)
                    $notification[r.type ? r.type : 'error'](r.message);
                return false;
            }
        };

        /**
        * @function Carrega e popula a habilidades segundo modelo
        * @private
        * @param 
        */
        function matrizDisciplinaCarregar(f, index) {
            TestModel.getByMatriz({ Id: f.model.Id }, function (r) {
                if (r.success) {
                    var arr = angular.copy(r.lista), lista;
                    for (var i = 0; i < arr.length; i++) {
                        lista = arr[i].ModelSkillLevels;
                        ng.e2_ListaComboBox[1 + i] = ({ tipo: 1 + i, model: null, lista: lista.Skills, Description: lista.Description });
                    }
                } else {
                    if (r.type && r.message)
                        $notification[r.type ? r.type : 'error'](r.message);
                    return false;
                }
            });
        };

        /**
        * @function Carrega e popula as habilidade
        * @private
        * @param 
        */
        function habilidadesCarregar(f, index) {
            if (f.model.LastLevel) return;
            TestModel.getByParent({ Id: f.model.Id }, function habilidadesCarregada(r) {
                if (r.success) {
                    var lista = angular.copy(r.lista);
                    index = index + 1;
                    lista.forEach(reduzLista);
                    ng.e2_ListaComboBox[index].lista = lista;
                } else {
                    if (r.type && r.message)
                        $notification[r.type ? r.type : 'error'](r.message);
                    return false;
                }
            });
        };

        /**
        * @function Faz uma limpeza dos combo altera uma combo
        * @private
        * @param 
        */
        function e2_limpaCombos(i) {
            var w = ng.e2_ListaComboBox;
            for (var q = i; q < w.length; q++) {
                if (w[q]) {
                    w[q].model = null;
                    w[q].lista = null;
                }
            }
        };

        /**
        * @function Reduz uma camada do objeto
        * @private
        * @param 
        */
        function reduzLista(elmt, idx, ary) {
            for (var str in elmt) {
                ary[idx] = ary[idx][str];
            }
        };

        /**
        * @function Pesquisa um conjunto de filtros
        * @private
        * @param 
        */
        ng.e2_pesquisarFiltros = pesquisarFiltros;
        function pesquisarFiltros() {


            ng.paginate.indexPage(0);
            ng.e2_PageSize = ng.paginate.getPageSize();

            var obj = {};

            if (!ng.e1_cbComponenteCurricular)
                return $notification.alert('Selecione um componente curricular.');

            if (ng.e2_Modalidade && (!ng.e2_ListaPeriodoChecked || ng.e2_ListaPeriodoChecked.length <= 0))
                return $notification.alert('Selecione ao menos um ' + ng.curriculumGradeLabel + ' da modalidade escolhida.');

            if (ng.e2_Proficiencia.Inicio || ng.e2_Proficiencia.Final) {

                if (ng.e2_Proficiencia.Inicio) {

                    ng.e2_Proficiencia.Inicio = parseInt(ng.e2_Proficiencia.Inicio);

                    if (ng.e2_Proficiencia.Inicio > 99 && ng.e2_Proficiencia.Inicio < 501)
                        obj.ProficiencyStart = ng.e2_Proficiencia.Inicio;
                    else {
                        return $notification.alert('A "Proficiência" deve conter valores entre 100 e 500');
                    }

                }

                if (ng.e2_Proficiencia.Final) {
                    ng.e2_Proficiencia.Final = parseInt(ng.e2_Proficiencia.Final);

                    if (ng.e2_Proficiencia.Inicio < ng.e2_Proficiencia.Final && ng.e2_Proficiencia.Final < 501) {
                        obj.ProficiencyEnd = ng.e2_Proficiencia.Final;
                    } else {
                        if (ng.e2_Proficiencia.Inicio)
                            if (ng.e2_Proficiencia.Inicio > ng.e2_Proficiencia.Final)
                                return $notification.alert('O valor final da "Proficiência" deve ser maior que o inicial.');
                        return $notification.alert('A "Proficiência" deve conter valores entre 100 e 500');
                    }
                }
            }


            if (ng.keyWords.length > 0)
                obj.Keywords = joinKeywords(ng.keyWords);


            if (ng.e2_ListaPeriodoChecked.length > 0)
                obj.TypeCurriculumGrades = validarFiltrosChecked(ng.e2_ListaPeriodoChecked, ng.e2_ListaPeriodo);


            if (ng.e2_ListaDificuldadeChecked.length > 0)
                obj.ItemLevelIds = validarFiltrosChecked(ng.e2_ListaDificuldadeChecked, ng.e2_ListaDificuldade);



            ///COMBOS 
            if (ng.e1_cbComponenteCurricular)
                obj.DisciplineId = ng.e1_cbComponenteCurricular.Id;


            if (ng.e2_ListaComboBox) {

                if (ng.e2_ListaComboBox[0] && ng.e2_ListaComboBox[0].model)
                    obj.EvaluationMatrixId = ng.e2_ListaComboBox[0].model.Id;

                if (ng.e2_ListaComboBox.length > 2) {

                    var ultimo;
                    for (var i = 1, q = ng.e2_ListaComboBox.length; i <= q; i++) {
                        if ((!ng.e2_ListaComboBox[i] || ng.e2_ListaComboBox[i].model == null) && i > 1) {
                            ultimo = ng.e2_ListaComboBox[i - 1].model;
                            break;
                        }
                    }

                    if (ultimo) {
                        obj.SkillId = ultimo.Id;
                        obj.SkillLastLevel = ultimo.LastLevel;
                    }
                }
            }


            if (ng.e2_codigoItem) {
                obj.ItemCode = ng.e2_codigoItem;
            }

            obj.ItemType = ng.ItemType ? ng.ItemType.Id : undefined;
            obj.Global = ng.Global;

            ng.e2_filtros = obj;

            ng.paginate.paginate(obj).then(e2_resultadoFiltrosCarregado, e2_resultadoFiltrosCarregado);
        };

        /**
       * @function Concatena palavras pára filtro
       * @private
       * @param 
       */
        function joinKeywords(_tags) {

            var q = 0, e = _tags.length, arr = "";

            for (q; q < e; q++) {
                arr += "" + (_tags[q].text) + ",";
            }
            arr = arr.substr(0, arr.length - 1);
            if (arr.length)
                return arr;
            return arr;
        };

        /**
        * @function Retira todos os ID utilizados da lista
        * @private
        * @param 
        */
        function validarFiltrosChecked(checkedList, list) {
            var q = 0, e = checkedList.length, arr = "";

            for (q; q < e; q++) {
                if (checkedList[q])
                    arr += "" + (list[q].Id) + ",";
            }
            arr = arr.substr(0, arr.length - 1);
            if (arr.length)
                return arr;
            return undefined;
        };

        /**
        * @function Carrega resultado dos filtros de busca
        * @private
        * @param i = pagina a ser carregada
        */
        ng.e2_resultadoFiltrosCarregar = e2_resultadoFiltrosCarregar;
        function e2_resultadoFiltrosCarregar(i) {
            //configurar filtros de busca aqui
            ng.paginate.paginate(ng.e2_filtros).then(e2_resultadoFiltrosCarregado);
        };

        /**
        * @function Tratamento para dados do caderno
        * @private
        * @param r = resposta do servidor
        */
        function e2_resultadoFiltrosCarregado(r) {

            if (!r || !r.success) {

                ng.e2_ResultadoBusca = [];

                ng.e2_Pagina = 0;
                ng.e2_TotalPaginas = 0;
                ng.paginate.indexPage(0);
                ng.e2_PageSize = ng.paginate.getPageSize();
                return false;
            }

            ng.e2_ResultadoBusca = angular.copy(r.lista);

            e2_ResultadoSelecionados();

            ng.e2_Pagina = ng.paginate.totalPages();

            ng.e2_TotalPaginas = ng.paginate.totalItens();


        };

        /**
        * @function Validar elementos que já foram selecionados pelo usuário
        * @private
        * @param 
        */
        function e2_ResultadoSelecionados() {

            var a = ng.e2_ListaItemSelecionados, s = ng.e2_ResultadoBusca, q = 0, w = a.length, e, r = s.length, total = 10;

            //Lista selecionados
            for (q; q < w; q++) {

                //Lista de resultados
                for (e = 0; e < r; e++) {

                    if (a[q].Id === s[e].Id) {
                        a[q].check = true;
                        s[e].check = true;
                        a[q] = s[e];
                        total--;
                        if (total === 0) break;
                    }
                }
            }

            ng.e2_ListaItemSelecionados = ng.e2_blockAtual.SelectedItens;
            ng.e2_blockAtual.QtdeKnowledgeArea = calculaQtdeKnowledgeArea(ng.e2_blockAtual.SelectedItens);

            contarItensSelecionados();
        };

        /**
         * @function Callback para visualizar
         * @private
         * @param {String} _id ID do item a ter seu texto base referênciado.
         */
        ng.e2_itemVisualizar = e2_itemVisualizar;
        function e2_itemVisualizar(item) {

            e2_avancarModal(5);
            var id = item.Id;

            ng.e2_itemVisualizado = (id);

            e2_itemVisualizarModal(id);
        };

        /**
         * @function Callback para visualizar
         * @private
         * @param {String} _id ID do item a ter seu texto base referênciado.
         */
        ng.openModelVersions = openModelVersions;
        function openModelVersions(item) {

            e2_avancarModal(6);
            var id = item.Id;

        };

        /**
         * @function Callback para visualizar
         * @private
         * @param {String} _id ID do item a ter seu texto base referênciado.
         */
        function e2_itemVisualizarModal(_id) {

            createView();

            ng.e2_itemTextoBaseCarregado = false;
            ng.e2_itemDadosCarregado = false;

            textoBaseCarregar(_id)
            itemByTextoBaseCarregar(_id);

        };

        /**
         * @function Criar um view para exibição do item completo
         * @public
         * @param
         */
        ng.createView = createView;
        function createView() {
            // armazena o item formatado para exibição
            ng.view = {
                item: {
                    ItemCode: undefined,
                    Situation: undefined,
                    Discipline: undefined,
                    Matriz: undefined,
                    Skills: undefined,
                    CurriculumGrade: undefined,
                    Proficiency: undefined,
                    Tips: undefined,
                    ItemType: undefined,
                    Keywords: undefined,
                    ItemLevel: undefined,
                    Versions: undefined,
                    TRI: undefined,
                    Sentence: undefined,
                    TextBase: undefined,
                    Statement: {
                        Descritpion: undefined
                    },
                    Alternatives: undefined
                }
            };

            // itens visualizados na modal
            ng.abstract = {
                textbase: false,
                navigation: 0,
                currentID: undefined,
                item: undefined,
                listaItensID: []
            };

        };

        /**
         * @function Carregar texto base e IDs de itens que o possuem
         * @public
         * @param
         */
        function textoBaseCarregar(_id) {
            TestModel.getBaseTextItems({ itemId: _id }, function (r) {
                if (r.success) {
                    ng.abstract.navigation = 0;
                    ng.abstract.textbase = (r.lista.Description == 'O Item não possui Texto base.') ? (false) : (angular.copy(r.lista.Description));
                    ng.abstract.statement = r.lista.Description != null ? r.lista.Description.Statement : "";
                    ng.abstract.listaItensID = (r.lista.Items.length > 0) ? r.lista.Items : [r.lista.Items];
                    ng.abstract.currentID = ng.abstract.listaItensID[0];
                    ng.e2_itemTextoBaseCarregado = true;

                } else {
                    if (r.type && r.message)
                        $notification[r.type ? r.type : 'error'](r.message);
                }
            });
        };

        /**
         * @function Carregar item exibido na modal de visualização do texto base
         * @private
         * @param
         */
        function itemByTextoBaseCarregar(_id) {
            TestModel.getItemSummaryById({ itemId: _id }, function (r) {
                if (r.success) {
                    ng.abstract.item = r.lista;
                    ng.abstract.currentID = r.lista.Id;
                    ng.e2_itemDadosCarregado = true;
                }
                else {
                    if (r.type && r.message)
                        $notification[r.type ? r.type : 'error'](r.message);
                }
            });
        };

        /**
         * @function 
         * @public
         * @param {string} _html
         */
        ng.minimize = minimize;
        function minimize(_text, _length) {

            if (_text == null || _text == undefined)
                return "";

            if (_text.length > _length)
                _text = _text.substring(0, _length) + "...";

            return _text;
        };

        /**
         * @function Valida se já é possivel exibir modal
         * @public
         */
        function exibirModal() {
            var b = true;

            if (!ng.e2_showFiltrosDificuldade) {
                b = false;
            }
            //else if (!ng.e2_showFiltrosPeriodos) {
            //b = false;
            //}
            //else if (!ng.e2_showFiltrosSituacao) {
            //    b = false;
            //}
            else if (!ng.e2_showFiltrosMatriz) {
                b = false;
            }

            ng.escondeModal = b;
        };

        /**
        * @function Tratamento para dados do caderno
        * @private
        * @param id = qual id deve ser chamado
        */
        ng.e2_callModal = e2_callModal;
        function e2_callModal(id, block) {

            initModalAdicao();

            ng.e2_Navegacao = id;

            if (block) {
                ng.e2_blockAtual = block;
            }

            if (id === 2) {

                e2_itensCarregar();
                ng.e2_ListaModal = true;

            }
            else if (id === 1) {

                if (ng.e2_ResultadoBusca)
                    if (ng.e2_ResultadoBusca.length < 1) {

                        ng.e2_ListaPeriodoChecked = [];
                        ng.e2_Modalidade = null;

                        e2_itensCarregar();
                        filtrosCarregar();

                        if (!ng.isMultidiscipline)
                            pesquisarFiltros();
                    }

                ng.e2_ListaModal = false;
            }
            else if (id === 7) {
                e2_knowledgeAreasCarregar();
                ng.e2_ListaModal = true;
            }

            angular.element("#modal").modal({ backdrop: 'static' });
        };

        ng.e2_cadernoExcluido = e2_cadernoExcluido;
        function e2_cadernoExcluido(r) {
            if (r.success) {
                var index = ng.cadernos.indexOf(ng.e2_blockAtual);

                $notification.success('Caderno excluído com sucesso');
                ng.cadernos.splice(index, 1);
            }
            else {
                if (r.type && r.message)
                    $notification[r.type ? r.type : 'error'](r.message);
            }

        }

        ng.e2_callModalRemoverItensSelecionados = e2_callModalRemoverItensSelecionados;
        function e2_callModalRemoverItensSelecionados(block) {

            if (block) {
                ng.e2_blockAtual = block;
            }

            angular.element("#modalRemoverItensSelecionados").modal({ backdrop: 'static' });
        };


        ng.e2_excluirCaderno = e2_excluirCaderno;
        function e2_excluirCaderno() {
            if (ng.e2_blockAtual.Id > 0) {
                self.etapa2.remover({ Id: ng.e2_blockAtual.Id }, e2_cadernoExcluido);
            } else {
                var index = ng.cadernos.indexOf(ng.e2_blockAtual);
                ng.cadernos.splice(index, 1);
                $notification.success('A prova foi salva com sucesso!');
            }
        }


        /**
        * @function Avançar para lista
        * @private
        * @param
        */
        ng.e2_AvancarModal = e2_avancarModal
        function e2_avancarModal(index) {

            ng.modalAnterior = ng.e2_Navegacao;

            if (index) {
                ng.e2_Navegacao = index;
            }
            else if (ng.e2_Navegacao < 2)
                ng.e2_Navegacao++;
        };

        /**
        * @function Volta para tela anterior do modal
        * @private
        * @param
        */
        ng.e2_VoltarModal = e2_voltarModal
        function e2_voltarModal(index) {

            if (index) {
                ng.e2_Navegacao = index;
            }
            else if (ng.e2_Navegacao > 1)
                ng.e2_Navegacao--;

        };

        function addEventkeyUp() {
            $("body").keyup(function (target) {
                if (target.keyCode == 27) {
                    ng.e2_Limpar();
                }//if
            });
        };

        function removeEventkeyUp() {
            $("body").unbind("keyup");
        };

        //ng.e2_AvancarModal = e2_AvancarModal;
        //function e2_AvancarModal() {

        //}

        /**
        * @function Limpa dados do modal
        * @private
        * @param
        */
        ng.e2_Limpar = e2_limparModal
        function e2_limparModal() {

            removeEventkeyUp();
            ng.alterouEtapaAtual = (false);
            ng.modalAnterior = null;
            // ng.e2_ListaItemSelecionados = self.etapa2.selecionados;
            atualizarBloco();
            ng.e2_ResultadoBusca = [];
            self.etapa2.selecionados = [];
            ng.e2_ListaItemSelecionados = [];
            ng.e2_ListaItemCheckedCache = [];
            ng.e2_ListaKnowledgeAreaSelecionadas = [];
            self.etapa2.knowledgeAreasSelecionadas = [];
            initModalAdicao();
        };

        /**
        * @function Limpa dados do modal
        * @private
        * @param
        */
        ng.e2_LimparKnowLedgeArea = e2_LimparKnowLedgeArea
        function e2_LimparKnowLedgeArea() {

            removeEventkeyUp();
            ng.alterouEtapaAtual = (false);
            ng.modalAnterior = null;
            ng.e2_ListaKnowledgeAreaSelecionadas = [];
            self.etapa2.knowledgeAreasSelecionadas = [];

        };

        /**
        * @function Salva dados do modal
        * @private
        * @param
        */
        ng.e2_Salvar = e2_Salvar
        function e2_Salvar(salvouPeloModal = false) {
            if (ng.showTestTAI) {
                var listaParaSalvar = e2_mapearParaListaTestTaiCurriculumGradeSave();
                var result = TestModel.saveTestTaiCurriculumGrade(listaParaSalvar);

                if (result.success) {
                    $notification.success('A prova foi salva com sucesso!');
                    ng.alterouEtapaAtual = (false);
                } else {
                    if (result.type && result.message)
                        $notification[result.type ? result.type : 'error'](result.message);
                }

            } else {
                ng.selecItensProxCaderno = salvouPeloModal;
                if (!ng.alterouEtapaAtual)
                    return;
                self.etapa2.salvar(validarEtapa2(), e2_salvo);
            }
        };

        /**
         * @function Retorna o objeto necessário para chamada de save dos itens selecionados
         * @name e2_CheckedItem
         * @namespace TestController
         * @memberOf Controller
         * @param {object} item - elemento selecionado
         * @param {object} remove
         * @author Julio Cesar da Silva - 28/10/2015
         * @author Alexandre Calil B. Paravani - 14/05/2015
         */
        function validarEtapa2() {

            if (ng.showTestTAI) {

                if (!ng.e2_cbComponenteCurricular)
                    return $notification.alert('Selecione um componente curricular.');

                if (!ng.e2_matrizAvaliacao)
                    return $notification.alert('Selecione uma matriz de avaliação.');

                if (!ng.anosItensAmostraProvaTai || ng.anosItensAmostraProvaTai.length == 0)
                    return $notification.alert('Selecione o(s) ano(s) dos itens da amostra.');

                var porcentagem = obterPorcentagemAnoItensAmostraTai();
                if (porcentagem != 100)
                    return $notification.alert('A soma da porcentagem dos anos escolares deve ser igual a 100.');

                return true;

            } else {
                var obj = {
                    entity: {
                        Id: ng.e2_blockAtual.Id,
                        Description: ng.e2_blockAtual.Description,
                        Test_Id: ng.provaId
                    }
                };

                var blockItem = [];
                var arr = ng.e2_ListaItemSelecionados;
                var len = arr.length;

                for (var i = 0; i < len; i++) {

                    if (arr[i].Id) {

                        blockItem[i] = {
                            Item_Id: arr[i].Id,
                            Block_Id: ng.e2_blockAtual.Id,
                            Order: i
                        };
                    }
                }

                obj.entity['BlockItems'] = blockItem;

                return obj;
            }            
        };

        ng.e2_callModalAposSalvar = e2_callModalAposSalvar;
        function e2_callModalAposSalvar() {
            angular.element("#modalSelecItensProxCaderno").modal({ backdrop: 'static' });
        };

        ng.callModalAdicionarContextoProva = callModalAdicionarContextoProva;
        function callModalAdicionarContextoProva() {
            e1_criarObjetoDadosModalContexto();
            angular.element("#modalNovoContextoProva").modal({ backdrop: 'static' });
        };

        ng.callModalAnoItensAmostraTai = callModalAnoItensAmostraTai;
        function callModalAnoItensAmostraTai() {
            angular.element("#modalAnoItensAmostraTai").modal({ backdrop: 'static' });
        };

        ng.e2_exibirModalProximoBloco = e2_exibirModalProximoBloco;
        function e2_exibirModalProximoBloco() {
            e2_limparModal();
            setTimeout(function () {
                ng.e2_callModal(1, ng.proximoBloco);
                ng.selecItensProxCaderno = false;
            }, 500);
        }

        ng.e2_limparModalProximoBloco = e2_limparModalProximoBloco;
        function e2_limparModalProximoBloco() {
            ng.proximoBloco = null;
            ng.selecItensProxCaderno = false;
            e2_limparModal();
        }

        function e2_tratarExibirProximoBlocoAposSalvar() {
            if (ng.selecItensProxCaderno) {
                const blocoAtual = ng.cadernos.find(cad => cad.Description === ng.e2_blockAtual.Description);
                const indexBlocoAtual = ng.cadernos.indexOf(blocoAtual);
                if (indexBlocoAtual || indexBlocoAtual === 0) {
                    const indexProximoBloco = indexBlocoAtual + 1;
                    if (indexProximoBloco <= ng.cadernos.length - 1) {
                        const proximoBloco = ng.cadernos[indexProximoBloco];
                        if (proximoBloco) {
                            ng.proximoBloco = proximoBloco;
                            e2_callModalAposSalvar();
                        }
                    }

                }
                ng.selecItensProxCaderno = false;
            }
        }

        function e2_salvo(r) {

            if (r.success) {
                $notification.success('A prova foi salva com sucesso!');
                if (!ng.e2_blockAtual.Id) {
                    ng.e2_blockAtual.Id = r.TestID || r.blockid;
                }
                if (ng.temBIB) {
                    self.etapa2.selecionados = [];
                } else {
                    self.etapa2.selecionados = ng.e2_ListaItemSelecionados;
                }

                ng.situacao = procurarElementoEm([{ Id: r.TestSituation }], self.situacaoList)[0];
                ng.alterouEtapaAtual = (false);
                atualizarBloco();
                ng.etapaAtual = 3;
                e2_tratarExibirProximoBlocoAposSalvar();
                ng.cadernos = [...ng.cadernos];
            }
            else {
                e2_limparModal();
                if (r.type && r.message)
                    $notification[r.type ? r.type : 'error'](r.message);
            }
        };

        function atualizarBloco() {
            ng.e2_blockAtual.SelectedItens = ng.e2_ListaItemSelecionados || [];
            ng.e2_blockAtual.QtdeKnowledgeArea = calculaQtdeKnowledgeArea(ng.e2_blockAtual.SelectedItens);
            ng.e2_blockAtual.ItensCount = ng.e2_blockAtual.SelectedItens.length;
            ng.e2_blockAtual.Resto = ng.e2_blockAtual.Total - ng.e2_blockAtual.ItensCount;
            contarItensSelecionados();
        };

        function calculaQtdeKnowledgeArea(listItem) {
            if (ng.isKnowledgeAreaBlock) {
                var knowledgeAreaIds = [];
                for (var index = 0; index < listItem.length; index++) {
                    if ($.inArray(listItem[index].KnowledgeArea_Id, knowledgeAreaIds) < 0) {
                        knowledgeAreaIds.push(listItem[index].KnowledgeArea_Id);
                    }
                };
                return knowledgeAreaIds.length;
            }
            else {
                return 0;
            }
        }

        /**
        * @function Salva dados do modal
        * @private
        * @param
        */
        ng.e2_SalvarKnowLedgeAreaOrder = e2_SalvarKnowLedgeAreaOrder
        function e2_SalvarKnowLedgeAreaOrder() {
            if (!ng.alterouEtapaAtual)
                return;

            self.etapa2.salvarKnowLedgeAreaOrder(validarEtapa2KnowLedgeAreaOrder(), e2_salvoKnowLedgeAreaOrder);
        }

        /**
         * @function Retorna o objeto necessário para chamada de save da ordenação das áreas de conhecimento
         * @name validarEtapa2KnowLedgeAreaOrder
         * @namespace TestController
         * @memberOf Controller
         */
        function validarEtapa2KnowLedgeAreaOrder() {

            var obj = {
                entity: {
                    Id: ng.e2_blockAtual.Id,
                    Description: ng.e2_blockAtual.Description,
                    Test_Id: ng.provaId
                }
            };

            var blockKnowledgeArea = [];
            var arr = ng.e2_ListaKnowledgeAreaSelecionadas;
            var len = arr.length;

            for (var i = 0; i < len; i++) {

                if (arr[i].Id) {

                    blockKnowledgeArea[i] = {
                        Id: arr[i].Id,
                        KnowledgeArea_Id: arr[i].KnowledgeArea_Id,
                        Block_Id: ng.e2_blockAtual.Id,
                        Order: i
                    };
                }
            }

            obj.entity['BlockKnowledgeAreas'] = blockKnowledgeArea;

            return obj;
        };

        function e2_salvoKnowLedgeAreaOrder(r) {

            if (r.success) {
                $notification.success('A prova foi salva com sucesso!');
                if (!ng.e2_blockAtual.Id) {
                    ng.e2_blockAtual.Id = r.TestID || r.blockid;
                }
                self.etapa2.knowledgeAreasSelecionadas = ng.e2_ListaKnowledgeAreaSelecionadas;
                ng.situacao = procurarElementoEm([{ Id: r.TestSituation }], self.situacaoList)[0];
                ng.alterouEtapaAtual = (false);
                ng.etapaAtual = 3;
            }
            else {
                e2_limparModal();
                if (r.type && r.message)
                    $notification[r.type ? r.type : 'error'](r.message);
            }
        };

        /**
         * @function Seleciona elemento da lista e armazena
         * @name e2_CheckedItem
         * @namespace TestController
         * @memberOf Controller
         * @param {object} item - elemento selecionado
         * @param {object} remove
         * @author Julio Cesar da Silva - 28/10/2015
         * @author Alexandre Calil B. Paravani - 14/05/2015
         */
        ng.e2_CheckedItem = e2_CheckedItem;
        function e2_CheckedItem(item, remove) {

            item = buscarItem(item, ng.e2_ResultadoBusca) || item;

            if (!item)
                return;

            if (item.ref)
                item = item.ref;

            if (!remove && !item.check) {

                var listaItens = angular.copy(ng.e2_ListaItemSelecionados);
                listaItens.push(item);
                ng.e2_blockAtual.QtdeKnowledgeArea = calculaQtdeKnowledgeArea(listaItens);

                if (listaItens.length + ng.e2_blockAtual.QtdeKnowledgeArea > 100) {
                    $notification.alert('A quantidade total não pode ser maior que 100 (itens + áreas de conhecimento distintas).');
                    return;
                }
                else if (ng.e2_ListaItemSelecionados.length >= ng.e2_blockAtual.Total) {
                    $notification.alert('O total máximo de itens já foi atingido.');
                    return;
                }

                var i = buscarItem(item, ng.e2_ListaItemSelecionados);
                if (!i) {

                    item.check = true;

                    ng.e2_addItemSelecionado(item);

                    i = buscarItem(item, ng.e2_ResultadoBusca);
                    if (i) {
                        i.ref.check = true;
                    }
                }
            }
            else {

                var i = buscarItem(item, ng.e2_ListaItemSelecionados);
                if (i) {
                    ng.e2_ListaItemSelecionados.splice(i.index, 1)
                    i.ref.check = false;
                }

                i = buscarItem(item, ng.e2_ListaItemCheckedCache);
                if (i) {
                    ng.e2_ListaItemCheckedCache.splice(i.index, 1);
                    i.ref.check = false;
                }

                i = buscarItem(item, ng.e2_ResultadoBusca);
                if (i) {
                    i.ref.check = false;
                }

            }

            ng.alterouEtapaAtual = (true);
        };

        ng.e2_LimparItensSelecionados = e2_LimparItensSelecionados;
        function e2_LimparItensSelecionados() {
            if (ng.e2_ResultadoBusca && ng.e2_ResultadoBusca.length) {
                ng.e2_ResultadoBusca.forEach(item => {
                    item.check = false;
                });
            }
            self.etapa2.selecionados = [];
            ng.e2_ListaItemSelecionados = [];
            ng.e2_ListaItemCheckedCache = [];
            ng.e2_ListaKnowledgeAreaSelecionadas = [];
            self.etapa2.knowledgeAreasSelecionadas = [];

            ng.alterouEtapaAtual = (true);
            e2_Salvar();
        }

        /**
         * @function Adicão dos itens na lista de selecionados (agrupar itens com mesmo texto base)
         * @name e2_addItemSelecionado
         * @namespace TestController
         * @memberOf Controller
         * @param {object} item - elemento selecionado
         * @return
         * @author Julio Cesar da Silva - 28/10/2015
         */
        ng.e2_addItemSelecionado = function __e2_addItemSelecionado(item) {

            var arr = ng.e2_ListaItemSelecionados;
            var arrCache = ng.e2_ListaItemCheckedCache;
            var rest = [];
            var index = undefined;

            if (item.BaseTextId === undefined || item.BaseTextId === null) {
                ng.e2_ListaItemSelecionados.push(angular.copy(item));
                ng.e2_ListaItemCheckedCache.push(angular.copy(item));
                return;
            }

            for (var a = 0; a < arr.length; a++) {
                if (arr[a].BaseTextId !== undefined && arr[a].BaseTextId !== null) {
                    if (arr[a].BaseTextId === item.BaseTextId) {
                        index = a;
                    }
                }
            }

            if (index === undefined || index === (arr.length - 1)) {
                ng.e2_ListaItemSelecionados.push(angular.copy(item));
                ng.e2_ListaItemCheckedCache.push(angular.copy(item));
                return;
            }

            for (var b = (arr.length - 1); b > index; b--) {
                rest.push(angular.copy(arr[b]));
                arr.splice(b, 1);
                arrCache.splice(b, 1);
            }

            ng.e2_ListaItemSelecionados.push(angular.copy(item));
            ng.e2_ListaItemCheckedCache.push(angular.copy(item));

            for (var c = (rest.length - 1); c >= 0; c--) {
                ng.e2_ListaItemSelecionados.push(angular.copy(rest[c]));
                ng.e2_ListaItemCheckedCache.push(angular.copy(rest[c]));
            }
        };

        /**
         * @function buscar ref. de 
         * @name buscarItem
         * @namespace TestController
         * @memberOf Controller
         * @param {object} item - elemento selecionado
         * @param {object} lista
         * @author Alexandre Calil B. Paravani - 14/05/2015
         */
        function buscarItem(item, lista) {

            if (!lista)
                return null;

            var i = 0, m = lista.length;

            for (i; i < m; i++)
                if (lista[i].Id)
                    if (lista[i].Id === item.Id)
                        return { index: i, ref: lista[i] };

            return null;
        };

        /**
         * @function Realizar os calculos para agrupamento de elementos para ordenação
         * @name e2_ListaItemSelecionadosHover
         * @namespace TestController
         * @memberOf Controller
         * @param {object} item - elemento que está sofrendo hover
         * @param {string} direction - sentido da ordenação
         * @return
         * @author Julio Cesar da Silva - 23/10/2015 - 27/10/2015
         */
        ng.e2_ListaItemSelecionadosHover = function __e2_ListaItemSelecionadosHover(item, direction) {

            if (ng.ordering)
                return;

            var arr = ng.e2_ListaItemSelecionados;
            var currentIndex = arr.indexOf(item);

            if (direction === 'up' && currentIndex >= 0) {

                var previus = arr[currentIndex - 1];

                if (previus !== undefined) {

                    if (previus.BaseTextId === undefined || previus.BaseTextId === null || previus.BaseTextId !== item.BaseTextId) {

                        if (hasGroupItem(item, arr)) {
                            setGroupItem();
                            setCurrentItems('group');
                        }
                        else {
                            setUniqueItem();
                            setCurrentItems('one');
                        }
                    }
                    else if (previus.BaseTextId === item.BaseTextId) {
                        setUniqueItem();
                        setCurrentItems('one');
                    }

                    setTargetItems(previus);
                }
            }
            else if (direction === 'down' && currentIndex >= 0) {

                var next = arr[currentIndex + 1];

                if (next !== undefined) {

                    if (next.BaseTextId === undefined || next.BaseTextId === null || next.BaseTextId !== item.BaseTextId) {

                        if (hasGroupItem(item, arr)) {
                            setGroupItem();
                            setCurrentItems('group');
                        }
                        else {
                            setUniqueItem();
                            setCurrentItems('one');
                        }
                    }
                    else if (next.BaseTextId === item.BaseTextId) {
                        setUniqueItem();
                        setCurrentItems('one');
                    }

                    setTargetItems(next);
                }
            }

            function setGroupItem() {
                ng.groupLimit = item.BaseTextId;
                ng.oneLimit = undefined;
            };

            function setUniqueItem() {
                ng.groupLimit = undefined;
                ng.oneLimit = item;
            };

            function setTargetItems(_target) {
                ng.targetItems = [];
                if (_target.BaseTextId === undefined || _target.BaseTextId === null) {
                    _target['Animation'] = false;
                    ng.targetItems.push(angular.copy(_target));
                    getRest(arr.indexOf(_target));
                }
                else if (_target.BaseTextId === item.BaseTextId) {
                    _target['Animation'] = false;
                    ng.targetItems.push(angular.copy(_target));
                    getRest(arr.indexOf(_target));
                }
                else {
                    var __crescIndex;
                    for (var a = 0; a < arr.length; a++) {
                        if (arr[a].BaseTextId === _target.BaseTextId) {
                            arr[a]['Animation'] = false;
                            ng.targetItems.push(angular.copy(arr[a]));
                            if (direction === 'up' && ng.targetItems.length === 1)
                                getRest(a);
                            else
                                __crescIndex = a;
                        }
                    }
                    if (direction === 'down')
                        getRest(__crescIndex);
                }
            };

            function setCurrentItems(procedure) {
                ng.currentItems = [];
                if (procedure === 'one') {
                    item['Animation'] = false;
                    ng.currentItems.push(angular.copy(item));
                }
                else if (procedure === 'group') {
                    for (var b = 0; b < arr.length; b++) {
                        if (arr[b].BaseTextId == item.BaseTextId) {
                            arr[b]['Animation'] = false;
                            ng.currentItems.push(angular.copy(arr[b]));
                        }
                    }
                }
            };

            function getRest(_index) {
                ng.arrRest = [];
                if (direction === 'up') {
                    for (var i = _index - 1; i >= 0; i--)
                        if (arr[i] !== undefined) {
                            arr[i]['Animation'] = false;
                            ng.arrRest.unshift(angular.copy(arr[i]));
                        }
                }
                else if (direction === 'down') {
                    for (var e = _index + 1; e < arr.length; e++)
                        if (arr[e] !== undefined) {
                            arr[e]['Animation'] = false;
                            ng.arrRest.push(angular.copy(arr[e]));
                        }
                }
            };
        };

        /**
         * @function Realizar os calculos para agrupamento de elementos para ordenação
         * @name e2_ListaKnowledgeAreaSelecionadasHover
         * @namespace TestController
         * @memberOf Controller
         * @param {object} item - elemento que está sofrendo hover
         * @param {string} direction - sentido da ordenação
         * @return
         */
        ng.e2_ListaKnowledgeAreaSelecionadasHover = function e2_ListaKnowledgeAreaSelecionadasHover(item, direction) {

            if (ng.ordering)
                return;

            var arr = ng.e2_ListaKnowledgeAreaSelecionadas;
            var currentIndex = arr.indexOf(item);

            if (direction === 'up' && currentIndex >= 0) {

                var previus = arr[currentIndex - 1];

                if (previus !== undefined) {
                    setUniqueItem();
                    setCurrentItems('one');
                    setTargetItems(previus);
                }
            }
            else if (direction === 'down' && currentIndex >= 0) {

                var next = arr[currentIndex + 1];

                if (next !== undefined) {
                    setUniqueItem();
                    setCurrentItems('one');
                    setTargetItems(next);
                }
            }

            function setGroupItem() {
                ng.groupLimit = item.KnowledgeArea_Id;
                ng.oneLimit = undefined;
            };

            function setUniqueItem() {
                ng.groupLimit = undefined;
                ng.oneLimit = item;
            };

            function setTargetItems(_target) {
                ng.targetItems = [];
                if (_target.KnowledgeArea_Id === undefined || _target.KnowledgeArea_Id === null) {
                    _target['Animation'] = false;
                    ng.targetItems.push(angular.copy(_target));
                    getRest(arr.indexOf(_target));
                }
                else if (_target.KnowledgeArea_Id === item.KnowledgeArea_Id) {
                    _target['Animation'] = false;
                    ng.targetItems.push(angular.copy(_target));
                    getRest(arr.indexOf(_target));
                }
                else {
                    var __crescIndex;
                    for (var a = 0; a < arr.length; a++) {
                        if (arr[a].KnowledgeArea_Id === _target.KnowledgeArea_Id) {
                            arr[a]['Animation'] = false;
                            ng.targetItems.push(angular.copy(arr[a]));
                            if (direction === 'up' && ng.targetItems.length === 1)
                                getRest(a);
                            else
                                __crescIndex = a;
                        }
                    }
                    if (direction === 'down')
                        getRest(__crescIndex);
                }
            };

            function setCurrentItems(procedure) {
                ng.currentItems = [];
                if (procedure === 'one') {
                    item['Animation'] = false;
                    ng.currentItems.push(angular.copy(item));
                }
                else if (procedure === 'group') {
                    for (var b = 0; b < arr.length; b++) {
                        if (arr[b].KnowledgeArea_Id == item.KnowledgeArea_Id) {
                            arr[b]['Animation'] = false;
                            ng.currentItems.push(angular.copy(arr[b]));
                        }
                    }
                }
            };

            function getRest(_index) {
                ng.arrRest = [];
                if (direction === 'up') {
                    for (var i = _index - 1; i >= 0; i--)
                        if (arr[i] !== undefined) {
                            arr[i]['Animation'] = false;
                            ng.arrRest.unshift(angular.copy(arr[i]));
                        }
                }
                else if (direction === 'down') {
                    for (var e = _index + 1; e < arr.length; e++)
                        if (arr[e] !== undefined) {
                            arr[e]['Animation'] = false;
                            ng.arrRest.push(angular.copy(arr[e]));
                        }
                }
            };
        };

        /**
         * @function Verificar se uma lista tem mais de um item com o mesmo texto base
         * @name hasGroupItem
         * @namespace TestController
         * @memberOf Controller
         * @param {object} item
         * @param {array} lista
         * @author Julio Cesar da Silva - 23/10/2015 - 27/10/2015
         * @return {boolean}
         */
        function hasGroupItem(item, lista) {

            var count = 0;

            for (var a = 0; a < lista.length; a++) {
                if (lista[a].BaseTextId !== undefined && lista[a].BaseTextId !== null) {
                    if (lista[a].BaseTextId === item.BaseTextId) {
                        count += 1;
                    }
                }
            }

            if (count > 1)
                return true;

            return false;
        };

        /**
         * @function Limpar seleção por hover
         * @name e2_ListaItemSelecionadosHover
         * @namespace TestController
         * @memberOf Controller
         * @param
         * @return
         * @author Julio Cesar da Silva - 23/10/2015 - 27/10/2015
         */
        ng.e2_ListaItemSelecionadosLeave = function __e2_ListaItemSelecionadosLeave() {
            if (ng.ordering)
                return;
            ng.groupLimit = undefined;
            ng.oneLimit = undefined;
            ng.targetItems = [];
            ng.currentItems = [];
            ng.arrRest = [];
        };

        /**
         * @function Limpar seleção por hover
         * @name e2_ListaItemSelecionadosHover
         * @namespace TestController
         * @memberOf Controller
         * @param
         * @return
         * @author Julio Cesar da Silva - 23/10/2015 - 27/10/2015
         */
        ng.e2_ListaKnowledgeAreaSelecionadasLeave = function __e2_ListaKnowledgeAreaSelecionadasLeave() {
            if (ng.ordering)
                return;
            ng.groupLimit = undefined;
            ng.oneLimit = undefined;
            ng.targetItems = [];
            ng.currentItems = [];
            ng.arrRest = [];
        };

        /**
         * @function Ordenar array
         * @name e2_ListaItemSelecionadosOrder
         * @namespace TestController
         * @memberOf Controller
         * @param
         * @return
         * @author Julio Cesar da Silva - 23/10/2015 - 27/10/2015
         */
        ng.ordering = false;
        ng.e2_ListaItemSelecionadosOrder = function __e2_ListaItemSelecionadosOrder(direction, last) {
            if (last)
                return;
            if (direction === 'up' && !ng.ordering) {
                ng.ordering = true;
                orderToUp();
            }
            else if (direction === 'down' && !ng.ordering) {
                ng.ordering = true;
                orderToDown();
            }
        };

        /**
         * @function Ordenar para acima
         * @name orderToUp
         * @namespace TestController
         * @memberOf Controller
         * @param
         * @return
         * @author Julio Cesar da Silva - 23/10/2015 - 27/10/2015
         */
        function orderToUp() {

            var arr = ng.e2_ListaItemSelecionados;
            if (ng.e2_Navegacao === 7) {
                arr = ng.e2_ListaKnowledgeAreaSelecionadas;
            }
            var len = (arr.length - 1);

            //ordenação bubble sort
            if (ng.currentItems.length === 1 && ng.targetItems.length === 1) {
                if ((ng.e2_Navegacao === 7 && ng.currentItems[0].KnowledgeArea_Id !== undefined && ng.currentItems[0].KnowledgeArea_Id !== null && ng.currentItems[0].KnowledgeArea_Id === ng.targetItems[0].KnowledgeArea_Id)
                    || (ng.currentItems[0].BaseTextId !== undefined &&
                        ng.currentItems[0].BaseTextId !== null &&
                        ng.currentItems[0].BaseTextId === ng.targetItems[0].BaseTextId)) {
                    var _selected = buscarItem(ng.currentItems[0], arr);
                    var _target = buscarItem(ng.targetItems[0], arr);
                    var aux = angular.copy(_target.ref);
                    arr[_target.index] = angular.copy(_selected.ref);
                    arr[_selected.index] = angular.copy(aux);
                    arr[_target.index]['Animation'] = true;

                    if (ng.e2_Navegacao === 7) {
                        finallyOrderKnowledgeArea();
                    }
                    else {
                        finallyOrder();
                    }
                    return;
                }
            }


            //ordenação de pilhas
            for (var i = len; i >= 0; i--) {

                for (var y = 0; y < ng.currentItems.length; y++)
                    if (arr[i] != undefined)
                        if (arr[i].Id == ng.currentItems[y].Id)
                            arr.splice(i, 1);
                for (var x = 0; x < ng.targetItems.length; x++)
                    if (arr[i] != undefined)
                        if (arr[i].Id == ng.targetItems[x].Id)
                            arr.splice(i, 1);

                for (var z = 0; z < ng.arrRest.length; z++)
                    if (arr[i] != undefined)
                        if (arr[i].Id == ng.arrRest[z].Id)
                            arr.splice(i, 1);
            }

            for (var a = ng.targetItems.length - 1; a >= 0; a--) {
                arr.unshift(angular.copy(ng.targetItems[a]));
            }

            for (var b = ng.currentItems.length - 1; b >= 0; b--) {
                ng.currentItems[b]['Animation'] = true;
                arr.unshift(angular.copy(ng.currentItems[b]));
            }

            for (var c = ng.arrRest.length - 1; c >= 0; c--) {
                arr.unshift(angular.copy(ng.arrRest[c]));
            }

            if (ng.e2_Navegacao === 7) {
                finallyOrderKnowledgeArea();
            }
            else {
                finallyOrder();
            }
        };

        /**
         * @function Ordenar para acima
         * @name orderToUp
         * @namespace TestController
         * @memberOf Controller
         * @param
         * @return
         * @author Julio Cesar da Silva - 23/10/2015 - 27/10/2015
         */
        function orderToDown() {

            var arr = ng.e2_ListaItemSelecionados;
            if (ng.e2_Navegacao === 7) {
                arr = ng.e2_ListaKnowledgeAreaSelecionadas;
            }
            var len = (arr.length - 1);

            //ordenação bubble sort
            if (ng.currentItems.length === 1 && ng.targetItems.length === 1) {
                if ((ng.e2_Navegacao === 7 && ng.currentItems[0].KnowledgeArea_Id !== undefined && ng.currentItems[0].KnowledgeArea_Id !== null && ng.currentItems[0].KnowledgeArea_Id === ng.targetItems[0].KnowledgeArea_Id)
                    || (ng.currentItems[0].BaseTextId !== undefined &&
                        ng.currentItems[0].BaseTextId !== null &&
                        ng.currentItems[0].BaseTextId === ng.targetItems[0].BaseTextId)) {

                    var _selected = buscarItem(ng.currentItems[0], arr);
                    var _target = buscarItem(ng.targetItems[0], arr);
                    var aux = angular.copy(_target.ref);
                    arr[_target.index] = angular.copy(_selected.ref);
                    arr[_selected.index] = angular.copy(aux);
                    arr[_target.index]['Animation'] = true;
                    if (ng.e2_Navegacao === 7) {
                        finallyOrderKnowledgeArea();
                    }
                    else {
                        finallyOrder();
                    }
                    return;
                }
            }

            //ordenação de pilhas
            for (var i = len; i >= 0; i--) {

                for (var y = 0; y < ng.currentItems.length; y++)
                    if (arr[i] != undefined)
                        if (arr[i].Id == ng.currentItems[y].Id)
                            arr.splice(i, 1);

                for (var x = 0; x < ng.targetItems.length; x++)
                    if (arr[i] != undefined)
                        if (arr[i].Id == ng.targetItems[x].Id)
                            arr.splice(i, 1);

                for (var z = 0; z < ng.arrRest.length; z++)
                    if (arr[i] != undefined)
                        if (arr[i].Id == ng.arrRest[z].Id)
                            arr.splice(i, 1);
            }

            //reposicionando os elementos
            for (var a = 0; a < ng.targetItems.length; a++) {
                arr.push(angular.copy(ng.targetItems[a]));
            }

            for (var b = 0; b < ng.currentItems.length; b++) {
                ng.currentItems[b]['Animation'] = true;
                arr.push(angular.copy(ng.currentItems[b]));
            }

            for (var c = 0; c < ng.arrRest.length; c++) {
                arr.push(angular.copy(ng.arrRest[c]));
            }
            /**/
            if (ng.e2_Navegacao === 7) {
                finallyOrderKnowledgeArea();
            }
            else {
                finallyOrder();
            }
        };

        /**
         * @function Finalizar a ordenação
         * @name finallyOrder
         * @namespace TestController
         * @memberOf Controller
         * @param
         * @return
         * @author Julio Cesar da Silva - 05/11/2015 - 05/11/2015
         */
        function finallyOrder() {
            ng.ordering = false;
            ng.e2_ListaItemSelecionadosLeave();
            ng.alterouEtapaAtual = true;
        };

        /**
         * @function Finalizar a ordenação
         * @name finallyOrder
         * @namespace TestController
         * @memberOf Controller
         * @param
         * @return
         * @author Julio Cesar da Silva - 05/11/2015 - 05/11/2015
         */
        function finallyOrderKnowledgeArea() {
            ng.ordering = false;
            ng.e2_ListaKnowledgeAreaSelecionadasLeave();
            ng.alterouEtapaAtual = true;
        };

        function initEtapa4() {


            ng.Provas;

            ng.provaCaminho = [];

            e4_cadernoCarregar();

            self.etapa4.carregou = true;

        };

        /**
        * @function Caderno Carregar
        * @private
         * @param item: elemento que esta sendo validado
       */
        function e4_cadernoCarregar() {
            self.etapa4.cadernos({ Id: ng.provaId }, e4_cadernoCarregados);
        };

        /**
        * @function Caderno Carregados
        * @private
        * @param item: elemento que esta sendo validado
        */
        function e4_cadernoCarregados(r) {

            if (r.success) {
                for (var i = 0; i < r.lista.length; i++) {

                    if (r.lista[i].Registered)
                        if (r.lista[i].File)
                            if (r.lista[i].File.length > 0) {
                                ng.provaCaminho[i] = r.lista[i].File[0].File;
                                r.lista[i].currentDate = r.lista[i].File[0].GenerationData;

                                continue;
                            }

                    ng.provaCaminho[i] = "";
                }

                ng.Provas = r.lista;

                if (!ng.Provas[0].Registered)
                    for (var i = 0; i < ng.Provas.length; i++) {
                        ng.Provas[i].Download = true;
                    }

            } else {
                if (r.type && r.message)
                    $notification[r.type ? r.type : 'error'](r.message);
                return false;
            }
        };

        /**
        * @function selecionaItem
        * @private
        * @param item: elemento que esta sendo validado
        */
        ng.selecionaItem = selecionaItem;
        function selecionaItem(index) {
            ng.Provas[index].gerar = !ng.Provas[index].gerar;
        };

        /**
        * @function Gerar Selecionadas
        * @private
        * @param item: elemento que esta sendo validado
        */
        ng.gerarSelecionadas = gerarSelecionadas;
        function gerarSelecionadas() {

            var selecionadas = [];

            for (var i = 0; i < ng.Provas.length; i++) {
                if (ng.Provas[i].gerar) {
                    selecionadas.push(ng.Provas[i].test)
                }
            }
        };

        /**
        * @function Baixar Selecionadas
        * @private
        * @param item: elemento que esta sendo validado
        */
        ng.baixarSelecioandas = baixarSelecioandas;
        function baixarSelecioandas() {
            var selecionadas = [];

            for (var i = 0; i < ng.Provas.length; i++) {
                if (ng.Provas[i].gerar) {
                    selecionadas.push(ng.Provas[i].test)
                }
            }
        };

        /**
        * @function Visualizar Prova
        * @private
        * @param item: elemento que esta sendo validado
        */
        ng.visualizarProva = visualizarProva;
        function visualizarProva(index) {

            $("#htmlProva").children().remove();
            $("#htmlGabarito").children().remove();

            ng.bookletAtual = ng.Provas[index];
            e4_callModal();
        };

        ng.selectedFolhaResposta = function _selectedFolhaResposta(registered) {
            if (!registered) {
                ng.gerarFolhaResposta = !ng.gerarFolhaResposta;
                e1_nivelDesempenhoMudou();
            }
        };

        ng.selectedFeedback = function _selectedFeedback(registered) {
            if (!registered) {
                ng.publicFeedback = !ng.publicFeedback;
            }
        };



        /**
        * @function Geração de modelo HTML da prova
        * @private
        * @param item: booklet a ser gerado
        */
        function htmlGerar(itemBooklet) {

            if (ng.bookletAtual != null) {
                ng.bookletAtual.sheet = ng.gerarFolhaResposta;
                ng.bookletAtual.publicFeedback = ng.publicFeedback;
            }

            if (itemBooklet != null) {
                itemBooklet.sheet = ng.gerarFolhaResposta;
                itemBooklet.publicFeedback = ng.publicFeedback;
            }

            self.etapa4.html(ng.bookletAtual || itemBooklet, htmlGerado);
        };

        /**
        * @function Tratamento de modelo HTML da prova
        * @private
        * @param item: booklet a ser gerado
        */
        function htmlGerado(r) {

            if (r.success) {
                $("#htmlProva").append(r.Test);
                $(".numeral").removeClass('prova');
                $("#htmlGabarito").append(r.TestFeedback);

            } else {
                if (r.type && r.message)
                    $notification[r.type ? r.type : 'error'](r.message);
                return false;
            }
        };

        /**
        * @function Inicia geração de prova
        * @private
        * @param item: booklet a ser gerado
        */
        ng.provaGerar = provaGerar;
        function provaGerar(itemBooklet) {

            //AnswerSheet
            if (ng.bookletAtual != null)
                ng.bookletAtual.sheet = ng.gerarFolhaResposta;//true;
            if (itemBooklet != null)
                itemBooklet.sheet = ng.gerarFolhaResposta;//true;

            self.etapa4.gerar(ng.bookletAtual || itemBooklet, provaGerada);
        };

        /**
        * @function Tratamento para prova gerada
        * @private
        * @param r: resposta do servidor
        */
        function provaGerada(r) {

            if (r.success) {
                $notification.success("A prova foi gerada com sucesso! <br> Salve a alteração!");

                var i = ng.Provas.indexOf(ng.bookletAtual);
                ng.BtnSaveDisabled = false;
                ng.bookletAtual.Registered = true;
                ng.provaPDF = r.generateTest;

                if (i < 0)
                    $notification.alert("Prova não encontrada.");

                ng.provaCaminho[i] = r.generateTest.File.Path;
                ng.filesTest = r.generateTest;

                ng.bookletAtual.currentDate = r.generateTest.File.GenerationData;
            } else {
                if (r.type && r.message)
                    $notification[r.type ? r.type : 'error'](r.message);
                return false;
            }
        };
        ng.provaExportDoc = provaExportDoc;
        function provaExportDoc() {
            $window.open('/Booklet/ExportTestDoc/' + ng.bookletAtual.Id, "_self");
        };
        ng.provaExportDocIndex = provaExportDocIndex;
        function provaExportDocIndex(index) {
            ng.bookletAtual = ng.Provas[index];
            $window.open('/Booklet/ExportTestDoc/' + ng.bookletAtual.Id, "_self");
        }

        function setGenerateTest() {
            TestModel.generateTest({ Id: ng.provaId, sheet: ng.gerarFolhaResposta, publicFeedback: ng.publicFeedback }, function (result) {
                if (result.success) {
                    ng.filesTest = result.generateTest;
                } else {
                    $notification.alert("Não foi possível baixar todos os arquivos!");
                }
            });
        };

        /**
        * @function Baixar Prova
        * @private
        * @param item: elemento que esta sendo validado
        */
        ng.baixarProva = baixarProva;
        function baixarProva(index) {

            if (ng.filesTest == null || ng.filesTest == undefined) {
                var filesTest = {
                    TestId: ng.Provas[index].TestId,
                    File: ng.Provas[index].File[0],
                    FileAnswerSheet: ng.Provas[index].FileAnswerSheet[0],
                    FileFeedback: ng.Provas[index].FileFeedback[0]
                };

                ng.filesTest = filesTest;
            }

            if (ng.filesTest.FileAnswerSheet != undefined || ng.filesTest.FileAnswerSheet != null) {
                angular.element("#modalDownloadFile").modal({ backdrop: 'static' });
            } else {

                if (ng.provaCaminho[index])
                    window.open(ng.provaCaminho[index]);
                else {
                    $notification.alert("Esta prova não foi gerada.");
                }
            }

        };

        ng.downloadAllFilesZip = function _downloadAllFilesZip() {
            TestModel.checkFilesExists({ Id: ng.provaId }, function (result) {
                if (result.success) {
                    window.open("/Booklet/DownloadZipFiles?Id=" + ng.provaId, "_self");
                } else {
                    $notification.alert("Arquivo(s) não encontrado(s).");
                }
            });
        };

        ng.downloadFiles = function (id) {
            window.open("/File/DownloadFile?Id=" + id, "_self");
        };

        /**
        * @function Chama modais da etapa 4
        * @private
        * @param item: elemento que esta sendo validado
        */
        ng.e4_callModal = e4_callModal;
        function e4_callModal(index) {

            htmlGerar(ng.bookletAtual);

            angular.element("#modalProva").modal({ backdrop: 'static' });

        };
        /**
        * @function ETAPA 4 - Salvar
        * @private
        * @param item: elemento que esta sendo validado
        */
        ng.e4_Salvar = e4_Salvar;
        function e4_Salvar() {

            var naoSalva = false;
            for (var i = 0; i < ng.provaCaminho.length; i++) {
                if (ng.provaCaminho[i] === "") {
                    naoSalva = true;
                    setGenerateTest();
                }

                if (naoSalva)
                    return $notification.alert("Não foi possível finalizar pois ainda existe(m) prova(s) não gerada(s).");


                self.etapa4.finalizar({ Id: ng.provaId }, e4_Salvou);

            }
        };

        function e4_Salvou(r) {

            if (r.success) {
                for (var i = 0; i < ng.provaCaminho.length; i++) {
                    if (ng.provaCaminho[i] !== "") {
                        ng.Provas[i].Registered = true;
                    }
                }
                $notification.success("A prova foi salva com sucesso!");
                ng.BtnSaveDisabled = true;
                ng.bookletAtual.Download = false;

                ng.situacao = procurarElementoEm([{ Id: r.TestSituation }], self.situacaoList)[0];
            }
            else {
                if (r.type && r.message)
                    $notification[r && r.type ? r.type : 'error'](r.message);
                return false;
            }
        };

        /**
        * @function Volta para passo anterior
        * @private
        * @param item: elemento que esta sendo validado
        */
        ng.voltar = voltar;
        function voltar() {
            if (ng.navigation > 0)
                ng.navigation--;
        };

        /**
        * @function Cancelar passo para provas
        * @private
        * @param item: elemento que esta sendo validado
        */
        ng.cancelar = cancelar;
        function cancelar() {

            window.location.href = base_url("Test");

            configVariaveis();
        };

        /**
        * @function Avancar para proximo passo
        * @private
        * @param item: elemento que esta sendo validado
        */
        ng.avancar = avancar;
        function avancar() {

            if (ng.navigation === 1) {


                if (ng.provaId && !self.etapa1.alterou)
                    initEtapa2();
                else if (self.etapa1.alterou) {
                    $notification.alert('Alguns dados foram alterados. Antes de avançar é necessário salvar as modificações.');
                    return false;
                }
            }


            if (ng.navigation === 2) {
                if (!ng.temBIB && (ng.e2_ItensAtuais + ng.e2_blockAtual.QtdeKnowledgeArea > 100)) {
                    return $notification.alert('A quantidade total não pode ser maior que 100 (itens + áreas de conhecimento distintas).');
                }
                else if (ng.temBIB) {
                    let itemsCadernos = 0;
                    if (ng.cadernos.length) {
                        ng.cadernos.forEach(cad => {
                            itemsCadernos += cad.ItensCount;
                        });
                    }

                    if (itemsCadernos === (parseInt(ng.e1_itensBlocos) * parseInt(ng.e1_qtdBlocos))) {
                        initEtapa4();
                    } else {
                        return $notification.alert('A quantidade total de itens ainda não foi atingida.');
                    }
                }
                else if (ng.e2_ItensAtuais === ng.itensTotais)
                    initEtapa4();
                else {
                    return $notification.alert('A quantidade total de itens ainda não foi atingida.');
                }
                ng.BtnSaveDisabled = true;
            }

            if (ng.navigation < ng.listaWizards.length)
                ng.navigation++;

        };

        /**
        * @function Dispara salvar para a Etapa atual
        * @private
        * @param item: elemento que esta sendo validado
        */
        ng.salvar = salvar;
        function salvar() {

            if (ng.navigation === 1) {
                if (validarEtapa1())
                    etapa1Salvar();
            }

            if (ng.navigation === 2 && validarEtapa2()) {
                ng.BtnSaveDisabled = true;
                e2_Salvar();
            }

            //Gerar prova
            if (ng.navigation === ng.ultimo) {
                e4_Salvar();
            }
        };

        /**
        * @function Valida se a prova esta correta
        * @private
        * @param 
        */
        ng.finalizar = finalizar;
        function finalizar() {
            window.location.href = base_url("Test");
        };

        ng.carregaPeriodos = carregaPeriodos;
        function carregaPeriodos() {

            if (!ng.e2_Modalidade)
                ng.e2_ListaPeriodoChecked = [];

            if (ng.e2_NivelEnsino && ng.e2_Modalidade)
                self.etapa2.filtroPeriodos({ LevelEducationID: ng.e2_NivelEnsino.Id, Modality: ng.e2_Modalidade.Id }, filtroPeriodosCarregado);
        };

        ng.activeModal = function __activeModal(label, text) {
            if (!text) return;
            ng.textSelected = {
                Description: label,
                TextDescription: text
            };
            angular.element("#modalTextMatriz").modal({ backdrop: 'static' });
        };

        ng.e1_aplicacao = null;
        ng.e1_correcao = null;

        /**
         * @function - Carrega o combo Tipo Nivel de Ensino
         * @param
         * @public
         */
        function carregaGrupoSubgrupo() {
            TestGroupModel.loadGroupsSubGroups(function (result) {
                if (result.success) {
                    ng.grupoSubgrupoList = result.groupSubGroup;
                    ng.e1_grupoSubgrupo = setValuesComb(ng.grupoSubgrupoList, result.groupSubGroup);
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };


        ng.carregarVersoes = function carregarVersoes(item) {
            for (var k = 0; k < ng.e2_ListaItemSelecionados.length; k++) {
                ng.e2_ListaItemSelecionados[k].expanded = false;
            };

            TestModel.getItemVersions({ itemCodeVersion: item.ItemCodeVersion, itemVersion: item.ItemVersion }, function (result) {
                if (result.success) {
                    ng.e2_ListaItemVersoes = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        ng.changeVersionItem = function changeVersionItem(itens, versoes) {
            TestModel.saveChangeItem({ item: versoes, test_id: ng.params, itemIdAntigo: itens.Id }, function (result) {
                if (result.success) {
                    $notification.success(result.message);

                    for (var k = 0; k < ng.e2_ListaItemSelecionados.length; k++) {
                        if (ng.e2_ListaItemSelecionados[k].Id == item.Id) {
                            ng.e2_ListaItemSelecionados[k].Id = versao.Id;
                            ng.e2_ListaItemSelecionados[k].BaseTextId = versao.BaseText.Id;
                            ng.e2_ListaItemSelecionados[k].BaseTextDescription = versao.BaseText.Description;
                            ng.e2_ListaItemSelecionados[k].Code = versao.ItemCode;
                            ng.e2_ListaItemSelecionados[k].ItemCodeVersion = versao.ItemCodeVersion;
                            ng.e2_ListaItemSelecionados[k].ItemVersion = versao.ItemVersion;
                            ng.e2_ListaItemSelecionados[k].Statement = versao.Statement;
                        }

                        ng.e2_ListaItemSelecionados[k].expanded = false;
                    };
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        ng.e1_criarObjetoDadosModalContexto = e1_criarObjetoDadosModalContexto;
        function e1_criarObjetoDadosModalContexto() {
            ng.e1_dadosModalContexto = {
                id: 0,
                imagePositionDescription: '',
                title: '',
                text: '',
                imagePath: '',
                image: { Id: '', Guid: '', Path: '' }
            };
            limparContextoSummernote();
        };

        ng.e1_addDadosModalContexto = e1_addDadosModalContexto;
        function e1_addDadosModalContexto() {
            if (ng.e1_dadosModalContexto.id) {
                const itemAlterar = ng.testContexts.find(t => t.id === ng.e1_dadosModalContexto.id);
                if (itemAlterar) {
                    const indexItemAlterar = ng.testContexts.indexOf(itemAlterar);
                    const itemAlterado = {
                        ...ng.e1_dadosModalContexto,
                        imagePath: ng.e1_dadosModalContexto.image.Path,
                    };
                    ng.testContexts[indexItemAlterar] = itemAlterado;
                }

            } else {
                const itemNovo = {
                    ...ng.e1_dadosModalContexto,
                    imagePath: ng.e1_dadosModalContexto.image.Path
                };
                ng.testContexts.push(itemNovo);
            }
            ng.e1_criarObjetoDadosModalContexto();
            self.etapa1.alterou = true
            angular.element('#modalNovoContextoProva').modal('hide');
        };

        ng.e1_confirmarDeletarItemTestContext = e1_confirmarDeletarItemTestContext;
        function e1_confirmarDeletarItemTestContext(item) {
            ng.e1_itemParaDeletarDaListaTestContex = item;
            angular.element('#modalDeleteItemTestContex').modal({ backdrop: 'static' });
        };

        ng.e1_deleteItemTestContext = e1_deleteItemTestContext;
        function e1_deleteItemTestContext() {
            if (ng.e1_itemParaDeletarDaListaTestContex) {
                const indexItemDelete = ng.testContexts.indexOf(ng.e1_itemParaDeletarDaListaTestContex);
                ng.testContexts.splice(indexItemDelete, 1);
                ng.e1_itemParaDeletarDaListaTestContex = '';
                self.etapa1.alterou = true
            }
            angular.element('#modalDeleteItemTestContex').modal('hide');
        };

        ng.e1_editarModalContexto = e1_editarModalContexto;
        function e1_editarModalContexto(item) {
            e1_criarObjetoDadosModalContexto();
            ng.e1_dadosModalContexto = {
                ...item,
                image: { Id: '', Guid: '', Path: item.imagePath }
            };
            editarContextoSummernote();
            angular.element('#modalNovoContextoProva').modal({ backdrop: 'static' });
        };

        function obterCampoContextoSummernote() {
            return angular.element('div .note-editable');
        }

        function editarContextoSummernote() {
            var campoContexto = obterCampoContextoSummernote();
            campoContexto.html(ng.e1_dadosModalContexto.text);
        }

        function limparContextoSummernote() {
            var campoContexto = obterCampoContextoSummernote();
            campoContexto.html('');
        }




        ng.e2_criarObjetoDadosModalAnoItensAmostraTai = e2_criarObjetoDadosModalAnoItensAmostraTai;
        function e2_criarObjetoDadosModalAnoItensAmostraTai() {
            ng.e2_dadosModalAnoItensAmostraTai = {
                id: 0,
                Ano: { Value: 0, Description: '' },
                Porcentagem: '',
            };
        };

        ng.e2_addDadosModalAnoItensAmostraTai = e2_addDadosModalAnoItensAmostraTai;
        function e2_addDadosModalAnoItensAmostraTai() {
            if (validaPorcentagem()) {
                if (ng.e2_itemParaAlterarDaListaAnosItensAmostraProvaTai != null) {
                    const indexItemDelete = ng.anosItensAmostraProvaTai.indexOf(ng.e2_itemParaAlterarDaListaAnosItensAmostraProvaTai);
                    ng.anosItensAmostraProvaTai.splice(indexItemDelete, 1);
                    ng.e2_itemParaAlterarDaListaAnosItensAmostraProvaTai = null;
                    const itemAlterado = {
                        ...ng.e2_dadosModalAnoItensAmostraTai
                    };
                    ng.anosItensAmostraProvaTai.push(itemAlterado);

                } else {
                    const itemNovo = {
                        ...ng.e2_dadosModalAnoItensAmostraTai
                    };
                    ng.anosItensAmostraProvaTai.push(itemNovo);
                }
                ng.e2_criarObjetoDadosModalAnoItensAmostraTai();
                self.etapa2.alterou = true
                angular.element('#modalAnoItensAmostraTai').modal('hide');
            } else {
                $notification.alert('A soma das porcentagens dos anos escolares não pode ser maior que 100.');
                return false;
            }
        };

        ng.e2_editarModalAnoItensAmostraTai = e2_editarModalAnoItensAmostraTai;
        function e2_editarModalAnoItensAmostraTai(item) {
            e2_criarObjetoDadosModalAnoItensAmostraTai();
            ng.e2_itemParaAlterarDaListaAnosItensAmostraProvaTai = item;
            ng.e2_dadosModalAnoItensAmostraTai = {
                ...item
            };
            angular.element('#modalAnoItensAmostraTai').modal({ backdrop: 'static' });
        };

        ng.e2_confirmarDeletarItemAnosItensAmostraProvaTai = e2_confirmarDeletarItemAnosItensAmostraProvaTai;
        function e2_confirmarDeletarItemAnosItensAmostraProvaTai(item) {
            ng.e2_itemParaDeletarDaListaAnosItensAmostraProvaTai = item;
            angular.element('#modalDeleteItemAnosItensAmostraProvaTai').modal({ backdrop: 'static' });
        };

        ng.e2_deletarItemAnosItensAmostraProvaTai = e2_deletarItemAnosItensAmostraProvaTai;
        function e2_deletarItemAnosItensAmostraProvaTai() {
            if (ng.e2_itemParaDeletarDaListaAnosItensAmostraProvaTai) {
                const indexItemDelete = ng.anosItensAmostraProvaTai.indexOf(ng.e2_itemParaDeletarDaListaAnosItensAmostraProvaTai);
                ng.anosItensAmostraProvaTai.splice(indexItemDelete, 1);
                ng.e2_itemParaDeletarDaListaAnosItensAmostraProvaTai = '';
                self.etapa2.alterou = true
            }
            angular.element('#modalDeleteItemAnosItensAmostraProvaTai').modal('hide');
        };

        ng.e2_limparDadosModalAnoItensAmostraTai = e2_limparDadosModalAnoItensAmostraTai;
        function e2_limparDadosModalAnoItensAmostraTai() {
            ng.e2_dadosModalAnoItensAmostraTai = null;
            ng.e2_itemParaAlterarDaListaAnosItensAmostraProvaTai = null;
        }

        function obterPorcentagemAnoItensAmostraTai() {
            var porcentagem = 0;
            for (var i = 0; i < ng.anosItensAmostraProvaTai.length; i++) {
                var pi = ng.anosItensAmostraProvaTai[i].Porcentagem;
                porcentagem = pi + porcentagem;
            };
            return porcentagem;
        }

        function validaPorcentagem() {
            var porcentagem = obterPorcentagemAnoItensAmostraTai();
            porcentagem = porcentagem + ng.e2_dadosModalAnoItensAmostraTai.Porcentagem;
            if (ng.e2_itemParaAlterarDaListaAnosItensAmostraProvaTai != null)
                porcentagem = porcentagem - ng.e2_itemParaAlterarDaListaAnosItensAmostraProvaTai.Porcentagem;
            if (porcentagem > 100) {
                return false;
            }
            return true;
        }     

        function habilitarComboMatriz() {

            if (!ng.e2_cbComponenteCurricular) {
                $(".comboMatriz").prop("disabled", true);
            }
            else {
                $(".comboMatriz").prop("disabled", false);
            }
        }

        function carregaMatrizAvaliacao() {
            try {
                EvaluationMatrixModel.loadByMatriz({ Id: ng.e2_cbComponenteCurricular.Id }, function (result) {
                    if (result.success) {
                        ng.e2_matrizAvaliacaoList = result.lista;
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
            catch (error) {
                $notification.error("Não há nenhuma matriz cadastrada!");
            }

        };

        function e2_ComponenteCurricularCarregado(r) {

            if (r.success) {
                if (ng.editMode)
                    return;

                ng.e2_listaComponenteCurricular = angular.copy(r.lista);

                if (ng.e2_cbComponenteCurricular)
                    ng.e2_cbComponenteCurricular = procurarElementoEm([ng.e2_cbComponenteCurricular], ng.e2_listaComponenteCurricular)[0];

                ng.bComponente = true;
            } else {
                if (r.type && r.message)
                    $notification[r.type ? r.type : 'error'](r.message);
                return false;
            }
        };

        ng.e2_ComponenteCurricularMudou = e2_ComponenteCurricularMudou;
        function e2_ComponenteCurricularMudou() {

            habilitarComboMatriz();

            ng.e2_matrizAvaliacaoList = [];
            if (!ng.e2_cbComponenteCurricular)
                return;

            carregaMatrizAvaliacao();

            if (ng.mostrarTela) ng.alterouEtapaAtual = self.etapa2.alterou = true;
        };

        function e2_ComponenteCurricularCarregar(tipoNivelEnsino) {
            if (tipoNivelEnsino || tipoNivelEnsino == 0) {
                self.etapa1.componenteCurricular({ typeLevelEducation: tipoNivelEnsino.Id }, e2_ComponenteCurricularCarregado);
            }
            else {
                $notification.alert('Este tipo de prova não possui nível ensino cadastrado.')
            }
        };

        function e2_mapearParaListaTestTaiCurriculumGradeSave() {
            var idComponente = ng.e2_cbComponenteCurricular.Id;
            var idMatriz = ng.e2_matrizAvaliacao.Id;
            ng.anosItensAmostraProvaTai

            var listaTestTaiCurriculumGradeSave = [];
            for (var i = 0; i < ng.anosItensAmostraProvaTai.length; i++) {
                var anoItem = ng.anosItensAmostraProvaTai[i];
                var item = {
                    id: 0,
                    disciplineId: idComponente,
                    matrixId: idMatriz,
                    typeCurriculumGradeId: anoItem.Ano.Id,
                    percentage: anoItem.Porcentagem,
                    testId: ng.provaId
                };
                listaTestTaiCurriculumGradeSave.push(item);
            };
            return listaTestTaiCurriculumGradeSave;
        }

    };

})(angular, jQuery);