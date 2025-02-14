using UnityEngine;

public static class MGData
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public class pkt
    {
        public int id;
        public string rxTime;
        public string txTime;
    }

    public class rlcQueue
    {
        public string time;
        public int queue;
    }

    public class rlcLoss
    {
        public string time;
        public int loss;
    }

    public class pdcpThroughput
    {
        public string time;
        public float throughput;
    }
}
