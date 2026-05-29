export interface SmsCodeModel {
    id: number
    userId: number
    userNumber: string
    expiresAt: Date
}