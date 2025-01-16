export interface Pagination {
    currentPage:number;
    itemPerPage:number;
    totalItems:number;
    totalPages:number ;
}

export class paginationResults<T>{
    items?:T ;//Member[]
    pagination?:Pagination;
}