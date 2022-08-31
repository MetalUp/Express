namespace Foo
{
    public static class BlackJack
    {
        public static List<char> Cards = new List<char> { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };

        public static int CardValue(char card) =>
            Char.GetNumericValue(card) != -1 ? (int)Char.GetNumericValue(card) :
            card == 'A' ? 11 : 10;

        public static int RawValue(List<char> hand) => hand.Sum(c => CardValue(c));

        public static bool HasSoftAce(List<char> hand) => RawValue(hand) < 22 && hand.Contains('A');

        public static int AdjustedValue(List<char> hand) =>
            RawValue(hand) == 21 && hand.Count() == 2 ? 100 : //BlackJack
              RawValue(hand) < 22 ? RawValue(hand) :
                HasSoftAce(hand) ? RawValue(hand) - 10 : //Convert the soft Ace from 11 to 1
                    0; //Bust

        public static char DealCard(FRandom rg) => Cards[rg.InRange(0, 13)];

        public static List<char> DrawCard(List<char> toHand, FRandom rg) => toHand.Append(DealCard(rg)).ToList();

        public static bool DealerDrawDecision(List<char> hand) => RawValue(hand) < 17 || (AdjustedValue(hand) > 0 && AdjustedValue(hand) < 17);

        public static (List<char>, FRandom) DrawOneDealerHand(List<char> hand, FRandom rnd) =>
            DrawOneHand(hand, rnd, DealerDrawDecision);
            //DealerDrawDecision(hand) ? DrawOneDealerHand(DrawCard(hand, rnd), rnd.Next()) : (hand, rnd);

        public static (List<char>, FRandom) DrawOneHand(List<char> hand, FRandom rnd, Func<List<char>, bool> drawDecision) =>
            drawDecision(hand) ? DrawOneHand(DrawCard(hand, rnd), rnd.Next(), drawDecision) : (hand, rnd);

        public static List<int> AllPossibleOutcomes() => new List<int> { 0, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 100 };

        public static List<(int, double)> ProbabilitiesOfPlayerOutcomesFromDraw(List<char> hand) =>
            Cards.Select(c => AdjustedValue(hand.Append(c).ToList())).GroupBy(oc => oc).Select(g => (g.Key, ((double) g.Count())/13)).ToList();

        //Payoff (as a ratio of stake) will be: -1 (lose), 0 (draw), 1 (win), 1.5 win with BJ
        public static double Payoff(int adjustedPlayer, int adjustedDealer) =>
            adjustedPlayer == 0 ? -1 :
                adjustedPlayer == 100 && adjustedDealer != 100 ? 1.5 :
                    adjustedPlayer > adjustedDealer ? 1 :
                        adjustedPlayer == adjustedDealer ? 0 :
                            -1;

        public static List<(int, double)> ProbabilitiesOfPlayerOutcomesFromStick(int adjustedHandValue) =>
            AllPossibleOutcomes().Select(oc => oc == adjustedHandValue ? (oc, 1.0) : (oc, 0.0)).ToList();

        public static double ExpectedValueOfFromDraw(List<char> hand, char dealerFaceCard) =>
            ExpectedValueFrom(ProbabilitiesOfPlayerOutcomesFromDraw(hand), ProbabilitiesOfDealerOutcomes(dealerFaceCard));

        public static double ExpectedValueFrom(List<(int, double)> playerOutcomeProbabilities, List<(int, double)> dealerOutcomeProbabilities) =>
            playerOutcomeProbabilities.Sum(p => dealerOutcomeProbabilities.Sum(d => Payoff(p.Item1, d.Item1) * p.Item2 * p.Item2));

        public static double ExpectedValueFromStick(List<char> hand, char dealerFaceCard) =>
             ExpectedValueFrom(ProbabilitiesOfPlayerOutcomesFromStick(AdjustedValue(hand)), ProbabilitiesOfDealerOutcomes(dealerFaceCard));

        public static bool CantBustFromOneMoreCard(List<char> hand) => hand.Count < 5 && AdjustedValue(hand) < 12;

        public static bool PlayerDecision1(List<char> hand, char dealerFaceCard) =>
           CantBustFromOneMoreCard(hand) ? true :
                HasSoftAce(hand) && AdjustedValue(hand) < 18 ? true :
                    AdjustedValue(hand) < 15;

        public static bool PlayerDecision2(List<char> hand, char dealerFaceCard) =>
             CantBustFromOneMoreCard(hand) ? true :
                HasSoftAce(hand) && AdjustedValue(hand) < 18 ? true :
                    AdjustedValue(hand) <= CardValue(dealerFaceCard) + 10;

        public static bool OptimalPlayerDecision(List<char> hand, char dealerFaceCard) =>
            hand.Count < 5 && AdjustedValue(hand) < 12 ? true :
                ExpectedValueOfFromDraw(hand, dealerFaceCard) >
                    ExpectedValueFromStick(hand, dealerFaceCard);

        public static char DealCard(int rand) => Cards[rand];


        //Examples of using aggregation with a random number.
        //Enumerable.Range(0, 10).Aggregate(("", FRandom.Seed()), (a, n) => (a.Item1 + n.ToString(), a.Item2.Next()));

        //Enumerable.Range(0, 10).Aggregate((new List<FRandom>(), FRandom.Seed()), (a, n) => (a.Item1.Append(a.Item2).ToList(), a.Item2.Next()));


        public static List<(int, double)> ProbabilitiesOfDealerOutcomes(char faceCard) =>
            dealerOutcomes.Where(el => el.Item1 == faceCard).Select(el => el.Item2).First();

        // Dealer outcomes (based on a million games for each facecard)
        static List<(char, List<(int, double)>)> dealerOutcomes = new List<(char, List<(int, double)>)> {
            ('2', new List<(int, double)> {(0,0.413),(17,0.121),(18,0.125),(19,0.119),(20,0.114),(21,0.108),} ),
            ('3', new List<(int, double)> {(0,0.432),(17,0.116),(18,0.121),(19,0.115),(20,0.111),(21,0.105),} ),
            ('4', new List<(int, double)> {(0,0.45),(17,0.113),(18,0.117),(19,0.112),(20,0.107),(21,0.102),} ),
            ('5', new List<(int, double)> {(0,0.472),(17,0.103),(18,0.113),(19,0.109),(20,0.104),(21,0.099),} ),
            ('6', new List<(int, double)> {(0,0.468),(17,0.135),(18,0.103),(19,0.103),(20,0.098),(21,0.094),} ),
            ('7', new List<(int, double)> {(0,0.288),(17,0.355),(18,0.135),(19,0.075),(20,0.075),(21,0.071),} ),
            ('8', new List<(int, double)> {(0,0.269),(17,0.117),(18,0.357),(19,0.126),(20,0.066),(21,0.066),} ),
            ('9', new List<(int, double)> {(0,0.25),(17,0.109),(18,0.117),(19,0.348),(20,0.117),(21,0.058),} ),
            ('T', new List<(int, double)> {(0,0.23),(17,0.102),(18,0.109),(19,0.109),(20,0.34),(21,0.032),(100,0.077),} ),
            ('J', new List<(int, double)> {(0,0.229),(17,0.102),(18,0.109),(19,0.109),(20,0.341),(21,0.033),(100,0.077),} ),
            ('Q', new List<(int, double)> {(0,0.23),(17,0.102),(18,0.109),(19,0.11),(20,0.341),(21,0.032),(100,0.077),} ),
            ('K', new List<(int, double)> {(0,0.23),(17,0.102),(18,0.11),(19,0.109),(20,0.34),(21,0.032),(100,0.077),} ),
            ('A', new List<(int, double)> {(0,0.266),(17,0.096),(18,0.102),(19,0.102),(20,0.101),(21,0.025),(100,0.308),} ),
        };

        //Not used at present - the tuple syntax will work better for Python, too.
        static Dictionary<char, Dictionary<int, double>> dealerOutcomes2 = new Dictionary<char, Dictionary<int, double>>() {
            {'2', new Dictionary<int, double>() { {0, 0.413}, {17, 0.121}, {18, 0.125}, {19, 0.119}, {20, 0.114}, {21, 0.108} } } ,
            {'3', new Dictionary<int, double>() { {0, 0.432}, {17, 0.116}, {18, 0.121}, {19, 0.115}, {20, 0.111}, {21, 0.105} } } ,
            {'4', new Dictionary<int, double>() { {0, 0.45}, {17, 0.113}, {18, 0.117}, {19, 0.112}, {20, 0.107}, {21, 0.102} } },
            {'5', new Dictionary<int, double>() { {0, 0.472}, {17, 0.103}, {18, 0.113}, {19, 0.109}, {20, 0.104}, {21, 0.099} } },
            {'6', new Dictionary<int, double>() { {0, 0.468}, {17, 0.135}, {18, 0.103}, {19, 0.103}, {20, 0.098}, {21, 0.094} } },
            {'7', new Dictionary<int, double>() { {0, 0.288}, {17, 0.355}, {18, 0.135}, {19, 0.075}, {20, 0.075}, { 21, 0.071 } } },
            {'8', new Dictionary<int, double>() { {0, 0.269}, {17, 0.117}, {18, 0.357}, {19, 0.126}, {20, 0.066}, {21, 0.066} } },
            {'9', new Dictionary<int, double>() { {0, 0.25}, {17, 0.109}, {18, 0.117}, {19, 0.348}, {20, 0.117}, {21, 0.058} } },
            {'T', new Dictionary<int, double>() { {0, 0.23}, {17, 0.102}, {18, 0.109}, {19, 0.109}, {20, 0.34}, {21, 0.032}, {100, 0.077} } },
            {'J', new Dictionary<int, double>() { {0, 0.229}, {17, 0.102}, {18, 0.109}, {19, 0.109}, {20, 0.341}, {21, 0.033}, {100, 0.077} } },
            {'Q', new Dictionary<int, double>() { {0, 0.23}, {17, 0.102}, {18, 0.109}, {19, 0.11}, {20, 0.341}, {21, 0.032}, {100, 0.077} } },
            {'K', new Dictionary<int, double>() { {0, 0.23}, {17, 0.102}, {18, 0.11}, {19, 0.109}, {20, 0.34}, {21, 0.032}, {100, 0.077} } },
            {'A', new Dictionary<int, double>() { {0, 0.266}, {17, 0.096}, {18, 0.102}, {19, 0.102}, {20, 0.101}, {21, 0.025}, {100, 0.308} } }
        };

        public static List<char> PlayDealerHand(FRandom seed) => throw new NotImplementedException();

        public static List<char> PlayPlayerHand(char dealerFaceCard, List<char> pack, Func<List<char>, char, bool> playerStrategy) => throw new NotImplementedException();

        public static double SimulateGames(int numberOfGames, Func<List<char>, char, bool> playerStrategy) => throw new NotImplementedException();

        public static List<double> SimulateGames(int numberOfGames, List<Func<List<char>, char, bool>> playerStrategys) => throw new NotImplementedException();

        public static List<(int, double)> CalculateDealerOutcomeProbabilities(int numberOfGames) => throw new NotImplementedException();

        //public static double PayoffFrom(int numberOfGames, Func<List<char>, char, bool> playerStrategy, FRandom seed) => 
        //    Enumerable.Range(0, numberOfGames).Aggregate( PayoffFromOneGame 
             
        public static double PlayerPayoffFromOneGame(Func<List<char>, char, bool> playerStrategy, FRandom seed) =>
            //PlayDealerHand(seed)
            //get face card
            //Skip seed by length of dealer hand
            //Pla
    }
}