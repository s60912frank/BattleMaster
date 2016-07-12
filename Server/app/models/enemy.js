// load the things we need
var mongoose = require('mongoose');

// define the schema for our enemy model
var enemySchema = mongoose.Schema({
  name: String,
  level: Number,
  reward: Number,
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
});

// create the model for users and expose it to our app
module.exports = mongoose.model('Enemy', enemySchema);
