export interface PageableResponse<T> {
    entities: T[];
    totalPages: number;
    currentPage: number;
}