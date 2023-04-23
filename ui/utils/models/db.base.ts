export interface DbObject {
    id: number;
    createdAt: Date | string;
    updatedAt: Date | string;
    deletedAt?: Date | string;
}