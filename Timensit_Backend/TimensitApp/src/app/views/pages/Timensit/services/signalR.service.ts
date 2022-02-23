import * as signalR from "@aspnet/signalr";
import { Injectable, EventEmitter } from '@angular/core';
import { environment } from "../../../../../environments/environment";
import { TokenStorage } from "../../../../core/auth/_services/token-storage.service";
const connection = new signalR.HubConnectionBuilder()
	.withUrl(environment.SignalR, {
		skipNegotiation: true,
		transport: signalR.HttpTransportType.WebSockets
	})
	.build();

@Injectable()
export class SignalRService {
	message: string = "";
	info: string = "";
	notifyReceived: EventEmitter<ThongBaoModel[]>;
	constructor(
		//   private changeDetectorRefs: ChangeDetectorRef,
		private tokenStorage: TokenStorage
	) {
		this.notifyReceived = new EventEmitter <ThongBaoModel[]>();
		this.connectToken();
		connection.onclose(() => {
			setTimeout(r => {
				this.reconnectToken();
			}, 5000);
		})
	}
	//  initializeSignalRConnection (){
	//      this.connectToken();
	//     //  this.registerEvent();
	//  }
	connectToken() {
		connection.start().then(() => {
			var _token = '';
			var _userID = -1;
			this.tokenStorage.getAccessToken().subscribe(t => { _token = t; });
			this.tokenStorage.getUserInfo().subscribe(user => {
				_userID = user.id;
			})
			let infoTokenCon = { "Token": _token, "UserID": _userID };
			connection.invoke("onConnectedTokenAsync", JSON.stringify(infoTokenCon));

		}).catch(err => {
			// document.write(err);
			console.log("error", err);
		});

		connection.on("recieveMessaged", (data: any) => {
			this.notifyReceived.emit(data);
		});

	}

	ReceiveThongBao() {
		var _token = '';
		var _userID = -1;
		this.tokenStorage.getAccessToken().subscribe(t => { _token = t; });
		this.tokenStorage.getUserInfo().subscribe(user => {
			_userID = user.id;
		})
		let infoTokenCon = { "Token": _token, "UserID": _userID, "Value": this.message };
		connection.send("getListNotify", JSON.stringify(infoTokenCon))
			.then(() => this.message = "");
	}
	reconnectToken(): void {
		var _token = '', _idUser = "0";
		this.tokenStorage.getAccessToken().subscribe(t => { _token = t; });
		this.tokenStorage.getUserInfo().subscribe(user => {
			_idUser = user.id;
		})
		let infoTokenCon = { "Token": _token, "UserID": _idUser };
		connection.start().then((data: any) => {
			console.log('Connect with ID', data);
			connection.invoke("ReconnectToken", JSON.stringify(infoTokenCon)).then(() => {
			});
		}).catch((error: any) => {
			console.log('Could not ReconnectToken! ', error);
		});
		///  console.log('Connect with ID',this.proxy.id);
	}
	disconnectToken() {
		var _token = '';
		var _userID = -1;
		this.tokenStorage.getAccessToken().subscribe(t => { _token = t; });
		this.tokenStorage.getUserInfo().subscribe(user => {
			_userID = user.id;
		})
		let infoTokenCon = { "Token": _token, "UserID": _userID };
		connection.invoke("onDisconnectToken", JSON.stringify(infoTokenCon));
	}
	keyup(e) {
		if (e.keyCode === 13) {
			this.send();
		}
	}
	send() {
		var _token = '';
		var _userID = -1;
		this.tokenStorage.getAccessToken().subscribe(t => { _token = t; });
		this.tokenStorage.getUserInfo().subscribe(user => {
			_userID = user.id;
		})
		let infoTokenCon = { "Token": _token, "UserID": _userID, "Value": this.message };
		connection.send("sendMessage", JSON.stringify(infoTokenCon))
			.then(() => this.message = "");
	}
}

export class ThongBaoModel {
	IdRow: number;
	ThongBao: string;
	Link: string;
	Loai: number;
	IsRead: boolean = false;
	IsNew: boolean = true;
	Disabled: boolean = false;
	CreatedDate: string;
	UpdatedDate: string;
}
