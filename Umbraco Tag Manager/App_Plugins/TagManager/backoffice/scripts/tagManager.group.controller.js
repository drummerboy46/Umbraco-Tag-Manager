(function () {
    'use strict';

    function areAllTagsMarkedForDelete(cmsTags) {
        var allTags = false;
        var tagsCountForDelete = 0;
        Object.entries(cmsTags).forEach(([key, value]) => {
            if (value.tagSelected) {
                tagsCountForDelete++;
            }
        });
        if (cmsTags.length === tagsCountForDelete) {
            allTags = true;
        }
        return allTags;
    }

    function manageGroup($scope, tagManagerResource, $location, $route, notificationsService, appState, overlayService, navigationService) {
        var vm = this;

        vm.filter = {
            searchTerm: ""
        };

        // Initialize an array to hold selected tags
        $scope.selectedTags = [];

        var group = $location.path().substring($location.path().lastIndexOf('/') + 1);
        $scope.title = group;

        // Fetch tags for a specific group from the server
        tagManagerResource.getTagsByGroup(group).then(function (response) {
            $scope.cmsTags = response.data;
            navigationService.syncTree({ tree: 'TagManagerTree', path: ["-1", "tagGroup-" + $scope.title], forceReload: true });

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

        // Function to toggle the selection of a tag
        $scope.toggleSelectTag = function (tag) {
            tag.tagSelected = !tag.tagSelected;
            if (tag.tagSelected) {
                $scope.selectedTags.push(tag);
            } else {
                var indexObj = $scope.selectedTags.indexOf(tag);
                if (indexObj !== -1) {
                    $scope.selectedTags.splice(indexObj, 1);
                }
            }
        };

        // Function to clear the selection of all tags
        $scope.clearSelection = function () {
            // Iterate through all tags and set selected to false
            $scope.cmsTags.tagsInGroup.forEach(function (tag) {
                tag.tagSelected = false;
            });

            // Clear the selectedTags array
            $scope.selectedTags = [];
        };

        // Function to filter tags based on the search query
        $scope.filterTags = function (tag) {
            var result = !$scope.searchQuery || tag.name.toLowerCase().includes($scope.searchQuery.toLowerCase());
            return result;
        };

        // Function to navigate to the create tag view
        $scope.navigateToCreateTag = function () {
            $location.path('/TagManager/TagManagerTree/create');
        };

        // Function to navigate to the edit tag view
        $scope.navigateToEditTag = function (tagId) {
            $location.path('/TagManager/TagManagerTree/edit/' + tagId);
        };

        $scope.openOverlay = function (cmsTags) {
            $scope.cmsTags = cmsTags;
            var options = {
                title: 'Confirm',
                content: 'Are you sure you want to delete the selected tag(s)?',
                disableBackdropClick: true,
                disableEscKey: true,
                confirmType: 'delete',
                submit: function () {

                    if (cmsTags.length === 0) {
                        return;
                    }
                    if ($scope.busy) {
                        return false;
                    }
                    $scope.busy = true;

                    var allTagsForDelete = areAllTagsMarkedForDelete(cmsTags.tagsInGroup);
                    if (allTagsForDelete) {
                        tagManagerResource.deleteTags(cmsTags).then(function (response) {
                            if (response.data === 0) {
                                notificationsService.error("Error", "There was a problem deleting the tags, please check the logs.");
                                return;
                            }
                            navigationService.syncTree({ tree: "TagManagerTree", path: ["-1"], forceReload: true }).then(function (syncArgs) {
                                navigationService.reloadNode(syncArgs.node);
                            });

                            $location.path('/TagManager/');
                            $route.reload();

                            notificationsService.success("Success", "All tags have been deleted, re-create the group if you need it again");

                            $scope.busy = false;
                        }, function (err) {
                            $scope.busy = false;
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
                        tagManagerResource.deleteTags(cmsTags).then(function (response) {
                            if (response.data === 0) {
                                notificationsService.error("Error", "There was a problem deleting the tags, please check the logs.");
                                return;
                            }
                            var node = appState.getTreeState("selectedNode");

                            navigationService.syncTree({ tree: "TagManagerTree", path: ["-1", "tagGroup-" + node.name], forceReload: true }).then(function (syncArgs) {
                                navigationService.reloadNode(syncArgs.node);
                            });;
                            $route.reload();
                            notificationsService.success("Success", "tags have been deleted.");

                            $scope.busy = false;
                        }, function (err) {
                            $scope.busy = false;
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


                    overlayService.close();
                }
            };

            overlayService.confirm(options);
        }
    }
    angular.module('umbraco').controller('tagManager.group.controller', manageGroup);
})();