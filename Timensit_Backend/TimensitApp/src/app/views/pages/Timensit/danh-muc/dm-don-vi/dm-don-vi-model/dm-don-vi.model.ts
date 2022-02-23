import { BaseModel } from 'app/core/_base/crud';

    export class DM_DonViModel extends BaseModel {
    
        
            Id : number
        
    
        
            LoaiDonVi : number
        
    
        
            DonVi : string
        
    
        
            MaDonvi : string
        
    
        
            MaDinhDanh : string
        
    
        
            Parent : number
            ParentName : string
        
    
        
            SDT : string
        
    
        
            Email : string
        
    
        
            DiaChi : string
        
    
        
            Logo : string
        
    
        
            Locked : boolean
        
    
        
            Priority : number
        
    
        
            DangKyLichLanhDao : boolean
        
    
        
            KhongCoVanThu : boolean
        
    
        
            CreatedDate : string
        
    
        
            UpdatedBy : string
        
    
        
            UpdatedDate : string
        
    
        
            Disabled : boolean
            listLinkImage: ListImageModel[]
            IsShow: boolean;
            LoaiXL: number=3
        clear() {      
                this.Id = 0;
                this.LoaiDonVi = 0;
                this.DonVi = '';
                this.MaDonvi = '';
                this.MaDinhDanh = '';
                
                this.Parent = 0;
                
            
           
        
            
                
                this.SDT = '';
                
            
           
        
            
                
                this.Email = '';
                
            
           
        
            
                
                this.DiaChi = '';
                
            
           
        
            
                
                this.Logo = "";
                
            
           
        
            
                
                
                this.Locked = false;
            
           
        
            
                
                this.Priority = 0;
                
            
           
        
            
                
                
                this.DangKyLichLanhDao = false;
            
           
        
            
                
                
                this.KhongCoVanThu = false;
            
           
        
            
                
                this.CreatedDate = '';
                
            
           
        
            
                
                this.UpdatedBy = '';
                
            
           
        
            
                
                this.UpdatedDate = '';
                
            
           
        
            
                
                
                this.Disabled = false;
            this.IsShow=false;
           this.LoaiXL=3;
        
        }
        copy(item:DM_DonViModel){
            
                this.Id = item.Id;
            
                this.LoaiDonVi = item.LoaiDonVi;
            
                this.DonVi = item.DonVi;
            
                this.MaDonvi = item.MaDonvi;
            
                this.MaDinhDanh = item.MaDinhDanh;
            
                this.Parent = item.Parent;
            
                this.SDT = item.SDT;
            
                this.Email = item.Email;
            
                this.DiaChi = item.DiaChi;
            
                this.Logo = item.Logo;
            
                this.Locked = item.Locked;
            
                this.Priority = item.Priority;
            
                this.DangKyLichLanhDao = item.DangKyLichLanhDao;
            
                this.KhongCoVanThu = item.KhongCoVanThu;
            
                this.CreatedDate = item.CreatedDate;
            
                this.UpdatedBy = item.UpdatedBy;
            
                this.UpdatedDate = item.UpdatedDate;
            
                this.Disabled = item.Disabled;
                this.IsShow=item.IsShow;
                this.LoaiXL=item.LoaiXL?item.LoaiXL:3;
        }
    }

    export class DM_User_DonViModel extends BaseModel {
    
        
        UserID : number
    

    
        Username : string
    

    
        FullName : string
    

    
        PhoneNumber : string
    

    
        ChucVu : string    


    
        Email : string    

    
        Active : number
        LoaiXL: number=3
        Type: number;//0: người dùng , 1: đơn vị,
        TenDonVi: string;     
        IdDV: number;     

    clear() {   
            this.UserID = 0;        
            this.Username = '';
            this.FullName = '';
            this.ChucVu = '';
            this.Email = '';
            this.PhoneNumber = '';           
            this.Active = 0;      
            this.LoaiXL=3; 
            this.Type=0;
            this.TenDonVi='';
        
    }
    copy( item: DM_User_DonViModel  ){
        this.UserID = item.UserID;        
        this.Username = item.Username;
        this.FullName = item.FullName;
        this.ChucVu = item.ChucVu;
        this.Email = item.Email;
        this.PhoneNumber = item.PhoneNumber;           
        this.Active = item.Active;  
        this.Type = item.Type?item.Type:0;  
        this.LoaiXL=item.LoaiXL?item.LoaiXL:3;
        this.TenDonVi= item.TenDonVi;
        this.IdDV= item.IdDV;
    }
    copyDonVi( item: DM_DonViModel){
        this.UserID = item.Id;        
        this.Username = item.MaDonvi;
        this.FullName = item.DonVi;
        this.ChucVu = '';
        this.Email = '';
        this.PhoneNumber = item.SDT;           
        this.Active = 1;  
        this.Type = 1;  
        this.LoaiXL=item.LoaiXL?item.LoaiXL:3;
        this.TenDonVi=item.DonVi;
        this.IdDV= item.Id;
    }   
}
export class ListImageModel extends BaseModel {
    strBase64 : string
    filename : string
    type : string
    src : string
    IsAdd : boolean
    IsDel : boolean
    IsImagePresent : boolean
clear() {
        this.strBase64 = '';
        this.filename = '';
        this.src = '';
        this.IsAdd = false;
        this.IsDel = false;
        this.IsImagePresent = false;
}
copy(item:ListImageModel){
        this.strBase64 = item.strBase64;
        this.filename = item.filename;
        this.src = item.src;
        this.IsAdd = item.IsAdd;
        this.IsDel = item.IsDel;
        this.IsImagePresent = item.IsImagePresent;
}
}


