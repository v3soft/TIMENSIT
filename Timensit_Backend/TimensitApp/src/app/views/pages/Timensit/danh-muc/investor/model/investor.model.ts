import { BaseModel } from '../../../../../../core/_base/crud';

export class investorModel extends BaseModel {
	ID: number;
	Name: string;
	Email: string;
	Phone: string;
	Address: string;
	CitizenID: string;
	clear() {
		this.ID = 0;
		this.Name = '';
		this.Email = '';
		this.Phone = '';
		this.Address = '';
		this.CitizenID = '';
	}
}
