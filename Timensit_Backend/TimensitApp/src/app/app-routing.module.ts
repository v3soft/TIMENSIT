// Angular
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MaterialModule } from './views/pages/material/material.module';

const routes: Routes = [
	{ path: 'auth', loadChildren: () => import('../app/views/pages/auth/auth.module').then(m => m.AuthModule) },
	{ path: 'viewer', loadChildren: () => import('../app/views/pages/Timensit/file-viewer/file-viewer.module').then(m => m.FileViewerModule) },
	{ path: 'material', loadChildren: () => MaterialModule },
	{ path: '', loadChildren: () => import('../app/views/theme/theme.module').then(m => m.ThemeModule) },
	{ path: '**', redirectTo: 'error/403', pathMatch: 'full' }
];

@NgModule({
	imports: [
		RouterModule.forRoot(routes)
	],
	exports: [RouterModule]
})
export class AppRoutingModule {
}
