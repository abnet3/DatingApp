export interface Pagination {

    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

export class PaginatedResult<T> {
    append(arg0: string, arg1: string): PaginatedResult<import("./member").Member[]> {
      throw new Error('Method not implemented.');
    }

    result!: T;
    pagination!: Pagination;
}