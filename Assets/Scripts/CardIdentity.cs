using UnityEngine;
using UnityEngine.UI;
using BaseGame;

public class CardIdentity : MonoBehaviour
{
	[SerializeField] Sprite[] deckOfCards;
	public int number;
    public string suit;

    private Image childImage;

    void Start()
    {
		PokerCard pokerCardScript = GetComponent<PokerCard>();
		Sprite clubs2 = deckOfCards[0], clubs3 = deckOfCards[1], clubs4 = deckOfCards[2], clubs5 = deckOfCards[3], clubs6 = deckOfCards[4], clubs7 = deckOfCards[5], clubs8 = deckOfCards[6], clubs9 = deckOfCards[7], clubs10 = deckOfCards[8], clubsJack = deckOfCards[9], clubsQueen = deckOfCards[10], clubsKing = deckOfCards[11], clubsAce = deckOfCards[12];
		Sprite diamonds2 = deckOfCards[13], diamonds3 = deckOfCards[14], diamonds4 = deckOfCards[15], diamonds5 = deckOfCards[16], diamonds6 = deckOfCards[17], diamonds7 = deckOfCards[18], diamonds8 = deckOfCards[19], diamonds9 = deckOfCards[20], diamonds10 = deckOfCards[21], diamondsJack = deckOfCards[22], diamondsQueen = deckOfCards[23], diamondsKing = deckOfCards[24], diamondsAce = deckOfCards[25];
		Sprite hearts2 = deckOfCards[26], hearts3 = deckOfCards[27], hearts4 = deckOfCards[28], hearts5 = deckOfCards[29], hearts6 = deckOfCards[30], hearts7 = deckOfCards[31], hearts8 = deckOfCards[32], hearts9 = deckOfCards[33], hearts10 = deckOfCards[34], heartsJack = deckOfCards[35], heartsQueen = deckOfCards[36], heartsKing = deckOfCards[37], heartsAce = deckOfCards[38];
		Sprite spades2 = deckOfCards[39], spades3 = deckOfCards[40], spades4 = deckOfCards[41], spades5 = deckOfCards[42], spades6 = deckOfCards[43], spades7 = deckOfCards[44], spades8 = deckOfCards[45], spades9 = deckOfCards[46], spades10 = deckOfCards[47], spadesJack = deckOfCards[48], spadesQueen = deckOfCards[49], spadesKing = deckOfCards[50], spadesAce = deckOfCards[51];
		Image childImage = GetComponent<Image>();
        number = pokerCardScript.GetCardNumber();
        suit = pokerCardScript.GetSuit();

        if (childImage != null)
        {
	        if (suit.Equals("clubs")) {
		        switch (number)
		        {
			        case 1:
				        childImage.sprite = clubsAce;
				        break;
			        case 2:
				        childImage.sprite = clubs2;
				        break;
			        case 3:
				        childImage.sprite = clubs3;
				        break;
			        case 4:
				        childImage.sprite = clubs4;
				        break;
			        case 5:
				        childImage.sprite = clubs5;
				        break;
			        case 6:
				        childImage.sprite = clubs6;
				        break;
			        case 7:
				        childImage.sprite = clubs7;
				        break;
			        case 8:
				        childImage.sprite = clubs8;
				        break;
			        case 9:
				        childImage.sprite = clubs9;
				        break;
			        case 10:
				        childImage.sprite = clubs10;
				        break;
			        case 11:
				        childImage.sprite = clubsJack;
				        break;
			        case 12:
				        childImage.sprite = clubsQueen;
				        break;
			        case 13:
				        childImage.sprite = clubsKing;
				        break;
		        }
				Debug.Log("clubs");
			} else if (suit.Equals("diamonds"))
	        {
		        switch (number)
		        {
			        case 1:
				        childImage.sprite = diamondsAce;
				        break;
			        case 2:
				        childImage.sprite = diamonds2;
				        break;
			        case 3:
				        childImage.sprite = diamonds3;
				        break;
			        case 4:
				        childImage.sprite = diamonds4;
				        break;
			        case 5:
				        childImage.sprite = diamonds5;
				        break;
			        case 6:
				        childImage.sprite = diamonds6;
				        break;
			        case 7:
				        childImage.sprite = diamonds7;
				        break;
			        case 8:
				        childImage.sprite = diamonds8;
				        break;
			        case 9:
				        childImage.sprite = diamonds9;
				        break;
			        case 10:
				        childImage.sprite = diamonds10;
				        break;
			        case 11:
				        childImage.sprite = diamondsJack;
				        break;
			        case 12:
				        childImage.sprite = diamondsQueen;
				        break;
			        case 13:
				        childImage.sprite = diamondsKing;
				        break;
		        }
		        Debug.Log("diamonds");
	        } else if (suit.Equals("hearts"))
	        {
		        switch (number)
		        {
			        case 1:
				        childImage.sprite = heartsAce;
				        break;
			        case 2:
				        childImage.sprite = hearts2;
				        break;
			        case 3:
				        childImage.sprite = hearts3;
				        break;
			        case 4:
				        childImage.sprite = hearts4;
				        break;
			        case 5:
				        childImage.sprite = hearts5;
				        break;
			        case 6:
				        childImage.sprite = hearts6;
				        break;
			        case 7:	
				        childImage.sprite = hearts7;
				        break;
			        case 8:
				        childImage.sprite = hearts8;
				        break;
			        case 9:
				        childImage.sprite = hearts9;
				        break;
			        case 10:
				        childImage.sprite = hearts10;
				        break;
			        case 11:
				        childImage.sprite = heartsJack;
				        break;
			        case 12:
				        childImage.sprite = heartsQueen;
				        break;
			        case 13:
				        childImage.sprite = heartsKing;
				        break;
		        }
		        Debug.Log("hearts");
	        }
			else if (suit.Equals("spades")){
				switch (number)
				{
					case 1:
						childImage.sprite = spadesAce;
						break;
					case 2:
						childImage.sprite = spades2;
						break;
					case 3:
						childImage.sprite = spades3;
						break;
					case 4:
						childImage.sprite = spades4;
						break;
					case 5:
						childImage.sprite = spades5;
						break;
					case 6:
						childImage.sprite = spades6;
						break;
					case 7:
						childImage.sprite = spades7;
						break;
					case 8:
						childImage.sprite = spades8;
						break;
					case 9:
						childImage.sprite = spades9;
						break;
					case 10:
						childImage.sprite = spades10;
						break;
					case 11:
						childImage.sprite = spadesJack;
						break;
					case 12:
						childImage.sprite = spadesQueen;
						break;
					case 13:
						childImage.sprite = spadesKing;
						break;
				}
				Debug.Log("spades");
			}
			else
			{
				childImage.sprite = clubs2;
			}
        }
        else
        {
            Debug.LogWarning("No Image component found in child.");
        }
    }
}
