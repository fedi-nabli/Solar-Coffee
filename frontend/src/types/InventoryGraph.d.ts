export interface IInventoryTimeline {
  productInventorySnapshots: ISnapshot[];
  timeline: [];
}

export interface ISnapshot {
  productId: number;
  quantityOnHand: number[];
}
