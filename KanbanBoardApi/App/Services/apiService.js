var Kandban;
(function (Kandban) {
    var ApiService = (function () {
        function ApiService(http) {
            this.http = http;
        }
        ApiService.prototype.createBoard = function (board, failure) {
            this.http.post("http://localhost:2943/boards", board).error(failure);
        };
        ApiService.prototype.getBoards = function (success, failure) {
            this.http.get("http://localhost:2943/boards").success(success).error(failure);
        };
        ApiService.prototype.getBoard = function (slug, success, failure) {
            this.http.get("http://localhost:2943/boards/" + slug).success(success).error(failure);
        };
        ApiService.prototype.createColumn = function (column, board, failure) {
            this.http.post("http://localhost:2943/boards/" + board.Slug + "/columns", column).error(failure);
        };
        ApiService.prototype.getColumns = function (boardSlug, success, failure) {
            this.http.get("http://localhost:2943/boards/" + boardSlug + "/columns/").success(success).error(failure);
        };
        ApiService.prototype.getTasks = function (boardSlug, boardColumnSlug, success, failure) {
            return this.http.get("http://localhost:2943//boards/" + boardSlug + "/columns/" + boardColumnSlug + "/tasks").success(success).error(failure);
        };
        ApiService.prototype.createTask = function (task, board, failure) {
            this.http.post("http://localhost:2943/boards/" + board.Slug + "/tasks", task).error(failure);
        };
        return ApiService;
    })();
    Kandban.ApiService = ApiService;
    angular.module("Kandban").service("ApiService", ["$http", ApiService]);
})(Kandban || (Kandban = {}));
//# sourceMappingURL=apiService.js.map