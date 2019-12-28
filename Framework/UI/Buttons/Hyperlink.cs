using Godot;
using System;

/// <summary>
/// A rich text label that can be clicked to visit a URL. You must set 'URL' to the correct URL, and 'LinkText'
/// to show the text that will be clicked on.
/// </summary>
public class Hyperlink : RichTextLabel
{
    [Export] public string URL;
    [Export] public string LinkText;

    public override void _Ready()
    {
        BbcodeEnabled = true;
        BbcodeText = $"[url={URL}]{LinkText}[/url]";
        Connect("meta_clicked", this, "OnClicked");
    }

    private void OnClicked(string meta)
    {
        OS.ShellOpen(meta);
    }

}
