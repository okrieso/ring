using System;
using System.Collections.Generic;
namespace Task2
{

    class Program
    {

        static int NOD(int a, int b)
        {
            if (Math.Min(a, b) == 0)
                return Math.Max(a, b);
            if (Math.Max(a, b) == a)
                return NOD(a % b, b);
            return NOD(a, b % a);
        }

        static int advancedNOD(int a, int b, out int pa, out int qa)
        {
            pa = 1;
            qa = 0;
            int pb = 0;
            int qb = 1;
            if (a < b)
            {
                int t = a;
                a = b;
                b = t;
            }
            while (b != 0)
            {
                int t = a / b;
                int tpb = pb;
                pb = pa - t * pb;
                pa = tpb;
                int tqb = qb;
                qb = qa - t * qb;
                qa = tqb;
                int tb = b;
                b = a % b;
                a = tb;
            }
            return a;
        }

        class ElemRing
        {
            public int Value { private set; get; }
            public int Base { private set; get; }
            public ElemRing(int v, int b)
            {
                if (b < 2)
                    throw new ArgumentException("Основание не может быть меньше 2");
                Value = v % b;
                Base = b;
            }
            public static ElemRing operator +(ElemRing a, ElemRing b)
            {
                if (a.Base != b.Base)
                    throw new ArgumentException("Основания чисел должны быть равны");
                return new ElemRing(((a.Value + b.Value) % a.Base), a.Base);
            }

            public static ElemRing operator -(ElemRing a, ElemRing b)
            {
                if (a.Base != b.Base)
                    throw new ArgumentException("Основания чисел должны быть равны");
                return new ElemRing(((a.Value - b.Value) % a.Base), a.Base);
            }

            public static ElemRing operator *(ElemRing a, ElemRing b)
            {
                if (a.Base != b.Base)
                    throw new ArgumentException("Основания чисел должны быть равны");
                return new ElemRing(((a.Value * b.Value) % a.Base), a.Base);
            }

            public static ElemRing operator /(ElemRing a, ElemRing b)
            {
                if (a.Base != b.Base)
                    throw new ArgumentException("Основания чисел должны быть равны");
                if (advancedNOD(b.Value, b.Base, out int y, out int x) == 1)
                {
                    if (x < 0)
                        x = b.Base - Math.Abs(x);
                    return new ElemRing(((a.Value * x) % a.Base), a.Base);
                }
                else
                    //давайте сделаем вид что этого костыля здесь нет
                    return new ElemRing(0, a.Base);
                //throw new DivideByZeroException("Делитель необратим");
            }

            public override bool Equals(Object obj)
            {
                if (obj == null || !(obj is ElemRing))
                    return false;
                else
                    return Value == ((ElemRing)obj).Value;
            }

            public override int GetHashCode()
            {
                return Value;
            }

            public override string ToString()
            {
                return Value.ToString();
            }

        }

        static void findX(List<ElemRing> factors, List<ElemRing> values)
        {
            Console.WriteLine("Решим данную систему уравнений: ");
            List<List<ElemRing>> equations = new List<List<ElemRing>>();
            for (int i = 0; i < factors.Count; i++)
            {
                Console.WriteLine($"{factors[i].Value} x = {values[i].Value} (mod {factors[i].Base})");
                try
                {
                    equations.Add(equation(factors[i], values[i]));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine("Система уравнений не имеет решения");
                    return;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Получили данное множество систем уравнений");
            for (int i = 0; i < equations[0].Count; i++)
                for (int j = 0; j < equations[1].Count; j++)
                    for (int g = 0; g < equations[2].Count; g++)
                    {
                        Console.WriteLine($"x = {equations[0][i].Value} (mod {equations[0][i].Base})");
                        Console.WriteLine($"x = {equations[1][j].Value} (mod {equations[1][j].Base})");
                        Console.WriteLine($"x = {equations[2][g].Value} (mod {equations[2][g].Base})");
                        Console.WriteLine();
                    }
            int k = 1;
            for (int i = 0; i < equations[0].Count; i++)
                for (int j = 0; j < equations[1].Count; j++)
                    for (int g = 0; g < equations[2].Count; g++)
                    {
                        Console.Write($"Решение {k} уравнения: x = ");
                        ElemRing n1 = equations[0][i];
                        ElemRing n2 = equations[1][j];
                        ElemRing n3 = equations[2][g];
                        int N = n1.Base * n2.Base * n3.Base;
                        ElemRing[] nn = new ElemRing[] { n1, n2, n3 };
                        int x = 0;
                        for (int b = 0; b < equations.Count; b++)
                        {
                            int y = N / nn[b].Base * (new ElemRing(1, nn[b].Base) / new ElemRing(N / nn[b].Base, nn[b].Base)).Value;
                            x += y * nn[b].Value;
                        }
                        Console.WriteLine(new ElemRing(x, N));
                        Console.WriteLine();
                        k++;
                    }

        }

        static List<ElemRing> equation(ElemRing factor, ElemRing value)
        {
            int Base = factor.Base;
            int d = NOD(Base, factor.Value);
            if (d == 1)
                return (new List<ElemRing>() { value / factor });
            else if (value.Value % d == 0)
            {
                var t = new List<ElemRing>();
                var vt = new ElemRing(value.Value / d, Base / d);
                var ft = new ElemRing(factor.Value / d, Base / d);
                var y = vt / ft;
                for (int j = 0; j < d; j++)
                {
                    int k = (j * Base) / d;
                    t.Add(new ElemRing(y.Value + k, Base));
                }
                return t;
            }
            else
                throw new Exception("Уравнение не имеет решений");

        }


        static void Main(string[] args)
        {
            List<ElemRing> factors = new List<ElemRing>();
            List<ElemRing> values = new List<ElemRing>();
            int[] Factors = { 3, 2, 1 };
            int[] Values = { 6, 1, 38 };
            int[] Bases = { 9, 5, 44 };
            int n = 3;

            for (int i = 0; i < n; i++)
            {
                int Base = Bases[i];
                factors.Add(new ElemRing(Factors[i], Base));
                values.Add(new ElemRing(Values[i], Base));
            }
            //Снять ком. если необходимо ввести с клавиатуры
            /*
            for (int i = 0; i < n; i++)
            {
                Console.Write($"Введите {i + 1} основание: ");
                int Base = Convert.ToInt32(Console.ReadLine());
                Console.Write($"Введите {i + 1} коэффицент при x: ");
                factors.Add(new ElemRing(Convert.ToInt32(Console.ReadLine()), Base));
                Console.Write($"Введите {i + 1} значение: ");
                values.Add(new ElemRing(Convert.ToInt32(Console.ReadLine()), Base));
            }
            */
            findX(factors, values);


        }
    }
}