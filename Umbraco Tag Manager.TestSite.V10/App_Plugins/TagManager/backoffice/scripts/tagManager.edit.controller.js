(function () {
    'use strict';

    function editTag($scope, $routeParams, $location, tagManagerResource, notificationsService, navigationService,
        $route, treeService, appState, overlayService) {
        var vm = this;

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
            languageId: null,
            group: "",
            alias: "mergeTags",
            label: "Select Tag",
            description: "The selected tag will be merged and will inherit this tags associations with content and media.",
            config: {
                multiple: false,
            },
            mergeTag: [],
            view: "/App_Plugins/TagManager/backoffice/propertyEditor/dropdownFlexible.html"
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
            tagManagerResource.getPagedContent(id, offset, limit).then(function (response) {
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
            tagManagerResource.getPagedMedia(id, offset, limit).then(function (response) {
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

        tagManagerResource.getTagsById($routeParams.id).then(function (response) {
            //$scope.cmsTags = response.data;
            $scope.cmsTags.mergeTag = null;
            $scope.cmsTags.tagItem = response.data.tagItem;
            $scope.cmsTags.tagsInGroup = response.data.tagsInGroup;
            $scope.cmsTags.mergeList = removeObjectWithId($scope.cmsTags.tagsInGroup, $routeParams.id);

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
            tagManagerResource.saveTag(cmsTags).then(function (response) {
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
                        tagManagerResource.deleteTag(cmsTags).then(function (response) {

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
                        tagManagerResource.deleteTag(cmsTags).then(function (response) {

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
    }

    angular.module('umbraco').controller('tagManager.edit.controller', editTag);
})();
