1.) Set your playfield bounds in Game.cs:
    public static float PlayfieldX = 0;
    public static float PlayfieldY = 0;
    public static float PlayfieldWidth = 800;
    public static float PlayfieldHeight = 600;
	
2.) Add a ClampToPlayfield behavior to anything you want to clamp to the playfield.

3.) Use the WhileInPlayfield duration on anything that should behave a certain way while it is in the playfield.