// load the things we need
var mongoose = require('mongoose');

// define the schema for our user model
var userSchema = mongoose.Schema({
  provider: String,
  token: String,
  name: String,
  abilityPoint: Number,
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
