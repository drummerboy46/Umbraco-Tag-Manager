angular.module("umbraco.resources").factory("tagManagerResource", function ($http) {
    return {
        getTagsById: function (id) {
            return $http.get("backoffice/TagManager/TagManagerApi/GetTagsById?id=" + id);
        },
        getTagsByGroup: function (group) {
            return $http.get("backoffice/TagManager/TagManagerApi/GetTagsByGroup?group=" + group);
        },
        getPagedContent: function (id, offset, limit) {
            return $http.get("backoffice/TagManager/TagManagerApi/GetPagedContent?id=" + id + "&offset=" + offset + "&limit=" + limit);
        },
        getPagedMedia: function (id, offset, limit) {
            return $http.get("backoffice/TagManager/TagManagerApi/GetPagedMedia?id=" + id + "&offset=" + offset + "&limit=" + limit);
        },
        getTagGroups: function () {
            return $http.get("backoffice/TagManager/TagManagerApi/GetTagGroups");
        },
        createGroup: function (group) {
            return $http.get("backoffice/TagManager/TagManagerApi/CreateGroup?group=" + group);
        },
        createTag: function (cmsTags) {
            return $http.post("backoffice/TagManager/TagManagerApi/CreateTag", angular.toJson(cmsTags));
        },
        saveTag: function (cmsTags) {
            return $http.post("backoffice/TagManager/TagManagerApi/SaveTag", angular.toJson(cmsTags));
        },
        deleteTag: function (cmsTags) {
            return $http.post("backoffice/TagManager/TagManagerApi/DeleteTag", angular.toJson(cmsTags));
        },
        deleteTags: function (cmsTags) {
            return $http.post("backoffice/TagManager/TagManagerApi/DeleteTags", angular.toJson(cmsTags));
        }
    };
});