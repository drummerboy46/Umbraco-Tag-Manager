(function () {
    'use strict';

    function checkExistingTagName(tag, tagsFromGroup) {
        var tagMatch = false;
        Object.entries(tagsFromGroup).forEach(([key, value]) => {
            if (tag === value.tag) {
                tagMatch = true;
            }
        });
        return tagMatch;
    }



    function createTag($scope, tagManagerResource, notificationsService, navigationService, appState, $location, $route, languageResource) {

        $scope.cmsTags = {
            tag: "",
            languages: {},
            languageId: null,
            group: ""
        };

        $scope.change = function (lang) {
            $scope.cmsTags.languageId = lang.id;
        }

        languageResource.getAll()
            .then(function (data) {
                $scope.cmsTags.languages = data;
            });

        // gets node from menu state first and if null gets from tree state
        var selectedMenu = appState.getMenuState("currentNode");

        if (selectedMenu === null) {
            var selectedNode = appState.getTreeState("selectedNode");
            $scope.cmsTags.group = selectedNode.name;
        }
        else {
            $scope.cmsTags.group = selectedMenu.name;
        }

        $scope.save = function (cmsTags) {
            if (!cmsTags.tag) {
                notificationsService.error("Error", "Tag name is required.");
                return;
            }

            // Fetch tags for a specific group from the server
            tagManagerResource.getTagsByGroup(cmsTags.group).then(function (response) {
                var existingTagName = checkExistingTagName(cmsTags.tag, response.data.tagsInGroup);

                if (existingTagName === false) {
                    tagManagerResource.createTag(cmsTags).then(function (response) {
                        if (response.data === 0) {
                            notificationsService.error("Error", "There was a problem saving the tag, please check the logs.");
                            return;
                        }
                        notificationsService.success("Success", "'" + cmsTags.tag + "' has been Created");
                        navigationService.syncTree({ tree: "TagManagerTree", path: ["-1", "tagGroup-" + cmsTags.group, response.data], forceReload: false });
                        navigationService.hideMenu();
                        $location.path('/TagManager/TagManagerTree/edit/' + response.data);
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
                    notificationsService.error("Error", "A tag with name already exists");
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

        $scope.cancel = function (cmsTags) {
            if ($location.path() === "/TagManager/TagManagerTree/create") {
                $location.path('/TagManager/TagManagerTree/group/' + cmsTags.group);
                $route.reload();
            }
            else {
                navigationService.hideMenu();
            }
        }
    }

    angular.module('umbraco').controller('tagManager.create.controller', createTag);
})();
