((() => {
    var api = $http => {
        var getboards = () => {
            return $http.get("http://localhost:2943/boards")
                .then((response) => {
                    return response.data;
                });
        };
        var data = { "Slug": "slugg", "Name": "danmo" };
        var createboard = () => {
            return $http.post("http://localhost:2943/boards", data)
                .then((response) => {
                    return response.data;
                });
        }

        return {
            getboards: getboards,
            createboard: createboard
        }
    };

    var module = angular.module("trainingapp");
    module.factory("apiService", api);
})());