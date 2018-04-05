/**
 * @function Paginação
 * @namespace Controller
 * @author Julio Cesar Silva - 03/03/2016
 */
(function (angular, $) {

    'use strict';

    angular
        .module('directives')
        .directive('page', page)
        .constant('paginationConfig', {
            itemsPerPage: 10,
            arrPageSize: [5, 10, 20, 30, 40, 50, 100]
        });

    page.$inject = ['paginationConfig'];

    function page(paginationConfig) {
    
        return {
            restrict: 'EA',
            transclude: true,
            scope: {
                method: '&',
                totalPages: '@',
                totalItens: '@',
                pageSize: '=',
                arrPageSize: '=',
                pager: "="
            },
            templateUrl: '/Assets/js/angular/directives/_bundle/page/page.html',
            replace: true,
            link: function ($scope, elem, attrs) {

                $scope.pages = [];
                $scope.page_offset = 10;
                $scope.page_size = $scope.pageSize || paginationConfig.itemsPerPage;
                $scope.arrPageSize = $scope.arrPageSize || paginationConfig.arrPageSize;
                $scope.atualPg = 0;
                $scope.current_page = 0;
                $scope.current_page_offset = 1;
                $scope.total_pages = attrs.totalPage || 0;
                $scope.total_itens = 0;

                $scope.$watchCollection('[totalPages ,totalItens]', function (n,o) {
                    $scope.total_pages = parseInt(n[0]);
                    $scope.total_itens = parseInt(n[1]);
                    $scope.create_pages();
                });

                $scope.paginacao = function __paginacao(index) {
                    activeClass(index);
                    $scope.atualPg = index;                    

                    var varkey = attrs.varkey || 'paginate';

                    if ($scope.$parent[varkey] && !$scope.pager)
                        $scope.$parent[varkey].indexPage(index - 1);
                    else if ($scope.pager)
                        $scope.pager.indexPage(index - 1);

                    $scope.method();
                }

                $scope.currentPage = function __currentPage() {
                    return $scope.atualPg;
                };

                $scope.change_total_page = function __change_total_page() {

                    var varkey = attrs.varkey || 'paginate';

                    if ($scope.$parent[varkey] && !$scope.pager)
                        $scope.$parent[varkey].pageSize($scope.page_size);
                    else if ($scope.pager)
                        $scope.pager.pageSize($scope.page_size);

                    $scope.total_pages = Math.ceil($scope.total_itens / $scope.page_size);
                    $scope.create_pages();
                    $scope.paginacao(1);
                };

                $scope.create_pages = function __create_pages() {
                    var paginas = [];
                    for (var i = 0; i < $scope.total_pages; i++) {

                        paginas.push({
                            number: i + 1,
                            active: i === $scope.current_page
                        });
                    }
                    $scope.pages = paginas;
                    $scope.current_page_offset = 1;
                };

                $scope.page_start = function __page_start() {
                    return $scope.page_offset * ($scope.current_page_offset - 1);
                };

                $scope.page_end = function __page_end() {
                    return $scope.page_offset * $scope.current_page_offset;
                };

                $scope.next_offset = function __next_offset() {
                    $scope.current_page_offset++;
                    $scope.change_page($scope.page_start() + 1);
                };

                $scope.prev_offset = function __prev_offset() {
                    $scope.current_page_offset--;
                    $scope.change_page($scope.page_end());
                };

                $scope.change_page = function __change_page(page) {
                    $scope.paginacao(page);
                };

                function activeClass(index) {
                    $scope.pages.forEach(function (value) {
                        if (value.active)
                            value.active = false;
                    });
                    $scope.pages[index - 1].active = true;
                };
            }
        };
    };

})(angular, jQuery);