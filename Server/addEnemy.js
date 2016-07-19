var mongoose = require('mongoose');
var configDB = require('./config/database.js');
mongoose.connect(configDB.url);

var enemy = (name,reward,stamina,attack,defense,evade) => {
  var Enemy = require('./app/models/enemy'); //資料庫ENEMY的shema
    var newEnemy = new Enemy(
      { //這是目前的初始數值
        'name': name,
        'reward': reward,
        'stamina': stamina,
        'attack': attack,
        'defense': defense,
        'evade': evade,
        skill: {
          ID: 1,
          CD: 3,
          SkillDesc: "Drain 10 Hp from Enemy and boost 3 attack and burn 2 damage every turn.",
          params: {
            damage: 10,
            recover: 10,
            burn: 2,
            attIncrease: 3
          }
        }
      }
    );

    newEnemy.save((err) => {
      if (err)
        throw err;
      console.log(newEnemy.name + " created!");
      //mongoose.disconnect();
    });
}

enemy("Trash",200,60,10,2,20);
enemy("Hammer",2000,100,40,5,5);
enemy("Flyer",800,20,20,2,60);
