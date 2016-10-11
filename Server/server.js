var express = require('express');
var app = express();
var port = process.env.OPENSHIFT_NODEJS_PORT || 8080;
var ip_address = process.env.OPENSHIFT_NODEJS_IP || '127.0.0.1';
var mongoose = require('mongoose');
var passport = require('passport');


var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');
var session = require('express-session');
var mongoStore = require('connect-mongo')(session);
var passportSocketIo = require("passport.socketio");

var io = require('socket.io')(app.server);//client跟server連接

var configDB = require('./config/database.js'); //db位置

var DEBUG = false;
if(!DEBUG){
  console.log = () => {};
}
else{
  app.use(require('morgan')('dev')); // log every request to the console
}

//public
app.use('/public', express.static(__dirname + '/public'));

// configuration ===============================================================
mongoose.connect(configDB.url); // connect to our database
var msInstance = new mongoStore({ mongooseConnection: mongoose.connection }); //save session in mongo

require('./config/passport')(passport); // pass passport for configuration

// set up our express application
app.use(cookieParser()); // read cookies (needed for auth)
app.use(bodyParser.urlencoded({ extended: true })); //讀取請求的body資料
app.use(bodyParser.json());

// required for passport
app.use(session({
    secret: 'gminissosmart', //??? // session secret
    resave: false,
    saveUninitialized: false,
    store: msInstance
})); 
app.use(passport.initialize());
app.use(passport.session()); // persistent login sessions

io.use(passportSocketIo.authorize({
  cookieParser: cookieParser,       // the same middleware you registrer in express
  key: 'connect.sid',       // the name of the cookie where express/connect stores its session_id
  secret: 'gminissosmart',    // the session_secret to parse the cookie
  store: msInstance,        // store sessions in mongodb
  success: function (data, accept){ accept() }
}));

//戰鬥!
require('./app/battle')(io);

// routes ======================================================================
require('./app/routes.js')(app, passport); // load our routes and pass in our app and fully configured passport

// launch ======================================================================
//io.listen(app.listen(port, ip_address, () => console.log('The magic happens on port ' + port)));
//debug用
io.listen(app.listen(port, () => console.log('The magic happens on port ' + port)));
