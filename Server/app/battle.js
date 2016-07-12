var rooms = [];
var roomCount = 0;

module.exports = (io) => {
	this.ioInstance = io;
	io.on('connection', (socket) => {
		console.log(socket.request.user.game.name + " connected to socket!");
		socket.emit("roomList", { "data": rooms });
		socket.on('createRoom', (data) => {
			roomCount++; //感覺會出問題w
			var room = {
				"Id": roomCount,
				"name": data.name,
				"user1": socket.id
			}
			console.log(room.name + " CREATED!");
			rooms.push(room);
			console.log("房間數:" + rooms.length);
			socket.join("Room" + room.Id); //加入某房間
			io.sockets.emit('roomAdded', room); //通知全體有房間建立了
		});

		socket.on('leaveRoom', (data) => {
			for(i = 0;i < rooms.length;i++){
				if(rooms[i].user1 == socket.id){
					console.log(socket.request.user.game.name + "離開了房間!");
					io.sockets.emit('roomRemoved', rooms[i]); //通知全體有房間刪除了
					rooms.splice(i, 1);
					return;
				}
			}
		});

		socket.on('joinRoom', (data) => {
			for(i = 0;i < rooms.length;i++){
				if(rooms[i].Id == data.Id){
					var userNumber = io.sockets.adapter.rooms['Room' + data.Id].length;
					console.log("COUNT:" + userNumber);
					if(userNumber == 1){
						socket.join("Room" + data.Id);
						io.sockets.emit('roomRemoved', rooms[i]); //通知全體有房間刪除了
						rooms.splice(i, 1);
						socket.emit('joinResult', {
							"status": "GOOD"
						});
						console.log("加入房間成功!")
						battle('Room' + data.Id);
						return;
					}
					else{
						socket.emit('joinResult', {
							"status": "BAD"
						});
						console.log("加入房間失敗!")
					}
					return;
				}
			}
		});

		socket.on('disconnect', () => { //client離線時觸發
			console.log(socket.id + "disconnected!");
			for(i = 0;i < rooms.length;i++){
				if(rooms[i].user1 == socket.id){
					io.sockets.emit('roomRemoved', rooms[i]); //通知全體有房間刪除了
					rooms.splice(i, 1);
					return;
				}
			}
	  });
	});


	//BATTLE!!!!!!!!!!///////////////////////////////////////////////////
	var battle = (roomName) => {
	  io.to(roomName).emit("battleStart", {}); //告訴房間所有人戰鬥開始了
		var room = io.sockets.adapter.rooms[roomName].sockets;
		var clients = [];
		if (room) {
	    for (var id in room) {
	      clients.push(io.sockets.adapter.nsp.connected[id]);
	    }
	  }
	  battlePhase(roomName, clients[0], clients[1]); //戰鬥囉
	  battlePhase(roomName, clients[1], clients[0]);
	}

	var battlePhase = (roomName, you, enemy) => {
		//等client準備好了就把敵人資料給他
		you.on("battleSceneReady", (data) => you.emit("enemyData", enemy.request.user.game.pet));

	  you.on("movement", (data) => { //當client選好動作時觸發
	    enemy.emit("enemyMovement", data); //把動作傳給敵人(使用者在這時看不到)
			console.log(data.movement);
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
			console.log("DAMAGE:" + data.damage);
	  });

	  you.on("battleEnd", (data) => { //戰鬥結束!
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
	      enemy.emit("enemyLeave", {}); //告訴對方我離線了
				delete(you);
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
		console.log(user.game.name + " now have " + user.game.mileage + "mileage!");
	});
	return battleResult;
}
