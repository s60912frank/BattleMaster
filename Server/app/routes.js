var User = require('./models/user'); //資料庫USER的shema
var Enemy = require('./models/enemy'); //資料庫ENEMY的shema
var petDefaults = require('./models/petDefault'); //三種pets的初始值
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
      console.log(battleResult);
      req.user.save((err) =>{
        if(err){
          console.log(err);
        }
        console.log(req.user.game.name + "BattleAI SAVED!");
      });
      res.send(battleResult);
    });
  });

    var consume100Mileage = (req, res) => {
      if(req.user.game.mileage >= 100){
        req.user.game.mileage -= 100;
        req.user.save((err) => {
          if(err) throw err;
          console.log(req.user.game.name + " mileage - 100!");
        });
        res.send(req.user.game);
      }
      else{
        res.status(401);
      }
    }

    app.get('/battleWithAI', isLoggedIn, consume100Mileage);

    app.get('/enterTraning', isLoggedIn, consume100Mileage);

    app.post('/mileageGain', isLoggedIn, (req, res) => {
      req.user.game.mileage += parseInt(req.body.mileageGain);
      req.user.save((err) => {
        if(err) throw err;
        console.log(req.user.game.name + "取得了" + req.body.mileageGain + "里程!");
      });
      res.send(req.user.game);
    });

    app.post('/traningResult', isLoggedIn, (req, res) => {
      req.user.game.pet.stamina += parseInt(req.body.staminaIncrease);
      req.user.game.pet.attack += parseInt(req.body.attackIncrease);
      req.user.game.pet.evade += parseInt(req.body.evadeIncrease);
      req.user.game.pet.defense += parseInt(req.body.defenseIncrease);
      req.user.save((err) => {
        if(err) throw err;
        console.log(req.user.game.name + " traning saved!");
      });
      res.send(req.user.game);
    });

    app.get('/allEnemyData',isLoggedIn ,(req, res) => {
      Enemy.find({}, { "_id": 0 }, (err, data) => {
        res.send(data);
      });
      console.log(req.user.game.name + "取得了怪物資訊!");
    });

    app.get('/isLoggedIn', isLoggedIn, (req, res) => {
      console.log(req.user.game.name + "LOGGED IN!");
      res.send({ ok: true, data: req.user.game });
      res.end();
    });

    //登出
    app.get('/logout', (req, res) => {
        req.logout();
        res.send('你已經成功登出');
    });
  
  //local登入
  app.post('/login', (req, res, next) => {
    passport.authenticate('local-login', (err, user, info) => {
      if(err){
        console.error(err);
        return res.send({ ok: false, message: info });
      }
      if(!user){
        console.log("Someone login failed");
        return res.send({ ok: false, message: info });
      }
      else{
        req.login(user, (loginErr) => {
          //登入user
          if (loginErr) return res.send({ ok: false, message: "未知錯誤!" });
          return res.send({ ok: true, data: req.user.game });
        });
      }
    })(req, res, next);
    //res.end();
  });
  
  //local註冊
  app.post('/signup', (req, res,next) => {
    passport.authenticate('local-signup', (err, user, info) => {
      if(err){
        console.error(err);
        return res.send({ ok: false, message: info });
      }
      if(!user){
        console.log("Someone login failed");
        return res.send({ ok: false, message: info });
      }
      else{
        req.login(user, (loginErr) => {
          //登入user
          if (loginErr) return res.send({ ok: false, message: "未知錯誤!" });
          return res.send({ ok: true, data: req.user.game });
        });
      }
    })(req, res ,next);
    //res.end();
  });

  //facebook註冊
  app.post('/signupFacebook', (req, res, next) => {
    passport.authenticate('facebook-signup', (err, user, info) => {
      if(err){
        console.error(err);
        return res.send({ ok: false, message: info });
      }
      if(!user){
        console.log("Someone login failed");
        return res.send({ ok: false, message: info });
      }
      else{
        req.login(user, (loginErr) => {
          //登入user
          if (loginErr) return res.send({ ok: false, message: "未知錯誤!" });
          return res.send({ ok: true, data: req.user.game });
        });
      }
    })(req, res ,next);
    //res.end();
  });
  
  //facebook登入
  app.post('/loginFacebook', (req, res,next) => {
    passport.authenticate('facebook-login', (err, user, info) => {
      if(err){
        console.error(err);
        return res.send({ ok: false, message: info });
      }
      if(!user){
        console.log("Someone login failed");
        return res.send({ ok: false, message: info });
      }
      else{
        req.login(user, (loginErr) => {
          //登入user
          if (loginErr) return res.send({ ok: false, message: "未知錯誤!" });
          return res.send({ ok: true, data: req.user.game });
        });
      }
    })(req, res ,next);
    //res.end();
  });

    //註冊一開始不會有寵物資訊 post這個決定
    app.post('/setPartner', isLoggedIn, (req, res) => {
      if(!req.user.game.pet.name){
        req.user.game["pet"] = petDefaults(req.body.type);
        req.user.save((err) =>{
          if(err){
            console.log(err);
            res.status(500);
            res.send(err);
          }
          else{
            console.log(req.user.game.name + "選好夥伴了!");
            res.send(req.user.game);
            res.end();
          }
        });
      }
      else{
        res.status(401);
        res.send('You already set partner!');
        console.log(req.user.game.name + "重複選夥伴欸!");
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
    //res.status(401);
    res.send({ ok: false, message: "You are not logged in!" });
    res.end();
}
