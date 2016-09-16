module Kandban {
    export interface IColumn {
        Slug: string;
        Name: string;
        Order: number;
        Tasks: ITask[];
    }
}