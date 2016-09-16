module Kandban {
    export interface IBoard {
        Slug: string;
        Name: string;
        Columns: IColumn[];
    }
}