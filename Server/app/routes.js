var User = require('./models/user'); //資料庫USER的shema
var Enemy = require('./models/enemy'); //資料庫ENEMY的shema
module.exports = (app, passport) => {
  app.get('/isAlive', (req, res) => {
    res.send("Yes!");
    res.end();
  });
    //跟AI打
    app.post('/battle', isLoggedIn, (req, res) => {
      //收到怪物類型，伺服器回傳怪物資訊，在client上打
      Enemy.findOne({'name': req.body.enemyName }, (err, enemy) => res.send(enemy));
    });

    //可能還要再改良
    app.post('/battleAIResult', isLoggedIn, (req, res) => {
      //收到怪物類型，伺服器回傳怪物資訊，在client上打
      Enemy.findOne({'name': req.body.enemyName }, (err, enemy) => {
        if(err) throw err;
        var battleResult = {};
        battleResult.result = req.body.result;
        if(req.body.result == 'win')
          battleResult["mileageIncrease"] = enemy.reward;
        else if(req.body.result == 'lose')
          battleResult["mileageIncrease"] = Math.round(enemy.reward / 10);
        else if(req.body.result == 'even')
          battleResult["mileageIncrease"] = Math.round(enemy.reward / 2);
        req.user.game.mileage = req.user.game.mileage + battleResult["mileageIncrease"];
        battleResult["mileage"] = req.user.game.mileage;
        req.user.save((err) =>{
          if(err) throw err;
          console.log(req.user.game.name + "BattleAI SAVED!");
        });
        res.send(battleResult);
      });
    });

    app.post('/traningResult', isLoggedIn, (req, res) => {
      if(req.user.game.mileage >= 100){
        console.log(req.body);
        req.user.game.mileage -= 100;
        req.user.game.pet.stamina += parseInt(req.body.staminaIncrease);
        req.user.game.pet.attack += parseInt(req.body.attackIncrease);
        req.user.game.pet.evade += parseInt(req.body.evadeIncrease);
        req.user.game.pet.defense += parseInt(req.body.defenseIncrease);
        req.user.save((err) => {
          if(err) throw err;
          console.log(req.user.game.name + " traning saved!");
        });
        res.send(req.user.game);
      }
      else{
        console.log("Invalid access");
        res.status(401);
      }
    });

    app.get('/allEnemyData',isLoggedIn ,(req, res) => {
      Enemy.find({}, { "_id": 0 }, (err, data) => {
        res.send(data);
      });
      console.log(req.user.game.name + "取得了怪物資訊!");
      /*Enemy.SomeValue.find({}).select({ "name": 1, "_id": 0}).query.exec((err, data) => {
        res.send(data);
      });*/
    });

    app.get('/isLoggedIn', isLoggedIn, (req, res) => {
      console.log(req.user.game.name + "LOGGED IN!");
      res.send(req.user.game);
      res.end();
    });

    //登出
    app.get('/logout', (req, res) => {
        req.logout();
        res.send('你已經成功登出');
    });

    //local登入
    app.post('/login', passport.authenticate('local-login') ,(req, res) => {
        if(req.user){
          res.send(req.user.game); //登入成功
        }
        else{
          res.send("Account not found"); //你誰
        }
      });

    //local註冊
    app.post('/signup', passport.authenticate('local-signup') ,(req, res) => {
        if(req.user){
          res.send(req.user.game); //註冊成功
        }
        else{
          res.send("Account already exists"); //你已經註冊過了!
        }
      });

    //facebook註冊
    app.post('/signupFacebook', passport.authenticate('facebook-signup') ,(req, res) => {
      if(req.user){
        res.send(req.user.game); //註冊成功
      }
    });

    //facebook登入
    app.post('/loginFacebook', passport.authenticate('facebook-login') ,(req, res) => {
      if(req.user){
        res.send(req.user.game); //登入成功
      }
    });
};

// route middleware to make sure a user is logged in
var isLoggedIn = (req, res, next) => {
  console.log(req.isAuthenticated());
    // if user is authenticated in the session, carry on
    if (req.isAuthenticated())
        return next();
    //如果他沒登入就告訴他沒登入
    res.status(401);
    res.send('You are not logged in!');
}
