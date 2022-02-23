import { BaseModel } from "../../../../../../core/_base/crud";

export class OrgStructureModel extends BaseModel {
	IDParent: string;
	TitleParent: string;
	Level: string;
	Position: string;
	Title: string;
	Code: string;
	ViTri: string;
	ID: string;
	RowID: number;
	ParentID: string;
	ID_Goc: string;

	drop_idfrom: string;
	drop_id_parent: string;
	drop_namefrom: string;
	drop_idto: string;
	drop_levelto: string;
	drop_nameto: string;
	drop_childrensto: string;
	StructureID: string;
	Id_parent: string;
	chucdanhParent: string;
	WorkingModeID: number;
	level: string;
	IsAbove: boolean;
	clear() {
		this.IDParent = '';
		this.TitleParent = '';
		this.Level = '';
		this.Position = '';
		this.Title = '';
		this.ID = '0';
		this.RowID = 0;
		this.Code = '';
		this.ViTri = '0';
		this.ParentID = '0';
		this.ID_Goc = null;
		this.WorkingModeID = 0;
		this.level = '';
		this.IsAbove = true;
	}
}
