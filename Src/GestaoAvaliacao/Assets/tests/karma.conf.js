/**
 * @function Modulo de configuração do karma para monitoramento de testes unitários
 * @author Julio Cesar da Silva
 * @since Generated on Fri May 13 2016 08:17:56 GMT-0300 (Hora oficial do Brasil)
 */
module.exports = function(config) {
  config.set({

    // base path that will be used to resolve all patterns (eg. files, exclude)
    basePath: '',


    // frameworks to use
    // available frameworks: https://npmjs.org/browse/keyword/karma-adapter
    frameworks: ['jasmine'],


    // list of files / patterns to load in the browser
    files: [
        // :arquivos base
        '../js/vendor/jquery-2.1.1.js',
        '../js/vendor/angular-1.4.9/angular.js',
        '../js/vendor/angular-1.4.9/angular-mocks.js',
        '../js/vendor/angular-1.4.9/angular-animate.js',
        '../js/vendor/angular-1.4.9/angular-aria.js',
        '../js/vendor/angular-1.4.9/angular-cookies.js',
        '../js/vendor/angular-1.4.9/angular-loader.js',
        '../js/vendor/angular-1.4.9/angular-message-format.js',
        '../js/vendor/angular-1.4.9/angular-messages.js',
        '../js/vendor/angular-1.4.9/angular-resource.js',
        '../js/vendor/angular-1.4.9/angular-route.js',
        '../js/vendor/angular-1.4.9/angular-sanitize.js',
        '../js/vendor/angular-1.4.9/angular-scenario.js',
        '../js/vendor/angular-1.4.9/angular-touch.js',
        // :fake para razor (@ViewBag) do Asp.Net
        'unit/razor.js',
        // :factories base
        '../js/angular/factories/_bundle/*/*.js',
        // :services base
        '../js/angular/services/services.js',
        '../js/angular/services/services.interceptor.js',
        '../js/angular/services/_bundle/*/*.js',
        '../js/angular/models/*/*.js',
        // :filters base
        '../js/angular/filters/filters.js',
        '../js/angular/filters/_bundle/*/*.js',
        // :directives base
        '../js/angular/directives/directives.js',
        '../js/angular/directives/_bundle/popover/helpers/*.js',
        '../js/angular/directives/_bundle/*/*.js',
        // :arquivos alvos dos testes
        '../js/angular/controllers/file/fileController.js',
        '../js/angular/controllers/item/formItemController.js',
        //arquivos de testes
        //'unit/controllers/fileController.test.js'
        'unit/controllers/formItemController.test.js'
    ],


    // list of files to exclude
    exclude: [
    ],


    // preprocess matching files before serving them to the browser
    // available preprocessors: https://npmjs.org/browse/keyword/karma-preprocessor
    preprocessors: {
    },


    // test results reporter to use
    // possible values: 'dots', 'progress'
    // available reporters: https://npmjs.org/browse/keyword/karma-reporter
    reporters: ['progress', 'mocha', 'kjhtml'],

    //plugins
    plugins: [
        'karma-jasmine-html-reporter',
        'karma-chrome-launcher',
        'karma-jasmine',
        'karma-mocha-reporter'
    ],

    // reporters
    mochaReporter: {
        colors: {
            success: 'blue',
            info: 'bgGreen',
            warning: 'cyan',
            error: 'bgRed'
        }
    },

    // web server port
    port: 9876,


    // enable / disable colors in the output (reporters and logs)
    colors: true,


    // level of logging
    // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
    logLevel: config.LOG_INFO,


    // enable / disable watching file and executing tests whenever any file changes
    autoWatch: true,


    // start these browsers
    // available browser launchers: https://npmjs.org/browse/keyword/karma-launcher
    browsers: ['Chrome'],


    // Continuous Integration mode
    // if true, Karma captures browsers, runs the tests and exits
    singleRun: false,

    // Concurrency level
    // how many browser should be started simultaneous
    concurrency: Infinity
  })
}
