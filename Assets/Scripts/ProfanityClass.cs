using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ProfanityClass
{
    public bool IsContentProfane(string content)
	{
		string alphabetAndSpaceOnly = string.Join("", content.Where(i => char.IsLetter(i) || i == ' '));
		string[] wordsInContent = alphabetAndSpaceOnly.Split(" "[0]);
		
		foreach (var word in wordsInContent)
		{
			if (ProfanityWords.Contains(word.ToLower()))
				return true;
		}
		
		return false;
	}

	public List<string> GetProfanity(string content)
	{
		string alphabetAndSpaceOnly = string.Join("", content.Where(i => char.IsLetter(i) || i == ' '));
		string[] wordsInContent = alphabetAndSpaceOnly.Split(" "[0]);
		List<string> profanityList = new List<string>();

		foreach (var word in wordsInContent)
		{
			if (ProfanityWords.Contains(word.ToLower()))
			{
				Debug.Log("RUDE! YOU SAID: " + word);
				profanityList.Add(word);
			}
		}

		Debug.Log("Your sins: " + string.Join(", ", profanityList));
		
		return profanityList;
	}

	public readonly List<string> ProfanityWords = new List<string>()
	{
		"anal",
		"anus",
		"arse",
		"ass",
		"ballsack",
		"balls",
		"bastard",
		"bitch",
		"biatch",
		"bloody",
		"blowjob",
		"blow job",
		"bollock",
		"bollok",
		"boner",
		"boob",
		"bugger",
		"bum",
		"butt",
		"buttplug",
		"clitoris",
		"cock",
		"coon",
		"crap",
		"cunt",
		"dick",
		"dildo",
		"dyke",
		"fag",
		"feck",
		"fellate",
		"fellatio",
		"felching",
		"fuck",
		"f u c k",
		"fudgepacker",
		"fudge packer",
		"flange",
		"Goddamn",
		"God damn",
		"hell",
		"homo",
		"jerk",
		"jizz",
		"knobend",
		"knob end",
		"labia",
		"lmao",
		"lmfao",
		"muff",
		"nigger",
		"nigga",
		"penis",
		"piss",
		"poop",
		"prick",
		"pube",
		"pussy",
		"queer",
		"scrotum",
		"sex",
		"shit",
		"s hit",
		"sh1t",
		"slut",
		"smegma",
		"spunk",
		"tit",
		"tosser",
		"turd",
		"twat",
		"vagina",
		"wank",
		"whore",
		"wtf"
	};
}
