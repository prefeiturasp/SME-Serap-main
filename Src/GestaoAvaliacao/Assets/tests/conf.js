/**
 * @function Modulo de configuração do protractor para testes end-to-end (e2e)
 * @author Julio Cesar da Silva
 * @since Generated on Fri May 13 2016 10:45:00 GMT-0300 (Hora oficial do Brasil)
 */
exports.config = {
    //baseUrl: 'http://localhost:8000/',
    framework: 'jasmine',
    seleniumAddress: 'http://localhost:4444/wd/hub',
    specs: [
        'e2e/controllers/OMRController.e2e.js'
    ],
    multiCapabilities: [
        //{browserName: 'firefox'},
        { browserName: 'chrome' }
    ],
    jasmineNodeOpts: {
        showColors: true,
        defaultTimeoutInterval: 30000,
        isVerbose: true,
        includeStackTrace: true
    }
}
