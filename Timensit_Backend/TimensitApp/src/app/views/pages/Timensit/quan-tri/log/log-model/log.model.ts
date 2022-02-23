import { BaseModel } from '../../../../../../core/_base/crud';

    export class LogModel extends BaseModel {
    
        
            Id : number
        
    
        
            DanhMuc : string
        
    
        
            MaDanhMuc : string
        
    
        
            DonVi : number
        
    
        
            Locked : boolean
        
    
        
            Priority : string
        
    
        clear() {
        
            
                this.Id = 0;
            
           
        
            
                
                this.DanhMuc = '';
                
            
           
        
            
                
                this.MaDanhMuc = '';
                
            
           
        
            
                this.DonVi = 0;
            
           
        
            
                
                
                this.Locked = false;
            
           
        
            
                
                this.Priority = '';
                
            
           
        
        }
        copy(item:LogModel){
            
                this.Id = item.Id;
            
                this.DanhMuc = item.DanhMuc;
            
                this.MaDanhMuc = item.MaDanhMuc;
            
                this.DonVi = item.DonVi;
            
                this.Locked = item.Locked;
            
                this.Priority = item.Priority;
            
        }
    }
