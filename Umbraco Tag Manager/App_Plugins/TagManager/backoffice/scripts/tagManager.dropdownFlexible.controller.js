﻿angular.module("umbraco").controller("tagManager.dropdownFlexible.controller",
    function ($scope, validationMessageService, tagManagerResource, $routeParams) {

        tagManagerResource.getTagsById($routeParams.id).then(function (response) {

            var tagsByVariant = objectWithLanguageId(response.data.tagsInGroup, response.data.tagItem.languageId);
            $scope.model.mergeList = removeObjectWithId(tagsByVariant, $routeParams.id);

        }, function (err) {
            //check if response is ysod
            if (err.status && err.status >= 500) {
                dialogService.ysodDialog(err);
            }
            if (err.data && angular.isArray(err.data.notifications)) {
                for (var i = 0; i < err.data.notifications.length; i++) {
                    notificationsService.showNotification(err.data.notifications[i]);
                }
            }
        });

        $scope.change = function (tag) {
            $scope.model.mergeTag = tag;
        }

        function objectWithLanguageId(arr, id) {
            return arr.filter((obj) => obj.languageId === id);
        }

        function removeObjectWithId(arr, id) {
            return arr.filter((obj) => obj.id !== parseInt(id));
        }
    });