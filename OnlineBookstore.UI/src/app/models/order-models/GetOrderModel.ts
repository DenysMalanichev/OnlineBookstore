import { GetOrderDetailModel } from "./GetOrderDetailModel";

export interface GetOrderModel {
    id: number;
    shipCity?: string;
    shipAddress?: string;
    orderDetails: GetOrderDetailModel[];
}