import axios from "axios";
import { IProduct } from "@/types/Product";

export class ProductService {
  API_URL = process.env.VUE_APP_API_URL;

  public async archive(productId: number) {
    const result = await axios.patch(`${this.API_URL}/products/${productId}`);
    return result.data;
  }

  public async save(newProduct: IProduct) {
    const result = await axios.post(`${this.API_URL}/product`, newProduct);
    return result.data;
  }
}
