angular.module("umbraco.resources").factory("TagListResource", function ($http) {
    return {
        /* umbraco api call to get all tags by group */ 
        getTagsByGroup: function (group, culture) {
            return $http.get("backoffice/Api/TagList/GetTagsByGroup/?group=" + group + "&culture=" + culture);
        }
    };
});