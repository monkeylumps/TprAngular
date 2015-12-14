module Kandban {
    export class MainController {
        constructor(private scope: IMainScope, private apiService: IApiService, $uibModal) {
            this.scope.greeting = "Welcome buddy";

            const boards = data => {
                this.scope.boards = data.Items;
                this.scope.state = 1;
            };

            const error = reason => {
                //rg4js()
                //Raygun.send(new Error('Failed $http request', reason));
                this.scope.error = reason;
                this.scope.state = 3;
            };

            const columns = data => {
                this.scope.board.Columns = data.Items;

                var thing = scope.board.Columns;
                angular.forEach(thing, (column) => {
                    apiService.getTasks(scope.board.Slug, column.Slug, (x) => {
                        column.Tasks = x.Items;
                    }, error);
                });
            }

            const board = data => {
                this.scope.board = data;
                this.scope.state = 2;

                apiService.getColumns(data.Slug, columns, error);
            };

            this.scope.changeBoard = (slug: string) => {
                apiService.getBoard(slug, board, error);
            }

            this.scope.openModal = () => {
                $uibModal.open({
                    animation: true,
                    templateUrl: 'myModalBoardContent.html',
                    controller: 'ModalInstanceController',
                    scope: this.scope
                });
            }

            this.scope.openColumnModal = () => {
                $uibModal.open({
                    animation: true,
                    templateUrl: 'myModalColumnContent.html',
                    controller: 'ModalInstanceController',
                    scope: this.scope
                });
            }

            this.scope.openTaskModal = () => {
                $uibModal.open({
                    animation: true,
                    templateUrl: 'myModalTaskContent.html',
                    controller: 'ModalInstanceController',
                    scope: this.scope
                });
            }

            this.scope.$on('BoardCreated', (event, args) => {
                apiService.createBoard(args, error);

                this.scope.boards.push(args);
            });

            this.scope.$on('ColumnCreated', (event, args: IColumn, board: IBoard) => {
                apiService.createColumn(args, board, error);

                this.scope.board.Columns.push(args);
            });

            this.scope.$on('TaskCreated', (event, args: ITask, board: IBoard) => {
                apiService.createTask(args, board, error);

                angular.forEach(this.scope.board.Columns, (column) => {
                    if (column.Slug === args.BoardColumnSlug) {
                        column.Tasks.push(args);
                    }
                });
            });

            this.scope.back = () => {
                this.scope.state = 1;
            }

            this.scope.onDragComplete = (data: ITask, event, column: IColumn) => {
                this.scope.sourceColumn = column;
            }

            this.scope.onDropComplete = (data: ITask, column: IColumn) => {
                data.BoardColumnSlug = column.Slug;

                column.Tasks.push(data);

                var index = this.scope.sourceColumn.Tasks.indexOf(data);

                this.scope.sourceColumn.Tasks.splice(index, 1);

                this.scope.hypno = true;
            }

            apiService.getBoards(boards, error);
        }
    }

    angular.module("Kandban").controller("MainController", ["$scope", "ApiService", "$uibModal", MainController]);
}