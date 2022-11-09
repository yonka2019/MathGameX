# Math Game (C# Xamarin)
School 12 Grade - Final Project

## MAIN SCREEN
START / STATS / SETTINGS - Buttons

### START
[GAME]

### STATS
Shows the player in which exercises types he had the best time, his strong aspects
also the history of last wrong answers

### SETTINGS
allows to limit the random according those settings, the settings are:
*single-digit numbers
* double-digit numbers
* triple-digit numbers
* plus
* minus
* divide
* multiply

each setting is a checkbox which can be applied to get randomized
## GAME
* Gives a random exercise according the chosen settings before
* in the bottom of the screen there is a timer which shows the time the player play
* there is option to skip the exercise (will count as wrong answer)
* option to stop the game and view results
* each answer interpreted as "EXERCISE TYPE" (3 * 4 => SINGLE-DIGIT & MULTIPLY) and added to DB which assist to STATS table

game have infinkty mode and level mode
- in infinity mode the player plays until he leaves the games, afterward, the game will show up his time, stats and mkre
- in level game, the game will represent three levels (ez, med, hard) each level dofferent from each other by the exercies, thr time will be the same (20 sec or some..), for exmp: ez will contain only one digit plus minus, the med one digit and double  digits, and minus plus and multiply, etc..

## RESULTS
* Shows total game time
* Shows average time to answer a exercise
