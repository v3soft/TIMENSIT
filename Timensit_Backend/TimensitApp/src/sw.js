
!function () {
	var lasttime = "";
	var pingtimer;
	//self.addEventListener('notificationclose', function (e) {
	//    var notification = e.notification;
	//    var primaryKey = notification.data.primaryKey;

	//    console.log('Closed notification: ' + primaryKey);
	//});
	self.addEventListener('notificationclick', function (e) {
		var notification = e.notification;
		var action = e.action;

		if (action === 'close') {
			notification.close();
		} else {
			if (notification.data.link)
				clients.openWindow(notification.data.link);
			notification.close();
		}
	});
	self.addEventListener('install', async function (event) {
		event.waitUntil(self.skipWaiting());
	});
	self.addEventListener('activate', function (event) {
		refreshAllWindows();
		event.waitUntil(clients.claim());
	});
	self.addEventListener('fetch', function (event) {
		event.respondWith(
			caches.match(event.request)
				.then(function (response) {
					// Cache hit - return response
					if (response) {
						return response;
					}
					return fetch(event.request);
				})
		);
	});
	self.addEventListener('message', function (event) {
		var data = JSON.parse(event.data);
		self.Token = data.Token;
		self.ApiUrl = data.ApiUrl;
		self.applicationServerPublicKey = data.applicationServerPublicKey;
		subscribeUser();
		//pingtimer = setInterval(message, 5000);
	});
	self.addEventListener('push', function (event) {
		if (event && event.data) {
			var data = event.data.json();
			const title = data.title;
			const options = {
				body: data.message,
				icon: data.icon,
				data: data
				//badge: 'images/badge.png',
			};
			console.log(options);
			const notificationPromise = self.registration.showNotification(title, options);
			event.waitUntil(notificationPromise);
		}
	});
	function urlB64ToUint8Array(base64String) {
		const padding = '='.repeat((4 - base64String.length % 4) % 4);
		const base64 = (base64String + padding)
			.replace(/\-/g, '+')
			.replace(/_/g, '/');

		const rawData = atob(base64);
		const outputArray = new Uint8Array(rawData.length);

		for (let i = 0; i < rawData.length; ++i) {
			outputArray[i] = rawData.charCodeAt(i);
		}
		return outputArray;
	}

	async function subscribeUser() {
		await unSubscribe();
		if (self.applicationServerPublicKey) {
			self.registration.pushManager.subscribe({
				userVisibleOnly: true,
				applicationServerKey: urlB64ToUint8Array(self.applicationServerPublicKey)
			})
				.then(function (subscription) {
					console.log('User is subscribed.');
					var url = self.ApiUrl + "/fcm/UpdateFCM";
					fetch(url, {
						method: 'post',
						headers: {
							"Content-Type": "application/json",
							"Authorization": "Bearer " + self.Token
						},
						body: JSON.stringify(subscription)
					}).then(json)
						.then(function (res) {
							if (res && res.status === 1) {
								//
							}
						})
						.catch(function (error) {
							console.log('Request failed', error);
						});
				})
				.catch(function (error) {
					console.log("publicKey", self.applicationServerPublicKey);
					console.error('Failed to subscribe the user: ', error);
				});
		}
	}
	var refreshAllWindows = function () {
		var t = this
			, e = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : {
				includeRelay: !1
			};
		self.clients.matchAll({
			includeUncontrolled: !0,
			type: "window"
		}).then((function (n) {
			n.forEach((function (n) {
				(e.includeRelay || !e.includeRelay && 0 !== new URL(n.url).pathname.indexOf(t.__relayPath)) && n.navigate(n.url);
			}
			));
		}
		));
	};
	//var message = function () {
	//    if (self.ApiUrl && self.Token) {
	//        var url = self.ApiUrl + "/pushmess/GetNotificationAdmin?ngay=" + lasttime;
	//        fetch(url, {
	//            method: 'get',
	//            headers: {
	//                "Content-Type": "application/json",
	//                "Token": self.Token
	//            }
	//        }).then(json)
	//            .then(function (res) {
	//                //console.log('Request succeeded with JSON response', res);
	//                if (res && res.status === 1) {
	//                    lasttime = res.lasttime;
	//                    if (res.data) {
	//                        if (res.data.tinnhan && res.data.tinnhan.sotin > 0) {
	//                            self.registration.showNotification(res.data.tinnhan.noidung);
	//                        }
	//                        if (res.data.mail && res.data.mail.sotin > 0) {
	//                            self.registration.showNotification(res.data.mail.noidung);
	//                        }
	//                        if (res.data.lich && res.data.lich.sotin > 0) {
	//                            self.registration.showNotification(res.data.lich.noidung);
	//                        }
	//                        if (res.data.lichmoi && res.data.lichmoi.sotin > 0) {
	//                            self.registration.showNotification(res.data.lichmoi.noidung);
	//                        }
	//                    }
	//                }
	//            })
	//            .catch(function (error) {
	//                console.log('Request failed', error);
	//            });
	//    }
	//};
	function json(response) {
		return response.json();
	}
	async function unSubscribe() {
		if (self.registration === undefined)
			return;
		await self.registration.pushManager.getSubscription()
			.then(function (subscription) {
				if (subscription) {
					// TODO: Tell application server to delete subscription
					var url = self.ApiUrl + "/fcm/DeleteFCM";
					fetch(url, {
						method: 'post',
						headers: {
							"Content-Type": "application/json",
							"Authorization": "Bearer " + self.Token
						},
						body: JSON.stringify(subscription)
					}).then(json)
						.then(function (res) {
							if (res && res.status === 1) {
								//
							}
						})
						.catch(function (error) {
							console.log('Request failed', error);
						});
					return subscription.unsubscribe();
				}
			})
			.catch(function (error) {
				console.log('Error unsubscribing', error);
			});
	}
}();
