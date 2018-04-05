describe('FileController.test.js', function () {

    beforeEach(module('services', 'filters', 'directives', 'tooltip', 'appMain'));

    var $controller, $notification, $rootScope, FileModel, TestListModel;

    beforeEach(inject(function (_$rootScope_, _$controller_, _$notification_, _FileModel_, _TestListModel_) {
        $rootScope = _$rootScope_;
        $controller = _$controller_;
        $notification = _$notification_;
        FileModel = _FileModel_;
        TestListModel = _TestListModel_;
    }));

    describe('functions exist', function () {
        it('deve existir load()', function () {
            var $scope = $rootScope.$new();
            var controller = $controller('FileController', { $scope: $scope });
            expect($scope.load).toBeDefined();
        });
    });

});