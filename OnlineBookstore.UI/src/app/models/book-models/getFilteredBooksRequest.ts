export interface GetFilteredBooksRequest {
    name?: string;
    authorName?: string;
    publisherId?: number;
    isDescending?: number;    
    minPrice?: number;
    maxPrice?: number;    
    page: number;
    itemsOnPage: number;    
    genres?: number[];
}