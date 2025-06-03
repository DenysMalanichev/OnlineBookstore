export interface CreateNewBook {
    name: string;
    description?: string;
    price: number;
    authorId: number;
    publisherId: number;
    genreIds: number[];
    isPaperback: boolean;
    language: string;
}