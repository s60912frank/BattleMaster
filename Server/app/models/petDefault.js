var auguDefault = {
    name: "Augu",
    stamina: 40,
    attack: 20,
    defense: 5,
    evade: 20,
    skill: {
      ID: 1,
      CD: 3,
      SkillDesc: "使敵人每回合受到6點燃燒傷害並提升自身攻擊力三點",
      params: {
        damage: 0,
        recover: 0,
        burn: 6,
        attIncrease: 3
      }
    }
}

var vDefault = {
    name: "V",
    stamina: 30,
    attack: 15,
    defense: 5,
    evade: 35,
    skill: {
      ID: 1,
      CD: 3,
      SkillDesc: "回復10點生命力並提升攻擊力三點",
      params: {
        damage: 0,
        recover: 10,
        burn: 0,
        attIncrease: 3
      }
    }
}

var charmanderDefault = {
    name: "Charmander",
    stamina: 65,
    attack: 5,
    defense: 10,
    evade: 10,
    skill: {
      ID: 1,
      CD: 3,
      SkillDesc: "回復20點生命力並提升攻擊力三點",
      params: {
        damage: 0,
        recover: 20,
        burn: 0,
        attIncrease: 3
      }
    }
}

module.exports = (type) => {
    switch(type){
        case "Augu":
            return auguDefault;
        case "V":
            return vDefault;
        case "Charmander":
            return charmanderDefault;
        default:
            throw "UNRECONIZED TYPE!";
    }
}