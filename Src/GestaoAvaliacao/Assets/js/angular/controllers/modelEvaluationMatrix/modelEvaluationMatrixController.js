/**
 * function ModelEvaluationMatrix Controller
 * @namespace Controller
 * @author Alexandre Calil Blasizza Paravani 16/04/2015
 * @author Alexandre Garcia Simões 10/06/2015
 * @author Thiago Macedo Silvestre 16/09/2015
 * @author Leticia Langeli Garcia De Goes 30/10/2015
 * @author Julio Cesar da Silva 10/11/2015
 */
(function (angular, $) {

    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives']);

    //~GETTER
    angular
        .module('appMain')
        .controller("ModelEvaluationMatrixController", ModelEvaluationMatrixController);


    ModelEvaluationMatrixController.$inject = ['$scope', 'ModelEvaluationMatrixModel', '$notification', '$location', '$anchorScroll', '$util', '$timeout'];


    function ModelEvaluationMatrixController(ng, ModelEvaluationMatrixModel, $notification, $location, $anchorScroll, $util, $timeout) {

        /**
         * @function Inicialização
         * @private
         * @param
         */
        function loaded() {

            $notification.clear();

            configInternalObjects();

            loadModelEvaluationMatrix(ng.params.Id);
        };
      
        /**
         * @function Configuração dos objetos e variaveis de uso interno
         * @private
         * @param
         */
        function configInternalObjects() {

            ng.listaErrados = [];

            ng.iNormal = 'form-control';
            ng.iLock = 'inputBack form-control';
            ng.iWrong = 'inputWrong form-control';

            ng.validarNomeModeloMatrizboo = false;

            ng.validarNomeModalboo = false;
            
            //Textos dos placeholders dos botões
            ng.addText = 'Clique para adicionar um nível';
            ng.altText = 'Clique para editar o nível';
            ng.lckText = 'Clique para bloquear o nível';
            ng.delText = 'Clique para remover o nível';

            //Nome do modelo de matriz
            ng.descriptionField;
            //Valor da situação do modelo de matriz
            ng.situacaoModal;
            //Lista de níveis presente no modelo de matriz
            ng.listaModal = [];
            //Ultimo nível do modelo de matriz atual
            ng.ultimo = 0;

            //Modal atual
            ng.modal;

            //Valores para situação
            ng.situacaoOptions = [
                {
                    Id: 1,
                    Description: 'Ativo'
                },
                {
                    Id: 2,
                    Description: 'Inativo'
                }
            ];
            ng.situacaoModal = ng.situacaoOptions[0];
            ng.itemDeletado = 0;
        };

        /**
         * @function Carrega parametros quando acessar pagina
         * @private
         * @param {Object} current
         */
        function loadModelEvaluationMatrix(id) {
            
            if (id != undefined && id > 0) {
                var bd = { Id: id };

                //Pega elemento caso exista, senão cria um novo
                ModelEvaluationMatrixModel.get(bd, function (result) {

                        if (result.success) {
                            ng.matriz = result.modelList;

                            ng.matriz.ModelSkillLevels.forEach(function (e, i, a) {
                                a[i] = new Segmento(e);
                                a[i].lock = true;
                                a[i].class = ng.iLock;
                            });

                            ng.listaModal = ng.matriz.ModelSkillLevels;
                            ng.descriptionField = ng.matriz.Description;
                            ng.situacaoModal = ng.situacaoOptions[ng.matriz.State < 1 ? undefined : ng.matriz.State - 1];
                        }
                        else {
                            newModal();
                        }
                    });
            }
            else {
                newModal();
            }
        };

        /**
         * @function Segmento: elementos que descrevem os subníveis
         * @private
         * @param {Object} current
         */
        function Segmento(obj) {

            //ID
            this.Id = obj ? obj.Id : 0;
            //Nome
            this.Description = obj ? obj.Description : undefined;
            //Nível
            this.Level = obj ? obj.Level : 0;
            // Evita edição
            this.LastLevel = obj ? obj.Id : false;
            this.class = 'form-control';
        };

        /**
         * @function - Aplica dados existentes no modal
         * @private
        * @params {object} dados Dados que serão preenchidos no modal
        */
        function popularModal(dados) {

            setModal(dados);

            ng.ultimo = undefined;

            ng.callModal();
        };

        /**
        * @function - Validadar a qualtidade máxima de caracteres para campo 'Nome do modelo de matriz'
        * @public
        * param
        */
        ng.validadeNomeModeloMatriz = function () {

            ng.descriptionField = ng.descriptionField.substring(0, 50);
        };

        /**
        * @function - Cria/Assossia dados ao modal e liga ao campos
        * @private
        * @params {object} dados Dados que serão preenchidos no modal
        */
        function setModal(dado) {

            ng.modal = dado || new ModeloMatriz();
            ng.descriptionField = ng.modal.Description;
            ng.situacaoModal = ng.modal.status;
            ng.listaModal = ng.modal.ModelSkillLevels || (ng.modal.ModelSkillLevels = []);
            ng.ultimo = 0;
        };

        /**
         * @function - Cria/Assossia dados ao modal e liga ao campos
         * @private
         * @params {object} dados Dados que serão preenchidos no modal
         */
        function validarListaErrados() {

            var a;

            for (a in ng.listaErrados) {
                if (!ng.listaErrados[a])
                    delete ng.listaErrados[a];
            }
        };

        /**
         * @function - Varre array e valida se o nome já existe
         * @private
        * @params {array}   array Array em que será buscado
        * @params {string}   campo Campo que será comparado ao nome
        */
        function hasName(array, campo) {
           
            var q = 0,
                w = 0,
                e = array.length,
                obj,
                repetiu = false,
                repetiuAlgum = false;

            //Varre todos os elementos
            for (q; q < e; q++) {
                obj = array[q];

                //Impede validar campos vazios
                if (obj[campo])

                    //Varre todos exceto o atual 
                    for (w = 0; w < e; w++) {

                        //Impede comparação do mesmo elemento
                        if (q === w) continue;
                    
                        //Valida igualdade do campo
                        if (obj[campo] === array[w][campo]) {
                            aplicaCSS(array[w], 1);
                            repetiuAlgum = repetiu = true;
                        }
                        else {
                            //Impede que alterer elemento Repetido, diferente do item atual
                            if (array[w].class === ng.iWrong) continue;

                            //Se tiver lock, mantem
                            if (array[w].lock)
                                aplicaCSS(array[w], 2);

                                //Deixa campo editável
                            else
                                aplicaCSS(array[w], 3);
                        }
                    }

                //Aplica CSS no item atual
                if (repetiu) {
                    aplicaCSS(obj, 1);
                    repetiu = false;
                }
                else if (obj.lock)
                    aplicaCSS(obj, 2);
                else
                    aplicaCSS(obj, 3);

            }

            return repetiuAlgum;
        };

        /**
         * @function - Aplica CSS
         * @private
         * @params
         */
        function aplicaCSS(obj, t) {
            if (t === 1) {
                obj.class = ng.iWrong;
            }
            else if (t === 2) {
                obj.class = ng.iLock;
            }
            else {
                obj.class = ng.iNormal;
            }
        };

        /**
         * @function - Validar Nome Modelo Matriz
         * @public
         * @params
         */
        ng.validarNomeModeloMatriz = function () {

            ng.validarNomeModeloMatrizboo = hasName(ng.descriptionField, ng.listaModeloMatriz, 'Description');
        };

        /**
         * @function - Validar Nome Modelo Matriz
         * @public
         * @params
         */
        ng.validarNomeModal = function (node) {
       
            ng.validarNomeModalboo = hasName(ng.listaModal, 'Description');

            if (node != undefined) {

                var index = ng.listaModal.indexOf(node);

                ng.listaModal[index].Description = ng.listaModal[index].Description.substring(0, 50);
            }
        };



        /**
         * @function - Salva os elementos no servidor
         * @public
        * @params {integer} index Posição do elemento clicado.
        * @return {boolean} se elemento é o ultimo da listaModal
        */
        ng.last = function (index) {
            return index === ng.listaModal.length - 1;
        };

        /**
         * @function - Altara um nível do modelo de matriz.
         * @public
        * @params {integer} index Posição do elemento clicado.
        */
        ng.altModal = function (index) {

            var q = ng.listaModal[index];

            if (q.Description)
                q.lock = !q.lock;

            if( ng.validarNomeModalboo )
                ng.validarNomeModal();
            else {
                if (q.lock)
                    aplicaCSS(q, 2);

                    //Deixa campo editável
                else
                    aplicaCSS(q, 3);
            }
        };

        /**
         * @function - Adiciona um novo nível para o modelo de matriz.
         * @public
         * @params
        */
        ng.addModal = function () {

            //Verifica se o ultimo elemento esta vázio antes de criar um novo
            if (ng.validarNomeModalboo || ng.ultimo < 0 || ng.listaModal[ng.ultimo] && !ng.listaModal[ng.ultimo].Description)
                return;


            //Tranca(lock) se não estiver 
            if (ng.listaModal[ng.ultimo])
                if (!ng.listaModal[ng.ultimo].lock )
                    ng.altModal(ng.ultimo);

            var q = new Segmento();

            q.Level = ng.listaModal.length;

            ng.listaModal.push(q);

            ng.ultimo = ng.listaModal.length - 1;
        };
        
        /**
         * @function - Deleta um nível do modelo de matriz.
         * @public
         * @params
        */
        ng.delModal = function () {
            angular.element("#modal").modal('hide');

            var q = ng.listaModal[ng.itemDeletado];

            q.Description = undefined;
            q.Id = 0;
            q.Level = undefined;
            q.LastLevel = undefined;
            q.lock = undefined;


            delete q.Description;
            delete q.Id;
            delete q.Level;
            delete q.LastLevel;
            delete q.lock;

            q = null;
            ng.listaModal[ng.itemDeletado] = null;
            ng.listaModal.splice(ng.itemDeletado, 1);

            ng.itemDeletado = undefined;


            if (ng.listaModal.length < 1)
                ng.addModal();// inicia um elemento
            else
                ng.ultimo = ng.listaModal.length - 1;
            
            
            ng.validarNomeModal();
        }; 
        
        /**
         * @function - Salva dados do Modal na lista de modelo.
         * @public
         * @params
        */
        ng.save = function () {

            //Valida descrição
            if (!ng.descriptionField) {
                $notification.alert('O campo "Nome do modelo de matriz" é obrigatório');
                return;
            }

            //Valida nome do modelos de matriz
            if (ng.validarNomeModeloMatrizboo) {
                $notification.alert('Não é permitido repetir os nomes dos modelos de matriz.');
                return;
            }


            //Valida nomes dos níveis
            if (ng.validarNomeModalboo) {
                $notification.alert('Não é permitido repetir os nomes dos níveis.');
                return;
            }

            var i, q, w, m, c;

            q = ng.listaModal.length,
            w = ng.listaModal;

            //Valida situação valida item vazio
            for (i = 0 ; i < q; i++) {
                if (!w[i].Description) {
                    return $notification.alert('Por favor, preencha o nome do nível ' + ((i + 1) ? (i + 1) : (i + 1) + ', ou remova-o'));
                }
            }

            c = [];
            m = {};

            //Retira informações da tela para enviar
            ng.listaModal.forEach(function (e, i, a) {
                var novo = {};

                novo.Description = e.Description;
                novo.Id = e.Id || 0;
                novo.Level = i + 1;
                novo.LastLevel = ((i + 1) === a.length);
                c.push(novo);

            });

            //Registras dados do Modelo de matriz para enviar pro server
            m.Id = ng.matriz ? ng.matriz.Id : 0;
            m.Description = ng.descriptionField;
            m.ModelSkillLevels = c;
            m.State = ng.situacaoModal.Id;

            //Manda salvar o modelo de matriz
            ModelEvaluationMatrixModel.save(
                //Parametros enviados
                m,

                //Função de retorno
                function (result) {

                    if (result.success) {
                        $notification.success('Modelo ' + (ng.matriz ? 'alterado' : 'salvo') + ' com sucesso!');
                        $timeout(function () {
                            window.location.href = '/ModelEvaluationMatrix/Index';
                        }, 3000);
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                }
            );

            m = null;
            c = null;
        };
        
        /**
         * @function - Fecha modal
         * @public
         * @params
        */
        ng.clsModal = function () {

            ng.modal = undefined;

            $notification.clear();
            angular.element("#modal").modal("hide");
        };
        
        /**
         * @function - configura o item a ser deletado
         * @public
         * @params {int} i
         */
        ng.callModal = function (i) {

            ng.itemDeletado = i;
            
            if (ng.listaModal[ng.itemDeletado].Description )
                angular.element("#modal").modal({ backdrop: 'static' });
            else {
                ng.delModal();
            }
        };
        
        /**
         * @function - Cria nova lista para edição
         * @public
         * @params
         */
        function newModal() {

            ng.listaModal = [];

            if (ng.listaModal.length < 1)
                ng.listaModal.push(new Segmento());

            ng.ultimo = ng.listaModal.length - 1;
        };


        ng.params = $util.getUrlParams();
        loaded();
    };


})(angular, jQuery);