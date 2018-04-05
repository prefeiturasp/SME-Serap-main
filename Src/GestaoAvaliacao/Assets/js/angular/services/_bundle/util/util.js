/*
 * Autor: Alex Figueiredo
 * Name: $util
 *
 * Serviço que contém helpers de aplicação
 *
 *
*/
(function (angular) {

	'use strict'

	angular.module('services')
		.factory('$util', $util)
		.config(Config);


	$util.$inject = ['$window'];
	Config.$inject = [];


	/**
	 * @function Configuração global para JQuery.
	 * @name Config
	 * @namespace Services
	 * @memberOf Factory
	 */
	function Config() {

		/**
		 * @function Remover comentários html via JQuery.
		 * @name uncomment
		 * @namespace Config
		 * @memberOf Factory
		 */
		(function ($) {
			$.fn.uncomment = function (recurse) {
				$(this).contents().each(function () {
					if (recurse && this.hasChildNodes()) {
						$(this).uncomment(recurse);
					} else if (this.nodeType == 8) {
						var e = $('');
						$(this).replaceWith(e.contents());
					}
				});
			};
		})(jQuery);
	};


	/**
	 * @function Utilidades globais
	 * @name Config
	 * @namespace Services
	 * @memberOf Factory
	 */
	function $util($window) {

		var app = {};

		// Detecta Internet Explorer 9 ou inferior
		app.internet_explorer = (function () {

			var undef,
                v = 3,
                div = document.createElement('div'),
                all = div.getElementsByTagName('i');

			while (
                div.innerHTML = '<!--[if gt IE ' + (++v) + ']><i></i><![endif]-->',
                all[0]
            );

			return v > 4 ? v : undef;

		}());


		/**
		 * @name compressImage
		 * @desc faz a compressao de uma imagem e reduz o seu tamanho para o tamanho maximo passado como parametro
		 * @returns {Image Object compressed}
		 * @memberOf Factories.Util
		 */
		app.compressImage = function __compressImage(source_img_obj, _maxWidth, _maxHeight, quality) {
			var mime_type = "image/jpeg";
			var maxWidth = _maxWidth;
			var maxHeight = _maxHeight;
			var width = source_img_obj.width;
			var height = source_img_obj.height;

			//calcula a proporcao da largura e altura para resize
			if (width > height) {
				if (width > maxWidth) {
					height = Math.round(height *= maxWidth / width);
					width = maxWidth;
				}
			}
			else {
				if (height > maxHeight) {
					width = Math.round(width *= maxHeight / height);
					height = maxHeight;
				}
			}

			var cvs = document.createElement('canvas');
			cvs.width = width;
			cvs.height = height;
			var ctx = cvs.getContext("2d").drawImage(source_img_obj, 0, 0, width, height);
			var newImageData = cvs.toDataURL(mime_type, quality / 100);
			var result_image_obj = new Image();
			result_image_obj.src = newImageData;
			result_image_obj.width = width;
			result_image_obj.height = height;

			return result_image_obj;
		};


		// Guid
		app.Guid = {
			Empty: "00000000-0000-0000-0000-000000000000",
			New: function () {
				return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
					var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
					return v.toString(16);
				});
			}
		};


		/**
		 * @name Guid
		 * @function Guid for id
		 * @param 
		 * @returns {int}
		 * @memberOf Factories.Util
		 */
		app.getGuid = function __Guid() {
			var d = new Date().getTime();
			var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
				var r = (d + Math.random() * 16) % 16 | 0;
				d = Math.floor(d / 16);
				return (c == 'x' ? r : (r & 0x7 | 0x8)).toString(16);
			});
			return uuid;
		};


		// Parâmetro na url
		app.urlParam = function __urlParam(name, url) {
			var results = new RegExp('[\\?&]' + name + '=([^&#]*)').exec(url || window.location.href);
			if (results == null) {
				return null;
			} else {
				return results[1] || 0;
			}
		};


		/**
		 * @function Obtem-se os params de uma url
		 * @name getUrlParams
		 * @returns {Object}
		 * @memberOf Factories.Util
		 */
		app.getUrlParams = function __getUrlParams() {
			// This function is anonymous, is executed immediately and 
			// the return value is assigned to QueryString!
			var query_string = {};
			var query = $window.location.search.substring(1);
			var vars = query.split("&");
			for (var i = 0; i < vars.length; i++) {
				var pair = vars[i].split("=");
				// If first entry with this name
				if (typeof query_string[pair[0]] === "undefined") {
					query_string[pair[0]] = decodeURI(pair[1]);
					// If second entry with this name
				} else if (typeof query_string[pair[0]] === "string") {
					var arr = [query_string[pair[0]], pair[1]];
					query_string[pair[0]] = arr;
					// If third or later entry with this name
				} else {
					query_string[pair[0]].push(pair[1]);
				}
			}
			return query_string;
		};


		/**
		 * @function retorna window.origin com path concatenado
		 * @name windowLocation
		 * @param {String} String com path
		 * @returns {String} url final
		 * @memberOf Factories.Util
		 */
		app.getWindowLocation = function __windowLocation(path) {
			var location = window.location;
			var origin = location.origin ? location.origin + "/" + path : location.protocol + "//" + location.host + "/" + path;
			return origin;
		};

	    /**
         * @function Validação para data final maior que inicial
         * @param {string} StartDate
         * @param {string} EndDate
         * @returns {boolean}
         */
		app.greaterEndDateThanStartDate = function __greaterThanStartDate(StartDate, EndDate) {
		    if (StartDate && EndDate) {
		        if (new Date(EndDate) >= new Date(StartDate)) {
		            return true;
		        } else {
		            return false;
		        }
		    }
		    return true;
		};

		return app;
	};

})(angular);