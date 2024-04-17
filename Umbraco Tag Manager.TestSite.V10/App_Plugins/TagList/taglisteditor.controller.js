angular.module("umbraco").controller("TagListController", function ($scope, $routeParams, TagListResource) {
    var vm = this;
    vm.cmsTags = {};

    /* get a complete list of tags for the selected "group" */
    TagListResource.getTagsByGroup($scope.model.config.group, $scope.model.culture).then(function (response) {
        vm.cmsTags = response.data;
    });
});