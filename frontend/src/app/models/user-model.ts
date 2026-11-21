export interface UserModel {
    id: number
    firstName: string
    lastName: string
    personalId: string
    phoneNumber: string
    createdAt: Date
    role: string
    username?: string | null
    avatarUrl?: string | null
    email?: string | null
}
