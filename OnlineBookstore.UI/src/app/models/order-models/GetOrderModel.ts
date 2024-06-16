import { GetOrderDetailModel } from "./GetOrderDetailModel";

export interface GetOrderModel {
    id: number;
    shipCity?: string;
    shipAddress?: string;
    status: string;
    orderDetails: GetOrderDetailModel[];
}