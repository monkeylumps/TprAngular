var Kandban;
(function (Kandban) {
    var MainController = (function () {
        function MainController(apiService) {
            var _this = this;
            this.apiService = apiService;
            this.greeting = "welcome buddy";
            var boardsss = function (data) {
                _this.boards = data.Items;
            };
            var error = function (reason) {
                _this.error = reason;
            };
            apiService.getBoards(boardsss, error);
        }
        MainController.prototype.getboard = function (slug) {
            var _this = this;
            this.apiService.getBoard(slug, function (data) {
                _this.board = data.Name;
            }, function (reason) {
                _this.error = reason;
            });
        };
        return MainController;
    })();
    Kandban.MainController = MainController;
    angular.module("Kandban").controller("MainController", ["ApiService", MainController]);
})(Kandban || (Kandban = {}));
//# sourceMappingURL=MainController.js.map