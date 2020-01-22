1.) (Optional) In Game.cs, set HIGHSCORE_PASS to something unique.
	* This is really only important for live games so the passcode is not predictable. It doesn't matter for game jams.
	
2.) Add a button to get to the High Scores Menu (GameScenes.HIGH_SCORES) to your title screen (or wherever you want it to go.)

3.) Add calls to Play.GainScore(int) any time the player does something that should give score.

4.) Whenever the player dies or quits the play scene (closing the app, returning to the title screen), you should call Play.CommitScore().
	* Note that by base design, you can only commit the score _once_ per play session. This is to protect against bugs that commit the same score multiple times, or let the player commit a sequence of scores from one play session.