using Godot;
using System;

public static class Evaluator
{
    public static int evaluation(gameState state2, character enemy){
		int grade =0, sum=0, i;

        if (state2.playerHP > 20) { grade = grade + 100; }
        else if (state2.playerHP > 10 && state2.playerHP < 21) { grade = grade + 50; }
        else if (state2.playerHP > 0 && state2.playerHP < 11) { grade = grade + 10; }
        else {  grade = grade - 1000; }

        if (state2.helperHP > 20) { grade = grade + 25; }
        else if (state2.helperHP > 10 && state2.helperHP < 21) { grade = grade + 15; }
        else if (state2.helperHP > 0 && state2.helperHP < 11) { grade = grade + 5; }
        else { grade = grade - 50; }

        if (state2.enemyHP > (enemy.HP * 0.33) && state2.enemyHP <= (enemy.HP * 0.66)) { grade = grade + 20; }
        else if (state2.enemyHP > 0 && state2.enemyHP < (enemy.HP * 0.33)) { grade = grade + 50; }
        else if (state2.enemyHP == 0) { grade = grade + 1000; }

		for (i=0; i<4; i++){  if (state2.counterState[i] >0){ sum = sum +1;}}
		
		if (state2.counterState[0] == 1 && state2.counterState[1] == 1){grade = grade + 10;}
		if (state2.counterState[2] == 1 && state2.counterState[3] == 1) {grade = grade + 10;}
		if (state2.counterState[0] == 0 && sum == 1 || state2.counterState[0] == 1 && sum==2){ grade = grade + 17;}
		if ( sum>0 && sum<4){ grade = grade + sum*5;} else {grade = grade + 15;}

		if (state2.dmg > 0){
			float dmgMul = 0.4f;

			if(state2.playerHP > 0 && state2.playerHP <11) { dmgMul = dmgMul*0.01f;}
			if (state2.helperHP >0 && state2.helperHP < 11) { dmgMul = dmgMul*0.5f;}
			if (state2.enemyHP < (enemy.HP*0.33)) { dmgMul = dmgMul*3f;}
			else if (state2.enemyHP <= (enemy.HP*0.66f)) {dmgMul = dmgMul*1.5f;}

			grade = grade + (int)Math.Floor(state2.dmg * dmgMul);
		}

		if (state2.heal == 1){
			if (state2.playerHP >0 && state2.playerHP < 11) { grade = grade +20;}
			else if (state2.playerHP > 10 && state2.playerHP <21) {grade = grade + 5;}
		} else if (state2.heal == 2){
			if (state2.helperHP >0 && state2.helperHP < 11) { grade = grade +10;}
		}


		return grade; 

	}
}
