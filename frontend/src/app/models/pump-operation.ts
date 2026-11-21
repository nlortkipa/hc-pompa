export interface PumpOperationModel {
    id: number
    date: Date
    operatorId: number
    operatorFirstName: string
    operatorLastName: string
    companyId: number
    companyName: string
    addressId: number
    addressName: string
    concreteVolume: number
    additionalStanding: number
    dismantleAssemble: number
    isIdle: boolean
    isDaily: boolean
    plateNumber: string
    pumpType: string
    distanceRange?: '<100მ' | '> 100მ' | null
    note?: string | null
    reviewStatus?: 'Pending' | 'Accepted' | 'Rejected' | string
    reviewedAt?: Date | null
    reviewedByCheckerGmail?: string | null
}

export interface PumpOperationFilter {
    fromDate?: string
    toDate?: string
    companyId?: number | null
    companyName?: string
    addressId?: number | null
    operatorId?: number | null
    operatorName?: string
    plateNumber?: string
}

export interface PumpOperationReviewModel {
    token: string
    /** Changes whenever the operator edits the operation; sent back with a decision. */
    version: string
    reviewStatus: 'Pending' | 'Accepted' | 'Rejected' | string
    reviewedAt?: Date | null
    reviewedByCheckerGmail?: string | null
    checkerGmail?: string | null
    pumpOperationId: number
    date: Date
    operatorId: number
    operatorFirstName: string
    operatorLastName: string
    companyId: number
    companyName: string
    addressId: number
    addressName: string
    concreteVolume: number
    additionalStanding: number
    dismantleAssemble: number
    isIdle: boolean
    isDaily: boolean
    plateNumber: string
    pumpType: string
    distanceRange?: '<100მ' | '> 100მ' | null
    note?: string | null
}
