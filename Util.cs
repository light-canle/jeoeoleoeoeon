using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jeoeoleoeoeon
{
    internal class Util
    {
        public static string MakeString(string input, int rs)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                result.Append(ValueAppend(Convert.ToInt32(input[i]), rs + i));
            }
            result.Append(MakeInstruction(true, 12, rs, input.Length, 0));
            return result.ToString();
        }

        public static string ValueAppend(int value, int rs)
        {
            List<int> imm3x = new List<int> { 0, 1, 4, 13, 40, 121, 364, 1093, 3280, 9841, 29524, 88573, 265720, 797161, 2391484, 7174453, 21523360, 64570081, 193710244, 581130733, 1743392200 };

            StringBuilder result = new StringBuilder();
            if (value == 10) { result.Append(MakeInstruction(false, 4, rs, 0, 0)); }
            else if (value == 32) { result.Append(MakeInstruction(false, 5, rs, 0, 0)); }
            else if (value < 13) { result.Append(MakeInstruction(false, 3, rs, value, 0)); }
            else
            {
                for (int i = 20; i >= 0; i--)
                {
                    if (value >= imm3x[i])
                    {
                        result.Append(MakeInstruction(false, 2, rs, i, 0));
                        value -= imm3x[i];
                        break;
                    }
                }
                while (value >= 13)
                {
                    for (int i = 20; i >= 0; i--)
                    {
                        if (value >= imm3x[i]) 
                        {
                            result.Append(MakeInstruction(false, 20, rs, i, 0));
                            value -= imm3x[i];
                            break;
                        }
                    }
                }
                if (value != 0) { result.Append(MakeInstruction(false, 21, rs, value, 0)); }
            }
            return result.ToString();
        }

        public static string MakeInstruction(bool special, int op, int rs, int imm, int rd)
        {
            StringBuilder result = new StringBuilder();
            if (special) { result.Append("앗! "); }
            string rdPart = op switch
            {
                23 or 27 or 32 or 35 or 38 => (special) ? "" : new string('아', rd / 10) + '앗' + new string('.', rd % 10),
                >= 40 and <= 45 => (special) ? "" : new string('아', rd / 10) + '앗' + new string('.', rd % 10),
                >= 50 and <= 65 => (special) ? "" : new string('아', rd / 10) + '앗' + new string('.', rd % 10),
                72 or 73 => (special) ? "" : new string('아', rd / 10) + '앗' + new string('.', rd % 10),
                _ => ""
            };
            string opPart = new string('어', op / 10) + new string('.', op % 10);
            string rsPart = new string('어', rs / 10) + new string('.', rs % 10);
            string immPart = new string('.', imm);
            if (rdPart.Length > 0) { result.Append($"{rdPart} ");  }
            result.Append($"저{opPart}러{rsPart}언{immPart}");
            result.AppendLine();
            return result.ToString();
        }
    }
}
