import { BaseModel } from "../../../../../../core/_base/crud";

export class navModel extends BaseModel {
	Id_row: number;
	ContractCode:string;
	ContractID: string;
	Type: string;
	Amount: number;
    RecordDate: string;
    EffectiveDate: string;
	clear() {
		this.ContractID = '';
		this.ContractCode = '';
		this.Type = '';
		this.RecordDate = '';
		this.Amount = 0;
		this.Id_row = 0;
	}
}
