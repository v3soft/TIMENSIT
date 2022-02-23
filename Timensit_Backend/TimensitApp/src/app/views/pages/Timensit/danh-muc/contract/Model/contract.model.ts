import { BaseModel } from "../../../../../../core/_base/crud";

export class contractModel extends BaseModel {
	Id_row: number;
	InvestorID: string;
	ContractCode: string;
	DepositPeriod: string;
	Fund: string;
	Amount: number;
	ProfitShare: number;
    StartDate: string;
    EndDate: string;
	clear() {
		this.InvestorID = '';
		this.ContractCode = '';
		this.DepositPeriod = '';
		this.Fund = '';
		this.StartDate = '';
		this.EndDate = '';

		this.Amount = 0;
		this.ProfitShare = 0;
		this.Id_row = 0;
	}
}
