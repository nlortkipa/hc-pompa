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
    pumpRelocation: number
    stationaryRelocation: number
    hydraulicRelocation: number
    stationaryReposition: number
    dismantleAssemble: number
    idleTime: number
    plateNumber: string
    reviewStatus?: 'Pending' | 'Accepted' | 'Rejected' | string
    reviewedAt?: Date | null
    reviewedByCheckerGmail?: string | null
}

export interface PumpOperationFilter {
    fromDate?: string
    toDate?: string
    companyId?: number | null
    addressId?: number | null
    operatorId?: number | null
    plateNumber?: string
}

export interface PumpOperationReviewModel {
    token: string
    reviewStatus: 'Pending' | 'Accepted' | 'Rejected' | string
    reviewedAt?: Date | null
    checkerGmail: string
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
    pumpRelocation: number
    stationaryRelocation: number
    hydraulicRelocation: number
    stationaryReposition: number
    dismantleAssemble: number
    idleTime: number
    plateNumber: string
}
