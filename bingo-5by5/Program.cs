using System.Diagnostics;

/// Constants
int MAX_CARD_ROWS = 5;
int MAX_CARD_COLUMNS = 5;
int MAX_POSSIBLE_NUMBERS = 99;
int STRAIGHT_LINE_FILLED_POINTS = 3;
int CARD_FILLED_POINTS = 5;

bool hasRowBeenFilled = false;
bool hasColumnBeenFilled = false;

int totalPlayers = 2;
int currentPlayerIndex = 0;
int[] possibleNumbers = createAllPossibleNumbers();
int[] drawnNumbers = new int[MAX_POSSIBLE_NUMBERS];
int currentDrawnNumbersIndex = 0;

int[][][,] playersCards = new int[totalPlayers][][,];
int[] playersPoints = new int[totalPlayers];

/**
 *  Returns true if some value exists in a vector
 *  
 *  @return bool
 */
bool contains(int[] vector, int value)
{
    for (int i = 0; i < vector.Length; i++)
        if (vector[i] == value)
            return true;

    return false;
}

/**
 *  Returns the index of the chosen element
 *  
 *  @param int[] vector
 *  @return int the chosen element
 */
int choose(int[] vector)
{
    return new Random().Next(0, vector.Length);
}

/**
 *  Verifies if there's any possible numbers left to be drawn
 *
 *  @return bool true if exists, false if not
 */
bool hasPossibleNextNumber()
{
    for (int i = 0; i < MAX_POSSIBLE_NUMBERS; i++)
        if (possibleNumbers[i] > 0)
            return true;

    return false;
}

/**
 *  Draws the next possible number
 *  
 *  @return int the drawn number
 */
int drawNextPossibleNumber()
{
    if (!hasPossibleNextNumber())
        return -1;

    while (true)
    {
        int index = choose(possibleNumbers);

        if (possibleNumbers[index] < 1)
            continue;

        int next = possibleNumbers[index];
        possibleNumbers[index] = 0;

        drawnNumbers[currentDrawnNumbersIndex++] = next;

        return next;
    }
}

/**
 *  Essentially this function just do the same as contains, so it's mainly for code readability and to abstract the 'drawnNumbers' vector from other unrelated functions
 *  
 *  @param  int number
 *  @return bool true if it's been drawn, false if not
 */
bool hasNumberBeenDrawn(int number)
{
    return contains(drawnNumbers, number);
}

/**
 *  Creates and returns an array with all the possible numbers
 * 
 *  @return int[] array of all the possible numbers from 1 to 99
 */
int[] createAllPossibleNumbers()
{
    int[] possibleNumbers = new int[MAX_POSSIBLE_NUMBERS];

    for (int i = 0; i < MAX_POSSIBLE_NUMBERS; i++)
        possibleNumbers[i] = i + 1;

    return possibleNumbers;
}

/**
 *  @return int[,]  the created card populated with random values from 1 to 99
 */
int[,] createCard()
{
    int[] possibleCardNumbers = createAllPossibleNumbers();

    int[,] card = new int[MAX_CARD_ROWS, MAX_CARD_COLUMNS];

    for (int i = 0; i < MAX_CARD_ROWS; i++)
        for (int j = 0; j < MAX_CARD_COLUMNS; j++)
        {
            int index = choose(possibleCardNumbers);

            while (possibleCardNumbers[index] < 1)
                index = choose(possibleCardNumbers);

            card[i, j] = possibleCardNumbers[index];

            possibleCardNumbers[index] = 0;
        }

    return card;
}

/**
 *  @param   int totalCards  the total number of cards for the player  
 *  @return  int             the index of the created player 
 */
int createPlayer(int totalCards)
{
    if (totalCards < 1)
        return 0;

    /*if (currentPlayerIndex >= totalPlayers)
        return 0;
    */

    playersPoints[currentPlayerIndex] = 0;

    playersCards[currentPlayerIndex] = new int[totalCards][,];

    for (int i = 0; i < totalCards; i++)
        playersCards[currentPlayerIndex][i] = createCard();

    return currentPlayerIndex++;
}

/**
 *  @param  int         playerIndex the position of the player to retrieve the cards
 *  @return int[][,]    the collection of cards of the given player
 */
int[][,] getPlayerCardsByIndex(int playerIndex)
{
    if (playerIndex < 0)
        return null;

    return playersCards[playerIndex];
}

/**
 *  @param  int playerIndex the position of the player to get the points
 *  @return int             the quantity of points of the given player
 */
int getPlayerPointsByIndex(int playerIndex)
{
    if (playerIndex < 0)
        return -1;

    return playersPoints[playerIndex];
}

/**
 *  Gives an specific amount of points to given player
 *  
 *  @param  int playerIndex the position of the player to add the points
 *  @param  int points      the amount of points to be given
 */
void givePointsToPlayer(int playerIndex, int points)
{
    if (points < 0)
        return;

    playersPoints[playerIndex] += points;
}

/**
 *  Returns true if a row is filled in a card
 *  
 *  @param  int[,]  card    the card being checked
 *  @return bool            true if is filled, false if not
 */
bool isRowFilled(int[,] card)
{
    for (int i = 0; i < MAX_CARD_ROWS; i++)
    {
        int hits = 0;

        for (int j = 0; j < MAX_CARD_COLUMNS; j++)
            if (hasNumberBeenDrawn(card[i, j]))
                hits++;

        if (hits == MAX_CARD_ROWS)
            return true;
    }

    return false;
}

/**
 *  Returns true if a column is filled in a card
 *  
 *  @param  int[,]  card    the card being checked
 *  @return bool            true if is filled, false if not
 */
bool isColumnFilled(int[,] card)
{
    for (int i = 0; i < MAX_CARD_COLUMNS; i++)
    {
        int hits = 0;

        for (int j = 0; j < MAX_CARD_ROWS; j++)
            if (hasNumberBeenDrawn(card[j, i]))
                hits++;

        if (hits == MAX_CARD_COLUMNS)
            return true;
    }

    return false;
}

/**
 *  Returns true if the whole card is filled 
 *  
 *  @param  int[,]   card   the card being checked
 *  @return bool            true if is filled, false if not
 */
bool isCardFilled(int[,] card)
{
    for (int i = 0; i < MAX_CARD_ROWS; i++)
        for (int j = 0; j < MAX_CARD_COLUMNS; j++)
            if (!hasNumberBeenDrawn(card[i, j]))
                return false;

    return true;
}

/**
 *  Returns a vector of booleans with indexes for each player indicating whether or not they won by having one of their card filled.
 *  If they have, 'true' will be set at their player index, if not, 'false'.
 *  The reason of this approach is because there can be more than only one winner.
 *  
 *  @return bool[]   the vector of booleans containing true if the player at his index won, false if not.
 */
bool[] verifyNextWinners()
{
    bool[] nextWinners = new bool[totalPlayers];

    for (int i = 0; i < totalPlayers; i++)
    {
        int[][,] cards = getPlayerCardsByIndex(i);

        for (int j = 0; j < cards.Length; j++)
            if (isCardFilled(cards[j]))
                nextWinners[i] = true;
    }

    return nextWinners;
}


/**
 *  Returns a vector of booleans with indexes for each player indicating whether or not they have any row filled.
 *  If they have, 'true' will be the set at their player index, if not, 'false'.
 *  The reason of this approach is because there can be more than only one player that filled a row at the same time.
 *  
 *  @return bool[]   the vector of booleans containing true if the player at his index filled a row, false if not.
 */
bool[] verifyFilledRows()
{
    bool[] playersWithFilledRows = new bool[totalPlayers];

    for (int i = 0; i < totalPlayers; i++)
    {
        int[][,] cards = getPlayerCardsByIndex(i);

        for (int j = 0; j < cards.Length; j++)
            if (isRowFilled(cards[j]))
                playersWithFilledRows[i] = true;
    }

    return playersWithFilledRows;
}

/**
 *  Returns a vector of booleans with indexes for each player indicating whether or not they have any column filled.
 *  If they have, 'true' will be set at their player index, if not, 'false'.
 *  The reason of this approach is because there can be more than only one player that filled a column at the same time.
 *  
 *  @return bool[]   the vector of booleans containing true if the player at his index filled a column, false if not.
 */
bool[] verifyFilledColumns()
{
    bool[] playersWithFilledColumns = new bool[totalPlayers];

    for (int i = 0; i < totalPlayers; i++)
    {
        int[][,] cards = getPlayerCardsByIndex(i);

        for (int j = 0; j < cards.Length; j++)
            if (isColumnFilled(cards[j]))
                playersWithFilledColumns[i] = true;
    }

    return playersWithFilledColumns;
}

/**
 *  Awards the players that filled any line (row or column) if it's the first time being filled in the round
 */
void awardPlayersThatFilledAnyLine()
{
    if (hasRowBeenFilled && hasColumnBeenFilled)
        return;

    bool[] playersThatFilledRows = verifyFilledRows();
    bool[] playersThatFilledColumns = verifyFilledColumns();

    for (int i = 0; i < totalPlayers; i++)
    {
        bool currentPlayerCanBeAwarded = playersThatFilledRows[i] && !hasRowBeenFilled;

        if (currentPlayerCanBeAwarded)
        {
            displayNotification($"Jogador {i + 1} preencheu uma linha e ganhou {STRAIGHT_LINE_FILLED_POINTS} pontos!");
            givePointsToPlayer(i, STRAIGHT_LINE_FILLED_POINTS);

            hasRowBeenFilled = true;
        }

        currentPlayerCanBeAwarded = playersThatFilledColumns[i] && !hasColumnBeenFilled;

        if (currentPlayerCanBeAwarded)
        {
            displayNotification($"Jogador {i + 1} preencheu uma coluna e ganhou {STRAIGHT_LINE_FILLED_POINTS} pontos!");
            givePointsToPlayer(i, STRAIGHT_LINE_FILLED_POINTS);

            hasColumnBeenFilled = true;
        }
    }
}

/**
 *  Awards given player for winning
 * 
 *  @param  int playerIndex the position of the player to award
 */
void awardPlayerForWinning(int playerIndex)
{
    displayNotification($"\nJogador {playerIndex + 1} marcou BINGO! +{CARD_FILLED_POINTS} pontos!");

    givePointsToPlayer(playerIndex, CARD_FILLED_POINTS);
}

/**
 *  Displays the given card
 *  
 *  @param int[,]   card    the card being displayed
 */
void displayCard(int[,] card)
{
    Console.WriteLine("--------------");

    for (int i = 0; i < MAX_CARD_ROWS; i++)
    {
        for (int j = 0; j < MAX_CARD_COLUMNS; j++)
        {
            if (hasNumberBeenDrawn(card[i, j]))
                Console.ForegroundColor = ConsoleColor.Red;

            Console.Write(card[i, j].ToString("00") + " ");

            Console.ResetColor();
        }

        Console.WriteLine();
    }

    Console.WriteLine("--------------");
}

/**
 *  Displays all the cards of a player
 * 
 *  @param  int playerIndex   the given player's cards to be displayed
 */
void displayCardsFromPlayer(int playerIndex)
{
    int[][,] cards = getPlayerCardsByIndex(playerIndex);

    Console.WriteLine($"Cartelas do jogador {playerIndex + 1}:");

    for (int i = 0; i < cards.Length; i++)
        displayCard(cards[i]);

    Console.WriteLine();
}

/**
 *  Displays the points of all players
 */
void displayAllPlayersPoints()
{
    for (int i = 0; i < totalPlayers; i++)
    {
        int points = getPlayerPointsByIndex(i);

        Console.WriteLine($"Jogador {i + 1} possui {points} ponto{(points == 1 ? "" : "s")}");
    }
}

/**
 *  Displays the current drawn numbers, if any
 */
void displayDrawnNumbers()
{
    bool hasAnyDrawnNumbers = currentDrawnNumbersIndex > 0;

    if (!hasAnyDrawnNumbers)
    {
        Console.WriteLine("Nenhum número foi sorteado ainda.");
        return;
    }

    Console.WriteLine();

    Console.WriteLine("Números sorteados:\n");

    for (int i = 0; i < currentDrawnNumbersIndex; i++)
    {
        Console.Write($"{drawnNumbers[i].ToString("00")}");

        if (i < currentDrawnNumbersIndex - 1)
            Console.Write(" - ");
    }

    Console.WriteLine();
}

/**
 *  Displays a generic notification
 *  
 *  @param  string  message the message to be displayed
 */
void displayNotification(string message)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{message}\n");
    Console.ResetColor();
}

/// Game's main loop

for (int i = 0; i < totalPlayers; i++)
    createPlayer(2); /// 1 card for player, hardcoded for now

while (true)
{
    Console.Clear();

    drawNextPossibleNumber();

    for (int i = 0; i < totalPlayers; i++)
        displayCardsFromPlayer(i);

    awardPlayersThatFilledAnyLine();

    displayAllPlayersPoints();

    displayDrawnNumbers();

    bool[] nextWinners = verifyNextWinners();
    int winners = 0;

    for (int i = 0; i < totalPlayers; i++)
        if (nextWinners[i])
        {
            awardPlayerForWinning(i);
            winners++;
        }

    if (winners > 0)
        break;

    Console.ReadKey();
}

/*
/// How to get player's data from index
int playerIndex = createPlayer(1);
int playerTotalCards = playersCards[playerIndex].Length;
int playerPoints = playersPoints[playerIndex];


 * Player:
 * points int,
 * cards int[,]


int[,] card = createCard();

card[3, 0] = 10;
card[3, 1] = 15;
card[3, 2] = 30;
card[3, 3] = 16;
card[3, 4] = 24;

drawnNumbers[0] = 15;
drawnNumbers[1] = 30;
drawnNumbers[2] = 10;
drawnNumbers[3] = 16;
drawnNumbers[4] = 24;

int[,] card2 = createCard();

card2[0, 0] = 17;
card2[1, 0] = 18;
card2[2, 0] = 20;
card2[3, 0] = 15;
card2[4, 0] = 8;

drawnNumbers[5] = 17;
drawnNumbers[6] = 18;
drawnNumbers[7] = 20;
drawnNumbers[8] = 15;
drawnNumbers[9] = 8;

int[,] card3 = createCard();

for (int i = 0; i < MAX_CARD_ROWS; i++)
{
    for (int j = 0; j < MAX_CARD_COLUMNS; j++)
    {
        card3[i, j] = (i + 1) * (j + 1);
        drawnNumbers[10 + ((i + 1) * (j + 1))] = (i + 1) * (j + 1);
    }
}

Console.WriteLine("Cartela do Player 1:");
displayCard(card);
Console.WriteLine("\nCartela do Player 2:");
displayCard(card2);
Console.WriteLine("\nCartela do Player 3:");
displayCard(card3);

// Tests
Debug.Assert(possibleNumbers.Length == 99);
Debug.Assert(card.Length == 25);
Debug.Assert(playerIndex == 0);
Debug.Assert(currentPlayerIndex == 1);
Debug.Assert(playerTotalCards == 1);
Debug.Assert(playerPoints == 0);
Debug.Assert(getPlayerCardsByIndex(0).Length == 1);
Debug.Assert(getPlayerPointsByIndex(0) == 0);
Debug.Assert(contains(possibleNumbers, 1));
Debug.Assert(!contains(possibleNumbers, 0));
Debug.Assert(isRowFilled(card));
Debug.Assert(!isColumnFilled(card));
Debug.Assert(!isRowFilled(card2));
Debug.Assert(isColumnFilled(card2));
Debug.Assert(isCardFilled(card3));
Debug.Assert(!isCardFilled(card2));
Debug.Assert(!isCardFilled(card));*/