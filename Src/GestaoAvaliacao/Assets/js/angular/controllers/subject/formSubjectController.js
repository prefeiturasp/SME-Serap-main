/**
 * function FormSubjectController Controller
 * @namespace Controller
 * @author Jessica Sartori 04/04/2017
 */
(function (angular, $) {

    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives']);

    //~GETTER
    angular
        .module('appMain')
        .controller("FormSubjectController", FormSubjectController);


    FormSubjectController.$inject = ['$scope', 'SubjectModel', '$notification', '$location', '$anchorScroll', '$util', '$timeout'];


    function FormSubjectController(ng, SubjectModel, $notification, $location, $anchorScroll, $util, $timeout) {

        /**
         * @function Inicialização
         * @private
         * @param
         */
        function loaded() {

            $notification.clear();

            configInternalObjects();

            loadSubject(ng.params.Id);
        };

        $(".comboAreaConhecimento").select2(
        {
            placeholder: "Selecione uma ou mais áreas de conhecimento",
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

        verificarVisibilidadeComboComponenteCurricular();

        $(".comboComponenteCurricular").select2({
            placeholder: "Selecione um ou mais componentes curriculares"
        });

        function verificarVisibilidadeComboComponenteCurricular() {

            var areaConhecimento = $('.comboAreaConhecimento').val();

            if (areaConhecimento == null) {
                $(".comboComponenteCurricular").prop("disabled", true);
            }
            else {
                $(".comboComponenteCurricular").prop("disabled", false);
                carregarComboComponenteCurricular();
            }
        }

        function carregarComboComponenteCurricular() {

            $(".comboComponenteCurricular").select2(
            {
                placeholder: "Selecione um ou mais componentes curriculares",
                width: '100%',
                ajax: {
                    url: "loaddisciplinebyknowledgearea",
                    dataType: 'json',
                    data: function (params, page) {
                        console.log($('.comboAreaConhecimento').val());
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
        }

        $('.comboAreaConhecimento').on("select2:select", function (e) {
            verificarVisibilidadeComboComponenteCurricular()
        });

        $('.comboAreaConhecimento').on('select2:unselecting', function (event) {
            $(".comboComponenteCurricular").empty().trigger('change');
            carregarComboComponenteCurricular();
        });



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

            ng.validarNomeAssuntoboo = false;
            ng.validarTamanhoNomeAssuntobool = false;
            ng.validarTamanhoNomeSubAssuntobool = false;

            ng.validarNomeModalboo = false;

            //Textos dos placeholders dos botões
            ng.addText = 'Clique para adicionar um subassunto';
            ng.altText = 'Clique para editar o subassunto';
            ng.lckText = 'Clique para bloquear o subassunto';
            ng.delText = 'Clique para remover o subassunto';

            //Nome do assunto
            ng.descriptionField;
            //Valor da situação do assunto
            ng.situacaoModal;
            //Lista de subassuntos presente no assunto
            ng.listaModal = [];

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
        function loadSubject(id) {
            ng.edicao = false;

            if (id != undefined && id > 0) {
                var bd = { Id: id };

                ng.edicao = true;

                //Pega elemento caso exista, senão cria um novo
                SubjectModel.get(bd, function (result) {

                    if (result.success) {
                        ng.assunto = result.modelList;

                        ng.assunto.SubSubject.forEach(function (e, i, a) {
                            a[i] = new Segmento(e);
                            a[i].lock = true;
                            a[i].class = ng.iLock;
                        });

                        var arrKnowledgeArea = [];
                        ng.assunto.KnowledgeArea.forEach(function (e, i, a) {
                            arrKnowledgeArea.push(e.Description);
                        });

                        var arrDiscipline = [];
                        ng.assunto.Discipline.forEach(function (e, i, a) {
                            arrDiscipline.push(e.Description);
                        });

                        $('.labelAreaConhecimento').val(arrKnowledgeArea.join(", "));
                        $('.labelComponenteCurricular').val(arrDiscipline.join(", "));

                        ng.listaModal = ng.assunto.SubSubject;
                        ng.descriptionField = ng.assunto.Description;
                        ng.situacaoModal = ng.situacaoOptions[ng.assunto.State < 1 ? undefined : ng.assunto.State - 1];
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
         * @function Segmento: elementos que descrevem os subassuntos
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
        * @function - Validadar a qualtidade máxima de caracteres para campo 'Nome do assunto'
        * @public
        * param
        */
        ng.validadeNomeAssunto = function () {

            ng.validarTamanhoNomeAssuntobool = ng.descriptionField.length > 200;
        };

        /**
       * @function - Validadar a qualtidade máxima de caracteres para campo 'Nome do subassunto'
       * @public
       * param
       */
        ng.validadeNomeSubAssunto = function (subassunto) {

            ng.validarTamanhoNomeSubAssuntobool = subassunto.Description.length > 200;
        };

        /**
        * @function - Cria/Assossia dados ao modal e liga ao campos
        * @private
        * @params {object} dados Dados que serão preenchidos no modal
        */
        function setModal(dado) {
            ng.modal = dado || new Assunto();
            ng.descriptionField = ng.modal.Description;
            ng.situacaoModal = ng.modal.status;
            ng.listaModal = ng.modal.TestSubGroups || (ng.modal.TestSubGroups = []);
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
         * @function - Validar Nome assunto
         * @public
         * @params
         */
        ng.validarNomeAssunto = function () {

            ng.validarNomeAssuntoboo = hasName(ng.descriptionField, ng.listaAssunto, 'Description');
        };

        /**
         * @function - Validar Nome assunto
         * @public
         * @params
         */
        ng.validarNomeModal = function (node) {

            ng.validarNomeModalboo = hasName(ng.listaModal, 'Description');

            if (node != undefined) {

                var index = ng.listaModal.indexOf(node);

                ng.listaModal[index].Description = ng.listaModal[index].Description.substring(0, 400);
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
         * @function - Altera um subassunto do assunto.
         * @public
        * @params {integer} index Posição do elemento clicado.
        */
        ng.altModal = function (index) {

            if (ng.validarTamanhoNomeSubAssuntobool) {
                $notification.alert('Subassunto deve conter até 200 caracteres.');
                return;
            }

            var q = ng.listaModal[index];

            if (q.Description)
                q.lock = !q.lock;
            
            if (ng.validarNomeModalboo) {
                ng.validarNomeModal();
            }
            else {
                if (q.lock)
                    aplicaCSS(q, 2);

                    //Deixa campo editável
                else
                    aplicaCSS(q, 3);
            }
        };

        /**
         * @function - Adiciona um novo subassunto para o assunto.
         * @public
         * @params
        */
        ng.addModal = function () {

            //Verifica se o ultimo elemento esta vázio antes de criar um novo
            if (ng.validarNomeModalboo || ng.ultimo < 0 || ng.listaModal[ng.ultimo] && !ng.listaModal[ng.ultimo].Description)
                return;

            if (ng.validarTamanhoNomeSubAssuntobool)
            {
                $notification.alert('Subassunto deve conter até 200 caracteres.');
                return;
            }

            //Tranca(lock) se não estiver 
            if (ng.listaModal[ng.ultimo])
                if (!ng.listaModal[ng.ultimo].lock)
                    ng.altModal(ng.ultimo);

            var q = new Segmento();

            q.Level = ng.listaModal.length;

            ng.listaModal.push(q);

            ng.ultimo = ng.listaModal.length - 1;
        };

        /**
         * @function - Deleta um subassunto do assunto.
         * @public
         * @params
        */
        ng.delModal = function () {

            angular.element("#modal").modal('hide');

            var id = ng.listaModal[ng.itemDeletado].Id;

            var q = ng.listaModal[ng.itemDeletado];

            q.Description = undefined;
            q.Id = 0;
            q.lock = undefined;

            delete q.Description;
            delete q.Id;
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
         * @function - Salva dados do Modal na lista de assunto.
         * @public
         * @params
        */
        ng.save = function () {

            if ($('.comboAreaConhecimento').val() == null && $('.labelAreaConhecimento').val() == "") {
                $notification.alert('Ao menos uma área de conhecimento deve ser adicionada.');
                return;
            }

            if ($('.comboComponenteCurricular').val() == null && $('.labelComponenteCurricular').val() == "") {
                $notification.alert('Ao menos um componente curricular deve ser adicionado.');
                return;
            }

            //Valida descrição
            if (!ng.descriptionField) {
                $notification.alert('O campo "Assunto" é obrigatório.');
                return;
            }

            if (ng.validarTamanhoNomeAssuntobool)
            {
                $notification.alert('Assunto deve conter até 200 caracteres.');
                return;
            }

            //Valida nome do assunto
            if (ng.validarNomeAssuntoboo) {
                $notification.alert('Não é permitido repetir os nomes dos assuntos.');
                return;
            }

            //Valida nomes dos subassuntos
            if (ng.validarNomeModalboo) {
                $notification.alert('Não é permitido repetir os nomes dos subassuntos.');
                return;
            }

            var i, q, w, m, c, areaConhecimento, disciplinas;

            q = ng.listaModal.length,
            w = ng.listaModal;

            //Valida situação valida item vazio
            for (i = 0 ; i < q; i++) {
                if (!w[i].Description) {
                    return $notification.alert('Por favor, preencha o nome do subassunto.');
                }

                if (w[i].Description.length > 200)
                {
                    return $notification.alert('Existem subassuntos com mais de 200 caracteres.');
                }
            }

            c = [];
            m = {};

            //Retira informações da tela para enviar
            ng.listaModal.forEach(function (e, i, a) {
                var novo = {};

                novo.Description = e.Description;
                novo.Id = e.Id || 0;
                c.push(novo);

            });

            areaConhecimento = [];

            if ($('.comboAreaConhecimento').val() != null) {
                $('.comboAreaConhecimento').val().forEach(function (e, i, a) {
                    var novo = {};

                    novo.Id = e || 0;
                    areaConhecimento.push(novo);

                });
            }
            else {
                ng.assunto.KnowledgeArea.forEach(function (e, i, a) {
                    areaConhecimento.push(e.Id);
                });
            }

            disciplinas = []

            if ($('.comboComponenteCurricular').val() != null) {
                $(".comboComponenteCurricular").val().forEach(function (e, i, a) {
                    var novo = {};

                    novo.Id = e || 0;
                    disciplinas.push(novo);

                });
            }
            else {
                ng.assunto.Discipline.forEach(function (e, i, a) {
                    disciplinas.push(e.Id);
                });
            }

            //Registras dados do assunto para enviar pro server
            m.Id = ng.assunto ? ng.assunto.Id : 0;
            m.Description = ng.descriptionField;

            m.SubSubjects = c;
            m.Disciplines = disciplinas;
            m.KnowledgeAreas = areaConhecimento;
            m.State = ng.situacaoModal.Id;

            //Manda salvar o assunto
            SubjectModel.save(
                //Parametros enviados
                m,

                //Função de retorno
                function (result) {

                    if (result.success) {
                        $notification.success('Assunto ' + (ng.assunto ? 'alterado' : 'salvo') + ' com sucesso!');
                        $timeout(function () {
                            window.location.href = '/Subject/List';
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

            if (ng.listaModal[ng.itemDeletado].Description)
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