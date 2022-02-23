// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: true,
  isMockEnabled: true, // You have to switch this, when your real back-end is done
	authTokenKey: 'authce9d77b308c149d5992a80073637e4d5',
	SignalR: 'http://testncc-api.vts-demo.com/signalr',
	ApiRoot: 'http://testncc-api.vts-demo.com/api',
	pageSize: [3, 5, 10, 20],
	numShowCaptcha:3,
	YOUR_SITE_KEY:"6LeOoN8ZAAAAAJggYafGWFhpWRL7C2eHO9x_RWL0",
	DungLuong: 104857600,
	BERoot: "http://testncc.vts-demo.com/"
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
