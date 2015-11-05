module Kandban {
    export class ModalInstanceController {
        constructor(private scope: IMainScope, private apiService: IApiService, $uibModalInstance) {
            this.scope.addBoard = (board: IBoard, isValid: boolean) => {
                if (isValid) {
                    board.Slug = board.Name;

                    this.scope.$emit('BoardCreated', board);

                    $uibModalInstance.close();
                }
            }

            this.scope.addColumn = (column: IColumn, board: IBoard, isValid: boolean) => {
                if (isValid) {
                    column.Slug = column.Name;

                    this.scope.$emit('ColumnCreated', column, board);

                    $uibModalInstance.close();
                }
            }

            this.scope.addTask = (task: ITask, column: IColumn, board: IBoard, isValid: boolean) => {
                if (isValid) {
                    task.BoardColumnSlug = column.Slug;

                    this.scope.$emit('TaskCreated', task, board);

                    $uibModalInstance.close();
                }
            }

            this.scope.close = () => {
                $uibModalInstance.dismiss('cancel');
            }
        }
    }

    angular.module("Kandban").controller("ModalInstanceController", ["$scope", "ApiService", "$uibModalInstance", ModalInstanceController]);
}