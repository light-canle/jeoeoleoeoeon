using jeoeoleoeoeon;

StreamReader sr = new(new BufferedStream(Console.OpenStandardInput()));
while (true)
{
    Console.Clear();
    Console.WriteLine("저어러어언 언어 인터프리터 v1.0");

    Console.WriteLine("");

    Console.WriteLine("메뉴를 선택하세요.");

    Console.WriteLine("1. 파일 실행");
    Console.WriteLine("2. 유틸리티");
    Console.WriteLine("3. 종료");
    try
    {
        int select = int.Parse(sr.ReadLine()!);

        if (select == 1)
        {
            Console.Clear();
            Console.WriteLine("읽을 파일의 경로를 복사해 여기에 입력해주세요. 양쪽 끝의 큰따옴표(\")는 제거해주세요.");
            string fileName = sr.ReadLine()!;
            try
            {
                Interpreter.Execute(fileName);
            }
            catch (InvalidDataException e)
            {
                Console.Write(e.Message);
            }
            Console.WriteLine("");
            Console.WriteLine("실행 종료! 아무 키나 눌러서 처음으로 돌아갑니다.");
            Console.ReadLine();
        }
        else if (select == 2)
        {
            Console.Clear();
            Console.WriteLine("원하는 기능을 선택하세요");
            Console.WriteLine("1. 특정 문자열(1줄) 출력");
            Console.WriteLine("2. 변수에 특정 값 대입");
            int select2;
            try
            {
                do
                {
                    select2 = int.Parse(sr.ReadLine()!);
                } while (select2 != 1 && select2 != 2);

                string output = "";
                int rs, value;
                switch (select2)
                {
                    case 1:
                        Console.Write("출력하고 싶은 문자열을 입력하세요. : ");
                        string toPrint = sr.ReadLine()!;
                        Console.Write("시작 변수의 rs값을 입력하세요. : ");
                        rs = int.Parse(sr.ReadLine()!);
                        if (rs < 0 || rs >= 16384) throw new IndexOutOfRangeException();
                        output = Util.MakeString(toPrint, rs);
                        break;
                    case 2:
                        Console.Write("대입할 정수를 입력하세요. : ");
                        value = int.Parse(sr.ReadLine()!);
                        Console.Write("값을 넣을 변수의 rs값을 입력하세요. : ");
                        rs = int.Parse(sr.ReadLine()!);
                        if (rs < 0 || rs >= 16384) throw new IndexOutOfRangeException();
                        output = Util.ValueAppend(value, rs);
                        break;
                    default: throw new InvalidDataException();
                }
                Console.WriteLine("요청한 명령을 수행하는 프로그램은 아래와 같습니다.(아래 내용을 복사해서 파일에 붙여 넣으세요.)");
                Console.WriteLine(output);
                Console.WriteLine("아무 키나 눌러서 처음으로 돌아갑니다.");
                Console.ReadLine();
            }
            catch
            {
                Console.WriteLine("유효하지 않은 값이 입력되어 처음으로 돌아갑니다.");
                Console.ReadLine();
            }
        }
        else if (select == 3)
        {
            break;
        }
    }
    catch
    {
        continue;
    }
}

sr.Close();