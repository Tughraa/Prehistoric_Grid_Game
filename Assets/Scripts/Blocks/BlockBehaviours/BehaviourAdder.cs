using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourAdder : MonoBehaviour
{
    public AllSystems allSystems;

    public void AddItemBehaviours(ItemState state)
    {
        switch (state.itemData.id)
        {
            case "throw_rock":
                GameObject throwRockFab = allSystems.entitySummonSystem.entityFabs["throw_rock"];
                state.AddBehaviour(new ThrowableItemBehaviour(throwObject: throwRockFab,inThrowDist: 1f,inThrowForce: 350f));
                state.AddBehaviour(new ConsumableItemBehaviour());
                break;
            case "block_bomb":
                GameObject throwBlockBombFab = allSystems.entitySummonSystem.entityFabs["block_bomb"];
                
                state.AddBehaviour(new ThrowableItemBehaviour(throwObject: throwBlockBombFab,inThrowDist: 1f,inThrowForce: 300f));
                state.AddBehaviour(new ConsumableItemBehaviour());
                break;
            case "gods_hands":
                state.AddBehaviour(new GodsHandsItemBehaviour());
                break;
            case "potion":
                state.AddBehaviour(new PotionBehaviour(RandomEffect(10f)));
                state.AddBehaviour(new ConsumableItemBehaviour());
                break;
            case "plant_seed":
                GameObject plantSeedFab = allSystems.entitySummonSystem.entityFabs["plant_seed"];
                state.AddBehaviour(new ThrowableItemBehaviour(throwObject: plantSeedFab,inThrowDist: 1f,inThrowForce: 350f));
                state.AddBehaviour(new ConsumableItemBehaviour());
                break;
            case "fire_starter":
                BlockData fireBlock = allSystems.blockLibrary.allBlocks["fire"];
                state.AddBehaviour(new BlockPlacerBehaviour(fireBlock));
                state.AddBehaviour(new ConsumableItemBehaviour());
                break;
            default:
                break;
        }
    }
    public void AddBlockBehaviours(BlockState state)
    {
        switch (state.blockData.id)
        {
            case "effect_gas":
                AddGasEffect(state);
                state.AddBehaviour(new GasDiffusionBehaviour(22f,0.6f));
                break;
            case "fire":
                state.AddBehaviour(new FireBehaviour(0.5f,12f));
                state.AddBehaviour(new SpreadBehaviour(true,0.4f,2f,1));
                break;
            case "explosive":
                state.AddBehaviour(new ExplosiveBehaviour(5f,null));
                break;
            case "flammable_gas":
                state.AddBehaviour(new FlamGasBehaviour());
                state.AddBehaviour(new GasDiffusionBehaviour(22f,10f));
                break;
            case "effect_explosive":
                List<IStatusEffect> bombEffects = new List<IStatusEffect>();
                bombEffects.Add(new BurnEffect(duration: 5.0f,strength: 0.5f,period: 0.3f));
                state.AddBehaviour(new ExplosiveBehaviour(7f,bombEffects));
                break;
            default:
                break;
        }
        /*switch (state.blockData.tags)
        {

        }*/
    }
    
    void AddGasEffect(BlockState state)
    {
        switch (state.blockData.tags[0])
        {
            case "randomEffect":
                state.AddBehaviour(new EffectGasBehaviour(RandomEffect(6f)));
                break;
            case "poison":
                state.AddBehaviour(new EffectGasBehaviour(new PoisonEffect(duration: 10f,strength: 0.125f,period: 0.30f)));
                break;
            case "speed":
                state.AddBehaviour(new EffectGasBehaviour(new SpeedEffect(duration: 10f,strength: 3f)));
                break;
            case "burn":
                state.AddBehaviour(new EffectGasBehaviour(new BurnEffect(duration: 5f,strength: 0.5f,period: 0.75f)));
                break;
            case "jumpBoost":
                state.AddBehaviour(new EffectGasBehaviour(new JumpBoostEffect(duration: 5f,strength: 2f)));
                break;
            case "healing":
                state.AddBehaviour(new EffectGasBehaviour(new HealingEffect(duration: 5f,strength: 0.5f,period: 0.75f)));
                break;
            case "floating":
                state.AddBehaviour(new EffectGasBehaviour(new FloatEffect(duration: 8f,strength: 0.2f)));
                break;
            case "flying":
                state.AddBehaviour(new EffectGasBehaviour(new FlyEffect(duration: 10f,strength: 0.2f)));
                break;
            default:
                state.AddBehaviour(new EffectGasBehaviour(RandomEffect(6f)));
                break;
        }
    }
    public IStatusEffect RandomEffect(float givenDuration)
    {
        int rand = allSystems.randomSystem.effectRNG.Next(0,9);
        //rand = 2; //To test certain effects
        switch (rand)
        {
            case 0:
                return (new PoisonEffect(duration: givenDuration,strength: 0.125f,period: 0.30f));
            case 1:
                return (new BurnEffect(duration: givenDuration,strength: 0.5f,period: 0.75f));
            case 2:
                return (new StunEffect(duration: givenDuration,strength: 30f));
            case 3:
                return (new FreezeEffect(duration: givenDuration,strength: 1.7f));
            case 4:
                return (new TremorEffect(duration: givenDuration,strength: 1.5f,period: 0.3f));
            case 5:
                return (new SpeedEffect(duration: givenDuration,strength: 3f));
            case 6:
                return (new JumpBoostEffect(duration: givenDuration,strength: 2f));
            case 7:
                return (new HealingEffect(duration: givenDuration,strength: 0.5f,period: 0.75f));
            case 8:
                return (new FlyEffect(duration: givenDuration,strength: 0.2f));
            default:
                return (new SpeedEffect(duration: 1f,strength: 3f));
        }
    }
}
