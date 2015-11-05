module Kandban {
    export interface IMainScope extends ng.IScope {
        greeting: string;
        state: number;
        error: string;
        hypno;
        boards: IBoard[];
        board: IBoard;
        column: IColumn;
        sourceColumn: IColumn;
        task: ITask;

        openModal();
        openColumnModal();
        openTaskModal();
        addBoard(board: IBoard, isValid: boolean);
        close();
        changeBoard(slug: string);

        back();

        addColumn(column: IColumn, board: IBoard, isValid: boolean);
        addTask(task: ITask, column: IColumn, board: IBoard, isValid: boolean);

        onDragComplete(data: ITask, event, column: IColumn);
        onDropComplete(data: ITask, column: IColumn);
    }
}