angular.module('filters').filter('between', function () {
    return function (input, start, end) {
        if (input)
            return input.slice(+start, +end);
        else
            return [];
    };
});