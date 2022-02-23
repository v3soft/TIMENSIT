export class PageModel {
	Page: number;
	AllPage: number;
	Size: number;
    Total: number;
    constructor() 
    {
        this.Page = 0;
        this.AllPage = 0;
        this.Size = 10;
        this.Total = 0;
    }
    setNewValue(_page:number,_size:number,_allpage:number=0,_total :number=0 ){
        this.Page =_page;
        this.AllPage = _allpage;
        this.Size = _size;
        this.Total = _total;
    }
}