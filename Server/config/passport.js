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

  // =========================================================================
  // LOCAL SIGNUP ============================================================
  // =========================================================================
  // we are using named strategies since we have one for login and one for signup
  // by default, if there was no name, it would just be called 'local'
  passport.use('local-signup', new LocalStrategy({
    usernameField: 'name',
    passwordField: 'token',
  },function (name, token, done) {
    User.findOne({ 'token': token }, function(err, user) {
      // if there are any errors, return the error
      if (err){
        return done(err);
      }
      // check to see if theres already a user with that email
      if (user) {
        return done(null, flase, "You already have an account!");
      } else {
        var newUser = new User();
        // set the user's local credentials
        newUser.name = name;
        newUser.token = token;
        newUser.provider = "local";
        newUser.pet = {
          "name": name,
          stamina: 50,
          attack: 12,
          defense: 2,
          evade: 30,
          skillCD: {
            ID: 1,
            CD: 3,
            params: undefined
          }
        };
        // save the user
        newUser.save(function(err) {
          if (err)
            throw err;
          console.log(newUser.name + "created!");
          return done(null, newUser);
        });
        return done(null, newUser);
      }
    });
  }));

  passport.use('local-login', new LocalStrategy({
      // by default, local strategy uses username and password, we will override with email
      usernameField: 'name',
      passwordField: 'token',
      passReqToCallback: true // allows us to pass back the entire request to the callback
    },
    function(req, name, token, done) {
      // asynchronous
      // User.findOne wont fire unless data is sent back
      process.nextTick(function() {
        // find a user whose email is the same as the forms email
        // we are checking to see if the user trying to login already exists
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
