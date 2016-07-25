var loki = require('lokijs'); //in memory db
var db = new loki('rooms.db');
var rooms = db.addCollection('rooms',{ unique: ["id"] });
var shortid = require('shortid');

module.exports = (io) => {
	this.ioInstance = io;
	io.on('connection', (socket) => {
		console.log(socket.request.user.game.name + " connected to socket!");
		console.log(rooms.find());
		socket.emit("roomList", { "data" : rooms.find() });
		socket.on('createRoom', (data) => {
			if(!rooms.findOne({ 'user1.id': socket.request.user._id })){
				var room = {
					"id": shortid.generate(),
					"name": data.name,
					"owner": {
						"id": socket.request.user._id,
						"name": socket.request.user.game.name
					}
				}
				rooms.insert(room);
				console.log(room.name + "(" + room.id + ") CREATED!");
				console.log("房間數:" + rooms.find().length);
				socket.join(room.id); //加入某房間
				socket["room"] = room.id;
				io.sockets.emit('roomAdded', room); //通知全體有房間建立了
			}
		});


		//還在改這裡
		socket.on('leaveRoom', (data) => {
			var room = rooms.findOne({ 'id': data.id });
			if(room){
				console.log(socket.request.user.game.name + "離開了房間!");
				socket.leave(socket.room);
				delete(socket.room);
				if(room.owner.id == socket.request.user._id){
					if(room.rival){
						room.owner = room.rival;
						delete(room.owner);
					}
				}
				else if(room.rival.id == socket.request.user._id){
					delete(room.rival);
				}

				if(!room.owner && !room.rival){
					io.sockets.emit('roomRemoved', room); //通知全體有房間刪除了
					rooms.remove(room);
				}
				else{
					io.sockets.emit('roomAvaliable', room); //通知全體有房間刪除了
					io.to(room.id).emit("roomChanged", { "data": room });
				}
			}
		});

		socket.on('joinRoom', (data) => {
			console.log(socket.request.user.game.name + " want to join room!");
			var roomToJoin = rooms.findOne({ "id": data.id });
			if(roomToJoin){
				if(roomToJoin.owner.id != socket.request.user._id){
					if(io.sockets.adapter.rooms[data.id].length && io.sockets.adapter.rooms[data.id].length == 1){
						socket.join(data.id);
						socket["room"] = data.id;
						io.sockets.emit('roomFull', roomToJoin); //通知全體有房間刪除了
						socket.emit('joinResult', {
							"status": "GOOD"
						});
						console.log("加入房間成功!");
						rooms.remove(roomToJoin);
						roomToJoin.rival = {
							"id": socket.request.user._id,
							"name": socket.request.user.game.name
						};
						battle(data.id);
					}
					else{
						socket.emit('joinResult', {
							"status": "BAD"
						});
						console.log("加入房間失敗!因為人數錯誤")
					}
				}
				else{
					socket.emit('joinResult', {
						"status": "BAD"
					});
					console.log("加入房間失敗!因為創房又加房");
				}
			}
			else{
				socket.emit('joinResult', {
					"status": "BAD"
				});
				console.log("加入房間失敗!因為找不到這個房間")
			}
		});

		socket.on('ready', () => {
			socket['ready'] = true;
			io.to(socket.room).emit("ready", { "who": socket.request.user.game.name });
			var room = io.sockets.adapter.rooms[socket.room].sockets;
			var clients = [];
			if (room) {
		    for (var id in room) {
		      clients.push(io.sockets.adapter.nsp.connected[id]);
		    }
		  }
			if(clients[0].ready && clients[1].ready){
				battle(clients);
			}
		});

		socket.on('unReady', () => {
			socket['ready'] = false;
			io.to(socket.room).emit("unReady", { "who": socket.request.user.game.name });
		});

		socket.on('disconnect', () => { //client離線時觸發
			console.log(socket.request.user.game.name + " disconnected!");
			var roomToRemove = rooms.findOne({ "user1.id": socket.request.user._id });
			io.sockets.emit('roomRemoved', roomToRemove); //通知全體有房間刪除了
			if(roomToRemove)
				rooms.remove(roomToRemove);
	  });
	});


	//BATTLE!!!!!!!!!!///////////////////////////////////////////////////
	var battle = (clients) => {
		if(clients.length == 2){
			clients[0].ready = false;
			clients[1].ready = false;
			io.to(roomName).emit("battleStart", {}); //告訴房間所有人戰鬥開始了
			console.log("BATTLE START!!!");
		  battlePhase(clients[0], clients[1]); //戰鬥囉
		  battlePhase(clients[1], clients[0]);
		}
		else{
			console.log("準備進入戰鬥可是人數錯了嗚嗚");
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
	      io.to(roomName).emit("attackStart", {}); //告訴雙方戰鬥開始
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
