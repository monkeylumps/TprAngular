var Kandban;
(function (Kandban) {
    var ApiService = (function () {
        function ApiService(http) {
            this.http = http;
        }
        ApiService.prototype.createBoard = function (board, success, failure) {
            this.http.post("http://localhost:2943/boards", board).success(success).error(failure);
        };
        ApiService.prototype.getBoards = function (success, failure) {
            this.http.get("http://localhost:2943/boards").success(success).error(failure);
        };
        ApiService.prototype.getBoard = function (slug, success, failure) {
            this.http.get("http://localhost:2943/boards/" + slug).success(success).error(failure);
        };
        return ApiService;
    })();
    Kandban.ApiService = ApiService;
    angular.module("Kandban").service("ApiService", ["$http", ApiService]);
})(Kandban || (Kandban = {}));
//# sourceMappingURL=apiService.js.map