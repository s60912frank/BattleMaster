var User = require('./models/user'); //資料庫USER的shema
module.exports = function(app, passport, battle) {
    //跟AI打
    app.post('/battle', isLoggedIn, function(req, res) {
      //收到怪物類型，伺服器回傳怪物資訊，在client上打
      res.send("ok"); 
    });

    //跟玩家打
    app.post('/waitforbattle', isLoggedIn, function(req, res) {
      User.findOne({'token': req.user.token}, function(err, user) {
        //找到此USER後丟到等待列，由伺服器主持戰鬥
        battle.addToWaitingQueue(user, req.body.sid);
        res.send("OK");
      });
    });

    //登出
    app.get('/logout', function(req, res) {
        req.logout();
        res.send('你已經成功登出');
    });

    //登入
    app.post('/login', passport.authenticate('local-login') ,function(req, res){
        if(req.user){
          res.send(req.user); //登入成功
        }
        else{
          res.send("Account not found"); //你誰
        }
      });

    //註冊
    app.post('/signup', passport.authenticate('local-signup') ,function(req, res){
        if(req.user){
          res.send(req.user); //註冊成功
        }
        else{
          res.send("Account already exists"); //你已經註冊過了!
        }
      });
};

// route middleware to make sure a user is logged in
function isLoggedIn(req, res, next) {
  console.log(req.isAuthenticated());
    // if user is authenticated in the session, carry on
    if (req.isAuthenticated())
        return next();
    //如果他沒登入就告訴他沒登入
    res.status(401);
    res.send('You are not logged in!');
}
