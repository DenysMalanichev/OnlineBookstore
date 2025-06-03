import { GetOrderDetailModel } from "./GetOrderDetailModel";

export interface GetOrderModel {
    id: number;
    shipCity?: string;
    shipAddress?: string;
    status: string;
    orderClosed: Date;
    orderDetails: GetOrderDetailModel[];
}