using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day07
{
    [TestFixture]
    public class Day07 : BaseAocV2
    {
        public class Hand : IComparable<Hand>
        {
            public enum HandType
            {
                HighCard,
                OnePair,
                TwoPair,
                ThreeOfAKind,
                FullHouse,
                FourOfAKind,
                FiveOfAKind
            }

            public int Bid { get; init; }
            public string Cards { get; init; }

            public HandType Type { get; init; }

            private string _sortableCards;

            public Hand(string input, bool joker = false)
            {
                var ss = input.Split(' ');
                Cards = ss[0];
                Bid = int.Parse(ss[1]);

                if (joker)
                {
                    _sortableCards = Cards
                        .Replace('J', 'a')
                        .Replace('2', 'b')
                        .Replace('3', 'c')
                        .Replace('4', 'd')
                        .Replace('5', 'e')
                        .Replace('6', 'f')
                        .Replace('7', 'g')
                        .Replace('8', 'h')
                        .Replace('9', 'i')
                        .Replace('T', 'j')
                        .Replace('Q', 'k')
                        .Replace('K', 'l')
                        .Replace('A', 'm');

                    var jokerCount = Cards.Count(c => c == 'J');

                    var csd = Cards.Where(c => c != 'J').GroupBy(c => c).Select(g => (g.Key, g.Count())).OrderByDescending(v => v.Item2).ToList();
                    if(!csd.Any())
                    {
                        csd.Add(('z', 0));
                    }

                    if (csd[0].Item2 + jokerCount == 5)
                    {
                        Type = HandType.FiveOfAKind;
                    }
                    else if (csd[0].Item2 + jokerCount == 4)
                    {
                        Type = HandType.FourOfAKind;
                    }
                    else if (csd[0].Item2 + jokerCount == 3)
                    {
                        if (csd[1].Item2 == 2)
                        {
                            Type = HandType.FullHouse;
                        }
                        else
                        {
                            Type = HandType.ThreeOfAKind;
                        }
                    }
                    else if (csd[0].Item2 + jokerCount == 2)
                    {
                        if (csd[1].Item2 == 2)
                        {
                            Type = HandType.TwoPair;
                        }
                        else
                        {
                            Type = HandType.OnePair;
                        }
                    }
                    else
                    {
                        Type = HandType.HighCard;
                    }
                }
                else
                {

                    _sortableCards = Cards
                        .Replace('2', 'a')
                        .Replace('3', 'b')
                        .Replace('4', 'c')
                        .Replace('5', 'd')
                        .Replace('6', 'e')
                        .Replace('7', 'f')
                        .Replace('8', 'g')
                        .Replace('9', 'h')
                        .Replace('T', 'i')
                        .Replace('J', 'j')
                        .Replace('Q', 'k')
                        .Replace('K', 'l')
                        .Replace('A', 'm');

                    var cs = Cards.OrderByDescending(c => Cards.Count(d => c == d)).ThenBy(c => c).ToList();

                    if (cs[0] == cs[4])
                    {
                        Type = HandType.FiveOfAKind;
                    }
                    else if (cs[0] == cs[3])
                    {
                        Type = HandType.FourOfAKind;
                    }
                    else if (cs[0] == cs[2])
                    {
                        if (cs[3] == cs[4])
                        {
                            Type = HandType.FullHouse;
                        }
                        else
                        {
                            Type = HandType.ThreeOfAKind;
                        }
                    }
                    else if (cs[0] == cs[1])
                    {
                        if (cs[2] == cs[3])
                        {
                            Type = HandType.TwoPair;
                        }
                        else
                        {
                            Type = HandType.OnePair;
                        }
                    }
                    else
                    {
                        Type = HandType.HighCard;
                    }
                }
            }

            public int CompareTo(Hand? other)
            {
                if(!(other is Hand)) return 1;

                if(other.Type == Type)
                {
                    return _sortableCards.CompareTo(other._sortableCards);
                }
                
                return Type.CompareTo(other.Type);
            }
        }

        [TestCase("JJJJJ 435", true, Hand.HandType.FiveOfAKind)]
        [TestCase("JJJJ8 154", true, Hand.HandType.FiveOfAKind)]
        [TestCase("7JJT7 1", true, Hand.HandType.FourOfAKind)]
        [TestCase("T74JJ 498", true, Hand.HandType.ThreeOfAKind)]
        [TestCase("T745J 498", true, Hand.HandType.OnePair)]
        [TestCase("3344T 498", true, Hand.HandType.TwoPair)]
        [TestCase("JAJAT 498", true, Hand.HandType.FourOfAKind)]
        [TestCase("JATAT 498", true, Hand.HandType.FullHouse)]
        public void HandTest(string input, bool joker, Hand.HandType expectedHandType)
        {
            var hand = new Hand(input, joker);

            Assert.That(hand.Type, Is.EqualTo(expectedHandType));
        }

        public override string PartOne(List<string> lines)
        {
            var hands = lines.Select(l => new Hand(l)).ToList();

            hands.Sort();

            var score = 0;

            for (int i = 0; i < hands.Count; i++)
            {
                score += (i + 1) * hands[i].Bid;
            }

            return score.ToString();
        }

        public override string PartTwo(List<string> lines)
        {
            var hands = lines.Select(l => new Hand(l, true)).ToList();

            hands.Sort();

            long score = 0;

            for (int i = 0; i < hands.Count; i++)
            {
                score += (i + 1) * hands[i].Bid;
            }

            return score.ToString();
        }
    }
}
