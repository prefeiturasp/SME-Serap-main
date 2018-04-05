'use strict';

angular.module('services')

    .factory('$notification', Notification);

Notification.$inject = ['$window', '$injector'];

function Notification($window, $injector) {

    var notifications = [],
    loadingMessage = null,
    defaults = {
        autoClose: true,
        type: '',
        closeable: true,
        title: '',
        message: '',
        delay: 4000 //4 segundos    
    };

    var service = {
        get: get,
        add: add,
        remove: remove,
        clear: clear,
        loading: loading,
        clearLoading: clearLoading,
        alert: alert,
        info: info,
        success: success,
        error: error,
        generateID: generateID
    };


    function get() {
        return notifications;
    };

    function add(notification) {

        notifications.push(notification);
    };

    function remove(id) {

        for (var i = (notifications.length - 1) ; i >= 0 ; i--) {
            if (notifications[i].id == id) {
                notifications.splice(i, 1);
                break;
            }
        }
    };

    function clear() {
        notifications = [];
    };

    function loading(text) {
        if (text)
            loadingMessage = text;

        return loadingMessage;
    };

    function clearLoading() {
        loadingMessage = null;
    };

    function alert(title, message, scrollable, autoClose, closeable, delay, scrolltime) {

        var custom = {
            id: this.generateID(),
            title: title,
            message: message,
            type: 'warning',
            autoClose: (autoClose == null || autoClose == undefined) ? true : autoClose,
            closeable: (closeable == null || closeable == undefined) ? true : closeable,
            delay: (delay == null || delay == undefined) ? 4000 : delay
        }

        this.add(custom);
    };

    function info(title, message, scrollable, autoClose, closeable, delay, scrolltime) {

        var custom = {
            id: this.generateID(),
            title: title,
            message: message,
            type: 'info',
            autoClose: (autoClose == null || autoClose == undefined) ? true : autoClose,
            closeable: (closeable == null || closeable == undefined) ? true : closeable,
            delay: (delay == null || delay == undefined) ? 4000 : delay
        }

        this.add(custom);
    }

    function success(title, message, scrollable, autoClose, closeable, delay, scrolltime) {

        var custom = {
            id: this.generateID(),
            title: title,
            message: message,
            type: 'success',
            autoClose: (autoClose == null || autoClose == undefined) ? true : autoClose,
            closeable: (closeable == null || closeable == undefined) ? true : closeable,
            delay: (delay == null || delay == undefined) ? 4000 : delay
        }

        this.add(custom);
    };

    function error(title, message, scrollable, autoClose, closeable, delay, scrolltime) {

        var custom = {
            id: this.generateID(),
            title: title,
            message: message,
            type: 'error',
            autoClose: (autoClose == null || autoClose == undefined) ? true : autoClose,
            closeable: (closeable == null || closeable == undefined) ? true : closeable,
            delay: (delay == null || delay == undefined) ? 4000 : delay
        }

        this.add(custom);
    };

    function generateID() {
        var d = new Date().getTime();
        var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (d + Math.random() * 16) % 16 | 0;
            d = Math.floor(d / 16);
            return (c == 'x' ? r : (r & 0x7 | 0x8)).toString(16);
        });
        return uuid;
    };

    // Retorna o serviço
    return service;

};