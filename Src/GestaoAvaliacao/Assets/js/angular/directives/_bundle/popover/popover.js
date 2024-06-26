﻿'use strict';

angular.module('popover', ['tooltip'])

  .provider('$popover', function () {

      var defaults = this.defaults = {
          animation: 'am-fade',
          customClass: '',
          container: false,
          target: false,
          placement: 'right',
          template: base_url('Assets/js/angular/directives/_bundle/popover/popover-menu.html'),
          contentTemplate: false,
          trigger: 'click',
          keyboard: true,
          html: false,
          title: '',
          content: '',
          delay: 0
      };

      this.$get = ['$tooltip', function ($tooltip) {

          function PopoverFactory(element, config) {

              // Configuração da url repassada por parametro pela tag - obter base do site
              config.template = base_url(config.template);

              // Common vars
              var options = angular.extend({}, defaults, config);

              var $popover = $tooltip(element, options);

              // Support scope as string options [/*title, */content]
              if (options.content) {
                  $popover.$scope.content = options.content;
              }

              return $popover;

          }

          return PopoverFactory;

      }];

  })

  .directive('bsPopover', ['$window', '$sce', '$popover', function ($window, $sce, $popover) {

      var requestAnimationFrame = $window.requestAnimationFrame || $window.setTimeout;

      return {
          restrict: 'EAC',
          scope: true,
          link: function postLink(scope, element, attr) {

              // Directive options
              var options = { scope: scope };

              // search for attributes on element  
              angular.forEach(['template', 'contentTemplate', 'placement', 'container', 'target', 'delay',
                               'trigger', 'keyboard', 'html', 'animation', 'customClass'], function (key) {
                                   
                                   if (angular.isDefined(attr[key]))
                                       options[key] = attr[key];
                               });

              // Support scope as data-attrs
              angular.forEach(['title', 'content'], function (key) {
                  attr[key] && attr.$observe(key, function (newValue, oldValue) {

                      scope[key] = $sce.trustAsHtml(newValue);

                      angular.isDefined(oldValue) && requestAnimationFrame(function () {
                          popover && popover.$applyPlacement();
                      });
                  });
              });

              // Support scope as an object
              attr.bsPopover && scope.$watch(attr.bsPopover, function (newValue, oldValue) {

                  if (angular.isObject(newValue)) {

                      for (var key in newValue) {
                          if (key === 'title' || key === 'content') {
                              scope[key] = $sce.trustAsHtml(newValue[key]);
                          }
                          else {
                              scope[key] = newValue[key];
                          }
                      }
                  } else {
                      scope.content = newValue;
                  }

                  angular.isDefined(oldValue) && requestAnimationFrame(function () {
                      popover && popover.$applyPlacement();
                  });
              }, true);



              // Visibility binding support
              attr.bsShow && scope.$watch(attr.bsShow, function (newValue, oldValue) {
                  if (!popover || !angular.isDefined(newValue)) return;
                  if (angular.isString(newValue)) newValue = !!newValue.match(/true|,?(popover),?/i);
                  newValue === true ? popover.show() : popover.hide();
              });

              // Initialize popover
              var popover = $popover(element, options);

              // Garbage collection
              scope.$on('$destroy', function () {
                  if (popover) popover.destroy();
                  options = null;
                  popover = null;
              });

          }
      };
  }]);
