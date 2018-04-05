/**
 * function TestListController Controller
 * @namespace Controller
 * @author julio cesar da silva - 14/05/2015
 */
(function (angular, $) {

	'use strict';

	//~SETTER
	angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

	//~GETTER
	angular
		.module('appMain')
		.controller("ModelTestController", ModelTestController);

	ModelTestController.$inject = ['$rootScope', '$scope', '$window', '$sce', '$util', '$notification', '$timeout', 'ModelTestModel'];


	/**
	 * @function Controller 'Modelo de prova'
	 * @name ModelTestController
	 * @namespace Controller
	 * @memberOf appMain
	 * @param {Object} $rootScope
	 * @param {Object} $scope
	 * @param {Object} $notification
	 * @param {Object} $timeout
	 * @return
	 */
	function ModelTestController($rootScope, $scope, $window, $sce, $util, $notification, $timeout, ModelTestModel) {
		
		var EnumPosition = {
			1: "Esquerda",
			2: "Centro",
			3: "Direita"
		};

		var EnumPositionInverse = {
			"Esquerda": 1,
			"Centro": 2,
			"Direita": 3 
		};

		var EnumSize = {
			1: "Médio (padrão)",
			2: "Pequeno",
			3: "Grande"
		};

		var EnumSizeInverse = {
			"Médio (padrão)": 1,
			"Pequeno": 2,
			"Grande": 3
		};

		//parâmetros enviados via url
		var params  = $util.getUrlParams();

		//foco no nome do modelo de prova quando carregada a página
		angular.element(document).ready(function () {
			angular.element('#modelTestDescription').focus();
		});

		setTypeFileUpload();

		//funcionalidade post diggest angulaJS
		var hasRegistered = false;
		$scope.$watch(function __cycleAngular() {
			if (hasRegistered) return;
			hasRegistered = true;
			$scope.$$postDigest(function __postDisgestAngular() {
				hasRegistered = false;
				//TODO
			});
		});


		/**
		 * @function Inicialização.
		 * @name initialize
		 * @namespace TestController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		(function initialize() {

			//edit/novo modelo
			if (params.Id != undefined && params.Id != null)
				loadTestModel();
			else
				configNewTestModel();

			//default Tab(Aba) conteúdo
			$scope.currentTab = 'conteudoTab';

		}).call(this);

		/**
		 * @function cria a lista do tipod e arquivos permitidos no upload.
		 * @name setTypeFileUpload
		 * @namespace TestController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		function setTypeFileUpload() {

			var paramsGeral = Parameters.General;
			$scope.typeFile = [];

			if (paramsGeral.GIF == "True") {
				$scope.typeFile.push('image/gif');
			}

			if (paramsGeral.PNG == "True") {
				$scope.typeFile.push('image/png');
			}

			if (paramsGeral.BMP == "True") {
				$scope.typeFile.push('image/bmp');
			}

			if (paramsGeral.JPEG == "True") {
				$scope.typeFile.push('image/jpeg');
			}

		};//setTypeFileUpload


		/**
		 * @function Carregar um modelo de prova
		 * @name save
		 * @namespace TestController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		function loadTestModel() {
			ModelTestModel.find({ Id: params.Id }, function (result) {
				if (result.success) {
					if (result.success) {
						$scope.modelTest = parseEnumFormServer(result.modelTest);
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
						redirectWhenError();
					}
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
					redirectWhenError();
				}
			});
		};


		/**
		 * @function Formatar os Enums recebidos do server
		 * @name parseEnumFormServer
		 * @namespace TestController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		function parseEnumFormServer(modelTest) {
			modelTest.LogoHeaderPosition = EnumPosition[modelTest.LogoHeaderPosition.toString()];
			modelTest.MessageHeaderPosition = EnumPosition[modelTest.MessageHeaderPosition.toString()];
			modelTest.LogoFooterPosition = EnumPosition[modelTest.LogoFooterPosition.toString()];
			modelTest.MessageFooterPosition = EnumPosition[modelTest.MessageFooterPosition.toString()];
			modelTest.LogoHeaderSize = EnumSize[modelTest.LogoHeaderSize.toString()];
			modelTest.LogoFooterSize = EnumSize[modelTest.LogoFooterSize.toString()];
			if (modelTest.LogoHeader === undefined || modelTest.LogoHeader === null) {
				modelTest.LogoHeader = {
					Id: undefined,
					Path: undefined,
					Guid: $util.getGuid()
				};
			}
			else {
				modelTest.LogoHeader['Guid'] = $util.getGuid();
			}
			if (modelTest.LogoFooter === undefined || modelTest.LogoFooter === null) {
				modelTest.LogoFooter = {
					Id: undefined,
					Path: undefined,
					Guid: $util.getGuid()
				};
			}
			else {
				modelTest.LogoFooter['Guid'] = $util.getGuid();
			}
			modelTest.MessageHeader = $scope.converterBrToBreakLine(modelTest.MessageHeader);
			modelTest.MessageFooter = $scope.converterBrToBreakLine(modelTest.MessageFooter);

			return modelTest;
		};


		/**
		 * @function Formatar os Enums enviado para server
		 * @name parseEnumFormServer
		 * @namespace TestController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		function parseEnumToServer(modelTest) {
			var _modelTest = angular.copy(modelTest);
			_modelTest.LogoHeaderPosition = parseInt(EnumPositionInverse[modelTest.LogoHeaderPosition]);
			_modelTest.MessageHeaderPosition = parseInt(EnumPositionInverse[modelTest.MessageHeaderPosition]);
			_modelTest.LogoFooterPosition = parseInt(EnumPositionInverse[modelTest.LogoFooterPosition]);
			_modelTest.MessageFooterPosition = parseInt(EnumPositionInverse[modelTest.MessageFooterPosition]);
			_modelTest.LogoHeaderSize = parseInt(EnumSizeInverse[modelTest.LogoHeaderSize]);
			_modelTest.LogoFooterSize = parseInt(EnumSizeInverse[modelTest.LogoFooterSize]);
			_modelTest.MessageHeader = $scope.converterBreakLineToBr(modelTest.MessageHeader);
			_modelTest.MessageFooter = $scope.converterBreakLineToBr(modelTest.MessageFooter);
			return _modelTest;
		};

		/**
		 * @function Formatar os Enums enviado para server
		 * @name parseEnumFormServer
		 * @namespace TestController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		function parseEnumToServerMensenger(modelTest) {
			var _modelTest = angular.copy(modelTest);
			_modelTest.MessageHeader = $scope.converterBreakLineToBr(modelTest.MessageHeader);
			_modelTest.MessageFooter = $scope.converterBreakLineToBr(modelTest.MessageFooter);
			return _modelTest;
		};


		/**
		 * @function Redirecionar quando os parêmetros enviados via url não forem válidos.
		 * @name redirectWhenError
		 * @namespace TestController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		function redirectWhenError() {
			$timeout(function () {
				$window.history.back();
			}, 3000);
		};


		/**
		 * @function Config novo modelo prova
		 * @name save
		 * @namespace TestController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		function configNewTestModel() {

			$scope.modelTest = {
				//Geral
				Description: undefined,
				Html: undefined,
				DefaultModel: false,
				//Opções do modelo
				ShowCoverPage: true,
				ShowBorderOnCoverPage: false,
				ShowBorder: true,
				//Cabeçalho
				LogoHeaderPosition: 'Esquerda',
				LogoHeaderSize: 'Médio (padrão)',
				ShowHeaderOnCoverPage: false,
				LogoHeaderWaterMark: false,
				MessageHeaderPosition: 'Centro',
				MessageHeader: "",
				ShowMessageHeader: true,
				MessageHeaderWaterMark: false,
				ShowLineBelowHeader: true,
				ShowLogoHeader: true,
				LogoHeader: {
					Id: undefined,
					Path: undefined,
					Guid:$util.getGuid()
				},
				//Rodapé
				LogoFooterPosition: 'Esquerda',
				LogoFooterSize: 'Médio (padrão)',
				LogoFooterWaterMark: false,
				MessageFooterPosition: 'Centro',
				MessageFooter: "",
				ShowMessageFooter: true,
				MessageFooterWaterMark: false,
				ShowLineAboveFooter: true,
				ShowLogoFooter: true,
				LogoFooter: {
					Id: undefined,
					Path: undefined,
					Guid: $util.getGuid()
				},
				//Dados do Estudante/Escola
				ShowSchool: true,
				ShowStudentName: true,
				ShowTeacherName: true,
				ShowClassName: true,
				ShowStudentNumber: true,
				ShowDate: true,
				ShowLineBelowStudentInformation: true,
			    //Item da prova
			    ShowItemLine: true,
				//DadosCapa
				CoverPageText: undefined,
				ShowStudentInformationsOnCoverPage: false,
				ShowFooterOnCoverPage: false,
				Files: []
			};
		};


		/**
		 * @function Salvar modelo de prova
		 * @name save
		 * @namespace TestController
		 * @memberOf Controller
		 * @param {boolean} formInvalid
		 * @return
		 */
		$scope.save = function __save(formInvalid) {
			if (!formInvalid) {
				var parserMensager = parseEnumToServerMensenger($scope.modelTest);
				$scope.modelTest.HeaderHtml = HeaderHtmlFormat(parserMensager.MessageHeader);
				$scope.modelTest.StudentInformationHtml = StudentInformationHtmlFormat();
				$scope.modelTest.FooterHtml = FooterHtmlFormat(parserMensager.MessageFooter);
				var wrapper = parseEnumToServer($scope.modelTest);

				ModelTestModel.save({ entity: wrapper, files: $scope.modelTest.Files }, function (result) {
					if (result.success) {
						$notification.success("Modelo de prova salvo com sucesso!");
						$scope.modelTest.Id = result.id;
						$timeout(function () {
						    $window.location.href = "/ModelTest/Index";
						}, 3000);
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
				
			}
			else {
				$notification.alert("O campo 'Nome do modelo de prova' é obrigatório.")
			}
		};

		/**
		 * @function Formata o html do cabeçalho 
		 * @name HeaderHtmlFormat
		 * @namespace TestController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		function HeaderHtmlFormat(mensager) {

			var content = $('<div id="modeloProva" class="tab-content"></div>');
			var Header = $('<div id="modelTestHeader" class="campoLabel previewTestPosition"></div>');
			
			//Heade 
			Header = templateFooterAndHeader($scope.modelTest.LogoHeaderPosition, Header, getCssHeaderImageName($scope.getCssHeaderImage()), $scope.modelTest.LogoHeader.Path);
			Header = showMessagerAll($scope.modelTest.ShowMessageHeader, mensager, Header, $scope.modelTest.MessageHeaderWaterMark, $scope.modelTest.MessageHeaderPosition);			
			content.append(Header);

			if ($scope.modelTest.ShowLineBelowHeader)
				return content.html() + '<hr>';
			else
				return content.html();

		};
		
		/**
		 * @function Formatar o html das informações do estudante
		 * @name StudentInformationHtmlFormat
		 * @namespace TestController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		function StudentInformationHtmlFormat() {

			var content = $('<div id="modeloProva" class="tab-content"></div>');
			var StudentData = $('<div id="modelTestStudentData" class="row testStudentData"></div>');

			//StudentData
			StudentData.append(templateStudentInformation());
			
			content.append(StudentData);

			/*if ($scope.modelTest.ShowLineBelowStudentInformation)
				return content.html() + '<hr>';
			else*/
				return content.html();
		};

		/**
		 * @function Formata o html do rodapé
		 * @name FooterHtmlFormat
		 * @namespace TestController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		function FooterHtmlFormat(mensager) {

			var content = $('<div id="modeloProva" class="tab-content"></div>');
			var Footer = $('<div id="modelTestFooter" class="campoLabel previewTestPosition"></div>');
			Footer = templateFooterAndHeader($scope.modelTest.LogoFooterPosition, Footer, getCssHeaderImageName($scope.getCssFooterImage()), $scope.modelTest.LogoFooter.Path);
			Footer = showMessagerAll($scope.modelTest.ShowMessageFooter, mensager, Footer, $scope.modelTest.MessageFooterWaterMark, $scope.modelTest.MessageFooterPosition);
			content.append(Footer);

			if ($scope.modelTest.ShowLineAboveFooter)
				return '<hr>' + content.html();
			else
				return content.html();
		};

		/**
		 * @function Formatação das informações dos alunos
		 * @name templateStudentInformation
		 * @namespace TestController
		 * @memberOf Controller
		 * @param 
		 * @return  
		 */
		function templateStudentInformation() {

			var StudentInformation = $('<div class="col-md-12"></div>');

			if ($scope.modelTest.ShowSchool) {
				var School = $('<div class="campoLabel col-md-12"><label>Escola: </label><div><input class="form-control" type="text" #school# /></div></div>');
				StudentInformation.append(School);
			}

			if ($scope.modelTest.ShowStudentName) {
			    var StudentName = $('<div class="campoLabel col-md-12"><label>Nome: </label><div><input class="form-control" type="text" #studentname# /></div></div>');
				StudentInformation.append(StudentName);
			}

			if ($scope.modelTest.ShowClassName) {
			    var ClassName = $('<div class="campoLabel col-md-8"><label>Turma: </label><div><input class="form-control" type="text" #classname# /></div></div>');
			    StudentInformation.append(ClassName);
			}

			if ($scope.modelTest.ShowStudentNumber) {
				var StudentNumber = $('<div class="campoLabel col-md-4"><label>Número: </label><div><input class="form-control" type="text" #studentnumber# /></div></div>');
				StudentInformation.append(StudentNumber);
			}

			if ($scope.modelTest.ShowTeacherName) {
			    var TeacherName = $('<div class="campoLabel col-md-8"><label>Professor: </label><div><input class="form-control" type="text" #teachername# /></div></div>');
			    StudentInformation.append(TeacherName);
			}

			if ($scope.modelTest.ShowDate) {
				var Date = $('<div class="campoLabel col-md-4"><label>Data: </label><div><input class="form-control" type="text" #date# /></div></div>');
				StudentInformation.append(Date);
			}

			return StudentInformation;
		};

		/**
		 * @function 
		 * @name showMessagerAll
		 * @namespace TestController
		 * @memberOf Controller
		 * @param showMessager - se a menssager será exibida ou não
		 * @param menssager - mensage a ser exibida
		 * @param html - mensage a ser exibida
		 * @param messageWaterMark - mensage a ser exibida
		 * @param position - Posição que a mensage ira ser exibida esquedaa, direira ou no centro
		 * @param html
		 * @return html 
		 */
		function showMessagerAll(showMessager, menssager, html, messageWaterMark, position) {
			
			if (showMessager) {
				var Message = $('<div class="addMessage"></div>');
				if (messageWaterMark) Message.attr('class', 'watermark');
				Message.append(menssager);
				if (position == 'Esquerda') {
					$(html).find('.leftTest').append(Message);
				} else if (position == 'Centro') {
					$(html).find('.centerTest').append(Message);
				} else if (position == 'Direita') {
					$(html).find('.rightTest').append(Message);
				}
			}

			return html;
		};

		/**
		 * @function Formatação da rodapé
		 * @name templateFooter
		 * @namespace TestController
		 * @memberOf Controller
		 * @param footer
		 * @return footer 
		 */
		function templateFooterAndHeader(position, htmlLogo, img, url) {

			var left = $('<div class="leftTest"></div>');
			var center = $('<div class="centerTest"></div>');
			var right = $('<div class="rightTest"></div>');

			if (position == 'Esquerda') {
				htmlLogo.append(setImageHeader(left, img, url));
			} else if (position == 'Centro') {
				htmlLogo.append(setImageHeader(center, img, url));
			} else if (position == 'Direita') {
				htmlLogo.append(setImageHeader(right, img, url));
			}

			htmlLogo.append(left);
			htmlLogo.append(center);
			htmlLogo.append(right);

			return htmlLogo;
		};

		/**
		 * @function Inclui a imagem ou não 
		 * @name setImageHeader
		 * @namespace TestController
		 * @memberOf Controller
		 * @param div
		 * @return div 
		 */
		function setImageHeader(div, img, url) {

			if (!url) {
				var Path = $('<div class="thumbnail-logo"></div>');
				Path.css('class', img);
				div.append(Path);
			}

			if (url) {
				var LogoImg = $('<div class="image-logo"><img/></div>');
				LogoImg.children().attr('src', url).attr('class', img);
				div.append(LogoImg);
			}

			return div;
		};

		/**
		 * @function Retorna o nome da class que estiver com parametro true
		 * @name getCssHeaderImageName
		 * @namespace TestController
		 * @memberOf Controller
		 * @param getCss
		 * @return getCss 
		 */
		function getCssHeaderImageName(getCss) {

			var concat = "";

			if (getCss.logomedium) {
				concat += ' logomedium'
			}
			if (getCss.logosmall) {
				concat += ' logosmall'
			}
			if (getCss.logobig) {
				concat += ' logobig'
			}
			if (getCss.watermark) {
				concat += ' watermark';
			}
			return concat;
		};

		/**
		 * @function Modal de conformação de cancelamento
		 * @name cancelModal
		 * @namespace TestController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		$scope.cancelModal = function __cancelModal() {
			angular.element('#cancelModal').modal({ backdrop: 'static' });
		};


		/**
		 * @function Cancelar
		 * @name cancel
		 * @namespace TestController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		$scope.cancel = function __cancel() {
			angular.element('#cancelModal').modal('hide');
			redirectToSearch();
		};


		/**
		 * @function Set Tab
		 * @name setTab
		 * @namespace TestController
		 * @memberOf Controller
		 * @param {string} value
		 * @return
		 */
		$scope.setTab = function __setTab(value) {
			$scope.currentTab = value;
		};


		/**
		 * @function Focus na Aba conteúdo
		 * @name setTabFocus
		 * @namespace TestController
		 * @memberOf Controller
		 * @param {boolean} value
		 * @return
		 */
		$scope.setTabFocus = function __setTabFocus(value) {
			if (!value) 
				$scope.currentTab = 'conteudoTab';
		};


		/**
		 * @function Utilizar logotipo no cabeçalho
		 * @name showLogoHeader
		 * @namespace TestController
		 * @memberOf Controller
		 * @param {boolean} value
		 * @return
		 */
		$scope.showLogoHeader = function __showLogoHeader(value) {
			if (value) {
				if ($scope.modelTest.MessageHeaderPosition !== 'Esquerda')
					$scope.modelTest.LogoHeaderPosition = 'Esquerda';
				else if ($scope.modelTest.MessageHeaderPosition !== 'Centro')
					$scope.modelTest.LogoHeaderPosition = 'Centro';
				else
					$scope.modelTest.LogoHeaderPosition = 'Direita';
			}
			else {
				$scope.modelTest.LogoHeaderPosition = undefined;
			}
		};


		/**
		 * @function Utilizar logotipo no rodapé
		 * @name showLogoFooter
		 * @namespace TestController
		 * @memberOf Controller
		 * @param {boolean} value
		 * @return
		 */
		$scope.showLogoFooter = function __showLogoFooter(value) {
			if (value) {
				if ($scope.modelTest.MessageFooterPosition !== 'Esquerda')
					$scope.modelTest.LogoFooterPosition = 'Esquerda';
				else if ($scope.modelTest.MessageFooterPosition !== 'Centro')
					$scope.modelTest.LogoFooterPosition = 'Centro';
				else
					$scope.modelTest.LogoFooterPosition = 'Direita';
			}
			else {
				$scope.modelTest.LogoFooterPosition = undefined;
			}
		};


		/**
		 * @function Utilizar mensagem no cabeçalho
		 * @name showMessageHeader
		 * @namespace TestController
		 * @memberOf Controller
		 * @param {boolean} value
		 * @return
		 */
		$scope.showMessageHeader = function __showMessageHeader(value) {
			if (value) {
				if ($scope.modelTest.LogoHeaderPosition !== 'Esquerda')
					$scope.modelTest.MessageHeaderPosition = 'Esquerda';
				else if ($scope.modelTest.LogoHeaderPosition !== 'Centro')
					$scope.modelTest.MessageHeaderPosition = 'Centro';
				else
					$scope.modelTest.MessageHeaderPosition = 'Direita';
			}
			else {
				$scope.modelTest.MessageHeaderPosition = undefined;
			}
		};


		/**
		 * @function Utilizar mensagem no rodapé
		 * @name showMessageFooter
		 * @namespace TestController
		 * @memberOf Controller
		 * @param {boolean} value
		 * @return
		 */
		$scope.showMessageFooter = function __showMessageFooter(value) {
			if (value) {
				if ($scope.modelTest.LogoFooterPosition !== 'Esquerda')
					$scope.modelTest.MessageFooterPosition = 'Esquerda';
				else if ($scope.modelTest.LogoFooterPosition !== 'Centro')
					$scope.modelTest.MessageFooterPosition = 'Centro';
				else
					$scope.modelTest.MessageFooterPosition = 'Direita';
			}
			else {
				$scope.modelTest.MessageFooterPosition = undefined;
			}
		};


		/**
		 * @function Set header position logo
		 * @name setHeaderLogoPosition
		 * @namespace TestController
		 * @memberOf Controller
		 * @param {string} value
		 * @return
		 */
		$scope.setLogoHeaderPosition = function __setLogoHeaderPosition(value) {
			if (value === $scope.modelTest.MessageHeaderPosition) {
				$notification.alert("A posição selecionada para o logotipo está ocupada pela mensagem do cabeçalho.");
				return;
			}
			$scope.modelTest.LogoHeaderPosition = value;
		};


		/**
		 * @function Set footer position logo
		 * @name setLogoFooterPosition
		 * @namespace TestController
		 * @memberOf Controller
		 * @param {string} value
		 * @return
		 */
		$scope.setLogoFooterPosition = function __setLogoFooterPosition(value) {
			if (value === $scope.modelTest.MessageFooterPosition) {
				$notification.alert("A posição selecionada para o logotipo está ocupada pela mensagem do rodapé.");
				return;
			}
			$scope.modelTest.LogoFooterPosition = value;
		};


		/**
		 * @function Set header logo size
		 * @name setLogoHeaderSize
		 * @namespace TestController
		 * @memberOf Controller
		 * @param {string} value
		 * @return
		 */
		$scope.setLogoHeaderSize = function __setLogoHeaderSize(value) {
			$scope.modelTest.LogoHeaderSize = value;
		};


		/**
		 * @function Set footer logo size
		 * @name setLogoFooterSize
		 * @namespace TestController
		 * @memberOf Controller
		 * @param {string} value
		 * @return
		 */
		$scope.setLogoFooterSize = function __setLogoFooterSize(value) {
			$scope.modelTest.LogoFooterSize = value;
		};


		/**
		 * @function Set header message position
		 * @name setMessageHeaderPosition
		 * @namespace TestController
		 * @memberOf Controller
		 * @param {string} value
		 * @return
		 */
		$scope.setMessageHeaderPosition = function __setMessageHeaderPosition(value) {
			if (value === $scope.modelTest.LogoHeaderPosition) {
				$notification.alert("A posição selecionada para mensagem está ocupada pelo logotipo do cabeçalho.");
				return;
			}
			$scope.modelTest.MessageHeaderPosition = value;
		};


		/**
		 * @function Set footer message position
		 * @name setMessageFooterPosition
		 * @namespace TestController
		 * @memberOf Controller
		 * @param {string} value
		 * @return
		 */
		$scope.setMessageFooterPosition = function __setMessageFooterPosition(value) {
			if (value === $scope.modelTest.LogoFooterPosition) {
				$notification.alert("A posição selecionada para mensagem está ocupada pelo logotipo do rodapé.");
				return;
			}
			$scope.modelTest.MessageFooterPosition = value;
		};


		/**
		 * @function Get css header image
		 * @name getCssHeaderImage
		 * @namespace TestController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		$scope.getCssHeaderImage = function __getCssHeaderImage(value) {

			var _class = {
				'watermark': false,
				'logosmall': false,
				'logomedium': false,
				'logobig': false
			};

			if ($scope.modelTest.LogoHeaderWaterMark)
				_class.watermark = true;

			if ($scope.modelTest.LogoHeaderSize === 'Pequeno' && $scope.modelTest.LogoHeader.Path !== undefined)
				_class.logosmall = true;
			else if ($scope.modelTest.LogoHeaderSize === 'Médio (padrão)' && $scope.modelTest.LogoHeader.Path !== undefined)
				_class.logomedium = true;
			else if ($scope.modelTest.LogoHeader.Path !== undefined)
				_class.logobig = true;

			return _class;
		};


		/**
		 * @function Get css footer image
		 * @name getCssFooterImage
		 * @namespace TestController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		$scope.getCssFooterImage = function __getCssFooterImage(value) {

			var _class = {
				'watermark': false,
				'logosmall': false,
				'logomedium': false,
				'logobig': false
			};

			if ($scope.modelTest.LogoFooterWaterMark)
				_class.watermark = true;

			if ($scope.modelTest.LogoFooterSize === 'Pequeno' && $scope.modelTest.LogoFooter.Path !== undefined)
				_class.logosmall = true;
			else if ($scope.modelTest.LogoFooterSize === 'Médio (padrão)' && $scope.modelTest.LogoFooter.Path !== undefined)
				_class.logomedium = true;
			else if ($scope.modelTest.LogoFooter.Path !== undefined)
				_class.logobig = true;

			return _class;
		};


		/**
		 * @function Redirecionar para tela Search
		 * @name redirectToSearch
		 * @namespace TestController
		 * @memberOf Controller
		 */
		function redirectToSearch() {
			$window.location.href = $util.getWindowLocation("ModelTest");
		};


		/**
		 * @function Atualizar o ciclo de compilação do angular de modo seguro
		 * @name safeApply
		 * @namespace TestController
		 * @memberOf Controller
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
		 * @function Set html string um html em trustesd para ng-bind-html 
		 * @name setTrustedHTML
		 * @namespace TestController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		$scope.setTrustedHTML = function __setTrustedHTML(_html) {
			return $sce.trustAsHtml(_html);
		};


		/**
		 * @function Converter \n para </br>
		 * @name converterBreakLineToBr
		 * @namespace TestController
		 * @memberOf Controller
		 * @param {string} s
		 * @return
		 */
		$scope.converterBreakLineToBr = function __converterBreakLineToBr(s) {

			if ( (s === undefined && s === null) || typeof s !== 'string')
				return "";

			var str = s.replace(/(?:\r\n|\r|\n)/g, '<br />');

			return str;
		};


		/**
		 * @function Converter </br> para \n 
		 * @name converterBrToBreakLine
		 * @namespace TestController
		 * @memberOf Controller
		 * @param {string} s
		 * @return
		 */
		$scope.converterBrToBreakLine = function __converterBrToBreakLine(s) {

			if ((s === undefined && s === null) || typeof s !== 'string')
				return "";

			var str = s.replace(/<br\s*\/?>/mg, "\n");

			return str;
		};
	};

})(angular, jQuery);