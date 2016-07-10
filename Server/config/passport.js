// load all the things we need
//var LocalStrategy = require('passport-local').Strategy;
var CustomStrategy = require('passport-custom').Strategy;

// load up the user model
var User = require('../app/models/user');

//跟FB驗證需要的
var request = require('request');
var auth = require('./auth.js')

// expose this function to our app using module.exports
module.exports = (passport) => {

  // =========================================================================
  // passport session setup ==================================================
  // =========================================================================
  // required for persistent login sessions
  // passport needs ability to serialize and unserialize users out of session

  // used to serialize the user for the session
  passport.serializeUser((user, done) => done(null, user._id));

  // used to deserialize the user
  passport.deserializeUser((id, done) => {
    User.findOne({'_id': id}, (err, user) => done(err, user));
  });

  //本地資料庫註冊
  passport.use('local-signup', new CustomStrategy((req, done) => {
    //搜尋這個使用者是否在資料庫內
    User.findOne({ 'token': req.body.token }, (err, user) => {
      // if there are any errors, return the error
      if (err) return done(err);
      if (user) return done(null, false, "You already have an account!");
      else {
        var newUser = new User();
        newUser.name = req.body.name;
        newUser.token = req.body.token;
        newUser.provider = "local";
        newUser.pet = petDefault(req.body.name);
        // save the user
        newUser.save((err) => {
          if (err) throw err;
          console.log(newUser.name + "created!");
          return done(null, newUser);
        });
      }
    });
  }));

  //facebook註冊
  passport.use('facebook-signup', new CustomStrategy((req, done) => {
    //需要三項資料:暱稱,fbid,token
    User.findOne({ fbid: req.body.fbid }, (err, user) => {
      if(!user){
        request('https://graph.facebook.com/me?access_token=' + req.body.token, (error, response, body) => {
          var data = JSON.parse(body);
          if (!error && response.statusCode == 200 && data.id == req.body.fbid) {
            var newUser = new User();
            newUser.name = req.body.name;
            newUser.fbid = req.body.fbid;
            newUser.token = req.body.token;
            newUser.provider = "facebook";
            newUser.pet = petDefault(req.body.name);
            // save the user
            newUser.save((err) => {
              if (err) throw err;
              console.log(newUser.name + "created!(Facebook)");
              return done(null, newUser);
            });
          }
          else{
            console.log(fbid + "FB註冊失敗!");
            return done(null, false, "Signup failed!");
          }
        });
      }
      else{
        return done(null, false, "Account already exists!");
      }
    });
  }
  ));

  //facebook登入
  passport.use('facebook-login', new CustomStrategy((req, done) => {
      //需要兩項資料:fbid,token
      User.findOne({ fbid: req.body.fbid }, (err, user) => {
        if(user){
          request('https://graph.facebook.com/me?access_token=' + req.body.token, (error, response, body) => {
            var data = JSON.parse(body);
            if (!error && response.statusCode == 200 && data.id == req.body.fbid) {
              user.token = req.body.token;
              // save the user
              user.save((err) => {
                if (err) throw err;
                console.log(user.name + "Logged in!(Facebook)");
                return done(null, user);
              });
            }
            else{
              console.log(fbid + "FB註冊失敗!");
              return done(null, false, "Login failed!");
            }
          });
        }
        else{
          return done(null, false, "User not found!");
        }
      });
    }
  ));

  //本地資料庫登入
  passport.use('local-login', new CustomStrategy((req, done) => {
      // asynchronous
      process.nextTick(() => {
        //同上搜尋資料庫
        User.findOne({'token': req.body.token }, (err, user) => {
          // if there are any errors, return the error
          if (err) return done(err);
          // check to see if theres already a user with that email
          if (user) {
            console.log(user.name + " logged in!");
            return done(null, user);
          } else {
            return done(null, false, "Account not found");
          }
        });
      });
    }));
};

var petDefault = (name) => {
  return { //這是目前的初始數值
    "name": name,
    stamina: 50,
    attack: 12,
    defense: 2,
    evade: 30,
    skill: {
      ID: 1,
      CD: 3,
      SkillDesc: "Drain 15 Hp from Enemy and boost 5 attack",
      params: {
        damage: 15,
        recover: 15,
        burn: 3,
        attIncrease: 5
      }
    }
  };
}
