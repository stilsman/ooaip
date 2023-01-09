namespace SpaceBattle.Lib;

public class Angle
{
    public int numerator { get; set; }
    public int denominator { get; set; }

    public Angle(int numerator, int denominator)
    {
        if (denominator == 0)
        {
            throw new Exception("ZeroDenominator");
        }

        int gcd = GCD(numerator, denominator);
        this.numerator = numerator / gcd;
        this.denominator = denominator / gcd;
    }

    private static int GCD(int a, int b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }

    public override string ToString()
    {
        return String.Format("[Rational: {0}/{1}]", this.numerator, this.denominator);
    }

    public static Angle operator +(Angle a, Angle b)
    {
        int num = a.numerator * b.denominator + b.numerator * a.denominator;
        int den = a.denominator * b.denominator;
        int nod = GCD(num, den);
        return new Angle(num / nod, den / nod);
    }

    public static bool operator ==(Angle a, Angle b)
    {
        return (a.numerator == b.numerator) && (a.denominator == b.denominator);
    }

    public static bool operator !=(Angle a, Angle b)
    {
        return !(a == b);
    }

    public override bool Equals(object? obj)
    {
        return obj is Angle a && this.numerator == a.numerator && this.denominator == a.denominator;
    }

    public override int GetHashCode()
    {
        return (this.ToString()).GetHashCode();
    }
}



