angular.module("umbraco").controller("TagListController", function ($scope, $routeParams, TagListResource, contentResource) {
    var vm = this;
    vm.cmsTags = {};

    /* get a complete list of tags for the selected "group" */
    TagListResource.getTagsByGroup($scope.model.config.group).then(function (response) {
        vm.cmsTags = response.data;
    });
});