using UnityEngine;
using UnityEngine.UI;
using BaseGame;

public class CardIdentity : MonoBehaviour
{
	[SerializeField] Sprite[] deckOfCards;
	public int number;
	public string suit;
	private SpriteRenderer sr;
	private PokerCard pokerCardScript;
	private Image childImage;
	[SerializeField] private Sprite cardBack;

	void Start()
	{
		sr = GetComponent<SpriteRenderer>();
		pokerCardScript = GetComponent<PokerCard>();
		number = pokerCardScript.GetCardNumber();
		suit = pokerCardScript.GetSuit();
		UpdateCardDisplay();
	}
	void Update()
	{
		UpdateCardDisplay();
	}
	private void UpdateCardDisplay()
	{
		Sprite clubs2 = deckOfCards[0], clubs3 = deckOfCards[1], clubs4 = deckOfCards[2], clubs5 = deckOfCards[3], clubs6 = deckOfCards[4], clubs7 = deckOfCards[5], clubs8 = deckOfCards[6], clubs9 = deckOfCards[7], clubs10 = deckOfCards[8], clubsJack = deckOfCards[9], clubsQueen = deckOfCards[10], clubsKing = deckOfCards[11], clubsAce = deckOfCards[12];
		Sprite diamonds2 = deckOfCards[13], diamonds3 = deckOfCards[14], diamonds4 = deckOfCards[15], diamonds5 = deckOfCards[16], diamonds6 = deckOfCards[17], diamonds7 = deckOfCards[18], diamonds8 = deckOfCards[19], diamonds9 = deckOfCards[20], diamonds10 = deckOfCards[21], diamondsJack = deckOfCards[22], diamondsQueen = deckOfCards[23], diamondsKing = deckOfCards[24], diamondsAce = deckOfCards[25];
		Sprite hearts2 = deckOfCards[26], hearts3 = deckOfCards[27], hearts4 = deckOfCards[28], hearts5 = deckOfCards[29], hearts6 = deckOfCards[30], hearts7 = deckOfCards[31], hearts8 = deckOfCards[32], hearts9 = deckOfCards[33], hearts10 = deckOfCards[34], heartsJack = deckOfCards[35], heartsQueen = deckOfCards[36], heartsKing = deckOfCards[37], heartsAce = deckOfCards[38];
		Sprite spades2 = deckOfCards[39], spades3 = deckOfCards[40], spades4 = deckOfCards[41], spades5 = deckOfCards[42], spades6 = deckOfCards[43], spades7 = deckOfCards[44], spades8 = deckOfCards[45], spades9 = deckOfCards[46], spades10 = deckOfCards[47], spadesJack = deckOfCards[48], spadesQueen = deckOfCards[49], spadesKing = deckOfCards[50], spadesAce = deckOfCards[51];
		//Image childImage = GetComponent<Image>();
		if (sr == null || pokerCardScript == null) return;

		if (!pokerCardScript.IsFaceUp)
		{
			sr.sprite = cardBack;
			return;
		}
		if (sr != null)
		{
			if (suit.Equals("clubs"))
			{
				switch (number)
				{
					case 1:
						sr.sprite = clubsAce;
						break;
					case 2:
						sr.sprite = clubs2;
						break;
					case 3:
						sr.sprite = clubs3;
						break;
					case 4:
						sr.sprite = clubs4;
						break;
					case 5:
						sr.sprite = clubs5;
						break;
					case 6:
						sr.sprite = clubs6;
						break;
					case 7:
						sr.sprite = clubs7;
						break;
					case 8:
						sr.sprite = clubs8;
						break;
					case 9:
						sr.sprite = clubs9;
						break;
					case 10:
						sr.sprite = clubs10;
						break;
					case 11:
						sr.sprite = clubsJack;
						break;
					case 12:
						sr.sprite = clubsQueen;
						break;
					case 13:
						sr.sprite = clubsKing;
						break;
				}
			}
			else if (suit.Equals("diamonds"))
			{
				switch (number)
				{
					case 1:
						sr.sprite = diamondsAce;
						break;
					case 2:
						sr.sprite = diamonds2;
						break;
					case 3:
						sr.sprite = diamonds3;
						break;
					case 4:
						sr.sprite = diamonds4;
						break;
					case 5:
						sr.sprite = diamonds5;
						break;
					case 6:
						sr.sprite = diamonds6;
						break;
					case 7:
						sr.sprite = diamonds7;
						break;
					case 8:
						sr.sprite = diamonds8;
						break;
					case 9:
						sr.sprite = diamonds9;
						break;
					case 10:
						sr.sprite = diamonds10;
						break;
					case 11:
						sr.sprite = diamondsJack;
						break;
					case 12:
						sr.sprite = diamondsQueen;
						break;
					case 13:
						sr.sprite = diamondsKing;
						break;
				}
			}
			else if (suit.Equals("hearts"))
			{
				switch (number)
				{
					case 1:
						sr.sprite = heartsAce;
						break;
					case 2:
						sr.sprite = hearts2;
						break;
					case 3:
						sr.sprite = hearts3;
						break;
					case 4:
						sr.sprite = hearts4;
						break;
					case 5:
						sr.sprite = hearts5;
						break;
					case 6:
						sr.sprite = hearts6;
						break;
					case 7:
						sr.sprite = hearts7;
						break;
					case 8:
						sr.sprite = hearts8;
						break;
					case 9:
						sr.sprite = hearts9;
						break;
					case 10:
						sr.sprite = hearts10;
						break;
					case 11:
						sr.sprite = heartsJack;
						break;
					case 12:
						sr.sprite = heartsQueen;
						break;
					case 13:
						sr.sprite = heartsKing;
						break;
				}
			}
			else if (suit.Equals("spades"))
			{
				switch (number)
				{
					case 1:
						sr.sprite = spadesAce;
						break;
					case 2:
						sr.sprite = spades2;
						break;
					case 3:
						sr.sprite = spades3;
						break;
					case 4:
						sr.sprite = spades4;
						break;
					case 5:
						sr.sprite = spades5;
						break;
					case 6:
						sr.sprite = spades6;
						break;
					case 7:
						sr.sprite = spades7;
						break;
					case 8:
						sr.sprite = spades8;
						break;
					case 9:
						sr.sprite = spades9;
						break;
					case 10:
						sr.sprite = spades10;
						break;
					case 11:
						sr.sprite = spadesJack;
						break;
					case 12:
						sr.sprite = spadesQueen;
						break;
					case 13:
						sr.sprite = spadesKing;
						break;
				}
			}
			else
			{
				sr.sprite = clubs2;
			}
		}
		else
		{
			Debug.LogWarning("No Image component found in child.");
		}
	}
}
