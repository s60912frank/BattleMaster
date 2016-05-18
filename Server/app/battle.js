var io = require('socket.io')({ //client跟server連接
	transports: ['websocket'],
});
var rooms = [];
var roomCount = 0;
var connections = [];

module.exports = {
  IOlisten: function(appListen){
    io.listen(appListen); //告訴socketio要監聽哪個port
  },

  addToWaitingQueue: function(user, sid){ //把想進戰鬥等待列&&有權限的client加入
    for(i = 0;i < connections.length;i++){ //這方法不是很好但目前我沒想到更好的方法@@
      if(connections[i].id == sid){ //找出已連接的clients中找出認證過的clent
				console.log("found!" + user.name);
        var socket = connections[i];
        socket["user"] = user; //加入user資訊
				connections.splice(i, 1); //從connections中移除這個socket
        return;
      }
    }
  }
}

io.on('connection', function(socket){ //有client連上時觸發
	connections.push(socket);
	//socket.emit("roomList", rooms);
	//socket.emit("hello", {});
	socket.emit("roomList", {
		"data": rooms
	});
	//console.log(JSON.stringify(rooms));
	console.log(socket.id + " CONNECTED!");
	socket.on('createRoom', function(data){
		if(socket['user']){
			console.log(rooms.length + "!!!");
			roomCount++; //感覺會出問題w
			var room = {
				"Id": roomCount,
				"name": data.name,
				"user1": socket.id
			}
			console.log(room.name + " CREATED!");
			rooms.push(room);
			socket.join("Room" + roomCount); //加入某房間
			//socket.emit("waitforsomeonejoin", {}); //temp
			io.sockets.emit('roomAdded', room); //通知全體有房間建立了
		}
	});

	socket.on('leaveRoom', function(data){
		if(socket['user']){
			for(i = 0;i < rooms.length;i++){
				if(rooms[i].user1 == socket.id){
					console.log(socket.user.name + "離開了房間!");
					io.sockets.emit('roomRemoved', rooms[i]); //通知全體有房間刪除了
					rooms.splice(i, 1);
					return;
				}
			}
		}
	});

	socket.on('joinRoom', function(data){
		if(socket['user']){
			for(i = 0;i < rooms.length;i++){
				if(rooms[i].Id == data.Id){
					var userNumber = io.sockets.adapter.rooms['Room' + data.Id].length;
					console.log(io.sockets.adapter.rooms['Room' + data.Id]);
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
		}
	});
	socket.on('disconnect', function(){ //client離線時觸發
		console.log(socket.id + "disconnected!");
    //removeItemById(waitingQueue, socket.id); //從這兩個array中移除離線的clent
    removeItemById(connections, socket.id);
		for(i = 0;i < rooms.length;i++){
			if(rooms[i].user1 == socket.id){
				io.sockets.emit('roomRemoved', rooms[i]); //通知全體有房間刪除了
				rooms.splice(i, 1);
				return;
			}
		}
  });
})

var battle = function(roomName){
  io.to(roomName).emit("battleStart", {}); //告訴房間所有人戰鬥開始了
	var room = io.sockets.adapter.rooms[roomName].sockets;
	console.log("LJKFDNKJL  " + room[0]);
	var clients = [];
	if (room) {
    for (var id in room) {
      clients.push(io.sockets.adapter.nsp.connected[id]);
    }
  }
  battlePhase(roomName, clients[0], clients[1]); //戰鬥囉
  battlePhase(roomName, clients[1], clients[0]);
}

var battlePhase = function (roomName, you, enemy) {
	you.on("battleSceneReady", function(data){ //等client準備好了就把敵人資料給他
		you.emit("enemyData", enemy.user.pet);
	});

  you.on("movement", function(data){ //當client選好動作時觸發
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

  you.on("result", function(data){ //clients計算自己受到的傷害並回傳
    enemy.emit("enemyMovementResult", data); //將自己受到的傷害傳給對方
		console.log("DAMAGE:" + data.damage);
  });

  you.on("dead", function(data){ //告訴敵人啊我死了
    enemy.emit("enemyDead", {});
  });

  you.on("disconnect", function () { //我離線了
      enemy.emit("enemyLeave", {}); //告訴對方我離線了
      //socket.enemy.leave(room.name); //把對方移出房間
      //delete room; //刪除房間
      //distroyRoom(room);
  });
}

var removeItemById = function(array, value){
  for(i = 0;i < array.length;i++){
    if(array[i].id == value){
      array.splice(i, 1);
      break;
    }
  }
}
