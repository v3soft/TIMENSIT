import { BaseModel } from "../../../../../../core/_base/crud";

export class receiptsModel extends BaseModel {
	Id_row: number;
	ContractCode:string;
	ContractID: string;
	Type: string;
	Amount: number;
    ReceiptDate: string;
    EffectiveDate: string;
	clear() {
		this.ContractID = '';
		this.ContractCode = '';
		this.Type = '';
		this.ReceiptDate = '';
		this.EffectiveDate = '';

		this.Amount = 0;
		this.Id_row = 0;
	}
}
