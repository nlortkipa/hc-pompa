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
}