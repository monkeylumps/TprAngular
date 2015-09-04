((() => {
    var app = angular.module("trainingapp");

    var MainController = ($scope, apiService) => {
        $scope.greeting = "welcome buddy";

        var boardsss = data => {
            $scope.data = data.Items;
        }

        var boardsssPost = data => {
            $scope.dataPost = data;
        }

        var error = reason => {
            $scope.error = reason;
        }
        //apiService.createboard().then(boardsssPost, error);
        apiService.getboards().then(boardsss, error);
    };

    app.controller("MainController", ["$scope", "apiService", MainController]);
})());