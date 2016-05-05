// load all the things we need
var LocalStrategy = require('passport-local').Strategy;

// load up the user model
var User = require('../app/models/user');

// expose this function to our app using module.exports
module.exports = function(passport) {

  // =========================================================================
  // passport session setup ==================================================
  // =========================================================================
  // required for persistent login sessions
  // passport needs ability to serialize and unserialize users out of session

  // used to serialize the user for the session
  passport.serializeUser(function(user, done) {
    done(null, user.token);
  });

  // used to deserialize the user
  passport.deserializeUser(function(token, done) {
    User.findOne({'token': token}, function(err, user) {
      done(err, user);
    });
  });

  //本地資料庫註冊
  passport.use('local-signup', new LocalStrategy({
    usernameField: 'name', //使用者輸入的暱稱
    passwordField: 'token' //這個使用unity提供的deviceID
  },function (name, token, done) {
    //搜尋這個使用者是否在資料庫內
    User.findOne({ 'token': token }, function(err, user) {
      // if there are any errors, return the error
      if (err){
        return done(err);
      }
      if (user) {
        return done(null, flase, "You already have an account!");
      } 
      else {
        var newUser = new User();
        newUser.name = name;
        newUser.token = token;
        newUser.provider = "local";
        newUser.pet = { //這是目前的初始數值
          "name": name,
          stamina: 50,
          attack: 12,
          defense: 2,
          evade: 30,
          skillCD: {
            ID: 1,
            CD: 3,
            params: undefined //技能還有待設計
          }
        };
        // save the user
        newUser.save(function(err) {
          if (err)
            throw err;
          console.log(newUser.name + "created!");
          return done(null, newUser);
        });
      }
    });
  }));

  //本地資料庫登入
  passport.use('local-login', new LocalStrategy({
      usernameField: 'name',
      passwordField: 'token'
    },
    function(name, token, done) {
      // asynchronous
      process.nextTick(function() {
        //同上搜尋資料庫
        User.findOne({
          'token': token
        }, function(err, user) {
          // if there are any errors, return the error
          if (err)
            return done(err);
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
