using System;

[Serializable]
public class Pair<TFirst, TSecond> {

	public TFirst first;
	public TSecond second;

	public Pair(TFirst first, TSecond second) {
		this.first = first;
		this.second = second;
	}

    public override bool Equals(object obj)
    {
        var other = (Pair<TFirst, TSecond>)obj;
        return first.Equals(other.first) && second.Equals(other.second);
    }
}


public static class Pair {

    public static Pair<TFirst, TSecond> Make<TFirst, TSecond>(TFirst first, TSecond second) {
        return new Pair<TFirst, TSecond>(first, second);
    }

}