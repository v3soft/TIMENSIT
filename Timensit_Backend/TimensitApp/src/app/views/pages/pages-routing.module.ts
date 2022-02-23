import { CanActivate } from '@angular/router';
// Angular
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
// Auth
import { AuthGuard } from '../../core/auth';

import { BaseComponent } from '../theme/base/base.component';
import { PermissionUrl } from '../../core/auth/_services/permissionurl';
import { ErrorPageComponent } from '../theme/content/error-page/error-page.component';

const routes: Routes = [
	{
		path: '',
		component: BaseComponent,
		canActivate: [AuthGuard],
		children: [
			{
				path: '',
				loadChildren: () => import('../../views/pages/dashboard/dashboard.module').then(m => m.DashboardModule)
			},
			{
				path: 'profile',
				loadChildren: () => import('../../views/pages/profile/profile.module').then(m => m.ProfileModule)
			},
			{
				path: 'chuc-vu',
				canActivate: [PermissionUrl],
				loadChildren: () => import('../../views/pages/Timensit/danh-muc/chucvu/chucvu.module').then(m => m.chucvuModule)
			},
			{
				path: 'chuc-danh',
				canActivate: [PermissionUrl],
				loadChildren: () => import('../../views/pages/Timensit/danh-muc/chucdanh/chucdanh.module').then(m => m.ChucDanhModule)
			},
			{
				path: 'so-do-to-chuc',
				canActivate: [PermissionUrl],
				loadChildren: () => import('../../views/pages/Timensit/danh-muc/so-do-to-chuc-new/so-do-to-chuc-new.module').then(m => m.SoDoToChucModule)
			},
			{
				path: 'co-cau-to-chuc',
				canActivate: [PermissionUrl],
				loadChildren: () => import('../../views/pages/Timensit/danh-muc/co-cau-to-chuc-moi-tree/co-cau-to-chuc-moi-tree.module').then(m => m.cocautochucmoiTreeModule)
			},
			{
				path: 'investor',
				canActivate: [PermissionUrl],
				loadChildren: () => import('../../views/pages/Timensit/danh-muc/investor/investor.module').then(m => m.investorModule)
			},
			{
				path: 'contract',
				canActivate: [PermissionUrl],
				loadChildren: () => import('../../views/pages/Timensit/danh-muc/contract/contract.module').then(m => m.contractModule)
			},
			{
				path: 'receipts',
				canActivate: [PermissionUrl],
				loadChildren: () => import('../../views/pages/Timensit/danh-muc/receipts/receipts.module').then(m => m.receiptsModule)
			},
			{
				path: 'nav',
				canActivate: [PermissionUrl],
				loadChildren: () => import('../../views/pages/Timensit/danh-muc/nav/nav.module').then(m => m.navModule)
			},
			//#endregion
			//#region Quản trị
			{
				path: 'vai-tro',
				canActivate: [PermissionUrl],
				loadChildren: () => import('../../views/pages/Timensit/nguoi-dung/nhom-nguoi-dung-dps/nhom-nguoi-dung-dps.module').then(m => m.NhomNguoiDungDPSModule)
			},
			{
				path: 'nguoi-dung',
				canActivate: [PermissionUrl],
				loadChildren: () => import('../../views/pages/Timensit/nguoi-dung/nguoi-dung-dps/nguoi-dung-dps.module').then(m => m.NguoiDungDPSModule)
			},
			{
				path: 'don-vi',
				loadChildren: () => import('../../views/pages/Timensit/danh-muc/dm-don-vi/dm-don-vi.module').then(m => m.DM_DonViModule)
			},
			//#region Quản lý
			{
				path: 'log',
				//canActivate: [PermissionUrl],
				loadChildren: () => import('../../views/pages/Timensit/quan-tri/log/log.module').then(m => m.LogModule)
			},
			//#endregion
			//#region cấu hình
			{
				path: 'cau-hinh-he-thong',
				canActivate: [PermissionUrl],
				loadChildren: () => import('../../views/pages/Timensit/cau-hinh/config/config.module').then(m => m.ConfigModule)
			},
			{
				path: 'cau-hinh-email',
				canActivate: [PermissionUrl],
				loadChildren: () => import('../../views/pages/Timensit/cau-hinh/cau-hinh-email/cau-hinh-email.module').then(m => m.CauHinhEmailModule)
			},
			//#endregion

			//#endregion,
			{
				path: 'builder',
				loadChildren: () => import('../../views/theme/content/builder/builder.module').then(m => m.BuilderModule)
			},
			{
				path: 'error/403',
				component: ErrorPageComponent,
				data: {
					type: 'error-v6',
					code: 403,
					title: '403... Access forbidden',
					desc: 'Looks like you don\'t have permission to access for requested page.<br> Please, contract administrator',
				},
			},
			{
				path: 'error/404',
				component: ErrorPageComponent,
				data: {
					type: 'error-v6',
					code: 404,
					title: '404... Page Not Found',
					desc: 'This page could not be found on the server.',
				}
			},
			{ path: 'error/:type', component: ErrorPageComponent },
			//{path: '', redirectTo: '', pathMatch: 'full'},
			{ path: '**', redirectTo: 'error/404', pathMatch: 'full' },
		],
	}
];
@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule]
})
export class PagesRoutingModule {
}
