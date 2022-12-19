using System;
namespace SpaceBattle.Lib

public class Vector{
    public int[] vec;
    public int size = 0;
    public Vector(params int[] args){
        size = args.Length;
        vec = args;
    }
    public int this[int i]{
        get { return vec[i]; } 
        set { vec[i] = value; }

    }
    public static Vector operator + (Vector v1, Vector v2){
        if (v1.size != v2.size)
            throw new System.ArgumentException();
        else{
            int[] sum = new int[v1.size];
            for (int i = 0; i < v1.size; i++)
                sum[i] = v1[i] + v2[i];
            return new Vector(sum);
        }
    }
    public static Vector operator - (Vector v1, Vector v2){
        if (v1.size != v2.size)
            throw new System.ArgumentException();
        else{
            int[] sum = new int[v1.size];
            for (int i = 0; i < v1.size; i++)
                sum[i] = v1[i] - v2[i];
            return new Vector(sum);
        }
    }

    public static bool operator == (Vector v1, Vector v2){
        if (v1.size != v2.size)
            return false;
        bool f = true;

        for (int i = 0; i < v1.size; i++)
            if (v1[i] != v2[i]) f = false;
        return f;
    }
    public static bool operator != (Vector v1, Vector v2){
        return !(v1==v2);
    }

    public override bool Equals(object? obj){
        return obj is Vector v && vec.SequenceEqual(v.vec);
    }

    public override int GetHashCode() {
        HashCode hash = new();
        foreach(int i in vec){
            hash.Add(vec[i]);
        }
        return hash.ToHashCode();

    }

}
