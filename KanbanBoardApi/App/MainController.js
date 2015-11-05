var Kandban;
(function (Kandban) {
    var MainController = (function () {
        function MainController(scope, apiService, $uibModal) {
            var _this = this;
            this.scope = scope;
            this.apiService = apiService;
            this.scope.greeting = "Welcome buddy";
            var boards = function (data) {
                _this.scope.boards = data.Items;
                _this.scope.state = 1;
            };
            var error = function (reason) {
                _this.scope.error = reason;
                _this.scope.state = 3;
            };
            var columns = function (data) {
                _this.scope.board.Columns = data.Items;
                var thing = scope.board.Columns;
                angular.forEach(thing, function (column) {
                    apiService.getTasks(scope.board.Slug, column.Slug, function (x) {
                        column.Tasks = x.Items;
                    }, error);
                });
            };
            var board = function (data) {
                _this.scope.board = data;
                _this.scope.state = 2;
                apiService.getColumns(data.Slug, columns, error);
            };
            this.scope.changeBoard = function (slug) {
                apiService.getBoard(slug, board, error);
            };
            this.scope.openModal = function () {
                $uibModal.open({
                    animation: true,
                    templateUrl: 'myModalContent.html',
                    controller: 'ModalInstanceController',
                    scope: _this.scope
                });
            };
            this.scope.openTaskModal = function () {
                $uibModal.open({
                    animation: true,
                    templateUrl: 'myModalTaskContent.html',
                    controller: 'ModalInstanceController',
                    scope: _this.scope
                });
            };
            this.scope.$on('BoardCreated', function (event, args) {
                apiService.createBoard(args, error);
                _this.scope.boards.push(args);
            });
            this.scope.$on('ColumnCreated', function (event, args, board) {
                apiService.createColumn(args, board, error);
                _this.scope.board.Columns.push(args);
            });
            this.scope.$on('TaskCreated', function (event, args, board) {
                apiService.createTask(args, board, error);
                angular.forEach(_this.scope.board.Columns, function (column) {
                    if (column.Slug === args.BoardColumnSlug) {
                        column.Tasks.push(args);
                    }
                });
            });
            this.scope.back = function () {
                _this.scope.state = 1;
            };
            this.scope.onDragComplete = function (data, event, column) {
                _this.scope.sourceColumn = column;
            };
            this.scope.onDropComplete = function (data, column) {
                data.BoardColumnSlug = column.Slug;
                column.Tasks.push(data);
                var index = _this.scope.sourceColumn.Tasks.indexOf(data);
                _this.scope.sourceColumn.Tasks.splice(index, 1);
                _this.scope.hypno = true;
            };
            apiService.getBoards(boards, error);
        }
        return MainController;
    })();
    Kandban.MainController = MainController;
    angular.module("Kandban").controller("MainController", ["$scope", "ApiService", "$uibModal", MainController]);
})(Kandban || (Kandban = {}));
//# sourceMappingURL=MainController.js.map