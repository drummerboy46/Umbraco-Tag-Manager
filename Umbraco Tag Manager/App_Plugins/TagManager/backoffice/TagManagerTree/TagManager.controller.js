angular.module("umbraco").controller("TagManager.EditController", function ($scope, $routeParams, $location,
    TagManagerResource, notificationsService, navigationService, $route, treeService,
    appState, overlayService) {

    const vm = this;

    vm.nextPageContent = nextPageContent;
    vm.prevPageContent = prevPageContent;
    vm.changePageContent = changePageContent;
    vm.goToPageContent = goToPageContent;
    vm.recordsPerPageContent = 10;
    vm.recordsContent = [];
    vm.paginationContent = {};

    vm.nextPageMedia = nextPageMedia;
    vm.prevPageMedia = prevPageMedia;
    vm.changePageMedia = changePageMedia;
    vm.goToPageMedia = goToPageMedia;
    vm.recordsPerPageMedia = 10;
    vm.recordsMedia = [];
    vm.paginationMedia = {};

    function nextPageContent(pageNumber) {
        vm.loading = true;
        var offset = (pageNumber - 1) * vm.recordsPerPageContent;
        getPagedContent($routeParams.id, offset, vm.recordsPerPageContent)
    }

    function prevPageContent(pageNumber) {
        vm.loading = true;
        var offset = (pageNumber - 1) * vm.recordsPerPageContent;
        getPagedContent($routeParams.id, offset, vm.recordsPerPageContent)
    }

    function changePageContent(pageNumber) {
        vm.loading = true;
        var offset = (pageNumber - 1) * vm.recordsPerPageContent;
        getPagedContent($routeParams.id, offset, vm.recordsPerPageContent)
    }

    function goToPageContent(pageNumber) {
        vm.loading = true;
        var offset = (pageNumber - 1) * vm.recordsPerPageContent;
        getPagedContent($routeParams.id, offset, vm.recordsPerPageContent)
    }

    function nextPageMedia(pageNumber) {
        vm.loading = true;
        var offset = (pageNumber - 1) * vm.recordsPerPageMedia;
        getPagedMedia($routeParams.id, offset, vm.recordsPerPageMedia)
    }

    function prevPageMedia(pageNumber) {
        vm.loading = true;
        var offset = (pageNumber - 1) * vm.recordsPerPageMedia;
        getPagedMedia($routeParams.id, offset, vm.recordsPerPageMedia)
    }

    function changePageMedia(pageNumber) {
        vm.loading = true;
        var offset = (pageNumber - 1) * vm.recordsPerPageMedia;
        getPagedMedia($routeParams.id, offset, vm.recordsPerPageMedia)
    }

    function goToPageMedia(pageNumber) {
        vm.loading = true;
        var offset = (pageNumber - 1) * vm.recordsPerPageMedia;
        getPagedMedia($routeParams.id, offset, vm.recordsPerPageMedia)
    }

    vm.filter = {
        searchTermContent: "",
        searchTermMedia: ""
    };

    vm.changeTab = changeTab;

    vm.tabs = [
        {
            "alias": "mergeTab",
            "label": "Merge Tags",
            "active": true
        },
        {
            "alias": "contentTab",
            "label": "Tagged Content"
        },
        {
            "alias": "mediaTab",
            "label": "Tagged Media"
        }
    ];

    $scope.cmsTags = {
        tag: "",
        group: ""
    };

    getPagedContent($routeParams.id, 0, vm.recordsPerPageContent);
    getPagedMedia($routeParams.id, 0, vm.recordsPerPageMedia);

    function changeTab(selectedTab) {
        vm.tabs.forEach(function (tab) {
            tab.active = false;
        });
        selectedTab.active = true;
    };

    function removeObjectWithId(arr, id) {
        return arr.filter((obj) => obj.id !== parseInt(id));
    }

    function getPagedContent(id, offset, limit) {
        vm.loading = true;
        TagManagerResource.getPagedContent(id, offset, limit).then(function (response) {
            console.log(response); // logging the response so we know what to do next!

            if (response.data.taggedContent.length > 0) {
                vm.recordsContent = response.data.taggedContent;
            }

            var totalPages = Math.ceil(response.data.totalRecords / limit);

            vm.paginationContent = {
                pageNumber: (offset / vm.recordsPerPageContent) + 1,
                totalPages: totalPages
            };

            vm.loading = false;
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

    function getPagedMedia(id, offset, limit) {
        vm.loading = true;
        TagManagerResource.getPagedMedia(id, offset, limit).then(function (response) {
            console.log(response); // logging the response so we know what to do next!

            if (response.data.taggedMedia.length > 0) {
                vm.recordsMedia = response.data.taggedMedia;
            }

            var totalPages = Math.ceil(response.data.totalRecords / limit);

            vm.paginationMedia = {
                pageNumber: (offset / vm.recordsPerPageMedia) + 1,
                totalPages: totalPages
            };

            vm.loading = false;
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

    TagManagerResource.getTagsById($routeParams.id).then(function (response) {
        $scope.cmsTags = response.data;
        $scope.cmsTags.lisItems = removeObjectWithId($scope.cmsTags.tagsInGroup, $routeParams.id);
        navigationService.syncTree({ tree: 'TagManagerTree', path: ["-1", "tagGroup-" + $scope.cmsTags.tagItem.group, $routeParams.id], forceReload: false });

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
        $scope.cmsTags.mergeTag = tag
    }

    $scope.save = function (cmsTags) {
        TagManagerResource.saveTag(cmsTags).then(function (response) {
            if (response.data === 0) {
                notificationsService.error("Error", "There was a problem saving the tag, please check the logs.");
                return;
            }
            navigationService.syncTree({ tree: 'TagManagerTree', path: ["-1", "tagGroup-" + cmsTags.group, cmsTags.id], forceReload: true });
            notificationsService.success("Success", "'" + cmsTags.tagItem.tag + "' has been saved");
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
    };

    $scope.openOverlay = function (cmsTags) {

        var options = {
            title: 'Confirm',
            content: 'Are you sure you want to delete this tag?',
            disableBackdropClick: true,
            disableEscKey: true,
            confirmType: 'delete',
            submit: function () {
                if ($scope.busy) {
                    return false;
                }
                $scope.busy = true;

                if (cmsTags.tagsInGroup.length === 1) {
                    TagManagerResource.deleteTag(cmsTags).then(function (response) {

                        if (response.data === 0) {
                            notificationsService.error("Error", "There was a problem deleting the tag, please check the logs.");
                            return;
                        }
                        notificationsService.success("Success", "'" + cmsTags.tagItem.tag + "' has been deleted. All tags have been deleted, re-create the group if you need it again");

                        //get the treeNode that we have selected
                        var node = appState.getTreeState("selectedNode");

                        //remove the node
                        treeService.removeNode(node);

                        navigationService.syncTree({ tree: "TagManagerTree", path: ["-1"], forceReload: true }).then(function (syncArgs) {
                            navigationService.reloadNode(syncArgs.node);
                        });;

                        // if the current edited item is the same one as we're deleting, we need to navigate elsewhere
                        $location.path('/TagManager/TagManagerTree/');
                        $route.reload();

                        // return to not busy
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
                    TagManagerResource.deleteTag(cmsTags).then(function (response) {

                        if (response.data === 0) {
                            notificationsService.error("Error", "There was a problem deleting the tag, please check the logs.");
                            return;
                        }
                        notificationsService.success("Success", "'" + cmsTags.tagItem.tag + "' has been deleted.");

                        //get the treeNode that we have selected
                        var node = appState.getTreeState("selectedNode");

                        //remove the node
                        treeService.removeNode(node);

                        // if the current edited item is the same one as we're deleting, we need to navigate elsewhere
                        // for if we want to use the menu to also delete as doesn't neccesarily require redirection                  
                        if ($location.path() == "/" + node.routePath) {
                            //set location to be parent
                            $location.path("/" + node.parent().routePath);
                        }

                        // return to not busy
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
});

angular.module("umbraco").controller("TagManager.GroupController", function ($scope, TagManagerResource,
    $location, $route, notificationsService, appState, treeService, overlayService, navigationService, eventsService) {

    const vm = this;

    vm.filter = {
        searchTerm: ""
    };

    // Initialize an array to hold selected tags
    $scope.selectedTags = [];

    var group = $location.path().substring($location.path().lastIndexOf('/') + 1);
    $scope.title = group;

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

    // Fetch tags for a specific group from the server
    TagManagerResource.getTagsByGroup(group).then(function (response) {
        $scope.cmsTags = response.data;
        navigationService.syncTree({ tree: 'TagManagerTree', path: ["-1", "tagGroup-" + $scope.title], forceReload: false });

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
                    TagManagerResource.deleteTags(cmsTags).then(function (response) {
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
                    TagManagerResource.deleteTags(cmsTags).then(function (response) {
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
});

angular.module("umbraco").controller("TagManager.CreateGroupController", function ($scope, TagManagerResource,
    notificationsService, navigationService, treeService, appState, $location, $route) {

    $scope.cmsTags = {
        group: ""
    };

    function checkExistingGroupName(group, groups) {
        var tagMatch = false;
        Object.entries(groups).forEach(([key, value]) => {
            if (group === value.group) {
                tagMatch = true;
            }
        });
        return tagMatch;
    }

    $scope.save = function (cmsTags) {
        if (!cmsTags.group) {
            notificationsService.error("Error", "Group name is required.");
            return;
        }

        TagManagerResource.getTagGroups(cmsTags.group).then(function (response) {
            var existingGroupName = checkExistingGroupName(cmsTags.group, response.data);

            if (existingGroupName === false) {
                TagManagerResource.createGroup(cmsTags.group).then(function (response) {
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
    $scope.cancel = function (cmsTags) {
        if ($location.path() === "/TagManager/TagManagerTree/create-group") {
            $location.path('/TagManager/');
            $route.reload();
        }
        else {
            navigationService.hideMenu();
        }
    }
});

angular.module("umbraco").controller("TagManager.CreateController", function ($scope, TagManagerResource,
    notificationsService, navigationService, treeService, appState, $location, $route) {

    $scope.cmsTags = {
        tag: "",
        group: ""
    };

    // gets node from menu state first and if null gets from tree state
    var selectedMenu = appState.getMenuState("currentNode");

    if (selectedMenu === null) {
        selectedNode = appState.getTreeState("selectedNode");
        $scope.cmsTags.group = selectedNode.name;
    }
    else {
        $scope.cmsTags.group = selectedMenu.name;
    }

    function checkExistingTagName(tag, tagsFromGroup) {
        var tagMatch = false;
        Object.entries(tagsFromGroup).forEach(([key, value]) => {
            if (tag === value.tag) {
                tagMatch = true;
            }
        });
        return tagMatch;
    }

    $scope.save = function (cmsTags) {
        if (!cmsTags.tag) {
            notificationsService.error("Error", "Tag name is required.");
            return;
        }

        // Fetch tags for a specific group from the server
        TagManagerResource.getTagsByGroup(cmsTags.group).then(function (response) {
            var existingTagName = checkExistingTagName(cmsTags.tag, response.data.tagsInGroup);

            if (existingTagName === false) {
                TagManagerResource.createTag(cmsTags).then(function (response) {
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
});

angular.module("umbraco").controller("TagManager.DeleteController", function ($scope, $location,
    TagManagerResource, notificationsService, navigationService, $route, treeService, appState) {

    var vm = this;

    //get the treeNode that we have selected
    var selectedMenuNode = appState.getMenuState("currentNode");
    var parent = selectedMenuNode.parent();

    TagManagerResource.getTagsById(selectedMenuNode.id).then(function (response) {
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
            TagManagerResource.deleteTag(cmsTags).then(function (response) {
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
            TagManagerResource.deleteTag(cmsTags).then(function (response) {
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
});

