var loki = require('lokijs'); //in memory db
var db = new loki('rooms.db');
var rooms = db.addCollection('rooms',{ unique: ["id"] });
var shortid = require('shortid');
var ERROR = {
	DUPLICATE_ROOM: "重複開房!",
	JOIN_MISSING_ROOM: "加入房間失敗!因為找不到這個房間!",
	CREATE_AND_JOIN_ROOM: "加入房間失敗!因為創房又加房",
	USER_COUNT_INVAILD: "人數錯誤!"
}

module.exports = (io) => {
	io.on('connection', (socket) => {
		console.log(socket.request.user.game.name + " connected to socket!");
		console.log(rooms.find());
		socket.join('lobby');
		socket.emit("roomList", { "data" : rooms.find() });

		socket.on('createRoom', (data) => {
			if(!rooms.findOne({ 'owner.id': socket.request.user._id })){
				var room = {
					"id": shortid.generate(),
					"name": data.name,
					"isFull": false,
					"owner": {
						"id": socket.request.user._id,
						"name": socket.request.user.game.name
					}
				}
				rooms.insert(room);
				console.log(room.name + "(" + room.id + ") CREATED!");
				console.log("房間數:" + rooms.find().length);
				socket.leave('lobby');
				socket.join(room.id); //加入某房間
				socket.emit('joinResult', {
					"success": true,
					"room": room
				});
				socket["room"] = room.id;
				io.to('lobby').emit('roomAdded', room); //通知全體有房間建立了
			}
			else{
				socket.emit('joinResult', {
					"success": false,
					"message": ERROR.DUPLICATE_ROOM
				});
				console.log(ERROR.DUPLICATE_ROOM);
			}
		});


		socket.on('leaveRoom', (data) => {
			socketLeaveRoom(socket, io);
		});

		socket.on('joinRoom', (data) => {
			console.log(socket.request.user.game.name + " want to join room!");
			var roomToJoin = rooms.findOne({ "id": data.id });
			if(roomToJoin){
				if(roomToJoin.owner.id != socket.request.user._id){
					if(io.sockets.adapter.rooms[roomToJoin.id].length && io.sockets.adapter.rooms[roomToJoin.id].length == 1){
						socket.leave('lobby');
						socket.join(roomToJoin.id);
						socket["room"] = roomToJoin.id;
						console.log("加入房間成功!");
						//rooms.remove(roomToJoin);
						roomToJoin["rival"] = {
							"id": socket.request.user._id,
							"name": socket.request.user.game.name
						};

						//臭
						var room = io.sockets.adapter.rooms[socket.room].sockets;
						var clients = [];
						if (room) {
		    				for (var id in room) {
		      					clients.push(io.sockets.adapter.nsp.connected[id]);
		    				}
							if(clients[0].ready){
								roomToJoin["enemyReady"] = true;
							}
							else{
								roomToJoin["enemyReady"] = false;
							}
		   				}

						socket.emit('joinResult', {
							"success": true,
							"room": roomToJoin
						});

						io.to(roomToJoin.id).emit("roomChanged", roomToJoin);
						io.to('lobby').emit('roomFull', { "id": roomToJoin.id }); //通知全體有房間滿了
						roomToJoin.isFull = true;
						//battle(data.id);
					}
					else{
						socket.emit('joinResult', {
							"success": false,
							"message": ERROR.USER_COUNT_INVAILD
						});
						console.log(ERROR.USER_COUNT_INVAILD)
					}
				}
				else{
					socket.emit('joinResult', {
						"success": false,
						"message": ERROR.CREATE_AND_JOIN_ROOM
					});
					console.log(ERROR.CREATE_AND_JOIN_ROOM);
				}
			}
			else{
				socket.emit('joinResult', {
					"success": false,
					"message": ERROR.JOIN_MISSING_ROOM
				});
				console.log(ERROR.JOIN_MISSING_ROOM);
			}
		});

		socket.on('ready', () => {
			socket['ready'] = true;
			io.to(socket.room).emit("ready", { "id": socket.request.user._id }); //這裡可能還要改
			var room = io.sockets.adapter.rooms[socket.room].sockets;
			var clients = [];
			if (room) {
		    	for (var id in room) {
		      		clients.push(io.sockets.adapter.nsp.connected[id]);
		    	}
		   	}
			else{
				//error handling
			}
			if(clients.length == 2){
				if(clients[0].ready && clients[1].ready){
					var room = rooms.findOne({ "id": socket.room });
					io.to("lobby").emit('roomRemoved', { "id": room.id }); //通知全體有房間刪除了
					rooms.remove(room);
					console.log("房間" + room.id + "刪除了!");
					battle(clients);
				}
			}
		});

		/*socket.on('unReady', () => {
			socket['ready'] = false;
			io.to(socket.room).emit("unReady", { "id": socket.request.user._id });
		});*/

		socket.on('disconnect', () => { //client離線時觸發
			console.log(socket.request.user.game.name + " disconnected!");
			if(socket.room){
				var roomToRemove = rooms.findOne({ "id": socket.room });
				//去觸發leave room等等改
				socketLeaveRoom(socket, io);
			}
	  });
	});


	//BATTLE!!!!!!!!!!///////////////////////////////////////////////////
	var battle = (clients) => {
		if(clients.length == 2){
			clients[0].ready = false;
			clients[1].ready = false;
			io.to(clients[0].room).emit("battleStart", {}); //告訴房間所有人戰鬥開始了
			console.log("BATTLE START!!!");
		  	battlePhase(clients[0], clients[1]); //戰鬥囉
		  	battlePhase(clients[1], clients[0]);
		}
		else{
			console.log(ERROR.USER_COUNT_INVAILD);
		}
	}

	var battlePhase = (you, enemy) => {
		you.removeAllListeners('disconnect');
		//等client準備好了就把敵人資料給他
		you.on("battleSceneReady", (data) => you.emit("enemyData", enemy.request.user.game.pet));

	  you.on("movement", (data) => { //當client選好動作時觸發
	    enemy.emit("enemyMovement", data); //把動作傳給敵人(使用者在這時看不到)
			console.log(data);
	    you["ready"] = true; //把它設為準備好了

	    if(you.ready && enemy.ready){
	      you["ready"] = false; //設回未準備
	      enemy["ready"] = false;
				console.log("READY!!");
	      io.to(you.room).emit("attackStart", {}); //告訴雙方戰鬥開始
	    }
	  });

	  you.on("result", (data) => { //clients計算自己受到的傷害並回傳
	    enemy.emit("enemyMovementResult", data); //將自己受到的傷害傳給對方
			console.log("DAMAGE:" + data.enemyDamageTake);
	  });

	  you.on("battleEnd", (data) => { //戰鬥結束!
			console.log("BATTLE END!" + you.request.user.game.name + " " + data.result + "!");
			you.end = data.result;
			if(you.end && enemy.end){
				var youResult, enemyResult;
				if(you.end == enemy.end){
					//平手
					youResult = MileageProcess(you.request.user, 'even');
					enemyResult = MileageProcess(enemy.request.user, 'even');
				}
				else if(you.end == 'win' && enemy.end == 'lose'){
					//"you"贏
					youResult = MileageProcess(you.request.user, 'win');
					enemyResult = MileageProcess(enemy.request.user, 'lose');
				}
				else if(you.end == 'lose' && enemy.end == 'win'){
					//"enemy"贏
					youResult = MileageProcess(you.request.user, 'lose');
					enemyResult = MileageProcess(enemy.request.user, 'win');
				}
				else{
					throw "Unknown battle result!!!!!";
				}
				you.emit("battleResult2", youResult);
				enemy.emit("battleResult2", enemyResult);
				console.log(you.request.user.game.name + " " + you.end);
				console.log(enemy.request.user.game.name + " " + enemy.end);
			}
	  });

	  you.on("disconnect", () => { //我離線了
			console.log(you.request.user.game.name + " disconnected!");
			//如果我戰鬥途中退出....
			if(!enemy.end && !you.end){
				console.log("wHEHEHBEHEBEKE:" + enemy.connected);
				enemy.emit("enemyLeave", {}); //告訴對方我離線了
				console.log("wHEHEHBEHEBEKE:" + enemy.connected);
				MileageProcess(you.request.user, 'lose');
				enemyResult = MileageProcess(enemy.request.user, 'win');
				enemy['end'] = 'win';
				enemy.emit("battleResult2", enemyResult);
				delete(you);
				enemy.disconnect();
			}
	  });
	}
};

//會把戰鬥結束的里程存到資料庫，並且return戰鬥結果
var MileageProcess = (user, result) => {
	console.log("HELLO! " + result);
	var battleResult = {};
	battleResult["result"] = result;
	if(result == 'win')
		battleResult["mileageIncrease"] = 500;
	else if(result == 'lose')
		battleResult["mileageIncrease"] = 50;
	else if(result == 'even')
		battleResult["mileageIncrease"] = 250;

	user.game.mileage = user.game.mileage + battleResult.mileageIncrease;
	battleResult["mileage"] = user.game.mileage;
	user.save((err) => {
		if(err) console.log(err);
		console.log(user.game.name + " now have " + user.game.mileage + "(" + battleResult.mileageIncrease + ")mileage!");
	});
	return battleResult;
}

var socketLeaveRoom = (socket, io) => {
	var room = rooms.findOne({ 'id': socket.room });
	if(room){
		console.log(socket.request.user.game.name + "離開了房間!");
		socket.leave(socket.room);
		delete(socket.room);
		if(room.owner.id == socket.request.user._id){
			if(room.rival){
				room.owner = room.rival;
				delete(room.rival);
				console.log(room.owner.name + "成為了新房主!");
			}
			else{
				delete(room.owner);
			}
		}
		else if(room.rival.id == socket.request.user._id){
			delete(room.rival);
		}

		if(!room.owner && !room.rival){
			io.to('lobby').emit('roomRemoved', { "id": room.id }); //通知全體有房間刪除了
			rooms.remove(room);
			console.log("房間" + room.id + "刪除了!");
		}
		else{
			io.to('lobby').emit('roomAvaliable', { "id": room.id }); //通知全體有房間空出來了
			room.isFull = false;
			io.to(room.id).emit("roomChanged", room);
			console.log("房間" + room.id + "有空位了!");
		}
	}
}
