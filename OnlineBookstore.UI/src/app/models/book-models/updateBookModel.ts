export interface UpdateBookModel {
    id: number;
    name: string;
    description?: string;
    price: number;
    authorId: number;
    publisherId: number;
    genreIds: number[];
    isPaperback: boolean;
    language: string;
}