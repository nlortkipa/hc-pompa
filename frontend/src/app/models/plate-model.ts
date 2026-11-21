export interface PlateChecker {
    userId: number;
    firstName: string;
    lastName: string;
    email?: string | null;
}

export interface Plate {
    id: number;
    number: string;
    type: string;
    isStationary: boolean;
    checkerUserIds: number[];
    checkers: PlateChecker[];
}
