using System;
namespace SpaceBattle.Lib{

public class Vector{
    public int[] vec;
    public int size = 0;
    public Vector(params int[] args){
        size = args.Length;
        vec = args;
    }
    public int this[int i]{
        get {
            return vec[i];
        }

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

    public static Vector operator * (int n, Vector v){
        int[] sum = new int[v.size];
            for (int i = 0; i < v.size; i++)
                sum[i] = v[i] * n;
            return new Vector(sum);
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

    public static bool operator < (Vector v1, Vector v2){
    if (v1.size != v2.size)
        return v1.size < v2.size;

    if (v1 == v2)
        return false;
    bool f = false;
    for (int i = 0; i < v1.size; i++){
        if (v1[i] < v2[i])
            f = true;
        else if (v1[i] > v2[i])
            return false;
    }
    return f;
    }
    public static bool operator > (Vector v1, Vector v2){
        return !(v1<v2);
    }

    public override string ToString(){  
        string s = "Vector(";
        for (int i = 0; i < size - 1; i++)
            s +=($"{vec[i]}, ");
        s += ($"{vec[size - 1]})");
        return s;
    }

    public override bool Equals(object? obj) 
    { 
        if (ReferenceEquals(obj, null)) return false;
        if (ReferenceEquals(this, obj)) return true;
        throw new NotImplementedException();
    }
    public override int GetHashCode() {return HashCode.Combine(vec);}

}

}