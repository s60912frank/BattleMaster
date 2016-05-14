var io = require('socket.io')({ //client跟server連接
	transports: ['websocket'],
});
var connections = []; //目前連上的clents存在這
var waitingQueue = []; //等待戰鬥的clients
var roomCount = 0;

module.exports = {
  IOlisten: function(appListen){
    io.listen(appListen); //告訴socketio要監聽哪個port
  },

  addToWaitingQueue: function(user, sid){ //把想進戰鬥等待列&&有權限的client加入
    for(i = 0;i < connections.length;i++){ //這方法不是很好但目前我沒想到更好的方法@@
      if(connections[i].id == sid){ //找出已連接的clients中找出認證過的clent
        var socket = connections[i];
        connections.splice(i, 1); //從connections中移除這個socket
        console.log(user.name + "/" + sid + "加入了戰鬥!");
        socket["user"] = user; //加入user資訊
        waitingQueue.push(socket); //加入waitingQueue
        if(waitingQueue.length < 2){ //如果等待列不足兩人就等
    			socket.emit("waiting", {});
    		}
    		else{ //超過兩人就可以開房間啦
    			createRoom();
    		}
        return;
      }
    }
  }
}

io.on('connection', function(socket){ //有client連上時觸發
	console.log("A client connected!");
  connections.push(socket); //加入connections array
	socket.on('disconnect', function(){ //client離線時觸發
		console.log("A client disconnected!");
    removeItemById(waitingQueue, socket.id); //從這兩個array中移除離線的clent
    removeItemById(connections, socket.id);
  });
})

var createRoom = function(){
  console.log("CREATE ROOM!");
	roomCount++;
	var room = {
		name: "Room" + roomCount, //room name隨便取的
		userOne: undefined,
		userTwo: undefined
	};
	room.userOne = waitingQueue.shift().join(room.name); //從等待列頭移出兩個socket開房間
	room.userTwo = waitingQueue.shift().join(room.name);
  room.userOne["enemy"] = room.userTwo; //設定對方為敵人
  room.userTwo["enemy"] = room.userOne;
	battle(room);
}

var battle = function(room){
  io.to(room.name).emit("battleStart", {}); //告訴房間所有人戰鬥開始了
  battlePhase(room.userOne, room); //戰鬥囉
  battlePhase(room.userTwo, room);
}

var battlePhase = function (socket, room) {
	socket.on("battleSceneReady", function(data){ //等client準備好了就把敵人資料給他
		socket.emit("enemyData", socket.enemy.user.pet);
	});

  socket.on("movement", function(data){ //當client選好動作時觸發
    socket.enemy.emit("enemyMovement", data); //把動作傳給敵人(使用者在這時看不到)
		console.log(data.movement);
    socket["ready"] = true; //把它設為準備好了

    if(room.userOne.ready && room.userTwo.ready){
      room.userOne["ready"] = false; //設回未準備
      room.userTwo["ready"] = false;
			console.log("READY!!");
      io.to(room.name).emit("attackStart", {}); //告訴雙方戰鬥開始
    }
  });

  socket.on("result", function(data){ //clients計算自己受到的傷害並回傳
    socket.enemy.emit("enemyMovementResult", data); //將自己受到的傷害傳給對方
		console.log("DAMAGE:" + data.damage);
  });

  socket.on("dead", function(data){ //告訴敵人啊我死了
    socket.enemy.emit("enemyDead", {});
  });

  socket.on("disconnect", function () { //我離線了
      socket.enemy.emit("enemyLeave", {}); //告訴對方我離線了
      socket.enemy.leave(room.name); //把對方移出房間
      delete room; //刪除房間
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
