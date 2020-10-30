(function ($root, $) {
	"use strict";

	$root.SessionManager = $root.SessionManager || SessionManager;

	function SessionManager(aluId, turId, testId, reportStartSessionError, reportEndSessionError) {

		$.connection.hub.logging = true;
		var sessionHub = $.connection.studentTestSessionHub;
		var hubConnection = null;
		var sessionStarted = false;

		sessionHub.client.reportStartSessionSuccess = function () {
			console.log("[STUDENT TEST SESSION] Open");
		};

		sessionHub.client.reportStartSessionError = function (messages) {
			reportStartSessionError(messages);
		};

		sessionHub.client.reportEndSessionSuccess = function () {
			console.log("[STUDENT TEST SESSION] End");
		};

		sessionHub.client.reportEndSessionError = function (messages) {
			reportEndSessionError(messages);
		};

		this.startSession = function () {
			if (hubConnection == null) connect();

			hubConnection.done(function () {
				sessionHub.server.startSession(aluId, turId, testId)
					.done(function () {
						sessionStarted = true;
					})
					.fail(function (error) {
						onError(reportStartSessionError, error);
					});
			});
		};

		this.endSession = function (callback) {
			if (!sessionStarted) return;
			if (hubConnection == null) connect();

			hubConnection.done(function () {
				sessionHub.server.endSession()
					.done(function () {
						callback();
					})
					.fail(function (error) {
						onError(reportEndSessionError, error);
						callback();
					});
			});
		};

		this.endTest = function (callback) {
			if (hubConnection == null) connect();
			hubConnection.done(function () {
				sessionHub.server.endTest(aluId, turId, testId)
					.done(function () {
						callback();
					})
					.fail(function (error) {
						onError(reportEndSessionError, error);
						callback();
					});
			});
		};

		function connect() {
			hubConnection = $.connection.hub.start();
		};

		function onError(callbackError, error) {
			if (!callbackError) return;
			callbackError(error);
		};
	}

})(window, jQuery);