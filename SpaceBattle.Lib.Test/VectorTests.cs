using System;
using Xunit;
using Moq;
namespace SpaceBattle.Lib.Test;

public class VecTests
{
    [Fact]
    public void PosSet()
    {
        Vector v1 = new Vector(1, 0);
        v1[1] = 1;
        Assert.Equal(new Vector(1, 1), v1);
    }

    [Fact]
    public void PosPlus()
    {
        Vector v1 = new Vector(1, 0);
        Vector v2 = new Vector(1, 1);
        Assert.Equal(new Vector(2, 1), v1 + v2);
    }

    [Fact]
    public void NegPlus()
    {
        Vector v1 = new Vector(1, 0,0);
        Vector v2 = new Vector(1, 1);
        Assert.Throws<ArgumentException>(() => v1 + v2);
    }

    [Fact]
    public void PosMin()
    {
        Vector v1 = new Vector(1, 0);
        Vector v2 = new Vector(1, 1);
        Assert.Equal(new Vector(0, -1), v1 - v2);
    }

    [Fact]
    public void NegMin()
    {
        Vector v1 = new Vector(1, 0,0);
        Vector v2 = new Vector(1, 1);
        Assert.Throws<ArgumentException>(() => v1 - v2);
    }

    [Fact]
    public void PosEquality()
    {
        Vector v1 = new Vector(1, 1);
        Vector v2 = new Vector(1, 1);
        Assert.True(v1==v2);

        v2+=v1;
        Assert.False(v1==v2);

        Vector v3 = new Vector(1, 0,0);
        Assert.False(v1==v3);
    }
    [Fact]
    public void nonEquality()
    {
        Vector v1 = new Vector(1, 1);
        Vector v2 = new Vector(2, 1);
        Assert.True(v1!=v2);
    }

    [Fact]
    public void PosEquals()
    {
        Vector v1 = new Vector(1, 1);
        Assert.True(v1.Equals(v1));

        int n = 1;
        Assert.False(v1.Equals(n));
    }
    [Fact]
    public void NegEquals()
    {
        Vector v1 = new Vector(1, 1);
        int n = 1;
        Assert.False(v1.Equals(n));
    }
    
    [Fact]
    public void HashCode()
    {
        Vector v1 = new Vector(1, 1);
        Vector v2 = new Vector(1, 1);
        
        Assert.Equal(v1.GetHashCode(),v2.GetHashCode());
    }
}
