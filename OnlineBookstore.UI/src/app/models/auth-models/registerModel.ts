export interface RegisterModel {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    confirmPassword: string;
    preferedLanguages: string[];
    preferedGenreIds: number[];
    preferedAuthoreIds: number[];
    isPaperbackPrefered: boolean;
}