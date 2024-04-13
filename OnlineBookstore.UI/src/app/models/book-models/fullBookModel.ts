export interface FullBookModel {
    name: string;
    price: number;
    description?: string;
    authorId: number;    
    publisherId: number;
    genreIds?: number[];
}