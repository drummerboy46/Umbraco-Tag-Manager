(function () {
    'use strict';

    function deleteTags($scope, $location,
        tagManagerResource, notificationsService, navigationService, $route, treeService, appState) {

        var vm = this;

        //get the treeNode that we have selected
        var selectedMenuNode = appState.getMenuState("currentNode");
        var parent = selectedMenuNode.parent();

        tagManagerResource.getTagsById(selectedMenuNode.id).then(function (response) {
            $scope.cmsTags = response.data;
            $scope.selectedTag = selectedMenuNode.id;
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

        $scope.cancel = function () {
            navigationService.hideMenu();
        }

        $scope.delete = function (cmsTags) {
            if (cmsTags.tagsInGroup.length === 1) {
                tagManagerResource.deleteTag(cmsTags).then(function (response) {
                    if (response.data === 0) {
                        notificationsService.error("Error", "There was a problem deleting the tag, please check the logs.");
                        return;
                    }
                    //remove the node
                    treeService.removeNode(selectedMenuNode);

                    navigationService.syncTree({ tree: "TagManagerTree", path: ["-1"], forceReload: true }).then(function (syncArgs) {
                        navigationService.reloadNode(syncArgs.node);
                    });;

                    // if the current edited item is the same one as we're deleting, we need to navigate elsewhere
                    $location.path('/TagManager/TagManagerTree/');
                    $route.reload();

                    notificationsService.success("Success", "'" + cmsTags.tagItem.tag + "' has been deleted. All tags have been deleted, re-create the group if you need it again");
                    navigationService.hideMenu();
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
            }
            else {
                tagManagerResource.deleteTag(cmsTags).then(function (response) {
                    if (response.data === 0) {
                        notificationsService.error("Error", "There was a problem deleting the tag, please check the logs.");
                        return;
                    }

                    //remove the node
                    treeService.removeNode(selectedMenuNode);

                    navigationService.syncTree({ tree: "TagManagerTree", path: ["-1", "tagGroup-" + parent.name], forceReload: true });

                    // if the current edited item is the same one as we're deleting, we need to navigate elsewhere
                    $location.path('/TagManager/TagManagerTree/group/' + parent.name);
                    $route.reload();

                    notificationsService.success("Success", "'" + cmsTags.tagItem.tag + "' has been deleted.");
                    navigationService.hideMenu();
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
            }
        }
    }

    angular.module('umbraco').controller('tagManager.delete.controller', deleteTags);
})();
