public class Range {
    public int start;
    public int end;
    public int inc = 1;

    public Range(int from, int to) {
        this.start = from;
        this.end = to;
        if (from <= to) {
            inc = 1;
        } else {
            inc = -1;
        }
    }

    public int next(int x) {
        return x + inc;
    }

    public bool includes(int x) {
        return (x >= start && x <= end) || (x >= end && x <= start);
    }
}
