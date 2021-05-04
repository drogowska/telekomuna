using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman
{
    class asciiTree
    {
        //z podanej niżej strony odczytano odpowiadające dla każdego znaku ASCI prawdopodobieństwa występowania w języku angielskim
        //jeśli podany tekst, który ma zostać skompresowany, zawiera znaki nie znajdujące się poniżej, funkcja kompresująca skończy się nie poprawnie
        //https://reusablesec.blogspot.com/2009/05/character-frequency-analysis-info.html?fbclid=IwAR1kdsz8gvUg6Swnp7clMEgUiFwRKUKVn2RR3h3tP1dQqXhtbgXWB5tkHl4
        public static Dictionary<char, int> initializeDictionary()
        {
            Dictionary<char, int> asciiFreq = new Dictionary<char, int>();              //dla każdej pary znak, częstość występowania, znak musi być unikalny (klucz)
            asciiFreq.Add('a', 7537);                                                   //poniżej zostają dodane pary klucz, częstość do kolekcji Dictionary
            asciiFreq.Add('e', 7092);
            asciiFreq.Add('o', 5170);
            asciiFreq.Add('r', 4960);
            asciiFreq.Add('i', 4697);
            asciiFreq.Add('s', 4610);
            asciiFreq.Add('n', 4568);
            asciiFreq.Add('1', 4350);
            asciiFreq.Add('t', 3873);
            asciiFreq.Add('l', 3777);
            asciiFreq.Add('2', 3123);
            asciiFreq.Add('m', 2999);
            asciiFreq.Add('d', 2764);
            asciiFreq.Add('0', 2743);
            asciiFreq.Add('c', 2572);
            asciiFreq.Add('p', 2455);
            asciiFreq.Add('3', 2433);
            asciiFreq.Add('h', 2413);
            asciiFreq.Add('b', 2291);
            asciiFreq.Add('u', 2101);
            asciiFreq.Add('k', 1968);
            asciiFreq.Add('4', 1942);
            asciiFreq.Add('5', 1885);
            asciiFreq.Add('g', 1853);
            asciiFreq.Add('9', 1795);
            asciiFreq.Add('6', 1756);
            asciiFreq.Add('8', 1662);
            asciiFreq.Add('7', 1621);
            asciiFreq.Add('y', 1524);
            asciiFreq.Add('f', 1247);
            asciiFreq.Add('w', 1244);
            asciiFreq.Add('j', 836);
            asciiFreq.Add('v', 833);
            asciiFreq.Add('z', 632);
            asciiFreq.Add('x', 573);
            asciiFreq.Add('q', 346);
            asciiFreq.Add('A', 130);
            asciiFreq.Add('S', 108);
            asciiFreq.Add('E', 97);
            asciiFreq.Add('R', 84);
            asciiFreq.Add('B', 80);
            asciiFreq.Add('T', 80);
            asciiFreq.Add('M', 78);
            asciiFreq.Add('L', 77);
            asciiFreq.Add('N', 74);
            asciiFreq.Add('P', 73);
            asciiFreq.Add('O', 72);
            asciiFreq.Add('I', 70);
            asciiFreq.Add('D', 69);
            asciiFreq.Add('C', 66);
            asciiFreq.Add('H', 54);
            asciiFreq.Add('G', 49);
            asciiFreq.Add('K', 46);
            asciiFreq.Add('F', 41);
            asciiFreq.Add('J', 36);
            asciiFreq.Add('U', 35);
            asciiFreq.Add('W', 32);
            asciiFreq.Add('.', 31);
            asciiFreq.Add('!', 30);
            asciiFreq.Add('Y', 25);
            asciiFreq.Add('*', 24);
            asciiFreq.Add('@', 24);
            asciiFreq.Add('V', 23);
            asciiFreq.Add('-', 20);
            asciiFreq.Add('Z', 17);
            asciiFreq.Add('Q', 15);
            asciiFreq.Add('X', 14);
            asciiFreq.Add('_', 12);
            asciiFreq.Add('$', 9);
            asciiFreq.Add('#', 8);
            asciiFreq.Add(',', 3);
            asciiFreq.Add('/', 3);
            asciiFreq.Add('+', 2);
            asciiFreq.Add('?', 2);
            asciiFreq.Add(';', 2);
            asciiFreq.Add('^', 1);
            asciiFreq.Add(' ', 70000);
            asciiFreq.Add('%', 1);
            asciiFreq.Add('~', 1);
            asciiFreq.Add('=', 1);
            asciiFreq.Add('&', 1);
            asciiFreq.Add('\\', 1);
            asciiFreq.Add(')', 1);
            asciiFreq.Add(']', 1);
            asciiFreq.Add('[', 1);
            asciiFreq.Add(':', 1);
            asciiFreq.Add('<', 1);
            asciiFreq.Add('(', 1);
            asciiFreq.Add('>', 1);
            asciiFreq.Add('"', 1);
            asciiFreq.Add('|', 1);
            asciiFreq.Add('{', 1);
            asciiFreq.Add('}', 1);
            asciiFreq.Add('\'', 1);
            return asciiFreq;
        }
    }
}
