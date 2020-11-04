(function ($root, $) {
	"use strict";

	$root.RequestManager = $root.RequestManager || RequestManager;

	function RequestManager(amountOfRequests, finallyCallback) {
		var requestsPending = [];
		var amountOfRequestProcessing = 0;
		var hadFinalized = false;

		if (!this) {
			return new RequestManager(amountOfRequests);
		}

		this.addRequest = function (request) {
			request = request || {};
			request.complete = callbackFn(request.complete);

			if (amountOfRequestProcessing < amountOfRequests) {
				processRequest(request);
			}
			else {
				requestsPending.push(request);
			}
		}

		function processRequest(request) {
			request = request || {};
			amountOfRequestProcessing++;
			return $.ajax(request);
		}

		function callbackFn(fn) {
			return function (data) {
				if (fn || fn instanceof Function) {
					fn(data);
				}
				next();
			}
		}

		function next() {
			amountOfRequestProcessing--;
			if (requestsPending.length > 0) {
				var request = requestsPending.shift();
				processRequest(request);
			}
			else {
				finalize();
            }
		}

		function finalize() {
			if (hadFinalized) return;
			hadFinalized = true;
			finallyCallback();
		}
	}

})(window, jQuery);