var io = require('socket.io')({
	transports: ['websocket'],
});
var connections = []
var waitingQueue = [];
var rooms = [];
var roomCount = 0;

module.exports = {
  IOlisten: function(appListen){
    io.listen(appListen);
  },

  addToWaitingQueue: function(user, sid){
    for(i = 0;i < connections.length;i++){
      if(connections[i].id == sid){
        var socket = connections[i];
        connections.splice(i, 1);
        console.log(user.name + "/" + sid + "加入了戰鬥!");
        socket["user"] = user;
        waitingQueue.push(socket);
        if(waitingQueue.length < 2){
    			socket.emit("waiting", {});
    		}
    		else{
    			createRoom();
    		}
        return;
      }
    }
  }
}

io.on('connection', function(socket){
	console.log("A client connected!");
  connections.push(socket);
	socket.on('disconnect', function(){
		console.log("A client disconnected!");
    removeItemById(waitingQueue, socket.id);
    removeItemById(connections, socket.id);
  });
})

var createRoom = function(){
  console.log("CREATE ROOM!");
	roomCount++;
	var room = {
		name: "Room" + roomCount,
		userOne: undefined,
		userTwo: undefined
	};
	room.userOne = waitingQueue.shift().join(room.name);
	room.userTwo = waitingQueue.shift().join(room.name);
  room.userOne["enemy"] = room.userTwo;
  room.userTwo["enemy"] = room.userOne;
	rooms.push(room);
	battle(room);
}

var battle = function(room){
  io.to(room.name).emit("battleStart", {});
  battlePhase(room.userOne, room);
  battlePhase(room.userTwo, room);
}

var battlePhase = function (socket, room) {
	socket.on("battleSceneReady", function(data){
		socket.emit("enemyData", socket.enemy.user.pet);
	});
	console.log("Enemy Data Sent:" + socket.user.pet);
  socket.on("movement", function(data){
    //do sth
    socket.enemy.emit("enemyMovement", data);
		console.log(data.movement);
    socket["ready"] = true;

    if(room.userOne.ready && room.userTwo.ready){
      room.userOne["ready"] = false;
      room.userTwo["ready"] = false;
			console.log("READY!!");
      io.to(room.name).emit("attackStart", {});
    }
  });

  socket.on("result", function(data){
    socket.enemy.emit("enemyMovementResult", data);
		//socket.enemy.emit("enemyMovementResult", {});
		console.log("DAMAGE:" + data.damage);
  });

  socket.on("dead", function(data){
    socket.enemy.emit("enemyDead", {});
  });

  socket.on("disconnect", function () {
      socket.enemy.emit("enemyLeave", {});
      distroyRoom(room);
  });
}

var distroyRoom = function(room){
  for(i = 0;i < rooms.length;i++){
    if(rooms[i].name == room.name){
      room.userOne.leave(room.name);
      room.userTwo.leave(room.name);
      console.log(room.name + " deleted!");
      rooms.splice(i, 1);
      delete room;
      break;
    }
  }
}

var removeItemById = function(array, value){
  for(i = 0;i < array.length;i++){
    if(array[i].id == value){
      array.splice(i, 1);
      break;
    }
  }
}
