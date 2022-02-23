import { stringify } from 'querystring';
import { BaseModel } from "../../../../../../core/_base/crud";

export class chedouudaiModel extends BaseModel {
    Id: number;
    CheDoUuDai: string;
    MoTa: string;
    Locked: boolean;
    Priority: number;
    CreatedBy: number;
    CreatedDate: string;
    UpdatedBy: number;
    UpdatedDate: string;

	clear() {
        this.CheDoUuDai = '';
		this.MoTa = '';
		this.Locked = true;
        this.Priority = 1;
        this.CreatedBy = 0;
        this.CreatedDate = '';
        this.UpdatedBy = 0;
        this.UpdatedDate = '';

        this.Id = 0;
	}
}