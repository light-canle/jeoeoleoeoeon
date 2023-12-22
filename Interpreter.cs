using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jeoeoleoeoeon
{
    internal class Interpreter
    {
        public class Insturction
        {
            public bool isSpecial { get; private set; }
            public int op {  get; private set; }
            public int rs { get; private set; }
            public int rd { get; private set; }
            public int imm { get; private set; }

            public Insturction(bool isSpecial, int op, int rs, int imm, int rd)
            {
                this.isSpecial = isSpecial;
                this.op = op;
                this.rs = rs;
                this.rd = rd;
                this.imm = imm;
            }
        }
        public static int[] memory = new int[16384];
        public static Dictionary<int, int> jumpLabels;
        public static List<Insturction?> instList;
        public static int pointer = 0;
        public static int PC = 1;
        public static void Execute(string fileName)
        {
            jumpLabels = new Dictionary<int, int>();
            instList = new List<Insturction?>();
            using (StreamReader sr = new StreamReader(new FileStream(fileName, FileMode.Open)))
            {
                int lineNum = 0;
                while (!sr.EndOfStream)
                {
                    lineNum++;
                    string? line = sr.ReadLine();
                    if (line == null) { break; }

                    // 1. 줄의 시작점 분석(주석인지 판단)
                    if (line[0] != '아' && line[0] != '저' && line[0] != '앗')
                    {
                        instList.Add(null);
                        continue;
                    }
                    // 2. 분기 레이블인지 판단
                    // 점프 레이블과 그 번호를 구함
                    if (line.Substring(0, 2) == "저런")
                    {
                        string temp = line.Substring(2);
                        jumpLabels.Add(temp.Length, lineNum);
                        instList.Add(null);
                        continue;
                    }
                    // 3. op번호 분석

                    // '앗!'으로 시작하는지 판단
                    bool isSpecial = false;
                    if (line.Substring(0, 2) == "앗!") isSpecial = true;
                    // '저'와 '러' 사이 문자열 추출
                    int pos1 = line.IndexOf('저');
                    int pos2 = line.IndexOf('러');
                    if (pos1 == -1 ||  pos2 == -1) throw new InvalidDataException($"줄 {lineNum} : \'저\', \'러\'가 해당 줄에 없습니다.");
                    string opPart = line.Substring(pos1 + 1, pos2 - pos1 - 1);
                    // 이 문자열에서 op번호를 구함
                    int op = 0;
                    foreach (char c in opPart)
                    {
                        switch (c)
                        {
                            case '어': op += 10; break;
                            case '.': op += 1; break;
                            default: throw new InvalidDataException($"줄 {lineNum} : op번호를 지정하는 \'저\'와 \'러\'사이에 다른 문자 '{c}'(이)가 들어있습니다.");
                        }
                    }
                    // 4. 타입에 따라 whitespace와 '러', '언' 기준으로 rs, rd, imm을 구함
                    // rs - half ~ full
                    int pos3 = line.IndexOf('언');
                    int rs = 0;
                    if (op != 24 || !isSpecial)
                    {
                        string rsPart = line.Substring(pos2 + 1, pos3 - pos2 - 1);
                        foreach (char c in rsPart)
                        {
                            switch (c)
                            {
                                case '어': rs += 10; break;
                                case '.': rs += 1; break;
                                default: throw new InvalidDataException($"줄 {lineNum} : rs번호를 지정하는 \'러\'와 \'언\'사이에 다른 문자 '{c}'(이)가 들어있습니다.");
                            }
                        }
                    }
                    // imm - semi-full, full
                    string immPart = line.Substring(pos3 + 1);
                    int imm = immPart.Length;
                    // rd - full 타입
                    int rd = 0;
                    if (!isSpecial && line.Contains('앗'))
                    {
                        int pos0 = line.IndexOf('앗');
                        rd += pos0 * 10;
                        string rdPart = line.Substring(pos0 + 1, pos1 - pos0 - 2);
                        rd += rdPart.Length;
                    }
                    // Console.WriteLine($"op : {(isSpecial ? "s": "") + op}, rs : {rs}, imm : {imm}, rd : {rd}");
                    // 5. 리스트에 명령어 추가
                    instList.Add(new Insturction(isSpecial, op, rs, imm, rd));
                }

            }
            int total = instList.Count;
            PC = 0;
            while (PC < total)
            {
                PC++;
                var inst = instList[PC - 1];
                if (inst == null) continue;
                InstExecute(inst.isSpecial, inst.op, inst.rs, inst.imm, inst.rd, PC);
            }
        }

        public static void InstExecute(bool special, int op, int rs, int imm, int rd, int lineNum)
        {
            int input, imm3x;
            StringBuilder output = new StringBuilder();
            if (special)
            {
                switch (op)
                {
                    // in - half, 사용자로부터 수를 입력받아 rs번 변수에 넣는다.
                    case 1:
                        input = int.Parse(Console.ReadLine()!);
                        memory[rs] = input;
                        break;
                    // inchr - half, 사용자로부터 문자 1개를 입력받아 UTF-8값을 rs번 변수에 넣는다.
                    case 2:
                        input = Console.Read();
                        memory[rs] = input;
                        break;
                    // inchrmul - semi-full, 사용자로부터 문자 imm개를 입력받아 rs번 변수부터 순차적으로 넣는다.
                    case 3:
                        for (int i = 0; i < imm; i++)
                        {
                            input = Console.Read();
                            memory[rs + i] = input;
                        }
                        break;
                    // out - half, rs에 들어 있는 값을 수 그대로 출력한다.
                    case 10:
                        Console.Write(memory[rs]);
                        break;
                    // outchr - half, rs에 들어 있는 값을 UTF-8 문자로 출력한다.
                    case 11:
                        Console.Write(Convert.ToChar(memory[rs]));
                        break;
                    // outchrmul - semi-full, rs부터 imm개의 변수에 들어 있는 값을 UTF-8 문자로 출력한다.
                    case 12:
                        output.Clear();
                        for (int i = 0; i < imm; i++)
                        {
                            output.Append(Convert.ToChar(memory[rs + i]));
                        }
                        Console.Write(output);
                        break;
                    // pleft - semi-full, 포인터의 위치를 왼쪽으로 imm[3x+1]칸 만큼 이동시킨다.	
                    case 20:
                        imm3x = IMM3x(imm);
                        pointer -= imm3x;
                        if (!ValidPointer()) { throw new IndexOutOfRangeException($"줄 {lineNum } : 포인터가 범위를 벗어났습니다. 벗어난 인덱스 : {pointer}"); }
                        break;
                    // plefti -	semi-full, 포인터의 위치를 왼쪽으로 imm칸 만큼 이동시킨다.
                    case 21:
                        pointer -= imm;
                        if (!ValidPointer()) { throw new IndexOutOfRangeException($"줄 {lineNum} : 포인터가 범위를 벗어났습니다. 벗어난 인덱스 : {pointer}"); }
                        break;
                    // pright - semi-full, 포인터의 위치를 오른쪽으로 imm[3x+1]칸 만큼 이동시킨다.	
                    case 22:
                        imm3x = IMM3x(imm);
                        pointer += imm3x;
                        if (!ValidPointer()) { throw new IndexOutOfRangeException($"줄 {lineNum} : 포인터가 범위를 벗어났습니다. 벗어난 인덱스 : {pointer}"); }
                        break;
                    // prighti - semi-full, 포인터의 위치를 오른쪽으로 imm칸 만큼 이동시킨다.
                    case 23:
                        pointer += imm;
                        if (!ValidPointer()) { throw new IndexOutOfRangeException($"줄 {lineNum} : 포인터가 범위를 벗어났습니다. 벗어난 인덱스 : {pointer}"); }
                        break;
                    // pset0 - one, 포인터가 0번 변수를 가르키게 한다.
                    case 24:
                        pointer = 0;
                        break;
                    // pputv - half, 포인터가 가르키는 변수에 rs번 변수의 값을 대입한다.
                    case 25:
                        memory[pointer] = memory[rs];
                        break;
                    // pputi - semi-full, 포인터가 가르키는 변수에 imm[3x+1]을 대입한다.
                    case 26:
                        memory[pointer] = IMM3x(imm);
                        break;
                    // pput0 - one, 포인터가 가르키는 변수에 0을 대입한다.
                    case 27:
                        memory[pointer] = 0;
                        break;
                    // pget - half, rs번 변수에 포인터가 가르키는 변수의 값을 대입한다.
                    case 28:
                        memory[rs] = memory[pointer];
                        break;
                    default: throw new InvalidOperationException($"줄 {lineNum} : 유효하지 않은 명령어 코드 : s{op}");
                }
            }
            else
            {
                switch (op)
                {
                    // putv - semi-full, rs번 변수에 imm번 변수의 값을 대입한다.
                    case 1:
                        memory[rs] = memory[imm];
                        break;
                    // puti - semi-full, rs번 변수에 imm[3x+1]의 값을 대입한다.
                    case 2:
                        memory[rs] = IMM3x(imm);
                        break;
                    // puti1 - semi-full, rs번 변수에 imm의 값을 대입한다.
                    case 3:
                        memory[rs] = imm;
                        break;
                    // put10 - half, rs번 변수에 10을 대입한다. (줄 바꿈의 UTF-8 값)
                    case 4:
                        memory[rs] = 10;
                        break;
                    // put32 - half, rs번 변수에 32를 대입한다. (띄어쓰기의 UTF-8 값)
                    case 5:
                        memory[rs] = 32;
                        break;
                    // ahhee - half, rs번 변수에 44,032를 대입한다. ('가'의 UTF-8 값)
                    case 6:
                        memory[rs] = 44_032;
                        break;
                    // mul3x1 - semi-full, val[rs] = val[rs] * 3 + 1를 imm번 반복한다.
                    case 10:
                        checked
                        {
                            for (int i = 0; i < imm; i++)
                            {
                                memory[rs] = memory[rs] * 3 + 1;
                            }
                        }
                        break;
                    // div2 - semi-full, val[rs] = val[rs] / 2 를 imm번 반복한다.(소수점 버림)
                    case 11:
                        checked
                        {
                            for (int i = 0; i < imm; i++)
                            {
                                memory[rs] /= 2;
                            }
                        }
                        break;
                    // addi - semi-full, rs번 변수 값을 imm[3x+1] 값만큼 증가시킨다.(val[rs] += imm[3x+1])
                    case 20:
                        checked
                        {
                            memory[rs] += IMM3x(imm);
                        }
                        break;
                    // addi1 - semi-full, rs번 변수 값을 imm만큼 증가시킨다.(val[rs] += imm)
                    case 21:
                        checked
                        {
                            memory[rs] += imm;
                        }
                        break;
                    // addv - semi-full, rs번 변수 값 imm번 변수의 값만큼 증가시킨다.(val[rs] += val[imm])
                    case 22:
                        checked
                        {
                            memory[rs] += memory[imm];
                        }
                        break;
                    // add2v - full, (rs번 변수 값 + imm번 변수의 값)을 rd변 변수에 저장한다.(val[rd] = val[rs] + val[imm])
                    case 23:
                        checked
                        {
                            memory[rd] = memory[rs] + memory[imm];
                        }
                        break;
                    // subi - semi-full, rs번 변수 값을 imm[3x+1] 값만큼 감소시킨다.(val[rs] -= imm[3x+1])
                    case 24:
                        checked
                        {
                            memory[rs] -= IMM3x(imm);
                        }
                        break;
                    // subi1 - semi-full, rs번 변수 값을 imm만큼 감소시킨다.(val[rs] -= imm)
                    case 25:
                        checked
                        {
                            memory[rs] -= imm;
                        }
                        break;
                    // subv - semi-full, rs번 변수 값을 imm번 변수의 값만큼 감소시킨다.(val[rs] -= val[imm])
                    case 26:
                        checked
                        {
                            memory[rs] -= memory[imm];
                        }
                        break;
                    // sub2v - full, (rs번 변수 값 - imm번 변수의 값)을 rd변 변수에 저장한다.(val[rd] = val[rs] - val[imm])
                    case 27:
                        checked
                        {
                            memory[rd] = memory[rs] - memory[imm];
                        }
                        break;
                    // mul - semi-full, rs번 변수 값에 imm의 값을 곱한다.(val[rs] *= imm)
                    case 30:
                        checked
                        {
                            memory[rs] *= imm;
                        }
                        break;
                    // mulv - semi-full, rs번 변수 값에 imm번 변수의 값을 곱한 것을 구한 뒤 rs번 변수에 저장한다.(val[rs] *= val[imm])
                    case 31:
                        checked
                        {
                            memory[rs] *= memory[imm];
                        }
                        break;
                    // mul2v - full, (rs번 변수 값 x imm번 변수의 값)을 rd 변수에 저장한다.(val[rd] = val[rs] * val[imm])
                    case 32:
                        checked
                        {
                            memory[rd] = memory[rs] * memory[imm];
                        }
                        break;
                    // div - semi-full, rs번 변수 값에 imm의 값을 나눈다.(val[rs] /= imm)
                    case 33:
                        checked
                        {
                            memory[rs] /= imm;
                        }
                        break;
                    // divv - semi-full, rs번 변수 값에 imm번 변수의 값을 나눈 값을 구한 뒤 rs번 변수에 저장한다.(val[rs] /= val[imm])
                    case 34:
                        checked
                        {
                            memory[rs] /= memory[imm];
                        }
                        break;
                    // div2v - full, (rs번 변수 값 / imm번 변수의 값)을 rd 변수에 저장한다.(val[rd] = val[rs] / val[imm])
                    case 35:
                        checked
                        {
                            memory[rd] = memory[rs] / memory[imm];
                        }
                        break;
                    // mod - semi-full, rs번 변수 값에 imm의 값을 나눈 나머지를 구한다.(val[rs] %= imm)
                    case 36:
                        checked
                        {
                            memory[rs] %= imm;
                        }
                        break;
                    // modv - semi-full, rs번 변수 값에 imm번 변수의 값을 나눈 나머지를 구한 뒤 rs번 변수에 저장한다.(val[rs] %= val[imm])
                    case 37:
                        checked
                        {
                            memory[rs] %= memory[imm];
                        }
                        break;
                    // mod2v - full, (rs번 변수 값 % imm번 변수의 값)을 rd 변수에 저장한다.(val[rd] = val[rs] % val[imm])
                    case 38:
                        checked
                        {
                            memory[rd] = memory[rs] % memory[imm];
                        }
                        break;
                    // andv - full, rs번 변수 값과 imm번 변수의 값을 비트 and 연산한 결과를 rd번 변수에 저장한다.
                    case 40:
                        memory[rd] = memory[rs] & memory[imm];
                        break;
                    // andi - full, rs번 변수 값에 imm의 값을 비트 and 연산한 결과를 rd번 변수에 저장한다.
                    case 41:
                        memory[rd] = memory[rs] & imm;
                        break;
                    // orv - full, rs번 변수 값과 imm번 변수의 값을 비트 or 연산한 결과를 rd번 변수에 저장한다.
                    case 42:
                        memory[rd] = memory[rs] | memory[imm];
                        break;
                    // ori - full, 	rs번 변수 값에 imm의 값을 비트 or 연산한 결과를 rd번 변수에 저장한다.
                    case 43:
                        memory[rd] = memory[rs] | imm;
                        break;
                    // xorv - full, rs번 변수 값과 imm번 변수의 값을 비트 xor 연산한 결과를 rd번 변수에 저장한다.
                    case 44:
                        memory[rd] = memory[rs] ^ memory[imm];
                        break;
                    // xori - full, rs번 변수 값에 imm의 값을 비트 xor 연산한 결과를 rd번 변수에 저장한다.
                    case 45:
                        memory[rd] = memory[rs] ^ imm;
                        break;
                    // not - half, rs번 변수 값에 비트 not 연산을 수행한 뒤 그 변수에 저장한다.
                    case 46:
                        memory[rs] = ~memory[rs];
                        break;
                    // sl - semi-full, rs번 변수 값에 좌측 시프트 연산을 imm번 한 결과를 구한 뒤 그 변수에 넣는다.
                    case 47:
                        memory[rs] <<= imm;
                        break;
                    // sr - semi-full, rs번 변수 값에 우측 시프트 연산을 imm번 한 결과를 구한 뒤 그 변수에 넣는다.
                    case 48:
                        memory[rs] >>= imm;
                        break;
                    // lt - full, rs번 변수 값이 imm번 변수의 값보다 작으면 rd번 변수에 1을, 아니면 0을 대입한다.
                    case 50:
                        memory[rd] = (memory[rs] < memory[imm]) ? 1 : 0;
                        break;
                    // lti - full, rs번 변수 값이 imm[3x+1]의 값보다 작으면 rd번 변수에 1을, 아니면 0을 대입한다.
                    case 51:
                        memory[rd] = (memory[rs] < IMM3x(imm)) ? 1 : 0;
                        break;
                    // lte - full, rs번 변수 값이 imm번 변수의 값보다 작거나 같으면 rd번 변수에 1을, 아니면 0을 대입한다.
                    case 52:
                        memory[rd] = (memory[rs] <= memory[imm]) ? 1 : 0;
                        break;
                    // ltei - full, rs번 변수 값이 imm[3x+1]의 값보다 작거나 같으면 rd번 변수에 1을, 아니면 0을 대입한다.
                    case 53:
                        memory[rd] = (memory[rs] <= IMM3x(imm)) ? 1 : 0;
                        break;
                    // gt - full, rs번 변수 값이 imm번 변수의 값보다 크면 rd번 변수에 1을, 아니면 0을 대입한다.
                    case 54:
                        memory[rd] = (memory[rs] > memory[imm]) ? 1 : 0;
                        break;
                    // gti - full, rs번 변수 값이 imm[3x+1]의 값보다 크면 rd번 변수에 1을, 아니면 0을 대입한다.
                    case 55:
                        memory[rd] = (memory[rs] > IMM3x(imm)) ? 1 : 0;
                        break;
                    // gte - full, 	rs번 변수 값이 imm번 변수의 값보다 크거나 같으면 rd번 변수에 1을, 아니면 0을 대입한다.
                    case 56:
                        memory[rd] = (memory[rs] >= memory[imm]) ? 1 : 0;
                        break;
                    // gtei - full, rs번 변수 값이 imm[3x+1]의 값보다 크거나 같으면 rd번 변수에 1을, 아니면 0을 대입한다.
                    case 57:
                        memory[rd] = (memory[rs] >= IMM3x(imm)) ? 1 : 0;
                        break;
                    // eql - full, rs번 변수 값과 imm번 변수의 값이 같으면 rd번 변수에 1을, 아니면 0을 대입한다.
                    case 60:
                        memory[rd] = (memory[rs] == memory[imm]) ? 1 : 0;
                        break;
                    // eqli - full, rs번 변수 값과 imm[3x+1]의 값이 같으면 rd번 변수에 1을, 아니면 0을 대입한다.
                    case 61:
                        memory[rd] = (memory[rs] == IMM3x(imm)) ? 1 : 0;
                        break;
                    // eqli1 - full, rs번 변수 값과 imm의 값이 같으면 rd번 변수에 1을, 아니면 0을 대입한다.
                    case 62:
                        memory[rd] = (memory[rs] == imm) ? 1 : 0;
                        break;
                    // neql - full, rs번 변수 값과 imm번 변수의 값이 다르면 rd번 변수에 1을, 아니면 0을 대입한다.
                    case 63:
                        memory[rd] = (memory[rs] != memory[imm]) ? 1 : 0;
                        break;
                    // neqli - full, rs번 변수 값과 imm[3x+1]의 값이 같으면 rd번 변수에 1을, 아니면 0을 대입한다.
                    case 64:
                        memory[rd] = (memory[rs] != IMM3x(imm)) ? 1 : 0;
                        break;
                    // neqli1 - full, rs번 변수 값과 imm의 값이 다르면 rd번 변수에 1을, 아니면 0을 대입한다.
                    case 65:
                        memory[rd] = (memory[rs] != imm) ? 1 : 0;
                        break;
                    // jzero - semi-full, rs번 변수에 들어 있는 값이 0이면 imm번의 레이블으로 점프한다.
                    case 70:
                        if (memory[rs] == 0) Jump(jumpLabels[imm]);
                        break;
                    // jnzero - semi-full, rs번 변수에 들어 있는 값이 0이 아니면 imm번의 레이블으로 점프한다.
                    case 71:
                        if (memory[rs] != 0) Jump(jumpLabels[imm]);
                        break;
                    // jv - full, rd번 변수에 들어 있는 값이 imm번 변수에 들어있는 값과 같으면 rs번의 레이블으로 점프한다.
                    case 72:
                        if (memory[rd] == memory[imm]) Jump(jumpLabels[rs]);
                        break;
                    // ji - full, rd번 변수에 들어 있는 값이 imm의 값과 같으면 rs번의 레이블으로 점프한다.
                    case 73:
                        if (memory[rd] == imm) Jump(jumpLabels[rs]);
                        break;
                    // j - half, 조건없이 rs번의 레이블으로 점프한다.
                    case 74:
                        Jump(jumpLabels[rs]);
                        break;
                    default:
                        throw new InvalidOperationException($"줄 {lineNum} : 유효하지 않은 명령어 코드 : {op}");
                }
            }
        }

        private static int IMM3x(int imm)
        {
            if (imm > 20) imm = 20;
            int ret = 0;
            for (int i = 0; i < imm; i++)
            {
                ret = ret * 3 + 1;
            }
            return ret;
        }

        private static bool ValidPointer()
        {
            return pointer >= 0 && pointer < memory.Length;
        }

        private static void Jump(int newPC)
        {
            PC = newPC;
        }

    }
}
