(function () {
    'use strict';

    function checkExistingGroupName(group, groups) {
        var tagMatch = false;
        Object.entries(groups).forEach(([key, value]) => {
            if (group === value.group) {
                tagMatch = true;
            }
        });
        return tagMatch;
    }

    function createGroup($scope, tagManagerResource, notificationsService, navigationService, $location, $route) {

        var vm = this;

        $scope.cmsTags = {
            group: ""
        };

        $scope.save = function (cmsTags) {
            if (!cmsTags.group) {
                notificationsService.error("Error", "Group name is required.");
                return;
            }

            tagManagerResource.getTagGroups(cmsTags.group).then(function (response) {
                var existingGroupName = checkExistingGroupName(cmsTags.group, response.data);

                if (existingGroupName === false) {
                    tagManagerResource.createGroup(cmsTags.group).then(function (response) {
                        if (response.data === 0) {
                            notificationsService.error("Error", "There was a problem saving the group, please check the logs.");
                            return;
                        }
                        notificationsService.success("Success", "'" + cmsTags.group + "' has been created");
                        navigationService.syncTree({ tree: "TagManagerTree", path: ["-1"], forceReload: false }).then(function (syncArgs) {
                            navigationService.reloadNode(syncArgs.node);
                        });
                        navigationService.hideMenu();
                        $location.path('/TagManager/' + response.data);
                        $route.reload();
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
                    notificationsService.error("Error", "A group with this name already exists");
                }


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

        };
        $scope.cancel = function () {
            if ($location.path() === "/TagManager/TagManagerTree/create-group") {
                $location.path('/TagManager/');
                $route.reload();
            }
            else {
                navigationService.hideMenu();
            }
        }
    }

    angular.module('umbraco').controller('tagManager.createGroup.controller', createGroup);
})();