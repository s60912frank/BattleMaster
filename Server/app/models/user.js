// load the things we need
var mongoose = require('mongoose');
//for shorter id
var shortid = require('shortid');

// define the schema for our user model
var userSchema = mongoose.Schema({
  _id: {
    type: String,
    'default': shortid.generate
  },
  provider: String,
  fbid: String,
  token: String,
  name: String,
  mileage: Number,
  pet: {
    name: String,
    level: Number,
    stamina: Number,
    attack: Number,
    defense: Number,
    evade: Number,
    skill: {
      ID: Number,
      CD: Number,
      SkillDesc: String,
      params: {
        damage: Number,
        recover: Number,
        burn: Number,
        attIncrease: Number
      }
    }
  }
});

// create the model for users and expose it to our app
module.exports = mongoose.model('User', userSchema);
