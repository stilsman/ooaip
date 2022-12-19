namespace SpaceBattle.Lib.Test;
using System;
using Xunit;
using Moq;

public class AngleTest
{
    [Fact]
    public void Positive()
    {
        Angle a = new Angle(45, 1);
        Angle b = new Angle(90, 2);
        Assert.Equal(a,b);

    }

    [Fact]
    public void PositiveSum()
    {
        Angle a = new Angle(45, 1);
        Angle b = new Angle(90, 2);
        
        Assert.Equal(a + b, new Angle(90, 1));

    }   
    [Fact]
    public void NegativeSum()
    {
        Angle a = new Angle(45, 1);
        Angle b = new Angle(90, 2);
        Assert.False(a + b == new Angle(90, 2));

    }

    [Fact]
    public void DivizionByZeroException()
    {
        Assert.Throws<Exception>(()=> new Angle(97,0)); 

    }
    [Fact]
    public void Equal()
    {
        Angle a = new Angle(45, 1);
        Assert.True(a.Equals(new Angle(90,2)));

        int n = 1;
        Assert.False(a.Equals(n));

    }
    [Fact]
    public void Hash()
    {
        Angle a = new Angle(45, 1);
        Angle b = new Angle(90, 2);
        Assert.Equal(a.GetHashCode(),b.GetHashCode());
    }



    [Fact]
    public void notEqual()
    {
    Angle a = new Angle(45, 1);
    Angle b = new Angle(40, 1);
    Assert.True(a != b);
    }

    [Fact]
    public void DifferentType()
    {
        Angle a = new Angle(30, 1);
        Assert.False(a.Equals("String"));
    }

    
    [Fact]
    public void PositiveEquals()
    {
    Angle a = new Angle(45, 1);
    Angle b = new Angle(90, 2);
    Assert.True(a == b);
    } 
}
